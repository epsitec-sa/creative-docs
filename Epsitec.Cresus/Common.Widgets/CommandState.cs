//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

using Epsitec.Common.Widgets.Collections;
using Epsitec.Common.Types;

namespace Epsitec.Common.Widgets
{
	public class CommandState : DependencyObject
	{
		internal CommandState()
		{
		}
		
		public CommandState(Command command)
		{
			this.DefineCommand (command);
		}
		
		public Command							Command
		{
			get
			{
				return this.command;
			}
		}

		public CommandContext					CommandContext
		{
			get
			{
				return this.context;
			}
		}
		
		public bool								Enable
		{
			get
			{
				return this.enable;
			}
			set
			{
				if (this.enable != value)
				{
					this.enable = value;
					this.Synchronize ();
				}
			}
		}
		
		public ActiveState						ActiveState
		{
			get
			{
				return this.activeState;
			}
			set
			{
				if (this.activeState != value)
				{
					this.activeState = value;
					this.Synchronize ();
				}
			}
		}
		
		public string							AdvancedState
		{
			get
			{
				return CommandState.GetAdvancedState (this);
			}
			set
			{
				if (this.AdvancedState != value)
				{
					CommandState.SetAdvancedState (this, value);
					this.Synchronize ();
				}
			}
		}

		#region Internal Methods

		internal void DefineCommand(Command command)
		{
			this.command = command;
			this.command.Lockdown ();

			this.OverrideDefineCommand ();
		}

		internal void DefineCommandContext(CommandContext context)
		{
			this.context = context;
		}
		
#endregion

		protected virtual void OverrideDefineCommand()
		{
		}
		
		protected void Synchronize()
		{
			CommandCache.Default.InvalidateState (this);
		}
		
		public static void SetAdvancedState(DependencyObject obj, string value)
		{
			if (value == null)
			{
				obj.ClearValue (CommandState.AdvancedStateProperty);
			}
			else
			{
				obj.SetValue (CommandState.AdvancedStateProperty, value);
			}
		}
		
		public static string GetAdvancedState(DependencyObject obj)
		{
			return obj.GetValue (CommandState.AdvancedStateProperty) as string;
		}

		public static readonly DependencyProperty AdvancedStateProperty = DependencyProperty.RegisterAttached ("AdvancedState", typeof (string), typeof (CommandState), new DependencyPropertyMetadata (null));
		public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterReadOnly ("Command", typeof (Command), typeof (CommandState));

		private Command							command;
		private CommandContext					context;
		private ActiveState						activeState = ActiveState.No;
		private bool							enable = true;
	}
}
