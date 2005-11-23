//	Copyright © 2003-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Text.RegularExpressions;

namespace Epsitec.Common.Widgets
{
	using ShortcutCollection = Epsitec.Common.Widgets.Collections.ShortcutCollection;
	
	/// <summary>
	/// La classe CommandState permet de représenter l'état d'une commande tout
	/// en maintenant la synchronisation avec les widgets associés.
	/// </summary>
	public class CommandState
	{
		public CommandState(string name) : this (name, CommandDispatcher.Default)
		{
		}
		
		public CommandState(string name, Window window) : this (name, window.CommandDispatcher)
		{
		}
		
		public CommandState(string name, CommandDispatcher dispatcher)
		{
			System.Diagnostics.Debug.Assert (name != null);
			System.Diagnostics.Debug.Assert (name.Length > 0);
			System.Diagnostics.Debug.Assert (dispatcher != null);
			
			System.Diagnostics.Debug.Assert (dispatcher[name] == null, "CommandState created twice.", string.Format ("The CommandState {0} for dispatcher {1} already exists.\nIt cannot be created more than once.", name, dispatcher.Name));
			
			this.name       = name;
			this.dispatcher = dispatcher;
			
			this.dispatcher.AddCommandState (this);
		}
		
		public CommandState(string name, CommandDispatcher dispatcher, Shortcut shortcut) : this (name, dispatcher)
		{
			this.Shortcuts.Add (shortcut);
		}
		
		public CommandState(string name, CommandDispatcher dispatcher, params Shortcut[] shortcuts) : this (name, dispatcher)
		{
			this.Shortcuts.AddRange (shortcuts);
		}
		
		
		public string						Name
		{
			get { return this.name; }
		}
		
		public CommandDispatcher			CommandDispatcher
		{
			get { return this.dispatcher; }
		}
		
		public virtual bool					Enabled
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
		
		
		protected Regex						Regex
		{
			get
			{
				if (this.regex == null)
				{
					this.regex = Support.RegexFactory.FromSimpleJoker (this.name, Support.RegexFactory.Options.None);
				}
				
				return this.regex;
			}
		}
		
		
		public Widget[] FindWidgets()
		{
			return Widget.FindAllCommandWidgets (this.Regex, this.CommandDispatcher);
		}
		
		
		public virtual void Synchronise()
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
		
		
		public static CommandState Find(string command_name, CommandDispatcher dispatcher)
		{
			if (dispatcher == null)
			{
				return null;
			}
			
			System.Diagnostics.Debug.Assert (command_name != null);
			System.Diagnostics.Debug.Assert (command_name.Length > 0);
			
			return dispatcher.CreateCommandState (command_name);
		}
		
		
		public override int GetHashCode()
		{
			return this.name.GetHashCode ();
		}
		
		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			
			CommandState other = obj as CommandState;
			
			if (other == null)
			{
				return false;
			}
			
			return this.name.Equals (other.name) && (this.dispatcher == other.dispatcher);
		}

		
		
		private WidgetState						widget_state = WidgetState.Enabled;
		private ActiveState						active_state = ActiveState.No;
		
		private Collections.ShortcutCollection	shortcuts;
		private string							name;
		private CommandDispatcher				dispatcher;
		private Regex							regex;
	}
}
