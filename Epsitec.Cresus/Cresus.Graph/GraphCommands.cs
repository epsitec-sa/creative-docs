//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Dialogs;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Graph.Controllers;

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

			if (chartViewController == null)
            {
				return;
            }

			var dialog = new FileSave ()
			{
				DefaultExt = ".emf",
				FilterIndex = 0,
				PromptForOverwriting = true,
				Title = "Exporter le graphique en tant qu'image",
			};

			dialog.Filters.Add ("EMF", "Windows Enhanced Metafile", "*.emf");
			dialog.Filters.Add ("PNG", "Image PNG", "*.png");
			dialog.Filters.Add ("GIF", "Image GIF", "*.gif");
			dialog.Filters.Add ("BMP", "Image Bitmap", "*.bmp");

			dialog.OpenDialog ();

			if (dialog.DialogResult == DialogResult.Accept)
			{
				chartViewController.ExportImage (dialog.FileName);
			}
		}

		[Command (ApplicationCommands.Id.Copy)]
		private void CopyCommand(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var chartViewController = ChartViewController.GetChartViewController (e.CommandContext);

			if (chartViewController == null)
			{
				return;
			}

			chartViewController.SaveMetafile (null);
		}

		[Command (ApplicationCommands.Id.Print)]
		private void PrintCommand(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var chartViewController = ChartViewController.GetChartViewController (e.CommandContext);

			if (chartViewController == null)
			{
				return;
			}

			chartViewController.Print ();
		}


		private readonly GraphApplication application;
	}
}
