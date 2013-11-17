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

			this.nodesGetter.UpdateData (TreeNodeOutputMode.OnlyDescendants);
			this.visibleSelectedRow = this.nodesGetter.Nodes.ToList ().FindIndex (x => x.Guid == selectedGuid);

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

				var node = this.nodesGetter[this.visibleSelectedRow];
				this.ObjectParent = node.Guid;
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

			var line1 = this.CreateFrame (CreateObjectPopup.Margins, 340, CreateObjectPopup.PopupWidth-CreateObjectPopup.Margins*2, CreateObjectPopup.LineHeight);
			var line2 = this.CreateFrame (CreateObjectPopup.Margins, 310, CreateObjectPopup.PopupWidth-CreateObjectPopup.Margins*2, CreateObjectPopup.LineHeight);
			var line3 = this.CreateFrame (CreateObjectPopup.Margins,  60, CreateObjectPopup.PopupWidth-CreateObjectPopup.Margins*2, 240);
			var line4 = this.CreateFrame (CreateObjectPopup.Margins,  20, CreateObjectPopup.PopupWidth-CreateObjectPopup.Margins*2, 24);

			this.CreateName      (line1);
			this.CreateGroup     (line2);
			this.CreateTreeTable (line3);
			this.CreateButtons   (line4);

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
			parent.BackColor = ColorManager.WindowBackgroundColor;
			parent.Padding   = new Margins (2);

			new StaticText
			{
				Parent           = parent,
				Text             = "Nom",
				ContentAlignment = ContentAlignment.MiddleRight,
				Dock             = DockStyle.Left,
				PreferredWidth   = CreateObjectPopup.Indent,
				Margins          = new Margins (0, 10, 0, 0),
			};

			this.textField = new TextField
			{
				Parent           = parent,
				Dock             = DockStyle.Fill,
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
				Text             = "Parent",
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
		}

		private void CreateButtons(Widget parent)
		{
			var ok = new Button
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

			var cancel = new Button
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

			ok.Clicked += delegate
			{
				this.ClosePopup ();
				this.OnButtonClicked (ok.Name);
			};

			cancel.Clicked += delegate
			{
				this.ClosePopup ();
				this.OnButtonClicked (cancel.Name);
			};
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


		private static readonly int TitleHeight      = 24;
		private static readonly int LineHeight       = 2+17+2;
		private static readonly int Indent           = 50;
		private static readonly int PopupWidth       = 300;
		private static readonly int PopupHeight      = 400;
		private static readonly int Margins          = 20;

		private readonly DataAccessor					accessor;
		private readonly BaseType						baseType;
		private readonly NavigationTreeTableController	controller;
		private readonly TreeNodesGetter				nodesGetter;
		private readonly SingleObjectsTreeTableFiller	dataFiller;

		private TextField								textField;
		private int										visibleSelectedRow;
	}
}