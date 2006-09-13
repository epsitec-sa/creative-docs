//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Text.RegularExpressions;
using System.Collections.Generic;

using Epsitec.Common.Types;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe CommandDispatcher permet de gérer la distribution des
	/// commandes de l'interface graphique vers les routines de traitement.
	/// </summary>
	public class CommandDispatcher : DependencyObject
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
			//	  soit une valeur numérique, soit un nom (avec des '.' pour
			//	  séparer les divers termes).
			//
			//	La capture retourne dans l'ordre <name>, puis la liste des <arg> trouvés.
			//	Il peut y avoir zéro à n arguments séparés par des virgules, le tout entre
			//	parenthèses.
			
			string regex_1 = @"\A(?<name>([a-zA-Z](\w|(\.\w))*))" +
				//	                      <---- nom valide --->
				/**/       @"\s*\(\s*((((?<arg>(" +
				/**/                          @"(\""[^\""]{0,}\"")|" +
				//	                            <-- guillemets -->
				/**/                          @"(\'[^\']{0,}\')|" +
				//	                            <-- apostr. -->
				/**/                          @"((\-|\+)?((\d{1,12}(\.\d{0,12})?0*)|(\d{0,12}\.(\d{0,12})?0*)))|" +
				//	                            <----------- valeur décimale avec signe en option ------------>
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

		/// <summary>
		/// Dispatches a command using the specified dispatcher and context chains.
		/// </summary>
		/// <param name="dispatcherChain">The dispatcher chain.</param>
		/// <param name="contextChain">The context chain.</param>
		/// <param name="commandObject">The command object.</param>
		/// <param name="source">The source of the command.</param>
		public static void Dispatch(CommandDispatcherChain dispatcherChain, CommandContextChain contextChain, Command commandObject, object source)
		{
			if (dispatcherChain != null)
			{
				foreach (CommandDispatcher dispatcher in dispatcherChain.Dispatchers)
				{
					if (dispatcher.InternalDispatchSingleCommand (contextChain, commandObject, source))
					{
						break;
					}
				}
			}
		}
		
		
		public void RegisterController(object controller)
		{
			if (controller != null)
			{
				System.Type type = controller.GetType ();
				
				foreach (System.Reflection.MemberInfo member in type.GetMembers (System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic))
				{
					if ((member.IsDefined (CommandDispatcher.commandAttributeType, true)) &&
						(member.MemberType == System.Reflection.MemberTypes.Method))
					{
						System.Reflection.MethodInfo info = member as System.Reflection.MethodInfo;
						this.RegisterMethod (controller, info);
					}
				}
			}
		}

		public void Register(Command command, Support.SimpleCallback handler)
		{
			this.Register (command, delegate (CommandDispatcher d, CommandEventArgs e) { handler (); });
		}

		public void Register(Command command, CommandEventHandler handler)
		{
			EventSlot slot;
			
			if (this.eventHandlers.TryGetValue (command, out slot) == false)
			{
				slot = new EventSlot (command);
				this.eventHandlers[command] = slot;
			}
			
			slot.Register (handler);
		}
		
		public void Unregister(Command command, CommandEventHandler handler)
		{
			EventSlot slot;
			
			if (this.eventHandlers.TryGetValue (command, out slot))
			{
				slot.Unregister (handler);
				
				if (slot.IsEmpty)
				{
					this.eventHandlers.Remove (command);
				}
			}
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
		private sealed class EventSlot : ICommandDispatcher
		{
			public EventSlot(Command command)
			{
				this.command = command;
			}
			
			
			public void Register(CommandEventHandler handler)
			{
				this.handler += handler;
			}
			
			public void Unregister(CommandEventHandler handler)
			{
				this.handler -= handler;
			}
			
			
			public bool DispatchCommand(CommandDispatcher sender, CommandEventArgs e)
			{
				if (this.handler != null)
				{
					this.handler (sender, e);
					return true;
				}
				
				return false;
			}
			
			
			public bool							IsEmpty
			{
				get
				{
					return this.handler == null;
				}
			}
			
			
			private Command						command;
			private event CommandEventHandler	handler;
		}
		#endregion
		
		protected void RegisterMethod(object controller, System.Reflection.MethodInfo info)
		{
			//	Ne parcourt que les attributs au niveau d'implémentation actuel (pas les classes dérivées,
			//	ni les classes parent). Le parcours des parent est assuré par l'appelant.
			
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
			
			this.Register (Command.Get (attribute.CommandName), handler);
			
		}


		protected bool InternalDispatchSingleCommand(CommandContextChain contextChain, Command commandObject, object source)
		{
			//	Transmet la commande à ceux qui sont intéressés

			if (commandObject == null)
			{
				return false;
			}
			
			CommandContext commandContext;
			CommandState commandState;

			int loopCounter = 0;

		again:
			
			if (commandObject.IsReadWrite)
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

				commandState = contextChain.GetCommandState (commandObject, out commandContext);

				System.Diagnostics.Debug.Assert (commandContext != null);
				System.Diagnostics.Debug.Assert (commandState != null);
				System.Diagnostics.Debug.Assert (commandState.Command != null);

				if (commandState.Enable == false)
				{
					return false;
				}

				if (contextChain.GetLocalEnable (commandState.Command) == false)
				{
					return false;
				}
			}
			
			CommandEventArgs e = new CommandEventArgs (source, commandObject, commandContext, commandState);

			EventSlot slot;
			int handled = 0;
			
			if (this.eventHandlers.TryGetValue (commandObject, out slot))
			{
				System.Diagnostics.Debug.WriteLine ("Command '" + commandObject.CommandId + "' (" + commandObject.Name + ") fired.");
				
				if (slot.DispatchCommand (this, e))
				{
					handled++;
				}
			}

			if (handled == 0)
			{
				if (loopCounter++ > 10)
				{
					throw new Exceptions.InfiniteCommandLoopException ();
				}
				
				if ((commandState != null) &&
					(commandObject.CommandType == CommandType.Multiple))
				{
					commandObject = MultiCommand.GetSelectedCommand (commandState);

					if (commandObject != null)
					{
						goto again;
					}
				}

				System.Diagnostics.Debug.WriteLine ("Command '" + commandObject.CommandId + "' (" + commandObject.Name + ") not handled.");
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
					System.Diagnostics.Debug.WriteLine ("Command '" + commandObject.CommandId + "' (" + commandObject.Name + ") handled; not marked as executed.");
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
			//	Indique qu'une commande (ou un paquet de commandes) a été exécutée.
			
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

		private Dictionary<Command, EventSlot>	eventHandlers = new Dictionary<Command, EventSlot> ();
		
		protected Support.OpletQueue			opletQueue;
		
		static object							exclusion = new object ();
		
		static Regex							commandArgRegex;
		static System.Type						commandAttributeType;
		
		static CommandDispatcher				defaultDispatcher;
		static long								nextId;
	}
}
