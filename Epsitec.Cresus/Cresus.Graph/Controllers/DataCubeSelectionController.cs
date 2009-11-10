//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.UI;
using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;
using System;

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

			this.cubesScrollable.Viewport.IsAutoFitting = true;
		}

		private void UpdateCubeList()
		{
			this.cubesScrollable.Viewport.Children.Widgets.ForEach (x => x.Dispose ());

			foreach (var cube in this.workspace.Document.Cubes)
			{
				this.CreateCubeButton (cube);
			}
		}


		private void CreateCubeButton(Epsitec.Cresus.Graph.GraphDataCube cube)
		{
			var button = new Button ()
			{
				Parent = this.cubesScrollable.Viewport,
				Dock = DockStyle.Stacked,
				Text = DataCubeSelectionController.GetCubeDescription (cube),
				PreferredHeight = 80,
				ContentAlignment = ContentAlignment.MiddleLeft,
				ButtonStyle = ButtonStyle.ListItem,
				Padding = new Margins (8, 8, 4, 4),
			};

			this.CreateCubeDragHandler (cube, button);
		}
		
		private static string GetCubeDescription(Epsitec.Cresus.Graph.GraphDataCube cube)
		{
			var names1 = cube.NaturalTableDimensionNames;
			var names2 = from name in cube.DimensionNames
						 where !names1.Contains (name)
						 select name;

			var buffer = new System.Text.StringBuilder ();

			buffer.Append (FormattedText.Escape (cube.Title));
			buffer.Append ("<br/>");
			buffer.AppendJoin (", ", names1.Select (x => string.Concat ("<b>", FormattedText.Escape (x), "</b>")));
			
			if (!buffer.EndsWith ("<br/>"))
			{
				buffer.Append (", ");
			}
			
			buffer.AppendJoin (", ", names2.Select (x => FormattedText.Escape (x)));

			buffer.Append ("<br/><font size=\"80%\">");
			buffer.Append (FormattedText.Escape (DataCubeSelectionController.GetCubeLoadPath (cube)));
			buffer.Append ("</font>");

			return buffer.ToString ();
		}

		private static string GetCubeLoadPath(Epsitec.Cresus.Graph.GraphDataCube cube)
		{
			var path = cube.LoadPath;
			
			if (string.IsNullOrEmpty (path))
            {
				return "<inconnu>";
            }
			else if (path == GraphDataCube.LoadPathClipboard)
			{
				return "Presse-papier";
			}
			else
			{
				var p1 = System.IO.Path.GetDirectoryName (path);
				var p2 = System.IO.Path.GetFileName (path);

				int len = System.Math.Max (0, 60 - p2.Length);

				if (p1.Length > len)
				{
					return string.Concat (p1.Substring (0, len), "...", System.IO.Path.DirectorySeparatorChar.ToString (), p2);
				}
				else
				{
					return path;
				}
			}
		}

		private void CreateCubeDragHandler(GraphDataCube cube, Button button)
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
		}

        private void HandleImportButtonClicked(object sender, MessageEventArgs e)
		{
			var dialog = new FileOpenDialog ()
			{
				Owner = this.container.Window,
				Title = "Importer des données",
			};

			dialog.Filters.Add ("csv", "Fichiers avec données tabulées", "*.csv");
			dialog.Filters.Add ("*", "Tous les fichiers", "*.*");

			dialog.OpenDialog ();
		}


		private readonly WorkspaceController	workspace;
		private Widgets.DataCubeFrame			container;
		private Scrollable						cubesScrollable;
	}
}
