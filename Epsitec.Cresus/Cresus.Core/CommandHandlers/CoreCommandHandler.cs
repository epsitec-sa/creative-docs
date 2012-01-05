//	Copyright © 2010-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	public sealed class CoreCommandHandler : ICommandHandler
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
			var orchestrator = this.Orchestrator;
			var businessContext = orchestrator.DefaultBusinessContext;

			Epsitec.Cresus.Core.Library.Business.EccEcfExport.EccEcfExport.Export (businessContext);
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

		private bool							initialized;
	}
}
