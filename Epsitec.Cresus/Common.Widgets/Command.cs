//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

using Epsitec.Common.Widgets.Collections;
using Epsitec.Common.Types;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe <c>Command</c> permet de représenter l'état d'une commande tout
	/// en maintenant la synchronisation avec l'état des widgets associés.
	/// </summary>
	public class Command : DependencyObject, System.IEquatable<Command>, Types.INamedType
	{
		public Command(string name)
		{
			System.Diagnostics.Debug.Assert (string.IsNullOrEmpty (name) == false);

			lock (Command.commands)
			{
				if (Command.commands.ContainsKey (name))
				{
					throw new System.ArgumentException (string.Format ("Command {0} already registered", name));
				}

				this.name = name;
				this.uniqueId = Command.nextUniqueId++;

				Command.commands[name] = this;
			}

			this.stateObjectType = Types.DependencyObjectType.FromSystemType (typeof (SimpleState));
		}
		
		public Command(string name, params Shortcut[] shortcuts) : this (name)
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
				return (string) this.GetValue (Command.IconNameProperty);
			}
			set
			{
				if (this.locked)
				{
					throw new Exceptions.CommandLockedException (this.Name);
				}
				
				this.SetValue (Command.IconNameProperty, value);
			}
		}
		
		public string							ShortCaption
		{
			get
			{
				return (string) this.GetValue (Command.ShortCaptionProperty);
			}
			set
			{
				if (this.locked)
				{
					throw new Exceptions.CommandLockedException (this.Name);
				}

				this.SetValue (Command.ShortCaptionProperty, value);
			}
		}
		
		public string							LongCaption
		{
			get
			{
				return (string) this.GetValue (Command.LongCaptionProperty);
			}
			set
			{
				if (this.locked)
				{
					throw new Exceptions.CommandLockedException (this.Name);
				}

				this.SetValue (Command.LongCaptionProperty, value);
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
				return (string) this.GetValue (Command.GroupProperty);
			}
			set
			{
				if (this.locked)
				{
					throw new Exceptions.CommandLockedException (this.Name);
				}

				this.SetValue (Command.GroupProperty, value);
			}
		}

		public DependencyObjectType				StateObjectType
		{
			get
			{
				return this.stateObjectType;
			}
			set
			{
				if (this.locked)
				{
					throw new Exceptions.CommandLockedException (this.Name);
				}

				if (this.stateObjectType != value)
				{
					this.stateObjectType = value;
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
				if (this.locked)
				{
					throw new Exceptions.CommandLockedException (this.Name);
				}

				if (this.statefull != value)
				{
					this.statefull = value;
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

		public bool								IsReadOnly
		{
			get
			{
				return this.locked;
			}
		}

		public bool								IsReadWrite
		{
			get
			{
				return !this.locked;
			}
		}

		
		public CommandState CreateDefaultState()
		{
			if (this.stateObjectType != null)
			{
				CommandState state = this.stateObjectType.CreateEmptyObject () as CommandState;
				
				this.InitializeDefaultState (state);
				
				return state;
			}
			
			return null;
		}

		
		public static Command Get(string commandName)
		{
			lock (Command.commands)
			{
				Command command = Command.Find (commandName);

				if (command == null)
				{
					command = new Command (commandName);
				}
				
				return command;
			}
		}
		
		public static Command Find(string commandName)
		{
			if (string.IsNullOrEmpty (commandName))
			{
				return null;
			}
			
			Command state;
			
			if (Command.commands.TryGetValue (commandName, out state))
			{
				return state;
			}
			
			return null;
		}

		public static Command Find(Shortcut shortcut)
		{
			foreach (Command command in Command.commands.Values)
			{
				if (command.Shortcuts.Contains (shortcut))
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
			return this.Equals (obj as Command);
		}

		public static bool operator==(Command a, Command b)
		{
			if (object.ReferenceEquals (a, b))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public static bool operator!=(Command a, Command b)
		{
			if (object.ReferenceEquals (a, b))
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		#region IEquatable<Command> Members

		public bool Equals(Command other)
		{
			//	Two commands can only be equal if they are represented by the same
			//	instance in memory :
			
			return object.ReferenceEquals (this, other);
		}

		#endregion

		#region INamedType Members

		System.Type INamedType.SystemType
		{
			get
			{
				return this.GetType ();
			}
		}

		#endregion

		#region INameCaption Members

		string INameCaption.Caption
		{
			get
			{
				return this.ShortCaption;
			}
		}

		string INameCaption.Description
		{
			get
			{
				return this.LongCaption;
			}
		}

		#endregion

		#region IName Members

		string IName.Name
		{
			get
			{
				return this.Name;
			}
		}

		#endregion

		#region Private SimpleState Class

		private class SimpleState : CommandState
		{
			public SimpleState()
			{
			}
		}

		#endregion

		#region Internal Methods

		internal void Lockdown()
		{
			this.locked = true;
		}
		
		#endregion

		#region Private Event Handlers

		private void OnGroupChanged(DependencyPropertyChangedEventArgs e)
		{
			CommandCache.Default.InvalidateCommand (this);
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
			Command that = o as Command;
			that.OnGroupChanged (new DependencyPropertyChangedEventArgs (Command.GroupProperty, old_value, new_value));
		}
		
		private static void NotifyIconNameChanged(DependencyObject o, object old_value, object new_value)
		{
			Command that = o as Command;
			that.OnIconNameChanged (new DependencyPropertyChangedEventArgs (Command.IconNameProperty, old_value, new_value));
		}
		
		private static void NotifyShortCaptionChanged(DependencyObject o, object old_value, object new_value)
		{
			Command that = o as Command;
			that.OnShortCaptionChanged (new DependencyPropertyChangedEventArgs (Command.ShortCaptionProperty, old_value, new_value));
		}
		
		private static void NotifyLongCaptionChanged(DependencyObject o, object old_value, object new_value)
		{
			Command that = o as Command;
			that.OnLongCaptionChanged (new DependencyPropertyChangedEventArgs (Command.LongCaptionProperty, old_value, new_value));
		}

		#endregion

		protected virtual void InitializeDefaultState(CommandState state)
		{
			state.DefineCommand (this);
		}

		public static string[] SplitGroupNames(string groups)
		{
			if (string.IsNullOrEmpty (groups))
			{
				return new string[0];
			}
			else
			{
				return groups.Split ('|');
			}
		}

		public static string JoinGroupNames(params string[] groups)
		{
			return string.Join ("|", groups);
		}

		public static CommandState CreateSimpleState(Command command)
		{
			CommandState state = new SimpleState ();
			state.DefineCommand (command);
			return state;
		}

		public static readonly DependencyProperty GroupProperty			= DependencyProperty.Register ("Group", typeof (string), typeof (Command), new DependencyPropertyMetadata (null, new PropertyInvalidatedCallback (Command.NotifyGroupChanged)));
		public static readonly DependencyProperty IconNameProperty		= DependencyProperty.Register ("IconName", typeof (string), typeof (Command), new DependencyPropertyMetadata (null, new PropertyInvalidatedCallback (Command.NotifyIconNameChanged)));
		public static readonly DependencyProperty ShortCaptionProperty	= DependencyProperty.Register ("ShortCaption", typeof (string), typeof (Command), new DependencyPropertyMetadata (null, new PropertyInvalidatedCallback (Command.NotifyShortCaptionChanged)));
		public static readonly DependencyProperty LongCaptionProperty	= DependencyProperty.Register ("LongCaption", typeof (string), typeof (Command), new DependencyPropertyMetadata (null, new PropertyInvalidatedCallback (Command.NotifyLongCaptionChanged)));
		
		private static Dictionary<string, Command> commands = new Dictionary<string, Command> ();
		private static int nextUniqueId;

		private DependencyObjectType			stateObjectType;
		private int								uniqueId;
		private bool							statefull;
		private bool							locked;
		
		private Collections.ShortcutCollection	shortcuts;
		private string							name;
	}
}
