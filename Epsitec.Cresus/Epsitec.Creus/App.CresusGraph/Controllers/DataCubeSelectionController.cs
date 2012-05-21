//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.UI;
using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Graph.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Graph.Controllers
{
	internal sealed class DataCubeSelectionController
	{
		public DataCubeSelectionController(WorkspaceController workspace)
		{
			this.workspace = workspace;
		}


		public void SetupUI(Widgets.DataCubeFrame container)
		{
			GraphProgram.Application.ClipboardDataChanged +=
				(sender, e) =>
				{
					if (e.Format == ClipboardDataFormat.Text)
					{
						GraphProgram.Application.SetEnable (ApplicationCommands.Paste, true);
					}
				};

			GraphProgram.Application.ActiveDocumentChanged += this.HandleActiveDocumentChanged;

			this.container = container;

			var innerFrame = new FrameBox ()
			{
				Parent = this.container,
//-				BackColor = Color.FromRgb (1, 0.8, 0.8),
				Dock = DockStyle.Fill,
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
			};

			var topFrame = new FrameBox ()
			{
				Parent = innerFrame,
				Name = "top",
//-				BackColor = Color.FromRgb (1, 1, 0.8),
				Dock = DockStyle.Top,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				PreferredHeight = 76,
				Padding = new Margins (0, 0, 2, 2),
			};

			new Separator ()
			{
				Parent = innerFrame,
				Dock = DockStyle.Top,
				PreferredHeight = 1,
				IsHorizontalLine = true,
			};

			var middleFrame = new FrameBox ()
			{
				Parent = innerFrame,
				Name = "mid",
//-				BackColor = Color.FromRgb (0.8, 1, 0.8),
				Dock = DockStyle.Fill,
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
			};

			var bottomFrame = new FrameBox ()
			{
				Parent = innerFrame,
				Name = "bot",
//-				BackColor = Color.FromRgb (0.8, 1, 1),
				Dock = DockStyle.Bottom,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				Visibility = false,
			};

			new Separator ()
			{
				Parent = innerFrame,
				PreferredHeight = 1,
				Dock = DockStyle.Bottom,
				IsHorizontalLine = true,
			};

			var importButton = new ConfirmationButton ()
			{
				Parent = topFrame,
				Dock = DockStyle.Fill,
				Text = "<font size=\"150%\">Importer de nouvelles données</font><br/>Charge les données dans un nouveau cube, à partir d'une source externe.",
//-				ContentAlignment = ContentAlignment.TopLeft,
				Margins = new Margins (4, 0, 0, 0),
				Padding = new Margins (4, 4, 0, 0),
				ButtonStyle = ButtonStyle.ToolItem,
			};

			var closeButton = new IconButton ()
			{
				Parent = topFrame,
				IconUri = "manifest:Epsitec.Common.Graph.Images.Glyph.DropItem.icon",
				PreferredWidth = 19,
				PreferredHeight = 19,
				Margins = new Margins (0, 4, 0, 0),
				VerticalAlignment = VerticalAlignment.Top,
				Dock = DockStyle.Right,
			};

			var pasteButton = new MetaButton ()
			{
				Parent = topFrame,
				Dock = DockStyle.Right,
				CommandObject = ApplicationCommands.Paste,
				PreferredWidth = 72,
//				ButtonStyle = ButtonStyle.ToolItem,
			};

			closeButton.Clicked +=
				delegate
				{
					this.container.FindDataCubeButton ().SimulateClicked ();
				};

			importButton.Clicked += this.HandleImportButtonClicked;

			this.cubesScrollable = new Scrollable ()
			{
				Parent = middleFrame,
				Dock = DockStyle.Left,
				PreferredWidth = 200,
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
				HorizontalScrollerMode = ScrollableScrollerMode.HideAlways,
				VerticalScrollerMode = ScrollableScrollerMode.ShowAlwaysOppositeSide,
				PaintViewportFrame = false,
				ViewportFrameMargins = new Margins (0, -1, 0, 0),
			};

			this.cubeDetails = new DataCubeTableDetails ()
			{
				Parent = middleFrame,
				Dock = DockStyle.Fill,
			};

			this.cubeDetails.SourceEdited +=
				(sender, e) =>
				{
					var cube  = this.cubeDetails.Cube;
					var table = cube.ExtractNaturalDataTable ();
					var sourceName = e.NewValue as string;
					var document   = this.workspace.Document;
					table.DimensionVector.Add ("Source", sourceName);
					cube.Clear ();
					cube.AddTable (table);
					document.RefreshDataSet ();
//-					document.ClearData ();
//-					document.ReloadDataSet ();
				};

			this.cubesScrollable.Viewport.IsAutoFitting = true;
			this.cubesScrollable.AddEventHandler (Scrollable.ViewportOffsetYProperty, (sender, e) => this.UpdateInsertionMark ());
			this.CreateCubeButtonAdorner();
			
			this.outputInsertionMark = new Separator ()
			{
				Parent = innerFrame,
				Visibility = false,
				Anchor = AnchorStyles.LeftAndRight | AnchorStyles.Bottom,
				PreferredHeight = 2,
				Color = DataCubeTableButton.HiliteColor,
			};

			Epsitec.Common.Widgets.Layouts.LayoutEngine.SetIgnoreMeasure (this.outputInsertionMark, true);
		}

		private void UpdateCubeList()
		{
			this.cubesScrollable.Viewport.Children.Widgets.ForEach (x => x.Dispose ());

			int index = 0;

			foreach (var cube in this.workspace.Document.Cubes)
			{
				if (index > 0)
				{
					var sep = new Separator ()
					{
						Parent = this.cubesScrollable.Viewport,
						Dock = DockStyle.Stacked,
						PreferredHeight = 1,
						IsHorizontalLine = true,
					};
				}

				this.CreateCubeButton (cube).Index = index++;
			}
		}


		private void UpdateSelectedCube()
		{
			var buttons = from button in this.cubesScrollable.Viewport.Children.Widgets
						  where button is DataCubeTableButton
						  select button as DataCubeTableButton;

			var doc  = this.workspace.Document;
			var cube = doc.ActiveCube;
			var guid = cube == null ? System.Guid.Empty : cube.Guid;

			buttons.ForEach (button => button.SetSelected (button.Cube.Guid == guid));

			var activeButton = buttons.FirstOrDefault (x => x.IsSelected);
			
			this.cubeDetails.Button = activeButton;
			this.cubesScrollable.Show (activeButton);

			var selectedSourceName = this.cubeDetails.GetSourceName ();

			this.cubeDetails.IsSourceNameReadOnly = doc.OutputSeries.Any (x => x.Source.Name == selectedSourceName);
		}

		private DataCubeTableButton CreateCubeButton(GraphDataCube cube)
		{
			var button = new DataCubeTableButton ()
			{
				Parent = this.cubesScrollable.Viewport,
				Dock = DockStyle.Stacked,
				Cube = cube,
				PreferredHeight = 60,
			};

			this.CreateCubeDragHandler (cube, button);

			return button;
		}
		

		private void CreateCubeDragHandler(GraphDataCube cube, Widget button)
		{
			DragController.Attach (button,
				(drag) =>
				{
					drag.LockX = true;
					drag.WindowPadding = new Margins (0, -1, 0, 0);
				},
				(drag, mouse) =>
				{
					this.DetectOutput (drag, mouse);
				},
				(drag, moved) =>
				{
					this.cubesScrollable.VerticalScroller.SimulateArrowEngaged (0);
					this.outputInsertionMark.Visibility = false;

					if (moved)
					{
						this.HandleCubeMoved (cube);
					}
					else
					{
						this.HandleCubeClicked (cube);
					}
				});
		}

		private void CreateCubeButtonAdorner()
		{
			this.cubesScrollable.PaintForeground +=
				(sender, e) =>
				{
					if (this.cubesScrollable.ViewportOffset != Point.Zero)
					{
						return;
					}
					
					var graphics = e.Graphics;
					var button   = this.cubesScrollable.Viewport.Children.Widgets.LastOrDefault ();

					if (button != null)
                    {
						var rect = this.cubesScrollable.MapRootToClient (button.MapClientToRoot (button.Client.Bounds));
						
						if (rect.Bottom > 0)
                        {
							var adorner = Epsitec.Common.Widgets.Adorners.Factory.Active;
							var color   = adorner.ColorBorder;
							
							graphics.AddFilledRectangle (rect.Left, rect.Bottom-1, rect.Width, 1);
							graphics.AddFilledRectangle (rect.Right-1, 0, 1, rect.Bottom);
							graphics.RenderSolid (color);
                        }
                    }
				};
		}
		private bool DetectOutput(DragController drag, Point screenMouse)
		{
			var container = this.cubesScrollable.Viewport;
			var mouse     = container.MapScreenToClient (screenMouse);
			var buttons   = container.Children.Widgets.OfType<DataCubeTableButton> ();

			this.SetScrollableAutoScroll (mouse.Y);

			//	Find the best possible match, but discard dropping on the item
			//	itself, or between it and its immediate neighbours :

			var dist = buttons.Select (button => new
			{
				Distance = button.ActualBounds.Center.Y - mouse.Y,
				Button   = button
			});

			var best = dist.OrderBy (x => System.Math.Abs (x.Distance)).FirstOrDefault ();

			if ((best != null) &&
				(best.Button != drag.Widget))
			{
				int bestIndex = best.Button.Index;
				int thisIndex = drag.Widget.Index;
				var bounds = best.Button.ActualBounds;

				if ((best.Distance < 0) &&
					(bestIndex != thisIndex+1))
				{
					this.SetOutputDropTarget (bounds.Top + 1, bestIndex);
					return true;
				}
				if ((best.Distance >= 0) &&
				    (bestIndex != thisIndex-1))
				{
					this.SetOutputDropTarget (bounds.Bottom, bestIndex+1);
					return true;
				}
			}

			this.outputInsertionMark.Visibility = false;
			this.outputIndex = -1;

			return false;
		}

		private void SetScrollableAutoScroll(double y)
		{
			double y1 = y - this.cubesScrollable.Viewport.Aperture.Top;
			double y2 = this.cubesScrollable.Viewport.Aperture.Bottom - y;

			if (y1 > 0)
			{
				this.cubesScrollable.VerticalScroller.SimulateArrowEngaged (1);
				this.cubesScrollable.Window.SetEngageTimerDelay (this.cubesScrollable.VerticalScroller.AutoEngageRepeatPeriod);
			}
			else if (y2 > 0)
			{
				this.cubesScrollable.VerticalScroller.SimulateArrowEngaged (-1);
				this.cubesScrollable.Window.SetEngageTimerDelay (this.cubesScrollable.VerticalScroller.AutoEngageRepeatPeriod);
			}
			else
			{
				this.cubesScrollable.VerticalScroller.SimulateArrowEngaged (0);
			}
		}
		
		private void SetOutputDropTarget(double y, int index)
		{
			this.outputOffset = y;
			this.outputIndex = index;
			this.outputInsertionMark.Visibility = true;
			this.UpdateInsertionMark ();
		}

		private void UpdateInsertionMark()
		{
			double y = this.outputInsertionMark.Parent.MapRootToClient (this.cubesScrollable.Viewport.MapClientToRoot (new Point (0, this.outputOffset))).Y;
			double dy = System.Math.Floor (this.outputInsertionMark.PreferredHeight / 2);
			
			this.outputInsertionMark.Margins = new Margins (AbstractScroller.DefaultBreadth+1, this.cubeDetails.ActualWidth+1, 0, y - dy - 1);
		}



		private void HandleActiveDocumentChanged(object sender)
		{
			var doc = this.workspace.Document;

			if (doc == null)
            {
				return;
            }

			if (doc.ActiveCube == null)
            {
				return;
            }

			this.UpdateCubeList ();
			this.UpdateSelectedCube ();
		}
        
		private void HandleCubeClicked(GraphDataCube cube)
		{
			this.workspace.Document.SelectCube (cube.Guid);
			this.UpdateSelectedCube ();
		}

		private void HandleCubeMoved(GraphDataCube cube)
		{
			int index = this.outputIndex;

			if (index >= 0)
			{
				this.workspace.Document.SetCubeIndex (cube.Guid, index);
				this.UpdateCubeList ();
				this.UpdateSelectedCube ();
			}
		}
        
		private void HandleImportButtonClicked(object sender, MessageEventArgs e)
		{
			var dialog = new FileOpenDialog ()
			{
				OwnerWindow = this.container.Window,
				Title = "Importer des données",
			};

			dialog.Filters.Add ("csv", "Fichiers avec données tabulées", "*.csv");
			dialog.Filters.Add ("txt", "Fichiers texte", "*.txt");
			dialog.Filters.Add ("*", "Tous les fichiers", "*.*");

			dialog.OpenDialog ();

			if (dialog.Result == DialogResult.Accept)
            {
				GraphProgram.Application.ExecuteImport (dialog.FileName, System.Text.Encoding.Default);
            }
		}


		private readonly WorkspaceController	workspace;
		private Widgets.DataCubeFrame			container;
		private Scrollable						cubesScrollable;
		private DataCubeTableDetails			cubeDetails;
		private Separator						outputInsertionMark;
		private int								outputIndex;
		private double							outputOffset;
	}
}
