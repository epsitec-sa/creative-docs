//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier

using System.Text.RegularExpressions;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// La classe CommandDispatcher permet de gérer la distribution des
	/// commandes de l'interface graphique vers les routines de traitement.
	/// </summary>
	public class CommandDispatcher : System.IDisposable
	{
		static CommandDispatcher()
		{
			//	Capture le nom et les arguments d'une commande complexe, en filtrant les
			//	caractères et vérifiant ainsi la validité de la syntaxe. Voici l'inter-
			//	prétation de la regex :
			//
			//	- un <name> est constitué de caractères alphanumériques;
			//	- suit une parenthèse ouvrante, avec évtl. des espaces;
			//	- suit zéro à n arguments <arg> séparés par une virgule;
			//	- chaque <arg> est soit une chaîne "", soit une chaîne '',
			//	  soit une valeur numérique, soit un mot.
			//
			//	La capture retourne dans l'ordre <name>, puis la liste des <arg> trouvés.
			
			string regex_1 = @"\A(?<name>(\w+))\s*\(\s*((?<arg>((\""[^\""]{0,}\""|(\'[^\']{0,}\'|(\-{0,}\d{1,}([\.]\d{1,}){0,1}|[\w.]{1,})))))\s*(\,?)\s*){0,}\)\s*\z";
			
			//	Filtre les valeurs numériques correctement formatées, avec ou sans signe '-'
			//	comme préfixe et avec une partie fractionnaire ('.nnn') optionnelle.
			
			string regex_2 = @"\A\s*\-{0,}\d{1,}([\.]\d{1,}){0,1}\s*\z";
			
			RegexOptions options = RegexOptions.Compiled | RegexOptions.ExplicitCapture;
			
			CommandDispatcher.command_arg_regex = new Regex (regex_1, options);
			CommandDispatcher.numeric_regex     = new Regex (regex_2, options);
			
			CommandDispatcher.global_list = new System.Collections.ArrayList ();
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
			}
			else
			{
				this.dispatcher_name = name;
				CommandDispatcher.global_list.Add (this);
			}
			
			this.validation_rules = new ValidationRule (this);
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
		
		
		public void Dispatch(string command, object source)
		{
			//	Transmet la commande à ceux qui sont intéressés
			
			string   command_name     = CommandDispatcher.ExtractCommandName (command);
			string[] command_elements = command_name.Split ('/');
			int      command_length   = command_elements.Length;
			string[] command_args     = CommandDispatcher.ExtractAndParseCommandArgs (command, source);
			
			System.Diagnostics.Debug.Assert (command_length == 1);
			System.Diagnostics.Debug.Assert (command_name.IndexOf ("*") < 0, "Found '*' in command name.", "The command '" + command + "' may not contain a '*' in its name.\nPlease fix the command name definition source code.");
			System.Diagnostics.Debug.Assert (command_name.IndexOf (".") < 0, "Found '.' in command name.", "The command '" + command + "' may not contain a '.' in its name.\nPlease fix the command name definition source code.");
			
			CommandEventArgs e = new CommandEventArgs (source, command_name, command_args);
			
			EventSlot slot = this.event_handlers[command_name] as EventSlot;
			
			if (slot != null)
			{
				slot.Fire (this, e);
			}
			else
			{
				System.Diagnostics.Debug.WriteLine ("Command '" + command_name + "' not handled.");
			}
		}
		
		public void SynchroniseCommandStates()
		{
			//	Passe en revue tous les CommandStates connus et resynchronise ceux-ci. Afin d'éviter
			//	des surprises en cas de modifications en cours de synchronisation, on copie la liste
			//	dans une table temporaire :
			
			CommandState[] states = new CommandState[this.command_states.Count];
			this.command_states.CopyTo (states);
			
			for (int i = 0; i < states.Length; i++)
			{
				states[i].Synchronise ();
			}
		}
		
		public CommandDispatcher.CommandState[] FindCommandStates(string command_name)
		{
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			foreach (CommandState state in this.command_states)
			{
				if (state.Name == command_name)
				{
					list.Add (state);
				}
			}
			
			CommandState[] states = new CommandState[list.Count];
			list.CopyTo (states);
			
			return states;
		}
		
		
		public void ApplyValidationRules()
		{
			this.validation_rules.Validate ();
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
		
		internal void Register(string command, CommandEventHandler handler)
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
		
		internal void Unregister(string command, CommandEventHandler handler)
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
		
		
		public static string ExtractCommandName(string command)
		{
			int pos = command.IndexOf ('(');
			if (pos < 0)
			{
				return command;
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
				Match  match = CommandDispatcher.numeric_regex.Match (arg);
				
				if (match.Success)
				{
					//	C'est une valeur numérique proprement formatée. On la garde telle
					//	quelle.
				}
				else if ((arg[0] == '\'') || (arg[0] == '\"'))
				{
					//	C'est un texte entre guillemets. On supprime le premier et le dernier
					//	caractère.
					
					System.Diagnostics.Debug.Assert (arg.Length > 1);
					System.Diagnostics.Debug.Assert (arg[arg.Length-1] == arg[0]);
					
					arg = arg.Substring (1, arg.Length - 2);
				}
				else
				{
					//	Ce n'est ni une valeur numérique, ni un texte; c'est probablement un
					//	symbole que l'on va passer tel quel plus loin, sauf si c'est une
					//	expression commençant par 'this.'.
					
					if (arg.StartsWith ("this."))
					{
						//	L'argument décrit une propriété de la source. On va tenter d'aller
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
		
		
		protected void RegisterMethod(object controller, System.Reflection.MethodInfo info)
		{
			//	Ne parcourt que les attributs au niveau d'implémentation actuel (pas les classes dérivées,
			//	ni les classes parent). Le parcours des parent est assuré par l'appelant.
			
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
					//	La méthode n'a aucun argument :
					
					handler = new CommandEventHandler (relay.InvokeWithoutArgument);
					break;
				
				case 1:
					//	La méthode a un unique argument. Ce n'est acceptable que si cet argument est
					//	de type CommandDispatcher, soit :
					//
					//		void Method(CommandDispatcher)
					
					if (param_info[0].ParameterType == typeof (CommandDispatcher))
					{
						handler = new CommandEventHandler (relay.InvokeWithCommandDispatcher);
					}
					break;
				
				case 2:
					//	La méthode a deux arguments. Ce n'est acceptable que si le premier est de type
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
			}
		}
		
		
		#region EventRelay class
		protected class EventRelay
		{
			public EventRelay(object controller, System.Reflection.MethodInfo method_info)
			{
				//	Cette classe réalise un relais entre le delegate CommandEventHandler et les
				//	diverses implémentations possibles au niveau des gestionnaires de commandes.
				//	Ainsi, les méthodes :
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
		protected class EventSlot
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
			
			
			public void Fire(CommandDispatcher sender, CommandEventArgs e)
			{
				if (this.command != null)
				{
					this.command (sender, e);
				}
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
				
				System.Diagnostics.Debug.Assert (dispatcher.FindCommandStates (name).Length == 0, "CommandState created twice.", string.Format ("The CommandState {0} for dispatcher {1} already exists.\nIt cannot be created more than once.", name, dispatcher.Name));
				
				this.name       = name;
				this.dispatcher = dispatcher;
				this.regex      = Support.RegexFactory.FromSimpleJoker (this.name, Support.RegexFactory.Options.None);
				
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

			
			protected string					name;
			protected Support.CommandDispatcher	dispatcher;
			protected Regex						regex;
		}
		#endregion
		
		public static CommandDispatcher			Default
		{
			get { return CommandDispatcher.default_dispatcher; }
		}
		
		
		protected System.Collections.Hashtable	event_handlers = new System.Collections.Hashtable ();
		protected System.Collections.ArrayList	command_states = new System.Collections.ArrayList ();
		protected System.Collections.ArrayList	validation_states = new System.Collections.ArrayList ();
		protected string						dispatcher_name;
		protected ValidationRule				validation_rules;
		
		static Regex							command_arg_regex;
		static Regex							numeric_regex;
		static System.Type						command_attr_type  = typeof (CommandAttribute);
		static CommandDispatcher				default_dispatcher;
		static int								generation = 1;
		static System.Collections.ArrayList		global_list;
	}
}
