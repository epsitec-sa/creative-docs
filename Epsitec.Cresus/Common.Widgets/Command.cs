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
		public Command()
		{
			this.DefineStateObjectType (Types.DependencyObjectType.FromSystemType (typeof (SimpleState)));
		}
		
		public Command(string id) : this ()
		{
			if (string.IsNullOrEmpty (id))
			{
				throw new System.ArgumentNullException ("id");
			}
			
			this.InitializeCommandId (id);
		}

		public Command(string id, params Shortcut[] shortcuts) : this (id)
		{
			this.Shortcuts.AddRange (shortcuts);
		}

		public Command(Support.Druid druid) : this ()
		{
			string id = druid.ToResourceId ();
			
			this.InitializeCommandId (id);
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
		
		public bool								HasShortcuts
		{
			get
			{
				return Shortcut.HasShortcuts (this.caption);
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
				throw new Exceptions.CommandLockedException (this.CommandId);
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
		/// Gets the command matching the specified id. If the command does
		/// not exist yet, it is created on the fly.
		/// </summary>
		/// <param name="id">The command id.</param>
		/// <returns>The command.</returns>
		public static Command Get(string id)
		{
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

			Support.Druid druid;

			if (Support.Druid.TryParse (commandId, out druid))
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
			System.Diagnostics.Debug.Assert (caption.Id == this.commandId);

			this.caption = caption;
		}

		private void InitializeDummyCaption()
		{
			this.caption = new Caption ();
			this.caption.Name = this.commandId;
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
		public static readonly DependencyProperty GroupProperty			= DependencyProperty.Register ("Group", typeof (string), typeof (Command), new DependencyPropertyMetadata (null, new PropertyInvalidatedCallback (Command.NotifyGroupChanged)));
		public static readonly DependencyProperty StatefullProperty		= DependencyProperty.Register ("Statefull", typeof (bool), typeof (Command), new DependencyPropertyMetadata (false));
		
		private static Dictionary<string, Command> commands = new Dictionary<string, Command> ();
		private static int nextUniqueId;

		private DependencyObjectType			stateObjectType;
		private int								uniqueId;
		private bool							locked;
		
		private string							commandId;
		private long							captionId = -1;
		private Types.Caption					caption;
	}
}
