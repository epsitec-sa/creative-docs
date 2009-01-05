//	Copyright � 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Remoting;
using Epsitec.Cresus.Services.Extensions;

using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Runtime.Remoting.Lifetime;

using System.Collections.Generic;
using System.ServiceModel;
using System.Linq;

namespace Epsitec.Cresus.Services
{
	/// <summary>
	/// La classe Engine d�marre les divers services (en mode serveur) ou donne
	/// acc�s aux services distants via les m�canismes ".NET Remoting".
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

			this.CreateServices ();
		}

		/// <summary>
		/// Creates all the services defined in the implementation assemblies;
		/// this will instanciate the services and populate the services list
		/// with the corresponding service records.
		/// </summary>
		private void CreateServices()
		{
			System.Reflection.Assembly assembly = Common.Support.AssemblyLoader.Load ("Cresus.Services.Implementation");

			var records = from type in assembly.GetTypes ()
						  where type.IsSubclassOf (typeof (AbstractServiceEngine))
						  let engine = System.Activator.CreateInstance (type, new object[] { this }) as AbstractServiceEngine
						  from interfaceType in type.GetInterfaces ()
						  where interfaceType.HasServiceContractAttribute ()
						  select new ServiceRecord (engine, interfaceType);

			this.services.AddRange (records);
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

		public System.Guid						DatabaseId
		{
			get
			{
				return this.databaseId;
			}
		}

		public string							DatabaseName
		{
			get
			{
				return this.databaseName;
			}
		}


		/// <summary>
		/// Gets the IDs for the running services.
		/// </summary>
		/// <returns>The IDs for the running services.</returns>
		public IEnumerable<System.Guid> GetServiceIds()
		{
			return from service in this.services
				   select service.ServiceId;
		}

		/// <summary>
		/// Gets the records for the running services.
		/// </summary>
		/// <returns>The records for the running services.</returns>
		public IEnumerable<ServiceRecord> GetServiceRecords()
		{
			return this.services;
		}

		/// <summary>
		/// Gets the service identified by its service ID.
		/// </summary>
		/// <param name="serviceId">The service ID.</param>
		/// <returns>The service instance, if any.</returns>
		public IRemoteService GetService(System.Guid serviceId)
		{
			var result = from service in this.services
						 where service.ServiceId == serviceId
						 select service.ServiceInstance as IRemoteService;

			return result.FirstOrDefault ();
		}

		/// <summary>
		/// Gets the unique name of the service identified by its service ID.
		/// </summary>
		/// <param name="serviceId">The service ID.</param>
		/// <returns>The unique name, if any.</returns>
		public string GetServiceUniqueName(System.Guid serviceId)
		{
			var result = from service in this.services
						 where service.ServiceId == serviceId
						 select service.UniqueName;

			return result.FirstOrDefault ();
		}

		
		internal AbstractOperation GetOperation(long operationId)
		{
			return OperationManager.Resolve<AbstractOperation> (operationId);
		}

		internal void SetAppDomainId(int id)
		{
			if (this.appDomainId != 0)
			{
				throw new System.InvalidOperationException ("AppDomain ID already assigned");
			}
			
			this.appDomainId = id;

			OperationManager.SetAppDomainId (id);
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
				}
			}
		}

		
		public static void ThrowExceptionBasedOnStatus(ProgressState status)
		{
			switch (status)
			{
				case ProgressState.None:
					throw new Remoting.Exceptions.InvalidOperationException ();
				case ProgressState.Running:
					throw new Remoting.Exceptions.PendingException ();
				case ProgressState.Cancelled:
					throw new Remoting.Exceptions.CancelledException ();
				case ProgressState.Failed:
					throw new Remoting.Exceptions.FailedException ();

				case ProgressState.Succeeded:
					break;

				default:
					throw new System.ArgumentOutOfRangeException ("status", status, "Unsupported status value.");
			}
		}

		
		internal static T GetService<T>(IRemoteServiceManager remoteServiceManager, string endpointAddress) where T : class
		{
			System.Uri baseUri = UriCache<IRemoteServiceManager>.Resolve (remoteServiceManager);
			System.Uri fullUri;

			System.Uri.TryCreate (baseUri, endpointAddress, out fullUri);

			return Engine.GetService<T> (fullUri);
		}
		
		internal static T GetService<T>(System.Uri uri) where T : class
		{
			T service = UriCache<T>.Resolve (uri);

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

				UriCache<T>.Add (uri, service);
			}

			return service;
		}


		readonly Database.DbInfrastructure		infrastructure;
		readonly System.Guid					databaseId;
		readonly string							databaseName;
		readonly List<ServiceRecord>			services;
		readonly Requests.Orchestrator			orchestrator;

		int appDomainId;
	}
}
