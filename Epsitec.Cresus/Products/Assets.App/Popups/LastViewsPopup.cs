//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.DataFillers;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.NodesGetter;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Choix d'une vue dans la liste des anciennes vues.
	/// </summary>
	public class LastViewsPopup : AbstractPopup
	{
		public LastViewsPopup(DataAccessor accessor, List<AbstractViewState> viewStates, Guid selection)
		{
			this.accessor   = accessor;
			this.viewStates = viewStates;

			this.controller = new NavigationTreeTableController();

			this.nodesGetter = new LastViewsNodesGetter ();
			this.nodesGetter.SetParams (this.ViewStatesToNavigationNodes (this.viewStates));
			this.SelectedIndex = selection;

			this.dataFiller = new LastViewsTreeTableFiller (this.accessor, this.nodesGetter);

			//	Connexion des événements.
			this.controller.ContentChanged += delegate (object sender, bool crop)
			{
				this.UpdateController (crop);
			};

			this.controller.RowClicked += delegate (object sender, int row)
			{
				this.visibleSelectedRow = this.controller.TopVisibleRow + row;
				this.UpdateController ();

				this.OnNavigate (this.SelectedIndex);
				this.ClosePopup ();
			};
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
			this.CreateTitle ("Dernières vues utilisées");
			this.CreateCloseButton ();

			this.CreateController ();
		}

		private void CreateController()
		{
			var frame = new FrameBox
			{
				Parent = this.mainFrameBox,
				Dock   = DockStyle.Fill,
			};

			this.controller.CreateUI (frame, rowHeight: LastViewsPopup.rowHeight, headerHeight: LastViewsPopup.headerHeight, footerHeight: 0);
			this.controller.AllowsMovement = false;

			TreeTableFiller<LastViewNode>.FillColumns (this.controller, this.dataFiller, 0);
			this.UpdateController ();
		}


		private Guid SelectedIndex
		{
			get
			{
				var node = this.nodesGetter[this.visibleSelectedRow];
				return node.NavigationGuid;
			}
			set
			{
				this.visibleSelectedRow = this.nodesGetter.SearchIndex (value);
			}
		}


		private Size GetSize()
		{
			var parent = this.GetParent ();

			double h = parent.ActualHeight
					 - LastViewsPopup.headerHeight
					 - AbstractScroller.DefaultBreadth;

			//	Utilise au maximum les 3/4 de la hauteur.
			int max = (int) (h*0.75) / LastViewsPopup.rowHeight;

			int rows = System.Math.Min (this.nodesGetter.Count, max);
			rows = System.Math.Max (rows, 3);

			int dx = LastViewsTreeTableFiller.TotalWidth
				   + (int) AbstractScroller.DefaultBreadth;

			int dy = AbstractPopup.titleHeight
				   + LastViewsPopup.headerHeight
				   + rows * LastViewsPopup.rowHeight
				   + (int) AbstractScroller.DefaultBreadth;

			return new Size (dx, dy);
		}

		private void UpdateController(bool crop = true)
		{
			TreeTableFiller<LastViewNode>.FillContent (this.controller, this.dataFiller, this.visibleSelectedRow, crop);
		}


		private int SearchIndex(Guid navigationGuid)
		{
			for (int i=0; i<this.viewStates.Count; i++)
			{
				if (navigationGuid == this.viewStates[i].Guid)
				{
					return i;
				}
			}

			return -1;
		}

		private List<LastViewNode> ViewStatesToNavigationNodes(List<AbstractViewState> viewStates)
		{
			var nodes = new List<LastViewNode> ();

			foreach (var viewState in viewStates)
			{
				var node = viewState.GetNavigationNode (this.accessor);
				nodes.Add (node);
			}

			return nodes;
		}


		#region Events handler
		private void OnNavigate(Guid navigationGuid)
		{
			this.Navigate.Raise (this, navigationGuid);
		}

		public event EventHandler<Guid> Navigate;
		#endregion


		private const int headerHeight     = 22;
		private const int rowHeight        = 18;


		private readonly DataAccessor					accessor;
		private readonly List<AbstractViewState>		viewStates;
		private readonly NavigationTreeTableController	controller;
		private readonly LastViewsNodesGetter			nodesGetter;
		private readonly LastViewsTreeTableFiller		dataFiller;

		private int										visibleSelectedRow;
	}
}