//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Runtime.Remoting.Lifetime;

using System.Collections.Generic;

namespace Epsitec.Cresus.Services
{
	/// <summary>
	/// La classe Engine démarre les divers services (en mode serveur) ou donne
	/// accès aux services distants via les mécanismes ".NET Remoting".
	/// </summary>
	public class Engine : System.IDisposable
	{
		public Engine(Database.DbInfrastructure infrastructure, EngineHost engineHost)
		{
			this.infrastructure = infrastructure;
			this.orchestrator   = new Requests.Orchestrator (infrastructure);
			this.engineHost     = engineHost;
			this.services       = new List<AbstractServiceEngine> ();
			
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
		
		public Dictionary<string, AbstractServiceEngine> Services
		{
			get
			{
				Dictionary<string, AbstractServiceEngine> dict = new Dictionary<string, AbstractServiceEngine> ();
				
				foreach (AbstractServiceEngine service in this.services)
				{
					dict[service.ServiceName] = service;
				}
				
				return dict;
			}
		}

		public IEnumerable<string> ServiceNames
		{
			get
			{
				foreach (AbstractServiceEngine service in this.services)
				{
					yield return service.ServiceName;
				}
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
					this.engineHost.AddService (engine);
				}
			}
		}
		
		private void StartServices()
		{
			foreach (AbstractServiceEngine service in this.services)
			{
				this.engineHost.RegisterService (service);
			}
		}
		
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.services.Count > 0)
				{
					//	TODO: unregister services from engine host
					
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
			System.Type type = typeof (Remoting.IConnectionService);
			
			Remoting.IConnectionService service = (Remoting.IConnectionService) System.Activator.GetObject (type, url);
			
			return service;
		}
		
		public static Remoting.IOperatorService GetRemoteOperatorService(string machine, int port)
		{
			string      url  = string.Format (System.Globalization.CultureInfo.InvariantCulture, "http://{0}:{1}/OperatorService.soap", machine, port);
			System.Type type = typeof (Remoting.IOperatorService);
			
			Remoting.IOperatorService service = (Remoting.IOperatorService) System.Activator.GetObject (type, url);
			
			return service;
		}
		
		public static Remoting.IReplicationService GetRemoteReplicationService(string machine, int port)
		{
			string      url  = string.Format (System.Globalization.CultureInfo.InvariantCulture, "http://{0}:{1}/ReplicationService.soap", machine, port);
			System.Type type = typeof (Remoting.IReplicationService);
			
			Remoting.IReplicationService service = (Remoting.IReplicationService) System.Activator.GetObject (type, url);
			
			return service;
		}
		
		
		private Database.DbInfrastructure		infrastructure;
		private EngineHost						engineHost;
		private Requests.Orchestrator			orchestrator;
		private List<AbstractServiceEngine>		services;
	}
}
