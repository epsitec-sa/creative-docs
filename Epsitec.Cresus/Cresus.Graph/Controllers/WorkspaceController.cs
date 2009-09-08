//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Graph.Renderers;
using Epsitec.Common.Graph.Widgets;

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Graph.Data;
using Epsitec.Cresus.Graph.Widgets;

namespace Epsitec.Cresus.Graph.Controllers
{
	internal sealed class WorkspaceController
	{
		public WorkspaceController(GraphApplication application)
		{
			this.application = application;

			this.inputItemsController = new ItemListController<MiniChartView> ()
			{
				ItemLayoutMode = ItemLayoutMode.Flow
			};

			this.outputItemsController = new ItemListController<MiniChartView> ()
			{
				ItemLayoutMode = ItemLayoutMode.Horizontal
			};

			this.groupItemsController = new ItemListController<MiniChartView> ()
			{
				ItemLayoutMode = ItemLayoutMode.Horizontal
			};

			this.groupDetailItemsController = new ItemListController<MiniChartView> ()
			{
				ItemLayoutMode = ItemLayoutMode.Horizontal,
				Anchor         = AnchorStyles.BottomLeft,
			};
		}
		
		
		public void SetupUI()
		{
			var container = this.application.MainWindowController.WorkspaceFrame;

			var inputFrame = new FrameBox ()
			{
				Dock = DockStyle.Fill,
				Parent = container,
				Name = "inputs",
				BackColor = Color.FromRgb (0.9, 1, 0.9),
				PreferredHeight = 320
			};

			var bottomFrame = new FrameBox ()
			{
				Dock = DockStyle.Bottom,
				Parent = container,
				Name = "bottom",
				PreferredHeight = 260
			};

			var bottomFrameLeft = new FrameBox ()
			{
				Dock = DockStyle.Fill,
				Parent = bottomFrame,
				Name = "left"
			};

			var groupFrame = new FrameBox ()
			{
				Dock = DockStyle.Fill,
				Parent = bottomFrameLeft,
				Name = "groups",
				BackColor = Color.FromRgb (0.9, 0.9, 1),
				PreferredHeight = 160
			};

			var outputFrame = new FrameBox ()
			{
				Dock = DockStyle.Bottom,
				Parent = bottomFrameLeft,
				Name = "outputs",
				BackColor = Color.FromRgb (1, 0.9, 0.8),
				PreferredHeight = 100
			};

			var previewFrame = new FrameBox ()
			{
				Dock = DockStyle.Right,
				Parent = bottomFrame,
				Name = "preview",
				BackColor = Color.FromRgb (1, 0.85, 0.8),
				PreferredWidth = 260
			};

			this.inputItemsController.SetupUI (inputFrame);
			this.groupItemsController.SetupUI (groupFrame);
			this.groupDetailItemsController.SetupUI (groupFrame);
			this.outputItemsController.SetupUI (outputFrame);
		}

		
		private GraphDataSource DataSource
		{
			get
			{
				return this.application.Document.ActiveDataSource;
			}
		}

		private GraphDocument Document
		{
			get
			{
				return this.application.Document;
			}
		}

		
		public void Refresh()
		{
			if ((this.DataSource != null) &&
				(this.DataSource.Count > 0))
			{
				this.RefreshInputs ();
				this.RefreshOutputs ();
				this.RefreshGroups ();
			}
		}

		
		private void RefreshInputs()
		{
			this.inputItemsController.Clear ();

			foreach (var item in this.DataSource)
			{
				this.inputItemsController.Add (this.CreateInputView (item));
			}
		}

		private void RefreshOutputs()
		{
			this.outputItemsController.Clear ();

			foreach (var item in this.Document.OutputSeries)
			{
				this.outputItemsController.Add (this.CreateOutputView (item));
			}
		}

		private void RefreshInputViewSelection()
		{
			int    count = this.inputItemsController.Where (x => x.IsSelected).Count () + this.groupItemsController.Where (x => x.IsSelected).Count ();
			string icon  = "manifest:Epsitec.Common.Graph.Images.Glyph.Group.icon";

			foreach (var item in this.inputItemsController)
			{
				var visibility = item.IsSelected && count > 1 ? ButtonVisibility.Show : ButtonVisibility.Hide;

				item.ShowIconButton (visibility, icon, this.CreateGroup);
			}
		}

		private void RefreshGroupView()
		{
			var view  = this.groupItemsController.ActiveItem;
			int index = this.groupItemsController.IndexOf (view);

			if (index < 0)
			{
				this.ShowGroupCalculator (null);
				this.ShowGroupDetails (null);
			}
			else
			{
				this.ShowGroupCalculator (view);
				this.ShowGroupDetails (this.Document.Groups[index]);
			}
		}

		private void RefreshGroups()
		{
			int index = this.groupItemsController.IndexOf (this.groupItemsController.ActiveItem);

			this.groupItemsController.Clear ();

			foreach (var group in this.Document.Groups)
			{
				this.groupItemsController.Add (this.CreateGroupView (group));
			}

			if ((index >= 0) &&
				(index < this.groupItemsController.Count))
			{
				var view  = this.groupItemsController[index];
				var group = this.Document.Groups[index];

				this.groupItemsController.ActiveItem = this.groupItemsController[index];
				
				this.ShowGroupCalculator (view);
				this.ShowGroupDetails (group);

				view.SetSelected (true);
			}
		}

		
		private MiniChartView CreateGroupView(GraphDataGroup group)
		{
			var view = this.CreateView (group);

			view.Clicked +=
				(sender, e) =>
				{
					if ((e.Message.Button == MouseButtons.Left) &&
						(e.Message.ButtonDownCount == 1))
					{
						this.HandleGroupViewClicked (group, view);
					}
				};

			return view;
		}

		private MiniChartView CreateGroupDetailView(GraphDataGroup group, GraphDataSeries item)
		{
			var view = this.CreateView (item);

			string iconName = "manifest:Epsitec.Common.Graph.Images.Glyph.DropItem.icon";

			view.ShowIconButton (ButtonVisibility.ShowOnlyWhenEntered, iconName,
				delegate
				{
					group.Remove (item);
					this.UpdateGroupName (group);
				});

			return view;
		}
		
		private MiniChartView CreateInputView(GraphDataSeries item)
		{
			var view = this.CreateView (item);

			view.AutoCheckButton = true;
			view.ActiveState = item.IsSelected ? ActiveState.Yes : ActiveState.No;

			view.ActiveStateChanged +=
				delegate
				{
					if (view.ActiveState == ActiveState.Yes)
					{
						this.Document.AddOutput (item);
						this.RefreshOutputs ();
					}
					else
					{
						this.Document.RemoveOutput (item);
						this.Refresh ();
					}
				};

			view.Clicked +=
				(sender, e) =>
				{
					if ((e.Message.Button == MouseButtons.Left) &&
						(e.Message.ButtonDownCount == 1))
					{
						this.HandleInputViewClicked (view);
					}
				};
			
			return view;
		}

		private MiniChartView CreateOutputView(GraphDataSeries item)
		{
			var view = this.CreateInputView (item);
			string iconName = "manifest:Epsitec.Common.Graph.Images.Glyph.DropItem.icon";

			view.ShowIconButton (ButtonVisibility.ShowOnlyWhenEntered, iconName,
				delegate
				{
					this.Document.RemoveOutput (item);
					this.Refresh ();
				});
			
			return view;
		}


		
		private void CreateGroup()
		{
			var view  = this.groupItemsController.ActiveItem;
			int index = this.groupItemsController.IndexOf (view);
			var items = this.inputItemsController.Where (x => x.IsSelected).Select (x => this.DataSource[x.Index]);

			GraphDataGroup group;

			if (index < 0)
			{
				group = this.CreateGroup (items);
			}
			else
			{
				group = this.UpdateGroup (this.Document.Groups[index], items);
			}

			this.UpdateGroupName (group);
		}

		private void UpdateGroupName(GraphDataGroup group)
		{
			int count = group.Count;
			
			group.Name = string.Format (count > 1 ? "{0} éléments" : "{0} élément", count);

			this.Refresh ();
		}

		private GraphDataGroup UpdateGroup(GraphDataGroup group, IEnumerable<GraphDataSeries> items)
		{
			foreach (var item in items)
			{
				if (!group.Contains (item))
				{
					group.Add (item);
				}
			}

			return group;
		}

		private GraphDataGroup CreateGroup(IEnumerable<GraphDataSeries> series)
		{
			var group = this.Document.AddGroup (series);
			
			this.groupItemsController.Add (this.CreateGroupView (group));

			return group;
		}

		
		private void HandleGroupViewClicked(GraphDataGroup group, MiniChartView view)
		{
			if (Message.CurrentState.IsControlPressed)
			{
				view.SetSelected (!view.IsSelected);
			}
			else
			{
				bool select = !view.IsSelected;

				this.inputItemsController.ForEach (x => x.SetSelected (false));
				this.groupItemsController.ForEach (x => x.SetSelected (false));

				view.SetSelected (select);
			}

			if (view.IsSelected)
			{
				this.groupItemsController.ActiveItem = view;
			}
			else
			{
				this.groupItemsController.ActiveItem = null;
			}
			
			this.RefreshInputViewSelection ();
			this.RefreshGroupView ();
		}
		
		private void HandleInputViewClicked(MiniChartView view)
		{
			if (Message.CurrentState.IsControlPressed)
			{
				view.SetSelected (!view.IsSelected);
			}
			else
			{
				bool select = !view.IsSelected;
				
				this.inputItemsController.ForEach (x => x.SetSelected (false));
				this.groupItemsController.ForEach (x => x.SetSelected (false));

				this.groupItemsController.ActiveItem = null;

				view.SetSelected (select);
			}

			this.RefreshInputViewSelection ();
			this.RefreshGroupView ();
		}


		
		private MiniChartView CreateView(GraphDataSeries item)
		{
			var view = CreateView ();
			var series = item.ChartSeries;

			view.Renderer.Collect (series);
			view.Title = item.Title;
			view.Label = item.Label;

			return view;
		}

		private MiniChartView CreateView(GraphDataGroup group)
		{
			var view = CreateView ();
			
			view.Renderer.CollectRange (group.InputDataSeries.Select (x => x.ChartSeries));
			view.Title = group.Name;
			view.Label = "Groupe";

			return view;
		}

		private MiniChartView CreateView()
		{
			var lineChartRenderer = new LineChartRenderer ();

			lineChartRenderer.AddStyle (new Epsitec.Common.Graph.Styles.ColorStyle ("line-color") { "Red", "DeepPink", "Coral", "Tomato", "SkyBlue", "RoyalBlue", "DarkBlue", "Green", "PaleGreen", "Lime", "Yellow", "Wheat" });
			lineChartRenderer.AddAdorner (new Epsitec.Common.Graph.Adorners.CoordinateAxisAdorner ()
			{
				VisibleGrid = false,
				VisibleLabels = false,
				VisibleTicks = false
			});

			lineChartRenderer.DefineValueLabels (this.Document.ChartColumnLabels);

			return new MiniChartView ()
			{
				Anchor = AnchorStyles.TopLeft,
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				PreferredWidth = 80,
				PreferredHeight = 80,
				Padding = new Margins (4, 4, 4, 4),
				Margins = new Margins (0, 0, 0, 0),
				Renderer = lineChartRenderer,
				Scale = 0.5,
			};
		}

		
		private void ShowGroupCalculator(MiniChartView view)
		{
			if (this.groupCalculatorArrow != null)
			{
				this.groupCalculatorArrow.Dispose ();
				this.groupCalculatorArrow = null;
			}

			if (view != null)
			{
				if ((!view.IsActualGeometryValid) &&
					(view.Window != null))
				{
					view.Window.ForceLayout ();
				}

				var bounds = view.MapClientToRoot (view.Client.Bounds);
				var arrow  = new VerticalInjectionArrow ()
				{
					Anchor = AnchorStyles.BottomLeft,
					Margins = new Margins (bounds.Left, 0, 0, bounds.Top),
					Parent = view.RootParent,
					PreferredWidth = view.PreferredWidth,
					ArrowWidth = 40,
					Padding = new Margins (0, 0, 24, 4),
					BackColor = view.Parent.BackColor,
					ContainerLayoutMode = ContainerLayoutMode.VerticalFlow
				};

				var b1 = new GraphicIconButton ()
				{
					IconFamilyName = "manifest:Epsitec.Cresus.Graph.Images.Button",
					HorizontalAlignment = HorizontalAlignment.Center,
					PreferredSize = new Size (36, 20),
					Dock = DockStyle.Stacked,
					Parent = arrow,
					AutoToggle = true
				};

				//	TODO: add button handlers here

				this.groupCalculatorArrow = arrow;
			}
		}

		private void ShowGroupDetails(GraphDataGroup group)
		{
			this.groupDetailItemsController.Clear ();

			if (group == null)
			{
				return;
			}

			var series1 = group.SyntheticDataSeries.Cast<GraphDataSeries> ();
			var series2 = group.InputDataSeries;
			var series = series1.Concat (series2);

			series.ForEach (x => this.groupDetailItemsController.Add (this.CreateGroupDetailView (group, x)));
		}


		
		public System.Action<IEnumerable<int>>	SumSeriesAction;
		public System.Action<IEnumerable<int>>	AddSeriesToGraphAction;
		public System.Action<IEnumerable<int>>	NegateSeriesAction;

		private readonly GraphApplication		application;

		private readonly ItemListController<MiniChartView>		inputItemsController;
		private readonly ItemListController<MiniChartView>		outputItemsController;
		private readonly ItemListController<MiniChartView>		groupItemsController;
		private readonly ItemListController<MiniChartView>		groupDetailItemsController;

		private VerticalInjectionArrow	groupCalculatorArrow;
	}
}
