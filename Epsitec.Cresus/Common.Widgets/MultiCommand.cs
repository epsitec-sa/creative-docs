//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

using Epsitec.Common.Widgets.Collections;
using Epsitec.Common.Types;

namespace Epsitec.Common.Widgets
{
	public class MultiCommand : Command
	{
		public MultiCommand(string name) : base (name)
		{
			this.StateObjectType = Types.DependencyObjectType.FromSystemType (typeof (MultiState));
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

			protected override void OverrideDefineCommand()
			{
				base.OverrideDefineCommand ();
			}

			public static DependencyProperty SelectedCommandProperty = DependencyProperty.Register ("SelectedCommand", typeof (Command), typeof (MultiState));
		}

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
				throw new System.ArgumentNullException ();
			}

			MultiState multi = state as MultiState;

			if (multi == null)
			{
				throw new System.ArgumentException ();
			}

			multi.SelectedCommand = command;
		}
		
		private List<Command> commands;
	}
}
