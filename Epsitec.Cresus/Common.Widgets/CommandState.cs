namespace Epsitec.Common.Widgets
{
	using Regex = System.Text.RegularExpressions.Regex;
	
	/// <summary>
	/// La classe CommandState permet de représenter l'état d'une commande tout
	/// en maintenant la synchronisation avec les widgets associés.
	/// </summary>
	public class CommandState
	{
		public CommandState(string name) : this (name, Support.CommandDispatcher.Default)
		{
		}
		
		public CommandState(string name, Window window) : this (name, window.CommandDispatcher)
		{
		}
		
		public CommandState(string name, Support.CommandDispatcher dispatcher)
		{
			this.name       = name;
			this.dispatcher = dispatcher;
			this.regex      = Support.RegexFactory.FromSimpleJoker (this.name, Support.RegexFactory.Options.None);
			this.state      = WidgetState.Enabled | WidgetState.ActiveNo;
		}
		
		
		public string						Name
		{
			get { return this.name; }
		}
		
		public Support.CommandDispatcher	CommandDispatcher
		{
			get { this.dispatcher; }
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
		
		
		protected string					name;
		protected Support.CommandDispatcher	dispatcher;
		protected Regex						regex;
		protected WidgetState				state;
	}
}
