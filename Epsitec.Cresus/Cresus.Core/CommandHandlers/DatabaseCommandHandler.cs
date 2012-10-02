//	Copyright © 2010-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.BrowserControllers;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Metadata;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.CommandHandlers
{
	/// <summary>
	/// The <c>DatabaseCommandHandler</c> handles the database related commands.
	/// </summary>
	public class DatabaseCommandHandler : ICommandHandler
	{
		public DatabaseCommandHandler(CoreCommandDispatcher commandDispatcher)
		{
			this.commandDispatcher = commandDispatcher;
			this.databaseCommandStates = new HashSet<CommandState> ();

			this.SetupDatabaseCommandStates ();
		}


		public string SelectedDatabaseCommandName
		{
			get
			{
				return this.databaseCommandStates
						.Where (x => x.ActiveState == ActiveState.Yes)
						.Select (x => x.Command.Name)
						.FirstOrDefault ();
			}
		}

		
		private void ProcessBaseGenericShow(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			//	The generic Base.Show command handler uses the name of the command to
			//	select the matching database (e.g. ShowCustomers => selects "Customers").

			this.SelectDatabase (e.Command);
		}

		

		
		private void SetupDatabaseCommandStates()
		{
			foreach (var command in Command.FindAll (command => command.Group == DatabaseCommandHandler.DatabaseSelectionGroup))
			{
				this.databaseCommandStates.Add (this.commandDispatcher.GetState (command));
				this.commandDispatcher.CommandDispatcher.Register (command, this.ProcessBaseGenericShow);
			}
		}

		private void SelectDatabase(Command command)
		{
			var browserController = this.commandDispatcher.GetApplicationComponent<BrowserViewController> ();
			var dataSetMetadata   = DataStoreMetadata.Current.FindDataSet (command);

			browserController.SelectDataSet (dataSetMetadata);
			
			this.OnChanged ();
		}

		private static string GetDataSetName(string commandName)
		{
			System.Diagnostics.Debug.Assert (commandName.StartsWith (DatabaseCommandHandler.ShowDatabaseCommandPrefix), string.Format ("Command {0} does not start with the expected {1} prefix", commandName, DatabaseCommandHandler.ShowDatabaseCommandPrefix));

			return commandName.Substring (DatabaseCommandHandler.ShowDatabaseCommandPrefix.Length);
		}
		
		private void UpdateActiveCommandState(string dataSetName)
		{
			var commandName  = string.Concat (DatabaseCommandHandler.ShowDatabaseCommandPrefix, dataSetName);
			var commandState = this.databaseCommandStates.Where (state => state.Command.Name == commandName).FirstOrDefault ();

			if (commandState != null)
			{
				this.UpdateActiveCommandState (commandState);
			}
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


		#region ICommandHandler Members

		void ICommandHandler.UpdateCommandStates(object sender)
		{
			var controller = sender as BrowserViewController;

			if (controller != null)
			{
				this.UpdateActiveCommandState (controller.DataSetName);
			}
		}

		#endregion

		public event EventHandler				Changed;


		private static readonly string DatabaseSelectionGroup = "DatabaseSelection";
		private static readonly string ShowDatabaseCommandPrefix = "Base.Show";

		private readonly CoreCommandDispatcher	commandDispatcher;
		private readonly HashSet<CommandState>	databaseCommandStates;
	}
}
