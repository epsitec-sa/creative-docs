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
	public class Command : DependencyObject, System.IEquatable<Command>, Types.INamedType
	{
		protected Command()
		{
		}

		protected Command(string id)
			: this ()
		{
			if (string.IsNullOrEmpty (id))
			{
				throw new System.ArgumentNullException ("id");
			}
			
			this.InitializeCommandId (id);
		}

		protected Command(string id, params Shortcut[] shortcuts)
			: this (id)
		{
			this.Shortcuts.AddRange (shortcuts);
		}

		protected Command(Support.Druid druid)
			: this ()
		{
			string id = druid.ToResourceId ();
			
			this.InitializeCommandId (id);
		}

		protected Command(Types.Caption caption)
			: this ()
		{
			this.uniqueId = -1;
			this.caption  = caption;
			
			this.InitializeCommandId (caption.Druid.ToResourceId ());
		}

		public static Command CreateTemporary(Types.Caption caption)
		{
			Command command = new Command (caption);
			command.temporary = true;
			return command;
		}

		public Caption							Caption
		{
			get
			{
				return this.caption;
			}
		}
		
		public string							CommandId
		{
			get
			{
				return this.commandId;
			}
		}

		public CommandType						CommandType
		{
			get
			{
				return Command.GetCommandType (this.caption);
			}
		}

		public string							Name
		{
			get
			{
				return this.caption.Name;
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
				return Command.GetGroup (this.caption);
			}
		}

		public bool								Statefull
		{
			get
			{
				return Command.GetStatefull (this.caption);
			}
		}

		public bool								HasShortcuts
		{
			get
			{
				return Shortcut.HasShortcuts (this.caption);
			}
		}

		public ShortcutCollection				Shortcuts
		{
			get
			{
				return Shortcut.GetShortcuts (this.caption);
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

		public bool								HasMultiCommands
		{
			get
			{
				return MultiCommand.HasCommands (this.caption);
			}
		}

		public Collections.CommandCollection	MultiCommands
		{
			get
			{
				return MultiCommand.GetCommands (this.caption);
			}
		}
		
		public StructuredType					StructuredType
		{
			get
			{
				return StructuredCommand.GetStructuredType (this.caption);
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


		public void ManuallyDefineCommand(string description, string icon, string group, bool statefull)
		{
			if (this.locked)
			{
				throw new Exceptions.CommandLockedException (this.CommandId);
			}

			this.caption.Description = description;
			this.caption.Icon        = icon;

			Command.SetGroup (this.caption, group);
			Command.SetStatefull (this.caption, statefull);
		}

		public void DefineGroup(string value)
		{
			if (this.locked)
			{
				throw new Exceptions.CommandLockedException (this.CommandId);
			}
			if (this.temporary == false)
			{
				throw new System.InvalidOperationException (string.Format ("Command {0} may not be modified", this.CommandId));
			}

			Command.SetGroup (this.caption, value);
		}

		public void DefineStatefull(bool value)
		{
			if (this.locked)
			{
				throw new Exceptions.CommandLockedException (this.CommandId);
			}
			if (this.temporary == false)
			{
				throw new System.InvalidOperationException (string.Format ("Command {0} may not be modified", this.CommandId));
			}

			Command.SetStatefull (this.caption, value);
		}

		public void DefineCommandType(CommandType value)
		{
			if (this.locked)
			{
				throw new Exceptions.CommandLockedException (this.CommandId);
			}
			if (this.temporary == false)
			{
				throw new System.InvalidOperationException (string.Format ("Command {0} may not be modified", this.CommandId));
			}

			Command.SetCommandType (this.caption, value);
		}

		public void DefineShortcuts(Collections.ShortcutCollection value)
		{
			if (this.locked)
			{
				throw new Exceptions.CommandLockedException (this.CommandId);
			}
			if (this.temporary == false)
			{
				throw new System.InvalidOperationException (string.Format ("Command {0} may not be modified", this.CommandId));
			}

			Collections.ShortcutCollection shortcuts = Shortcut.GetShortcuts (this.caption);

			shortcuts.Clear ();

			if ((value != null) &&
				(value.Count > 0))
			{
				shortcuts.AddRange (value);
			}
		}

		public void DefineMultiCommands(Collections.CommandCollection value)
		{
			if (this.locked)
			{
				throw new Exceptions.CommandLockedException (this.CommandId);
			}
			if (this.temporary == false)
			{
				throw new System.InvalidOperationException (string.Format ("Command {0} may not be modified", this.CommandId));
			}

			Collections.CommandCollection commands = MultiCommand.GetCommands (this.caption);

			commands.Clear ();

			if ((value != null) &&
				(value.Count > 0))
			{
				commands.AddRange (value);
			}
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
			if (druid.IsEmpty)
			{
				return null;
			}
			
			return Command.Get (druid.ToResourceId ());
		}

		/// <summary>
		/// Gets the command matching the specified id. If the command does
		/// not exist yet, it is created on the fly.
		/// </summary>
		/// <param name="id">The command id.</param>
		/// <returns>The command.</returns>
		public static Command Get(string id)
		{
			if (string.IsNullOrEmpty (id))
			{
				return null;
			}
			
			lock (Command.commands)
			{
				Command command = Command.Find (id);

				if (command == null)
				{
					command = new Command (id);
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
			if (druid.IsEmpty)
			{
				return null;
			}

			return Command.Find (druid.ToResourceId ());
		}

		/// <summary>
		/// Finds the command matching the specified id.
		/// </summary>
		/// <param name="id">The command id.</param>
		/// <returns>The command, or <c>null</c> if none could be found.</returns>
		public static Command Find(string id)
		{
			if (string.IsNullOrEmpty (id))
			{
				return null;
			}
			
			Command state;
			
			if (Command.commands.TryGetValue (id, out state))
			{
				return state;
			}
			
			return null;
		}

		/// <summary>
		/// Finds and enumerates the commands matching the specified shortcut.
		/// </summary>
		/// <param name="shortcut">The command shortcut.</param>
		/// <returns>The command enumeration.</returns>
		public static IEnumerable<Command> Find(Shortcut shortcut)
		{
			foreach (Command command in Command.commands.Values)
			{
				if (command.Shortcuts.Contains (shortcut))
				{
					yield return command;
				}
			}
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

		Support.Druid ICaption.CaptionId
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
				return this.CommandId;
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
				return command.CommandId;
			}

			public object ConvertFromString(string value, IContextResolver context)
			{
				return Command.Get (value);
			}

			#endregion
		}

		#endregion
		
		#region Internal Methods

		private void InitializeCommandId(string commandId)
		{
			System.Diagnostics.Debug.Assert (string.IsNullOrEmpty (commandId) == false);
			System.Diagnostics.Debug.Assert (this.commandId == null);

			if (this.uniqueId == 0)
			{
				lock (Command.commands)
				{
					if (Command.commands.ContainsKey (commandId))
					{
						throw new System.ArgumentException (string.Format ("Command {0} already registered", commandId));
					}

					this.commandId = commandId;
					this.uniqueId = Command.nextUniqueId++;

					Command.commands[commandId] = this;
				}
			}
			
			Support.Druid druid;

			if (Support.Druid.TryParse (commandId, out druid))
			{
				this.InitializeDruid (druid);
			}
			else
			{
				this.InitializeDummyCaption ();
			}

			this.caption.Changed += this.HandleCaptionChanged;
			this.caption.AddEventHandler (Command.CommandTypeProperty, this.HandleCommandTypeChanged);

			this.InitializeCommandType ();
		}

		private void InitializeCommandType()
		{
			CommandType type = this.CommandType;
			
			switch (type)
			{
				case CommandType.Standard:
					this.DefineStateObjectType (Types.DependencyObjectType.FromSystemType (typeof (Command.SimpleState)));
					break;
				
				case CommandType.Multiple:
					this.DefineStateObjectType (Types.DependencyObjectType.FromSystemType (typeof (MultiCommand.MultiState)));
					break;
				
				case CommandType.Structured:
					this.DefineStateObjectType (Types.DependencyObjectType.FromSystemType (typeof (StructuredCommand.StructuredState)));
					break;
				
				default:
					throw new System.InvalidOperationException (string.Format ("Unsupported command type: {0}", type));
			}
		}

		private void InitializeDruid(Support.Druid druid)
		{
			System.Diagnostics.Debug.Assert (druid.IsValid);
			System.Diagnostics.Debug.Assert (this.captionId == Support.Druid.Empty);

			this.captionId = druid;

			//	This command is defined by a DRUID.

			if (this.caption == null)
			{
				Types.Caption caption = Support.Resources.DefaultManager.GetCaption (druid);

				System.Diagnostics.Debug.Assert (caption != null);
				System.Diagnostics.Debug.Assert (caption.Druid.ToResourceId () == this.commandId);

				this.caption = caption;
			}
			
			System.Diagnostics.Debug.Assert (this.caption.Druid == druid);
		}

		private void InitializeDummyCaption()
		{
			this.caption = new Caption ();
			this.caption.Name = this.commandId;
		}

		internal void Lockdown()
		{
			if (this.temporary)
			{
				throw new System.InvalidOperationException (string.Format ("Temporary command {0} may not be locked", this.CommandId));
			}
			
			this.locked = true;
		}
		
		#endregion

		#region Private Event Handlers

		private void HandleCaptionChanged(object sender)
		{
			CommandCache.Instance.InvalidateCommandCaption (this);
		}

		private void HandleCommandTypeChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			this.InitializeCommandType ();
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

		
		public static void SetStatefull(DependencyObject obj, bool value)
		{
			obj.SetValue (Command.StatefullProperty, value);
		}

		public static bool GetStatefull(DependencyObject obj)
		{
			return (bool) obj.GetValue (Command.StatefullProperty);
		}

		public static void SetGroup(DependencyObject obj, string value)
		{
			if (value == null)
			{
				obj.ClearValue (Command.GroupProperty);
			}
			else
			{
				obj.SetValue (Command.GroupProperty, value);
			}
		}

		public static string GetGroup(DependencyObject obj)
		{
			return (string) obj.GetValue (Command.GroupProperty);
		}

		public static void SetCommandType(DependencyObject obj, CommandType value)
		{
			obj.SetValue (Command.CommandTypeProperty, value);
		}

		public static CommandType GetCommandType(DependencyObject obj)
		{
			return (CommandType) obj.GetValue (Command.CommandTypeProperty);
		}


		private static object GetCaptionValue(DependencyObject obj)
		{
			Command that = (Command) obj;
			return that.Caption;
		}

		private static object GetCommandIdValue(DependencyObject obj)
		{
			Command that = (Command) obj;
			return that.CommandId;
		}



		public static readonly DependencyProperty CaptionProperty		= DependencyProperty.RegisterReadOnly ("Caption", typeof (Caption), typeof (Command), new DependencyPropertyMetadata (Command.GetCaptionValue));
		public static readonly DependencyProperty CommandIdProperty		= DependencyProperty.RegisterReadOnly ("CommandId", typeof (string), typeof (Command), new DependencyPropertyMetadata (Command.GetCommandIdValue));
		
		public static readonly DependencyProperty GroupProperty			= DependencyProperty.RegisterAttached ("Group", typeof (string), typeof (Command));
		public static readonly DependencyProperty StatefullProperty		= DependencyProperty.RegisterAttached ("Statefull", typeof (bool), typeof (Command), new DependencyPropertyMetadata (false));
		public static readonly DependencyProperty CommandTypeProperty	= DependencyProperty.RegisterAttached ("CommandType", typeof (CommandType), typeof (Command), new DependencyPropertyMetadata (CommandType.Standard));
		
		private static Dictionary<string, Command> commands = new Dictionary<string, Command> ();
		private static int nextUniqueId;

		private DependencyObjectType			stateObjectType;
		private int								uniqueId;
		private bool							locked;
		private bool							temporary;
		
		private string							commandId;
		private Support.Druid					captionId;
		private Types.Caption					caption;
	}
}
