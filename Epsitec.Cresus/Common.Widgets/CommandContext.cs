//	Copyright � 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>CommandContext</c> class is used to locally disable commands
	/// defined by <c>Command</c>.
	/// </summary>
	public class CommandContext : DependencyObject
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:CommandContext"/> class.
		/// </summary>
		public CommandContext()
		{
			this.localDisables = new HashSet<int> ();
			this.groupDisables = new Dictionary<string, int> ();
			this.validations = new Dictionary<long, Validation> ();
			this.states = new Dictionary<long, CommandState> ();
			this.commandHandlers = new HashSet<ICommandHandler> ();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CommandContext"/> class.
		/// </summary>
		/// <param name="fence">If set to <c>true</c>, this command context will
		/// be handled as a fence.</param>
		public CommandContext(bool fence)
			: this ()
		{
			this.fence = fence;
		}

		public CommandContext(string name)
			: this ()
		{
			this.name = name;
		}

		public CommandContext(string name, bool fence)
			: this (fence)
		{
			this.name = name;
		}

#if false
		/// <summary>
		/// Gets the dispatchers in the dispatcher chain associated with this
		/// command context.
		/// </summary>
		/// <value>The dispatchers.</value>
		public IEnumerable<CommandDispatcher>	Dispatchers
		{
			get
			{
				if (this.dispatcherChain == null)
				{
					return Collections.EmptyEnumerable<CommandDispatcher>.Instance;
				}
				else
				{
					return this.dispatcherChain.Dispatchers;
				}
			}
		}
#endif

		/// <summary>
		/// Gets a value indicating whether this <see cref="CommandContext"/> is
		/// a fence. When a <see cref="CommandContextChain"/> is evaluating the
		/// state of a command and it reaches a fence, it stops. This basically
		/// means that any command states defined in parent contexts will become
		/// invisible.
		/// </summary>
		/// <value><c>true</c> if this is a fence; otherwise, <c>false</c>.</value>
		public bool Fence
		{
			get
			{
				return this.fence;
			}
		}

		/// <summary>
		/// Sets the local enable state of the specified command.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <param name="value">Enable if set to <c>true</c>, disable otherwise.</param>
		public void SetLocalEnable(Command command, bool value)
		{
			if (command == null)
			{
				return;
			}
			
			bool oldValue = this.localDisables.Contains (command.SerialId) ? false : true;
			bool newValue = value;

			if (newValue != oldValue)
			{
				//	The local enable is in fact implemented as a local disable;
				//	when the local disable is required, just put the command into
				//	the local disables hash set :
				
				if (newValue)
				{
					this.localDisables.Remove (command.SerialId);
				}
				else
				{
					this.localDisables.Add (command.SerialId);
				}

				this.NotifyCommandEnableChanged (command);
			}
		}

		/// <summary>
		/// Gets the local enable state of the command.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <returns><c>false</c> if the command is disabled locally, <c>true</c> otherwise.</returns>
		public bool GetLocalEnable(Command command)
		{
			if (this.localDisables.Contains (command.SerialId))
			{
				return false;
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
		/// Sets the group enable state based on a validation context.
		/// </summary>
		/// <param name="context">The validation context.</param>
		/// <param name="group">The command group.</param>
		/// <param name="enable">Enable group if set to <c>true</c>, otherwise disable.</param>
		public void SetGroupEnable(ValidationContext context, string group, bool enable)
		{
			long id = context.UniqueId;
			Validation record;

			if (this.validations.TryGetValue (id, out record) == false)
			{
				//	There is no record for the specifed context. If the caller is
				//	specifying that the group is enabled for the context, we have
				//	nothing to record, as this is the default.

				if (enable == false)
				{
					record = new Validation (context);
					this.validations[id] = record;

					record.GroupDisables.Add (group);
					this.IncrementGroupDisable (group);
				}
			}
			else
			{
				System.Diagnostics.Debug.Assert (record.Context == context);
				System.Diagnostics.Debug.Assert (record.GroupDisables.Count > 0);
				
				if (record.GroupDisables.Contains (group))
				{
					if (enable)
					{
						//	Remove the disable for the specified group if we have
						//	found a current disable (the caller wants to revoke the
						//	disable).

						record.GroupDisables.Remove (group);
						this.DecrementGroupDisable (group);

						if (record.GroupDisables.Count == 0)
						{
							//	We no longer need the record. Just release the memory
							//	associated to it.

							this.validations.Remove (id);
						}
					}
					else
					{
						this.IncrementGroupDisable (group);
					}
				}
				else if (enable == false)
				{
					//	Create a disable for the specified group, as it is not yet
					//	disabled.

					record.GroupDisables.Add (group);
					this.IncrementGroupDisable (group);
				}
			}
		}

		/// <summary>
		/// Gets the command state for the specified command. If there is no
		/// matching command state in this context, a new one will be created.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <returns>The command state.</returns>
		public CommandState GetCommandState(Command command)
		{
			CommandState state = this.FindCommandState (command);

			if (state == null)
			{
				state = command.CreateDefaultState (this);
				this.SetCommandState (command, state);
			}
			
			return state;
		}

		/// <summary>
		/// Clears the state of the command; the <c>CommandState</c> associated with the command will
		/// be removed from the command context.
		/// </summary>
		/// <param name="command">The command.</param>
		public void ClearCommandState(Command command)
		{
			this.SetCommandState (command, null);
		}



		public void AttachCommandHandler(ICommandHandler handler)
		{
			this.commandHandlers.Add (handler);
		}

		public void DetachCommandHandler(ICommandHandler handler)
		{
			this.commandHandlers.Remove (handler);
		}

		public T GetCommandHandler<T>()
			where T : class, ICommandHandler
		{
			return this.commandHandlers.Select (x => x as T).Where (x => x != null).FirstOrDefault ();
		}


		
		#region Internal Methods
		
		internal CommandState FindCommandState(Command command)
		{
			CommandState state;
			
			if (this.states.TryGetValue (command.SerialId, out state))
			{
				return state;
			}
			else
			{
				return null;
			}
		}

		internal void SetCommandState(Command command, CommandState state)
		{
			if (state == null)
			{
				this.states.Remove (command.SerialId);
			}
			else
			{
				this.states[command.SerialId] = state;
			}

			CommandCache.Instance.InvalidateCommand (command);
		}
		
#if false
		internal void UpdateDispatcherChain(Visual visual)
		{
			this.dispatcherChain = CommandDispatcherChain.BuildChain (visual);
		}
#endif

		#endregion

		#region Private Methods

		private void NotifyCommandEnableChanged(Command command)
		{
			CommandCache.Instance.InvalidateCommand (command);
		}

		private void NotifyGroupEnableChanged(string group)
		{
			CommandCache.Instance.InvalidateGroup (group);
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

		#region Private Validation Structure
		
		/// <summary>
		/// The <c>Validation</c> structure associates a validation context with
		/// a set of disabled groups.
		/// </summary>
		private struct Validation
		{
			public Validation(ValidationContext context)
			{
				this.weakContext = new Weak<ValidationContext> (context);
				this.groupDisables = new HashSet<string> ();
			}

			public ValidationContext			Context
			{
				get
				{
					return this.weakContext.Target;
				}
			}

			public HashSet<string>				GroupDisables
			{
				get
				{
					return this.groupDisables;
				}
			}

			readonly Weak<ValidationContext>	weakContext;
			readonly HashSet<string>			groupDisables;
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
			if (CommandContext.GetContext (obj) != context)
			{
				obj.SetValue (CommandContext.ContextProperty, context);

				CommandCache.Instance.InvalidateContext (context);
			}
		}

		/// <summary>
		/// Clears the <c>CommandContext</c> for the <c>DependencyObject</c>.
		/// </summary>
		/// <param name="obj">The dependency object.</param>
		public static void ClearContext(DependencyObject obj)
		{
			if (obj.ContainsLocalValue (CommandContext.ContextProperty))
			{
				CommandContext context = CommandContext.GetContext (obj);
				
				obj.ClearValue (CommandContext.ContextProperty);

				if (context != null)
				{
					CommandCache.Instance.InvalidateContext (context);
				}
			}
		}
		
		
		public static readonly DependencyProperty ContextProperty = DependencyProperty.RegisterAttached ("Context", typeof (CommandContext), typeof (CommandContext), new DependencyPropertyMetadata ().MakeNotSerializable ());

		
		readonly HashSet<int>					localDisables;
		readonly Dictionary<string, int>		groupDisables;
		readonly Dictionary<long, Validation>	validations;
		readonly Dictionary<long, CommandState>	states;
		readonly bool							fence;
		readonly string							name;
		readonly HashSet<ICommandHandler>		commandHandlers;

#if false
		private CommandDispatcherChain dispatcherChain;
#endif
	}
}
