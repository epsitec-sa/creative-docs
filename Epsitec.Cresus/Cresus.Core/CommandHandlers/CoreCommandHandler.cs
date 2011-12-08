//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Common.Dialogs;

using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Orchestrators;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.CommandHandlers
{
	public class CoreCommandHandler : ICommandHandler
	{
		public CoreCommandHandler(CoreCommandDispatcher commandDispatcher)
		{
			this.commandDispatcher = commandDispatcher;
			this.application = this.commandDispatcher.Host as CoreApplication;
		}


		public DataViewOrchestrator Orchestrator
		{
			get
			{
				return this.commandDispatcher.Host.FindActiveComponent<DataViewOrchestrator> ();
			}
		}

		[Command (ApplicationCommands.Id.Quit)]
		public void ProcessQuit(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.application.ShutdownApplication ();
		}


		[Command (Library.Res.CommandIds.Edition.SaveRecord)]
		public void ProcessEditionSaveRecord(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var orchestrator = this.Orchestrator;
			var navigator    = orchestrator.Navigator;

			navigator.PreserveNavigation (
				delegate
				{
					orchestrator.ClearActiveEntity ();
				});

			e.Executed = true;
		}

		[Command (Library.Res.CommandIds.Edition.DiscardRecord)]
		public void ProcessEditionDiscardRecord(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var orchestrator = this.Orchestrator;
			var navigator    = orchestrator.Navigator;

			navigator.PreserveNavigation (
				delegate
				{
					orchestrator.DefaultBusinessContext.Discard ();
					orchestrator.ClearActiveEntity ();
				});
		}

		[Command (Res.CommandIds.Edition.Print)]
		public void ProcessEditionPrint(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var orchestrator = this.Orchestrator;
			var mainViewController = orchestrator.MainViewController;

			mainViewController.PrintPrintableEntity ();
		}

		[Command (Res.CommandIds.Edition.Preview)]
		public void ProcessEditionPreview(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var orchestrator = this.Orchestrator;
			var mainViewController = orchestrator.MainViewController;

			mainViewController.PreviewPrintableEntity ();
		}

		[Command (Res.CommandIds.File.ImportV11)]
		public void ProcessFileImportV11(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var v11 = new V11.ImportFile ();
			v11.Import (this.application);
		}

		[Command (Res.CommandIds.File.ExportAccountingEntries)]
		public void ProcessFileExportAccountingEntries(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			FormattedText message = "";
			message += "Cette opération va préparer les écritures en vue de leur comptabilisation<br/>";
			message += "avec le logiciel Crésus Comptabilité, sous la forme de fichiers ecc/ecf.<br/>";
			message += "<br/>";
			message += "Pour l'instant, seules 4 écritures fixes de test (1 en 2010 et 3 en 2011)<br/>";
			message += "seront exportées.<br/>";
			message += "<br/>";
			message += "Voulez-vous exporter les écritures ?";

			var dialog = MessageDialog.CreateYesNo ("Comptabilisation", DialogIcon.Question, message);
			dialog.OpenDialog ();
			if (dialog.Result != DialogResult.Yes)
			{
				return;
			}

			var orchestrator = this.Orchestrator;
			var businessContext = orchestrator.DefaultBusinessContext;
			var businessSettings = businessContext.GetCachedBusinessSettings ();
			var businessFinance = businessSettings.Finance;

			var builder = new System.Text.StringBuilder ();
			bool ok = true;
			int total = 0;

			//	On simule 4 écritures.
			//	TODO: à remplacer par le vrai code dès que cela sera possible !
			var écritures = new List<string> ();
			écritures.Add ("01.09.2010/10/1000;2000;Virement 1;123.45");
			écritures.Add ("01.04.2011/12/1000;...;Virement 4.1;11.00/1030;...;Virement 4.2;22.00/...;2000;Virement 4.3;33.00");
			écritures.Add ("31.03.2011/11/1000;2000;Virement 2;100.00");
			écritures.Add ("26.07.2011/13/1000;2000;Virement 3;20.50");
#if false
			//	Génère volontairement 10 écritures fausses (sans libellé).
			for (int i = 0; i < 10; i++)
			{
				écritures.Add (string.Format ("27.07.2011/{0}/1000;2000;;1.00", (i+100).ToString ()));
			}
#endif

			var charts = businessFinance.GetAllChartsOfAccounts ().OrderBy (x => x.BeginDate);
			foreach (Business.Accounting.CresusChartOfAccounts chart in charts)
			{
				FormattedText result;
				int nbEcritures = Business.Accounting.CresusAccountingEntriesConnector.GenerateFiles (chart, écritures, out result, 6);

				if (nbEcritures == -1)  // erreur ?
				{
					ok = false;
				}
				else  // ok ?
				{
					total += nbEcritures;
				}

				builder.Append (result);
			}

			FormattedText summary;
			if (ok)
			{
				summary = TextFormatter.FormatText ("Les fichiers sont prêts pour la combtabilisation.").ApplyBold ().ApplyFontSize (13.0);

				if (total < écritures.Count)
				{
					int n = écritures.Count - total;
					summary += string.Format ("<br/><br/><i><b>Remarque:</b> {0} écritures n'ont pas été exportées, car elles ne correspondaient pas<br/>aux périodes des plans comptables définis dans les réglages de l'entreprise.</i>", n.ToString ());
				}
			}
			else
			{
				summary = TextFormatter.FormatText ("La comptabilisation n'est pas possible, car des erreurs sont survenues.").ApplyBold ().ApplyFontSize (13.0);
			}

			message = FormattedText.Concat (summary, "<br/><br/>", builder.ToString (), " ");

			dialog = MessageDialog.CreateOk ("Comptabilisation", ok ? DialogIcon.Question : DialogIcon.Warning, message);
			dialog.OpenDialog ();
		}

		[Command (Res.CommandIds.Feedback)]
		public void ProcessFeedback(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.commandDispatcher.Dispatch (dispatcher, e,
				delegate
				{
					using (var dialog = new Dialogs.FeedbackDialog (this.application))
					{
						dialog.OpenDialog ();
					}
				});
		}

		[Command (Res.CommandIds.Global.ShowSettings)]
		public void ProcessGlobalShowSettings(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.commandDispatcher.Dispatch (dispatcher, e,
				delegate
				{
					using (var dialog = new Dialogs.SettingsDialog (this.application))
					{
						dialog.OpenDialog ();
					}
				});
		}

		[Command (Res.CommandIds.Global.ShowDebug)]
		public void ProcessGlobalShowDebug(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.commandDispatcher.Dispatch (dispatcher, e,
				delegate
				{
					using (var dialog = new Dialogs.DebugDialog (this.application))
					{
						dialog.OpenDialog ();
					}
				});
		}

		[Command (Res.CommandIds.Global.ShowUserManager)]
		public void ProcessGlobalShowUserManager(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.commandDispatcher.Dispatch (dispatcher, e,
				delegate
				{
					var app = this.application;
					var manager = app.UserManager;
					manager.Authenticate (app, manager.AuthenticatedUser, softwareStartup: false);
				});
		}


		[Command (Core.Res.CommandIds.Test.Crash)]
		public void ProcessTestCrash()
		{
			throw new System.Exception ("Crashing the application on purpose");
		}

		private void HandleViewChanged(object sender)
		{
			this.UpdateCommandEnables ();
		}

		private void UpdateCommandEnables()
		{
			this.application.SetEnable (Res.Commands.Edition.Print, this.Orchestrator.MainViewController.GetPrintCommandEnable ());
		}

		#region ICommandHandler Members

		void ICommandHandler.UpdateCommandStates(object sender)
		{
			if (this.initialized == false)
			{
				this.initialized = true;
				this.Orchestrator.ViewChanged += this.HandleViewChanged;
			}

			this.UpdateCommandEnables ();
		}

		#endregion

		private readonly CoreCommandDispatcher	commandDispatcher;
		private readonly CoreApplication		application;

		private bool initialized;
	}
}
