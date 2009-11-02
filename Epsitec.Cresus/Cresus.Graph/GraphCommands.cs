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

			var dialog = new FileSaveDialog ()
			{
				DefaultExt = "emf",
				FilterIndex = 0,
				PromptForOverwriting = true,
				Title = "Exporter le graphique en tant qu'image",
			};

			dialog.Filters.Add ("EMF", "Windows Enhanced Metafile", "*.emf");
			dialog.Filters.Add ("PNG", "Image PNG", "*.png");
			dialog.Filters.Add ("GIF", "Image GIF", "*.gif");
			dialog.Filters.Add ("BMP", "Image Bitmap", "*.bmp");

			dialog.OpenDialog ();

			if (dialog.Result == DialogResult.Accept)
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

		[Command (ApplicationCommands.Id.Save)]
		private void SaveCommand()
		{
			this.Save (this.application.Document, false);
		}

		[Command (ApplicationCommands.Id.SaveAs)]
		private void SaveAsCommand()
		{
			throw new System.NotSupportedException ();
			this.Save (this.application.Document, true);
		}

		public bool Save(GraphDocument document, bool alwaysAsk)
		{
			if (string.IsNullOrEmpty (document.SavePath))
            {
				alwaysAsk = true;
            }

			if (alwaysAsk)
			{
				var dialog = new FileSaveDialog ()
				{
					DefaultExt = "crgraph",
					PromptForOverwriting = true,
					Title = "Enregistrer le graphe",
					FileName = document.SavePath ?? "",
				};

				dialog.Filters.Add ("CRGRAPH", "Document Crésus Graphe", "*.crgraph");

				dialog.OpenDialog ();

				if (dialog.Result == DialogResult.Accept)
				{
					document.SaveDocument (dialog.FileName);
					return document.IsDirty == false;
				}
				else
				{
					return false;
				}
			}
			else
			{
				if (document.IsDirty)
				{
					document.SaveDocument (document.SavePath);
				}

				return document.IsDirty == false;
			}
		}

		[Command (ApplicationCommands.Id.Open)]
		private void OpenCommand()
		{
			var dialog = new FileOpenDialog ()
			{
				DefaultExt = "crgraph",
				Title = "Ouvrir le graphe",
			};

			dialog.Filters.Add ("CRGRAPH", "Document Crésus Graphe", "*.crgraph");

			dialog.OpenDialog ();

			if (dialog.Result == DialogResult.Accept)
			{
				if (this.application.Document.IsEmpty)
				{
					this.application.Document.LoadDocument (dialog.FileName);
				}
				else
				{
					var info = new System.Diagnostics.ProcessStartInfo ()
					{
						Arguments = string.Concat ("-open ", @"""", dialog.FileName, @""""),
						FileName = Globals.ExecutablePath,
					};

					var process = System.Diagnostics.Process.Start (info);

					if (process != null)
                    {
						process.WaitForInputIdle ();
                    }
				}
			}
		}

		private readonly GraphApplication application;
	}
}
