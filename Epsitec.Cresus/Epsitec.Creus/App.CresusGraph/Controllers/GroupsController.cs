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
	internal sealed class GroupsController
	{
		public GroupsController(WorkspaceController workspace)
		{
			this.workspace = workspace;

			this.itemsController = new ItemListController<GroupView> ()
			{
				Anchor = AnchorStyles.LeftAndRight | AnchorStyles.Top,
				ItemLayoutMode = ItemLayoutMode.Vertical,
				OverlapY = 0,
			};

			this.detailsController = new GroupDetailsController (this.workspace);
		}


		public Widget							Container
		{
			get
			{
				return this.container;
			}
		}

		public int								Count
		{
			get
			{
				return this.itemsController.Count;
			}
		}

		public IEnumerable<MiniChartView>		GroupViews
		{
			get
			{
				return this.itemsController.Select (widget => widget.View);
			}
		}

		public IEnumerable<MiniChartView>		DetailViews
		{
			get
			{
				return this.detailsController.MiniChartViews;
			}
		}


		public void SetupUI(Widget container)
		{
			this.container = container;
			this.itemsController.SetupUI (container);
			this.detailsController.SetupUI (container);
		}


		public void UpdateLayout()
		{
			this.itemsController.UpdateLayout ();
		}

		public void Refresh()
		{
			this.itemsController.Clear ();

			this.activeGroupView = null;

			foreach (var group in this.workspace.Document.Groups)
			{
				var view = this.CreateGroupView (group);

				if (group == this.activeGroup)
				{
					this.SetActiveGroup (group, view);
				}

				this.itemsController.Add (view);
			}

			if (this.activeGroupView == null)
			{
				this.ClearActiveGroup ();
			}

			this.detailsController.ShowGroupDetails (this.activeGroup, this.activeGroupView);
			this.workspace.RefreshHints ();
		}

		public void SetActiveGroup(GraphDataGroup group, GroupView view)
		{
			this.ClearActiveGroup ();
			
			this.activeGroup = group;
			this.activeGroupView = view;
			this.activeGroupView.SetSelected (true);
		}

		public void ClearActiveGroup()
		{
			if (this.activeGroupView != null)
			{
				this.activeGroupView.SetSelected (false);
				this.activeGroupView = null;
			}

			if (this.activeGroup != null)
			{
				this.activeGroup = null;
				this.detailsController.HideGroupDetails ();
			}
		}

		
		public GraphDataGroup CreateGroup(IEnumerable<GraphDataSeries> series)
		{
			var group = this.workspace.Document.AddGroup (series);

			this.itemsController.Add (this.CreateGroupView (group));
			this.UpdateGroupName (group);

			this.ClearActiveGroup ();

			group.AddSyntheticDataSeries (Functions.FunctionFactory.FunctionSum);

			return group;
		}

		public GraphDataGroup UpdateGroup(GraphDataGroup group, IEnumerable<GraphDataSeries> items)
		{
			foreach (var item in items)
			{
				if (!group.Contains (item))
				{
					group.Add (item);
				}
			}

			this.UpdateGroupName (group);

			return group;
		}

		public GraphDataGroup RemoveGroup(GraphDataGroup group, IEnumerable<GraphDataSeries> items)
		{
			items.ForEach (item => group.Remove (item));
			
			this.UpdateGroupName (group);
			
			return group;
		}

		public void DeleteGroup(GraphDataGroup group, MiniChartView view)
		{
			this.workspace.Document.RemoveGroup (group);
			var groupView = this.itemsController.Find (x => x.View == view);
			this.itemsController.Remove (groupView);
		}

		public void DetectGroup(Point mouse, System.Action<int, bool> setGroup, System.Action<double, double> setGeometry)
		{
			if (this.itemsController.Count == 0)
			{
				setGroup (0, false);
				setGeometry (0.0, 0.0);
				return;
			}

			var drop = this.itemsController.Where (x => x.HitTest (mouse)).FirstOrDefault ();

			if (drop != null)
			{
				double y = drop.Parent.ActualHeight - drop.ActualBounds.Top;
				setGroup (drop.Index, true);
				setGeometry (y, drop.ActualHeight);
				return;
			}
			
			var dist = this.itemsController.Select (x => new
			{
				Distance = mouse.Y - x.ActualBounds.Center.Y,
				View = x
			});

			var best = dist.Where (x => x.Distance >= 0).OrderBy (x => System.Math.Abs (x.Distance)).FirstOrDefault ();

			if (best != null)
			{
				double y = best.View.Parent.ActualHeight - best.View.ActualBounds.Top - 2;
				setGroup (best.View.Index, false);
				setGeometry (y, 2.0);
			}
			else
			{
				double y = this.itemsController.Container.ActualHeight - this.itemsController.Last ().ActualLocation.Y - 2; 
				setGroup (this.itemsController.Count, false);
				setGeometry (y, 2.0);
			}
		}

		
		private void UpdateGroupName(GraphDataGroup group)
		{
			int count = group.Count;

			group.Name = string.Format (count > 1 ? "{0} éléments" : "{0} élément", count);

			this.Refresh ();
		}

		private GroupView CreateGroupView(GraphDataGroup group)
		{
			var view = this.workspace.CreateView (group);

			var groupView = new GroupView ()
			{
				PreferredHeight = view.PreferredHeight + 8,
				Padding = new Margins (0, 0, 4, 4),
				View = view,
			};

			this.CreateFunctionButton (group, groupView.ButtonSurface, null, "Groupe");

			foreach (string functionName in Functions.FunctionFactory.GetFunctionNames ())
			{
				this.CreateFunctionButton (group, groupView.ButtonSurface, functionName, Functions.FunctionFactory.GetFunctionCaption (functionName));
			}
			
			ViewDragDropManager drag = null;
			
			view.Pressed +=
				(sender, e) =>
				{
					drag = new ViewDragDropManager (this.workspace, view, view.MapClientToScreen (e.Point))
					{
						Group = group,
					};

					drag.DefineMouseMoveBehaviour (MouseCursor.AsHand,
						delegate
						{
							this.ClearActiveGroup ();
						});
					e.Message.Captured = true;
				};

			view.Released +=
				(sender, e) =>
				{
					if (drag.ProcessDragEnd () == false)
					{
						this.HandleGroupViewClicked (group, groupView);
					}
				};
			
			string iconUri = "manifest:Epsitec.Common.Graph.Images.Glyph.DropItem.icon";

			view.DefineIconButton (ButtonVisibility.ShowOnlyWhenEntered, iconUri,
				delegate
				{
					//	TODO: make this fool proof (deleting a group which is still used is fatal)
					this.DeleteGroup (group, view);
					this.ClearActiveGroup ();
					this.Refresh ();
				});

			return groupView;
		}

		private void HandleGroupViewClicked(GraphDataGroup group, GroupView view)
		{
			if (this.activeGroup == group)
			{
				this.ClearActiveGroup ();
				this.detailsController.HideGroupDetails ();
			}
			else
			{
				this.SetActiveGroup (group, view);
				
				this.workspace.RefreshInputViewSelection ();
				this.detailsController.ShowGroupDetails (this.activeGroup, this.activeGroupView);
			}
		}

		private void CreateFunctionButton(GraphDataGroup group, Widget container, string function, string label)
		{
			var button = new RadioButton ()
			{
				PreferredHeight = 15,
				Dock = DockStyle.Stacked,
				Parent = container,
				Name = function ?? "none",
				Text = label,
				ActiveState = group.DefaultFunctionName == function ? ActiveState.Yes : ActiveState.No,
			};
			
			button.ActiveStateChanged +=
				delegate
				{
					if (button.ActiveState == ActiveState.Yes)
					{
						group.DefaultFunctionName = function;
					}
					
					this.workspace.Document.UpdateSyntheticSeries ();
					this.Refresh ();
				};
		}


		private readonly WorkspaceController	workspace;
		private readonly ItemListController<GroupView> itemsController;
		private readonly GroupDetailsController detailsController;
		
		private Widget							container;
		private GraphDataGroup					activeGroup;
		private GroupView						activeGroupView;
	}
}
