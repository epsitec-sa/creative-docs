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
using Epsitec.Common.Graph.Styles;
using Epsitec.Common.Graph;

namespace Epsitec.Cresus.Graph.Controllers
{
	internal sealed class WorkspaceController
	{
		public WorkspaceController(GraphApplication application)
		{
			this.application = application;
			this.filterCategories = new HashSet<GraphDataCategory> ();
			this.colorStyle = new ColorStyle ("line-color") { "Red", "DeepPink", "Coral", "Tomato", "SkyBlue", "RoyalBlue", "DarkBlue", "Green", "PaleGreen", "Lime", "Yellow", "Wheat" };
			
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


			this.labelColorStyle = new ColorStyle ("labels")
			{
				Color.FromRgb (1.0, 1.0, 1.0),
				Color.FromRgb (0.8, 1.0, 0.8),
				Color.FromRgb (1.0, 0.8, 0.8),
				Color.FromRgb (0.8, 0.8, 1.0),
				Color.FromRgb (1.0, 0.8, 1.0),
			};
			
			this.graphType = Res.Commands.GraphType.UseLineChart;
		}

		
		public Command GraphType
		{
			get
			{
				return this.graphType;
			}
			set
			{
				if (this.graphType != value)
				{
					this.graphType = value;
//-					this.commandBar.SelectedItem = this.graphType;
					this.RefreshPreview ();
				}
			}
		}
		
		
		public void SetupUI()
		{
			this.SetupToolsFrameUI ();
			this.SetupWorkspaceFrameUI ();
		}

		private void SetupToolsFrameUI()
		{
			var controller = this.application.MainWindowController;
			var container  = controller.ToolsFrame;

			var settingsFrame = new FrameBox ()
			{
				Dock = DockStyle.Fill,
				PreferredWidth = 400,
				Name = "settings",
				BackColor = Color.FromRgb (0.9, 1, 0.9),
				Parent = container,
			};

			var filterFrame = new FrameBox ()
			{
				Dock = DockStyle.Left,
				PreferredWidth = 100,
				Name = "filters",
				Parent = settingsFrame,
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
			};

		}
		
		private void SetupWorkspaceFrameUI()
		{
			var controller = this.application.MainWindowController;
			var container  = controller.WorkspaceFrame;
			
			var inputFrame = new FrameBox ()
			{
				Dock = DockStyle.Fill,
				Parent = container,
				Name = "inputs",
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
				Name = "left",
				Padding = new Margins (0, 0, 2, 0),
			};

			var groupFrame = new FrameBox ()
			{
				Dock = DockStyle.Fill,
				Parent = bottomFrameLeft,
				Name = "groups",
				BackColor = Color.FromBrightness (1.0),
				PreferredHeight = 161,
				Padding = new Margins (0, 0, 1, 0),
			};

			var outputFrame = new FrameBox ()
			{
				Dock = DockStyle.Bottom,
				Parent = bottomFrameLeft,
				Name = "outputs",
				PreferredHeight = 104
			};

			var outputBook = new TabBook ()
			{
				Dock = DockStyle.Fill,
				Parent = outputFrame,
				Name = "book",
			};

			var outputPage = new TabPage ()
			{
				Name = "page1",
				TabTitle = "Graphique",
			};

			outputBook.Items.Add (outputPage);
			outputBook.ActivePage = outputPage;

			var previewFrame = new FrameBox ()
			{
				Dock = DockStyle.Right,
				Parent = bottomFrame,
				Name = "preview",
				BackColor = Color.FromRgb (1, 0.85, 0.8),
				PreferredWidth = 260,
				Padding = new Margins (1, 0, 1, 0)
			};

			WorkspaceController.PatchTabBookPaintBackground (outputBook);
			WorkspaceController.PatchInputFramePaintBackground (inputFrame);
			WorkspaceController.PatchBottomFrameLeftPaintBackground (bottomFrameLeft);
			WorkspaceController.PatchPreviewFramePaintBackground (outputBook, previewFrame);

			this.inputItemsController.SetupUI (inputFrame);
			this.groupItemsController.SetupUI (groupFrame);
			this.groupDetailItemsController.SetupUI (groupFrame);
			this.outputItemsController.SetupUI (outputPage);

			this.chartView = new ChartView ()
			{
				Dock = DockStyle.Fill,
				Parent = previewFrame,
				Padding = new Margins (16, 24, 24, 16),
			};

			this.inputItemsWarning = new StaticText ()
			{
				Anchor = AnchorStyles.All,
				Parent = inputFrame,
				Visibility = false,
				Text = "Aucune donnée n'est visible pour l'instant.<br/>Cochez les catégories que vous souhaitez voir apparaître ici.",
				ContentAlignment = ContentAlignment.MiddleCenter,
				PreferredHeight = 40,
			};
		}


		private static void PatchTabBookPaintBackground(TabBook outputBook)
		{
			outputBook.PaintBackground +=
				(sender, e) =>
				{
					var graphics = e.Graphics;
					var adorner  = Epsitec.Common.Widgets.Adorners.Factory.Active;
					var part     = Rectangle.Deflate (outputBook.Client.Bounds, new Margins (0.0, 0.0, outputBook.TabHeight + 0.5, 0.0));

					graphics.AddLine (part.TopLeft, part.TopRight);
					graphics.RenderSolid (adorner.ColorBorder);

					graphics.AddFilledRectangle (Rectangle.Deflate (part, new Margins (0, 0, 0.5, 0)));
					graphics.RenderSolid (Color.FromBrightness (1));

					e.Suppress = true;
				};
		}

		private static void PatchInputFramePaintBackground(FrameBox inputFrame)
		{
			inputFrame.PaintBackground +=
				(sender, e) =>
				{
					var bounds   = inputFrame.Client.Bounds;
					var graphics = e.Graphics;
					var adorner  = Epsitec.Common.Widgets.Adorners.Factory.Active;

					double y2 = bounds.Top - 0.5;
					double x2 = bounds.Right - 0.5;

					graphics.AddFilledRectangle (bounds);
					graphics.RenderSolid (Color.FromBrightness (1.0));

					graphics.AddLine (0.0, y2, x2, y2);
					graphics.RenderSolid (adorner.ColorBorder);

					e.Suppress = true;
				};
		}

		private static void PatchBottomFrameLeftPaintBackground(FrameBox bottomFrameLeft)
		{
			bottomFrameLeft.PaintBackground +=
				(sender, e) =>
				{
					var bounds   = bottomFrameLeft.Client.Bounds;
					var graphics = e.Graphics;
					var adorner  = Epsitec.Common.Widgets.Adorners.Factory.Active;

					double y2 = bounds.Top - 0.5;
					double x2 = bounds.Right - 0.5;

					graphics.AddFilledRectangle (bounds);
					graphics.RenderSolid (Color.FromBrightness (1.0));

					graphics.AddLine (0.0, y2, x2, y2);
					graphics.RenderSolid (adorner.ColorBorder);

					e.Suppress = true;
				};
		}

		private static void PatchPreviewFramePaintBackground(TabBook outputBook, FrameBox previewFrame)
		{
			previewFrame.PaintBackground +=
				(sender, e) =>
				{
					var bounds   = previewFrame.Client.Bounds;
					var graphics = e.Graphics;
					var adorner  = Epsitec.Common.Widgets.Adorners.Factory.Active;

					double y1 = outputBook.Client.Bounds.Top - outputBook.TabHeight - 0.5;
					double y2 = bounds.Top - 0.5;
					double x2 = bounds.Right - 0.5;

					graphics.AddFilledRectangle (bounds);
					graphics.RenderSolid (Color.FromBrightness (1.0));

					graphics.AddLine (0.5, y1, 0.5, y2);
					graphics.AddLine (0.5, y2, x2, y2);
					graphics.RenderSolid (adorner.ColorBorder);

					e.Suppress = true;
				};
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
			if (this.Document != null)
			{
				this.RefreshInputs ();
				this.RefreshOutputs ();
				this.RefreshGroups ();
				this.RefreshPreview ();
				this.RefreshFilters ();
			}
		}

		private void RefreshFilters()
		{

			var container = this.application.MainWindowController.ToolsFrame.FindChild ("filters", Widget.ChildFindMode.Deep);

			container.Children.Widgets.ForEach (x => x.Dispose ());

			System.Diagnostics.Debug.Assert (container.Children.Count == 0);

			var label = new StaticText ()
			{
				Dock = DockStyle.Stacked,
				PreferredHeight = 20,
				Parent = container,
				Text = "Catégories",
				ContentAlignment = ContentAlignment.MiddleCenter,
			};

			foreach (var category in this.Document.ActiveDataSource.Categories)
			{
				this.CreateFilterButton (container, category);
			}
		}

		private void CreateFilterButton(Widget filters, GraphDataCategory category)
		{
			var frame = new FrameBox ()
			{
				Dock = DockStyle.Stacked,
				PreferredHeight = 24,
				BackColor = this.labelColorStyle[category.Index],
				Parent = filters,
				Padding = new Margins (4, 4, 1, 1),
			};

			frame.PaintBackground +=
				(sender, e) =>
				{
					var label = Rectangle.Deflate (frame.Client.Bounds, new Margins (3, 3, 3, 3));
					var graphics = e.Graphics;
					var transform = graphics.Transform;
					graphics.RotateTransformDeg (0, label.Center.X, label.Center.Y);

					MiniChartView.PaintShadow (graphics, label);
					graphics.AddFilledRectangle (label);

					Color color1 = frame.BackColor;
					Color color2 = Color.Mix (color1, Color.FromBrightness (1), 0.75);

					graphics.GradientRenderer.Fill = GradientFill.Y;
					graphics.GradientRenderer.SetColors (color1, color2);
					graphics.GradientRenderer.SetParameters (0, 100);
					graphics.GradientRenderer.Transform = Transform.Identity.Scale (1.0, label.Height / 100.0).Translate (label.BottomLeft);
					graphics.RenderGradient ();

					graphics.Transform = transform;
					e.Suppress = true;
				};

			var button = new CheckButton ()
			{
				Text = category.Name,
				Dock = DockStyle.Fill,
				Parent = frame,
				ActiveState = this.filterCategories.Contains (category) ? ActiveState.Yes : ActiveState.No,
			};

			button.ActiveStateChanged +=
				sender =>
				{
					bool changed;

					if (button.ActiveState == ActiveState.Yes)
					{
						changed = this.filterCategories.Add (category);
					}
					else
					{
						changed = this.filterCategories.Remove (category);
					}

					if (changed)
					{
						this.RefreshInputs ();
					}
				};
		}

		
		private void RefreshInputs()
		{
			this.inputItemsController.Clear ();

			foreach (var item in this.Document.DataSeries)
			{
				var category = item.GetCategory ();

				if (category.IsGeneric || this.filterCategories.Contains (category))
				{
					this.inputItemsController.Add (this.CreateInputView (item));
				}
			}

			this.inputItemsWarning.Visibility = (this.inputItemsController.Count == 0);
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

				item.DefineIconButton (visibility, icon, this.CreateGroup);
			}
		}

		private void RefreshGroupView()
		{
			var view  = this.groupItemsController.ActiveItem;
			int index = this.groupItemsController.IndexOf (view);

			if (index < 0)
			{
				this.ShowGroupCalculator (null, null);
				this.ShowGroupDetails (null);
			}
			else
			{
				var group = this.Document.Groups[index];
				this.ShowGroupCalculator (group, view);
				this.ShowGroupDetails (group);
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
				
				this.ShowGroupCalculator (group, view);
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
			var synt = item as GraphSyntheticDataSeries;
			
			string iconName = "manifest:Epsitec.Common.Graph.Images.Glyph.DropItem.icon";

			if (synt != null)
			{
				view.DefineIconButton (ButtonVisibility.ShowOnlyWhenEntered, iconName,
					delegate
					{
						group.RemoveSyntheticDataSeries (synt.FunctionName);
						this.Document.UpdateSyntheticSeries ();
						this.Refresh ();
					});
			}
			else
			{
				view.DefineIconButton (ButtonVisibility.ShowOnlyWhenEntered, iconName,
					delegate
					{
						group.Remove (item);
						this.UpdateGroupName (group);
					});
			}

			return view;
		}
		
		private MiniChartView CreateInputView(GraphDataSeries item)
		{
			var view = this.CreateView (item);
			var cat  = item.Source.GetCategory (item);

			view.AutoCheckButton = true;
			view.ActiveState = item.IsSelected ? ActiveState.Yes : ActiveState.No;
			view.LabelColor = this.labelColorStyle[cat.Index];

			view.ActiveStateChanged +=
				delegate
				{
					if (view.ActiveState == ActiveState.Yes)
					{
						this.Document.AddOutput (item);
						this.RefreshOutputs ();
						this.RefreshPreview ();
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
			var view = this.CreateView (item);
			string iconName = "manifest:Epsitec.Common.Graph.Images.Glyph.DropItem.icon";

			view.DefineIconButton (ButtonVisibility.ShowOnlyWhenEntered, iconName,
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
			var items = this.inputItemsController.Where (x => x.IsSelected).Select (x => this.Document.DataSeries.ElementAt (x.Index));

			GraphDataGroup group;

			if (index < 0)
			{
				group = this.CreateGroup (items);
				index = this.Document.Groups.IndexOf (group);
			}
			else
			{
				group = this.UpdateGroup (this.Document.Groups[index], items);
			}

			this.groupItemsController.ActiveItem = this.groupItemsController[index];
			this.UpdateGroupName (group);
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

		private void UpdateGroupName(GraphDataGroup group)
		{
			int count = group.Count;

			group.Name = string.Format (count > 1 ? "{0} éléments" : "{0} élément", count);

			this.Refresh ();
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

			lineChartRenderer.AddStyle (this.colorStyle);
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

				if ((!view.IsActualGeometryValid) &&
					(view.Window != null))
				{
					view.Window.ForceLayout ();
				}

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

			foreach (var item in series)
			{
				this.groupDetailItemsController.Add (this.CreateGroupDetailView (group, item));
			}
		}

		private void CreateFunctionButton(GraphDataGroup group, VerticalInjectionArrow arrow, string function)
		{
			var button = new GraphicIconButton ()
			{
				IconFamilyName = "manifest:Epsitec.Cresus.Graph.Images.Button",
				HorizontalAlignment = HorizontalAlignment.Center,
				PreferredSize = new Size (36, 20),
				Dock = DockStyle.Stacked,
				Parent = arrow,
				AutoToggle = true,
				Name = function,
				ActiveState = group.SyntheticDataSeries.Where (x => x.Enabled && x.FunctionName == function).Count () == 0 ? ActiveState.No : ActiveState.Yes,
			};

			button.ActiveStateChanged +=
				delegate
				{
					if (button.ActiveState == ActiveState.Yes)
					{
						group.AddSyntheticDataSeries (function);
					}
					else
					{
						group.RemoveSyntheticDataSeries (function);
					}
					this.Document.UpdateSyntheticSeries ();
					this.Refresh ();
				};
		}


		private void RefreshPreview()
		{
			if (this.Document != null)
			{
				var renderer = this.CreateRenderer ();

				if (renderer == null)
				{
					this.chartView.Renderer   = null;
//-					this.captionView.Captions = null;
				}
				else
				{
					List<ChartSeries> series = new List<ChartSeries> (this.GetDocumentChartSeries ());

					bool stackValues = false;

					renderer.Clear ();
					renderer.ChartSeriesRenderingMode = stackValues ? ChartSeriesRenderingMode.Stacked : ChartSeriesRenderingMode.Separate;
					renderer.DefineValueLabels (this.Document.ChartColumnLabels);
					renderer.CollectRange (series);
					renderer.UpdateCaptions (series);
					renderer.AlwaysIncludeZero = true;

//-					Size size = renderer.Captions.GetCaptionLayoutSize (Size.MaxValue) + this.captionView.Padding.Size;

					var layoutMode = ContainerLayoutMode.None;

					switch (layoutMode)
					{
						case ContainerLayoutMode.HorizontalFlow:
//-							this.captionView.PreferredHeight = size.Height;
							break;

						case ContainerLayoutMode.VerticalFlow:
//-							this.captionView.PreferredWidth = size.Width;
							break;
					}

					this.chartView.Renderer = renderer;
//-					this.captionView.Captions = renderer.Captions;
				}

				this.chartView.Invalidate ();
//-				this.captionView.Invalidate ();
			}
		}

		private IEnumerable<ChartSeries> GetDocumentChartSeries()
		{
			bool accumulateValues = false;
			foreach (var series in this.Document.OutputSeries.Select (x => x.ChartSeries))
			{
				yield return new ChartSeries (series.Label, accumulateValues ? WorkspaceController.Accumulate (series.Values) : series.Values);
			}
		}

		private static IEnumerable<ChartValue> Accumulate(IEnumerable<ChartValue> collection)
		{
			double accumulation = 0.0;

			foreach (var value in collection)
			{
				accumulation += value.Value;
				yield return new ChartValue (value.Label, accumulation);
			}
		}

		private AbstractRenderer CreateRenderer()
		{
			AbstractRenderer renderer = null;
			bool stackValues = false;

			if (this.GraphType == Res.Commands.GraphType.UseLineChart)
			{
				renderer = new LineChartRenderer ()
				{
					SurfaceAlpha = stackValues ? 1.0 : 0.0
				};
			}
			else if (this.GraphType == Res.Commands.GraphType.UseBarChartVertical)
			{
				renderer = new BarChartRenderer ();
			}

			if (renderer != null)
			{
				var adorner = new Epsitec.Common.Graph.Adorners.CoordinateAxisAdorner ()
				{
					GridColor = Color.FromBrightness (0.8),
					VisibleGrid = true,
					VisibleLabels = false,
					VisibleTicks = true,
				};

				renderer.AddStyle (this.colorStyle);
				renderer.AddAdorner (adorner);
			}

			return renderer;
		}

		
		public System.Action<IEnumerable<int>>	SumSeriesAction;
		public System.Action<IEnumerable<int>>	AddSeriesToGraphAction;
		public System.Action<IEnumerable<int>>	NegateSeriesAction;

		private readonly GraphApplication		application;

		private readonly ItemListController<MiniChartView>		inputItemsController;
		private readonly ItemListController<MiniChartView>		outputItemsController;
		private readonly ItemListController<MiniChartView>		groupItemsController;
		private readonly ItemListController<MiniChartView>		groupDetailItemsController;
		private readonly HashSet<GraphDataCategory> filterCategories;
		
		private ChartView				chartView;

		private StaticText				inputItemsWarning;
		private VerticalInjectionArrow	groupCalculatorArrow;
		private ColorStyle labelColorStyle;
		private Command							graphType;
		private ColorStyle colorStyle;
	}
}
