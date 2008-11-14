//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Runtime.Remoting.Lifetime;

using System.Collections.Generic;
using Epsitec.Cresus.Remoting;
using System.ServiceModel;

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
			this.services       = new List<ServiceRecord> ();

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

					foreach (var interfaceType in type.GetInterfaces ())
					{
						if (interfaceType.GetCustomAttributes (typeof (System.ServiceModel.ServiceContractAttribute), false).Length > 0)
						{
							this.services.Add (new ServiceRecord (engine, interfaceType));
						}
					}
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
				yield return service.ServiceId;
			}
		}

		public IEnumerable<ServiceRecord> GetServiceRecords()
		{
			return this.services;
		}

		public IRemoteService GetService(System.Guid serviceId)
		{
			foreach (var service in this.services)
			{
				if (service.ServiceId == serviceId)
				{
					return service.ServiceInstance as IRemoteService;
				}
			}

			return null;
		}
		
		public string GetServiceUniqueName(System.Guid serviceId)
		{
			foreach (var service in this.services)
			{
				if (service.ServiceId == serviceId)
				{
					return service.UniqueName;
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


		public static void ThrowExceptionBasedOnStatus(ProgressStatus status)
		{
			switch (status)
			{
				case ProgressStatus.None:
					throw new Remoting.Exceptions.InvalidOperationException ();
				case ProgressStatus.Running:
					throw new Remoting.Exceptions.PendingException ();
				case ProgressStatus.Cancelled:
					throw new Remoting.Exceptions.CancelledException ();
				case ProgressStatus.Failed:
					throw new Remoting.Exceptions.FailedException ();

				case ProgressStatus.Succeeded:
					break;

				default:
					throw new System.ArgumentOutOfRangeException ("status", status, "Unsupported status value.");
			}
		}

		
		public static IRemoteServiceManager GetRemoteServiceManager(string machine, int port)
		{
			System.Uri uri = EngineHost.GetAddress (machine, port, EngineHost.RemoteServiceManagerName);
			return Engine.GetService<IRemoteServiceManager> (uri);
		}

		public static T GetService<T>(IRemoteServiceManager remoteServiceManager, string endpointAddress) where T : class
		{
			System.Uri baseUri = Cache<IRemoteServiceManager>.Resolve (remoteServiceManager);
			System.Uri fullUri;

			System.Uri.TryCreate (baseUri, endpointAddress, out fullUri);

			return Engine.GetService<T> (fullUri);
		}
		
		private static T GetService<T>(System.Uri uri) where T : class
		{
			T service = Cache<T>.Resolve (uri);

			if (service == null)
			{
				NetTcpBinding tcpBinding = new NetTcpBinding (SecurityMode.None, false);

				tcpBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;
				tcpBinding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.None;

				EndpointAddress address = new EndpointAddress (uri);
				ChannelFactory<T> factory = new ChannelFactory<T> (tcpBinding, address);
				service = factory.CreateChannel ();
				System.ServiceModel.Channels.IChannel channel = service as System.ServiceModel.Channels.IChannel;
				channel.Open ();

				Cache<T>.Add (uri, service);
			}

			return service;
		}


		[System.Serializable]
		public struct ServiceRecord
		{
			public ServiceRecord(AbstractServiceEngine service, System.Type type)
			{
				this.serviceInstance = service;
				this.serviceType = type;
				this.serviceId = service.GetServiceId ();
				this.uniqueName = string.Format ("{0}-{1}", type.Name, System.Threading.Interlocked.Increment (ref ServiceRecord.id));
			}

			public System.MarshalByRefObject ServiceInstance
			{
				get
				{
					return this.serviceInstance;
				}
			}

			public System.Type ServiceType
			{
				get
				{
					return this.serviceType;
				}
			}

			public System.Guid ServiceId
			{
				get
				{
					return this.serviceId;
				}
			}

			public string UniqueName
			{
				get
				{
					return this.uniqueName;
				}
			}

			static int id;

			private System.MarshalByRefObject serviceInstance;
			private System.Type serviceType;
			private System.Guid serviceId;
			private string uniqueName;
		}
		
		private static class Cache<T> where T : class
		{
			public static System.Uri Resolve(T instance)
			{
				if (Cache<T>.cache == null)
				{
					return null;
				}

				foreach (var item in Cache<T>.cache)
				{
					if (item.Value == instance)
					{
						return new System.Uri (item.Key);
					}
				}

				return null;
			}

			public static T Resolve(System.Uri uri)
			{
				if (Cache<T>.cache == null)
				{
					return null;
				}

				T instance;

				Cache<T>.cache.TryGetValue (uri.OriginalString, out instance);

				return instance;
			}

			public static void Add(System.Uri uri, T instance)
			{
				if (Cache<T>.cache == null)
				{
					Cache<T>.cache = new Dictionary<string, T> ();
				}

				Cache<T>.cache[uri.OriginalString] = instance;
			}

			[System.ThreadStatic]
			static Dictionary<string, T> cache;
		}


		readonly Database.DbInfrastructure		infrastructure;
		readonly System.Guid					databaseId;
		readonly string							databaseName;
		readonly List<ServiceRecord>			services;


		Requests.Orchestrator					orchestrator;
	}
}
