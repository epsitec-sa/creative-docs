namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe CommandState permet de représenter l'état d'une commande tout
	/// en maintenant la synchronisation avec les widgets associés.
	/// </summary>
	public class CommandState : Support.CommandDispatcher.CommandState
	{
		public CommandState(string name) : this (name, Support.CommandDispatcher.Default)
		{
		}
		
		public CommandState(string name, Window window) : this (name, window.CommandDispatcher)
		{
		}
		
		public CommandState(string name, Support.CommandDispatcher dispatcher) : base (name, dispatcher)
		{
			this.state = WidgetState.Enabled | WidgetState.ActiveNo;
		}
		
		
		public bool							Enabled
		{
			get
			{
				return (this.state & WidgetState.Enabled) != 0;
			}
			set
			{
				if (this.Enabled != value)
				{
					if (value)
					{
						this.state |= WidgetState.Enabled;
					}
					else
					{
						this.state &= ~ WidgetState.Enabled;
					}
					
					foreach (Widget widget in this.FindWidgets ())
					{
						System.Diagnostics.Debug.WriteLine ("Enable="+value.ToString ()+" for "+widget.ToString ());
						widget.SetEnabled (value);
						widget.Invalidate ();
					}
				}
			}
		}
		
		public WidgetState					ActiveState
		{
			get
			{
				return this.state & WidgetState.ActiveMask;
			}
			set
			{
				System.Diagnostics.Debug.Assert ((value & WidgetState.ActiveMask) == value);
				
				if (this.ActiveState != value)
				{
					this.state &= ~WidgetState.ActiveMask;
					this.state |= value & WidgetState.ActiveMask;
					
					foreach (Widget widget in this.FindWidgets ())
					{
						widget.ActiveState = value;
						widget.Invalidate ();
					}
				}
			}
		}
		
		
		public Widget[] FindWidgets()
		{
			return Widget.FindAllCommandWidgets (this.regex, this.dispatcher);
		}
		
		
		public override void Synchronise()
		{
			bool        enabled = this.Enabled;
			WidgetState active  = this.ActiveState;
			
			foreach (Widget widget in this.FindWidgets ())
			{
				if ((widget.IsEnabled != enabled) ||
					(widget.ActiveState != active))
				{
					widget.SetEnabled (enabled);
					widget.ActiveState = active;
					widget.Invalidate ();
				}
			}
		}
		
		
		protected WidgetState				state;
	}
}
