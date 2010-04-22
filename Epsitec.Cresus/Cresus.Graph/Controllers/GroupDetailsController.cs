//	Copyright © 2009-2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
		public GroupDetailsController(WorkspaceController workspace)
		{
			this.workspace = workspace;
			this.itemsController = new ItemListController<MiniChartView> ()
			{
				ItemLayoutMode = ItemLayoutMode.Flow,
			};
		}


		public IEnumerable<MiniChartView> MiniChartViews
		{
			get
			{
				return this.itemsController;
			}
		}

		public void SetupUI(Widget container)
		{
			this.balloon = new BalloonTip ()
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
				Parent = this.balloon,
			};

			this.itemsController.SetupUI (detailsFrame);
		}

		
		public void ShowGroupDetails(GraphDataGroup group, GroupView groupView)
		{
			this.itemsController.Clear ();
			this.HideGroupDetails ();

			if ((group == null) ||
				(groupView == null))
			{
				return;
			}

			this.activeGroup = group;
			this.activeGroupView = groupView.View;

			//-			var series1 = group.SyntheticDataSeries.Cast<GraphDataSeries> ();
			//			var series2 = group.InputDataSeries;
			//			var series = series1.Concat (series2);

			foreach (var item in group.InputDataSeries)
			{
				this.itemsController.Add (this.CreateGroupDetailView (group, item));
			}

			var window = groupView.Window;

			if (window != null)
			{
				this.workspace.Groups.UpdateLayout ();
				this.itemsController.UpdateLayout ();

				double width  = 0;
				double height = WorkspaceController.DefaultViewHeight;

				switch (group.Count)
				{
					case 0:
					case 1:
						width = WorkspaceController.DefaultViewWidth;
						break;

					case 2:
						width = WorkspaceController.DefaultViewWidth * 2 - this.itemsController.OverlapX;
						break;

					default:
						width = WorkspaceController.DefaultViewWidth * 3 - this.itemsController.OverlapX * 2;

						if (group.Count > 3)
						{
							height = WorkspaceController.DefaultViewHeight * 2 - this.itemsController.OverlapY;
							width += AbstractScroller.DefaultBreadth + 2;
						}
						break;
				}

				this.itemsController.VisibleScroller = group.Count > 3;

				var clip   = groupView.MapClientToRoot (groupView.Client.Bounds);
				var bounds = Rectangle.Intersection (clip, Rectangle.Deflate (groupView.View.MapClientToRoot (groupView.View.Client.Bounds), 4, 4));
				var mark   = ButtonMarkDisposition.Below;
				var rect   = BalloonTip.GetBestPosition (new Size (width + 12, height + 12 + 10), bounds, window.ClientSize, ref mark);

				this.balloon.Margins = new Margins (rect.Left, 0, 0, rect.Bottom);
				this.balloon.PreferredSize = rect.Size;
				this.balloon.TipAttachment = bounds.Center - rect.BottomLeft;

				System.Diagnostics.Debug.Assert (this.balloon.Visibility == false);
				
				this.balloon.Visibility = true;
				this.RegisterFilter ();
			}

			this.workspace.RefreshHilites ();
		}

		public void HideGroupDetails()
		{
			if (this.balloon.Visibility)
			{
				this.activeGroup = null;
				this.activeGroupView = null;
				this.balloon.Visibility = false;
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
				var container = this.itemsController.Container;
				var pos = container.MapRootToClient (message.Cursor);

				if ((container.Client.Bounds.Contains (pos)) ||
					((this.activeGroupView != null) && (this.activeGroupView.Parent.Client.Bounds.Contains (this.activeGroupView.Parent.MapRootToClient (message.Cursor)))))
				{
					//	OK
				}
				else
				{
					if (message.MessageType == MessageType.MouseDown)
					{
						this.workspace.Groups.ClearActiveGroup ();
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
			var view = this.workspace.CreateView (item);
			var synt = item as GraphSyntheticDataSeries;

			string iconUri = "manifest:Epsitec.Common.Graph.Images.Glyph.DropItem.icon";

			if ((synt != null) &&
				(synt.SourceGroup == group))
			{
				view.DefineIconButton (ButtonVisibility.ShowOnlyWhenEntered, iconUri,
					delegate
					{
						group.RemoveSyntheticDataSeries (synt.FunctionName);
						this.workspace.Document.UpdateSyntheticSeries ();
						this.workspace.Refresh ();
					});
			}
			else
			{
				view.DefineIconButton (ButtonVisibility.ShowOnlyWhenEntered, iconUri,
					delegate
					{
						this.workspace.Groups.RemoveGroup (group, Collection.Single (item));
						this.workspace.Refresh ();
					});
			}

			return view;
		}
		

		private readonly WorkspaceController	workspace;
		private readonly ItemListController<MiniChartView> itemsController;
		private BalloonTip						balloon;
		private GraphDataGroup					activeGroup;
		private MiniChartView					activeGroupView;
	}
}
