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
	/// Popup permettant la saisir des informations nécessaires à la création d'une
	/// nouvelle catégorie, à savoir le nom de la catégorie et son éventuel modèle.
	/// </summary>
	public class CreateCategoryPopup : AbstractPopup
	{
		public CreateCategoryPopup(DataAccessor accessor)
		{
			this.accessor = accessor;

			this.controller = new NavigationTreeTableController ();

			var primary      = this.accessor.GetNodesGetter (BaseType.Categories);
			var secondary    = new SortableNodesGetter (primary, this.accessor, BaseType.Categories);
			this.nodesGetter = new SorterNodesGetter (secondary);

			secondary.SetParams (null, SortingInstructions.Default);
			this.nodesGetter.SetParams (SortingInstructions.Default);

			this.visibleSelectedRow = -1;
			this.UpdateSelectedRow ();

			this.dataFiller = new SingleCategoriesTreeTableFiller (this.accessor, this.nodesGetter);
		}


		public string							ObjectName;
		public Guid								ObjectModel;


		protected override Size DialogSize
		{
			get
			{
				return new Size (CreateCategoryPopup.popupWidth, CreateCategoryPopup.popupHeight);
			}
		}

		public override void CreateUI()
		{
			this.CreateTitle ("Création d'une nouvelle catégorie");

			var line1 = this.CreateFrame (CreateCategoryPopup.margin, 327, CreateCategoryPopup.popupWidth-CreateCategoryPopup.margin*2, CreateCategoryPopup.lineHeight);
			var line2 = this.CreateFrame (CreateCategoryPopup.margin,  50, CreateCategoryPopup.popupWidth-CreateCategoryPopup.margin*2, 260);

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
				PreferredWidth   = CreateCategoryPopup.indent,
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
				Text             = "Modèle",
				ContentAlignment = ContentAlignment.TopRight,
				Dock             = DockStyle.Left,
				PreferredWidth   = CreateCategoryPopup.indent,
				Margins          = new Margins (0, 10, 0, 0),
			};

			var frame = new FrameBox
			{
				Parent           = parent,
				Dock             = DockStyle.Fill,
			};

			this.controller.CreateUI (frame, headerHeight: 0, footerHeight: 0);
			this.controller.AllowsMovement = false;

			TreeTableFiller<SortableNode>.FillColumns (this.controller, this.dataFiller);
			this.UpdateController ();

			//	Connexion des événements.
			this.controller.RowClicked += delegate (object sender, int row, int column)
			{
				this.visibleSelectedRow = this.controller.TopVisibleRow + row;

				var node = this.nodesGetter[this.visibleSelectedRow];
				this.ObjectModel = node.Guid;

				this.UpdateController ();
				this.UpdateSelectedRow ();
				this.UpdateButtons ();
			};

			this.controller.ContentChanged += delegate (object sender, bool crop)
			{
				this.UpdateController (crop);
			};
		}

		private void CreateButtons()
		{
			var footer = this.CreateFooter ();

			this.createButton = this.CreateFooterButton (footer, DockStyle.Left,  "create", "Créer");
			this.cancelButton = this.CreateFooterButton (footer, DockStyle.Right, "cancel", "Annuler");
		}


		private void UpdateController(bool crop = true)
		{
			TreeTableFiller<SortableNode>.FillContent (this.controller, this.dataFiller, this.visibleSelectedRow, crop);
		}

		private void UpdateSelectedRow()
		{
			if (this.visibleSelectedRow != -1)
			{
				var node = this.nodesGetter[this.visibleSelectedRow];
				this.ObjectModel = node.Guid;
			}
		}

		private void UpdateButtons()
		{
			this.createButton.Enable = !string.IsNullOrEmpty (this.ObjectName);
		}


		private const int lineHeight  = 2+AbstractFieldController.lineHeight+2;
		private const int indent      = 40;
		private const int popupWidth  = 310;
		private const int popupHeight = 390;
		private const int margin      = 20;

		private readonly DataAccessor						accessor;
		private readonly NavigationTreeTableController		controller;
		private readonly SorterNodesGetter					nodesGetter;
		private readonly SingleCategoriesTreeTableFiller	dataFiller;

		private TextField									textField;
		private Button										createButton;
		private Button										cancelButton;
		private int											visibleSelectedRow;
	}
}