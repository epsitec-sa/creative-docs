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
	/// Choix d'une vue dans l'historique de navigation.
	/// </summary>
	public class NavigationPopup : AbstractPopup
	{
		public NavigationPopup(DataAccessor accessor, List<AbstractViewState> viewStates, int selection)
		{
			this.accessor   = accessor;
			this.viewStates = viewStates;

			this.controller = new NavigationTreeTableController();

			this.nodesGetter = new NavigationNodesGetter ();
			this.nodesGetter.SetParams (this.ViewStatesToNavigationNodes (this.viewStates, 100));
			this.SelectedIndex = selection;

			this.dataFiller = new NavigationTreeTableFiller (this.accessor, this.nodesGetter);

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

			this.controller.CreateUI (frame, rowHeight: NavigationPopup.rowHeight, headerHeight: NavigationPopup.headerHeight, footerHeight: 0);
			this.controller.AllowsMovement = false;

			TreeTableFiller<NavigationNode>.FillColumns (this.controller, this.dataFiller, 0);
			this.UpdateController ();
		}


		private int SelectedIndex
		{
			get
			{
				var node = this.nodesGetter[this.visibleSelectedRow];
				return this.SearchIndex (node.NavigationGuid);
			}
			set
			{
				if (value >= 0 && value < this.viewStates.Count)
				{
					this.visibleSelectedRow = this.nodesGetter.SearchIndex (this.viewStates[value].Guid);
				}
			}
		}


		private Size GetSize()
		{
			var parent = this.GetParent ();

			double h = parent.ActualHeight
					 - NavigationPopup.headerHeight
					 - AbstractScroller.DefaultBreadth;

			//	Utilise au maximum les 3/4 de la hauteur.
			int max = (int) (h*0.75) / NavigationPopup.rowHeight;

			int rows = System.Math.Min (this.nodesGetter.Count, max);
			rows = System.Math.Max (rows, 3);

			int dx = NavigationTreeTableFiller.TotalWidth
				   + (int) AbstractScroller.DefaultBreadth;

			int dy = AbstractPopup.titleHeight
				   + NavigationPopup.headerHeight
				   + rows * NavigationPopup.rowHeight
				   + (int) AbstractScroller.DefaultBreadth;

			return new Size (dx, dy);
		}

		private void UpdateController(bool crop = true)
		{
			TreeTableFiller<NavigationNode>.FillContent (this.controller, this.dataFiller, this.visibleSelectedRow, crop);
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

		private List<NavigationNode> ViewStatesToNavigationNodes(List<AbstractViewState> viewStates, int max)
		{
			var nodes = new List<NavigationNode> ();

			int count = viewStates.Count;
			int first = System.Math.Max(count-max, 0);

			for (int i=first; i<count; i++)
			{
				var viewState = viewStates[i];

				var node = viewState.GetNavigationNode (this.accessor);
				nodes.Add (node);
			}

			return nodes;
		}


		#region Events handler
		private void OnNavigate(int index)
		{
			this.Navigate.Raise (this, index);
		}

		public event EventHandler<int> Navigate;
		#endregion


		private const int headerHeight     = 22;
		private const int rowHeight        = 18;


		private readonly DataAccessor					accessor;
		private readonly List<AbstractViewState>		viewStates;
		private readonly NavigationTreeTableController	controller;
		private readonly NavigationNodesGetter			nodesGetter;
		private readonly NavigationTreeTableFiller		dataFiller;

		private int										visibleSelectedRow;
	}
}