//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Graph;
using Epsitec.Common.Graph.Data;
using Epsitec.Common.Graph.Renderers;
using Epsitec.Common.Graph.Styles;
using Epsitec.Common.Graph.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Graph.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Graph.Controllers
{
	internal sealed class GroupDetailsController
	{
		public GroupDetailsController(WorkspaceController workspaceController, ItemListController<Widget> groupItemsController)
		{
			this.workspaceController = workspaceController;
			this.groupItemsController = groupItemsController;
			this.groupDetailItemsController = new ItemListController<MiniChartView> ()
			{
				ItemLayoutMode = ItemLayoutMode.Flow,
			};
		}


		public IEnumerable<MiniChartView> MiniChartViews
		{
			get
			{
				return this.groupDetailItemsController;
			}
		}

		public void SetupUI(Widget container)
		{
			this.groupDetailsBalloon = new BalloonTip ()
			{
				Anchor = AnchorStyles.BottomLeft,
				Disposition = ButtonMarkDisposition.Below,
				Padding = new Margins (5, 5, 17, 8),
				Visibility = false,
				Parent = container.Window.Root,
				BackColor = Color.FromBrightness (1),
			};

			var detailsFrame = new FrameBox ()
			{
				Dock = DockStyle.Fill,
				Parent = this.groupDetailsBalloon,
			};

			this.groupDetailItemsController.SetupUI (detailsFrame);
		}

		
		public void ShowGroupDetails(GraphDataGroup group, MiniChartView view)
		{
			this.groupDetailItemsController.Clear ();
			this.CloseGroupDetails ();

			if (group == null)
			{
				return;
			}

			this.activeGroup = group;
			this.activeGroupView = view;

			//-			var series1 = group.SyntheticDataSeries.Cast<GraphDataSeries> ();
			//			var series2 = group.InputDataSeries;
			//			var series = series1.Concat (series2);

			foreach (var item in group.InputDataSeries)
			{
				this.groupDetailItemsController.Add (this.CreateGroupDetailView (group, item));
			}

			var window = view.Window;

			if (window != null)
			{
				this.groupItemsController.UpdateLayout ();
				this.groupDetailItemsController.UpdateLayout ();

				double width  = 0;
				double height = WorkspaceController.DefaultViewHeight;

				switch (group.Count)
				{
					case 0:
					case 1:
						width = WorkspaceController.DefaultViewWidth;
						break;

					case 2:
						width = WorkspaceController.DefaultViewWidth * 2 - this.groupDetailItemsController.OverlapX;
						break;

					default:
						width = WorkspaceController.DefaultViewWidth * 3 - this.groupDetailItemsController.OverlapX * 2;

						if (group.Count > 3)
						{
							height = WorkspaceController.DefaultViewHeight * 2 - this.groupDetailItemsController.OverlapY;
							width += AbstractScroller.DefaultBreadth + 2;
						}
						break;
				}

				this.groupDetailItemsController.VisibleScroller = group.Count > 3;

				var clip   = view.Parent.MapClientToRoot (view.Parent.Client.Bounds);
				var bounds = Rectangle.Intersection (clip, Rectangle.Deflate (view.MapClientToRoot (view.Client.Bounds), 4, 4));
				var mark   = ButtonMarkDisposition.Below;
				var rect   = BalloonTip.GetBestPosition (new Size (width + 12, height + 12 + 10), bounds, window.ClientSize, ref mark);

				this.groupDetailsBalloon.Margins = new Margins (rect.Left, 0, 0, rect.Bottom);
				this.groupDetailsBalloon.PreferredSize = rect.Size;
				this.groupDetailsBalloon.TipAttachment = bounds.Center - rect.BottomLeft;
				this.ShowGroupDetails ();
			}

			this.workspaceController.RefreshHilites ();
		}

		private void ShowGroupDetails()
		{
			if (this.groupDetailsBalloon.Visibility == false)
			{
				this.groupDetailsBalloon.Visibility = true;
				this.RegisterFilter ();
			}
		}

		private void CloseGroupDetails()
		{
			if (this.groupDetailsBalloon.Visibility)
			{
				this.activeGroup = null;
				this.activeGroupView = null;
				this.groupDetailsBalloon.Visibility = false;
				this.UnregisterFilter ();
			}
		}

		private void RegisterFilter()
		{
			Window.ApplicationDeactivated += this.HandleApplicationDeactivated;
			Window.MessageFilter          += this.MessageFilter;
		}

		private void UnregisterFilter()
		{
			Window.ApplicationDeactivated -= this.HandleApplicationDeactivated;
			Window.MessageFilter          -= this.MessageFilter;
		}

		private void MessageFilter(object sender, Message message)
		{
			if (message.IsMouseType)
			{
				var container = this.groupDetailItemsController.Container;
				var pos = container.MapRootToClient (message.Cursor);

				if (container.Client.Bounds.Contains (pos))
				{
					//	OK
				}
				else
				{
					if (message.MessageType == MessageType.MouseDown)
					{
						this.CloseGroupDetails ();
					}
				}
			}
		}

		private void HandleApplicationDeactivated(object sender)
		{
			//-			this.CloseGroupDetails ();
		}

		private MiniChartView CreateGroupDetailView(GraphDataGroup group, GraphDataSeries item)
		{
			var view = this.workspaceController.CreateView (item);
			var synt = item as GraphSyntheticDataSeries;

			string iconName = "manifest:Epsitec.Common.Graph.Images.Glyph.DropItem.icon";

			if ((synt != null) &&
				(synt.SourceGroup == group))
			{
				view.DefineIconButton (ButtonVisibility.ShowOnlyWhenEntered, iconName,
					delegate
					{
						group.RemoveSyntheticDataSeries (synt.FunctionName);
						this.workspaceController.Document.UpdateSyntheticSeries ();
						this.workspaceController.Refresh ();
					});
			}
			else
			{
				view.DefineIconButton (ButtonVisibility.ShowOnlyWhenEntered, iconName,
					delegate
					{
						group.Remove (item);
						this.workspaceController.UpdateGroupName (group);
					});
			}

			return view;
		}
		

		private readonly GraphApplication application;
		private readonly WorkspaceController workspaceController;
		private readonly ItemListController<Widget> groupItemsController;
		private readonly ItemListController<MiniChartView>		groupDetailItemsController;
		private BalloonTip				groupDetailsBalloon;
		private GraphDataGroup activeGroup;
		private MiniChartView activeGroupView;
	}
}
