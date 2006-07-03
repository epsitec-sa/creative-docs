//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

using Epsitec.Common.Widgets.Collections;
using Epsitec.Common.Types;

namespace Epsitec.Common.Widgets
{
	public class MultiCommand : Command, IEnumType
	{
		public MultiCommand(string name) : base (name)
		{
			this.DefineStateObjectType (Types.DependencyObjectType.FromSystemType (typeof (MultiState)));
			this.commands = new List<Command> ();
		}

		public MultiCommand(string name, IEnumerable<Command> commands) : this (name)
		{
			this.AddRange (commands);
		}


		public void Add(Command command)
		{
			if (this.IsReadOnly)
			{
				throw new Exceptions.CommandLockedException (this.Name);
			}
			
			if (command != null)
			{
				if (this.commands.Contains (command) == false)
				{
					this.commands.Add (command);
				}
			}
		}

		public void AddRange(IEnumerable<Command> commands)
		{
			if (commands != null)
			{
				foreach (Command command in commands)
				{
					this.Add (command);
				}
			}
		}

		#region MultiState Class

		private class MultiState : CommandState
		{
			public MultiState()
			{
			}
			
			public Command SelectedCommand
			{
				get
				{
					return (Command) this.GetValue (MultiState.SelectedCommandProperty);
				}
				set
				{
					this.SetValue (MultiState.SelectedCommandProperty, value);
				}
			}

			public MultiCommand MultiCommand
			{
				get
				{
					return this.Command as MultiCommand;
				}
			}

			private static object CoerceSelectedCommandValue(DependencyObject obj, DependencyProperty property, object value)
			{
				if (value == null)
				{
					return null;
				}
				
				System.Diagnostics.Debug.Assert (property == MultiState.SelectedCommandProperty);
				System.Diagnostics.Debug.Assert (obj is MultiState);

				MultiState   state = (MultiState) obj;
				MultiCommand multi = (MultiCommand) state.Command;
				Command      command = (Command) value;

				if (multi.commands.Contains (command))
				{
					return command;
				}
				
				throw new System.ArgumentException ();
			}

			static MultiState()
			{
				DependencyPropertyMetadata metadata = new DependencyPropertyMetadata ();

				metadata.CoerceValue = MultiState.CoerceSelectedCommandValue;
				
				MultiState.SelectedCommandProperty = DependencyProperty.Register ("SelectedCommand", typeof (Command), typeof (MultiState), metadata);
			}

			public static readonly DependencyProperty SelectedCommandProperty;
		}

		#endregion

		#region CommandEnumValue Class
		
		private class CommandEnumValue : IEnumValue
		{
			public CommandEnumValue(int rank, Command command)
			{
				this.rank = rank;
				this.command = command;
			}
			
			#region IEnumValue Members

			int IEnumValue.Rank
			{
				get
				{
					return this.rank;
				}
			}

			System.Enum IEnumValue.Value
			{
				get
				{
					return Types.NotAnEnum.Instance;
				}
			}

			bool IEnumValue.IsHidden
			{
				get
				{
					return false;
				}
			}

			#endregion

			#region INameCaption Members

			long ICaption.CaptionId
			{
				get
				{
					return this.command.CaptionId;
				}
			}

			#endregion

			#region IName Members

			string IName.Name
			{
				get
				{
					return this.command.Name;
				}
			}

			#endregion

			private int rank;
			private INamedType command;
		}

		#endregion

		#region IEnumType Members

		IEnumerable<IEnumValue> IEnumType.Values
		{
			get
			{
				for (int i = 0; i < this.commands.Count; i++)
				{
					yield return new CommandEnumValue (i, this.commands[i]);
				}
			}
		}

		IEnumValue IEnumType.this[string name]
		{
			get
			{
				int rank = 0;
				
				foreach (Command command in this.commands)
				{
					if (command.Name == name)
					{
						return new CommandEnumValue (rank, command);
					}
					
					rank++;
				}
				
				return null;
			}
		}

		IEnumValue IEnumType.this[int rank]
		{
			get
			{
				return new CommandEnumValue (rank, this.commands[rank]);
			}
		}

		bool IEnumType.IsCustomizable
		{
			get
			{
				return false;
			}
		}

		bool IEnumType.IsDefinedAsFlags
		{
			get
			{
				return false;
			}
		}

		#endregion


		public static Command GetSelectedCommand(CommandState state)
		{
			if (state == null)
			{
				throw new System.ArgumentNullException ();
			}

			MultiState multi = state as MultiState;

			if (multi == null)
			{
				throw new System.ArgumentException ();
			}

			return multi.SelectedCommand;
		}

		public static void SetSelectedCommand(CommandState state, Command command)
		{
			if (state == null)
			{
				throw new System.ArgumentNullException ("Specified state is null");
			}
			
			MultiState multiState = state as MultiState;

			if (multiState == null)
			{
				throw new System.ArgumentException ("Specified state is not a MultiState");
			}

			if (multiState.SelectedCommand != command)
			{
				multiState.SelectedCommand = command;
				
				//	Update the active state of the sub-commands found in the same
				//	context as the multi-command state itself.
				
				MultiCommand   multiCommand = multiState.MultiCommand;
				CommandContext multiContext = multiState.CommandContext;

				foreach (Command item in multiCommand.commands)
				{
					CommandState itemState = multiContext.GetCommandState (item);
					itemState.ActiveState = (item == command) ? ActiveState.Yes : ActiveState.No;
				}
			}
		}
		
		private List<Command> commands;
	}
}
