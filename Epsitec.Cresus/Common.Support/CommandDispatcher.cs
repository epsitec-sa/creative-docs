//	Copyright � 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	using Match        = System.Text.RegularExpressions.Match;
	using Regex        = System.Text.RegularExpressions.Regex;
	using RegexOptions = System.Text.RegularExpressions.RegexOptions;
	
	/// <summary>
	/// La classe CommandDispatcher permet de g�rer la distribution des
	/// commandes de l'interface graphique vers les routines de traitement.
	/// </summary>
	public class CommandDispatcher : System.IDisposable
	{
		static CommandDispatcher()
		{
			//	Initialise les champs statiques :
			
			CommandDispatcher.global_list = new System.Collections.ArrayList ();
			CommandDispatcher.local_list  = new System.Collections.ArrayList ();
			
			//	Capture le nom et les arguments d'une commande complexe, en filtrant les
			//	caract�res et v�rifiant ainsi la validit� de la syntaxe. Voici l'inter-
			//	pr�tation de la regex :
			//
			//	- un <name> est constitu� de caract�res alphanum�riques;
			//	- suit une parenth�se ouvrante, avec �vtl. des espaces;
			//	- suit z�ro � n arguments <arg> s�par�s par une virgule;
			//	- chaque <arg> est soit une cha�ne "", soit une cha�ne '',
			//	  soit une valeur num�rique, soit un nom (avec des '.' pour
			//	  s�parer les divers termes).
			//
			//	La capture retourne dans l'ordre <name>, puis la liste des <arg> trouv�s.
			//	Il peut y avoir z�ro � n arguments s�par�s par des virgules, le tout entre
			//	parenth�ses.
			
			string regex_1 = @"\A(?<name>([a-zA-Z](\w|(\.\w))*))" +
				//                       <---- nom valide ---->
				/**/       @"\s*\(\s*((((?<arg>(" +
				/**/                          @"(\""[^\""]{0,}\"")|" +
				//                              <-- guillemets -->
				/**/                          @"(\'[^\']{0,}\')|" +
				//                              <-- apostr. -->
				/**/                          @"((\-|\+)?((\d{1,12}(\.\d{0,12})?0*)|(\d{0,12}\.(\d{0,12})?0*)))|" +
				//                              <----------- valeur d�cimale avec signe en option ------------>
				/**/                          @"([a-zA-Z](\w|(\.\w))*)))" +
				//                              <---- nom valide ---->
				/**/                         @"((\s*\,\s*)|(\s*\)\s*\z)))*)|(\)\s*))\z";
			
			RegexOptions options = RegexOptions.Compiled | RegexOptions.ExplicitCapture;
			
			CommandDispatcher.command_arg_regex  = new Regex (regex_1, options);
			CommandDispatcher.default_dispatcher = new CommandDispatcher ("default");
		}
		
		public CommandDispatcher() : this ("anonymous", true)
		{
		}
		
		public CommandDispatcher(string name) : this (name, false)
		{
		}
		
		public CommandDispatcher(string name, bool private_dispatcher)
		{
			if (private_dispatcher)
			{
				this.dispatcher_name = string.Format ("{0}_{1}", name, CommandDispatcher.generation++);
				CommandDispatcher.local_list.Add (this);
			}
			else
			{
				this.dispatcher_name = name;
				CommandDispatcher.global_list.Add (this);
			}
			
			this.validation_rule = new ValidationRule (this);
		}
		
		
		public CommandDispatcher.CommandState	this[string command_name]
		{
			get
			{
				foreach (CommandState state in this.command_states)
				{
					if (state.Name == command_name)
					{
						return state;
					}
				}
				
				return null;
			}
		}
		
		
		public string							Name
		{
			get
			{
				return this.dispatcher_name;
			}
		}
		
		public bool								Aborted
		{
			get
			{
				return this.aborted;
			}
			set
			{
				this.aborted = value;
			}
		}
		
		public bool								HasPendingMultipleCommands
		{
			get
			{
				return this.pending_commands.Peek () != null;
			}
		}
		
		public string							TopPendingMulitpleCommands
		{
			get
			{
				return this.pending_commands.Peek () as string;
			}
		}
		
		public OpletQueue						OpletQueue
		{
			get
			{
				return this.oplet_queue;
			}
			set
			{
				if (this.oplet_queue != value)
				{
					this.oplet_queue = value;
					this.OnOpletQueueBindingChanged ();
				}
			}
		}
		
		public ValidationRule					ValidationRule
		{
			get
			{
				return this.validation_rule;
			}
		}
		
		public string[]							CommandNames
		{
			get
			{
				string[] names = new string[this.event_handlers.Keys.Count];
				this.event_handlers.Keys.CopyTo (names, 0);
				System.Array.Sort (names);
				return names;
			}
		}
		
		
		
		public void Dispatch(string command, object source)
		{
			this.aborted = false;
			
			//	L'appelant peut sp�cifier une ou plusieurs commandes. Dans ce dernier cas, les
			//	commandes sont cha�n�es au moyen du symbole "->" et elles sont ex�cut�es dans
			//	l'ordre. Une commande peut prendre connaissance des commandes encore en attente
			//	d'ex�cution au moyen de XxxPendingMultipleCommands.
			//
			//	De cette fa�on, une commande interactive peut annuler les commandes en attente
			//	et les ex�cuter soi-m�me lorsque l'utilisateur valide le dialogue, par exemple.
			
			if (command.IndexOf ("->") >= 0)
			{
				string[] commands = System.Utilities.Split (command, "->");
				
				System.Diagnostics.Debug.Assert (commands.Length > 0);
				
				while (commands.Length > 1)
				{
					command = string.Join ("->", commands, 1, commands.Length-1);
					
					try
					{
						this.pending_commands.Push (command);
						this.DispatchSingleCommand (commands[0], source);
					}
					finally
					{
						command = this.pending_commands.Pop () as string;
					}
					
					//	Si la commande a �t� annul�e, on s'arr�te imm�diatement.
					
					if (command == null)
					{
						return;
					}
					
					//	Il reste des commandes inexploit�es. On va donc passer � la suite.
					
					commands = System.Utilities.Split (command, "->");
				}
			}
			
			try
			{
				this.pending_commands.Push (null);
				this.DispatchSingleCommand (command, source);
			}
			finally
			{
				this.pending_commands.Pop ();
			}
		}
		
		public void DispatchCancelTopPendingMultipleCommands()
		{
			this.pending_commands.Pop ();
			this.pending_commands.Push (null);
		}
		
		
		public void SyncCommandStates()
		{
			//	Passe en revue tous les CommandStates connus et resynchronise ceux-ci. Afin d'�viter
			//	des surprises en cas de modifications en cours de synchronisation, on copie la liste
			//	dans une table temporaire :
			
			CommandState[] states = new CommandState[this.command_states.Count];
			this.command_states.CopyTo (states);
			
			for (int i = 0; i < states.Length; i++)
			{
				states[i].Synchronise ();
			}
		}
		
		public void SyncValidationRule()
		{
			if (this.validation_rule.State == ValidationState.Dirty)
			{
				this.validation_rule.Validate ();
			}
		}
		
		
		public CommandDispatcher.CommandState CreateCommandState(string command_name)
		{
			//	Retourne un object CommandState pour le nom sp�cifi�; si l'objet n'existe pas encore,
			//	il sera cr�� dynamiquement.
			
			CommandState state = this[command_name];
			
			if (state != null)
			{
				return state;
			}
			
			//	La cr�ation se fait dans Widgets.CommandState et pour �viter des probl�mes de
			//	d�pendances circulaires, on utilise un callback :
			
			if (CommandDispatcher.create_command_state_callback == null)
			{
				throw new System.InvalidOperationException ("Missing CommandState creation callback.");
			}
			
			return CommandDispatcher.create_command_state_callback (command_name, this);
		}
		
		
		
		public void RegisterController(object controller)
		{
			if (controller != null)
			{
				System.Type type = controller.GetType ();
				System.Reflection.MemberInfo[] members = type.GetMembers (System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
				
				for (int i = 0; i < members.Length; i++)
				{
					if ((members[i].IsDefined (CommandDispatcher.command_attr_type, true)) &&
						(members[i].MemberType == System.Reflection.MemberTypes.Method))
					{
						System.Reflection.MethodInfo info = members[i] as System.Reflection.MethodInfo;
						this.RegisterMethod (controller, info);
					}
				}
			}
		}
		
		public void Register(string command, CommandEventHandler handler)
		{
			System.Diagnostics.Debug.Assert (command.IndexOf ("*") < 0, "Found '*' in command name.", "The command '" + command + "' may not contain a '*' in its name.\nPlease fix the registration source code.");
			System.Diagnostics.Debug.Assert (command.IndexOf (".") < 0, "Found '.' in command name.", "The command '" + command + "' may not contain a '.' in its name.\nPlease fix the registration source code.");
			
			EventSlot slot;
			
			if (this.event_handlers.Contains (command))
			{
				slot = this.event_handlers[command] as EventSlot;
			}
			else
			{
				slot = new EventSlot (command);
				this.event_handlers[command] = slot;
			}
			
			slot.Register (handler);
		}
		
		public void Unregister(string command, CommandEventHandler handler)
		{
			if (this.event_handlers.Contains (command))
			{
				EventSlot slot = this.event_handlers[command] as EventSlot;
				
				slot.Unregister (handler);
				
				if (slot.Count == 0)
				{
					this.event_handlers.Remove (command);
				}
			}
		}
		
		public void RegisterExtraDispatcher(ICommandDispatcher extra)
		{
			this.extra_dispatchers.Add (extra);
		}
		
		public void UnregisterExtraDispatcher(ICommandDispatcher extra)
		{
			this.extra_dispatchers.Remove (extra);
		}
		
		
		#region Internal use only
		public static void SyncAllValidationRules()
		{
			foreach (CommandDispatcher dispatcher in CommandDispatcher.global_list)
			{
				dispatcher.SyncValidationRule ();
			}
			foreach (CommandDispatcher dispatcher in CommandDispatcher.local_list)
			{
				dispatcher.SyncValidationRule ();
			}
		}
		
		public static void DefineCommandStateCreationCallback(CreateCommandStateCallback value)
		{
			CommandDispatcher.create_command_state_callback = value;
		}
		#endregion
		
		public static string ExtractCommandName(string command)
		{
			int pos = command.IndexOf ('(');
			if (pos < 0)
			{
				return command.Trim ();
			}
			
			return command.Substring (0, pos).Trim ();
		}
		
		public static bool IsSimpleCommand(string command)
		{
			int pos = command.IndexOf ('(');
			return (pos < 0) ? true : false;
		}
		
		public static string[] ExtractCommandArgs(string command)
		{
			int pos = command.IndexOf ('(');
			
			if (pos < 0)
			{
				return new string[0];
			}
			
			Match match = CommandDispatcher.command_arg_regex.Match (command);
			
			if ((match.Success) &&
				(match.Groups.Count == 3))
			{
				int      n    = match.Groups[2].Captures.Count;
				string[] args = new string[n];
				
				for (int i = 0; i < n; i++)
				{
					args[i] = match.Groups[2].Captures[i].Value;
				}
				
				return args;
			}
			
			throw new System.FormatException (string.Format ("Command '{0}' is not well formed.", command));
		}
		
		public static string[] ExtractAndParseCommandArgs(string command, object source)
		{
			string[] args = CommandDispatcher.ExtractCommandArgs (command);
			
			for (int i = 0; i < args.Length; i++)
			{
				string arg   = args[i];
				Match  match = RegexFactory.DecimalNum.Match (arg);
				
				if (match.Success)
				{
					//	C'est une valeur num�rique proprement format�e. On la garde telle
					//	quelle.
				}
				else if ((arg[0] == '\'') || (arg[0] == '\"'))
				{
					//	C'est un texte entre guillemets. On supprime le premier et le dernier
					//	caract�re.
					
					System.Diagnostics.Debug.Assert (arg.Length > 1);
					System.Diagnostics.Debug.Assert (arg[arg.Length-1] == arg[0]);
					
					arg = arg.Substring (1, arg.Length - 2);
				}
				else
				{
					//	Ce n'est ni une valeur num�rique, ni un texte; c'est probablement un
					//	symbole que l'on va passer tel quel plus loin, sauf si c'est une
					//	expression commen�ant par 'this.'.
					
					if (arg.StartsWith ("this."))
					{
						//	L'argument d�crit une propri�t� de la source. On va tenter d'aller
						//	lire la source.
						
						System.Reflection.PropertyInfo info;
						System.Reflection.BindingFlags flags = System.Reflection.BindingFlags.GetProperty
							/**/							 | System.Reflection.BindingFlags.Instance
							/**/							 | System.Reflection.BindingFlags.Public;
						
						string prop_name = arg.Substring (5);
						System.Type type = source.GetType ();
						
						info = type.GetProperty (prop_name, flags);
						
						if (info == null)
						{
							throw new System.FieldAccessException (string.Format ("Command {0} tries to access property {1} which cannot be found in class {2}.", command, prop_name, type.Name));
						}
						
						object data = info.GetValue (source, null);
						
						arg = data.ToString ();
					}
				}
				
				args[i] = arg;
			}
			
			return args;
		}
		
		
		#region IDisposable Members
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}
		#endregion
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (CommandDispatcher.global_list.Contains (this))
				{
					CommandDispatcher.global_list.Remove (this);
				}
				if (CommandDispatcher.local_list.Contains (this))
				{
					CommandDispatcher.local_list.Remove (this);
				}
			}
		}
		
		
		#region EventRelay class
		protected class EventRelay
		{
			public EventRelay(object controller, System.Reflection.MethodInfo method_info)
			{
				//	Cette classe r�alise un relais entre le delegate CommandEventHandler et les
				//	diverses impl�mentations possibles au niveau des gestionnaires de commandes.
				//	Ainsi, les m�thodes :
				//
				//		void Method()
				//		void Method(CommandDispatcher)
				//		void Method(CommandDispatcher, CommandEventArgs)
				//
				//	sont toutes appelables via CommandEventHandler.
				
				this.controller  = controller;
				this.method_info = method_info;
			}
			
			
			public void InvokeWithoutArgument(CommandDispatcher sender, CommandEventArgs e)
			{
				this.method_info.Invoke (this.controller, null);
			}
			
			public void InvokeWithCommandDispatcher(CommandDispatcher sender, CommandEventArgs e)
			{
				object[] p = new object[1];
				p[0] = sender;
				this.method_info.Invoke (this.controller, p);
			}
			
			public void InvokeWithCommandDispatcherAndEventArgs(CommandDispatcher sender, CommandEventArgs e)
			{
				object[] p = new object[2];
				p[0] = sender;
				p[1] = e;
				this.method_info.Invoke (this.controller, p);
			}
			
			
			protected object						controller;
			protected System.Reflection.MethodInfo	method_info;
		}
		#endregion
		
		#region EventSlot class
		protected class EventSlot : ICommandDispatcher
		{
			public EventSlot(string name)
			{
				this.name = name;
			}
			
			
			public void Register(CommandEventHandler handler)
			{
				this.command += handler;
				this.count++;
			}
			
			public void Unregister(CommandEventHandler handler)
			{
				this.command -= handler;
				this.count--;
			}
			
			
			public bool DispatchCommand(CommandDispatcher sender, CommandEventArgs e)
			{
				if (this.command != null)
				{
					this.command (sender, e);
					return true;
				}
				
				return false;
			}
			
			
			public int							Count
			{
				get { return this.count; }
			}
			
			public string						Name
			{
				get { return this.name; }
			}
			
			
			protected string					name;
			protected event CommandEventHandler	command;
			protected int						count;
		}
		#endregion
		
		#region CommandState class
		public abstract class CommandState
		{
			public CommandState(string name, CommandDispatcher dispatcher)
			{
				System.Diagnostics.Debug.Assert (name != null);
				System.Diagnostics.Debug.Assert (name.Length > 0);
				System.Diagnostics.Debug.Assert (dispatcher != null);
				
				System.Diagnostics.Debug.Assert (dispatcher[name] == null, "CommandState created twice.", string.Format ("The CommandState {0} for dispatcher {1} already exists.\nIt cannot be created more than once.", name, dispatcher.Name));
				
				this.name       = name;
				this.dispatcher = dispatcher;
				
				this.dispatcher.command_states.Add (this);
			}
			
			
			public string						Name
			{
				get { return this.name; }
			}
			
			public Support.CommandDispatcher	CommandDispatcher
			{
				get { return this.dispatcher; }
			}
			
			public Regex						Regex
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
			public abstract bool				Enabled { get; set; }
			
			
			public abstract void Synchronise();
			
			public override int GetHashCode()
			{
				return this.name.GetHashCode ();
			}
			
			public override bool Equals(object obj)
			{
				CommandState other = obj as CommandState;
				
				if (other == null)
				{
					return false;
				}
				
				return this.name.Equals (other.name) && (this.dispatcher == other.dispatcher);
			}

			
			private string						name;
			private Support.CommandDispatcher	dispatcher;
			private Regex						regex;
		}
		#endregion
		
		#region CreateCommandStateCallback Delegate
		public delegate CommandDispatcher.CommandState CreateCommandStateCallback(string name, CommandDispatcher dispatcher);
		#endregion
		
		internal void NotifyValidationRuleBecameDirty()
		{
			this.OnValidationRuleBecameDirty ();
		}
		
		
		protected void RegisterMethod(object controller, System.Reflection.MethodInfo info)
		{
			//	Ne parcourt que les attributs au niveau d'impl�mentation actuel (pas les classes d�riv�es,
			//	ni les classes parent). Le parcours des parent est assur� par l'appelant.
			
			object[] attributes = info.GetCustomAttributes (CommandDispatcher.command_attr_type, false);
			
			foreach (CommandAttribute attribute in attributes)
			{
				this.RegisterMethod (controller, info, attribute);
			}
		}
		
		protected void RegisterMethod(object controller, System.Reflection.MethodInfo method_info, CommandAttribute attribute)
		{
			System.Diagnostics.Debug.WriteLine ("Command '" + attribute.CommandName + "' implemented by method " + method_info.Name + " in class " + method_info.DeclaringType.Name + ", prototype: " + method_info.ToString ());
			
			System.Diagnostics.Debug.Assert (attribute.CommandName.IndexOf ("*") < 0, "Found '*' in command name.", "The method handling command '" + attribute.CommandName + "' may not contain specify '*' in the command name.\nPlease fix the source code for " + method_info.Name + " in class " + method_info.DeclaringType.Name + ".");
			System.Diagnostics.Debug.Assert (attribute.CommandName.IndexOf (".") < 0, "Found '.' in command name.", "The method handling command '" + attribute.CommandName + "' may not contain specify '.' in the command name.\nPlease fix the source code for " + method_info.Name + " in class " + method_info.DeclaringType.Name + ".");
			
			System.Reflection.ParameterInfo[] param_info = method_info.GetParameters ();
			
			CommandEventHandler handler = null;
			EventRelay          relay   = new EventRelay (controller, method_info);
			
			switch (param_info.Length)
			{
				case 0:
					//	La m�thode n'a aucun argument :
					
					handler = new CommandEventHandler (relay.InvokeWithoutArgument);
					break;
				
				case 1:
					//	La m�thode a un unique argument. Ce n'est acceptable que si cet argument est
					//	de type CommandDispatcher, soit :
					//
					//		void Method(CommandDispatcher)
					
					if (param_info[0].ParameterType == typeof (CommandDispatcher))
					{
						handler = new CommandEventHandler (relay.InvokeWithCommandDispatcher);
					}
					break;
				
				case 2:
					//	La m�thode a deux arguments. Ce n'est acceptable que si le premier est de type
					//	CommandDispatcher et le second de type CommandEventArgs, soit :
					//
					//		void Method(CommandDispatcher, CommandEventArgs)
					
					if ((param_info[0].ParameterType == typeof (CommandDispatcher)) &&
						(param_info[1].ParameterType == typeof (CommandEventArgs)))
					{
						handler = new CommandEventHandler (relay.InvokeWithCommandDispatcherAndEventArgs);
					}
					break;
			}
			
			if (handler == null)
			{
				throw new System.FormatException (string.Format ("{0}.{1} uses invalid signature: {2}.", controller.GetType ().Name, method_info.Name, method_info.ToString ()));
			}
			
			this.Register (attribute.CommandName, handler);
			
		}
		
		
		protected void DispatchSingleCommand(string command, object source)
		{
			command = command.Trim ();
			
			//	Transmet la commande � ceux qui sont int�ress�s
			
			string   command_name     = CommandDispatcher.ExtractCommandName (command);
			string[] command_elements = command_name.Split ('/');
			int      command_length   = command_elements.Length;
			string[] command_args     = CommandDispatcher.ExtractAndParseCommandArgs (command, source);
			
			System.Diagnostics.Debug.Assert (command_length == 1);
			System.Diagnostics.Debug.Assert (command_name.IndexOf ("*") < 0, "Found '*' in command name.", "The command '" + command + "' may not contain a '*' in its name.\nPlease fix the command name definition source code.");
			System.Diagnostics.Debug.Assert (command_name.IndexOf (".") < 0, "Found '.' in command name.", "The command '" + command + "' may not contain a '.' in its name.\nPlease fix the command name definition source code.");
			
			CommandEventArgs e = new CommandEventArgs (source, command_name, command_args);
			
			EventSlot slot = this.event_handlers[command_name] as EventSlot;
			int    handled = 0;
			
			if (slot != null)
			{
				System.Diagnostics.Debug.WriteLine ("Command '" + command_name + "' fired.");
				
				if (slot.DispatchCommand (this, e))
				{
					handled++;
				}
			}
			
			foreach (ICommandDispatcher extra in this.extra_dispatchers)
			{
				if (extra.DispatchCommand (this, e))
				{
					handled++;
				}
			}
			
			if (handled == 0)
			{
				System.Diagnostics.Debug.WriteLine ("Command '" + command_name + "' not handled.");
			}
		}
		
		
		protected void OnValidationRuleBecameDirty()
		{
			if (this.ValidationRuleBecameDirty != null)
			{
				this.ValidationRuleBecameDirty (this);
			}
		}
		
		protected void OnOpletQueueBindingChanged()
		{
			if (this.OpletQueueBindingChanged != null)
			{
				this.OpletQueueBindingChanged (this);
			}
		}
		
		
		public event EventHandler				ValidationRuleBecameDirty;
		public event EventHandler				OpletQueueBindingChanged;
		
		public static CommandDispatcher			Default
		{
			get { return CommandDispatcher.default_dispatcher; }
		}
		
		
		protected System.Collections.Hashtable	event_handlers    = new System.Collections.Hashtable ();
		protected System.Collections.ArrayList	command_states    = new System.Collections.ArrayList ();
		protected System.Collections.ArrayList	validation_states = new System.Collections.ArrayList ();
		protected System.Collections.Stack		pending_commands  = new System.Collections.Stack ();
		protected System.Collections.ArrayList	extra_dispatchers = new System.Collections.ArrayList ();
		protected string						dispatcher_name;
		protected ValidationRule				validation_rule;
		protected bool							aborted;
		protected OpletQueue					oplet_queue;
		
		static CreateCommandStateCallback		create_command_state_callback;
		static Regex							command_arg_regex;
		static System.Type						command_attr_type  = typeof (CommandAttribute);
		static CommandDispatcher				default_dispatcher;
		static int								generation = 1;
		static System.Collections.ArrayList		global_list;
		static System.Collections.ArrayList		local_list;
	}
}
