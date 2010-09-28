//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
        


		[Command (Core.Res.CommandIds.Base.ShowCustomers)]
		public void ProcessBaseShowCustomers(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.SelectDatabase (e);
		}

		[Command (Core.Res.CommandIds.Base.ShowArticleDefinitions)]
		public void ProcessBaseShowArticleDefinitions(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.SelectDatabase (e);
		}

		[Command (Core.Res.CommandIds.Base.ShowDocuments)]
		public void ProcessBaseShowDocuments(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.SelectDatabase (e);
		}

		[Command (Core.Res.CommandIds.Base.ShowInvoiceDocuments)]
		public void ProcessBaseShowInvoiceDocuments(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.SelectDatabase (e);
		}

		[Command (Core.Res.CommandIds.Base.ShowBusinessSettings)]
		public void ProcessBaseShowBusinessSettings(CommandDispatcher dispatcher, CommandEventArgs e)
		{
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
		private static readonly string databaseSelectionGroup = "DatabaseSelection";
		private static readonly string showDatabaseCommandPrefix = "Base.Show";

        private readonly CoreCommandDispatcher commandDispatcher;
		private readonly HashSet<CommandState> databaseCommandStates;
	}
}
