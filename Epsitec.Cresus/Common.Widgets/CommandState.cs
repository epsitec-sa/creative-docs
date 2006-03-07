//	Copyright © 2003-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Text.RegularExpressions;

using Epsitec.Common.Widgets.Collections;
using Epsitec.Common.Types;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe CommandState permet de représenter l'état d'une commande tout
	/// en maintenant la synchronisation avec les widgets associés.
	/// </summary>
	public class CommandState : Types.DependencyObject
	{
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
		
		
		public string							Name
		{
			get
			{
				return this.name;
			}
		}
		
		public CommandDispatcher				CommandDispatcher
		{
			get
			{
				return this.dispatcher;
			}
		}
		
		public string							IconName
		{
			get
			{
				return (string) this.GetValue (CommandState.IconNameProperty);
			}
			set
			{
				this.SetValue (CommandState.IconNameProperty, value);
			}
		}
		
		public string							ShortCaption
		{
			get
			{
				return (string) this.GetValue (CommandState.ShortCaptionProperty);
			}
			set
			{
				this.SetValue (CommandState.ShortCaptionProperty, value);
			}
		}
		
		public string							LongCaption
		{
			get
			{
				return (string) this.GetValue (CommandState.LongCaptionProperty);
			}
			set
			{
				this.SetValue (CommandState.LongCaptionProperty, value);
			}
		}
		
		
		public virtual bool						Enable
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
		
		public bool								Statefull
		{
			get
			{
				return this.statefull;
			}
			set
			{
				this.statefull = value;
			}
		}
		
		public ActiveState						ActiveState
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
					this.Synchronize ();
				}
			}
		}
		
		public ShortcutCollection				Shortcuts
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
		
		public Shortcut							PreferredShortcut
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
		
		
		public bool								HasShortcuts
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
		
		
		protected Regex							Regex
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
		
		
		protected virtual void Synchronize()
		{
			CommandCache.Default.UpdateWidgets (this);
		}
		
		
		public static CommandState Find(string command_name, CommandDispatcher dispatcher)
		{
			if (dispatcher == null)
			{
				return null;
			}
			
			return dispatcher.GetCommandState (command_name);
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

		
		protected virtual void OnIconNameChanged(Types.DependencyPropertyChangedEventArgs e)
		{
		}
		
		protected virtual void OnShortCaptionChanged(Types.DependencyPropertyChangedEventArgs e)
		{
		}
		
		protected virtual void OnLongCaptionChanged(Types.DependencyPropertyChangedEventArgs e)
		{
		}
		
		
		private static void NotifyIconNameChanged(DependencyObject o, object old_value, object new_value)
		{
			CommandState that = o as CommandState;
			that.OnIconNameChanged (new DependencyPropertyChangedEventArgs (CommandState.IconNameProperty, old_value, new_value));
		}
		
		private static void NotifyShortCaptionChanged(DependencyObject o, object old_value, object new_value)
		{
			CommandState that = o as CommandState;
			that.OnShortCaptionChanged (new DependencyPropertyChangedEventArgs (CommandState.ShortCaptionProperty, old_value, new_value));
		}
		
		private static void NotifyLongCaptionChanged(DependencyObject o, object old_value, object new_value)
		{
			CommandState that = o as CommandState;
			that.OnLongCaptionChanged (new DependencyPropertyChangedEventArgs (CommandState.LongCaptionProperty, old_value, new_value));
		}
		
		
		public static readonly DependencyProperty			IconNameProperty = DependencyProperty.Register ("IconName", typeof (string), typeof (CommandState), new DependencyPropertyMetadata (null, new PropertyInvalidatedCallback (CommandState.NotifyIconNameChanged)));
		public static readonly DependencyProperty			ShortCaptionProperty = DependencyProperty.Register ("ShortCaption", typeof (string), typeof (CommandState), new DependencyPropertyMetadata (null, new PropertyInvalidatedCallback (CommandState.NotifyShortCaptionChanged)));
		public static readonly DependencyProperty			LongCaptionProperty	= DependencyProperty.Register ("LongCaption", typeof (string), typeof (CommandState), new DependencyPropertyMetadata (null, new PropertyInvalidatedCallback (CommandState.NotifyLongCaptionChanged)));
		
		private ActiveState						active_state = ActiveState.No;
		private bool							enable       = true;
		private bool							statefull;
		
		private Collections.ShortcutCollection	shortcuts;
		private string							name;
		private CommandDispatcher				dispatcher;
		private Regex							regex;
	}
}
