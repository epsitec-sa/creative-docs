//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Runtime.Remoting.Lifetime;

namespace Epsitec.Cresus.Services
{
	/// <summary>
	/// La classe Engine démarre les divers services (en mode serveur) ou donne
	/// accès aux services distants via les mécanismes ".NET Remoting".
	/// </summary>
	public class Engine : System.IDisposable
	{
		public Engine(Database.DbInfrastructure infrastructure, int port_number)
		{
			this.infrastructure = infrastructure;
			this.orchestrator   = new Requests.Orchestrator (infrastructure);
			this.port_number    = port_number;
			this.services       = new System.Collections.ArrayList ();
			
			this.LoadServices ();
			this.StartServices ();
		}
		
		
		public Database.DbInfrastructure		Infrastructure
		{
			get
			{
				return this.infrastructure;
			}
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
		
		
		#region IDisposable Members
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (true);
		}
		#endregion
		
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
#if true
			System.Collections.Hashtable setup = new System.Collections.Hashtable ();
			
			setup["port"] = this.port_number;
			
			SoapServerFormatterSinkProvider sink_provider = new SoapServerFormatterSinkProvider ();
			sink_provider.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;
			
			HttpChannel channel = new HttpChannel (setup, null, sink_provider);
#else
			HttpChannel channel = new HttpChannel (this.port_number);
#endif
			ChannelServices.RegisterChannel (channel);
			
			foreach (AbstractServiceEngine service in this.services)
			{
				string int_name = service.ServiceName;
				string pub_name = string.Concat (int_name, "Service", ".soap");
				
				RemotingServices.Marshal (service, pub_name);
			}
		}
		
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.services.Count > 0)
				{
					AbstractServiceEngine[] array = new AbstractServiceEngine[this.services.Count];
					this.services.CopyTo (array);
					
					for (int i = 0; i < array.Length; i++)
					{
						array[i].Dispose ();
					}
					
					this.services.Clear ();
				}
				
				if (this.orchestrator != null)
				{
					this.orchestrator.Dispose ();
					this.orchestrator = null;
				}
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
		
		public static Remoting.IOperatorService GetRemoteOperatorService(string machine, int port)
		{
			string      url  = string.Format (System.Globalization.CultureInfo.InvariantCulture, "http://{0}:{1}/OperatorService.soap", machine, port);
			System.Type type = typeof (Remoting.IRequestExecutionService);
			
			Remoting.IOperatorService service = (Remoting.IOperatorService) System.Activator.GetObject (type, url);
			
			return service;
		}
		
		
		private Database.DbInfrastructure		infrastructure;
		private int								port_number;
		private Requests.Orchestrator			orchestrator;
		private System.Collections.ArrayList	services;
	}
}
