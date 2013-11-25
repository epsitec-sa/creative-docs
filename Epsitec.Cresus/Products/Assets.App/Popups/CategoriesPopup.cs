﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	public class CategoriesPopup : AbstractPopup
	{
		public CategoriesPopup(DataAccessor accessor)
		{
			this.accessor = accessor;

			this.visibleSelectedRow = -1;

			this.controller = new NavigationTreeTableController();

			var primary      = this.accessor.GetNodesGetter (BaseType.Categories);
			var secondary    = new SortableNodesGetter (primary, this.accessor, BaseType.Categories);
			this.nodesGetter = new SorterNodesGetter (secondary);

			secondary       .SortingInstructions = SortingInstructions.Default;
			this.nodesGetter.SortingInstructions = SortingInstructions.Default;
			this.nodesGetter.UpdateData ();

			this.dataFiller = new CategoriesTreeTableFiller (this.accessor, this.nodesGetter);
		}


		protected override Size DialogSize
		{
			get
			{
				return this.GetSize ();
			}
		}

		public override void CreateUI()
		{
			this.CreateTitle ("Choix de la catégorie d'immobilisation à importer");
			this.CreateCloseButton ();

			var frame = new FrameBox
			{
				Parent = this.mainFrameBox,
				Dock   = DockStyle.Fill,
			};

			this.controller.CreateUI (frame, headerHeight: CategoriesPopup.HeaderHeight, footerHeight: 0);
			this.controller.AllowsMovement = false;

			TreeTableFiller<SortableNode>.FillColumns (this.dataFiller, this.controller, 0);
			this.UpdateController ();

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
					 - CategoriesPopup.HeaderHeight
					 - AbstractScroller.DefaultBreadth;

			//	Utilise au maximum les 3/4 de la hauteur.
			int max = (int) (h*0.75) / CategoriesPopup.RowHeight;

			int rows = System.Math.Min (this.nodesGetter.Count, max);
			rows = System.Math.Max (rows, 3);

			int dx = CategoriesPopup.PopupWidth
				   + (int) AbstractScroller.DefaultBreadth;

			int dy = CategoriesPopup.TitleHeight
				   + CategoriesPopup.HeaderHeight
				   + rows * CategoriesPopup.RowHeight
				   + (int) AbstractScroller.DefaultBreadth;

			return new Size (dx, dy);
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

			TreeTableFiller<SortableNode>.FillContent (this.dataFiller, this.controller, firstRow, count, selection);
		}


		#region Events handler
		private void OnNavigate(Guid guid)
		{
			this.Navigate.Raise (this, guid);
		}

		public event EventHandler<Guid> Navigate;
		#endregion


		private static readonly int TitleHeight      = AbstractPopup.TitleHeight;
		private static readonly int HeaderHeight     = 22;
		private static readonly int RowHeight        = 18;
		private static readonly int PopupWidth       = 390;

		private readonly DataAccessor					accessor;
		private readonly NavigationTreeTableController	controller;
		private readonly SorterNodesGetter				nodesGetter;
		private readonly CategoriesTreeTableFiller		dataFiller;

		private int										visibleSelectedRow;
	}
}