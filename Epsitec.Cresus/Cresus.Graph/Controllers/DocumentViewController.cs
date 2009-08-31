﻿//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Graph.Renderers;
using Epsitec.Common.Graph.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Graph.Controllers;
using Epsitec.Cresus.Graph.Widgets;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.UI;
using Epsitec.Common.Graph.Data;
using System.Xml.Linq;
using Epsitec.Common.Graph;

[assembly: DependencyClass (typeof (DocumentViewController))]

namespace Epsitec.Cresus.Graph.Controllers
{
	/// <summary>
	/// The <c>DocumentViewController</c> class creates and manages one view of a document,
	/// usually inside a tab page.
	/// </summary>
	internal sealed class DocumentViewController : DependencyObject
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DocumentViewController"/> class.
		/// </summary>
		/// <param name="container">The container.</param>
		/// <param name="document">The document.</param>
		/// <param name="showContainer">The callback used to make the container visible.</param>
		public DocumentViewController(Widget container, GraphDocument document, System.Action<DocumentViewController> showContainer, System.Action<DocumentViewController> disposeContainer)
		{
			if (DocumentViewController.commandController == null)
			{
				DocumentViewController.commandController = new CommandController (GraphProgram.Application.CommandDispatcher);
			}

			this.document = document;
			this.showContainerCallback = showContainer;
			this.disposeContainerCallback = disposeContainer;
			this.graphType = Res.Commands.GraphType.UseLineChart;

			Epsitec.Common.Types.Binding binding = new Epsitec.Common.Types.Binding (Epsitec.Common.Types.BindingMode.OneTime, document);
			Epsitec.Common.Types.DataObject.SetDataContext (container, binding);

			this.commandContext = this.CreateCommandContext (container);

			var chartFrame = new FrameBox ()
			{
				Dock = DockStyle.Fill,
				Parent = container,
				Name = "Chart"
			};

			this.commandBar = new CommandSelectionBar ()
			{
				Dock = DockStyle.Top,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				Parent = container,
				BackColor = container.BackColor,
				Name = "CommandBar"
			};

			this.CreateGraphTypeButtons ();

			var optionsContainer = new FrameBox ()
			{
				Dock = DockStyle.StackFill,
				Parent = this.commandBar
			};

			var optionsFrame = new FrameBox ()
			{
				Dock = DockStyle.Right,
				Padding = new Margins (0, 2, 2, 2),
				Parent = optionsContainer,
				Name = "Options"
			};

			this.accumulateValuesCheckButton = new CheckButton ()
			{
				Dock = DockStyle.Top,
				CaptionId = Res.CaptionIds.DocumentView.Options.AccumulateValues,
				Parent = optionsFrame
			};

			this.stackValuesCheckButton = new CheckButton ()
			{
				Dock = DockStyle.Top,
				Text = "empile",
				Parent = optionsFrame
			};

			this.accumulateValuesCheckButton.UpdatePreferredSize ();
			this.stackValuesCheckButton.UpdatePreferredSize ();

			this.commandBar.SelectedItemChanged += (sender, e) => this.GraphType = this.commandBar.SelectedItem;
			this.accumulateValuesCheckButton.ActiveStateChanged += sender => this.AccumulateValues = (this.accumulateValuesCheckButton.ActiveState == ActiveState.Yes);
			this.stackValuesCheckButton.ActiveStateChanged += sender => this.StackValues = (this.stackValuesCheckButton.ActiveState == ActiveState.Yes);

			

			this.chartView = new ChartView ()
			{
				Dock = DockStyle.Fill,
				Parent = chartFrame
			};

			this.captionView = new CaptionView ()
			{
				Parent = chartFrame,
				Padding = new Margins(4, 4, 2, 2),
				PreferredWidth = 160,
				PreferredHeight = 80
			};
			
			this.splitter = new AutoSplitter ()
			{
				Parent = chartFrame
			};

			this.LayoutMode = ContainerLayoutMode.HorizontalFlow;
			
			this.detectionController= new SeriesDetectionController (this.chartView, this.captionView);

			var button = new Button ()
			{
				Anchor = AnchorStyles.TopRight,
				Margins = new Margins (0, 4, 4, 0),
				Text = "/",
				PreferredWidth = 20,
				PreferredHeight = 20,
				Parent = chartFrame
			};

			button.Clicked +=
				delegate
				{
					if (this.LayoutMode == ContainerLayoutMode.VerticalFlow)
					{
						this.LayoutMode = ContainerLayoutMode.HorizontalFlow;
					}
					else
					{
						this.LayoutMode = ContainerLayoutMode.VerticalFlow;
					}
				};
		}

		private CommandContext CreateCommandContext(Widget container)
		{
			var context = new CommandContext ("DocumentViewController");

			CommandContext.SetContext (container, context);
			DocumentViewController.SetDocumentViewController (context, this);

			return context;
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

		private IEnumerable<Command> GetGraphTypeCommands()
		{
			yield return Res.Commands.GraphType.UseLineChart;
			yield return Res.Commands.GraphType.UseBarChartVertical;
			yield return Res.Commands.GraphType.UseBarChartHorizontal;
		}


		public ContainerLayoutMode LayoutMode
		{
			get
			{
				return this.layoutMode;
			}
			set
			{
				if (this.layoutMode != value)
				{
					this.layoutMode = value;

					switch (this.layoutMode)
					{
						case ContainerLayoutMode.HorizontalFlow:
							this.splitter.Dock = DockStyle.Right;
							this.captionView.Dock = DockStyle.Right;
							break;

						case ContainerLayoutMode.VerticalFlow:
							this.splitter.Dock = DockStyle.Bottom;
							this.captionView.Dock = DockStyle.Bottom;
							break;
					}
				}
			}
		}

		public bool AccumulateValues
		{
			get
			{
				return this.accumulateValues;
			}
			set
			{
				if (this.accumulateValues != value)
				{
					this.accumulateValues = value;
					this.accumulateValuesCheckButton.ActiveState = value ? ActiveState.Yes : ActiveState.No;
					this.Refresh ();
				}
			}
		}

		public bool StackValues
		{
			get
			{
				return this.stackValues;
			}
			set
			{
				if (this.stackValues != value)
				{
					this.stackValues = value;
					this.stackValuesCheckButton.ActiveState = value ? ActiveState.Yes : ActiveState.No;
					this.Refresh ();
				}
			}
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
					this.commandBar.SelectedItem = this.graphType;
					this.Refresh ();
				}
			}
		}
		
		
		public void Refresh()
		{
			if ((this.document != null) &&
				(this.document.DataSet != null) &&
				(this.document.DataSet.DataTable != null))
			{
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
					renderer.DefineValueLabels (this.document.DataSet.DataTable.ColumnLabels);
					renderer.CollectRange (series);
					renderer.UpdateCaptions (series);
					renderer.AlwaysIncludeZero = true;

					this.chartView.Renderer = renderer;
					this.captionView.Captions = renderer.Captions;
				}

				this.chartView.Invalidate ();
				this.captionView.Invalidate ();
			}
		}


		public XElement SaveSettings(XElement xml)
		{
			xml.Add (new XAttribute ("accumulateValues", this.accumulateValues ? "yes" : "no"));
			xml.Add (new XAttribute ("stackValues", this.stackValues ? "yes" : "no"));
			xml.Add (new XAttribute ("graphType", this.graphType.CommandId));

			return xml;
		}

		public void RestoreSettings(XElement xml)
		{
			string accumulateValues = (string) xml.Attribute ("accumulateValues");
			string stackValues = (string) xml.Attribute ("stackValues");
			string graphTypeId = (string) xml.Attribute ("graphType");

			this.GraphType        = Command.Find (graphTypeId);
			this.AccumulateValues = (accumulateValues == "yes");
			this.StackValues      = (stackValues == "yes");
		}


		private AbstractRenderer CreateRenderer()
		{
			AbstractRenderer renderer = null;

			if (this.GraphType == Res.Commands.GraphType.UseLineChart)
			{
				renderer = new LineChartRenderer ()
				{
					SurfaceAlpha = this.StackValues ? 1.0 : 0.0
				};
			}
			else if (this.GraphType == Res.Commands.GraphType.UseBarChartVertical)
			{
				renderer = new BarChartRenderer ();
			}

			if (renderer != null)
			{
				renderer.AddStyle (new Epsitec.Common.Graph.Styles.ColorStyle ("line-color") { "Red", "DeepPink", "Coral", "Tomato", "SkyBlue", "RoyalBlue", "DarkBlue", "Green", "PaleGreen", "Lime", "Yellow", "Wheat" });
				renderer.AddAdorner (new Epsitec.Common.Graph.Adorners.CoordinateAxisAdorner ());
			}

			return renderer;
		}

		
		private IEnumerable<ChartSeries> GetDocumentChartSeries()
		{
			foreach (var series in this.document.ChartSeries)
			{
				yield return new ChartSeries (series.Label, this.accumulateValues ? DocumentViewController.Accumulate (series.Values) : series.Values);
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

		public void MakeVisible()
		{
			if (this.showContainerCallback != null)
			{
				this.showContainerCallback (this);
			}
		}


		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.commandBar.Dispose ();
				this.chartView.Dispose ();
				this.splitter.Dispose ();
				this.captionView.Dispose ();
				this.accumulateValuesCheckButton.Dispose ();
				this.stackValuesCheckButton.Dispose ();
//-				this.detectionController.Dispose ();

				if (this.disposeContainerCallback != null)
				{
					this.disposeContainerCallback (this);
				}
			}

			base.Dispose (disposing);
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
				var controller = DocumentViewController.GetDocumentViewController (e.CommandContext);
				controller.commandBar.SelectedItem = e.Command;
			}
		}

		
		public static void SetDocumentViewController(DependencyObject o, DocumentViewController value)
		{
			o.SetValue (DocumentViewController.DocumentViewControllerProperty, value);
		}

		public static void ClearDocumentViewController(DependencyObject o)
		{
			o.ClearValue (DocumentViewController.DocumentViewControllerProperty);
		}

		public static DocumentViewController GetDocumentViewController(DependencyObject o)
		{
			return o.GetValue (DocumentViewController.DocumentViewControllerProperty) as DocumentViewController;
		}

		public static readonly DependencyProperty DocumentViewControllerProperty = DependencyProperty.RegisterAttached ("DocumentViewController", typeof (DocumentViewController), typeof (DocumentViewController), new DependencyPropertyMetadata ());


		private static CommandController		commandController;

		private readonly GraphDocument			document;
		private readonly CommandContext			commandContext;
		private readonly CommandSelectionBar	commandBar;
		private readonly ChartView				chartView;
		private readonly AutoSplitter			splitter;
		private readonly CaptionView			captionView;
		private readonly CheckButton			accumulateValuesCheckButton;
		private readonly CheckButton			stackValuesCheckButton;
		private readonly SeriesDetectionController detectionController;
		private readonly System.Action<DocumentViewController> showContainerCallback;
		private readonly System.Action<DocumentViewController> disposeContainerCallback;

		private bool							accumulateValues;
		private bool							stackValues;
		private Command							graphType;
		private ContainerLayoutMode				layoutMode;
	}
}
