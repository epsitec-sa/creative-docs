//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Graph
{
	public sealed class GraphCommands
	{
		public GraphCommands(GraphApplication application)
		{
			this.application = application;
			this.application.CommandDispatcher.RegisterController (this);
		}


		[Command(ApplicationCommands.Id.Undo)]
		private void UndoCommand()
		{
			this.application.Document.Undo ();
		}

		[Command (ApplicationCommands.Id.Redo)]
		private void RedoCommand()
		{
			this.application.Document.Redo ();
		}

		[Command (Res.CommandIds.File.ExportImage)]
		private void ExportImageCommand()
		{
			var doc = GraphProgram.Application.Document;

			System.Windows.Forms.SaveFileDialog dialog = new System.Windows.Forms.SaveFileDialog ()
			{
				AddExtension = true,
				AutoUpgradeEnabled = true,
				CheckPathExists = true,
				DefaultExt = ".emf",
				Filter = "Windows Enhanced Metafile (*.emf)|*.emf|Image PNG (*.png)|*.png|Image GIF (*.gif)|*.gif|Image Bitmap (*.bmp)|*.bmp",
				FilterIndex = 0,
				OverwritePrompt = true,
				RestoreDirectory = true,
				Title = "Exporter le graphique en tant qu'image",
				ValidateNames = true
			};

			var result = dialog.ShowDialog (GraphProgram.Application.Window.PlatformWindowObject as System.Windows.Forms.IWin32Window);

			if (result == System.Windows.Forms.DialogResult.OK)
			{
				this.application.Document.ExportImage (dialog.FileName);
			}
		}

		[Command (ApplicationCommands.Id.Copy)]
		private void CopyCommand()
		{
			this.application.Document.ExportImage ();
		}

		[Command (ApplicationCommands.Id.Properties)]
		private void PropertiesCommand()
		{
			this.application.SeriesPickerController.ShowWindow ();
		}


		private readonly GraphApplication application;
	}
}
