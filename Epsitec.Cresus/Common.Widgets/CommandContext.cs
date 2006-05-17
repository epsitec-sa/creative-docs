//	Copyright � 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

using Epsitec.Common.Types;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>CommandContext</c> class is used to locally disable commands
	/// defined by <c>CommandState</c>.
	/// </summary>
	public class CommandContext : DependencyObject
	{
		public CommandContext()
		{
		}

		public IEnumerable<CommandDispatcher> Dispatchers
		{
			get
			{
				if (this.dispatcherChain == null)
				{
					return CommandDispatcherChain.EmptyDispatcherEnumeration;
				}
				else
				{
					return this.dispatcherChain.Dispatchers;
				}
			}
		}

		/// <summary>
		/// Sets the local enable state of the command.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <param name="value">Enable if set to <c>true</c>, disable otherwise.</param>
		public void SetLocalEnable(CommandState command, bool value)
		{
			if (command == null)
			{
				return;
			}
			
			bool oldValue;
			bool newValue = value;

			if (this.commandEnables.TryGetValue (command.UniqueId, out oldValue))
			{
				System.Diagnostics.Debug.Assert (oldValue == false);
			}
			else
			{
				oldValue = true;
			}

			if (newValue != oldValue)
			{
				if (newValue)
				{
					this.commandEnables.Remove (command.UniqueId);
				}
				else
				{
					this.commandEnables[command.UniqueId] = false;
				}

				if (command.Enable)
				{
					this.NotifyCommandEnableChanged (command);
				}
			}
		}

		/// <summary>
		/// Sets the local enable state of the command.
		/// </summary>
		/// <param name="commandName">The command name.</param>
		/// <param name="value">Enable if set to <c>true</c>, disable otherwise.</param>
		public void SetLocalEnable(string commandName, bool value)
		{
			this.SetLocalEnable (CommandState.Find (commandName), value);
		}

		/// <summary>
		/// Gets the local enable state of the command.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <returns><c>false</c> if the command is disabled locally, <c>true</c> otherwise.</returns>
		public bool GetLocalEnable(CommandState command)
		{
			bool value;

			if (this.commandEnables.TryGetValue (command.UniqueId, out value))
			{
				System.Diagnostics.Debug.Assert (value == false);

				return value;
			}
			else
			{
				string group = command.Group;
				int disables;

				if ((!string.IsNullOrEmpty (group)) &&
					(this.groupDisables.TryGetValue (group, out disables)))
				{
					System.Diagnostics.Debug.Assert (disables > 0);

					return false;
				}
				else
				{
					return true;
				}
			}
		}

		/// <summary>
		/// Sets the group enable state.
		/// </summary>
		/// <param name="context">The validation context.</param>
		/// <param name="group">The command group.</param>
		/// <param name="value">Enable group if set to <c>true</c>, otherwise disable.</param>
		public void SetGroupEnable(ValidationContext context, string group, bool value)
		{
			long id = context.UniqueId;
			Record record;

			if (this.records.TryGetValue (id, out record) == false)
			{
				//	There is no record for the specifed context. If the caller is
				//	specifying that the group is enabled for the context, we have
				//	nothing to record, as this is the default.
				
				if (value == true)
				{
					return;
				}

				record = new Record (context);
				this.records[id] = record;
				record.GroupEnable[group] = false;

				this.IncrementGroupDisable (group);
			}
			else
			{
				bool enable;

				System.Diagnostics.Debug.Assert (record.Context == context);
				System.Diagnostics.Debug.Assert (record.GroupEnable.Count > 0);
				
				if (record.GroupEnable.TryGetValue (group, out enable))
				{
					System.Diagnostics.Debug.Assert (enable == false);

					if (value == true)
					{
						//	Remove the disable for the specified group if we have
						//	found a current disable (the caller wants to revoke the
						//	disable).

						record.GroupEnable.Remove (group);

						this.DecrementGroupDisable (group);

						if (record.GroupEnable.Count == 0)
						{
							//	We no longer need the record. Just release the memory
							//	associated to it.

							this.records.Remove (id);
						}
					}
				}
				else if (value == false)
				{
					//	Create a disable for the specified group, as it is not yet
					//	disabled.

					record.GroupEnable[group] = false;
					
					this.IncrementGroupDisable (group);
				}
			}
		}

		internal void UpdateDispatcherChain(Visual visual)
		{
			this.dispatcherChain = CommandDispatcherChain.BuildChain (visual);
		}


		#region Private Methods

		private void NotifyCommandEnableChanged(CommandState command)
		{
			CommandCache.Default.InvalidateCommand (command);
		}

		private void NotifyGroupEnableChanged(string group)
		{
			CommandCache.Default.InvalidateGroup (group);
		}

		private void IncrementGroupDisable(string group)
		{
			int disables;

			lock (this.groupDisables)
			{
				if (this.groupDisables.TryGetValue (group, out disables))
				{
					System.Diagnostics.Debug.Assert (disables > 0);

					disables++;
					this.groupDisables[group] = disables;
				}
				else
				{
					this.groupDisables[group] = 1;
					this.NotifyGroupEnableChanged (group);
				}
			}
		}

		private void DecrementGroupDisable(string group)
		{
			int disables;

			lock (this.groupDisables)
			{
				if (this.groupDisables.TryGetValue (group, out disables))
				{
					System.Diagnostics.Debug.Assert (disables > 0);

					disables--;

					if (disables > 0)
					{
						this.groupDisables[group] = disables;
					}
					else
					{
						this.groupDisables.Remove (group);
						this.NotifyGroupEnableChanged (group);
					}
				}
				else
				{
					throw new System.InvalidOperationException ();
				}
			}
		}

		#endregion

		#region Private Record Structure
		
		private struct Record
		{
			public Record(ValidationContext context)
			{
				this.weakContext = new System.WeakReference (context);
				this.groupEnable = new Dictionary<string, bool> ();
			}

			public ValidationContext Context
			{
				get
				{
					return this.weakContext.Target as ValidationContext;
				}
			}

			public Dictionary<string, bool> GroupEnable
			{
				get
				{
					return this.groupEnable;
				}
			}

			System.WeakReference weakContext;
			Dictionary<string, bool> groupEnable;
		}

		#endregion


		/// <summary>
		/// Gets the <c>CommandContext</c> from the <c>DependencyObject</c>.
		/// </summary>
		/// <param name="obj">The dependency object.</param>
		/// <returns></returns>
		public static CommandContext GetContext(DependencyObject obj)
		{
			return (CommandContext) obj.GetValue (CommandContext.ContextProperty);
		}

		/// <summary>
		/// Sets the <c>CommandContext</c> for the <c>DependencyObject</c>.
		/// </summary>
		/// <param name="obj">The dependency object.</param>
		/// <param name="context">The context.</param>
		public static void SetContext(DependencyObject obj, CommandContext context)
		{
			obj.SetValue (CommandContext.ContextProperty, context);
		}

		/// <summary>
		/// Clears the <c>CommandContext</c> for the <c>DependencyObject</c>.
		/// </summary>
		/// <param name="obj">The dependency object.</param>
		public static void ClearContext(DependencyObject obj)
		{
			obj.ClearValueBase (CommandContext.ContextProperty);
		}
		
		
		public static readonly DependencyProperty ContextProperty = DependencyProperty.RegisterAttached ("Context", typeof (CommandContext), typeof (CommandContext));

		
		private Dictionary<int, bool> commandEnables = new Dictionary<int, bool> ();
		private Dictionary<string, int> groupDisables = new Dictionary<string, int> ();
		private Dictionary<long, Record> records = new Dictionary<long, Record> ();
		private CommandDispatcherChain dispatcherChain;
	}
}
