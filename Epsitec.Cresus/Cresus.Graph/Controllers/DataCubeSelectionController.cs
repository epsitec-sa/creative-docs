//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.UI;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

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
				//Text = "<img src=\"manifest:Epsitec.Common.Widgets.Images.Cmd.Paste.icon\" dx=\"31\" dy=\"31\"/>",
				PreferredWidth = 72,
//				ButtonStyle = ButtonStyle.ToolItem,
			};

			closeButton.Clicked +=
				delegate
				{
					this.container.FindDataCubeButton ().SimulateClicked ();
				};

			importButton.Clicked += this.HandleImportButtonClicked;
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
	}
}
