//	Copyright � 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

using Epsitec.Common.Widgets.Collections;
using Epsitec.Common.Types;

namespace Epsitec.Common.Widgets
{
	public class CommandState : DependencyObject
	{
		public CommandState()
		{
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
				return Command.GetAdvancedState (this);
			}
			set
			{
				if (this.AdvancedState != value)
				{
					Command.SetAdvancedState (this, value);
					this.Synchronize ();
				}
			}
		}

		private void Synchronize()
		{
			CommandCache.Default.InvalidateState (this);
		}
		
		public static void SetAdvancedState(DependencyObject obj, string value)
		{
			if (value == null)
			{
				obj.ClearValueBase (CommandState.AdvancedStateProperty);
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

		private ActiveState						activeState = ActiveState.No;
		private bool							enable = true;
	}
}
