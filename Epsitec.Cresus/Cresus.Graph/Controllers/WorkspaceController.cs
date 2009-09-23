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
			
			var inputFrame = new FrameBox ()
			{
				Dock = DockStyle.Fill,
				Parent = container,
				Name = "inputs",
				Padding = new Margins (0, 0, 1, 0),
				PreferredHeight = 320
			};

			var bottomFrame = new FrameBox ()
			{
				Dock = DockStyle.Bottom,
				Parent = container,
				Name = "bottom",
				PreferredHeight = 264
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
				PreferredHeight = 164,
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
				Parent = outputBook,
			};

			var newPage = new TabPage ()
			{
				Name = "page+",
				TabTitle = "+",
				Parent = outputBook,
			};

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

			this.chartViewController.SetupUI (previewFrame);

			this.inputItemsWarning = new StaticText ()
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
				Parent = outputFrame,
				Visibility = false,
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

			this.inputItemsWarning.Visibility = (this.inputItemsController.Count == 0);
		}

		private void RefreshOutputs()
		{
			this.outputItemsController.Clear ();

			int index = 0;

			foreach (var item in this.Document.OutputSeries)
			{
				this.outputItemsController.Add (this.CreateOutputView (item, index++));
			}

			if ((this.outputActiveIndex >= 0) &&
				(this.outputActiveIndex < this.outputItemsController.Count))
			{
				this.outputItemsController.ActiveItem = this.outputItemsController[this.outputActiveIndex];
			}
			
			this.outputItemsHint.Visibility = (this.inputItemsController.Count > 0) && (this.outputItemsController.Count == 0);
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
				this.ShowGroupDetails (null, null);
			}
			else
			{
				var group = this.Document.Groups[index];
				this.ShowGroupCalculator (group, view);
				this.ShowGroupDetails (group, view);
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
				this.ShowGroupDetails (group, view);

				view.SetSelected (true);
			}

			this.groupItemsHint.Visibility = (this.inputItemsController.Count > 0) && (this.groupItemsController.Count == 0);
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

			view.AutoCheckButton = true;
			view.ActiveState = item.IsSelected ? ActiveState.Yes : ActiveState.No;
			view.LabelColor = this.labelColorStyle[cat.Index];

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

			view.Pressed +=
				delegate
				{
					view.Enable = false;
				};

			view.Released +=
				delegate
				{
					view.Enable = true;
				};
			
			return view;
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
			var view  = this.groupItemsController.ActiveItem;
			int index = this.groupItemsController.IndexOf (view);
			var items = this.inputItemsController.Where (x => x.IsSelected).Select (x => this.viewToSeries[x.GetVisualSerialId ()]);

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

		private void HandleViewHiliteChanged(MiniChartView view, bool entered)
		{
			long id = view.GetVisualSerialId ();

			GraphDataGroup group;
			GraphDataSeries series;

			this.hilites.Clear ();
			
			if (this.balloonTip != null)
			{
				this.balloonTip.Dispose ();
				this.balloonTip = null;
			}

			if (entered)
			{
				string summary = "";

				if (this.viewToGroup.TryGetValue (id, out group))
				{
					//	Hovering over a group in the group view.

					this.hilites.Add (new HiliteInfo (group, 0, HiliteType.Default));

					summary = this.GetSummary (group);

					group.InputDataSeries.ForEach (x => WorkspaceController.AddInputSeries (this.hilites, x, 0));
					group.SyntheticDataSeries.ForEach (x => WorkspaceController.AddOutputSeries (this.hilites, x, 0));
				}
				else if (this.viewToSeries.TryGetValue (id, out series))
				{
					//	Hovering over a series (either in the input view, details view of a group or output view).

					summary = this.GetSummary (series);

					while (series != null)
					{
						this.hilites.Add (new HiliteInfo (series, 0, HiliteType.Default));

						WorkspaceController.AddInputSeries (this.hilites, series, 0);

						series.Groups.ForEach (x => WorkspaceController.AddOutputGroup (this.hilites, x, 0));
						series = series.Parent;
					}
				}

				this.balloonTip = this.CreateSummaryBalloon (view, summary);
			}

			this.RefreshHilites ();
		}

		
		private BalloonTip CreateSummaryBalloon(MiniChartView view, string summary)
		{
			var window = view.Window;
			
			if (window == null)
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
				buffer.Append ("<font size=\"120%\">");
				buffer.AppendFormat ("Compte {0}", series.Label);
				buffer.Append ("</font><br/>");
				buffer.Append (series.Title);
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
				PreferredWidth = 80,
				PreferredHeight = 80,
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

			double offset = 0;

			if ((view != null) &&
				(view.Window != null))
			{
				if (view.IsActualGeometryValid == false)
				{
					view.Window.ForceLayout ();
				}

				offset = view.ActualBounds.Left;
			}

			this.groupDetailItemsController.DefineOffset (offset);

			this.RefreshHilites ();
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
		
		private StaticText				inputItemsWarning;
		private StaticText				groupItemsHint;
		private StaticText				outputItemsHint;
		private VerticalInjectionArrow	groupCalculatorArrow;
		private BalloonTip				balloonTip;
		private ColorStyle labelColorStyle;
		private Command							graphType;
		private ColorStyle colorStyle;
		private int outputActiveIndex;
	}
}
