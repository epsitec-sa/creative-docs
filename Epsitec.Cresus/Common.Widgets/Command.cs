//	Copyright � 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Collections;

using System.Collections.Generic;

[assembly: DependencyClass (typeof (Command), Converter = typeof (Command.SerializationConverter))]

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe <c>Command</c> permet de repr�senter l'�tat d'une commande tout
	/// en maintenant la synchronisation avec l'�tat des widgets associ�s.
	/// </summary>
	public class Command : DependencyObject, System.IEquatable<Command>, INamedType
	{
		protected Command()
		{
		}

		protected Command(string id)
			: this (id, Resources.DefaultManager)
		{
		}
		
		protected Command(string id, ICaptionResolver manager)
			: this ()
		{
			if (string.IsNullOrEmpty (id))
			{
				throw new System.ArgumentNullException ("id");
			}

			this.InitializeCommandId (id, manager);
		}

		protected Command(string id, params Shortcut[] shortcuts)
			: this (id)
		{
			this.Shortcuts.AddRange (shortcuts);
		}

		protected Command(Druid druid)
			: this (druid, Resources.DefaultManager)
		{
		}

		protected Command(Druid druid, ICaptionResolver manager)
			: this ()
		{
			string id = druid.ToResourceId ();
			
			this.InitializeCommandId (id, manager);
		}

		protected Command(Caption caption, ICaptionResolver manager)
			: this ()
		{
			this.uniqueId = -1;
			this.caption  = caption;
			
			this.InitializeCommandId (caption.Id.ToResourceId (), manager);
		}

		public static Command CreateTemporary(Caption caption, ICaptionResolver manager)
		{
			Command command = new Command (caption, manager);
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

		public CommandParameters				CommandParameters
		{
			get
			{
				return new CommandParameters (Command.GetDefaultParameter (this.caption));
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
				string value = this.caption.Description;

				if (string.IsNullOrEmpty (value))
				{
					value = this.caption.DefaultLabel;
				}

				return value;
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
				return this.frozen;
			}
		}

		public bool								IsReadWrite
		{
			get
			{
				return !this.frozen;
			}
		}


		public void ManuallyDefineCommand(string description, string icon, string group, bool statefull)
		{
			this.EnsureWriteable ();

			this.caption.Description = description;
			this.caption.Icon        = icon;

			Command.SetGroup (this.caption, group);
			Command.SetStatefull (this.caption, statefull);
		}

		public void DefineGroup(string value)
		{
			this.EnsureWriteable ();
			this.EnsureTemporary ();

			Command.SetGroup (this.caption, value);
		}

		public void DefineStatefull(bool value)
		{
			this.EnsureWriteable ();
			this.EnsureTemporary ();

			Command.SetStatefull (this.caption, value);
		}

		public void DefineCommandType(CommandType value)
		{
			this.EnsureWriteable ();
			this.EnsureTemporary ();

			Command.SetCommandType (this.caption, value);
		}

		public void DefineDefaultParameter(string value)
		{
			this.EnsureWriteable ();
			this.EnsureTemporary ();

			Command.SetDefaultParameter (this.caption, value);
		}

		public void DefineShortcuts(Collections.ShortcutCollection value)
		{
			this.EnsureWriteable ();
			this.EnsureTemporary ();
			
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
			this.EnsureWriteable ();
			this.EnsureTemporary ();
			
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

		public string GetDescriptionWithShortcut()
		{
			return Shortcut.AppendShortcutText (this.Description, this.Shortcuts);
		}

		/// <summary>
		/// Gets the command matching the specified DRUID. If the command does
		/// not exist yet, it is created on the fly.
		/// </summary>
		/// <param name="druid">The command DRUID.</param>
		/// <returns>The command.</returns>
		public static Command Get(Druid druid)
		{
			return Command.Get (druid, null);
		}

		public static Command Get(Druid druid, ICaptionResolver manager)
		{
			if (druid.IsEmpty)
			{
				return null;
			}
			
			return Command.Get (druid.ToResourceId (), manager);
		}

		/// <summary>
		/// Gets the command matching the specified id. If the command does
		/// not exist yet, it is created on the fly.
		/// </summary>
		/// <param name="id">The command id.</param>
		/// <returns>The command.</returns>
		public static Command Get(string id)
		{
			return Command.Get (id, null);
		}

		public static Command Get(string id, ICaptionResolver manager)
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
					command = new Command (id, manager);
				}
				
				return command;
			}
		}

		/// <summary>
		/// Finds the command matching the specified DRUID.
		/// </summary>
		/// <param name="druid">The command DRUID.</param>
		/// <returns>The command, or <c>null</c> if none could be found.</returns>
		public static Command Find(Druid druid)
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
		/// Determines whether the specified command is defined.
		/// </summary>
		/// <param name="id">The command id.</param>
		/// <returns>
		/// 	<c>true</c> if the specified command is defined; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsDefined(string id)
		{
			return Command.Find (id) != null;
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

		string INamedType.DefaultControllerParameters
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

		Druid ICaption.CaptionId
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

		private void InitializeCommandId(string commandId, ICaptionResolver manager)
		{
			System.Diagnostics.Debug.Assert (string.IsNullOrEmpty (commandId) == false);
			System.Diagnostics.Debug.Assert (this.commandId == null);

			if (manager == null)
			{
				manager = Resources.DefaultManager;
			}

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
			
			Druid druid;

			if (Druid.TryParse (commandId, out druid))
			{
				this.InitializeId (druid, manager);
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
					this.DefineStateObjectType (DependencyObjectType.FromSystemType (typeof (Command.SimpleState)));
					break;
				
				case CommandType.Multiple:
					this.DefineStateObjectType (DependencyObjectType.FromSystemType (typeof (MultiCommand.MultiState)));
					break;
				
				case CommandType.Structured:
					this.DefineStateObjectType (DependencyObjectType.FromSystemType (typeof (StructuredCommand.StructuredState)));
					break;
				
				default:
					throw new System.InvalidOperationException (string.Format ("Unsupported command type: {0}", type));
			}
		}

		private void InitializeId(Druid druid, ICaptionResolver manager)
		{
			System.Diagnostics.Debug.Assert (manager != null);
			System.Diagnostics.Debug.Assert (druid.IsValid);
			System.Diagnostics.Debug.Assert (this.captionId == Druid.Empty);

			this.captionId = druid;

			//	This command is defined by a DRUID.

			if (this.caption == null)
			{
				Caption caption = manager.GetCaption (druid);

				System.Diagnostics.Debug.Assert (caption != null);
				System.Diagnostics.Debug.Assert (caption.Id.ToResourceId () == this.commandId);

				this.caption = caption;
			}
			
			System.Diagnostics.Debug.Assert (this.caption.Id == druid);
		}

		private void InitializeDummyCaption()
		{
			this.caption = new Caption ();
			this.caption.Name = this.commandId;
		}

		internal void Freeze()
		{
			if (this.temporary)
			{
				throw new System.InvalidOperationException (string.Format ("Temporary command {0} may not be frozen", this.CommandId));
			}
			
			this.frozen = true;
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

		private void EnsureWriteable()
		{
			if (this.frozen)
			{
				throw new Exceptions.CommandLockedException (this.CommandId);
			}
		}

		private void EnsureTemporary()
		{
			if (this.temporary == false)
			{
				throw new System.InvalidOperationException (string.Format ("Command {0} may not be modified", this.CommandId));
			}
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

		public static void SetDefaultParameter(DependencyObject obj, string value)
		{
			obj.SetValue (Command.DefaultParameterProperty, value);
		}

		public static string GetDefaultParameter(DependencyObject obj)
		{
			return (string) obj.GetValue (Command.DefaultParameterProperty);
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
		
		public static readonly DependencyProperty DefaultParameterProperty = DependencyProperty.RegisterAttached ("DefaultParameter", typeof (string), typeof (Command));
		
		private static Dictionary<string, Command> commands = new Dictionary<string, Command> ();
		private static int nextUniqueId;

		private DependencyObjectType			stateObjectType;
		private int								uniqueId;
		private bool							frozen;
		private bool							temporary;
		
		private string							commandId;
		private Druid							captionId;
		private Caption							caption;
	}
}
