//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class CategoriesPopup : AbstractPopup
	{
		private CategoriesPopup(DataAccessor accessor)
		{
			this.accessor = accessor;

			this.visibleSelectedRow = -1;

			this.controller = new NavigationTreeTableController(this.accessor);

			var primary     = this.accessor.GetNodeGetter (BaseType.Categories);
			var secondary   = new SortableNodeGetter (primary, this.accessor, BaseType.Categories);
			this.nodeGetter = new SorterNodeGetter (secondary);

			secondary.SetParams (null, SortingInstructions.Default);
			this.nodeGetter.SetParams (SortingInstructions.Default);

			this.dataFiller = new CategoriesTreeTableFiller (this.accessor, this.nodeGetter);
		}


		protected override Size DialogSize
		{
			get
			{
				return this.GetSize ();
			}
		}

		protected override void CreateUI()
		{
			this.CreateTitle (Res.Strings.Popup.Categories.Title.ToString ());
			this.CreateCloseButton ();

			var frame = new FrameBox
			{
				Parent = this.mainFrameBox,
				Dock   = DockStyle.Fill,
			};

			this.controller.CreateUI (frame, headerHeight: CategoriesPopup.headerHeight, footerHeight: 0);
			this.controller.AllowsMovement = false;
			this.controller.AllowsSorting  = false;

			TreeTableFiller<SortableNode>.FillColumns (this.controller, this.dataFiller, "Popup.Categories");
			this.UpdateController ();

			//	Connexion des événements.
			this.controller.ContentChanged += delegate (object sender, bool crop)
			{
				this.UpdateController (crop);
			};

			this.controller.RowClicked += delegate (object sender, int row, int column)
			{
				this.visibleSelectedRow = this.controller.TopVisibleRow + row;
				this.UpdateController ();

				var node = this.nodeGetter[this.visibleSelectedRow];
				this.OnNavigate (node.Guid);
				this.ClosePopup ();
			};
		}


		private Size GetSize()
		{
			var parent = this.GetParent ();

			//	On calcule une hauteur adaptée au contenu, mais qui ne dépasse
			//	évidement pas la hauteur de la fenêtre principale.
			double h = parent.ActualHeight
					 - CategoriesPopup.headerHeight
					 - AbstractScroller.DefaultBreadth;

			//	Utilise au maximum les 3/4 de la hauteur.
			int max = (int) (h*0.75) / CategoriesPopup.rowHeight;

			int rows = System.Math.Min (this.nodeGetter.Count, max);
			rows = System.Math.Max (rows, 3);

			int dx = CategoriesPopup.popupWidth
				   + (int) AbstractScroller.DefaultBreadth;

			int dy = AbstractPopup.titleHeight
				   + CategoriesPopup.headerHeight
				   + rows * CategoriesPopup.rowHeight
				   + (int) AbstractScroller.DefaultBreadth;

			return new Size (dx, dy);
		}

		private void UpdateController(bool crop = true)
		{
			TreeTableFiller<SortableNode>.FillContent (this.controller, this.dataFiller, this.visibleSelectedRow, crop);
		}


		#region Events handler
		private void OnNavigate(Guid guid)
		{
			this.Navigate.Raise (this, guid);
		}

		private event EventHandler<Guid> Navigate;
		#endregion


		#region Helpers
		public static void Show(Widget target, DataAccessor accessor, System.Action<Guid> action)
		{
			//	Affiche le Popup pour choisir une catégorie.
			var popup = new CategoriesPopup (accessor);

			popup.Create (target, leftOrRight: true);

			popup.Navigate += delegate (object sender, Guid guid)
			{
				action (guid);
			};
		}
		#endregion


		private const int headerHeight     = 22;
		private const int rowHeight        = 18;
		private const int popupWidth       = 490;

		private readonly DataAccessor					accessor;
		private readonly NavigationTreeTableController	controller;
		private readonly SorterNodeGetter				nodeGetter;
		private readonly CategoriesTreeTableFiller		dataFiller;

		private int										visibleSelectedRow;
	}
}