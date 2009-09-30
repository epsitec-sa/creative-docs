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

using Epsitec.Cresus.Graph.Controllers;
using Epsitec.Cresus.Graph.Widgets;

using System.Collections.Generic;
using System.Linq;

[assembly: DependencyClass (typeof (ChartViewController))]

namespace Epsitec.Cresus.Graph.Controllers
{
	internal sealed class ChartViewController : DependencyObject
	{
		public ChartViewController(GraphApplication application, WorkspaceController workspace)
		{
			this.application = application;
			this.workspace   = workspace;
		}

		public bool IsStandalone
		{
			get;
			set;
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
					this.Refresh ();
				}
			}
		}

		public ColorStyle ColorStyle
		{
			get
			{
				return this.colorStyle;
			}
			set
			{
				if (this.colorStyle != value)
				{
					this.colorStyle = value;
					this.Refresh ();
				}
			}
		}

		public Widget Container
		{
			get
			{
				return this.container;
			}
		}

		public bool StackValues
		{
			get;
			set;
		}

		public void SetupUI(Widget container)
		{
			this.container = container;

			if (this.IsStandalone)
			{
				var dispatcher = new CommandDispatcher ("chart view", CommandDispatcherLevel.Secondary);
				CommandDispatcher.SetDispatcher (this.container, dispatcher);
				this.localController = new CommandController (dispatcher);
			}
			else
			{
				if (ChartViewController.commandController == null)
				{
					ChartViewController.commandController = new CommandController (this.application.CommandDispatcher);
				}
			}
			
			this.commandContext = this.CreateCommandContext (this.container);

			var chartSurface = new FrameBox ()
			{
				Dock = DockStyle.Fill,
				Parent = this.container,
			};

			this.chartView = new ChartView ()
			{
				Dock = DockStyle.Fill,
				Parent = chartSurface,
				Padding = this.IsStandalone ? new Margins (48, 24, 24, 24) : new Margins (16, 24, 24, 16),
			};

			var palette = new AnchoredPalette ()
			{
				Anchor = AnchorStyles.TopRight,
				Margins = new Margins (0, 4, 4, 0),
				Parent = chartSurface,
				BackColor = Color.FromBrightness (1),
				Padding = new Margins (4, 4, 2, 2),
				Visibility = false,
			};

			this.captionView = new CaptionView ()
			{
				Dock = DockStyle.Fill,
				Parent = palette,
			};

			this.commandBar = new CommandSelectionBar ()
			{
				Anchor = AnchorStyles.LeftAndRight | AnchorStyles.Top,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				Parent = this.container,
				BackColor = Color.FromBrightness (1),
				Name = "command bar",
				Visibility = false,
			};

			this.commandButton = new Button ()
			{
				Anchor = AnchorStyles.All,
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				Parent = this.container,
				Name = "command button",
				Visibility = false,
				Text = "Montrer dans une nouvelle fenêtre",
				PreferredWidth = 120,
				PreferredHeight = 40,
			};

			if (this.IsStandalone)
			{
				this.commandBar.Dock = DockStyle.Top;
				this.commandBar.Visibility = true;
				palette.Visibility = true;
			}
			else
			{
				this.container.Entered +=
					delegate
					{
						this.commandBar.Visibility = true;
						this.commandButton.Visibility = true;
					};

				this.container.Exited +=
					delegate
					{
						this.commandBar.Visibility = false;
						this.commandButton.Visibility = false;
					};
			}

			this.commandButton.Clicked +=
				delegate
				{
					this.workspace.OpenChartViewWindow ();
				};

			this.CreateGraphTypeButtons ();
			
			this.commandBar.SelectedItemChanged += (sender, e) => this.GraphType = this.commandBar.SelectedItem;
		}

		public void Refresh()
		{
			this.Refresh (this.document);
		}

		public void Refresh(GraphDocument document)
		{
			this.document = document;

			if (this.document == null)
			{
				return;
			}

			var renderer = this.CreateRenderer ();

			if (renderer == null)
			{
				this.chartView.Renderer   = null;
				this.captionView.Captions = null;
			}
			else
			{
				List<ChartSeries> series = new List<ChartSeries> (this.GetDocumentChartSeries ());

				renderer.Clear ();
				renderer.ChartSeriesRenderingMode = this.StackValues ? ChartSeriesRenderingMode.Stacked : ChartSeriesRenderingMode.Separate;
				renderer.DefineValueLabels (this.document.ChartColumnLabels);
				renderer.CollectRange (series);
				renderer.UpdateCaptions (series);
				renderer.AlwaysIncludeZero = true;

				this.chartView.Renderer = renderer;
				this.captionView.Captions = renderer.Captions;
				this.captionView.Captions.LayoutMode = ContainerLayoutMode.VerticalFlow;
				this.captionView.Parent.PreferredSize = this.captionView.Captions.GetCaptionLayoutSize (new Size (240, 600)) + this.captionView.Parent.Padding.Size;
			}

			this.chartView.Invalidate ();
			this.captionView.Invalidate ();
		}

		private void CreateGraphTypeButtons()
		{
			foreach (var command in this.GetGraphTypeCommands ())
			{
				this.commandBar.Items.Add (command);
				this.commandContext.GetCommandState (command).Enable = true;
			}

			this.commandBar.ItemSize = new Size (64, 40);
			this.commandBar.SelectedItem = this.graphType;
		}

		private CommandContext CreateCommandContext(Widget container)
		{
			var context = new CommandContext ("ChartViewController");

			CommandContext.SetContext (container, context);
			ChartViewController.SetChartViewController (context, this);

			return context;
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
					VisibleLabels = this.IsStandalone,
					VisibleTicks = true,
				};

				renderer.AddStyle (this.GetDocumentChartSeriesColorStyle ());
				renderer.AddAdorner (adorner);
			}

			return renderer;
		}

		private ColorStyle GetDocumentChartSeriesColorStyle()
		{
			ColorStyle style = new ColorStyle (this.colorStyle.Name);

			foreach (int index in this.document.OutputSeries.Select (x => x.ColorIndex))
			{
				style.Add (this.colorStyle[index]);
			}
			
			return style;
		}

		private IEnumerable<Command> GetGraphTypeCommands()
		{
			yield return Res.Commands.GraphType.UseLineChart;
			yield return Res.Commands.GraphType.UseBarChartVertical;
//-			yield return Res.Commands.GraphType.UseBarChartHorizontal;
		}


		private IEnumerable<ChartSeries> GetDocumentChartSeries()
		{
			bool accumulateValues = false;
			foreach (var series in this.document.OutputSeries.Select (x => x.ChartSeries))
			{
				yield return new ChartSeries (series.Label, accumulateValues ? ChartViewController.Accumulate (series.Values) : series.Values);
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


		private class CommandController
		{
			public CommandController(CommandDispatcher dispatcher)
			{
				dispatcher.RegisterController (this);
			}

			[Command (Res.CommandIds.GraphType.UseLineChart)]
			[Command (Res.CommandIds.GraphType.UseBarChartVertical)]
			[Command (Res.CommandIds.GraphType.UseBarChartHorizontal)]
			private void GraphTypeCommand(CommandDispatcher sender, CommandEventArgs e)
			{
				var controller = ChartViewController.GetChartViewController (e.CommandContext);
				controller.commandBar.SelectedItem = e.Command;
			}
		}


		public static void SetChartViewController(DependencyObject o, ChartViewController value)
		{
			o.SetValue (ChartViewController.ChartViewControllerProperty, value);
		}

		public static void ClearChartViewController(DependencyObject o)
		{
			o.ClearValue (ChartViewController.ChartViewControllerProperty);
		}

		public static ChartViewController GetChartViewController(DependencyObject o)
		{
			return o.GetValue (ChartViewController.ChartViewControllerProperty) as ChartViewController;
		}





		public static readonly DependencyProperty ChartViewControllerProperty = DependencyProperty.RegisterAttached ("ChartViewController", typeof (ChartViewController), typeof (ChartViewController), new DependencyPropertyMetadata ());
		private static CommandController		commandController;

		private CommandController				localController;
		private readonly GraphApplication		application;
		private readonly WorkspaceController	workspace;
		private CommandContext					commandContext;
		private CommandSelectionBar				commandBar;
		private Button							commandButton;
		private Widget							container;
		private ChartView						chartView;
		private CaptionView						captionView;
		private GraphDocument					document;
		private Command							graphType;
		private ColorStyle						colorStyle;

	}
}
