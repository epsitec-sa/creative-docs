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

			GraphProgram.Application.ActiveDocumentChanged +=
				(sender) =>
				{
					this.UpdateCubeList ();
					this.UpdateSelectedCube ();
				};

			this.container = container;

			var innerFrame = new FrameBox ()
			{
				Parent = this.container,
				BackColor = Color.FromRgb (1, 0.8, 0.8),
				Dock = DockStyle.Fill,
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
			};

			var topFrame = new FrameBox ()
			{
				Parent = innerFrame,
				Name = "top",
				BackColor = Color.FromRgb (1, 1, 0.8),
				Dock = DockStyle.Top,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				PreferredHeight = 76,
				Padding = new Margins (0, 0, 2, 2),
			};

			var middleFrame = new FrameBox ()
			{
				Parent = innerFrame,
				Name = "mid",
				BackColor = Color.FromRgb (0.8, 1, 0.8),
				Dock = DockStyle.Fill,
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
			};

			var bottomFrame = new FrameBox ()
			{
				Parent = innerFrame,
				Name = "bot",
				BackColor = Color.FromRgb (0.8, 1, 1),
				Dock = DockStyle.Bottom,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
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
				IconName = "manifest:Epsitec.Common.Graph.Images.Glyph.DropItem.icon",
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
				PaintViewportFrame = true,
				ViewportFrameMargins = new Margins (0, -1, 0, 0),
			};

			this.cubeDetails = new DataCubeTableDetails ()
			{
				Parent = middleFrame,
				Dock = DockStyle.Fill,
			};

			this.cubesScrollable.Viewport.IsAutoFitting = true;
		}

		private void UpdateCubeList()
		{
			this.cubesScrollable.Viewport.Children.Widgets.ForEach (x => x.Dispose ());

			bool first = true;

			foreach (var cube in this.workspace.Document.Cubes)
			{
				if (first)
				{
					first = false;
				}
				else
				{
					var sep = new Separator ()
					{
						Parent = this.cubesScrollable.Viewport,
						Dock = DockStyle.Stacked,
						PreferredHeight = 1,
						IsHorizontalLine = true,
					};
				}

				this.CreateCubeButton (cube);
			}
		}


		private void UpdateSelectedCube()
		{
			var buttons = from button in this.cubesScrollable.Viewport.Children.Widgets
						  where button is DataCubeTableButton
						  select button as DataCubeTableButton;

			var cube = this.workspace.Document.ActiveCube;
			var guid = cube == null ? System.Guid.Empty : cube.Guid;

			this.cubeDetails.Cube = cube;

			buttons.ForEach (button => button.SetSelected (button.Cube.Guid == guid));
		}
        
		private void CreateCubeButton(GraphDataCube cube)
		{
			var button = new DataCubeTableButton ()
			{
				Parent = this.cubesScrollable.Viewport,
				Dock = DockStyle.Stacked,
				Cube = cube,
				PreferredHeight = 60,
			};

			this.CreateCubeDragHandler (cube, button);
		}
		

		private void CreateCubeDragHandler(GraphDataCube cube, Widget button)
		{
			DragController.Attach (button,
				(action) =>
				{
					action.LockX = true;
				},
				(action, moved) =>
				{
					if (moved)
					{
						//	Cube was moved to a new position
					}
					else
					{
						this.HandleCubeClicked (cube);
					}
				});
		}

		private void HandleCubeClicked(GraphDataCube cube)
		{
			this.workspace.Document.SelectCube (cube.Guid);
			this.UpdateSelectedCube ();
		}

        private void HandleImportButtonClicked(object sender, MessageEventArgs e)
		{
			var dialog = new FileOpenDialog ()
			{
				Owner = this.container.Window,
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
	}
}
