//	Copyright � 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Text.RegularExpressions;
using System.Collections.Generic;

using Epsitec.Common.Types;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe CommandDispatcher permet de g�rer la distribution des
	/// commandes de l'interface graphique vers les routines de traitement.
	/// </summary>
	public class CommandDispatcher : DependencyObject
	{
		static CommandDispatcher()
		{
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
				//	                      <---- nom valide --->
				/**/       @"\s*\(\s*((((?<arg>(" +
				/**/                          @"(\""[^\""]{0,}\"")|" +
				//	                            <-- guillemets -->
				/**/                          @"(\'[^\']{0,}\')|" +
				//	                            <-- apostr. -->
				/**/                          @"((\-|\+)?((\d{1,12}(\.\d{0,12})?0*)|(\d{0,12}\.(\d{0,12})?0*)))|" +
				//	                            <----------- valeur d�cimale avec signe en option ------------>
				/**/                          @"([a-zA-Z](\w|(\.\w))*)))" +
				//	                            <---- nom valide ---->
				/**/                         @"((\s*\,\s*)|(\s*\)\s*\z)))*)|(\)\s*))\z";
			
			RegexOptions options = RegexOptions.Compiled | RegexOptions.ExplicitCapture;
			
			CommandDispatcher.commandArgRegex = new Regex (regex_1, options);
			CommandDispatcher.commandAttributeType = typeof (Support.CommandAttribute);
			
			CommandDispatcher defaultDispatcher = new CommandDispatcher ("default", CommandDispatcherLevel.Root);
			
			System.Diagnostics.Debug.Assert (defaultDispatcher == CommandDispatcher.defaultDispatcher);
			System.Diagnostics.Debug.Assert (defaultDispatcher.id == 1);
		}
		
		
		public CommandDispatcher() : this ("anonymous", CommandDispatcherLevel.Secondary)
		{
		}
		
		public CommandDispatcher(string name, CommandDispatcherLevel level)
		{
			lock (CommandDispatcher.exclusion)
			{
				this.name  = name;
				this.level = level;
				this.id    = System.Threading.Interlocked.Increment (ref CommandDispatcher.nextId);
				
				switch (level)
				{
					case CommandDispatcherLevel.Root:
						if (CommandDispatcher.defaultDispatcher == null)
						{
							CommandDispatcher.defaultDispatcher = this;
						}
						else
						{
							throw new System.InvalidOperationException ("Root command dispatcher already defined");
						}
						break;
					
					case CommandDispatcherLevel.Secondary:
					case CommandDispatcherLevel.Primary:
						break;
					
					default:
						throw new System.ArgumentException (string.Format ("CommandDispatcherLevel {0} not valid for dispatcher {1}", level, name), "level");
				}
			}
		}
		
		
		
		public string							Name
		{
			get
			{
				return this.name;
			}
		}
		
		public CommandDispatcherLevel			Level
		{
			get
			{
				return this.level;
			}
		}

#if false //#fix
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
#endif
		
		public Support.OpletQueue				OpletQueue
		{
			get
			{
				return this.opletQueue;
			}
			set
			{
				if (this.opletQueue != value)
				{
					this.opletQueue = value;
					this.OnOpletQueueBindingChanged ();
				}
			}
		}
		
		public static void Dispatch(CommandDispatcherChain dispatcherChain, CommandContextChain contextChain, string command, object source)
		{
			if (dispatcherChain != null)
			{
				foreach (CommandDispatcher dispatcher in dispatcherChain.Dispatchers)
				{
					if (dispatcher.InternalDispatch (contextChain, command, source))
					{
						break;
					}
				}
			}
		}


		public bool InternalDispatch(CommandContextChain contextChain, string command, object source)
		{
			
#if false //#fix
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
#endif
			bool handled = false;
			
#if true //#fix
			handled = this.InternalDispatchSingleCommand (contextChain, command, source);
#else
			try
			{
				this.pending_commands.Push (null);
				handled = this.InternalDispatchSingleCommand (command, source);
			}
			finally
			{
				this.pending_commands.Pop ();
			}
#endif

			return handled;
		}
		
#if false //#fix
		public void InternalCancelTopPendingMultipleCommands()
		{
			this.pending_commands.Pop ();
			this.pending_commands.Push (null);
		}
#endif
		
		
		public void RegisterController(object controller)
		{
			if (controller != null)
			{
				System.Type type = controller.GetType ();
				System.Reflection.MemberInfo[] members = type.GetMembers (System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
				
				for (int i = 0; i < members.Length; i++)
				{
					if ((members[i].IsDefined (CommandDispatcher.commandAttributeType, true)) &&
						(members[i].MemberType == System.Reflection.MemberTypes.Method))
					{
						System.Reflection.MethodInfo info = members[i] as System.Reflection.MethodInfo;
						this.RegisterMethod (controller, info);
					}
				}
			}
		}
		
		public void Register(string commandName, CommandEventHandler handler)
		{
			System.Diagnostics.Debug.Assert (commandName.IndexOf ("*") < 0, "Found '*' in command name.", "The command '" + commandName + "' may not contain a '*' in its name.\nPlease fix the registration source code.");
			System.Diagnostics.Debug.Assert (commandName.IndexOf (".") < 0, "Found '.' in command name.", "The command '" + commandName + "' may not contain a '.' in its name.\nPlease fix the registration source code.");
			
			EventSlot slot;
			
			if (this.eventHandlers.TryGetValue (commandName, out slot) == false)
			{
				slot = new EventSlot (commandName);
				this.eventHandlers[commandName] = slot;
			}
			
			slot.Register (handler);
		}
		
		public void Unregister(string commandName, CommandEventHandler handler)
		{
			EventSlot slot;
			
			if (this.eventHandlers.TryGetValue (commandName, out slot))
			{
				slot.Unregister (handler);
				
				if (slot.Count == 0)
				{
					this.eventHandlers.Remove (commandName);
				}
			}
		}
		
		public void Register(ICommandDispatcher extra)
		{
			if (this.extraDispatchers == null)
			{
				this.extraDispatchers = new List<ICommandDispatcher> ();
			}
			
			this.extraDispatchers.Add (extra);
		}
		
		public void Unregister(ICommandDispatcher extra)
		{
			if (this.extraDispatchers != null)
			{
				this.extraDispatchers.Remove (extra);
				
				if (this.extraDispatchers.Count == 0)
				{
					this.extraDispatchers = null;
				}
			}
		}
		
		public static bool IsSimpleCommand(string commandName)
		{
			if (string.IsNullOrEmpty (commandName))
			{
				return true;
			}
			
			int pos = commandName.IndexOf ('(');
			return (pos < 0) ? true : false;
		}
		
		
		public static string   ExtractCommandName(string commandName)
		{
			if (string.IsNullOrEmpty (commandName))
			{
				return null;
			}
			
			int pos = commandName.IndexOf ('(');
			
			if (pos >= 0)
			{
				commandName = commandName.Substring (0, pos);
			}
			
			commandName = commandName.Trim ();
			
			if (commandName.Length == 0)
			{
				return null;
			}
			else
			{
				return commandName;
			}
		}
		
		public static string[] ExtractCommandArgs(string commandName)
		{
			if (string.IsNullOrEmpty (commandName))
			{
				return new string[0];
			}
			
			int pos = commandName.IndexOf ('(');
			
			if (pos < 0)
			{
				return new string[0];
			}
			
			Match match = CommandDispatcher.commandArgRegex.Match (commandName);
			
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
			
			throw new System.FormatException (string.Format ("Command '{0}' is not well formed.", commandName));
		}
		
		public static string[] ExtractAndParseCommandArgs(string commandLine, object source)
		{
			string[] args = CommandDispatcher.ExtractCommandArgs (commandLine);
			
			for (int i = 0; i < args.Length; i++)
			{
				string arg   = args[i];
				Match  match = Support.RegexFactory.InvariantDecimalNum.Match (arg);
				
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
							throw new System.FieldAccessException (string.Format ("Command {0} tries to access property {1} which cannot be found in class {2}.", commandLine, prop_name, type.Name));
						}
						
						object data = info.GetValue (source, null);
						
						arg = data.ToString ();
					}
				}
				
				args[i] = arg;
			}
			
			return args;
		}
		
		
		
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
			}
			
			base.Dispose (disposing);
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
		
		protected void RegisterMethod(object controller, System.Reflection.MethodInfo info)
		{
			//	Ne parcourt que les attributs au niveau d'impl�mentation actuel (pas les classes d�riv�es,
			//	ni les classes parent). Le parcours des parent est assur� par l'appelant.
			
			object[] attributes = info.GetCustomAttributes (CommandDispatcher.commandAttributeType, false);
			
			foreach (Support.CommandAttribute attribute in attributes)
			{
				this.RegisterMethod (controller, info, attribute);
			}
		}
		
		protected void RegisterMethod(object controller, System.Reflection.MethodInfo method_info, Support.CommandAttribute attribute)
		{
//-			System.Diagnostics.Debug.WriteLine ("Command '" + attribute.CommandName + "' implemented by method " + method_info.Name + " in class " + method_info.DeclaringType.Name + ", prototype: " + method_info.ToString ());
			
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


		protected bool InternalDispatchSingleCommand(CommandContextChain contextChain, string commandLine, object source)
		{
			//	Transmet la commande � ceux qui sont int�ress�s

			string commandName = CommandDispatcher.ExtractCommandName (commandLine);
			
			if (commandName == null)
			{
				return false;
			}

			int loopCounter = 0;

			Command command = Command.Find (commandName);
			CommandContext commandContext;
			CommandState commandState;

		again:
			
			if ((command == null) ||
				(command.IsReadWrite))
			{
				//	The command will always be considered to be "enabled" if it
				//	has never been defined as such or if no command state has ever
				//	been created for it.

				commandContext = null;
				commandState = null;
			}
			else
			{
				System.Diagnostics.Debug.Assert (contextChain != null);
				System.Diagnostics.Debug.Assert (contextChain.IsEmpty == false);
				
				commandState = contextChain.GetCommandState (command, out commandContext);

				System.Diagnostics.Debug.Assert (commandContext != null);
				System.Diagnostics.Debug.Assert (commandState != null);
				System.Diagnostics.Debug.Assert (commandState.Command != null);

				if (commandState.Enable == false)
				{
					return false;
				}
			}
			
			string[] commandElements = commandName.Split ('/');
			int      commandLength   = commandElements.Length;
			string[] commandArgs     = CommandDispatcher.ExtractAndParseCommandArgs (commandLine, source);
			
			System.Diagnostics.Debug.Assert (commandLength == 1);
			System.Diagnostics.Debug.Assert (commandName.IndexOf ("*") < 0, "Found '*' in command name.", "The command '" + commandLine + "' may not contain a '*' in its name.\nPlease fix the command name definition source code.");
			System.Diagnostics.Debug.Assert (commandName.IndexOf (".") < 0, "Found '.' in command name.", "The command '" + commandLine + "' may not contain a '.' in its name.\nPlease fix the command name definition source code.");

			CommandEventArgs e = new CommandEventArgs (source, command, commandArgs, commandContext, commandState);

			EventSlot slot;
			int handled = 0;
			
			
			if (this.eventHandlers.TryGetValue (commandName, out slot))
			{
				System.Diagnostics.Debug.WriteLine ("Command '" + commandName + "' fired.");
				
				if (slot.DispatchCommand (this, e))
				{
					handled++;
				}
			}

			if ((this.extraDispatchers != null) &&
				(this.extraDispatchers.Count > 0))
			{
				ICommandDispatcher[] extra = this.extraDispatchers.ToArray ();
				
				for (int i = 0; i < extra.Length; i++)
				{
					if (extra[i].DispatchCommand (this, e))
					{
						handled++;
					}
				}
			}
			
			if (handled == 0)
			{
				if (loopCounter++ > 10)
				{
					throw new Exceptions.InfiniteCommandLoopException ();
				}
				
				if ((commandState != null) &&
					(command is MultiCommand))
				{
					command = MultiCommand.GetSelectedCommand (commandState);
					
					if (command != null)
					{
						commandName = command.Name;
						commandLine = commandName;

						goto again;
					}
				}
				
				System.Diagnostics.Debug.WriteLine ("Command '" + commandName + "' not handled.");
				return false;
			}
			else
			{
				if (e.Executed)
				{
					this.OnCommandDispatched ();
					return true;
				}
				else
				{
					System.Diagnostics.Debug.WriteLine ("Command '" + commandName + "' handled but not marked as executed.");
					return false;
				}
			}
		}
		
		
		protected void OnOpletQueueBindingChanged()
		{
			if (this.OpletQueueBindingChanged != null)
			{
				this.OpletQueueBindingChanged (this);
			}
		}
		
		protected void OnCommandDispatched()
		{
			//	Indique qu'une commande (ou un paquet de commandes) a �t� ex�cut�e.
			
			if (this.CommandDispatched != null)
			{
				this.CommandDispatched (this);
			}
		}
		
		
		public static CommandDispatcher GetDispatcher(DependencyObject obj)
		{
			return (CommandDispatcher) obj.GetValue (CommandDispatcher.DispatcherProperty);
		}

		public static void SetDispatcher(DependencyObject obj, CommandDispatcher value)
		{
			obj.SetValue (CommandDispatcher.DispatcherProperty, value);
		}

		public static void ClearDispatcher(DependencyObject obj)
		{
			obj.ClearValue (CommandDispatcher.DispatcherProperty);
		}
		
		public static readonly DependencyProperty DispatcherProperty = DependencyProperty.RegisterAttached ("Dispatcher", typeof (CommandDispatcher), typeof (CommandDispatcher), new DependencyPropertyMetadata ().MakeNotSerializable ());
		
		public event Support.EventHandler		OpletQueueBindingChanged;
		public event Support.EventHandler		CommandDispatched;
		
		private string							name;
		private CommandDispatcherLevel			level;
		private long							id;

		protected Dictionary<string, EventSlot> eventHandlers    = new Dictionary<string, EventSlot> ();
//#		protected System.Collections.Stack		pending_commands  = new System.Collections.Stack ();
		protected List<ICommandDispatcher>		extraDispatchers;
		
		protected Support.OpletQueue			opletQueue;
		
		static object							exclusion = new object ();
		
		static Regex							commandArgRegex;
		static System.Type						commandAttributeType;
		
		static CommandDispatcher				defaultDispatcher;
		static long								nextId;
	}
}
