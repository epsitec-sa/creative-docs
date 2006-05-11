//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

using Epsitec.Common.Widgets.Collections;
using Epsitec.Common.Types;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe <c>CommandState</c> permet de représenter l'état d'une commande tout
	/// en maintenant la synchronisation avec l'état des widgets associés.
	/// </summary>
	public sealed class CommandState : DependencyObject, System.IEquatable<CommandState>
	{
		public CommandState(string name)
		{
			System.Diagnostics.Debug.Assert (string.IsNullOrEmpty (name) == false);

			lock (CommandState.commands)
			{
				if (CommandState.commands.ContainsKey (name))
				{
					throw new System.ArgumentException (string.Format ("CommandState {0} already registered", name));
				}

				this.name = name;
				this.uniqueId = CommandState.nextUniqueId++;

				CommandState.commands[name] = this;
			}
		}
		
		public CommandState(string name, params Shortcut[] shortcuts) : this (name)
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

		public int								UniqueId
		{
			get
			{
				return this.uniqueId;
			}
		}

		public string							Group
		{
			get
			{
				return (string) this.GetValue (CommandState.GroupProperty);
			}
			set
			{
				this.SetValue (CommandState.GroupProperty, value);
			}
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


		public static CommandState Get(string commandName)
		{
			lock (CommandState.commands)
			{
				CommandState commandState = CommandState.Find (commandName);

				if (commandState == null)
				{
					commandState = new CommandState (commandName);
				}
				
				return commandState;
			}
		}
		
		public static CommandState Find(string commandName)
		{
			CommandState state;
			
			if (CommandState.commands.TryGetValue (commandName, out state))
			{
				return state;
			}
			
			return null;
		}

		public static CommandState Find(Shortcut shortcut)
		{
			foreach (CommandState command in CommandState.commands.Values)
			{
				if (command.Shortcuts.Match (shortcut))
				{
					return command;
				}
			}
			
			return null;
		}
		
		
		public override int GetHashCode()
		{
			return this.uniqueId;
		}
		
		public override bool Equals(object obj)
		{
			return this.Equals (obj as CommandState);
		}

		#region IEquatable<CommandState> Members

		public bool Equals(CommandState other)
		{
			if (other == null)
			{
				return false;
			}
			else
			{
				return this.uniqueId == other.uniqueId;
			}
		}

		#endregion

		private void Synchronize()
		{
			CommandCache.Default.UpdateWidgets (this);
		}

		private void OnGroupChanged(DependencyPropertyChangedEventArgs e)
		{
			CommandCache.Default.Invalidate (this);
		}
		
		private void OnIconNameChanged(DependencyPropertyChangedEventArgs e)
		{
		}

		private void OnShortCaptionChanged(DependencyPropertyChangedEventArgs e)
		{
		}

		private void OnLongCaptionChanged(DependencyPropertyChangedEventArgs e)
		{
		}


		private static void NotifyGroupChanged(DependencyObject o, object old_value, object new_value)
		{
			CommandState that = o as CommandState;
			that.OnGroupChanged (new DependencyPropertyChangedEventArgs (CommandState.GroupProperty, old_value, new_value));
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

		public static readonly DependencyProperty GroupProperty			= DependencyProperty.Register ("Group", typeof (string), typeof (CommandState), new DependencyPropertyMetadata (null, new PropertyInvalidatedCallback (CommandState.NotifyGroupChanged)));
		public static readonly DependencyProperty IconNameProperty		= DependencyProperty.Register ("IconName", typeof (string), typeof (CommandState), new DependencyPropertyMetadata (null, new PropertyInvalidatedCallback (CommandState.NotifyIconNameChanged)));
		public static readonly DependencyProperty ShortCaptionProperty	= DependencyProperty.Register ("ShortCaption", typeof (string), typeof (CommandState), new DependencyPropertyMetadata (null, new PropertyInvalidatedCallback (CommandState.NotifyShortCaptionChanged)));
		public static readonly DependencyProperty LongCaptionProperty	= DependencyProperty.Register ("LongCaption", typeof (string), typeof (CommandState), new DependencyPropertyMetadata (null, new PropertyInvalidatedCallback (CommandState.NotifyLongCaptionChanged)));
		
		public static readonly DependencyProperty	AdvancedStateProperty = DependencyProperty.RegisterAttached ("AdvancedState", typeof (string), typeof (CommandState), new DependencyPropertyMetadata (null));

		private static Dictionary<string, CommandState> commands = new Dictionary<string, CommandState> ();
		private static int nextUniqueId;

		private int								uniqueId;
		private ActiveState						activeState = ActiveState.No;
		private bool							enable = true;
		private bool							statefull;
		
		private Collections.ShortcutCollection	shortcuts;
		private string							name;
	}
}
