//	Copyright © 2009-2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Graph.Data;
using Epsitec.Common.Graph.Renderers;
using Epsitec.Common.Graph.Styles;
using Epsitec.Common.Graph.Widgets;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Graph.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Graph.Controllers
{
	internal sealed partial class WorkspaceController
	{
		public WorkspaceController(GraphApplication application)
		{
			this.application = application;
			this.colorStyle = GraphDocument.GetDefaultColorStyle ();
			
			this.inputItemsController = new ItemListController<MiniChartView> ()
			{
				ItemLayoutMode = ItemLayoutMode.Flow
			};

			this.outputItemsController = new ItemListController<MiniChartView> ()
			{
				ItemLayoutMode = ItemLayoutMode.Horizontal
			};

			this.groupsController = new GroupsController (this);
			this.snapshotsController = new SnapshotsController (this);
			this.cubeSelController = new DataCubeSelectionController (this);

			this.chartViewController = new ChartViewController (this.application, this)
			{
				GraphType = Res.Commands.GraphType.UseLineChart,
				ColorStyle = this.colorStyle,
			};

			this.floatingChartViews = new List<ChartViewController> ();
			
			this.viewToGroup = new Dictionary<long, GraphDataGroup> ();
			this.viewToSeries = new Dictionary<long, GraphDataSeries> ();
			this.hilites = new List<HiliteInfo> ();

			this.labelColorStyle = new ColorStyle ("labels")
			{
				Color.FromRgb (1.0, 1.0, 1.0),
				Color.FromRgb (0.8, 1.0, 0.8),
				Color.FromRgb (1.0, 0.8, 0.8),
				Color.FromRgb (0.8, 0.8, 1.0),
				Color.FromRgb (1.0, 0.8, 1.0),
			};
		}


		public ItemListController<MiniChartView> Outputs
		{
			get
			{
				return this.outputItemsController;
			}
		}

		public GroupsController Groups
		{
			get
			{
				return this.groupsController;
			}
		}

		public SnapshotsController Snapshots
		{
			get
			{
				return this.snapshotsController;
			}
		}

		public Widget ToolsFrame
		{
			get
			{
				return this.application.MainWindowController.ToolsFrame;
			}
		}

		public Widget WorkspaceFrame
		{
			get
			{
				return this.application.MainWindowController.WorkspaceFrame;
			}
		}
		
		public void SetupUI()
		{
			this.SetupToolsFrameUI ();
			this.SetupWorkspaceFrameUI ();
		}

		private void SetupToolsFrameUI()
		{
			var container  = this.ToolsFrame;

			var settingsFrame = new FrameBox ()
			{
				Dock = DockStyle.Fill,
				PreferredWidth = 400,
				Name = "settings",
//-				BackColor = Color.FromRgb (0.9, 1, 0.9),
				Parent = container,
			};

			new Separator ()
			{
				Dock = DockStyle.Left,
				PreferredWidth = 3,
				Name = "filters-sep",
				IsVerticalLine = true,
				Parent = settingsFrame,
			};

			var filterFrame = new FrameBox ()
			{
				Dock = DockStyle.Left,
				PreferredWidth = 100,
				Name = "filters",
				Parent = settingsFrame,
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
			};

			new Separator ()
			{
				Dock = DockStyle.Left,
				PreferredWidth = 3,
				IsVerticalLine = true,
				Parent = settingsFrame,
			};

			var sourceFrame = new FrameBox ()
			{
				Dock= DockStyle.Left,
				PreferredWidth = 160,
				Name = "sources",
				Parent = settingsFrame,
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
			};

			new Separator ()
			{
				Dock = DockStyle.Left,
				PreferredWidth = 3,
				Name = "sources-sep",
				IsVerticalLine = true,
				Parent = settingsFrame,
			};

			var licensing = GraphSerial.LicensingInfo;

			if (licensing >	LicensingInfo.ValidPiccolo)
			{
				var cubeButton = new DataCubeButton ()
				{
					Dock = DockStyle.Left,
					PreferredWidth = 100,
					Name = "data cube",
					Parent = settingsFrame,
				};

				var cubeFrame = new DataCubeFrame ()
				{
					Anchor = AnchorStyles.Top | AnchorStyles.Right,
					Parent = settingsFrame.RootParent,
					Visibility = false,
					FrameHeight = 300,
					FrameWidth = 400,
				};

				cubeButton.Clicked +=
					delegate
					{
						cubeButton.SetSelected (!cubeButton.IsSelected);
						cubeFrame.Visibility = cubeButton.IsSelected;
						cubeFrame.UpdateGeometry ();
					};

				new Separator ()
				{
					Dock = DockStyle.Left,
					PreferredWidth = 3,
					IsVerticalLine = true,
					Parent = settingsFrame,
				};

				this.cubeSelController.SetupUI (cubeFrame);
			}
			else
			{
				var comptaDate = GraphSerial.ComptaExpirationDate;

				var messageFrame = new FrameBox ()
				{
					Dock = DockStyle.Fill,
					MinWidth = 200,
					Name = "message",
					Parent = settingsFrame,
					BackColor = Color.FromBrightness (1),
				};

				if ((GraphSerial.HasGraphLicense == false) &&
					(comptaDate.HasValue))
				{
					var date = comptaDate.Value;
					
					date = date.AddMonths (1);
					date = date.AddDays (-1);

					var message1 = new StaticText ()
					{
						Parent = messageFrame,
						Dock = DockStyle.Top,
						Text = string.Format (Res.Strings.Message.FreePiccoloBecauseOfCompta.ToString (), date.ToShortDateString ()),
						PreferredHeight = 36,
						Margins = new Margins (4, 4, 0, 0),
					};

					messageFrame.MinWidth = 300;
				}

				var message2 = new StaticText ()
				{
					Parent = messageFrame,
					Dock = DockStyle.Fill,
					Text = Res.Strings.Message.MoreThanPiccolo.ToString (),
					Margins = new Margins (4, 4, 0, 0),
				};
			}
		}
		
		private void SetupWorkspaceFrameUI()
		{
			var container  = this.WorkspaceFrame;

			var topFrame = new FrameBox ()
			{
				Dock = DockStyle.Fill,
				Parent = container,
				Name = "top",
				PreferredHeight = 320,
				Padding = new Margins (0, 0, 1, 0),
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
			};
			
			//	Top half of the workspace : input frame & group frame
			
			var inputFrame = new FrameBox ()
			{
				Dock = DockStyle.Fill,
				Parent = topFrame,
				Name = "inputs",
				PreferredWidth = 480,
				BackColor = Color.FromBrightness (1),
			};

			bool groupVisibility = false;

#if DEBUG
			groupVisibility = true;
#endif

			var groupFrame = new FrameBox ()
			{
				Dock = DockStyle.Right,
				Parent = topFrame,
				Name = "groups",
				BackColor = Color.FromBrightness (1.0),
				PreferredWidth = 200,
				Padding = new Margins (0, 0, 1, 0),
				Visibility = groupVisibility,
			};

			var splitter1 = new VSplitter ()
			{
				Dock = DockStyle.Right,
				Parent = topFrame,
				PreferredWidth = 3,
				Visibility = groupVisibility,
			};

			//	Bottom half of the workspace : book with preview and output

			var bottomFrame = new FrameBox ()
			{
				Dock = DockStyle.Bottom,
				Parent = container,
				Name = "bottom",
				PreferredHeight = 264
			};

			var splitter2 = new HSplitter ()
			{
				Dock = DockStyle.Bottom,
				Parent = container,
				PreferredHeight = 3,
			};

			var outputBook = new TabBook ()
			{
				Dock = DockStyle.Fill,
				Parent = bottomFrame,
				Name = "book",
				Margins = new Margins (0, 0, 4, 0),
				InternalPadding = new Margins (0, 0, 1, 0),
			};

			//	Book contents :

			var outputPage = new TabPage ()
			{
				Name = "output",
				TabTitle = "Graphique",
				Parent = outputBook,
			};

			var snapshotsPage = new TabPage ()
			{
				Name = "snapshots",
				TabTitle = "Clichés",
				Parent = outputBook,
			};

#if false
			var newPage = new TabPage ()
			{
				Name = "page+",
				TabTitle = "+",
				Parent = outputBook,
			};
#endif

			this.outputPageFrame = new FrameBox ()
			{
				Dock = DockStyle.Fill,
				Name = "output page frame",
				Parent = outputPage,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
			};

			var snapshotsPageFrame = new FrameBox ()
			{
				Dock = DockStyle.Fill,
				Name = "snapshots frame",
				Parent = snapshotsPage,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
			};

			var previewFrame = new FrameBox ()
			{
				Dock = DockStyle.Left,
				Parent = this.outputPageFrame,
				Name = "preview",
				BackColor = Color.FromRgb (1, 0.85, 0.8),
				PreferredWidth = 260,
				Padding = new Margins (0, 1, 0, 0)
			};

			var splitter3 = new VSplitter ()
			{
				Dock = DockStyle.Left,
				Parent = this.outputPageFrame,
				PreferredWidth = 3,
			};

			var outputFrame = new FrameBox ()
			{
				Dock = DockStyle.Fill,
				Parent = this.outputPageFrame,
				Name = "outputs",
				PreferredHeight = 104
			};

			outputFrame.SizeChanged +=
				delegate
				{
					this.AdjustOutputItemsWidth ();
				};

			this.outputActiveInfo = new BalloonTip ()
			{
				Anchor = AnchorStyles.BottomLeft,
				Parent = outputFrame,
				Name = "output info",
				BackColor = Color.FromBrightness (1),
				Visibility = false,
				PreferredWidth = 120,
				PreferredHeight = 150,
				Padding = new Margins (5, 5, 17, 8),
			};

			this.outputColorPalette = new ColorPalette ()
			{
				Parent = this.outputActiveInfo,
				Dock = DockStyle.Fill,
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				OptionButtonVisibility = false,
				RowCount = 12,
				ColumnCount = 9,
				ContentAlignment = ContentAlignment.MiddleCenter,
				ColorCollection = new ColorCollection (WorkspaceController.GetColors ()),
			};

			Epsitec.Common.Widgets.Layouts.LayoutEngine.SetIgnoreMeasure (this.outputActiveInfo, true);
			
			this.outputColorPalette.PreferredSize = this.outputColorPalette.GetBestFitSize (11);
			this.outputColorPalette.ExportSelectedColor += sender => this.DefineOutputColor (this.outputColorPalette.SelectedColor.Basic);

			outputBook.ActivePage = outputPage;

			WorkspaceController.PatchTabBookPaintBackground (outputBook);
//-			WorkspaceController.PatchInputFramePaintBackground (inputFrame);
//-			WorkspaceController.PatchBottomFrameLeftPaintBackground (bottomFrameLeft);
			WorkspaceController.PatchPreviewFramePaintBackground (outputBook, previewFrame);

			this.inputItemsController.SetupUI (inputFrame);
			this.outputItemsController.SetupUI (outputFrame);
			this.groupsController.SetupUI (groupFrame);
			this.chartViewController.SetupUI (previewFrame);
			this.snapshotsController.SetupUI (snapshotsPageFrame);

			this.inputItemsHint = new StaticText ()
			{
				Anchor = AnchorStyles.All,
				Parent = topFrame,
				Visibility = false,
				Text = "<font size=\"120%\">Aucune donnée n'est visible pour l'instant.</font><br/>" +
				"Cochez les catégories que vous souhaitez voir apparaître ici.",
				BackColor = Color.FromBrightness (1),
				ContentAlignment = ContentAlignment.MiddleCenter,
				PreferredHeight = 40,
			};

			this.groupItemsHint = new StaticText ()
			{
				Anchor = AnchorStyles.All,
				Parent = groupFrame,
				Visibility = false,
				Text = "<font size=\"120%\">Groupes et calculs.</font><br/>" +
				"Glissez et empilez ici les éléments que vous souhaitez grouper.",
				ContentAlignment = ContentAlignment.MiddleCenter,
				BackColor = Color.FromBrightness (1),
				PreferredHeight = 40,
			};

			this.outputItemsHint = new StaticText ()
			{
				Anchor = AnchorStyles.All,
				Parent = outputPage,
				Visibility = false,
				BackColor = Color.FromBrightness (1),
				Text = "<font size=\"120%\">Graphique vide.</font><br/>" +
				"Cliquez (ou glissez ici) les éléments que vous souhaitez voir apparaître dans le graphique.",
				ContentAlignment = ContentAlignment.MiddleCenter,
				PreferredHeight = 40,
			};
		}

		private static IEnumerable<RichColor> GetColors()
		{
			foreach (int value in new int[] { 100, 75, 50 })
			{
				foreach (int saturation in new int[] { 100, 60, 30 })
				{
					for (int hue = 0; hue < 360; hue += 30)
					{
						yield return RichColor.FromHsv (hue, saturation / 100.0, value / 100.0);
					}
				}
			}
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

					e.Cancel = true;
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

					e.Cancel = true;
				};
			
			inputFrame.PaintForeground +=
				(sender, e) =>
				{
					var bounds   = inputFrame.Client.Bounds;
					var graphics = e.Graphics;
					var adorner  = Epsitec.Common.Widgets.Adorners.Factory.Active;

					double y2 = bounds.Top - 0.5;
					double x2 = bounds.Right - 0.5;

					graphics.AddLine (0.0, y2, x2, y2);
					graphics.RenderSolid (adorner.ColorBorder);

					e.Cancel = true;
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

					e.Cancel = true;
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

					graphics.AddFilledRectangle (bounds);
					graphics.RenderSolid (Color.FromBrightness (1.0));

					e.Cancel = true;
				};
		}

		
		public GraphDocument Document
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
				this.RefreshColors ();
				this.RefreshInputs ();
				this.RefreshOutputs ();
				this.RefreshGroups ();
				this.RefreshPreview ();
				this.RefreshFilters ();
				this.RefreshSources ();
				this.RefreshHilites ();
				
				this.application.Window.Root.Invalidate ();
			}
		}

		public void RefreshColors()
		{
			var doc = this.Document;

			if (doc != null)
			{
				this.colorStyle = doc.DefaultColorStyle;
				this.chartViewController.ColorStyle = this.colorStyle;
			}
		}
        
		public void RefreshGroups()
		{
			this.groupsController.Refresh ();
		}

		public void RefreshSnapshots()
		{
			this.snapshotsController.Refresh ();
		}

		public void RefreshFilters()
		{
			var container = this.ToolsFrame.FindChild ("filters", WidgetChildFindMode.Deep);
			var separator = this.ToolsFrame.FindChild ("filters-sep", WidgetChildFindMode.Deep);
			
			container.Children.Widgets.ForEach (x => x.Dispose ());

			System.Diagnostics.Debug.Assert (container.Children.Count == 0);

			WorkspaceController.CreateLabel (container, "Catégories");

			var document   = this.Document;
			var dataSource = document.ActiveDataSource;

			if (dataSource == null)
			{
				return;
			}

			var categories = dataSource.Categories;

			if ((categories.Count > 1) ||
				((categories.Count == 1) && !categories[0].IsGeneric))
			{
				foreach (var category in categories)
				{
					if (category.IsGeneric)
                    {
						continue;
                    }
					
					this.CreateFilterButton (container, category);
				}

				container.Visibility = true;
				separator.Visibility = true;
			}
			else
			{
				container.Visibility = false;
				separator.Visibility = false;
			}
		}

		public void RefreshSources()
		{
			var container  = this.ToolsFrame.FindChild ("sources", WidgetChildFindMode.Deep);
			int numSources = this.Document.DataSourceCount;

			container.Children.Widgets.ForEach (x => x.Dispose ());

			System.Diagnostics.Debug.Assert (container.Children.Count == 0);

			if (numSources < 5)
			{
				container.ContainerLayoutMode = ContainerLayoutMode.VerticalFlow;

				WorkspaceController.CreateLabel (container, "Sources de données");
			}
			else
			{
				container.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;
				
				var text = WorkspaceController.CreateLabel (container, "Sources");
				
				text.PreferredWidth = 16;

				text.PaintBackground +=
					(sender, e) =>
					{
						var bounds = text.Client.Bounds;
						var center = bounds.Center;
						e.Graphics.RotateTransformDeg (90, center.X, center.Y);
						e.Graphics.Color = text.TextLayout.DefaultRichColor.Basic;
						e.Graphics.PaintText (bounds.X, bounds.Y, bounds.Width, bounds.Height, text.Text, text.TextLayout.DefaultFont, text.TextLayout.DefaultFontSize, text.ContentAlignment);
						e.Cancel = true;
					};
			}

			var bottom = new FrameBox ()
			{
				Dock = DockStyle.StackFill,
				Parent = container,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
			};

			int columns = 1;
			int rows    = 1;
			var space   = 1.0;
			var containers = new List<FrameBox> ();
			
			if (numSources <= 2)
            {
				columns = 1;
				rows    = numSources;
            }
			else if (numSources <= 4)
			{
				columns = 2;
				rows    = 2;
			}
			else if (numSources <= 6)
            {
				columns = 2;
				rows    = 3;
			}
			else
			{
				columns = (numSources+3) / 4;
				rows    = 4;
				space   = 0.0;
			}

			for (int i = 0; i < System.Math.Max (2, columns); i++)
			{
				var frame = new FrameBox ()
				{
					Dock = DockStyle.Fill,
					ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
					Parent = bottom,
				};

				containers.Add (frame);
			}

			int index = 0;

			foreach (var source in this.Document.DataSources)
			{
				int c = index / rows;
				int r = index % rows;

				this.CreateSourceButton (containers[c], source, space);

				index++;
			}
		}

		public void RefreshInputs()
		{
			this.inputItemsController.Clear ();

			var document   = this.Document;
			var categories = new HashSet<GraphDataCategory> (document.ActiveFilterCategories);

			if (document.DataSeries == null)
			{
				return;
			}

			foreach (var item in document.DataSeries)
			{
				var category = item.GetCategory ();

				if (category.IsGeneric || categories.Contains (category))
				{
					this.inputItemsController.Add (this.CreateInputView (item));
				}
			}

			this.RefreshHints ();
		}

		public void RefreshOutputs()
		{
			this.outputItemsController.Clear ();

			int index = 0;

			foreach (var item in this.Document.OutputSeries)
			{
				this.outputItemsController.Add (this.CreateOutputView (item, index++));
			}
			
			this.outputPageFrame.Visibility = (this.outputItemsController.Count > 0);

			this.AdjustOutputItemsWidth ();
			this.RefreshHints ();
		}

		public void RefreshHints()
		{
			this.inputItemsHint.Visibility  = (this.inputItemsController.Count == 0);
			this.outputItemsHint.Visibility = !this.inputItemsHint.Visibility  && (this.outputItemsController.Count == 0);
			this.groupItemsHint.Visibility  = !this.outputItemsHint.Visibility && (this.groupsController.Count == 0);

			if (this.groupItemsHint.Visibility)
			{
				this.groupItemsHint.ZOrder = 0;
			}
		}

		public void RefreshInputViewSelection()
		{
			int    count = this.inputItemsController.Where (x => x.IsSelected).Count ();
			string icon  = "manifest:Epsitec.Common.Graph.Images.Glyph.Group.icon";

			foreach (var item in this.inputItemsController)
			{
				var visibility = item.IsSelected && count > 1 ? ButtonVisibility.Show : ButtonVisibility.Hide;

				item.DefineIconButton (visibility, icon,
					delegate
					{
						this.groupsController.CreateGroup (this.GetSelectedSeries ());
					});
			}
		}

		public void RefreshHilites()
		{
			foreach (var item in this.GetAllViews ())
			{
				long id = item.GetVisualSerialId ();
				GraphDataSeries series;
				GraphDataGroup group;
				Color color = Color.Empty;
				HiliteInfo info = HiliteInfo.Empty;

				if (this.viewToSeries.TryGetValue (id, out series))
				{
					info = this.hilites.Find (x => x.Series == series || ((x.Series != null) && (x.Series == series.Parent)));
				}
				else if (this.viewToGroup.TryGetValue (id, out group))
				{
					info = this.hilites.Find (x => x.Group == group);
				}

				switch (info.Type)
				{
					case HiliteType.Default:
						color = Color.FromRgb (1, 1, 0);
						break;

					case HiliteType.Input:
						color = Color.FromRgb (0, 1, 0);
						break;
					case HiliteType.Output:
						color = Color.FromRgb (0, 0, 1);
						break;
				}

				if (info.Depth > 0)
				{
					double mix = 1.0 / (1 << info.Depth);
					color = Color.Mix (color, Color.FromBrightness (1), mix);
				}

				item.HiliteColor = color;
			}
		}

		
		public void ExcludeOutput(GraphDataSeries item)
		{
			this.outputActiveIndex = -1;

			GraphActions.DocumentRemoveSeriesFromOutput (this.Document.GetSeriesId (item));
		}

		public void IncludeOutput(GraphDataSeries item)
		{
			int n = this.Document.OutputSeries.Count;
			
			this.outputActiveIndex = n;
			this.AddOutputToDocument (item);

			System.Diagnostics.Debug.Assert (this.Document.OutputSeries.Count == n+1);
		}

		public void AddOutputToDocument(GraphDataSeries item)
		{
			this.SelectUnusedColor (item);
			GraphActions.DocumentAddSeriesToOutput (this.Document.GetSeriesId (item));
		}

		public void SetPreferredGraphType(Command graphType)
		{
			if ((this.chartViewController != null) &&
				(graphType != null))
            {
				this.chartViewController.GraphType = graphType;
            }
		}

		public void SetOutputIndex(GraphDataSeries item, int index)
		{
			GraphActions.DocumentSetSeriesOutputIndex (this.Document.GetSeriesId (item), index);
		}

		/// <summary>
		/// Finds the window containing the chart view of the live document output.
		/// </summary>
		/// <returns>The window or <c>null</c>.</returns>
		public Window FindLiveChartViewWindow()
		{
			return this.floatingChartViews.Where (x => x.ChartSnapshot == null).Select (x => x.Container.Window).FirstOrDefault ();
		}

		/// <summary>
		/// Creates a window with a chart view; the chart view will either be the live copy
		/// of the document output or an existing chart snapshot.
		/// </summary>
		/// <param name="snapshot">The snapshot.</param>
		/// <returns></returns>
		public Window CreateChartViewWindow(GraphChartSnapshot snapshot)
		{
			int index = this.GetNewChartViewIndex ();

			var windowName = index.ToString (System.Globalization.CultureInfo.InstalledUICulture);
			var windowSize = new Size (800, 600);

			if (snapshot != null)
            {
				windowName = snapshot.GuidName;
            }
			
			Window window = new Window ()
			{
				Icon = this.application.Window.Icon,
				Text = this.application.Window.Text,
				ClientSize = windowSize,
				Name = string.Concat ("floating chart view ", windowName),
				Parent = this.application.Window,
			};

			var frame = new FrameBox ()
			{
				Parent = window.Root,
				Dock = DockStyle.Fill,
			};

			var controller = new ChartViewController (this.application, this)
			{
				IsStandalone = true,
				GraphType = this.chartViewController.GraphType,
				ColorStyle = this.chartViewController.ColorStyle,
				Index = index,
				ChartSnapshot = snapshot,
			};

			controller.SetupUI (frame);
			controller.Refresh (this.Document);

			this.floatingChartViews.Add (controller);

			window.WindowClosed +=
				delegate
				{
					this.floatingChartViews.Remove (controller);
					window.Dispose ();
				};

			UI.RegisterWindowPositionSaver (window);
			UI.RestoreWindowPosition (window);

			//	Associate the window with the snapshot : there is at most one window for
			//	a given snapshot.
			
			if (snapshot != null)
            {
				snapshot.Window = window;
			}
			
			return window;
		}


		public MiniChartView CreateView(GraphDataSeries item)
		{
			var view = this.CreateView ();
			var series = item.ChartSeries;

			view.Renderer.Collect (series);
			view.Title = DataCube.CleanUpLabel (item.Title);
			view.Label = DataCube.CleanUpLabel (item.Label);

			long id = view.GetVisualSerialId ();

			this.viewToSeries[id] = item;

			view.Disposed +=
				delegate
				{
					this.viewToSeries.Remove (id);
				};

			return view;
		}

		public MiniChartView CreateView(GraphDataGroup group)
		{
			var view = this.CreateView ();
			var func = group.DefaultFunctionName;

			view.Title = group.Name;
			view.PaintPaperClip = true;

			if (string.IsNullOrEmpty (func))
			{
				view.Renderer.CollectRange (group.InputDataSeries.Select (x => x.ChartSeries));
				view.Label = "Groupe";
			}
			else
			{
				view.Renderer.Collect (group.GetSyntheticDataSeries (func).ChartSeries);
				view.Label = Functions.FunctionFactory.GetFunctionCaption (func);
			}

			long id = view.GetVisualSerialId ();

			this.viewToGroup[id] = group;

			view.Disposed +=
				delegate
				{
					this.viewToGroup.Remove (id);
				};

			return view;
		}


		public void HideBalloonTip()
		{
			if (this.balloonTip != null)
			{
				this.balloonTip.Dispose ();
				this.balloonTip = null;
			}
		}

		public void HideGroupItemsHint()
		{
			this.groupItemsHint.Visibility = false;
		}

		public void HideOutputItemsHint()
		{
			this.outputItemsHint.Visibility = false;
		}

		
		private IEnumerable<GraphDataSeries> GetSelectedSeries()
		{
			return this.inputItemsController.Where (x => x.IsSelected).Select (x => this.viewToSeries[x.GetVisualSerialId ()]);
		}

		private IEnumerable<MiniChartView> GetAllViews()
		{
			var inputs  = this.inputItemsController;
			var groups  = this.groupsController.GroupViews;
			var details = this.groupsController.DetailViews;
			var outputs = this.outputItemsController;

			return inputs.Concat (groups).Concat (details).Concat (outputs);
		}


		public static StaticText CreateLabel(Widget container, string title)
		{
			return new StaticText ()
			{
				Dock = DockStyle.Stacked,
				PreferredHeight = 20,
				Parent = container,
				Text = title,
				ContentAlignment = ContentAlignment.MiddleCenter,
			};
		}
		
		private int GetNewChartViewIndex()
		{
			int index = 0;

			foreach (var item in this.floatingChartViews)
			{
				if (item.Index == index)
				{
					index++;
				}
				else
				{
					break;
				}
			}
			
			return index;
		}

		private void SelectUnusedColor(GraphDataSeries item)
		{
			int n = this.colorStyle.Count;
			int[] colors = new int[n];

			this.Document.OutputSeries.ForEach (x => colors[x.ColorIndex % n]++);
			int min = colors.Min ();

			for (int i = 0; i < n; i++)
			{
				if (colors[i] == min)
				{
					item.ColorIndex = i;
					break;
				}
			}
		}

		private void AdjustOutputItemsWidth()
		{
			int count = this.outputItemsController.Count;

			if (count > 0)
			{
				double availableWidth = this.outputItemsController.Container.ActualWidth - this.outputItemsController.OverlapX;
				double itemWidth = System.Math.Min (WorkspaceController.DefaultViewWidth, System.Math.Floor (availableWidth / count) + this.outputItemsController.OverlapX);

				this.outputItemsController.ForEach (x => x.PreferredWidth = itemWidth);
			}

			this.RefreshActiveOutput ();
		}

		private void RefreshActiveOutput()
		{
			this.outputItemsController.UpdateLayout ();

			if ((this.outputActiveIndex >= 0) &&
				(this.outputActiveIndex < this.outputItemsController.Count))
			{
				var active = this.outputItemsController[this.outputActiveIndex];
				System.Diagnostics.Debug.WriteLine (string.Format ("item {0} at {1} - {2}", this.outputActiveIndex, active.ActualBounds.Left, active.ActualBounds.Right));
				this.ShowOutputInfo (active);
			}
			else
			{
				this.HideOutputInfo ();
			}
		}


		private void ShowOutputInfo(MiniChartView view)
		{
			if (this.outputActiveInfo != null)
			{
				var size = this.outputColorPalette.PreferredSize + this.outputColorPalette.Margins.Size + this.outputActiveInfo.Padding.Size;
				
				var disposition = ButtonMarkDisposition.Below;
				var balloonRect = BalloonTip.GetBestPosition (size, view.ActualBounds, view.Parent.Client.Size, ref disposition);

				this.outputActiveInfo.PreferredSize = size;
				this.outputActiveInfo.TipAttachment = view.ActualBounds.Center -  balloonRect.BottomLeft;
				this.outputActiveInfo.Margins = new Margins (balloonRect.Left, 0, 0, balloonRect.Bottom);
				this.outputActiveInfo.Disposition = disposition;
				this.outputActiveInfo.Visibility = true;
				this.outputActiveInfo.ZOrder = 0;
			}
		}

		private void HideOutputInfo()
		{
			if (this.outputActiveInfo != null)
			{
				this.outputActiveInfo.Visibility = false;
			}
		}

		private void DefineOutputColor(Color color)
		{
			if ((this.outputActiveIndex >= 0) &&
				(this.outputActiveIndex < this.outputItemsController.Count))
			{
				var doc = this.Document;
				var colorIndex = doc.OutputSeries[this.outputActiveIndex].ColorIndex;
				var colorValue = color.ToString ();

				GraphActions.DocumentDefineColor (colorIndex, colorValue);

//-				doc.DefaultColorStyle.DefineColor (colorIndex, color);
//-				this.Refresh ();
			}
		}

		
		private MiniChartView CreateInputView(GraphDataSeries item)
		{
			var view = this.CreateView (item);
			var cat  = item.Source.GetCategory (item);

			Color color = this.labelColorStyle[cat.Index];
			Color dark  = Color.FromHsv (color.Hue, 1.0, 0.5);

			view.PreferredWidth  = WorkspaceController.InputViewWidth;
			view.PreferredHeight = WorkspaceController.InputViewHeight;
//-			view.AutoCheckButton = true;
			view.ActiveState = item.IsSelected ? ActiveState.Yes : ActiveState.No;
			view.BackColor = color;
			view.Renderer.ClearStyles ();
			view.Renderer.AddStyle (new ColorStyle (this.colorStyle.Name) { dark });

			view.ActiveStateChanged +=
				delegate
				{
					if (view.ActiveState == ActiveState.Yes)
					{
						this.IncludeOutput (item);
					}
					else
					{
						this.ExcludeOutput (item);
					}
				};

			this.CreateInputDragAndDropHandler (item, view);
			
			return view;
		}

		private MiniChartView CreateOutputView(GraphDataSeries item, int index)
		{
			var view = this.CreateView (item);

			view.MouseCursor = MouseCursor.AsHand;
			view.DisplaySample = true;

			view.Renderer.ClearStyles ();
			view.Renderer.AddStyle (new ColorStyle (this.colorStyle.Name)
			{
				this.colorStyle[item.ColorIndex]
			});

			string iconUri = "manifest:Epsitec.Common.Graph.Images.Glyph.DropItem.icon";

			view.DefineIconButton (ButtonVisibility.ShowOnlyWhenEntered, iconUri,
				delegate
				{
					this.ExcludeOutput (item);
				});

			this.CreateOutputDragAndDropHandler (item, view);
			
			return view;
		}

		private MiniChartView CreateView()
		{
			var lineChartRenderer = new LineChartRenderer ();

			lineChartRenderer.AddStyle (new ColorStyle (this.colorStyle.Name) { Color.FromBrightness (0.3) });
			lineChartRenderer.AddAdorner (new Epsitec.Common.Graph.Adorners.CoordinateAxisAdorner ()
			{
				VisibleGrid = false,
				VisibleLabels = false,
				VisibleTicks = false
			});

			lineChartRenderer.DefineValueLabels (this.Document.ChartColumnLabels);
			lineChartRenderer.AlwaysIncludeZero = true;

			var view = new MiniChartView ()
			{
				Anchor = AnchorStyles.TopLeft,
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				PreferredWidth = WorkspaceController.DefaultViewWidth,
				PreferredHeight = WorkspaceController.DefaultViewHeight,
				Padding = new Margins (4, 4, 4, 4),
				Margins = new Margins (0, 0, 0, 0),
				Renderer = lineChartRenderer,
				Scale = 0.5,
				PaintPaperStack = true,
				DisplayValue = true,
			};

			view.Entered +=
				delegate
				{
					this.HandleViewHiliteChanged (view, true);
					view.HiliteColor = Color.FromRgb (1.0, 1.0, 0.0);
				};

			view.Exited +=
				delegate
				{
					view.HiliteColor = Color.Empty;
					this.HandleViewHiliteChanged (view, false);
				};

			return view;
		}

		
		private void CreateOutputDragAndDropHandler(GraphDataSeries item, MiniChartView view)
		{
			ViewDragDropManager drag = null;

			view.Pressed +=
				(sender, e) =>
				{
					switch (e.Message.Button)
					{
						case MouseButtons.Left:
							drag = new ViewDragDropManager (this, view, view.MapClientToScreen (e.Point))
							{
								Series = item,
								LockY = true,
							};
							drag.DefineMouseMoveBehaviour (MouseCursor.AsSizeWE);
							e.Message.Captured = true;
							break;

						case MouseButtons.Right:
							this.ShowInputContextMenu (item.Parent, view, e.Point);
							break;
					}
				};

			view.Released +=
				delegate
				{
					if ((drag != null) &&
						(drag.ProcessDragEnd () == false))
					{
						this.outputActiveIndex = this.Document.ResolveOutputSeries (item).Index;
					}
					else
					{
						this.outputActiveIndex = -1;
					}

					this.RefreshActiveOutput ();
					
					drag = null;
				};
		}

		private void CreateInputDragAndDropHandler(GraphDataSeries item, MiniChartView view)
		{
			ViewDragDropManager drag = null;

			view.Pressed +=
				(sender, e) =>
				{
					switch (e.Message.Button)
					{
						case MouseButtons.Left:
							drag = new ViewDragDropManager (this, view, view.MapClientToScreen (e.Point))
							{
								Series = item,
							};
							drag.DefineMouseMoveBehaviour (MouseCursor.AsHand);
							break;
						
						case MouseButtons.Right:
							this.ShowInputContextMenu (item, view, e.Point);
							break;
					}

					e.Message.Captured = true;
				};

			view.Released +=
				(sender, e) =>
				{
					if ((drag != null) &&
						(drag.ProcessDragEnd () == false))
					{
						this.HandleInputViewClicked (item, view);

						this.inputItemsController.UpdateLayout ();
						this.application.Window.RefreshEnteredWidgets (e.Message);
					}

					drag = null;
				};
		}

		private void ShowInputContextMenu(GraphDataSeries item, MiniChartView view, Point pos)
		{
			VMenu contextMenu = new VMenu ()
			{
				Host = view,
				AutoDispose = true,
			};

			var item1 = new MenuItem ()
			{
				Text = this.Document.ResolveOutputSeries (item) != null ? "Exclure du graphique" : "Inclure dans le graphique",
				Name = this.Document.ResolveOutputSeries (item) != null ? "-" : "+",
			};

			var item2 = new MenuItem ()
			{
				Text = "Changer le signe",
			};

			item1.Clicked +=
				delegate
				{
					if (item1.Name == "-")
					{
						this.ExcludeOutput (item);
					}
					else
					{
						this.IncludeOutput (item);
					}
				};

			item2.Clicked +=
				delegate
				{
					item.NegateValues = !item.NegateValues;
					this.Document.InvalidateCache ();
					this.Refresh ();
				};

			contextMenu.Items.Add (item1);
			contextMenu.Items.Add (item2);

			this.HideBalloonTip ();
			this.outputActiveIndex = -1;
			this.RefreshActiveOutput ();

			contextMenu.ShowAsContextMenu (view, view.MapClientToScreen (pos));
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
					Color color2 = MiniChartView.GetLightColor (color1);

					graphics.GradientRenderer.Fill = GradientFill.Y;
					graphics.GradientRenderer.SetColors (color1, color2);
					graphics.GradientRenderer.SetParameters (0, 100);
					graphics.GradientRenderer.Transform = Transform.Identity.Scale (1.0, label.Height / 100.0).Translate (label.BottomLeft);
					graphics.RenderGradient ();

					graphics.Transform = transform;
					e.Cancel = true;
				};

			var button = new CheckButton ()
			{
				Text = category.Name,
				Dock = DockStyle.Fill,
				Parent = frame,
				ActiveState = this.Document.ActiveFilterCategories.Contains (category) ? ActiveState.Yes : ActiveState.No,
			};

			button.ActiveStateChanged +=
				sender =>
				{
					if (button.ActiveState == ActiveState.Yes)
					{
						GraphActions.DocumentIncludeFilterCategory (category.Name);
					}
					else
					{
						GraphActions.DocumentExcludeFilterCategory (category.Name);
					}
				};
		}

		private void CreateSourceButton(Widget filters, GraphDataSource source, double verticalSpace)
		{
			var frame = new FrameBox ()
			{
				Dock = DockStyle.Stacked,
				PreferredHeight = 20 + 4 * verticalSpace,
				BackColor = MiniChartView.StickyNoteYellow,
				Parent = filters,
				Padding = new Margins (4, 4, verticalSpace, verticalSpace),
			};

			frame.PaintBackground +=
				(sender, e) =>
				{
					var label = Rectangle.Deflate (frame.Client.Bounds, new Margins (3, 3, 1+2*verticalSpace, 1+2*verticalSpace));
					var graphics = e.Graphics;
					var transform = graphics.Transform;
					graphics.RotateTransformDeg (0, label.Center.X, label.Center.Y);

					MiniChartView.PaintShadow (graphics, label);
					graphics.AddFilledRectangle (label);

					Color color1 = frame.BackColor;
					Color color2 = MiniChartView.GetLightColor (color1);

					graphics.GradientRenderer.Fill = GradientFill.Y;
					graphics.GradientRenderer.SetColors (color1, color2);
					graphics.GradientRenderer.SetParameters (0, 100);
					graphics.GradientRenderer.Transform = Transform.Identity.Scale (1.0, label.Height / 100.0).Translate (label.BottomLeft);
					graphics.RenderGradient ();

					graphics.Transform = transform;
					e.Cancel = true;
				};

			var button = new RadioButton ()
			{
				Text = FormattedText.Escape (DataCube.CleanUpLabel (source.Name)),
				Dock = DockStyle.Fill,
				Parent = frame,
				ActiveState = this.Document.ActiveDataSource == source ? ActiveState.Yes : ActiveState.No,
			};

			button.ActiveStateChanged +=
				sender =>
				{
					if (button.ActiveState == ActiveState.Yes)
					{
						GraphActions.DocumentSelectDataSource (source.Name);
					}
				};
		}


		private void HandleInputViewClicked(GraphDataSeries item, MiniChartView view)
		{
			if (Message.CurrentState.IsControlPressed)
			{
				view.SetSelected (!view.IsSelected);
			}
			else
			{
#if true
				view.Toggle ();
#else
				bool select = !view.IsSelected;
				
				this.inputItemsController.ForEach (x => x.SetSelected (false));
				this.groupItemsController.ForEach (x => x.SetSelected (false));

				this.groupItemsController.ActiveItem = null;

				view.SetSelected (select);
#endif
			}

			this.RefreshInputViewSelection ();
			this.RefreshGroups ();
		}

		private void HandleViewHiliteChanged(MiniChartView view, bool entered)
		{
			this.hilites.Clear ();
			
			this.HideBalloonTip ();

			if (entered && view.IsEnabled)
			{
				this.ShowBalloonTip (view, this.hilites);
			}

			this.RefreshHilites ();
		}

		private void ShowBalloonTip(MiniChartView view, List<HiliteInfo> hilites)
		{
			long id = view.GetVisualSerialId ();

			GraphDataGroup group;
			GraphDataSeries series;

			string summary = "";

			if (this.viewToGroup.TryGetValue (id, out group))
			{
				//	Hovering over a group in the group view.

#if false
				hilites.Add (new HiliteInfo (group, 0, HiliteType.Default));

				summary = this.GetSummary (group);

				group.InputDataSeries.ForEach (x => WorkspaceController.AddInputSeries (hilites, x, 0));
				group.SyntheticDataSeries.ForEach (x => WorkspaceController.AddOutputSeries (hilites, x, 0));
#endif
			}
			else if (this.viewToSeries.TryGetValue (id, out series))
			{
				//	Hovering over a series (either in the input view, details view of a group or output view).

				summary = this.GetSummary (series);

				while (series != null)
				{
					hilites.Add (new HiliteInfo (series, 0, HiliteType.Default));

					WorkspaceController.AddInputSeries (hilites, series, 0);

					series.Groups.ForEach (x => WorkspaceController.AddOutputGroup (hilites, x, 0));
					series = series.Parent;
				}
			}

			this.balloonTip = this.CreateSummaryBalloon (view, summary);
		}

		
		private BalloonTip CreateSummaryBalloon(MiniChartView view, string summary)
		{
			var window = view.Window;
			
			if ((window == null) ||
				(string.IsNullOrEmpty (summary)))
			{
				return null;
			}

			TextLayout layout = new TextLayout ()
			{
				Text = summary,
				LayoutSize = new Size (this.WorkspaceFrame.ActualWidth, 1000)
			};

			var size   = layout.StandardRectangle.Size + new Size (20, 20);

			if (size.Width < 160)
            {
				size.Width = 160;
            }

			var clip   = view.Parent.MapClientToRoot (view.Parent.Client.Bounds);
			var bounds = Rectangle.Intersection (clip, Rectangle.Deflate (view.MapClientToRoot (view.Client.Bounds), 4, 4));
			var mark   = ButtonMarkDisposition.Below;
			var rect   = BalloonTip.GetBestPosition (size, bounds, window.ClientSize, ref mark);

			return new BalloonTip ()
			{
				Anchor = AnchorStyles.BottomLeft,
				Margins = new Margins (rect.Left, 0, 0, rect.Bottom),
				Parent = window.Root,
				PreferredSize = rect.Size,
				BackColor = Color.FromName ("Info"),
				TipAttachment = bounds.Center - rect.BottomLeft,
				Disposition = mark,
				Text = summary,
				ContentAlignment = ContentAlignment.TopLeft,
			};
		}

		private string GetSummary(GraphDataSeries series)
		{
			var buffer = new System.Text.StringBuilder ();
			var valCount = series.ChartSeries.Values.Count;
			var minValue = series.ChartSeries.GetMinValue ();
			var maxValue = series.ChartSeries.GetMaxValue ();
			
			string minMax;

			switch (valCount)
			{
				case 0:
					minMax = "Aucune valeur";
					break;
				
				case 1:
					minMax = string.Format ("Valeur: {0:0.#}", minValue.Value);
					break;

				default:
					minMax = string.Format ("Min: {0:0.#} Max: {1:0.#}", minValue.Value, maxValue.Value);
					break;
			}

			buffer.Append ("<font face=\"Futura\" style=\"Condensed Medium\">");

			if (this.Document.OutputSeries.Contains (series))
			{
				buffer.Append ("<font size=\"120%\">");
				buffer.AppendFormat ("Source {0}", FormattedText.Escape (DataCube.CleanUpLabel (series.Label)));
				buffer.Append ("</font><br/>");
				buffer.Append (FormattedText.Escape (DataCube.CleanUpLabel (series.Title)));
				buffer.Append ("<br/>");
				buffer.Append ("<font size=\"80%\">");
				buffer.Append (minMax);
				buffer.Append ("</font>");
			}
			else
			{
				string name = DataCube.CleanUpLabelPrefixOnly (series.Title);
				int    pos  = name.IndexOf ('\t');
				string compte  = pos < 0 ? "" : name.Substring (0, pos).Trim ();
				string libellé = name.Substring (pos+1).Trim ();

				if (string.IsNullOrEmpty (compte))
				{
					buffer.Append ("<font size=\"120%\">");
					buffer.Append (FormattedText.Escape (libellé));
					buffer.Append ("</font><br/>");
				}
				else
				{
					buffer.Append ("<font size=\"120%\">");
					buffer.AppendFormat ("Compte {0}", FormattedText.Escape (compte));
					buffer.Append ("</font><br/>");
					buffer.Append (FormattedText.Escape (libellé));
				}
				buffer.Append ("<br/>");
				buffer.Append ("<font size=\"80%\">");
				buffer.AppendFormat ("Source {0}<br/>", FormattedText.Escape (DataCube.CleanUpLabel (series.Source.Name)));
				buffer.Append (minMax);
				buffer.Append ("</font>");
			}

			buffer.Append ("</font>");

			return buffer.ToString ();
		}

		private string GetSummary(GraphDataGroup group)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			buffer.Append ("<font face=\"Futura\" style=\"Condensed Medium\">");
			buffer.Append ("<font size=\"120%\">");
			buffer.Append (group.Name);
			buffer.Append ("</font><br/>");
			buffer.AppendFormat (group.Count < 2 ? "Groupe avec {0} élément" : "Groupe avec {0} éléments<br/>", group.Count);
			buffer.Append ("<font size=\"80%\">");
			buffer.AppendJoin (", ", group.InputDataSeries.Select (x => DataCube.CleanUpLabel (x.Label)));
			buffer.Append ("</font>");
			buffer.Append ("</font>");

			return buffer.ToString ();
		}


		#region HiliteType Enumeration

		enum HiliteType
		{
			Undefined,
			Default,
			Input,
			Output,
		}

		#endregion

		#region HiliteInfo Structure

		struct HiliteInfo
		{
			public HiliteInfo(GraphDataSeries series, int depth, HiliteType type)
			{
				this.series = series;
				this.group = null;
				this.depth = depth;
				this.type = type;
			}

			public HiliteInfo(GraphDataGroup group, int depth, HiliteType type)
			{
				this.series = null;
				this.group = group;
				this.depth = depth;
				this.type = type;
			}

			public GraphDataSeries Series
			{
				get
				{
					return this.series;
				}
			}
			
			public GraphDataGroup Group
			{
				get
				{
					return this.group;
				}
			}
			
			public int Depth
			{
				get
				{
					return this.depth;
				}
			}
			
			public HiliteType Type
			{
				get
				{
					return this.type;
				}
			}

			public static readonly HiliteInfo Empty = new HiliteInfo ();

			private readonly GraphDataSeries series;
			private readonly GraphDataGroup group;
			private int depth;
			private HiliteType type;
		}

		#endregion

		private static void AddOutputSeries(List<HiliteInfo> outputs, GraphDataSeries item, int depth)
		{
			if (outputs.Any (x => x.Series == item && x.Type != HiliteType.Default))
			{
				return;
			}

			outputs.Add (new HiliteInfo (item, depth, HiliteType.Output));

			item.Groups.ForEach (x => x.SyntheticDataSeries.ForEach (y => WorkspaceController.AddOutputSeries (outputs, y, depth+1)));
		}

		private static void AddInputSeries(List<HiliteInfo> inputs, GraphDataSeries item, int depth)
		{
			if (inputs.Any (x => x.Series == item && x.Type != HiliteType.Default))
			{
				return;
			}

			inputs.Add (new HiliteInfo (item, depth, HiliteType.Input));

			var synth = item as GraphSyntheticDataSeries;

			if ((synth != null) &&
				(synth.SourceGroup != null))
			{
				WorkspaceController.AddInputGroup (inputs, synth.SourceGroup, depth);
			}
		}

		private static void AddOutputGroup(List<HiliteInfo> outputs, GraphDataGroup item, int depth)
		{
			if (outputs.Any (x => x.Group == item && x.Type != HiliteType.Default))
			{
				return;
			}

			outputs.Add (new HiliteInfo (item, depth, HiliteType.Output));

			item.SyntheticDataSeries.ForEach (x => WorkspaceController.AddOutputSeries (outputs, x, depth));
		}

		private static void AddInputGroup(List<HiliteInfo> inputs, GraphDataGroup item, int depth)
		{
			if (inputs.Any (x => x.Group == item && x.Type != HiliteType.Default))
			{
				return;
			}

			inputs.Add (new HiliteInfo (item, depth, HiliteType.Input));

			foreach (var series in item.InputDataSeries)
			{
				WorkspaceController.AddInputSeries (inputs, series, depth+1);
			}
		}

		private void RefreshPreview()
		{
			if (this.Document != null)
			{
				this.chartViewController.Refresh (this.Document);
				this.floatingChartViews.ForEach (x => x.Refresh (this.Document));
			}
		}

		public static readonly double DefaultViewWidth  = 100;
		public static readonly double DefaultViewHeight =  80;

		private static readonly double InputViewWidth  = 120;
		private static readonly double InputViewHeight =  60;
		
		private readonly GraphApplication		application;

		private readonly ItemListController<MiniChartView>		inputItemsController;
		private readonly ItemListController<MiniChartView>		outputItemsController;
		private readonly GroupsController		groupsController;
		private readonly SnapshotsController	snapshotsController;
		private readonly DataCubeSelectionController cubeSelController;

		private readonly Dictionary<long, GraphDataSeries> viewToSeries;
		private readonly Dictionary<long, GraphDataGroup> viewToGroup;
		private readonly List<HiliteInfo>		hilites;

		private ChartViewController				chartViewController;
		private List<ChartViewController>		floatingChartViews;
		
		private StaticText				inputItemsHint;
		private StaticText				groupItemsHint;
		private FrameBox				outputPageFrame;
		private StaticText				outputItemsHint;
		private BalloonTip				balloonTip;
		private ColorStyle				labelColorStyle;
		private ColorStyle				colorStyle;
		private int						outputActiveIndex;
		private BalloonTip				outputActiveInfo;
		private ColorPalette			outputColorPalette;
	}
}
