﻿//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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

			this.groupItemsController = new ItemListController<Widget> ()
			{
				ItemLayoutMode = ItemLayoutMode.Vertical,
				OverlapY = 0,
			};

			this.groupDetailsController = new GroupDetailsController (this.workspace);
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
				return this.groupItemsController.Count;
			}
		}

		public IEnumerable<MiniChartView>		GroupViews
		{
			get
			{
				return this.groupItemsController.Select (widget => widget.Children[0] as MiniChartView);
			}
		}

		public IEnumerable<MiniChartView>		DetailViews
		{
			get
			{
				return this.groupDetailsController.MiniChartViews;
			}
		}


		public void SetupUI(Widget container)
		{
			this.container = container;
			this.groupItemsController.SetupUI (container);
			this.groupDetailsController.SetupUI (container);
		}


		public void UpdateLayout()
		{
			this.groupItemsController.UpdateLayout ();
		}

		public void Refresh()
		{
			this.groupItemsController.Clear ();

			this.activeGroupView = null;

			foreach (var group in this.workspace.Document.Groups)
			{
				var view = this.CreateGroupView (group);

				if (group == this.activeGroup)
				{
					this.activeGroupView = view.Children[0] as MiniChartView;
				}

				this.groupItemsController.Add (view);
			}

			this.groupDetailsController.ShowGroupDetails (this.activeGroup, this.activeGroupView);
			this.workspace.RefreshHints ();
		}


		public GraphDataGroup CreateGroup(IEnumerable<GraphDataSeries> series)
		{
			var group = this.workspace.Document.AddGroup (series);

			this.groupItemsController.Add (this.CreateGroupView (group));
			this.UpdateGroupName (group);

			this.activeGroup = null;
			this.activeGroupView = null;

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
			this.groupItemsController.Remove (view);
			this.UpdateGroupName (group);
		}

		public void DetectGroup(Point mouse, System.Action<int, bool> setGroup, System.Action<double, double> setGeometry)
		{
			if (this.groupItemsController.Count == 0)
			{
				setGroup (0, false);
				setGeometry (0.0, 0.0);
				return;
			}

			var drop = this.groupItemsController.Where (x => x.HitTest (mouse)).FirstOrDefault ();

			if (drop != null)
			{
				double y = drop.Parent.ActualHeight - drop.ActualBounds.Top;
				setGroup (drop.Index, true);
				setGeometry (y, drop.ActualHeight);
				return;
			}
			
			var dist = this.groupItemsController.Select (x => new
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
				double y = this.groupItemsController.Container.ActualHeight - this.groupItemsController.Last ().ActualLocation.Y - 2; 
				setGroup (this.groupItemsController.Count, false);
				setGeometry (y, 2.0);
			}
		}

		private void UpdateGroupName(GraphDataGroup group)
		{
			int count = group.Count;

			group.Name = string.Format (count > 1 ? "{0} éléments" : "{0} élément", count);

			this.Refresh ();
		}

		private Widget CreateGroupView(GraphDataGroup group)
		{
			var view = this.workspace.CreateView (group);

			view.Clicked +=
				(sender, e) =>
				{
					if ((e.Message.Button == MouseButtons.Left) &&
						(e.Message.ButtonDownCount == 1))
					{
						this.HandleGroupViewClicked (group, view);
					}
				};

			string iconName = "manifest:Epsitec.Common.Graph.Images.Glyph.DropItem.icon";

			view.DefineIconButton (ButtonVisibility.ShowOnlyWhenEntered, iconName,
				delegate
				{
					//	TODO: make this fool proof (deleting a group which is still used is fatal)
					this.DeleteGroup (group, view);
					this.Refresh ();
				});

			var frame = new FrameBox ()
			{
				PreferredHeight = view.PreferredHeight + 8,
				Padding = new Margins (0, 0, 4, 4),
			};

			view.Parent = frame;
			view.Dock   = DockStyle.Left;

			return frame;
		}

		private void HandleGroupViewClicked(GraphDataGroup group, MiniChartView view)
		{
			this.activeGroup = group;
			this.activeGroupView = view;
			this.workspace.RefreshInputViewSelection ();
			this.groupDetailsController.ShowGroupDetails (this.activeGroup, this.activeGroupView);
		}

		private void ShowGroupCalculator(GraphDataGroup group, MiniChartView view)
		{
			if (this.groupCalculatorArrow != null)
			{
				this.groupCalculatorArrow.Dispose ();
				this.groupCalculatorArrow = null;
			}

			if (view != null)
			{
				this.groupItemsController.UpdateLayout ();

				var bounds = view.MapClientToRoot (view.Client.Bounds);
				var arrow  = new VerticalInjectionArrow ()
				{
					Anchor = AnchorStyles.BottomLeft,
					Margins = new Margins (bounds.Left, 0, 0, bounds.Top + 2),
					Parent = view.RootParent,
					PreferredWidth = view.PreferredWidth,
					ArrowWidth = 40,
					Padding = new Margins (0, 0, 24, 4),
					BackColor = Color.FromBrightness (1),
					ContainerLayoutMode = ContainerLayoutMode.VerticalFlow
				};

				this.CreateFunctionButton (group, arrow, Functions.FunctionFactory.FunctionSum);

				this.groupCalculatorArrow = arrow;
			}
		}

		private void CreateFunctionButton(GraphDataGroup group, VerticalInjectionArrow arrow, string function)
		{
			var container = new FrameBox ()
			{
				Dock = DockStyle.Stacked,
				Parent = arrow,
				PreferredHeight = 20,
			};

			var button = new GraphicIconButton ()
			{
				IconFamilyName = "manifest:Epsitec.Cresus.Graph.Images.Button",
				HorizontalAlignment = HorizontalAlignment.Center,
				PreferredSize = new Size (36, 20),
				Dock = DockStyle.Fill,
				Parent = container,
				AutoToggle = true,
				Name = function,
				ActiveState = group.SyntheticDataSeries.Where (x => x.Enabled && x.FunctionName == function).Count () == 0 ? ActiveState.No : ActiveState.Yes,
			};

			var check = new CheckButton ()
			{
				Anchor = AnchorStyles.TopRight,
				Text = "",
				Parent = container,
				Margins = new Margins (0, 2, 2, 0),
				PreferredSize = new Size (15, 15),
				ActiveState = group.SyntheticDataSeries.Where (x => x.Enabled && x.FunctionName == function && x.IsSelected).Count () == 0 ? ActiveState.No : ActiveState.Yes,
				Visibility = button.ActiveState == ActiveState.Yes
			};

			//	TODO: ...handle check box...

			button.ActiveStateChanged +=
				delegate
				{
					if (button.ActiveState == ActiveState.Yes)
					{
						group.AddSyntheticDataSeries (function);
						check.Visibility = true;
					}
					else
					{
						group.RemoveSyntheticDataSeries (function);
						check.Visibility = false;
					}
					this.workspace.Document.UpdateSyntheticSeries ();
					this.Refresh ();
				};

			check.ActiveStateChanged +=
				delegate
				{
					var item = group.SyntheticDataSeries.Where (x => x.Enabled && x.FunctionName == function).FirstOrDefault ();

					if (item != null)
					{
						if (check.ActiveState == ActiveState.Yes)
						{
							this.workspace.IncludeOutput (item);
						}
						else
						{
							this.workspace.ExcludeOutput (item);
						}
					}
				};

		}


		private readonly WorkspaceController workspace;
		private readonly ItemListController<Widget>				groupItemsController;
		private readonly GroupDetailsController groupDetailsController;
		
		private Widget container;
		private GraphDataGroup activeGroup;
		private MiniChartView activeGroupView;
		private VerticalInjectionArrow	groupCalculatorArrow;
	}
}
