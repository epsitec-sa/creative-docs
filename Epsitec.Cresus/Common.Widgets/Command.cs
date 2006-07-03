//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

using Epsitec.Common.Widgets.Collections;
using Epsitec.Common.Types;

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Widgets.Command),
	/**/										 Converter = typeof (Epsitec.Common.Widgets.Command.SerializationConverter))]

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe <c>Command</c> permet de représenter l'état d'une commande tout
	/// en maintenant la synchronisation avec l'état des widgets associés.
	/// </summary>
	[SerializationConverter (typeof (Command.SerializationConverter))]
	public class Command : DependencyObject, System.IEquatable<Command>, Types.INamedType
	{
		public Command()
		{
			this.DefineStateObjectType (Types.DependencyObjectType.FromSystemType (typeof (SimpleState)));
		}
		
		public Command(string name) : this ()
		{
			if (string.IsNullOrEmpty (name))
			{
				throw new System.ArgumentNullException ("name");
			}
			
			this.InitializeName (name);
		}

		public Command(string name, params Shortcut[] shortcuts) : this (name)
		{
			this.Shortcuts.AddRange (shortcuts);
		}

		public Command(Support.Druid druid) : this ()
		{
			string name = druid.ToResourceId ();
			
			this.InitializeName (name);
		}

		public Caption							Caption
		{
			get
			{
				return this.caption;
			}
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
				return this.caption.Icon;
			}
		}
		
		public string							Description
		{
			get
			{
				return this.caption.Description;
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
				return (bool) this.GetValue (Command.StatefullProperty);
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

			this.caption.Description = description;
			this.caption.Icon        = icon;
			
			this.SetValue (Command.StatefullProperty, statefull);
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

		#region SerializationConverter Class

		public class SerializationConverter : ISerializationConverter
		{
			#region ISerializationConverter Members

			public string ConvertToString(object value, IContextResolver context)
			{
				Command command = value as Command;
				return command.Name;
			}

			public object ConvertFromString(string value, IContextResolver context)
			{
				return Command.Get (value);
			}

			#endregion
		}

		#endregion
		
		#region Internal Methods

		private void InitializeName(string name)
		{
			System.Diagnostics.Debug.Assert (string.IsNullOrEmpty (name) == false);
			System.Diagnostics.Debug.Assert (this.name == null);
			
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

			Support.Druid druid;

			if (Support.Druid.TryParse (name, out druid))
			{
				this.InitializeDruid (druid);
			}
			else
			{
				this.InitializeDummyCaption ();
			}

			this.caption.AddEventHandler (Caption.IconProperty, this.HandleIconChanged);
			this.caption.AddEventHandler (Caption.DescriptionProperty, this.HandleDescriptionChanged);
		}

		private void InitializeDruid(Support.Druid druid)
		{
			System.Diagnostics.Debug.Assert (druid.IsValid);
			System.Diagnostics.Debug.Assert (this.captionId == -1);

			this.captionId = druid.ToLong ();
			
			//	This command is defined by a DRUID.

			Types.Caption caption = Support.Resources.DefaultManager.GetCaption (druid);

			System.Diagnostics.Debug.Assert (caption != null);
			System.Diagnostics.Debug.Assert (caption.Id == this.name);

			this.caption = caption;
		}

		private void InitializeDummyCaption()
		{
			this.caption = new Caption ();
			this.caption.Name = this.name;
		}

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

		private void HandleIconChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
		}

		private void HandleDescriptionChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
		}


		private static void NotifyGroupChanged(DependencyObject o, object old_value, object new_value)
		{
			Command that = o as Command;
			that.OnGroupChanged (new DependencyPropertyChangedEventArgs (Command.GroupProperty, old_value, new_value));
		}
		
		#endregion

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

		private static object GetCaptionValue(DependencyObject obj)
		{
			Command that = (Command) obj;
			return that.Caption;
		}

		private static object GetNameValue(DependencyObject obj)
		{
			Command that = (Command) obj;
			return that.Name;
		}

		private static void SetNameValue(DependencyObject obj, object value)
		{
			Command that = (Command) obj;
			string name = (string) value;

			that.InitializeName (name);
		}

		public static readonly DependencyProperty CaptionProperty		= DependencyProperty.RegisterReadOnly ("Caption", typeof (Caption), typeof (Command), new DependencyPropertyMetadata (Command.GetCaptionValue));
		public static readonly DependencyProperty NameProperty			= DependencyProperty.Register ("Name", typeof (string), typeof (Command), new DependencyPropertyMetadata (Command.GetNameValue, Command.SetNameValue));
		public static readonly DependencyProperty GroupProperty			= DependencyProperty.Register ("Group", typeof (string), typeof (Command), new DependencyPropertyMetadata (null, new PropertyInvalidatedCallback (Command.NotifyGroupChanged)));
		public static readonly DependencyProperty ShortcutsProperty		= DependencyProperty.RegisterReadOnly ("Shortcuts", typeof (Collections.ShortcutCollection), typeof (Command), new DependencyPropertyMetadata (Command.GetShortcutsValue).MakeReadOnlySerializable ());
		public static readonly DependencyProperty StatefullProperty		= DependencyProperty.Register ("Statefull", typeof (bool), typeof (Command), new DependencyPropertyMetadata (false));
		
		private static Dictionary<string, Command> commands = new Dictionary<string, Command> ();
		private static int nextUniqueId;

		private DependencyObjectType			stateObjectType;
		private int								uniqueId;
		private bool							locked;
		
		private string							name;
		private long							captionId = -1;
		private Types.Caption					caption;
		private Collections.ShortcutCollection	shortcuts;
	}
}
