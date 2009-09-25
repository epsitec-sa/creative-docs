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

using Epsitec.Cresus.Graph.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Graph.Controllers
{
	internal sealed class WorkspaceController
	{
		public WorkspaceController(GraphApplication application)
		{
			this.application = application;
			this.filterCategories = new HashSet<GraphDataCategory> ();
			this.colorStyle = new ColorStyle ("line-color");
			
			for (int hue = 0; hue < 360; hue += 36)
			{
				this.colorStyle.Add (Color.FromAlphaHsv (1.0, hue, 1.0, 1.0));
			}
			
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
				ItemLayoutMode = ItemLayoutMode.Vertical,
				OverlapY = -8,
			};

			this.groupDetailItemsController = new ItemListController<MiniChartView> ()
			{
				ItemLayoutMode = ItemLayoutMode.Flow,
			};

			this.chartViewController = new ChartViewController (this.application)
			{
				GraphType = Res.Commands.GraphType.UseLineChart,
				ColorStyle = this.colorStyle,
			};
			
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
//-				BackColor = Color.FromRgb (0.9, 1, 0.9),
				Parent = container,
			};

			new Separator ()
			{
				Dock = DockStyle.Left,
				PreferredWidth = 3,
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
				IsVerticalLine = true,
				Parent = settingsFrame,
			};
		}
		
		private void SetupWorkspaceFrameUI()
		{
			var controller = this.application.MainWindowController;
			var container  = controller.WorkspaceFrame;

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

			var groupFrame = new FrameBox ()
			{
				Dock = DockStyle.Right,
				Parent = topFrame,
				Name = "groups",
				BackColor = Color.FromBrightness (1.0),
				PreferredWidth = 200,
				Padding = new Margins (0, 0, 1, 0),
			};

			var splitter1 = new VSplitter ()
			{
				Dock = DockStyle.Right,
				Parent = topFrame,
				PreferredWidth = 3,
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
				Name = "page1",
				TabTitle = "Graphique",
				Parent = outputBook,
			};

			var newPage = new TabPage ()
			{
				Name = "page+",
				TabTitle = "+",
				Parent = outputBook,
			};

			this.outputPageFrame = new FrameBox ()
			{
				Dock = DockStyle.Fill,
				Name = "page frame",
				Parent = outputPage,
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

			outputBook.ActivePage = outputPage;

			WorkspaceController.PatchTabBookPaintBackground (outputBook);
//-			WorkspaceController.PatchInputFramePaintBackground (inputFrame);
//-			WorkspaceController.PatchBottomFrameLeftPaintBackground (bottomFrameLeft);
			WorkspaceController.PatchPreviewFramePaintBackground (outputBook, previewFrame);

			this.groupDetailsBalloon = new BalloonTip ()
			{
				Anchor = AnchorStyles.BottomLeft,
				Disposition = ButtonMarkDisposition.Below,
				Padding = new Margins (5, 5, 17, 8),
				Visibility = false,
				Parent = container.Window.Root,
				BackColor = Color.FromBrightness (1),
			};

			var detailsFrame = new FrameBox ()
			{
				Dock = DockStyle.Fill,
				Parent = this.groupDetailsBalloon,
			};

			this.inputItemsController.SetupUI (inputFrame);
			this.groupItemsController.SetupUI (groupFrame);
			this.groupDetailItemsController.SetupUI (detailsFrame);
			this.outputItemsController.SetupUI (outputFrame);
			this.chartViewController.SetupUI (previewFrame);

			this.inputItemsHint = new StaticText ()
			{
				Anchor = AnchorStyles.All,
				Parent = inputFrame,
				Visibility = false,
				Text = "<font size=\"120%\">Aucune donnée n'est visible pour l'instant.</font><br/>" +
				"Cochez les catégories que vous souhaitez voir apparaître ici.",
				ContentAlignment = ContentAlignment.MiddleCenter,
				PreferredHeight = 40,
			};

			this.groupItemsHint = new StaticText ()
			{
				Anchor = AnchorStyles.All,
				Parent = groupFrame,
				Visibility = false,
				Text = "<font size=\"120%\">Aucun groupe n'existe pour l'instant.</font><br/>" +
				"Pour créer un groupe, sélectionnez plusieurs éléments ci-dessus en maintenant la touche Ctrl<br/>" +
				"enfoncée (Ctrl-clic), puis cliquez sur le trombone miniature d'un des éléments sélectionnés.",
				ContentAlignment = ContentAlignment.MiddleCenter,
				PreferredHeight = 40,
			};

			this.outputItemsHint = new StaticText ()
			{
				Anchor = AnchorStyles.All,
				Parent = outputPage,
				Visibility = false,
				BackColor = Color.FromBrightness (1),
				Text = "<font size=\"120%\">Graphique vide.</font><br/>" +
				"Cochez les éléments que vous souhaitez voir apparaître dans le graphique.",
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

					e.Suppress = true;
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

					graphics.AddFilledRectangle (bounds);
					graphics.RenderSolid (Color.FromBrightness (1.0));

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
				this.RefreshSources ();
				this.RefreshHilites ();
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

		private void RefreshSources()
		{
			var container = this.application.MainWindowController.ToolsFrame.FindChild ("sources", Widget.ChildFindMode.Deep);

			container.Children.Widgets.ForEach (x => x.Dispose ());

			System.Diagnostics.Debug.Assert (container.Children.Count == 0);

			var label = new StaticText ()
			{
				Dock = DockStyle.Stacked,
				PreferredHeight = 20,
				Parent = container,
				Text = "Sources de données",
				ContentAlignment = ContentAlignment.MiddleCenter,
			};

			var bottom = new FrameBox ()
			{
				Dock = DockStyle.StackFill,
				Parent = container,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
			};

			var container1 = new FrameBox ()
			{
				Dock = DockStyle.Fill,
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
				Parent = bottom,
			};

			var container2 = new FrameBox ()
			{
				Dock = DockStyle.Fill,
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
				Parent = bottom,
			};

			int index = 0;

			foreach (var source in this.Document.DataSources)
			{
				if (index < 2)
				{
					this.CreateSourceButton (container1, source);
				}
				else if (index < 4)
				{
					this.CreateSourceButton (container2, source);
				}

				index++;
			}
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

			this.RefreshHints ();
		}

		private void RefreshOutputs()
		{
			this.outputItemsController.Clear ();

			int index = 0;

			foreach (var item in this.Document.OutputSeries)
			{
				this.outputItemsController.Add (this.CreateOutputView (item, index++));
			}

			this.AdjustOutputItemsWidth ();

			if ((this.outputActiveIndex >= 0) &&
				(this.outputActiveIndex < this.outputItemsController.Count))
			{
				this.outputItemsController.ActiveItem = this.outputItemsController[this.outputActiveIndex];
			}

			this.RefreshHints ();
		}

		private void RefreshHints()
		{
			this.inputItemsHint.Visibility  = (this.inputItemsController.Count == 0);
			this.outputItemsHint.Visibility = (this.inputItemsController.Count > 0) && (this.outputItemsController.Count == 0);
			this.outputPageFrame.Visibility = (this.outputItemsController.Count > 0);
			this.groupItemsHint.Visibility  = (this.inputItemsController.Count > 0) && (this.groupItemsController.Count == 0);
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
			this.ShowGroupDetails (this.activeGroup, this.activeGroupView);
		}

		private void RefreshGroups()
		{
			this.groupItemsController.Clear ();

			this.activeGroupView = null;

			foreach (var group in this.Document.Groups)
			{
				var view = this.CreateGroupView (group);

				if (group == this.activeGroup)
				{
					this.activeGroupView = view;
				}

				this.groupItemsController.Add (view);
			}

			this.RefreshGroupView ();
			this.RefreshHints ();
		}

		private void RefreshHilites()
		{
			foreach (var item in this.GetAllMiniChartViews ())
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

			string iconName = "manifest:Epsitec.Common.Graph.Images.Glyph.DropItem.icon";

			view.DefineIconButton (ButtonVisibility.ShowOnlyWhenEntered, iconName,
				delegate
				{
					//	TODO: make this fool proof (deleting a group which is still used is fatal)
					this.DeleteGroup (group, view);
					this.Refresh ();
				});
			
			return view;
		}

		private MiniChartView CreateGroupDetailView(GraphDataGroup group, GraphDataSeries item)
		{
			var view = this.CreateView (item);
			var synt = item as GraphSyntheticDataSeries;
			
			string iconName = "manifest:Epsitec.Common.Graph.Images.Glyph.DropItem.icon";

			if ((synt != null) &&
				(synt.SourceGroup == group))
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

			view.PreferredWidth  = WorkspaceController.InputViewWidth;
			view.PreferredHeight = WorkspaceController.InputViewHeight;
//-			view.AutoCheckButton = true;
			view.ActiveState = item.IsSelected ? ActiveState.Yes : ActiveState.No;
			view.BackColor = this.labelColorStyle[cat.Index];

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

		private void ExcludeOutput(GraphDataSeries item)
		{
			this.Document.RemoveOutput (item);

			this.outputActiveIndex = this.Document.OutputSeries.Count - 1;

			this.RefreshInputs ();
			this.RefreshGroups ();
			this.RefreshOutputs ();
			this.RefreshPreview ();
		}

		private void IncludeOutput(GraphDataSeries item)
		{
			this.Document.AddOutput (item);

			this.outputActiveIndex = this.Document.OutputSeries.Count - 1;

			this.RefreshInputs ();
			this.RefreshGroups ();
			this.RefreshOutputs ();
			this.RefreshPreview ();
		}

		private MiniChartView CreateOutputView(GraphDataSeries item, int index)
		{
			var view = this.CreateView (item);

			view.MouseCursor = MouseCursor.AsHand;

			view.Renderer.RemoveStyle (this.colorStyle);
			view.Renderer.AddStyle (new ColorStyle (this.colorStyle.Name)
			{
				this.colorStyle[index]
			});

			string iconName = "manifest:Epsitec.Common.Graph.Images.Glyph.DropItem.icon";

			view.DefineIconButton (ButtonVisibility.ShowOnlyWhenEntered, iconName,
				delegate
				{
					this.Document.RemoveOutput (item);
					this.Refresh ();
				});

			this.CreateOutputDragAndDropHandler (item, view);
			
			return view;
		}

		private void CreateOutputDragAndDropHandler(GraphDataSeries item, MiniChartView view)
		{
			ViewDragDropManager target = null;

			view.Pressed +=
				(sender, e) =>
				{
					e.Message.Captured = true;

					target = new ViewDragDropManager (item, view, this, view.MapClientToScreen (e.Point))
					{
						LockY = true,
					};

					view.MouseMove +=
						(sender2, e2) =>
						{
							target.ProcessMouseMove (e2.Point,
								delegate
								{
									view.Enable = false;
									view.MouseCursor = MouseCursor.AsSizeWE;
								});

							e2.Suppress = true;
						};
				};

			view.Released +=
				delegate
				{
					target.ProcessDragEnd ();
				};
		}

		private void CreateInputDragAndDropHandler(GraphDataSeries item, MiniChartView view)
		{
			ViewDragDropManager target = null;

			view.Pressed +=
				(sender, e) =>
				{
					e.Message.Captured = true;

					target = new ViewDragDropManager (item, view, this, view.MapClientToScreen (e.Point));

					view.MouseMove +=
						(sender2, e2) =>
						{
							target.ProcessMouseMove (e2.Point,
								delegate
								{
									view.Enable = false;
									view.MouseCursor = MouseCursor.AsHand;
								});

							e2.Suppress = true;
						};
				};

			view.Released +=
				(sender, e) =>
				{
					if (target.ProcessDragEnd () == false)
					{
						this.HandleInputViewClicked (item, view);

						this.inputItemsController.UpdateLayout ();
						this.application.Window.RefreshEnteredWidgets (e.Message);
					}
				};
		}

		class ViewDragDropManager
		{
			public ViewDragDropManager(GraphDataSeries item, MiniChartView view, WorkspaceController host, Point mouse)
			{
				this.item = item;
				this.view = view;
				this.host = host;
				this.viewMouseCursor = this.view.MouseCursor;
				
				this.host.CloseBalloonTip ();
				
				this.viewOrigin  = view.MapClientToScreen (new Point (0, 0));
				this.mouseOrigin = mouse;
				
				this.outputIndex = -1;
				this.Separator = new Separator ()
				{
					Parent = this.host.outputItemsController.Container,
					Visibility = false,
					Anchor = AnchorStyles.TopAndBottom | AnchorStyles.Left,
					PreferredWidth = 2,
				};
			}

			public bool LockX
			{
				get;
				set;
			}

			public bool LockY
			{
				get;
				set;
			}

			public DragWindow DragWindow
			{
				get;
				private set;
			}

			public Separator Separator
			{
				get;
				private set;
			}

			

			
			public bool ProcessMouseMove(Point mouse, System.Action dragStartAction)
			{
				mouse = this.view.MapClientToScreen (mouse);
				
				if (this.DragWindow == null)
				{
					if (Point.Distance (mouse, this.mouseOrigin) > 2.0)
					{
						this.CreateDragWindow ();
					}

					dragStartAction ();
				}

				if (this.DragWindow != null)
				{
					double x = this.viewOrigin.X + (this.LockX ? 0 : mouse.X - this.mouseOrigin.X);
					double y = this.viewOrigin.Y + (this.LockY ? 0 : mouse.Y - this.mouseOrigin.Y);

					this.DragWindow.WindowLocation = new Point (x, y);
					
					if ((this.DetectOutput (mouse)) ||
						(this.DetectGroup (mouse)))
					{
						return true;
					}

					this.host.RefreshHints ();
				}
				
				return false;
			}

			public bool ProcessDragEnd()
			{
				bool ok = this.DragWindow != null;

				if (this.DragWindow != null)
				{
					this.DragWindow.Dispose ();
					this.DragWindow = null;
				}

				if (this.Separator != null)
				{
					this.Separator.Dispose ();
					this.Separator = null;
				}
				
				if (this.outputIndex >= 0)
				{
					this.host.Document.SetOutputIndex (this.item, this.outputIndex);
					this.host.Refresh ();
				}
				else if (this.groupIndex >= 0)
				{
					var groups = this.host.Document.Groups;

					if ((this.groupIndex < groups.Count) &&
						(this.groupUpdate))
					{
						this.host.UpdateGroup (groups[this.groupIndex], Collection.Single (this.item));
					}
					else
					{
						this.host.CreateGroup (Collection.Single (this.item));
					}
					
					this.host.Refresh ();
				}

				this.view.Enable = true;
				this.view.MouseCursor = this.viewMouseCursor;
				this.view.ClearUserEventHandlers (Widget.EventNames.MouseMoveEvent);

				return ok;
			}

			private bool DetectOutput(Point screenMouse)
			{
				var container = this.host.outputItemsController.Container;
				var mouse     = container.MapScreenToClient (screenMouse);

				if (container.Client.Bounds.Contains (mouse))
				{
					//	If the target is empty, accept to drop independently of the position :

					if (this.host.outputItemsController.Count == 0)
					{
						this.SetOutputDropTarget (2, 0);
						return true;
					}

					//	Otherwise, find the best possible match, but discard dropping on the item
					//	itself, or between it and its immediate neighbours :

					var dist = this.host.outputItemsController.Select (x => new
					{
						Distance = mouse.X - x.ActualBounds.Center.X,
						View = x
					});

					System.Diagnostics.Debug.WriteLine (string.Join (", ", dist.Select (x => string.Format ("{0}:{1}", x.View.Index, x.Distance)).ToArray ()));

					var best = dist.OrderBy (x => System.Math.Abs (x.Distance)).FirstOrDefault ();

					if ((best != null) &&
						(best.View != this.view))
					{
						if ((best.Distance < 0) &&
							((best.View.Index != this.view.Index+1) || !this.host.Document.OutputSeries.Contains (this.item)))
						{
							this.SetOutputDropTarget (best.View.ActualBounds.Left - this.Separator.PreferredWidth + 3, best.View.Index);
							return true;
						}
						if ((best.Distance >= 0) &&
						    ((best.View.Index != this.view.Index-1) || !this.host.Document.OutputSeries.Contains (this.item)))
						{
							this.SetOutputDropTarget (best.View.ActualBounds.Right - 3, best.View.Index+1);
							return true;
						}
					}
				}
				else if (this.host.outputPageFrame.Parent.Client.Bounds.Contains (this.host.outputPageFrame.MapScreenToParent (screenMouse)))
				{
					int n = this.host.outputItemsController.Count;

					this.SetOutputDropTarget (n > 0 ? this.host.outputItemsController.Last ().ActualBounds.Right - 3 : 2, n);
					
					return true;
				}
				
				this.Separator.Visibility = false;
				this.outputIndex = -1;
				
				return false;
			}

			private void SetOutputDropTarget(double x, int index)
			{
				this.Separator.Margins = new Margins (x, 0, 0, 0);
				this.Separator.Visibility = true;
				
				this.outputIndex = index;
				
				this.host.outputItemsHint.Visibility = false;
			}

			private void SetGroupDropTarget(int index, bool update)
			{
				this.groupIndex = index;
				this.groupUpdate = update;

				this.host.groupItemsHint.Visibility = false;
			}


			private bool DetectGroup(Point screenMouse)
			{
				var container = this.host.groupItemsController.Container;
				var mouse     = container.MapScreenToClient (screenMouse);

				if (container.Client.Bounds.Contains (mouse))
				{
					if (this.host.groupItemsController.Count == 0)
					{
						this.SetGroupDropTarget (0, false);
						return true;
					}

					var drop = this.host.groupItemsController.Where (x => x.HitTest (mouse)).FirstOrDefault ();

					if (drop != null)
					{
						this.SetGroupDropTarget (drop.Index, true);
						return true;
					}
					
					var dist = this.host.groupItemsController.Select (x => new
					{
						Distance = mouse.Y - x.ActualBounds.Center.Y,
						View = x
					});

					System.Diagnostics.Debug.WriteLine (string.Join (", ", dist.Select (x => string.Format ("{0}:{1}", x.View.Index, x.Distance)).ToArray ()));

					var best = dist.Where (x => x.Distance >= 0).OrderBy (x => System.Math.Abs (x.Distance)).FirstOrDefault ();

					if (best != null)
					{
						this.SetGroupDropTarget (best.View.Index, false);
					}
					else
					{
						this.SetGroupDropTarget (this.host.groupItemsController.Count, false);
					}

					return true;
				}

				this.groupIndex = -1;

				return false;
			}
			
			private void CreateDragWindow()
			{
				this.DragWindow = new DragWindow ()
				{
					Alpha = 0.6,
					WindowLocation = this.viewOrigin,
					Owner = this.view.Window,
				};

				this.DragWindow.DefineWidget (this.host.CreateView (this.item), this.view.PreferredSize, Margins.Zero);
				this.DragWindow.Show ();
			}

			
			private readonly GraphDataSeries item;
			private readonly MiniChartView view;
			private readonly WorkspaceController host;
			private readonly Point viewOrigin;
			private readonly Point mouseOrigin;
			private readonly MouseCursor viewMouseCursor;

			private int outputIndex;
			private int groupIndex;
			private bool groupUpdate;
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
						this.RefreshGroups ();
						this.RefreshOutputs ();
					}
				};
		}

		private void CreateSourceButton(Widget filters, GraphDataSource source)
		{
			var frame = new FrameBox ()
			{
				Dock = DockStyle.Stacked,
				PreferredHeight = 24,
				BackColor = Color.FromRgb (1.0, 1.0, 0.8),
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

			var button = new RadioButton ()
			{
				Text = source.Name,
				Dock = DockStyle.Fill,
				Parent = frame,
				ActiveState = this.Document.ActiveDataSource == source ? ActiveState.Yes : ActiveState.No,
			};

			button.ActiveStateChanged +=
				sender =>
				{
					if (button.ActiveState == ActiveState.Yes)
					{
						this.Document.ActiveDataSource = source;
					}
					
					this.RefreshInputs ();
					this.RefreshGroups ();
					this.RefreshOutputs ();
				};
		}
		
		private void CreateGroup()
		{
			var items = this.inputItemsController.Where (x => x.IsSelected).Select (x => this.viewToSeries[x.GetVisualSerialId ()]);
			var group = this.CreateGroup (items);
			int index = this.Document.Groups.IndexOf (group);
			this.activeGroup = null;
			this.activeGroupView = null;
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

		private void DeleteGroup(GraphDataGroup group, MiniChartView view)
		{
			this.Document.RemoveGroup (group);
			this.groupItemsController.Remove (view);
		}

		private void UpdateGroupName(GraphDataGroup group)
		{
			int count = group.Count;

			group.Name = string.Format (count > 1 ? "{0} éléments" : "{0} élément", count);

			this.Refresh ();
		}

		
		private void HandleGroupViewClicked(GraphDataGroup group, MiniChartView view)
		{
			this.activeGroup = group;
			this.activeGroupView = view;
			this.RefreshInputViewSelection ();
			this.RefreshGroupView ();
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
			this.RefreshGroupView ();
		}

		private void HandleViewHiliteChanged(MiniChartView view, bool entered)
		{
			this.hilites.Clear ();
			
			this.CloseBalloonTip ();

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

		private void CloseBalloonTip()
		{
			if (this.balloonTip != null)
			{
				this.balloonTip.Dispose ();
				this.balloonTip = null;
			}
		}

		
		private BalloonTip CreateSummaryBalloon(MiniChartView view, string summary)
		{
			var window = view.Window;
			
			if ((window == null) ||
				(string.IsNullOrEmpty (summary)))
			{
				return null;
			}

			if (!view.IsActualGeometryValid)
			{
				window.ForceLayout ();
			}

			var clip   = view.Parent.MapClientToRoot (view.Parent.Client.Bounds);
			var bounds = Rectangle.Intersection (clip, Rectangle.Deflate (view.MapClientToRoot (view.Client.Bounds), 4, 4));
			var mark   = ButtonMarkDisposition.Below;
			var rect   = BalloonTip.GetBestPosition (new Size (160, 80), bounds, window.ClientSize, ref mark);

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
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			buffer.Append ("<font face=\"Futura\" style=\"Condensed Medium\">");

			if (this.Document.OutputSeries.Contains (series))
			{
				buffer.Append ("<font size=\"120%\">");
				buffer.AppendFormat ("Source {0}", series.Label);
				buffer.Append ("</font><br/>");
				buffer.Append (series.Title);
				buffer.Append ("<br/>");
				buffer.Append ("<font size=\"80%\">");
				buffer.Append ("Min: 12.0 Max: 3456.7");
				buffer.Append ("</font>");
			}
			else
			{
				string name = series.Title;
				string compte = name.Substring (0, name.IndexOf (' ')+1).Trim ();
				string libellé = name.Substring (name.IndexOf (' ')+1).Trim ();

				buffer.Append ("<font size=\"120%\">");
				buffer.AppendFormat ("Compte {0}", compte);
				buffer.Append ("</font><br/>");
				buffer.Append (libellé);
				buffer.Append ("<br/>");
				buffer.Append ("<font size=\"80%\">");
				buffer.AppendFormat ("Source: {0}<br/>", series.Source.Name);
				buffer.Append ("Min: 12.0 Max: 3456.7");
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
			buffer.Append (string.Join (", ", group.InputDataSeries.Select (x => x.Label).ToArray ()));
			buffer.Append ("</font>");
			buffer.Append ("</font>");

			return buffer.ToString ();
		}

		
		private MiniChartView CreateView(GraphDataSeries item)
		{
			var view = CreateView ();
			var series = item.ChartSeries;

			view.Renderer.Collect (series);
			view.Title = item.Title;
			view.Label = item.Label;

			long id = view.GetVisualSerialId ();

			this.viewToSeries[id] = item;

			view.Disposed +=
				delegate
				{
					this.viewToSeries.Remove (id);
				};

			return view;
		}

		private MiniChartView CreateView(GraphDataGroup group)
		{
			var view = CreateView ();
			
			view.Renderer.CollectRange (group.InputDataSeries.Select (x => x.ChartSeries));
			view.Title = group.Name;
			view.Label = "Groupe";

			long id = view.GetVisualSerialId ();

			this.viewToGroup[id] = group;

			view.Disposed +=
				delegate
				{
					this.viewToGroup.Remove (id);
				};

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

		private IEnumerable<MiniChartView> GetAllMiniChartViews()
		{
			return this.inputItemsController.Concat (this.groupItemsController).Concat (this.groupDetailItemsController).Concat (this.outputItemsController);
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

		private void ShowGroupDetails(GraphDataGroup group, MiniChartView view)
		{
			this.groupDetailItemsController.Clear ();
			this.CloseGroupDetails ();

			if (group == null)
			{
				return;
			}

//-			var series1 = group.SyntheticDataSeries.Cast<GraphDataSeries> ();
//			var series2 = group.InputDataSeries;
//			var series = series1.Concat (series2);

			foreach (var item in group.InputDataSeries)
			{
				this.groupDetailItemsController.Add (this.CreateGroupDetailView (group, item));
			}

			var window = view.Window;
			
			if (window != null)
			{
				if (view.IsActualGeometryValid == false)
				{
					window.ForceLayout ();
				}

				double width  = 0;
				double height = WorkspaceController.DefaultViewHeight;

				switch (group.Count)
				{
					case 0:
					case 1:
						width = WorkspaceController.DefaultViewWidth;
						break;

					case 2:
						width = WorkspaceController.DefaultViewWidth * 2 - this.groupDetailItemsController.OverlapX;
						break;

					default:
						width = WorkspaceController.DefaultViewWidth * 3 - this.groupDetailItemsController.OverlapX * 2;
						
						if (group.Count > 3)
						{
							height = WorkspaceController.DefaultViewHeight * 2 - this.groupDetailItemsController.OverlapY;
							width += AbstractScroller.DefaultBreadth + 2;
						}
						break;
				}

				this.groupDetailItemsController.VisibleScroller = group.Count > 3;
				
				var clip   = view.Parent.MapClientToRoot (view.Parent.Client.Bounds);
				var bounds = Rectangle.Intersection (clip, Rectangle.Deflate (view.MapClientToRoot (view.Client.Bounds), 4, 4));
				var mark   = ButtonMarkDisposition.Below;
				var rect   = BalloonTip.GetBestPosition (new Size (width + 12, height + 12 + 10), bounds, window.ClientSize, ref mark);

				this.groupDetailsBalloon.Margins = new Margins (rect.Left, 0, 0, rect.Bottom);
				this.groupDetailsBalloon.PreferredSize = rect.Size;
				this.groupDetailsBalloon.TipAttachment = bounds.Center - rect.BottomLeft;
				this.ShowGroupDetails ();
			}

			this.RefreshHilites ();
		}

		private void ShowGroupDetails()
		{
			if (this.groupDetailsBalloon.Visibility == false)
			{
				this.groupDetailsBalloon.Visibility = true;
				this.RegisterFilter ();
			}
		}

		private void CloseGroupDetails()
		{
			if (this.groupDetailsBalloon.Visibility)
			{
				this.activeGroup = null;
				this.activeGroupView = null;
				this.groupDetailsBalloon.Visibility = false;
				this.UnregisterFilter ();
			}
		}

		private void RegisterFilter()
		{
			Window.ApplicationDeactivated += this.HandleApplicationDeactivated;
			Window.MessageFilter          += this.MessageFilter;
		}

		private void UnregisterFilter()
		{
			Window.ApplicationDeactivated -= this.HandleApplicationDeactivated;
			Window.MessageFilter          -= this.MessageFilter;
		}

		private void MessageFilter(object sender, Message message)
		{
			if (message.IsMouseType)
			{
				var container = this.groupDetailItemsController.Container;
				var pos = container.MapRootToClient (message.Cursor);
				
				if (container.Client.Bounds.Contains (pos))
				{
					//	OK
				}
				else
				{
					if (message.MessageType == MessageType.MouseDown)
					{
						this.CloseGroupDetails ();
					}
				}
			}
		}

		private void HandleApplicationDeactivated(object sender)
		{
			this.CloseGroupDetails ();
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
					this.Document.UpdateSyntheticSeries ();
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
							this.IncludeOutput (item);
						}
						else
						{
							this.ExcludeOutput (item);
						}
					}
				};

		}


		private void RefreshPreview()
		{
			if (this.Document != null)
			{
				this.chartViewController.Refresh (this.Document);
			}
		}

		private static readonly double DefaultViewWidth  = 100;
		private static readonly double DefaultViewHeight =  80;

		private static readonly double InputViewWidth  = 120;
		private static readonly double InputViewHeight =  60;
		
		public System.Action<IEnumerable<int>>	SumSeriesAction;
		public System.Action<IEnumerable<int>>	AddSeriesToGraphAction;
		public System.Action<IEnumerable<int>>	NegateSeriesAction;

		private readonly GraphApplication		application;

		private readonly ItemListController<MiniChartView>		inputItemsController;
		private readonly ItemListController<MiniChartView>		outputItemsController;
		private readonly ItemListController<MiniChartView>		groupItemsController;
		private readonly ItemListController<MiniChartView>		groupDetailItemsController;
		private readonly HashSet<GraphDataCategory> filterCategories;

		private readonly Dictionary<long, GraphDataSeries> viewToSeries;
		private readonly Dictionary<long, GraphDataGroup> viewToGroup;
		private readonly List<HiliteInfo> hilites;

		private ChartViewController		chartViewController;
		
		private StaticText				inputItemsHint;
		private StaticText				groupItemsHint;
		private FrameBox				outputPageFrame;
		private StaticText				outputItemsHint;
		private VerticalInjectionArrow	groupCalculatorArrow;
		private BalloonTip				balloonTip;
		private BalloonTip				groupDetailsBalloon;
		private ColorStyle labelColorStyle;
		private ColorStyle colorStyle;
		private GraphDataGroup activeGroup;
		private MiniChartView activeGroupView;
		private int outputActiveIndex;
	}
}
