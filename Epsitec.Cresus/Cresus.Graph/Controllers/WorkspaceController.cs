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

		
		public GraphDataSource DataSource
		{
			get
			{
				return this.application.Document.ActiveDataSource;
			}
		}

		
		public void Refresh()
		{
			if ((this.DataSource != null) &&
				(this.DataSource.Count > 0))
			{
				this.RefreshInputs ();
				this.RefreshOutputs ();
			}
		}

		private void RefreshInputs()
		{
			this.inputItemsController.Clear ();

			foreach (var item in this.DataSource)
			{
				this.inputItemsController.Add (this.CreateInputMiniChartView (item));
			}
		}

		private void RefreshOutputs()
		{
			this.outputItemsController.Clear ();

			foreach (var item in this.application.Document.OutputSeries)
			{
				this.outputItemsController.Add (this.CreateOutputMiniChartView (item));
			}
		}

		private MiniChartView CreateOutputMiniChartView(GraphDataSeries item)
		{
			var view = this.CreateInputMiniChartView (item);

			view.ShowIconButton (ButtonVisibility.ShowOnlyWhenEntered,
				delegate
				{
					this.HandleDropButtonClicked (item);
				},
				"manifest:Epsitec.Common.Graph.Images.Glyph.DropItem.icon");
			
			return view;
		}

		private MiniChartView CreateInputMiniChartView(GraphDataSeries item)
		{
			var view = this.CreateMiniChartView (item);

			view.AutoCheckButton = true;
			view.ActiveState = item.IsSelected ? ActiveState.Yes : ActiveState.No;

			view.ActiveStateChanged +=
				delegate
				{
					if (view.ActiveState == ActiveState.Yes)
					{
						this.AddSeriesToGraph (item);
					}
					else
					{
						this.RemoveSeriesFromGraph (item);
					}
				};

			view.Clicked +=
				(sender, e) =>
				{
					if ((e.Message.Button == MouseButtons.Left) &&
						(e.Message.ButtonDownCount == 1))
					{
						if (Message.CurrentState.IsControlPressed)
						{
							view.SetSelected (!view.IsSelected);
						}
						else
						{
							bool select = !view.IsSelected;
							this.inputItemsController.ForEach (x => x.SetSelected (false));
							view.SetSelected (select);
						}

						this.UpdateUserSelection ();
					}
				};
			
			return view;
		}

		private void UpdateUserSelection()
		{
			int n = this.inputItemsController.Where (x => x.IsSelected).Count ();
			
			foreach (var item in this.inputItemsController)
			{
				item.ShowIconButton (item.IsSelected && n > 1 ? ButtonVisibility.Show : ButtonVisibility.Hide,
					this.HandleGroupButtonClicked,
					"manifest:Epsitec.Common.Graph.Images.Glyph.Group.icon");
			}
		}

		private void HandleDropButtonClicked(GraphDataSeries item)
		{
			this.application.Document.RemoveOutput (item);
			this.Refresh ();
		}

		private void HandleGroupButtonClicked()
		{
			this.CreateGroupView (this.inputItemsController.Where (x => x.IsSelected).Select (x => x.Renderer.SeriesItems.First ()));
		}

		private void CreateGroupView(IEnumerable<ChartSeries> series)
		{
			var view = this.CreateMiniChartView (null);

			this.groupItemsController.Add (view);

			view.Renderer.Clear ();
			view.Renderer.CollectRange (series);
			view.Label = "Nouveau";
			view.Title = string.Format (view.Renderer.SeriesCount > 1 ? "{0} éléments" : "{0} élément", view.Renderer.SeriesCount);
			view.Clicked +=
				(sender, e) =>
				{
					MiniChartView v = sender as MiniChartView;

					if ((e.Message.Button == MouseButtons.Left) &&
						(e.Message.ButtonDownCount == 1))
					{
						bool select = !v.IsSelected;
						this.groupItemsController.ForEach (x => x.SetSelected (false));
						v.SetSelected (select);

						var bounds = v.MapClientToRoot (v.Client.Bounds);

						var arrow = new VerticalInjectionArrow ()
						{
							Anchor = AnchorStyles.BottomLeft,
							Margins = new Margins (bounds.Left, 0, 0, bounds.Top),
							Parent = v.RootParent,
							PreferredWidth = v.PreferredWidth,
							ArrowWidth = 40,
							Padding = new Margins (0, 0, 24, 4),
							BackColor = v.Parent.BackColor,
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
						/*
						var b2 = new GraphicIconButton ()
						{
							IconFamilyName = "manifest:Epsitec.Cresus.Graph.Images.Button",
							HorizontalAlignment = HorizontalAlignment.Center,
							PreferredSize = new Size (36, 20),
							Dock = DockStyle.Stacked,
							Parent = arrow,
							AutoToggle = true
						};

						var b3 = new GraphicIconButton ()
						{
							IconFamilyName = "manifest:Epsitec.Cresus.Graph.Images.Button",
							HorizontalAlignment = HorizontalAlignment.Center,
							PreferredSize = new Size (36, 20),
							Dock = DockStyle.Stacked,
							Parent = arrow,
							AutoToggle = true
						};
						*/
						if (select)
						{
//-							view.Renderer.SeriesItems.ForEach (s => this.groupDetailItemsController.Add (this.CreateMiniChartView (-1, s)));
						}
						else
						{
							this.groupDetailItemsController.Clear ();
						}
					}
				};

			this.inputItemsController.ForEach (x => x.SetSelected (false));
			this.UpdateUserSelection ();
		}

		private MiniChartView CreateMiniChartView(GraphDataSeries item)
		{
			var lineChartRenderer = new LineChartRenderer ();

			lineChartRenderer.AddStyle (new Epsitec.Common.Graph.Styles.ColorStyle ("line-color") { "Red", "DeepPink", "Coral", "Tomato", "SkyBlue", "RoyalBlue", "DarkBlue", "Green", "PaleGreen", "Lime", "Yellow", "Wheat" });
			lineChartRenderer.AddAdorner (new Epsitec.Common.Graph.Adorners.CoordinateAxisAdorner ()
			{
				VisibleGrid = false,
				VisibleLabels = false,
				VisibleTicks = false
			});

			lineChartRenderer.DefineValueLabels (this.application.Document.ChartColumnLabels);

			var view = new MiniChartView ()
			{
				Anchor = AnchorStyles.TopLeft,
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				PreferredWidth = 80,
				PreferredHeight = 80,
				Padding = new Margins (4, 4, 4, 4),
				Margins = new Margins (0, 0, 0, 0),
				Scale = 0.5,
			};

			if (item != null)
			{
				var series = item.ChartSeries;
				
				lineChartRenderer.Collect (series);
				
				view.Renderer = lineChartRenderer;
				view.Title = item.Title;
				view.Label = item.Label;
			}
			
			return view;
		}

		private void AddSeriesToGraph(GraphDataSeries item)
		{
			this.application.Document.AddOutput (item);
			this.RefreshOutputs ();
		}

		private void RemoveSeriesFromGraph(GraphDataSeries item)
		{
			this.application.Document.RemoveOutput (item);
			this.Refresh ();
		}

		private void OnChanged()
		{
			var handler = this.Changed;

			if (handler != null)
			{
				handler (this);
			}
		}


		public event EventHandler				Changed;
		
		public System.Action<IEnumerable<int>>	SumSeriesAction;
		public System.Action<IEnumerable<int>>	AddSeriesToGraphAction;
		public System.Action<IEnumerable<int>>	NegateSeriesAction;

		private readonly GraphApplication		application;

		private readonly ItemListController<MiniChartView>		inputItemsController;
		private readonly ItemListController<MiniChartView>		outputItemsController;
		private readonly ItemListController<MiniChartView>		groupItemsController;
		private readonly ItemListController<MiniChartView>		groupDetailItemsController;
	}
}
