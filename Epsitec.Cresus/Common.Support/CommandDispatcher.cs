//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier

namespace Epsitec.Common.Support
{
	using Regex = System.Text.RegularExpressions.Regex;
	
	/// <summary>
	/// La classe CommandDispatcher permet de gérer la distribution des
	/// commandes de l'interface graphique vers les routines de traitement.
	/// </summary>
	public class CommandDispatcher
	{
		public CommandDispatcher() : this ("anonymous")
		{
		}
		
		public CommandDispatcher(string name)
		{
			this.dispatcher_name = name;
		}
		
		
		public void Dispatch(string command, object source)
		{
			//	Transmet la commande à ceux qui sont intéressés
			
			CommandEventArgs command_event = new CommandEventArgs (source, command);
			
			string[] command_elements = command.Split ('/');
			int      command_length   = command_elements.Length;
			
			System.Diagnostics.Debug.Assert (command_length == 1);
			System.Diagnostics.Debug.Assert (command.IndexOf ("*") < 0, "Found '*' in command name.", "The command '" + command + "' may not contain a '*' in its name.\nPlease fix the command name definition source code.");
			System.Diagnostics.Debug.Assert (command.IndexOf (".") < 0, "Found '.' in command name.", "The command '" + command + "' may not contain a '.' in its name.\nPlease fix the command name definition source code.");
			
			
			EventSlot slot = this.event_handlers[command] as EventSlot;
			
			if (slot != null)
			{
				slot.Fire (this, command_event);
			}
			else
			{
				System.Diagnostics.Debug.WriteLine ("Command '" + command + "' not handled.");
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
		
		public abstract class CommandState
		{
			public CommandState(string name, CommandDispatcher dispatcher)
			{
				System.Diagnostics.Debug.Assert (name != null);
				System.Diagnostics.Debug.Assert (name.Length > 0);
				System.Diagnostics.Debug.Assert (dispatcher != null);
				
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
			
			
			public abstract void Synchronise();
			
			protected string					name;
			protected Support.CommandDispatcher	dispatcher;
			protected Regex						regex;
		}
		
		public static CommandDispatcher			Default
		{
			get { return CommandDispatcher.default_dispatcher; }
		}
		
		
		protected System.Collections.Hashtable	event_handlers = new System.Collections.Hashtable ();
		protected System.Collections.ArrayList	command_states = new System.Collections.ArrayList ();
		protected string						dispatcher_name;
		
		private static System.Type				command_attr_type  = typeof (CommandAttribute);
		private static CommandDispatcher		default_dispatcher = new CommandDispatcher ("default");
	}
}
