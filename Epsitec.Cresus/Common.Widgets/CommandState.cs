//	Copyright © 2003-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	using ShortcutCollection = Collections.ShortcutCollection;
	
	/// <summary>
	/// La classe CommandState permet de représenter l'état d'une commande tout
	/// en maintenant la synchronisation avec les widgets associés.
	/// </summary>
	public sealed class CommandState : Support.CommandDispatcher.CommandState
	{
		static CommandState()
		{
			Support.CommandDispatcher.DefineCommandStateCreationCallback (new Support.CommandDispatcher.CreateCommandStateCallback (CommandState.DefaultCreate));
		}
		
		
		public CommandState(string name) : this (name, Support.CommandDispatcher.Default)
		{
		}
		
		public CommandState(string name, Window window) : this (name, window.CommandDispatcher)
		{
		}
		
		public CommandState(string name, Support.CommandDispatcher dispatcher) : base (name, dispatcher)
		{
		}
		
		public CommandState(string name, Support.CommandDispatcher dispatcher, Shortcut shortcut) : base (name, dispatcher)
		{
			this.Shortcuts.Add (shortcut);
		}
		
		public CommandState(string name, Support.CommandDispatcher dispatcher, params Shortcut[] shortcuts) : base (name, dispatcher)
		{
			this.Shortcuts.AddRange (shortcuts);
		}
		
		
		public override bool				Enabled
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
		
		
		public static void Initialise()
		{
			//	En appelant cette méthode statique, on peut garantir que le constructeur
			//	statique de CommandState a bien été exécuté.
		}
		
		public static CommandState Find(string command_name, Support.CommandDispatcher dispatcher)
		{
			if (dispatcher == null)
			{
				return null;
			}
			
			System.Diagnostics.Debug.Assert (command_name != null);
			System.Diagnostics.Debug.Assert (command_name.Length > 0);
			
			return dispatcher.CreateCommandState (command_name) as CommandState;
		}
		
		
		static Support.CommandDispatcher.CommandState DefaultCreate(string command_name, Support.CommandDispatcher dispatcher)
		{
			return new CommandState (command_name, dispatcher);
		}
		
		
		private WidgetState						state		= WidgetState.Enabled | WidgetState.ActiveNo;
		private Collections.ShortcutCollection	shortcuts;
	}
}
