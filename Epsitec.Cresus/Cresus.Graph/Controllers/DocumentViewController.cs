//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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

[assembly: DependencyClass (typeof (DocumentViewController))]

namespace Epsitec.Cresus.Graph.Controllers
{
	/// <summary>
	/// The <c>DocumentViewController</c> class creates and manages one view of a document,
	/// usually inside a tab page.
	/// </summary>
	internal sealed class DocumentViewController : DependencyObject, System.IDisposable
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

			var lineChartRenderer = new LineChartRenderer ();

			lineChartRenderer.AddStyle (new Epsitec.Common.Graph.Styles.ColorStyle ("line-color") { "Red", "DeepPink", "Coral", "Tomato", "SkyBlue", "RoyalBlue", "DarkBlue", "Green", "PaleGreen", "Lime", "Yellow", "Wheat" });
			lineChartRenderer.AddAdorner (new Epsitec.Common.Graph.Adorners.CoordinateAxisAdorner ());

			Epsitec.Common.Types.Binding binding = new Epsitec.Common.Types.Binding (Epsitec.Common.Types.BindingMode.OneTime, document);
			Epsitec.Common.Types.DataObject.SetDataContext (container, binding);

			var context = new CommandContext ("DocumentViewController");
			
			CommandContext.SetContext (container, context);
			DocumentViewController.SetDocumentViewController (context, this);

			var frame = new FrameBox ()
			{
				Dock = DockStyle.Fill,
				Parent = container
			};

			this.commandBar = new CommandSelectionBar ()
			{
				Dock = DockStyle.Top,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				Parent = container,
				BackColor = container.BackColor
			};

			this.CreateGraphTypeButtons (context);

			var optionsContainer = new FrameBox ()
			{
				Dock = DockStyle.StackFill,
				Parent = this.commandBar
			};

			var optionsFrame = new FrameBox ()
			{
				Dock = DockStyle.Right,
				Padding = new Margins (0, 2, 2, 2),
				Parent = optionsContainer
			};

			this.accumulateValuesCheckButton = new CheckButton ()
			{
				Anchor = AnchorStyles.TopLeft,
				Text = "accumulation des valeurs",
				Margins = new Margins (0, 0, 0, 0),
				Parent = optionsFrame
			};

			this.accumulateValuesCheckButton.PreferredSize = this.accumulateValuesCheckButton.GetBestFitSize ();
			this.accumulateValuesCheckButton.ActiveStateChanged += sender => this.AccumulateValues = (this.accumulateValuesCheckButton.ActiveState == ActiveState.Yes);

			

			this.chartView = new ChartView ()
			{
				Dock = DockStyle.Fill,
				Parent = frame,
				Renderer = lineChartRenderer
			};

			this.captionView = new CaptionView ()
			{
				Parent = frame,
				Padding = new Margins(4, 4, 2, 2),
				PreferredWidth = 160,
				PreferredHeight = 80,
				Captions = lineChartRenderer.Captions
			};
			
			this.splitter = new AutoSplitter ()
			{
				Parent = frame
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
				Parent = frame
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

		private void CreateGraphTypeButtons(CommandContext context)
		{
			foreach (var command in this.GetGraphTypeCommands ())
			{
				this.commandBar.Items.Add (command);
				context.GetCommandState (command).Enable = true;
			}
			
			this.commandBar.ItemSize = new Size (64, 40);
			this.commandBar.SelectedItem = Res.Commands.GraphType.UseLineChart;
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
		
		
		public void Refresh()
		{
			if ((this.document != null) &&
				(this.document.DataSet != null) &&
				(this.document.DataSet.DataTable != null))
			{
				var renderer = this.chartView.Renderer;

				List<ChartSeries> series = new List<ChartSeries> (this.GetDocumentChartSeries ());
				
				renderer.Clear ();
				renderer.DefineValueLabels (this.document.DataSet.DataTable.ColumnLabels);
				renderer.CollectRange (series);
				renderer.UpdateCaptions (series);
				
				this.chartView.Invalidate ();
				this.captionView.Invalidate ();
			}
		}


		public XElement SaveSettings(XElement xml)
		{
			xml.Add (new XAttribute ("accumulateValues", this.accumulateValues ? "yes" : "no"));

			return xml;
		}

		public void RestoreSettings(XElement xml)
		{
			string accumulateValues = (string) xml.Attribute ("accumulateValues");

			this.AccumulateValues = (accumulateValues == "yes");
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


		#region IDisposable Members

		public void Dispose()
		{
			this.commandBar.Dispose ();
			this.chartView.Dispose ();
			this.splitter.Dispose ();
			this.captionView.Dispose ();
			this.accumulateValuesCheckButton.Dispose ();
//-			this.detectionController.Dispose ();

			if (this.disposeContainerCallback != null)
			{
				this.disposeContainerCallback (this);
			}
		}

		#endregion


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
		private readonly CommandSelectionBar	commandBar;
		private readonly ChartView				chartView;
		private readonly AutoSplitter			splitter;
		private readonly CaptionView			captionView;
		private readonly CheckButton			accumulateValuesCheckButton;
		private readonly SeriesDetectionController detectionController;
		private readonly System.Action<DocumentViewController> showContainerCallback;
		private readonly System.Action<DocumentViewController> disposeContainerCallback;

		private bool							accumulateValues;
		private ContainerLayoutMode				layoutMode;
	}
}
