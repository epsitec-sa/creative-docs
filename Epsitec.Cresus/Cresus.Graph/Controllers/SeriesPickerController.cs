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
	internal sealed class SeriesPickerController
	{
		public SeriesPickerController(Window owner)
		{
			this.negatedSeriesLabels = new HashSet<string> ();


			this.window = new Window ()
			{
				Text = Res.Strings.DataPicker.Title.ToSimpleText (),
				ClientSize = new Epsitec.Common.Drawing.Size (1008, 736),
				Name = "SeriesPicker2",
				Owner = owner,
				Icon = owner.Icon,
				PreventAutoClose = true
			};

			this.window.MakeSecondaryWindow ();

			var root = this.window.Root;

			root.Padding = new Margins (0, 0, 0, 0);

			var frameTop = new FrameBox ()
			{
				Dock = DockStyle.Top,
				PreferredHeight = 150,
				Parent = root,
				Name = "top",
				BackColor = Color.FromRgb (1, 0.9, 0.9)
			};

			var frameWorkspace = new FrameBox ()
			{
				Dock = DockStyle.Fill,
				Parent = root,
				Name = "workspace",
				BackColor = Color.FromRgb (1, 1, 0.9)
			};

			var frameInputs = new FrameBox ()
			{
				Dock = DockStyle.Fill,
				Parent = frameWorkspace,
				Name = "inputs",
				BackColor = Color.FromRgb (0.9, 1, 0.9),
				PreferredHeight = 320
			};

			var frameOutputs = new FrameBox ()
			{
				Dock = DockStyle.Bottom,
				Parent = frameWorkspace,
				Name = "outputs",
				PreferredHeight = 260
			};

			var frameOutputsLeft = new FrameBox ()
			{
				Dock = DockStyle.Fill,
				Parent = frameOutputs,
				Name = "left"
			};

			var frameGroups = new FrameBox ()
			{
				Dock = DockStyle.Fill,
				Parent = frameOutputsLeft,
				Name = "groups",
				BackColor = Color.FromRgb (0.9, 0.9, 1),
				PreferredHeight = 160
			};

			var frameSelection = new FrameBox ()
			{
				Dock = DockStyle.Bottom,
				Parent = frameOutputsLeft,
				Name = "selection",
				BackColor = Color.FromRgb (1, 0.9, 0.8),
				PreferredHeight = 100
			};

			var framePreview = new FrameBox ()
			{
				Dock = DockStyle.Right,
				Parent = frameOutputs,
				Name = "preview",
				BackColor = Color.FromRgb (1, 0.85, 0.8),
				PreferredWidth = 260
			};

			this.inputItemsController = new ItemListController<MiniChartView> (frameInputs)
			{
				ItemLayoutMode = ItemLayoutMode.Flow
			};

			this.selectionItemsController = new ItemListController<MiniChartView> (frameSelection)
			{
				ItemLayoutMode = ItemLayoutMode.Horizontal
			};

			this.groupItemsController = new ItemListController<MiniChartView> (frameGroups)
			{
				ItemLayoutMode = ItemLayoutMode.Horizontal
			};

			this.groupDetailItemsController = new ItemListController<MiniChartView> (frameGroups)
			{
				ItemLayoutMode = ItemLayoutMode.Horizontal,
				Anchor = AnchorStyles.BottomLeft
			};

			this.window.WindowCloseClicked += sender => this.HideWindow ();

			GraphProgram.Application.ActiveDocumentChanged += sender => this.UpdateScrollListItems ();
		}

		public void ShowWindow()
		{
			this.window.Show ();
		}

		public void HideWindow()
		{
			this.window.Hide ();
		}

		public GraphDataSet DataSet
		{
			get
			{
				return GraphProgram.Application.Document.DataSet;
			}
		}

		public void UpdateScrollListItems()
		{
			if ((this.DataSet != null) &&
				(this.DataSet.DataTable != null))
			{
				List<string> labels = new List<string> (this.DataSet.DataTable.RowLabels);

				this.inputItemsController.Clear ();

				int index = 0;

				foreach (var item in this.DataSet.DataTable.RowSeries)
				{
					var view = this.CreateMiniChartView (index++, item);

					view.AutoCheckButton = true;

					view.ActiveStateChanged +=
						sender =>
						{
							MiniChartView v = sender as MiniChartView;
							if (v.ActiveState == ActiveState.Yes)
							{
								this.AddSeriesToGraph (v.Index);
							}
							else
							{
								this.RemoveSeriesFromGraph (v.Index);
							}
						};

					view.Clicked +=
						(sender, e) =>
						{
							MiniChartView v = sender as MiniChartView;
							
							if ((e.Message.Button == MouseButtons.Left) &&
								(e.Message.ButtonDownCount == 1))
							{
								if (Message.CurrentState.IsControlPressed)
								{
									v.SetSelected (!v.IsSelected);
								}
								else
								{
									bool select = !v.IsSelected;
									this.inputItemsController.ForEach (x => x.SetSelected (false));
									v.SetSelected (select);
								}

								this.UpdateUserSelection ();
							}
						};

					this.inputItemsController.Add (view);
				}
			}
		}

		private void UpdateUserSelection()
		{
			int n = this.inputItemsController.Where (x => x.IsSelected).Count ();
			this.inputItemsController.ForEach (x => x.ShowIconButton (x.IsSelected && n > 1 ? ButtonVisibility.Show : ButtonVisibility.Hide, this.HandleGroupButtonClicked, "manifest:Epsitec.Common.Graph.Images.Glyph.Group.icon"));
		}

		private void HandleDropButtonClicked(int index)
		{
			this.inputItemsController.Find (x => x.Index == index).ActiveState = ActiveState.No;
		}

		private void HandleGroupButtonClicked()
		{
			this.CreateGroupView (this.inputItemsController.Where (x => x.IsSelected).Select (x => x.Renderer.SeriesItems.First ()));
		}

		private void CreateGroupView(IEnumerable<ChartSeries> series)
		{
			var view = this.CreateMiniChartView (this.groupItemsController.Count, null);

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
							view.Renderer.SeriesItems.ForEach (s => this.groupDetailItemsController.Add (this.CreateMiniChartView (-1, s)));
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

		private MiniChartView CreateMiniChartView(int index, ChartSeries item)
		{
			var lineChartRenderer = new LineChartRenderer ();

			lineChartRenderer.AddStyle (new Epsitec.Common.Graph.Styles.ColorStyle ("line-color") { "Red", "DeepPink", "Coral", "Tomato", "SkyBlue", "RoyalBlue", "DarkBlue", "Green", "PaleGreen", "Lime", "Yellow", "Wheat" });
			lineChartRenderer.AddAdorner (new Epsitec.Common.Graph.Adorners.CoordinateAxisAdorner ()
			{
				VisibleGrid = false,
				VisibleLabels = false,
				VisibleTicks = false
			});

			lineChartRenderer.DefineValueLabels (this.DataSet.DataTable.ColumnLabels);
			lineChartRenderer.Collect (item);

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
				Renderer = lineChartRenderer,
				Title = item == null ? "" : item.Label.Substring (item.Label.IndexOf (' ')+1),
				Label = item == null ? "" : item.Label.Substring (0, item.Label.IndexOf (' ')),
				Index = index
			};
			
			return view;
		}

		private void AddSeriesToGraph(int index)
		{
			var view = this.CreateMiniChartView (index, this.DataSet.DataTable.GetRowSeries (index));

			view.ShowIconButton (ButtonVisibility.ShowOnlyWhenEntered, () => this.HandleDropButtonClicked (index), "manifest:Epsitec.Common.Graph.Images.Glyph.DropItem.icon");

			this.selectionItemsController.Add (view);
		}

		private void RemoveSeriesFromGraph(int index)
		{
			var view = this.selectionItemsController.Find (item => item.Index == index);
			this.selectionItemsController.Remove (view);
		}

		public void UpdateChartView()
		{
		}

		public void SetSelectedItem(int index)
		{
		}

		public void ClearNegatedSeries()
		{
			this.negatedSeriesLabels.Clear ();
		}

		public void NegateSeries(string seriesLabel)
		{
			if (this.negatedSeriesLabels.Contains (seriesLabel))
			{
				this.negatedSeriesLabels.Remove (seriesLabel);
			}
			else
			{
				this.negatedSeriesLabels.Add (seriesLabel);
			}
		}

		public ChartSeries GetRowSeries(int index)
		{
			var  table  = this.DataSet.DataTable;
			var  series = table.GetRowSeries (index);
			bool negate = this.negatedSeriesLabels.Contains (series.Label);

			if (negate)
			{
				return new ChartSeries ("-" + series.Label, series.Values.Select (x => new ChartValue (x.Label, -x.Value)));
			}
			else
			{
				return series;
			}
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

		readonly private Window					window;
		readonly private HashSet<string>		negatedSeriesLabels;

		readonly private ItemListController<MiniChartView>		inputItemsController;
		readonly private ItemListController<MiniChartView>		selectionItemsController;
		readonly private ItemListController<MiniChartView>		groupItemsController;
		readonly private ItemListController<MiniChartView>		groupDetailItemsController;
	}
}
