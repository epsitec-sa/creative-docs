//	Copyright © 2006-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Widgets.Collections;

using System.Collections.Generic;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>CommandState</c> class stores the state of a command, i.e. if a
	/// command is currently enabled and the active state of the associated UI
	/// visuals.
	/// </summary>
	public class CommandState : DependencyObject
	{
		protected CommandState()
		{
		}

		/// <summary>
		/// Gets the associated command.
		/// </summary>
		/// <value>The command.</value>
		public Command							Command
		{
			get
			{
				return (Command) this.GetValue (CommandState.CommandProperty);
			}
		}

		/// <summary>
		/// Gets the associated command context.
		/// </summary>
		/// <value>The command context.</value>
		public CommandContext					CommandContext
		{
			get
			{
				return (CommandContext) this.GetValue (CommandState.CommandContextProperty);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the associated command is
		/// currently enabled.
		/// </summary>
		/// <value><c>true</c> if the command is enabled; otherwise, <c>false</c>.</value>
		public bool								Enable
		{
			get
			{
				return !this.disable;
			}
			set
			{
				if (this.disable == value)
				{
					this.disable = !value;
					this.Synchronize ();
				}
			}
		}

		/// <summary>
		/// Gets or sets the active state of the associated command.
		/// </summary>
		/// <value>The active state of the command.</value>
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

		/// <summary>
		/// Gets or sets the advanced state of the command.
		/// </summary>
		/// <value>The advanced state of the command.</value>
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

		/// <summary>
		/// Defines the associated command.
		/// </summary>
		/// <param name="command">The command.</param>
		internal void DefineCommand(Command command)
		{
			command.Freeze ();
			this.SetValue (CommandState.CommandProperty, command);

			this.OverrideDefineCommand ();
		}

		/// <summary>
		/// Defines the associated command context.
		/// </summary>
		/// <param name="context">The context.</param>
		internal void DefineCommandContext(CommandContext context)
		{
			System.Diagnostics.Debug.Assert (this.Command != null);
			System.Diagnostics.Debug.Assert (this.Command.IsReadOnly);

			this.SetValue (CommandState.CommandContextProperty, context);
		}
		
		#endregion

		/// <summary>
		/// Called immediately after a command is defined for this command state.
		/// This method is meant to be overridden.
		/// </summary>
		protected virtual void OverrideDefineCommand()
		{
		}

		/// <summary>
		/// Synchronizes the command state; this will let the command cache handle
		/// the (asynchronous) synchronization.
		/// </summary>
		private void Synchronize()
		{
			CommandCache.Instance.InvalidateState (this);
		}


		/// <summary>
		/// Sets the advanced state for the specified object.
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <param name="value">The advanced state.</param>
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

		/// <summary>
		/// Gets the advanced state for the specified object.
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <returns>The advanced state or <c>null</c>.</returns>
		public static string GetAdvancedState(DependencyObject obj)
		{
			return obj.GetValue (CommandState.AdvancedStateProperty) as string;
		}

		public static readonly DependencyProperty AdvancedStateProperty  = DependencyProperty<CommandState>.RegisterAttached ("AdvancedState", typeof (string));
		public static readonly DependencyProperty CommandProperty        = DependencyProperty<CommandState>.RegisterReadOnly (x => x.Command);
		public static readonly DependencyProperty CommandContextProperty = DependencyProperty<CommandState>.RegisterReadOnly (x => x.CommandContext);

		private ActiveState						activeState;
		private bool							disable;
	}
}
