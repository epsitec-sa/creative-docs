//	Copyright � 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Text.RegularExpressions;
using System.Collections.Generic;

using Epsitec.Common.Types;

namespace Epsitec.Common.Widgets
{
	using BindingFlags=System.Reflection.BindingFlags;

	/// <summary>
	/// La classe CommandDispatcher permet de g�rer la distribution des
	/// commandes de l'interface graphique vers les routines de traitement.
	/// </summary>
	public class CommandDispatcher : DependencyObject
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CommandDispatcher"/> class.
		/// </summary>
		public CommandDispatcher() : this ("anonymous", CommandDispatcherLevel.Secondary)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CommandDispatcher"/> class.
		/// </summary>
		/// <param name="name">The dispatcher name.</param>
		/// <param name="level">The dispatcher level.</param>
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



		/// <summary>
		/// Gets the name of the dispatcher.
		/// </summary>
		/// <value>The name.</value>
		public string							Name
		{
			get
			{
				return this.name;
			}
		}

		/// <summary>
		/// Gets the level of the dispatcher.
		/// </summary>
		/// <value>The level.</value>
		public CommandDispatcherLevel			Level
		{
			get
			{
				return this.level;
			}
		}

		/// <summary>
		/// Gets or sets the oplet queue associated with the dispatcher.
		/// </summary>
		/// <value>The oplet queue.</value>
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
		/// Gets or sets a value indicating whether this dispatcher auto forwards commands
		/// to its predecessor when linked in a dispatcher chain.
		/// </summary>
		/// <value><c>true</c> to auto forward commands; otherwise, <c>false</c>.</value>
		public bool								AutoForwardCommands
		{
			get
			{
				return this.autoForwardCommands;
			}
			set
			{
				this.autoForwardCommands = value;
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
			if ((dispatcherChain != null) &&
				(commandObject != null))
			{
				foreach (CommandDispatcher dispatcher in dispatcherChain.Dispatchers)
				{
					if (dispatcher.DispatchCommand (contextChain, commandObject, source))
					{
						break;
					}
				}
			}
		}


		/// <summary>
		/// Registers a command controller. The object must implement methods marked
		/// with the <see cref="Epsitec.Common.Support.CommandAttribute"/> attribute.
		/// </summary>
		/// <param name="controller">The controller.</param>
		public void RegisterController(object target)
		{
			object controller = CommandDispatcher.ResolveWeakController (target);

			if (controller != null)
			{
				System.Type type = controller.GetType ();
				
				foreach (System.Reflection.MemberInfo member in type.GetMembers (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
				{
					if ((member.IsDefined (CommandDispatcher.commandAttributeType, true)) &&
						(member.MemberType == System.Reflection.MemberTypes.Method))
					{
						this.RegisterMethod (target, member as System.Reflection.MethodInfo);
					}
				}
			}
		}

		/// <summary>
		/// Registers the specified command handler.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <param name="handler">The command handler.</param>
		public void Register(Command command, Support.SimpleCallback handler)
		{
			this.Register (command, delegate (CommandDispatcher d, CommandEventArgs e) { handler (); });
		}

		/// <summary>
		/// Registers the specified command handler.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <param name="handler">The command handler.</param>
		public void Register(Command command, CommandEventHandler handler)
		{
			this.Register (command, new DelegateHandler (handler));
		}

		/// <summary>
		/// Unregisters the specified command handler.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <param name="handler">The command handler.</param>
		public void Unregister(Command command, CommandEventHandler handler)
		{
			EventSlot slot;
			
			if (this.eventHandlers.TryGetValue (command, out slot))
			{
				slot.Unregister (new DelegateHandler (handler));
				
				if (slot.IsEmpty)
				{
					this.eventHandlers.Remove (command);
				}
			}
		}

		/// <summary>
		/// Registers several command/handler pairs.
		/// </summary>
		/// <param name="commandHandlers">The command/handler pairs.</param>
		public void RegisterRange(IEnumerable<CommandHandlerPair> commandHandlers)
		{
			foreach (var item in commandHandlers)
			{
				this.Register (item.Command, item.Handler);
			}
		}

		/// <summary>
		/// Unregisters several command/handler pairs.
		/// </summary>
		/// <param name="commandHandlers">The command/handler pairs.</param>
		public void UnregisterRange(IEnumerable<CommandHandlerPair> commandHandlers)
		{
			foreach (var item in commandHandlers)
			{
				this.Unregister (item.Command, item.Handler);
			}
		}


		
		private void Register(Command command, AbstractHandler handler)
		{
			EventSlot slot;

			if (this.eventHandlers.TryGetValue (command, out slot) == false)
			{
				slot = new EventSlot (command);
				this.eventHandlers[command] = slot;
			}

			slot.Register (handler);
		}

		private void Unregister(MethodHandler handler)
		{
			List<Command> list = new List<Command> ();

			foreach (EventSlot slot in this.eventHandlers.Values)
			{
				slot.Unregister (handler);

				if (slot.IsEmpty)
				{
					list.Add (slot.Command);
				}
			}

			foreach (Command command in list)
			{
				this.eventHandlers.Remove (command);
			}
		}

		/// <summary>
		/// Checks if the dispatcher contains a handler for the specified command.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <returns><c>true</c> if the dispatcher knows how to execute the command;
		/// otherwise, <c>false</c>.</returns>
		public bool Contains(Command command)
		{
			if (this.eventHandlers.ContainsKey (command))
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				//	...
			}
			
			base.Dispose (disposing);
		}
		
		static CommandDispatcher()
		{
#if false
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
			
			string regex1 = @"\A(?<name>([a-zA-Z](\w|(\.\w))*))" +
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
			
			CommandDispatcher.commandArgRegex = new Regex (regex1, options);
#endif

			CommandDispatcher.commandAttributeType = typeof (Support.CommandAttribute);

			CommandDispatcher defaultDispatcher = new CommandDispatcher ("default", CommandDispatcherLevel.Root);

			System.Diagnostics.Debug.Assert (defaultDispatcher == CommandDispatcher.defaultDispatcher);
			System.Diagnostics.Debug.Assert (defaultDispatcher.id == 1);
		}

		private static object ResolveWeakController(object controller)
		{
			System.WeakReference weak = controller as System.WeakReference;

			if (weak == null)
			{
				return controller;
			}
			else
			{
				return weak.Target;
			}
		}
		
		private void RegisterMethod(object target, System.Reflection.MethodInfo info)
		{
			object[] attributes = info.GetCustomAttributes (CommandDispatcher.commandAttributeType, true);
			
			foreach (Support.CommandAttribute attribute in attributes)
			{
				this.RegisterMethod (target, info, attribute);
			}
		}

		private void RegisterMethod(object target, System.Reflection.MethodInfo methodInfo, Support.CommandAttribute attribute)
		{
			if (string.IsNullOrEmpty (attribute.CommandName))
			{
				this.Register (Command.Get (attribute.CommandId), new MethodHandler (target, methodInfo));
			}
			else
			{
				this.Register (Command.Get (attribute.CommandName), new MethodHandler (target, methodInfo));
			}
		}

		#region EventSlot Class
		
		/// <summary>
		/// The <c>EventSlot</c> class is used to map a command to one or more
		/// command handlers (see <see cref="AbstractHandler"/>).
		/// </summary>
		private sealed class EventSlot : ICommandDispatcher
		{
			public EventSlot(Command command)
			{
				this.command = command;
				this.handlers = new List<AbstractHandler> ();
			}
			
			public bool							IsEmpty
			{
				get
				{
					return this.handlers.Count == 0;
				}
			}

			public Command						Command
			{
				get
				{
					return this.command;
				}
			}

			/// <summary>
			/// Registers the specified handler. Duplicates are allowed.
			/// </summary>
			/// <param name="handler">The handler.</param>
			public void Register(AbstractHandler handler)
			{
				this.handlers.Add (handler);
			}

			/// <summary>
			/// Unregisters the specified handler. The comparison uses instance
			/// equality, not reference equality.
			/// </summary>
			/// <param name="handler">The handler.</param>
			public void Unregister(AbstractHandler handler)
			{
				this.handlers.RemoveAll (x => x.Equals (handler));
			}

			#region ICommandDispatcher Members

			public bool ExecuteCommand(CommandDispatcher sender, CommandEventArgs e)
			{
				AbstractHandler[] handlers = this.handlers.ToArray ();
				bool executed = false;

				foreach (AbstractHandler handler in handlers)
				{
					if (handler.Invoke (sender, e))
					{
						executed = true;
					}
				}
				
				return executed;
			}

			#endregion

			readonly Command					command;
			readonly List<AbstractHandler>		handlers;
		}
		
		#endregion

		#region AbstractHandler Class

		/// <summary>
		/// The <c>AbstractHandler</c> class is an abstract base class used to
		/// implement <see cref="DelegateHandler"/> and <see cref="MethodHandler"/>.
		/// </summary>
		private abstract class AbstractHandler
		{
			public abstract bool Invoke(CommandDispatcher dispatcher, CommandEventArgs e);

			public abstract bool Equals(AbstractHandler other);
		}

		#endregion

		#region DelegateHandler Class

		/// <summary>
		/// The <c>DelegateHandler</c> class knows how to invoke a delegate
		/// which maps to a command handler.
		/// </summary>
		private class DelegateHandler : AbstractHandler
		{
			public DelegateHandler(CommandEventHandler handler)
			{
				this.handler = handler;
			}

			public override bool Invoke(CommandDispatcher dispatcher, CommandEventArgs e)
			{
				if (this.handler != null)
				{
					this.handler (dispatcher, e);
					return true;
				}
				else
				{
					return false;
				}
			}

			public override bool Equals(AbstractHandler obj)
			{
				DelegateHandler other = obj as DelegateHandler;

				if (other == null)
				{
					return false;
				}
				else
				{
					return this.handler == other.handler;
				}
			}

			private readonly CommandEventHandler handler;
		}

		#endregion

		#region MethodHandler Class

		/// <summary>
		/// The <c>MethodHandler</c> class knows how to invoke the method of a
		/// target object, which might be defined as a weak reference.
		/// </summary>
		private class MethodHandler : AbstractHandler
		{
			public MethodHandler(object controller, System.Reflection.MethodInfo methodInfo)
			{
				System.Reflection.ParameterInfo[] paramInfo = methodInfo.GetParameters ();
				
				switch (paramInfo.Length)
				{
					case 0:		//	void Method()
						this.methodVersion = MethodVersion.ZeroArgument;
						break;

					case 1:		//	void Method(CommandDispatcher)
						if (paramInfo[0].ParameterType == typeof (CommandDispatcher))
						{
							this.methodVersion = MethodVersion.DispatcherOnly;
						}
						break;

					case 2:		//	void Method(CommandDispatcher, CommandEventArgs)
						if ((paramInfo[0].ParameterType == typeof (CommandDispatcher)) &&
							(paramInfo[1].ParameterType == typeof (CommandEventArgs)))
						{
							this.methodVersion = MethodVersion.DispatcherAndArgs;
						}
						break;
				}

				if (this.methodVersion == MethodVersion.Undefined)
				{
					throw new System.FormatException (string.Format ("{0}.{1} uses invalid signature: {2}.", controller.GetType ().Name, methodInfo.Name, methodInfo.ToString ()));
				}

				this.controller = controller;
				this.methodInfo = methodInfo;
			}

			public object Target
			{
				get
				{
					return CommandDispatcher.ResolveWeakController (this.controller);
				}
			}

			public override bool Invoke(CommandDispatcher dispatcher, CommandEventArgs e)
			{
				object target = this.Target;

				if (target == null)
				{
					dispatcher.Unregister (this);
				}
				else
				{
					switch (this.methodVersion)
					{
						case MethodVersion.ZeroArgument:
							this.methodInfo.Invoke (target, null);
							return true;

						case MethodVersion.DispatcherOnly:
							this.methodInfo.Invoke (target, new object[] { dispatcher });
							return true;

						case MethodVersion.DispatcherAndArgs:
							this.methodInfo.Invoke (target, new object[] { dispatcher, e });
							return true;
					}
				}

				return false;
			}

			public override bool Equals(AbstractHandler obj)
			{
				MethodHandler other = obj as MethodHandler;

				if (other == null)
				{
					return false;
				}
				else
				{
					return this.Target == other.Target
						&& this.methodInfo == other.methodInfo;
				}
			}

			private enum MethodVersion
			{
				Undefined,
				ZeroArgument,
				DispatcherOnly,
				DispatcherAndArgs
			}

			private readonly object controller;
			private readonly MethodVersion methodVersion;
			private readonly System.Reflection.MethodInfo methodInfo;
		}

		#endregion

		private bool DispatchCommand(CommandContextChain contextChain, Command commandObject, object source)
		{
			//	Transmet la commande � ceux qui sont int�ress�s

			if (commandObject == null)
			{
				return false;
			}
			
			CommandContext commandContext;
			CommandState   commandState;

			int loopCounter = 0;

		again:
			
			if (commandObject.IsReadWrite)
			{
				//	The command will always be considered to be "enabled" if it
				//	has never been defined as such or if no command state has ever
				//	been created for it.

				commandContext = null;
				commandState   = null;
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

			this.OnCommandDispatching ();

			EventSlot slot;
			int handled = 0;
			
			if (this.eventHandlers.TryGetValue (commandObject, out slot))
			{
				System.Diagnostics.Debug.WriteLine ("Command '" + commandObject.CommandId + "' (" + commandObject.Name + ") fired.");
				
				if (slot.ExecuteCommand (this, e))
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
				this.OnCommandDispatched ();
				
				if (e.Executed)
				{
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

		protected void OnCommandDispatching()
		{
			if (this.CommandDispatching != null)
			{
				this.CommandDispatching (this);
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


		/// <summary>
		/// Gets the dispatcher associated with a given object.
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <returns>The dispatcher associated with the object, or <c>null</c>.</returns>
		public static CommandDispatcher GetDispatcher(DependencyObject obj)
		{
			return (CommandDispatcher) obj.GetValue (CommandDispatcher.DispatcherProperty);
		}

		/// <summary>
		/// Sets the dispatcher associated with a given object.
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <param name="value">The dispatcher.</param>
		public static void SetDispatcher(DependencyObject obj, CommandDispatcher value)
		{
			obj.SetValue (CommandDispatcher.DispatcherProperty, value);
		}

		/// <summary>
		/// Clears the dispatcher associated with a given object.
		/// </summary>
		/// <param name="obj">The object.</param>
		public static void ClearDispatcher(DependencyObject obj)
		{
			obj.ClearValue (CommandDispatcher.DispatcherProperty);
		}
		
		public static readonly DependencyProperty DispatcherProperty = DependencyProperty.RegisterAttached ("Dispatcher", typeof (CommandDispatcher), typeof (CommandDispatcher), new DependencyPropertyMetadata ().MakeNotSerializable ());

		public static CommandDispatcher			DefaultDispatcher
		{
			get
			{
				return CommandDispatcher.defaultDispatcher;
			}
		}
		
		public event Support.EventHandler		OpletQueueBindingChanged;
		public event Support.EventHandler		CommandDispatching;
		public event Support.EventHandler		CommandDispatched;
		
		private string							name;
		private CommandDispatcherLevel			level;
		private long							id;
		private bool							autoForwardCommands;

		private Dictionary<Command, EventSlot>	eventHandlers = new Dictionary<Command, EventSlot> ();
		
		private Support.OpletQueue				opletQueue;
		
		static object							exclusion = new object ();
		
//-		static Regex							commandArgRegex;
		static System.Type						commandAttributeType;
		
		static CommandDispatcher				defaultDispatcher;
		static long								nextId;
	}
}
