//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.NodesGetter;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class CreateGroupPopup : AbstractPopup
	{
		public CreateGroupPopup(DataAccessor accessor, Guid selectedGuid)
		{
			this.accessor = accessor;

			this.controller = new NavigationTreeTableController();

			//	GuidNode -> ParentPositionNode -> LevelNode -> TreeNode
			var primaryNodesGetter = this.accessor.GetNodesGetter (BaseType.Groups);
			this.nodesGetter = new GroupTreeNodesGetter (this.accessor, primaryNodesGetter);

			this.nodesGetter.SortingInstructions = SortingInstructions.Default;
			this.nodesGetter.UpdateData ();

			this.visibleSelectedRow = this.nodesGetter.Nodes.ToList ().FindIndex (x => x.Guid == selectedGuid);
			this.UpdateSelectedRow ();

			this.dataFiller = new SingleGroupsTreeTableFiller (this.accessor, this.nodesGetter);
		}


		public string							ObjectName;
		public Guid								ObjectParent;


		protected override Size DialogSize
		{
			get
			{
				return new Size (CreateGroupPopup.PopupWidth, CreateGroupPopup.PopupHeight);
			}
		}

		public override void CreateUI()
		{
			this.CreateTitle ("Création d'un nouveau groupe");

			var line1 = this.CreateFrame (CreateGroupPopup.Margin, 327, CreateGroupPopup.PopupWidth-CreateGroupPopup.Margin*2, CreateGroupPopup.LineHeight);
			var line2 = this.CreateFrame (CreateGroupPopup.Margin,  50, CreateGroupPopup.PopupWidth-CreateGroupPopup.Margin*2, 260);

			this.CreateName      (line1);
			this.CreateTreeTable (line2);
			this.CreateButtons   ();

			this.UpdateButtons ();
			this.textField.Focus ();
		}

		private void CreateTitle(Widget parent)
		{
			new StaticText
			{
				Parent           = parent,
				Text             = "Création d'un nouveau groupe",
				ContentAlignment = ContentAlignment.MiddleCenter,
				Dock             = DockStyle.Top,
				PreferredHeight  = CreateGroupPopup.TitleHeight - 4,
				BackColor        = ColorManager.SelectionColor,
			};

			new StaticText
			{
				Parent           = parent,
				Dock             = DockStyle.Top,
				PreferredHeight  = 4,
				BackColor        = ColorManager.SelectionColor,
			};
		}

		private void CreateName(Widget parent)
		{
			new StaticText
			{
				Parent           = parent,
				Text             = "Nom",
				ContentAlignment = ContentAlignment.MiddleRight,
				Dock             = DockStyle.Left,
				PreferredWidth   = CreateGroupPopup.Indent,
				Margins          = new Margins (0, 10, 0, 0),
			};

			var frame = new FrameBox
			{
				Parent           = parent,
				Dock             = DockStyle.Fill,
				BackColor        = ColorManager.WindowBackgroundColor,
				Padding          = new Margins (2),
			};

			this.textField = new TextField
			{
				Parent           = frame,
				Dock             = DockStyle.Fill,
			};

			this.textField.TextChanged += delegate
			{
				this.ObjectName = this.textField.Text;
				this.UpdateButtons ();
			};
		}

		private void CreateTreeTable(Widget parent)
		{
			new StaticText
			{
				Parent           = parent,
				Text             = "Dans",
				ContentAlignment = ContentAlignment.TopRight,
				Dock             = DockStyle.Left,
				PreferredWidth   = CreateGroupPopup.Indent,
				Margins          = new Margins (0, 10, 0, 0),
			};

			var frame = new FrameBox
			{
				Parent           = parent,
				Dock             = DockStyle.Fill,
			};

			this.controller.CreateUI (frame, headerHeight: 0, footerHeight: 0);
			this.controller.AllowsMovement = false;

			TreeTableFiller<TreeNode>.FillColumns (this.dataFiller, this.controller);
			this.UpdateController ();

			//	Connexion des événements.
			this.controller.RowClicked += delegate (object sender, int row)
			{
				this.visibleSelectedRow = this.controller.TopVisibleRow + row;

				var node = this.nodesGetter[this.visibleSelectedRow];
				this.ObjectParent = node.Guid;

				this.UpdateController ();
				this.UpdateSelectedRow ();
				this.UpdateButtons ();
			};

			this.controller.ContentChanged += delegate (object sender, bool crop)
			{
				this.UpdateController (crop);
			};

			this.controller.TreeButtonClicked += delegate (object sender, int row, NodeType type)
			{
				this.OnCompactOrExpand (this.controller.TopVisibleRow + row);
			};
		}

		private void CreateButtons()
		{
			var footer = this.CreateFooter ();

			this.createButton = this.CreateFooterButton (footer, DockStyle.Left,  "create", "Créer");
			this.cancelButton = this.CreateFooterButton (footer, DockStyle.Right, "cancel", "Annuler");
		}


		private void OnCompactOrExpand(int row)
		{
			//	Etend ou compacte une ligne (inverse son mode actuel).
			var guid = this.SelectedGuid;

			this.nodesGetter.CompactOrExpand (row);
			this.UpdateController ();

			this.SelectedGuid = guid;
		}

		private Guid SelectedGuid
		{
			//	Retourne le Guid de l'objet actuellement sélectionné.
			get
			{
				int sel = this.visibleSelectedRow;
				if (sel != -1 && sel < this.nodesGetter.Count)
				{
					return this.nodesGetter[sel].Guid;
				}
				else
				{
					return Guid.Empty;
				}
			}
			//	Sélectionne l'objet ayant un Guid donné. Si la ligne correspondante
			//	est cachée, on est assez malin pour sélectionner la prochaine ligne
			//	visible, vers le haut.
			set
			{
				this.visibleSelectedRow = this.nodesGetter.SearchBestIndex (value);
				this.UpdateController ();
			}
		}


		private void UpdateController(bool crop = true)
		{
			this.controller.RowsCount = this.nodesGetter.Count;

			int visibleCount = this.controller.VisibleRowsCount;
			int rowsCount    = this.controller.RowsCount;
			int count        = System.Math.Min (visibleCount, rowsCount);
			int firstRow     = this.controller.TopVisibleRow;
			int selection    = this.visibleSelectedRow;

			if (selection != -1)
			{
				//	La sélection ne peut pas dépasser le nombre maximal de lignes.
				selection = System.Math.Min (selection, rowsCount-1);

				//	Si la sélection est hors de la zone visible, on choisit un autre cadrage.
				if (crop && (selection < firstRow || selection >= firstRow+count))
				{
					firstRow = this.controller.GetTopVisibleRow (selection);
				}

				if (this.controller.TopVisibleRow != firstRow)
				{
					this.controller.TopVisibleRow = firstRow;
				}

				selection -= this.controller.TopVisibleRow;
			}

			TreeTableFiller<TreeNode>.FillContent (this.dataFiller, this.controller, firstRow, count, selection);
		}

		private void UpdateSelectedRow()
		{
			if (this.visibleSelectedRow != -1)
			{
				var node = this.nodesGetter[this.visibleSelectedRow];
				this.ObjectParent = node.Guid;
			}
		}

		private void UpdateButtons()
		{
			this.createButton.Enable = !string.IsNullOrEmpty (this.ObjectName) &&
									   !this.ObjectParent.IsEmpty;
		}


		private static readonly int TitleHeight = 24;
		private static readonly int LineHeight  = 2+17+2;
		private static readonly int Indent      = 40;
		private static readonly int PopupWidth  = 310;
		private static readonly int PopupHeight = 390;
		private static readonly int Margin      = 20;

		private readonly DataAccessor					accessor;
		private readonly NavigationTreeTableController	controller;
		private readonly GroupTreeNodesGetter				nodesGetter;
		private readonly SingleGroupsTreeTableFiller	dataFiller;

		private TextField								textField;
		private Button									createButton;
		private Button									cancelButton;
		private int										visibleSelectedRow;
	}
}