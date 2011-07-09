//	Copyright © 2003-2011, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Widgets
{
	using BindingFlags=System.Reflection.BindingFlags;

	/// <summary>
	/// La classe CommandDispatcher permet de gérer la distribution des
	/// commandes de l'interface graphique vers les routines de traitement.
	/// </summary>
	public class CommandDispatcher : DependencyObject
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CommandDispatcher"/> class.
		/// </summary>
		/// <param name="name">The dispatcher name (defaults to <c>"anonymous"</c>).</param>
		/// <param name="level">The dispatcher level (defaults to <c>Secondary</c>).</param>
		/// <param name="options">The dispatcher options (defaults to <c>None</c>).</param>
		public CommandDispatcher(string name = "anonymous", CommandDispatcherLevel level = CommandDispatcherLevel.Secondary, CommandDispatcherOptions options = CommandDispatcherOptions.None)
		{
			lock (CommandDispatcher.exclusion)
			{
				this.name    = name;
				this.level   = level;
				this.options = options;
				this.id      = System.Threading.Interlocked.Increment (ref CommandDispatcher.nextId);
				
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
				return this.options.HasFlag (CommandDispatcherOptions.AutoForwardCommands);
			}
		}

		/// <summary>
		/// Gets a value indicating whether this dispatcher, when attached to a <see cref="Visual"/>,
		/// will be visible in the <see cref="CommandDispatcherChain"/> even if the visual does
		/// not have the focus.
		/// </summary>
		/// <value>
		///   <c>true</c> if this command dispatcher should be active in a window, even if the
		///   attached visual does not have the focus; otherwise, <c>false</c>.
		/// </value>
		public bool ActiveWithoutFocus
		{
			get
			{
				return this.options.HasFlag (CommandDispatcherOptions.ActivateWithoutFocus);
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
			//	Transmet la commande à ceux qui sont intéressés

			if (commandObject == null)
			{
				return false;
			}
			
			CommandContext commandContext;
			CommandState   commandState;

			if (commandObject.IsTemporary)
			{
				//	Don't fetch a state for a temporary command -- anyways, we should never
				//	dispatch to a temporary command !?

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
			
			CommandEventArgs e = new CommandEventArgs (source, commandObject, contextChain, commandContext, commandState);

			this.OnCommandDispatching (e);

			if (e.Cancel)
			{
				return true;
			}

			EventSlot slot;
			
			if (this.eventHandlers.TryGetValue (commandObject, out slot))
			{
				System.Diagnostics.Debug.WriteLine ("Command '" + commandObject.CommandId + "' (" + commandObject.Name + ") fired.");
				
				if (slot.ExecuteCommand (this, e))
				{
					e.Handled = true;
				}
			}

			this.OnCommandDispatched (e);

			if (e.Executed)
			{
				return true;
			}

			if (e.Handled)
			{
				System.Diagnostics.Debug.WriteLine ("Command '" + commandObject.CommandId + "' (" + commandObject.Name + ") handled; not marked as executed.");
			}
			else
			{
				System.Diagnostics.Debug.WriteLine ("Command '" + commandObject.CommandId + "' (" + commandObject.Name + ") not handled.");
			}
			
			return e.Cancel;
		}

		protected void OnOpletQueueBindingChanged()
		{
			var handler = this.OpletQueueBindingChanged;

			if (handler != null)
            {
				handler (this);
			}
		}

		protected void OnCommandDispatching(CommandEventArgs e)
		{
			var handler = CommandDispatcher.CommandDispatching;

			if (handler != null)
            {
				handler (this, e);
            }
		}


		protected void OnCommandDispatched(CommandEventArgs e)
		{
			var handler = CommandDispatcher.CommandDispatched;

			if (handler != null)
			{
				handler (this, e);
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
		
		public event EventHandler				OpletQueueBindingChanged;
		
		public static event EventHandler<CommandEventArgs>	CommandDispatching;
		public static event EventHandler<CommandEventArgs>	CommandDispatched;
		
		private readonly string					name;
		private readonly CommandDispatcherLevel	level;
		private readonly long					id;
		private CommandDispatcherOptions		options;

		private Dictionary<Command, EventSlot>	eventHandlers = new Dictionary<Command, EventSlot> ();
		
		private Support.OpletQueue				opletQueue;
		
		static object							exclusion = new object ();
		
		static System.Type						commandAttributeType;
		
		static CommandDispatcher				defaultDispatcher;
		static long								nextId;
	}
}
