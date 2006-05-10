//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

using Epsitec.Common.Types;

namespace Epsitec.Common.Widgets
{
	public class CommandContext : DependencyObject
	{
		public CommandContext()
		{
		}

		public void SetLocalEnable(CommandState command, bool value)
		{
			bool oldValue;
			bool newValue = value;

			if (this.commands.TryGetValue (command.UniqueId, out oldValue))
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
					this.commands.Remove (command.UniqueId);
				}
				else
				{
					this.commands[command.UniqueId] = false;
				}

				if (command.Enable)
				{
					this.NotifyCommandEnableChanged (command);
				}
			}
		}

		public bool GetLocalEnable(CommandState command)
		{
			bool value;

			if (this.commands.TryGetValue (command.UniqueId, out value))
			{
				System.Diagnostics.Debug.Assert (value == false);

				return value;
			}
			else
			{
				return true;
			}
		}

		#region Private Methods

		private void NotifyCommandEnableChanged(CommandState command)
		{
			CommandCache.Default.Invalidate (command);
		}

		#endregion

		public static CommandContext GetContext(DependencyObject obj)
		{
			return (CommandContext) obj.GetValue (CommandContext.ContextProperty);
		}
		
		public static void SetContext(DependencyObject obj, CommandContext context)
		{
			obj.SetValue (CommandContext.ContextProperty, context);
		}

		public static void ClearContext(DependencyObject obj)
		{
			obj.ClearValueBase (CommandContext.ContextProperty);
		}
		
		
		public static readonly DependencyProperty ContextProperty = DependencyProperty.RegisterAttached ("Context", typeof (CommandContext), typeof (CommandContext));

		
		private Dictionary<int, bool> commands = new Dictionary<int, bool> ();
	}
}
