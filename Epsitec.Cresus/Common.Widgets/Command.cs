//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

using Epsitec.Common.Widgets.Collections;
using Epsitec.Common.Types;

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Widgets.Command))]

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe <c>Command</c> permet de représenter l'état d'une commande tout
	/// en maintenant la synchronisation avec l'état des widgets associés.
	/// </summary>
	public class Command : DependencyObject, System.IEquatable<Command>, Types.INamedType
	{
		public Command()
		{
			this.DefineStateObjectType (Types.DependencyObjectType.FromSystemType (typeof (SimpleState)));
		}
		
		public Command(string name) : this ()
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

			if (Support.Druid.TryParse (name, out this.druid))
			{
				//	This command is defined by a DRUID.

				this.captionId = druid.ToLong ();
				
				Types.Caption caption = Support.Resources.DefaultManager.GetCaption (druid);

				System.Diagnostics.Debug.Assert (caption != null);
				System.Diagnostics.Debug.Assert (caption.Id == this.name);

				this.caption = caption;
				this.RefreshCommandBasedOnCaption (this.caption);
			}
		}

		public Command(string name, params Shortcut[] shortcuts) : this (name)
		{
			this.Shortcuts.AddRange (shortcuts);
		}

		public Command(Support.Druid druid) : this (druid.ToResourceId ())
		{
		}
		
		
		public string							Name
		{
			get
			{
				return this.name;
			}
		}
		
		public string							Icon
		{
			get
			{
				return (string) this.GetValue (Command.IconNameProperty);
			}
		}
		
		public string							Description
		{
			get
			{
				return (string) this.GetValue (Command.DescriptionProperty);
			}
		}

		public int								SerialId
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
		}

		public DependencyObjectType				CommandStateObjectType
		{
			get
			{
				return this.stateObjectType;
			}
		}
		
		public bool								Statefull
		{
			get
			{
				return this.statefull;
			}
		}
		
		public ShortcutCollection				Shortcuts
		{
			get
			{
				if (this.shortcuts == null)
				{
					this.shortcuts = new Collections.ShortcutCollection ();
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
					return shortcuts.Count > 0;
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


		public void ManuallyDefineCommand(string description, string icon, bool statefull)
		{
			if (this.locked)
			{
				throw new Exceptions.CommandLockedException (this.Name);
			}

			this.SetValue (Command.DescriptionProperty, description);
			this.SetValue (Command.IconNameProperty, icon);
			this.statefull = statefull;
		}
		
		public CommandState CreateDefaultState(CommandContext context)
		{
			if (this.stateObjectType != null)
			{
				CommandState state = this.stateObjectType.CreateEmptyObject () as CommandState;
				
				this.InitializeDefaultState (state, context);
				
				return state;
			}
			
			return null;
		}

		/// <summary>
		/// Gets the command matching the specified DRUID. If the command does
		/// not exist yet, it is created on the fly.
		/// </summary>
		/// <param name="druid">The command DRUID.</param>
		/// <returns>The command.</returns>
		public static Command Get(Support.Druid druid)
		{
			return Command.Get (druid.ToResourceId ());
		}

		/// <summary>
		/// Gets the command matching the specified name. If the command does
		/// not exist yet, it is created on the fly.
		/// </summary>
		/// <param name="name">The command name.</param>
		/// <returns>The command.</returns>
		public static Command Get(string name)
		{
			lock (Command.commands)
			{
				Command command = Command.Find (name);

				if (command == null)
				{
					command = new Command (name);
				}
				
				return command;
			}
		}

		/// <summary>
		/// Finds the command matching the specified DRUID.
		/// </summary>
		/// <param name="druid">The command DRUID.</param>
		/// <returns>The command, or <c>null</c> if none could be found.</returns>
		public static Command Find(Support.Druid druid)
		{
			return Command.Find (druid.ToResourceId ());
		}

		/// <summary>
		/// Finds the command matching the specified name.
		/// </summary>
		/// <param name="name">The command name.</param>
		/// <returns>The command, or <c>null</c> if none could be found.</returns>
		public static Command Find(string name)
		{
			if (string.IsNullOrEmpty (name))
			{
				return null;
			}
			
			Command state;
			
			if (Command.commands.TryGetValue (name, out state))
			{
				return state;
			}
			
			return null;
		}

		/// <summary>
		/// Finds the command matching the specified shortcut.
		/// </summary>
		/// <param name="shortcut">The command shortcut.</param>
		/// <returns>The command, or <c>null</c> if none could be found.</returns>
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

		string INamedType.DefaultController
		{
			get
			{
				return "String";
			}
		}

		string INamedType.DefaultControllerParameter
		{
			get
			{
				return null;
			}
		}

		#endregion
		
		#region ISystemType Members

		System.Type ISystemType.SystemType
		{
			get
			{
				return this.GetType ();
			}
		}

		#endregion

		#region ICaption Members

		long ICaption.CaptionId
		{
			get
			{
				return this.captionId;
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

		private void OnDescriptionChanged(DependencyPropertyChangedEventArgs e)
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
		
		private static void NotifyDescriptionChanged(DependencyObject o, object old_value, object new_value)
		{
			Command that = o as Command;
			that.OnDescriptionChanged (new DependencyPropertyChangedEventArgs (Command.DescriptionProperty, old_value, new_value));
		}

		#endregion

		protected virtual void RefreshCommandBasedOnCaption(Types.Caption caption)
		{
			string[] labels = Types.Collection.ToArray (caption.SortedLabels);

			if (!string.IsNullOrEmpty (caption.Description))
			{
				this.SetValue (Command.DescriptionProperty, caption.Description);
			}
			else if (labels.Length > 1)
			{
				this.SetValue (Command.DescriptionProperty, labels[labels.Length-1]);
			}
			else if (labels.Length == 1)
			{
				this.SetValue (Command.DescriptionProperty, labels[0]);
			}
		}

		protected virtual void InitializeDefaultState(CommandState state, CommandContext context)
		{
			state.DefineCommand (this);
			state.DefineCommandContext (context);
			
			TypeRosetta.SetTypeObject (state, this);
		}

		protected void DefineStateObjectType(DependencyObjectType type)
		{
			this.stateObjectType = type;
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


		private static object GetShortcutsValue(DependencyObject obj)
		{
			Command that = (Command) obj;
			return that.Shortcuts;
		}
		
		public static readonly DependencyProperty GroupProperty			= DependencyProperty.Register ("Group", typeof (string), typeof (Command), new DependencyPropertyMetadata (null, new PropertyInvalidatedCallback (Command.NotifyGroupChanged)));
		public static readonly DependencyProperty IconNameProperty		= DependencyProperty.Register ("Icon", typeof (string), typeof (Command), new DependencyPropertyMetadata (null, new PropertyInvalidatedCallback (Command.NotifyIconNameChanged)));
		public static readonly DependencyProperty DescriptionProperty	= DependencyProperty.Register ("Description", typeof (string), typeof (Command), new DependencyPropertyMetadata (null, new PropertyInvalidatedCallback (Command.NotifyDescriptionChanged)));
		public static readonly DependencyProperty ShortcutsProperty		= DependencyProperty.RegisterReadOnly ("Shortcuts", typeof (Collections.ShortcutCollection), typeof (Command), new DependencyPropertyMetadata (Command.GetShortcutsValue).MakeReadOnlySerializable ());
		
		private static Dictionary<string, Command> commands = new Dictionary<string, Command> ();
		private static int nextUniqueId;

		private DependencyObjectType			stateObjectType;
		private int								uniqueId;
		private bool							statefull;
		private bool							locked;
		
		private string							name;
		private Support.Druid					druid;
		private Types.Caption					caption;
		private long							captionId = -1;
		private Collections.ShortcutCollection	shortcuts;
	}
}
