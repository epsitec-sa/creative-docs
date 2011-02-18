//	Copyright © 2003-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Collections;

using System.Collections.Generic;
using System.Linq;

[assembly: DependencyClass (typeof (Command), Converter = typeof (Command.SerializationConverter))]

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>Command</c> class represents a command which can be executed
	/// by the application. The associated <see cref="CommandState"/> defines
	/// if the command is enabled; there can be several states for one command.
	/// </summary>
	public sealed class Command : DependencyObject, System.IEquatable<Command>, INamedType
	{
		private Command()
		{
		}

		private Command(string id)
			: this (id, Epsitec.Common.Support.Resources.DefaultManager)
		{
		}

		private Command(string id, ICaptionResolver manager)
			: this ()
		{
			if (string.IsNullOrEmpty (id))
			{
				throw new System.ArgumentNullException ("id");
			}

			this.InitializeCommandId (id, manager);
		}

		private Command(string id, params Shortcut[] shortcuts)
			: this (id)
		{
			this.Shortcuts.AddRange (shortcuts);
		}

		private Command(Druid druid)
			: this (druid, Epsitec.Common.Support.Resources.DefaultManager)
		{
		}

		private Command(Druid druid, ICaptionResolver manager)
			: this ()
		{
			string id = druid.ToResourceId ();
			
			this.InitializeCommandId (id, manager);
		}

		private Command(Caption caption, ICaptionResolver manager)
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

		public bool								IsTemporary
		{
			get
			{
				return this.temporary;
			}
		}

		public bool								IsAdminLevelRequired
		{
			get
			{
				return this.CommandParameters["Level"] == "Admin";
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

		public CommandState CreateDefaultState(CommandContext context)
		{
			CommandState state = new SimpleState ();

			state.DefineCommand (this);
			state.DefineCommandContext (context);
			
			return state;
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

			lock (Command.commands)
			{
				if (Command.commands.TryGetValue (id, out state))
				{
					return state;
				}
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
		/// Finds all commands matching the specified shortcut. The order in which
		/// the items are returned is not defined.
		/// </summary>
		/// <param name="shortcut">The command shortcut.</param>
		/// <returns>The collection of matching commands.</returns>
		public static IEnumerable<Command> FindAll(Shortcut shortcut)
		{
			return Command.FindAll (command => command.Shortcuts.Contains (shortcut));
		}

		/// <summary>
		/// Finds all commands which match the specified predicate. The order in
		/// wich the items are returned is not defined.
		/// </summary>
		/// <param name="predicate">The predicate.</param>
		/// <returns>The collection of matching commands.</returns>
		public static IEnumerable<Command> FindAll(System.Predicate<Command> predicate)
		{
			lock (Command.commands)
			{
				return Command.commands.Values.Where (command => predicate (command)).ToList ();
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
				manager = Epsitec.Common.Support.Resources.DefaultManager;
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

		#endregion

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

		public static void SetDefaultParameter(DependencyObject obj, string value)
		{
			obj.SetValue (Command.DefaultParameterProperty, value);
		}

		public static string GetDefaultParameter(DependencyObject obj)
		{
			return (string) obj.GetValue (Command.DefaultParameterProperty);
		}

		public static bool GetHideWhenDisabled(Visual obj)
		{
			return (bool) obj.GetValue (Command.HideWhenDisabledProperty);
		}

		public static void SetHideWhenDisabled(Visual obj, bool hideWhenDisabled)
		{
			if (hideWhenDisabled)
			{
				obj.SetValue (Command.HideWhenDisabledProperty, true);
			}
			else
			{
				obj.ClearValue (Command.HideWhenDisabledProperty);
			}
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



		public static readonly DependencyProperty CaptionProperty	= DependencyProperty<Command>.RegisterReadOnly (x => x.Caption, new DependencyPropertyMetadata (Command.GetCaptionValue));
		public static readonly DependencyProperty CommandIdProperty	= DependencyProperty<Command>.RegisterReadOnly (x => x.CommandId, new DependencyPropertyMetadata (Command.GetCommandIdValue));
		
		public static readonly DependencyProperty GroupProperty			   = DependencyProperty<Command>.RegisterAttached ("Group", typeof (string));
		public static readonly DependencyProperty StatefullProperty		   = DependencyProperty<Command>.RegisterAttached ("Statefull", typeof (bool), new DependencyPropertyMetadata (false));
		public static readonly DependencyProperty HideWhenDisabledProperty = DependencyProperty<Command>.RegisterAttached ("HideWhenDisabled", typeof (bool), new DependencyPropertyMetadata (false));
		public static readonly DependencyProperty DefaultParameterProperty = DependencyProperty<Command>.RegisterAttached ("DefaultParameter", typeof (string));
		
		private static readonly Dictionary<string, Command> commands = new Dictionary<string, Command> ();
		private static int nextUniqueId;

		private int								uniqueId;
		private bool							frozen;
		private bool							temporary;
		
		private string							commandId;
		private Druid							captionId;
		private Caption							caption;
	}
}
