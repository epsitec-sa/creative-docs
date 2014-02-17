//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Popup permettant la saisir des informations nécessaires à la création d'un
	/// nouveau groupe, à savoir le nom du groupe et son parent.
	/// </summary>
	public class CreateGroupPopup : AbstractPopup
	{
		public CreateGroupPopup(DataAccessor accessor, Guid selectedGuid)
		{
			this.accessor = accessor;

			this.controller = new NavigationTreeTableController();

			//	GuidNode -> ParentPositionNode -> LevelNode -> TreeNode
			var primaryNodeGetter = this.accessor.GetNodeGetter (BaseType.Groups);
			this.nodeGetter = new GroupTreeNodeGetter (this.accessor, primaryNodeGetter);
			this.nodeGetter.SetParams (null,SortingInstructions.Default );

			this.visibleSelectedRow = this.nodeGetter.Nodes.ToList ().FindIndex (x => x.Guid == selectedGuid);
			this.UpdateSelectedRow ();

			this.dataFiller = new SingleGroupsTreeTableFiller (this.accessor, this.nodeGetter);
		}


		public string							ObjectName;
		public Guid								ObjectParent;


		protected override Size DialogSize
		{
			get
			{
				return new Size (CreateGroupPopup.popupWidth, CreateGroupPopup.popupHeight);
			}
		}

		public override void CreateUI()
		{
			this.CreateTitle ("Création d'un nouveau groupe");

			var line1 = this.CreateFrame (CreateGroupPopup.margin, 327, CreateGroupPopup.popupWidth-CreateGroupPopup.margin*2, CreateGroupPopup.lineHeight);
			var line2 = this.CreateFrame (CreateGroupPopup.margin,  50, CreateGroupPopup.popupWidth-CreateGroupPopup.margin*2, 260);

			this.CreateName      (line1);
			this.CreateTreeTable (line2);
			this.CreateButtons   ();

			this.UpdateButtons ();
			this.textField.Focus ();
		}

		private void CreateName(Widget parent)
		{
			new StaticText
			{
				Parent           = parent,
				Text             = "Nom",
				ContentAlignment = ContentAlignment.MiddleRight,
				Dock             = DockStyle.Left,
				PreferredWidth   = CreateGroupPopup.indent,
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
				PreferredWidth   = CreateGroupPopup.indent,
				Margins          = new Margins (0, 10, 0, 0),
			};

			var frame = new FrameBox
			{
				Parent           = parent,
				Dock             = DockStyle.Fill,
			};

			this.controller.CreateUI (frame, headerHeight: 0, footerHeight: 0);
			this.controller.AllowsMovement = false;

			TreeTableFiller<TreeNode>.FillColumns (this.controller, this.dataFiller);
			this.UpdateController ();

			//	Connexion des événements.
			this.controller.RowClicked += delegate (object sender, int row, int column)
			{
				this.visibleSelectedRow = this.controller.TopVisibleRow + row;

				var node = this.nodeGetter[this.visibleSelectedRow];
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

			this.nodeGetter.CompactOrExpand (row);
			this.UpdateController ();

			this.SelectedGuid = guid;
		}

		private Guid SelectedGuid
		{
			//	Retourne le Guid de l'objet actuellement sélectionné.
			get
			{
				int sel = this.visibleSelectedRow;
				if (sel != -1 && sel < this.nodeGetter.Count)
				{
					return this.nodeGetter[sel].Guid;
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
				this.visibleSelectedRow = this.nodeGetter.SearchBestIndex (value);
				this.UpdateController ();
			}
		}


		private void UpdateController(bool crop = true)
		{
			TreeTableFiller<TreeNode>.FillContent (this.controller, this.dataFiller, this.visibleSelectedRow, crop);
		}

		private void UpdateSelectedRow()
		{
			if (this.visibleSelectedRow != -1)
			{
				var node = this.nodeGetter[this.visibleSelectedRow];
				this.ObjectParent = node.Guid;
			}
		}

		private void UpdateButtons()
		{
			this.createButton.Enable = !string.IsNullOrEmpty (this.ObjectName) &&
									   !this.ObjectParent.IsEmpty;
		}


		private const int lineHeight  = 2+AbstractFieldController.lineHeight+2;
		private const int indent      = 40;
		private const int popupWidth  = 310;
		private const int popupHeight = 390;
		private const int margin      = 20;

		private readonly DataAccessor					accessor;
		private readonly NavigationTreeTableController	controller;
		private readonly GroupTreeNodeGetter			nodeGetter;
		private readonly SingleGroupsTreeTableFiller	dataFiller;

		private TextField								textField;
		private Button									createButton;
		private Button									cancelButton;
		private int										visibleSelectedRow;
	}
}