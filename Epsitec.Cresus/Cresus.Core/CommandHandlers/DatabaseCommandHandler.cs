//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.BrowserControllers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.CommandHandlers
{
	public class DatabaseCommandHandler : ICommandHandler
	{
		public DatabaseCommandHandler(CoreCommandDispatcher commandDispatcher)
		{
			this.commandDispatcher = commandDispatcher;
			this.databaseCommandStates = new HashSet<CommandState> ();

			this.SetupDatabaseCommands ();
			this.SetupDatabaseCommandStates ();
		}


        public void UpdateActiveCommandState(string databaseName)
        {
			var commandName  = string.Concat (DatabaseCommandHandler.showDatabaseCommandPrefix, databaseName);
			var commandState = this.databaseCommandStates.Where (state => state.Command.Name == commandName).FirstOrDefault ();

			if (commandState != null)
            {
				this.UpdateActiveCommandState (commandState);
			}
        }

		public string SelectedDatabaseCommandName
		{
			get
			{
				return this.databaseCommandStates
						.Where (x => x.ActiveState == ActiveState.Yes)
						.Select (x => x.Command.Name)
						.FirstOrDefault () as string;
			}
		}


		[Command (Core.Res.CommandIds.Base.ShowCustomers)]
		[Command (Core.Res.CommandIds.Base.ShowArticleDefinitions)]
		[Command (Core.Res.CommandIds.Base.ShowDocuments)]
		[Command (Core.Res.CommandIds.Base.ShowInvoiceDocuments)]
		[Command (Core.Res.CommandIds.Base.ShowBusinessSettings)]
		[Command (Core.Res.CommandIds.Base.ShowImages)]
		[Command (Core.Res.CommandIds.Base.ShowImageBlobs)]
		[Command (Core.Res.CommandIds.Base.ShowWorkflowDefinitions)]
		[Command (Core.Res.CommandIds.Base.ShowDocumentCategory)]
		[Command (Core.Res.CommandIds.Base.ShowDocumentOptions)]
		public void ProcessBaseGenericShow(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			//	The generic Base.Show command handler uses the name of the command to
			//	select the matching database (e.g. ShowCustomers => selects "Customers").

			this.SelectDatabase (e);
		}

		

		private void SetupDatabaseCommands()
		{
			//	Make sure all command objects are properly initialized before trying
			//	to iterate on the available commands in SetupDatabaseCommandStates :

			Res.Initialize ();
		}
		
		private void SetupDatabaseCommandStates()
		{
			foreach (var command in Command.FindAll (command => command.Group == DatabaseCommandHandler.databaseSelectionGroup))
			{
				this.databaseCommandStates.Add (this.commandDispatcher.GetState (command));
			}
		}

		private void SelectDatabase(CommandEventArgs e)
		{
			var commandName  = e.Command.Name;

			System.Diagnostics.Debug.Assert (commandName.StartsWith (DatabaseCommandHandler.showDatabaseCommandPrefix));

			var databaseName = commandName.Substring (DatabaseCommandHandler.showDatabaseCommandPrefix.Length);
			var activeState  = e.CommandState;
			var context      = e.CommandContext;
			var controller   = CoreApplication.GetController<BrowserViewController> (context);

			controller.SelectDataSet (databaseName);
			this.OnChanged ();
		}

		private void UpdateActiveCommandState(CommandState activeState)
		{
			foreach (var state in this.databaseCommandStates)
			{
				if (state == activeState)
				{
					state.ActiveState = ActiveState.Yes;
				}
				else
				{
					state.ActiveState = ActiveState.No;
				}
			}
		}


		private void OnChanged()
		{
			if (this.Changed != null)
			{
				this.Changed (this);
			}
		}

		public event EventHandler Changed;


		private static readonly string databaseSelectionGroup = "DatabaseSelection";
		private static readonly string showDatabaseCommandPrefix = "Base.Show";

        private readonly CoreCommandDispatcher commandDispatcher;
		private readonly HashSet<CommandState> databaseCommandStates;
	}
}
