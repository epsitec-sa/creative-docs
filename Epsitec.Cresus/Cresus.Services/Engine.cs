//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;

namespace Epsitec.Cresus.Services
{
	/// <summary>
	/// Summary description for Engine.
	/// </summary>
	public class Engine
	{
		public Engine(Requests.Orchestrator orchestrator, int port_number)
		{
			this.orchestrator = orchestrator;
			this.port_number  = port_number;
			this.services     = new System.Collections.ArrayList ();
			
			this.LoadServices ();
			this.StartServices ();
		}
		
		
		public Requests.Orchestrator			Orchestrator
		{
			get
			{
				return this.orchestrator;
			}
		}
		
		public System.Collections.Hashtable		Services
		{
			get
			{
				System.Collections.Hashtable hash = new System.Collections.Hashtable ();
				
				foreach (AbstractServiceEngine service in this.services)
				{
					hash[service.ServiceName] = service;
				}
				
				return hash;
			}
		}
		
		
		private void LoadServices()
		{
			System.Reflection.Assembly assembly = Common.Support.AssemblyLoader.Load ("Cresus.Services.Implementation");
			System.Type[] types_in_assembly = assembly.GetTypes ();
			
			foreach (System.Type type in types_in_assembly)
			{
				if (type.IsSubclassOf (typeof (AbstractServiceEngine)))
				{
					AbstractServiceEngine engine = System.Activator.CreateInstance (type, new object[] { this }) as AbstractServiceEngine;
					
					this.services.Add (engine);
				}
			}
		}
		
		private void StartServices()
		{
			HttpChannel channel = new HttpChannel (this.port_number);
			ChannelServices.RegisterChannel (channel);
			
			foreach (AbstractServiceEngine service in this.services)
			{
				string int_name = service.ServiceName;
				string pub_name = string.Concat (int_name, "Service", ".soap");
				
				RemotingServices.Marshal (service, pub_name);
			}
		}
		
		
		public static Remoting.IRequestExecutionService GetRemoteRequestExecutionService(string machine, int port)
		{
			string      url  = string.Format (System.Globalization.CultureInfo.InvariantCulture, "http://{0}:{1}/RequestExecutionService.soap", machine, port);
			System.Type type = typeof (Remoting.IRequestExecutionService);
			
			Remoting.IRequestExecutionService service = (Remoting.IRequestExecutionService) System.Activator.GetObject (type, url);
			
			return service;
		}
		
		public static Remoting.IConnectionService GetRemoteConnectionService(string machine, int port)
		{
			string      url  = string.Format (System.Globalization.CultureInfo.InvariantCulture, "http://{0}:{1}/ConnectionService.soap", machine, port);
			System.Type type = typeof (Remoting.IRequestExecutionService);
			
			Remoting.IConnectionService service = (Remoting.IConnectionService) System.Activator.GetObject (type, url);
			
			return service;
		}
		
		
		private int								port_number;
		private Requests.Orchestrator			orchestrator;
		private System.Collections.ArrayList	services;
	}
}
