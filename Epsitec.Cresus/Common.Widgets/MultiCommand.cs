//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

using Epsitec.Common.Widgets.Collections;
using Epsitec.Common.Types;

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Widgets.MultiCommand))]

namespace Epsitec.Common.Widgets
{
	public class MultiCommand : DependencyObject
	{
		private MultiCommand()
		{
		}

		#region MultiState Class

		internal class MultiState : CommandState
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

			private static object CoerceSelectedCommandValue(DependencyObject obj, DependencyProperty property, object value)
			{
				if (value == null)
				{
					return null;
				}
				
				System.Diagnostics.Debug.Assert (property == MultiState.SelectedCommandProperty);
				System.Diagnostics.Debug.Assert (obj is MultiState);

				MultiState state   = (MultiState) obj;
				Command    multi   = state.Command;
				Command    command = (Command) value;

				if ((multi.HasMultiCommands) &&
					(multi.MultiCommands.Contains (command)))
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

#if false
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
					if (command.CommandId == name)
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
#endif

		public static Command GetSelectedCommand(CommandState state)
		{
			if (state == null)
			{
				throw new System.ArgumentNullException ();
			}

			MultiState multi = state as MultiState;

			if (multi == null)
			{
				return null;
			}
			else
			{
				return multi.SelectedCommand;
			}
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
				
				Command        multiCommand = multiState.Command;
				CommandContext multiContext = multiState.CommandContext;

				if (multiCommand.HasMultiCommands)
				{
					foreach (Command item in multiCommand.MultiCommands)
					{
						CommandState itemState = multiContext.GetCommandState (item);
						itemState.ActiveState = (item == command) ? ActiveState.Yes : ActiveState.No;
					}
				}
			}
		}

		public static Collections.CommandCollection GetCommands(DependencyObject obj)
		{
			return obj.GetValue (MultiCommand.CommandsProperty) as Collections.CommandCollection;
		}

		public static bool HasCommands(DependencyObject obj)
		{
			Collections.CommandCollection commands = obj.GetValueBase (MultiCommand.CommandsProperty) as Collections.CommandCollection;

			return (commands != null) && (commands.Count > 0);
		}

		
		private static object GetCommandsValue(DependencyObject obj)
		{
			Collections.CommandCollection commands = obj.GetValueBase (MultiCommand.CommandsProperty) as Collections.CommandCollection;

			if (commands == null)
			{
				commands = new Collections.CommandCollection ();
				obj.SetLocalValue (MultiCommand.CommandsProperty, commands);
				obj.InvalidateProperty (MultiCommand.CommandsProperty, null, commands);
			}

			return commands;
		}

		public static readonly DependencyProperty CommandsProperty	= DependencyProperty.RegisterAttached ("Commands", typeof (Collections.CommandCollection), typeof (MultiCommand), new DependencyPropertyMetadata (MultiCommand.GetCommandsValue));
	}
}
