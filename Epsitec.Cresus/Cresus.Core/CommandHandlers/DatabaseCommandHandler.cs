//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Controllers;

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


		[Command (Core.Res.CommandIds.Base.ShowCustomers)]
		public void ProcessBaseShowCustomers(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.SelectDatabase ("Customers", e);
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

		private void SelectDatabase(string databaseName, CommandEventArgs e)
		{
			var activeState = e.CommandState;
			var context     = e.CommandContext;
			var controller  = CoreApplication.GetController<BrowserViewController> (context);

			this.UpdateActiveCommandState (activeState);
			
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

		private readonly CoreCommandDispatcher commandDispatcher;
		private readonly HashSet<CommandState> databaseCommandStates;
	}
}
