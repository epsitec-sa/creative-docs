//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Runtime.Remoting.Lifetime;

using System.Collections.Generic;
using Epsitec.Cresus.Remoting;

namespace Epsitec.Cresus.Services
{
	/// <summary>
	/// La classe Engine démarre les divers services (en mode serveur) ou donne
	/// accès aux services distants via les mécanismes ".NET Remoting".
	/// </summary>
	public class Engine : System.MarshalByRefObject, System.IDisposable
	{
		public Engine(Database.DbInfrastructure infrastructure, System.Guid databaseId)
		{
			this.infrastructure = infrastructure;
			this.databaseId     = databaseId;
			this.databaseName   = infrastructure.Access.Database;
			this.orchestrator   = new Requests.Orchestrator (infrastructure);
			this.services       = new List<AbstractServiceEngine> ();

			this.InstanciateServices ();
		}

		private void InstanciateServices()
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

		public System.Guid DatabaseId
		{
			get
			{
				return this.databaseId;
			}
		}

		public string DatabaseName
		{
			get
			{
				return this.databaseName;
			}
		}
		
		public IEnumerable<System.Guid> GetServiceIds()
		{
			foreach (var service in this.services)
			{
				yield return service.GetServiceId ();
			}
		}

		public IRemoteService GetService(System.Guid serviceId)
		{
			foreach (var service in this.services)
			{
				if (service.GetServiceId () == serviceId)
				{
					return service;
				}
			}

			return null;
		}
		
		
		#region IDisposable Members
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (true);
		}
		#endregion
		
		
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


		public static void ThrowExceptionBasedOnStatus(Remoting.ProgressStatus status)
		{
			switch (status)
			{
				case Remoting.ProgressStatus.None:
					throw new Remoting.Exceptions.InvalidOperationException ();
				case Remoting.ProgressStatus.Running:
					throw new Remoting.Exceptions.PendingException ();
				case Remoting.ProgressStatus.Cancelled:
					throw new Remoting.Exceptions.CancelledException ();
				case Remoting.ProgressStatus.Failed:
					throw new Remoting.Exceptions.FailedException ();

				case Remoting.ProgressStatus.Succeeded:
					break;

				default:
					throw new System.ArgumentOutOfRangeException ("status", status, "Unsupported status value.");
			}
		}
		
		public static Remoting.IRemoteServiceManager GetRemoteServiceManager(string machine, int port)
		{
			string      url  = string.Format (System.Globalization.CultureInfo.InvariantCulture, "http://{0}:{1}/{2}", machine, port, EngineHost.RemoteServiceManagerServiceName);
			System.Type type = typeof (Remoting.IRemoteServiceManager);

			Remoting.IRemoteServiceManager service = (Remoting.IRemoteServiceManager) System.Activator.GetObject (type, url);

			return service;
		}


		readonly Database.DbInfrastructure		infrastructure;
		readonly System.Guid					databaseId;
		readonly string							databaseName;
		readonly List<AbstractServiceEngine>	services;
		Requests.Orchestrator					orchestrator;
	}
}
