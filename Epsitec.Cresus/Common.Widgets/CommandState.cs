//	Copyright © 2003-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	using ShortcutCollection = Collections.ShortcutCollection;
	
	/// <summary>
	/// La classe CommandState permet de représenter l'état d'une commande tout
	/// en maintenant la synchronisation avec les widgets associés.
	/// </summary>
	public sealed class CommandState : CommandDispatcher.CommandState
	{
		static CommandState()
		{
			CommandDispatcher.DefineCommandStateCreationCallback (new CommandDispatcher.CreateCommandStateCallback (CommandState.DefaultCreate));
		}
		
		
		public CommandState(string name) : this (name, CommandDispatcher.Default)
		{
		}
		
		public CommandState(string name, Window window) : this (name, window.CommandDispatcher)
		{
		}
		
		public CommandState(string name, CommandDispatcher dispatcher) : base (name, dispatcher)
		{
		}
		
		public CommandState(string name, CommandDispatcher dispatcher, Shortcut shortcut) : base (name, dispatcher)
		{
			this.Shortcuts.Add (shortcut);
		}
		
		public CommandState(string name, CommandDispatcher dispatcher, params Shortcut[] shortcuts) : base (name, dispatcher)
		{
			this.Shortcuts.AddRange (shortcuts);
		}
		
		
		public override bool				Enabled
		{
			get
			{
				return (this.widget_state & WidgetState.Enabled) != 0;
			}
			set
			{
				if (this.Enabled != value)
				{
					if (value)
					{
						this.widget_state |= WidgetState.Enabled;
					}
					else
					{
						this.widget_state &= ~ WidgetState.Enabled;
					}
					
					foreach (Widget widget in this.FindWidgets ())
					{
						widget.SetEnabled (value);
						widget.Invalidate ();
					}
				}
			}
		}
		
		public ActiveState					ActiveState
		{
			get
			{
				return this.active_state;
			}
			set
			{
				if (this.active_state != value)
				{
					this.active_state = value;
					
					foreach (Widget widget in this.FindWidgets ())
					{
						widget.ActiveState = value;
						widget.Invalidate ();
					}
				}
			}
		}
		
		public ShortcutCollection			Shortcuts
		{
			get
			{
				if (this.shortcuts == null)
				{
					this.shortcuts = new ShortcutCollection ();
				}
				
				return this.shortcuts;
			}
		}
		
		public bool							HasShortcuts
		{
			get
			{
				if (this.shortcuts != null)
				{
					return this.shortcuts.Count > 0;
				}
				else
				{
					return false;
				}
			}
		}
		
		public Shortcut						PreferredShortcut
		{
			get
			{
				if (this.HasShortcuts)
				{
					return this.Shortcuts[0];
				}
				
				return null;
			}
		}
		
		
		public Widget[] FindWidgets()
		{
			return Widget.FindAllCommandWidgets (this.Regex, this.CommandDispatcher);
		}
		
		
		public override void Synchronise()
		{
			bool        enabled = this.Enabled;
			ActiveState active  = this.ActiveState;
			
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
		
		
		public static void Initialise()
		{
			//	En appelant cette méthode statique, on peut garantir que le constructeur
			//	statique de CommandState a bien été exécuté.
		}
		
		public static CommandState Find(string command_name, CommandDispatcher dispatcher)
		{
			if (dispatcher == null)
			{
				return null;
			}
			
			System.Diagnostics.Debug.Assert (command_name != null);
			System.Diagnostics.Debug.Assert (command_name.Length > 0);
			
			return dispatcher.CreateCommandState (command_name) as CommandState;
		}
		
		
		static CommandDispatcher.CommandState DefaultCreate(string command_name, CommandDispatcher dispatcher)
		{
			return new CommandState (command_name, dispatcher);
		}
		
		
		private WidgetState						widget_state = WidgetState.Enabled;
		private ActiveState						active_state = ActiveState.No;
		
		private Collections.ShortcutCollection	shortcuts;
	}
}
