//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier

namespace Epsitec.Common.Support
{
	/// <summary>
	/// La classe CommandDispatcher permet de gérer la distribution des
	/// commandes de l'interface graphique vers les routines de traitement.
	/// </summary>
	public class CommandDispatcher
	{
		public CommandDispatcher()
		{
		}
		
		public void Dispatch(string command, object source)
		{
			//	Transmet la commande à ceux qui sont intéressés
			
			System.Diagnostics.Debug.WriteLine ("Command: " + command);
			System.Diagnostics.Debug.WriteLine ("Source:  " + source);
			
			if (this.event_handlers.Contains (command))
			{
				EventSlot slot = this.event_handlers[command] as EventSlot;
				slot.Fire (this, new CommandEventArgs (source, command));
			}
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
						System.Diagnostics.Debug.WriteLine ("Method: " + info.Name + " in " + type.Name + " is a command.");
						this.RegisterMethod (controller, info);
					}
				}
			}
		}
		
		public void RegisterMethod(object controller, System.Reflection.MethodInfo info)
		{
			object[] attributes = info.GetCustomAttributes (CommandDispatcher.command_attr_type, true);
			
			foreach (CommandAttribute attribute in attributes)
			{
				this.RegisterMethod (controller, info, attribute);
			}
		}
		
		public void Register(string command, CommandEventHandler handler)
		{
			EventSlot slot;
			
			if (this.event_handlers.Contains (command))
			{
				slot = this.event_handlers[command] as EventSlot;
			}
			else
			{
				slot = new EventSlot ();
				this.event_handlers[command] = slot;
			}
			
			slot.Command += handler;
		}
		
		public void Unregister(string command, CommandEventHandler handler)
		{
			if (this.event_handlers.Contains (command))
			{
				EventSlot slot = this.event_handlers[command] as EventSlot;
				
				slot.Command -= handler;
			}
		}
		
		
		
		protected void RegisterMethod(object controller, System.Reflection.MethodInfo info, CommandAttribute attribute)
		{
			System.Reflection.ParameterInfo[] param_info = info.GetParameters ();
			
			CommandEventHandler handler = null;
			EventRelay          relay   = new EventRelay (controller, info);
			
			switch (param_info.Length)
			{
				case 0:
					//	Cas simple: c'est une méthode sans aucun argument.
					
					handler = new CommandEventHandler (relay.InvokeWithoutArgument);
					break;
				
				case 1:
					if (param_info[0].ParameterType == typeof (CommandDispatcher))
					{
						handler = new CommandEventHandler (relay.InvokeWithCommandDispatcher);
					}
					break;
				
				case 2:
					if ((param_info[0].ParameterType == typeof (CommandDispatcher)) &&
						(param_info[1].ParameterType == typeof (CommandEventArgs)))
					{
						handler = new CommandEventHandler (relay.InvokeWithCommandDispatcherAndEventArgs);
					}
					break;
			}
			
			if (handler == null)
			{
				throw new System.FormatException (string.Format ("{0}.{1} uses invalid signature: {2}.", controller.GetType ().Name, info.Name, info.ToString ()));
			}
			
			this.Register (attribute.CommandName, handler);
			
		}
		
		protected class EventRelay
		{
			public EventRelay(object controller, System.Reflection.MethodInfo method_info)
			{
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
		
		protected class EventSlot
		{
			public void Fire(CommandDispatcher sender, CommandEventArgs e)
			{
				if (this.Command != null)
				{
					this.Command (sender, e);
				}
			}
			
			public event CommandEventHandler	Command;
		}
		
		
		public static CommandDispatcher			Default
		{
			get { return CommandDispatcher.default_dispatcher; }
		}
		
		
		protected System.Collections.Hashtable	event_handlers = new System.Collections.Hashtable ();
		
		private static System.Type				command_attr_type  = typeof (CommandAttribute);
		private static CommandDispatcher		default_dispatcher = new CommandDispatcher ();
	}
}
