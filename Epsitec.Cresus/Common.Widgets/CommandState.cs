namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe CommandState permet de repr�senter l'�tat d'une commande tout
	/// en maintenant la synchronisation avec les widgets associ�s.
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
		
		
		public static CommandState Find(string command_name, Support.CommandDispatcher dispatcher)
		{
			if (dispatcher == null)
			{
				return null;
			}
			
			System.Diagnostics.Debug.Assert (command_name != null);
			System.Diagnostics.Debug.Assert (command_name != "");
			
			Support.CommandDispatcher.CommandState[] states = dispatcher.FindCommandStates (command_name);
			
			for (int i = 0; i < states.Length; i++)
			{
				CommandState state = states[i] as CommandState;
				
				if (state != null)
				{
					return state;
				}
			}
			
			//	Il n'y a pas d'objet CommandState avec ce nom de commande. Il faut donc
			//	en cr�er un. La prochaine fois que cette m�thode sera appel�e, on retournera
			//	le m�me objet (gr�ce � CommandDispatcher).
			
			return new CommandState (command_name, dispatcher);
		}
		
		
		protected WidgetState				state = WidgetState.Enabled | WidgetState.ActiveNo;
	}
}
