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
	public class CreateObjectPopup : AbstractPopup
	{
		public CreateObjectPopup(DataAccessor accessor, BaseType baseType, Guid selectedGuid)
		{
			this.accessor = accessor;
			this.baseType = baseType;

			this.controller = new NavigationTreeTableController();

			//	GuidNode -> ParentPositionNode -> LevelNode -> TreeNode
			var primaryNodesGetter = this.accessor.GetNodesGetter (this.baseType);
			this.nodesGetter = new TreeNodesGetter (this.accessor, this.baseType, primaryNodesGetter);

			this.nodesGetter.UpdateData (TreeNodeOutputMode.All);
			this.nodesGetter.CompactFinals ();

			this.visibleSelectedRow = this.nodesGetter.SearchBestIndex (selectedGuid);
			this.UpdateSelectedRow ();

			this.dataFiller = new SingleObjectsTreeTableFiller (this.accessor, this.baseType, this.nodesGetter);

			//	Connexion des événements.
			this.controller.ContentChanged += delegate (object sender, bool crop)
			{
				this.UpdateController (crop);
			};

			this.controller.RowClicked += delegate (object sender, int row)
			{
				this.visibleSelectedRow = this.controller.TopVisibleRow + row;
				this.UpdateController ();
				this.UpdateSelectedRow ();
			};
		}


		public string							ObjectName;
		public bool								ObjectGroup;
		public Guid								ObjectParent;


		protected override Size DialogSize
		{
			get
			{
				return new Size (CreateObjectPopup.PopupWidth, CreateObjectPopup.PopupHeight);
			}
		}

		protected override void CreateUI()
		{
			this.CreateTitle (this.mainFrameBox);
			this.CreateCloseButton ();

			var line1 = this.CreateFrame (CreateObjectPopup.Margins, 337, CreateObjectPopup.PopupWidth-CreateObjectPopup.Margins*2, CreateObjectPopup.LineHeight);
			var line2 = this.CreateFrame (CreateObjectPopup.Margins, 310, CreateObjectPopup.PopupWidth-CreateObjectPopup.Margins*2, CreateObjectPopup.LineHeight);
			var line3 = this.CreateFrame (CreateObjectPopup.Margins,  60, CreateObjectPopup.PopupWidth-CreateObjectPopup.Margins*2, 240);
			var line4 = this.CreateFrame (CreateObjectPopup.Margins,  20, CreateObjectPopup.PopupWidth-CreateObjectPopup.Margins*2, 24);

			this.CreateName      (line1);
			this.CreateGroup     (line2);
			this.CreateTreeTable (line3);
			this.CreateButtons   (line4);

			this.UpdateButtons ();
			this.textField.Focus ();
		}

		private void CreateTitle(Widget parent)
		{
			new StaticText
			{
				Parent           = parent,
				Text             = "Création d'un nouveal objet",
				ContentAlignment = ContentAlignment.MiddleCenter,
				Dock             = DockStyle.Top,
				PreferredHeight  = CreateObjectPopup.TitleHeight - 4,
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
				PreferredWidth   = CreateObjectPopup.Indent,
				Margins          = new Margins (0, 10, 0, 0),
			};

			var framne = new FrameBox
			{
				Parent           = parent,
				Dock             = DockStyle.Fill,
				BackColor        = ColorManager.WindowBackgroundColor,
				Padding          = new Margins (1),
			};

			this.textField = new TextField
			{
				Parent           = framne,
				Dock             = DockStyle.Fill,
			};

			this.textField.TextChanged += delegate
			{
				this.ObjectName = this.textField.Text;
				this.UpdateButtons ();
			};
		}

		private void CreateGroup(Widget parent)
		{
			new CheckButton
			{
				Parent           = parent,
				Text             = "Objet de groupement",
				AutoFocus        = false,
				Dock             = DockStyle.Fill,
				Margins          = new Margins (CreateObjectPopup.Indent + 10, 0, 0, 0),
			};
		}

		private void CreateTreeTable(Widget parent)
		{
			new StaticText
			{
				Parent           = parent,
				Text             = "Dans le groupe",
				ContentAlignment = ContentAlignment.TopRight,
				Dock             = DockStyle.Left,
				PreferredWidth   = CreateObjectPopup.Indent,
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

			this.controller.RowClicked += delegate (object sender, int row)
			{
				var node = this.nodesGetter[row];
				this.ObjectParent = node.Guid;
				this.UpdateButtons ();
			};
		}

		private void CreateButtons(Widget parent)
		{
			this.createButton = new Button
			{
				Parent        = parent,
				Name          = "create",
				Text          = "Créer",
				ButtonStyle   = ButtonStyle.Icon,
				AutoFocus     = false,
				Dock          = DockStyle.Left,
				PreferredSize = new Size (CreateObjectPopup.PopupWidth/2 - CreateObjectPopup.Margins - 5, parent.PreferredHeight),
				Margins       = new Margins (0, 5, 0, 0),
			};

			this.cancelButton = new Button
			{
				Parent        = parent,
				Name          = "cancel",
				Text          = "Annuler",
				ButtonStyle   = ButtonStyle.Icon,
				AutoFocus     = false,
				Dock          = DockStyle.Left,
				PreferredSize = new Size (CreateObjectPopup.PopupWidth/2 - CreateObjectPopup.Margins - 5, parent.PreferredHeight),
				Margins       = new Margins (5, 0, 0, 0),
			};

			this.createButton.Clicked += this.HandleButtonClicked;
			this.cancelButton.Clicked += this.HandleButtonClicked;
		}

		private void HandleButtonClicked(object sender, MessageEventArgs e)
		{
			var button = sender as Button;

			this.ClosePopup ();
			this.OnButtonClicked (button.Name);
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
		private static readonly int LineHeight  = 1+17+1;
		private static readonly int Indent      = 80;
		private static readonly int PopupWidth  = 330;
		private static readonly int PopupHeight = 400;
		private static readonly int Margins     = 20;

		private readonly DataAccessor					accessor;
		private readonly BaseType						baseType;
		private readonly NavigationTreeTableController	controller;
		private readonly TreeNodesGetter				nodesGetter;
		private readonly SingleObjectsTreeTableFiller	dataFiller;

		private TextField								textField;
		private Button									createButton;
		private Button									cancelButton;
		private int										visibleSelectedRow;
	}
}