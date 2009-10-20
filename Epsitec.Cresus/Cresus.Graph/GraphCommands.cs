//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Graph.Controllers;

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
			this.application.Document.UndoRedo.Undo ();
		}

		[Command (ApplicationCommands.Id.Redo)]
		private void RedoCommand()
		{
			this.application.Document.UndoRedo.Redo ();
		}

		[Command (Res.CommandIds.File.ExportImage)]
		private void ExportImageCommand(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var chartViewController = ChartViewController.GetChartViewController (e.CommandContext);
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
		private void CopyCommand(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var chartViewController = ChartViewController.GetChartViewController (e.CommandContext);
			
			if (chartViewController != null)
            {
				chartViewController.SaveMetafile (null);
            }
		}


		private readonly GraphApplication application;
	}
}
