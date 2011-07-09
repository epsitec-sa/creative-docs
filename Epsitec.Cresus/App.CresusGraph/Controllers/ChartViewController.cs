//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Graph.Data;
using Epsitec.Common.Graph.Styles;
using Epsitec.Common.Graph.Widgets;
using Epsitec.Common.Printing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.UI;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Graph.Controllers;
using Epsitec.Cresus.Graph.Widgets;
using Epsitec.Common.Graph.Renderers;

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
				if (this.ChartSnapshot == null)
				{
					return this.graphType;
				}
				else
				{
					return this.ChartSnapshot.GraphType;
				}
			}
			set
			{
				if (this.ChartSnapshot == null)
				{
					if (this.graphType != value)
					{
						this.graphType = value;
						this.Refresh ();
					}
				}
				else
				{
					if (this.ChartSnapshot.GraphType != value)
                    {
						this.ChartSnapshot.GraphType = value;
						this.Refresh ();
                    }
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

		public int Index
		{
			get;
			set;
		}

		public GraphDocument Document
		{
			get
			{
				return this.workspace.Document;
			}
		}

		public GraphChartSnapshot ChartSnapshot
		{
			get;
			set;
		}

		
		public void SetupUI(Widget container)
		{
			this.container = container;

			if (this.IsStandalone)
			{
				var dispatcher = new CommandDispatcher ("chart view", CommandDispatcherLevel.Secondary, CommandDispatcherOptions.AutoForwardCommands);
				
				CommandDispatcher.SetDispatcher (this.container.Window, dispatcher);
				
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

            this.seriesCaptionsView = new SeriesCaptionsView ()
            {
                Anchor = AnchorStyles.All,
                Parent = chartSurface,
                Visibility = this.IsStandalone
            };

            // Handling the mouse click, passing it to the ChartView
            this.seriesCaptionsView.Clicked += this.chartView.OnClicked;

            // Loading a snapshot with available options
            if (this.ChartSnapshot != null)
            {
                seriesCaptionsView.Visibility = this.ChartSnapshot.ChartOptions.ShowSeriesCaptions;
            }

			var summaryCaptionsPalette = new AnchoredPalette ()
			{
				Anchor = AnchorStyles.TopRight,
				Margins = new Margins (0, 4, 4, 0),
				Parent = chartSurface,
				BackColor = Color.FromBrightness (1),
				Padding = new Margins (4, 4, 2, 2),
                Visibility = this.IsStandalone
            };

            // Loading a snapshot with available options
            if (this.ChartSnapshot != null)
            {
                summaryCaptionsPalette.Margins = this.ChartSnapshot.ChartOptions.SummaryCaptionsPosition;
                summaryCaptionsPalette.Visibility = this.ChartSnapshot.ChartOptions.ShowSummaryCaptions;
            }

			this.summaryCaptionView = new SummaryCaptionsView ()
			{
				Dock = DockStyle.Fill,
				Parent = summaryCaptionsPalette
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

			if (this.IsStandalone)
			{
				this.commandBar.Dock = DockStyle.Top;
				this.commandBar.Visibility = true;
			}
			else
			{
				this.commandButton = new Button ()
				{
					Anchor = AnchorStyles.All,
					HorizontalAlignment = HorizontalAlignment.Center,
					VerticalAlignment = VerticalAlignment.Center,
					Parent = this.container,
					Name = "command button",
					Visibility = false,
					Text = "Afficher dans une fenêtre séparée",
					PreferredWidth = 120,
					PreferredHeight = 40,
				};

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
				
				this.commandButton.Clicked +=
					delegate
					{
						var window = this.workspace.FindLiveChartViewWindow ()
									 ?? this.workspace.CreateChartViewWindow (null);

						window.Show ();
						window.MakeActive ();

						this.application.AsyncSaveApplicationState ();
					};
			}

			if (this.IsStandalone)
			{
				this.CreateLeftToolButtons ();
                this.CreateGraphTypeButtons();
                this.chartOptionsController = new ChartOptionsController (this.commandBar, summaryCaptionsPalette, seriesCaptionsView);
                this.CreateSnapshotButton ();

                // Copy ChartOptions if available
                if (this.ChartSnapshot != null)
                {
                    this.chartOptionsController.ChartOptions = this.ChartSnapshot.ChartOptions;
                }

			}
			else
			{
				this.CreateGraphTypeButtons ();
			}
			
			this.commandBar.SelectedItemChanged += (sender, e) => this.GraphType = this.commandBar.SelectedItem;

            this.seriesDetection = new SeriesDetectionController (chartView, summaryCaptionView, seriesCaptionsView);
            
            // Tells the renderer that the mouse is over the graph
            this.seriesDetection.HoverIndexChanged += (sender, e) => this.chartView.HoverIndexChanged(e.OldValue, e.NewValue);
		}

		public void Refresh(GraphDocument document)
		{
			this.document = document;
			this.Refresh ();
		}

		public void ExportImage(string path)
		{
			string ext = System.IO.Path.GetExtension (path).ToLowerInvariant ();

			switch (ext)
			{
				case ".emf":
					this.SaveMetafile (path);
					break;

				case ".bmp":
				case ".png":
				case ".gif":
					this.SaveBitmap (path);
					break;

				default:
					throw new System.ArgumentException ("Unsupported file extension");
			}
		}

		
		public void SaveMetafile(string path)
		{
			if (string.IsNullOrEmpty (path))
			{
				this.RenderToPort ((painter, dx, dy) => PrintPort.PrintToClipboardMetafile (painter, (int) dx, (int) dy));
			}
			else
			{
				this.RenderToPort ((painter, dx, dy) => PrintPort.PrintToMetafile (painter, path, (int) dx, (int) dy));
			}
		}

		public void SaveBitmap(string path)
		{
			this.RenderToPort ((painter, dx, dy) => PrintPort.PrintToBitmap (painter, path, (int) dx, (int) dy));
		}

		public void Print()
		{
			if (ChartViewController.printDialog == null)
			{
				ChartViewController.printDialog = new Epsitec.Common.Dialogs.PrintDialog ();
			}

			var dialog = ChartViewController.printDialog;
			
			dialog.Owner = this.container.Window;
			dialog.AllowFromPageToPage = false;
			dialog.AllowSelectedPages = false;
			dialog.Document.PrinterSettings.FromPage = 1;
			dialog.Document.PrinterSettings.ToPage = 1;
			dialog.OpenDialog ();

			if (dialog.Result == Epsitec.Common.Dialogs.DialogResult.Accept)
			{
				var print = dialog.Document;
				this.RenderToPort ((painter, dx, dy) => PrintPort.PrintFittedSinglePage (painter, print, (int) dx, (int) dy));
			}
		}
		
		
		private void RenderToPort(System.Action<System.Action<IPaintPort>, double, double> callback)
		{
			double dx = this.chartView.ActualWidth;
			double dy = this.chartView.ActualHeight;

			var chartRect = new Rectangle (this.chartView.Padding.Left, this.chartView.Padding.Bottom, dx - this.chartView.Padding.Width, dy - this.chartView.Padding.Height);
			var captionRect = this.summaryCaptionView.Parent.MapClientToParent (this.summaryCaptionView.ActualBounds);
			var captionFrame = Rectangle.Inflate (captionRect, 4, 2);

			System.Action<IPaintPort> painter =
				port =>
				{
                    this.chartView.Renderer.Render (port, chartRect, chartRect);

					using (var p = new Path ())
					{
						p.AppendRectangle (captionFrame);
						port.Color = Color.FromBrightness (1);
						port.PaintSurface (p);
					}

					using (var p = new Path ())
					{
						p.AppendRectangle (Rectangle.Deflate (captionFrame, 0.5, 0.5));
						port.Color = Color.FromBrightness (0);
						port.LineWidth = 1.0;
						port.LineJoin = JoinStyle.Miter;
						port.PaintOutline (p);
					}
					
					this.summaryCaptionView.Captions.Render (port, captionRect);
				};

			callback (painter, dx, dy);
		}
		
		
		private void CreateLeftToolButtons()
		{
			foreach (var command in ChartViewController.GetToolCommands ())
			{
				var button = new MetaButton ()
				{
					Dock = DockStyle.Stacked,
					ButtonClass = ButtonClass.FlatButton,
					PreferredSize = new Size (40, 40),
					CommandObject = command,
					Margins = new Margins (0, 0, 1, 1),
					Parent = this.commandBar,
				};
			}

			new Separator ()
			{
				IsVerticalLine = true,
				Dock = DockStyle.Stacked,
				PreferredWidth = 1,
				Parent = this.commandBar,
				Margins = new Margins (2, 2, 0, 0),
			};
		}

        private void CreateSnapshotButton()
		{
			new Separator ()
			{
				IsVerticalLine = true,
				Dock = DockStyle.Stacked,
				PreferredWidth = 1,
				Parent = this.commandBar,
				Margins = new Margins (2, 2, 0, 0),
			};
			
			var frame = new FrameBox ()
			{
				Dock = DockStyle.Stacked,
				Parent = this.commandBar,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
			};

			var snapshotButton = new MetaButton ()
			{
				Dock = DockStyle.Left,
				PreferredWidth = 130,
				IconUri = "manifest:Epsitec.Cresus.Graph.Images.TakeSnapshot.icon",
				Text = "Créer un cliché instantané",
				ButtonClass = ButtonClass.FlatButton,
				Parent = frame,
				Margins = new Margins (0, 0, 1, 1),
				Padding = new Margins (40, 4, 0, 0),
				Visibility = this.ChartSnapshot == null,
			};

			snapshotButton.Clicked +=
				(sender, e) =>
				{
                    var snapshot  = GraphChartSnapshot.FromDocument (this.document, this.GraphType, this.chartOptionsController.ChartOptions, this.rendererOptions);
					var newWindow = this.workspace.CreateChartViewWindow (snapshot);
					var oldWindow = this.container.Window;
					var placement = oldWindow.WindowPlacement;

					this.document.ChartSnapshots.Add (snapshot);
					this.workspace.RefreshSnapshots ();

					newWindow.WindowPlacement = new WindowPlacement (placement.Bounds, placement.IsFullScreen, placement.IsMinimized, true);
					newWindow.Show ();
					oldWindow.Close ();

					this.document.NotifyNeedsSave (true);
                };
        }

		private void CreateGraphTypeButtons()
		{
			foreach (var command in ChartViewController.GetGraphTypeCommands ())
			{
				this.commandBar.Items.Add (command);
				this.commandContext.GetCommandState (command).Enable = true;
			}

			this.commandBar.ItemSize = new Size (64, 40);
			this.commandBar.SelectedItem = this.GraphType;
		}

		private CommandContext CreateCommandContext(Widget container)
		{
			var context = new CommandContext ("ChartViewController");

			CommandContext.SetContext (container, context);
			ChartViewController.SetChartViewController (context, this);

			return context;
		}


		private void Refresh()
		{
			if (this.document == null)
			{
				return;
			}

            // Change the tab index
            this.commandBar.SelectedItem = this.GraphType;

			var snapshot = this.ChartSnapshot ?? GraphChartSnapshot.FromDocument (this.document, this.graphType);
			var renderer = snapshot.CreateAndSetupRenderer (this.IsStandalone);

			if (renderer == null)
			{
				this.chartView.Renderer   = null;
				this.summaryCaptionView.Captions = null;
                this.seriesCaptionsView.Renderer = null;
			}
			else
			{
				this.chartView.Renderer = renderer;
				this.summaryCaptionView.Captions = renderer.Captions;
				this.summaryCaptionView.Captions.LayoutMode = ContainerLayoutMode.VerticalFlow;
				this.summaryCaptionView.Parent.PreferredSize = this.summaryCaptionView.Captions.GetCaptionLayoutSize (new Size (240, 600)) + this.summaryCaptionView.Parent.Padding.Size;
                this.seriesCaptionsView.Renderer = renderer;
                this.rendererOptions = snapshot.RendererOptions;
			}

			this.chartView.Invalidate ();
			this.summaryCaptionView.Invalidate ();
            this.seriesCaptionsView.Invalidate ();
		}

		private IEnumerable<ChartSeries> GetDocumentChartSeries()
		{
			bool accumulateValues = false;
			
			foreach (var series in this.document.OutputSeries.Select (x => x.ChartSeries))
			{
				yield return new ChartSeries (series.Label, accumulateValues ? ChartViewController.Accumulate (series.Values) : series.Values);
			}
		}

		private IEnumerable<string> GetDocumentChartColumnLabels()
		{
			return this.document.ChartColumnLabels;
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

		private static IEnumerable<Command> GetGraphTypeCommands()
		{
			yield return Res.Commands.GraphType.UseLineChart;
			yield return Res.Commands.GraphType.UseBarChartVertical;
//-			yield return Res.Commands.GraphType.UseBarChartHorizontal;
			yield return Res.Commands.GraphType.UsePieChart;
//-			yield return Res.Commands.GraphType.UseGeoChart;
		}


		#region CommandController Class

		private class CommandController
		{
			public CommandController(CommandDispatcher dispatcher)
			{
				dispatcher.RegisterController (this);
			}

			[Command (Res.CommandIds.GraphType.UseLineChart)]
			[Command (Res.CommandIds.GraphType.UseBarChartVertical)]
            [Command (Res.CommandIds.GraphType.UseBarChartHorizontal)]
            [Command (Res.CommandIds.GraphType.UsePieChart)]
            [Command (Res.CommandIds.GraphType.UseGeoChart)]
			private void GraphTypeCommand(CommandDispatcher sender, CommandEventArgs e)
			{
				var controller = ChartViewController.GetChartViewController (e.CommandContext);
				controller.commandBar.SelectedItem = e.Command;
			}
		}

		#endregion


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


		private static IEnumerable<Command> GetToolCommands()
		{
			yield return ApplicationCommands.Copy;
			yield return Res.Commands.File.ExportImage;
			yield return ApplicationCommands.Print;
		}


		public static readonly DependencyProperty ChartViewControllerProperty = DependencyProperty.RegisterAttached ("ChartViewController", typeof (ChartViewController), typeof (ChartViewController), new DependencyPropertyMetadata ());
		private static CommandController		commandController;
		private static Epsitec.Common.Dialogs.PrintDialog printDialog;

        private ChartOptionsController          chartOptionsController;
        private AbstractRenderer.IChartRendererOptions rendererOptions;
        private SeriesDetectionController       seriesDetection;
        private CommandController				localController;
		private readonly GraphApplication		application;
		private readonly WorkspaceController	workspace;
		private CommandContext					commandContext;
		private CommandSelectionBar				commandBar;
		private Button							commandButton;
		private Widget							container;
		private ChartView						chartView;
		private SummaryCaptionsView				summaryCaptionView;
        private SeriesCaptionsView              seriesCaptionsView;
		private GraphDocument					document;
		private Command							graphType;
		private ColorStyle						colorStyle;
	}
}
