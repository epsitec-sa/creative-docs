//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Remoting;
using Epsitec.Cresus.Services.Extensions;

using System.Collections.Generic;
using System.ServiceModel;
using System.Net.Security;
using System.Linq;

namespace Epsitec.Cresus.Services
{
	/// <summary>
	/// The <c>EngineHost</c> class hosts one or several database engines, which are
	/// published through WCF and implement the dedicated web services. Every service
	/// can run in its own isolated application domain.
	/// </summary>
	public sealed class EngineHost : System.MarshalByRefObject, System.IDisposable
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EngineHost"/> class.
		/// </summary>
		/// <param name="portNumber">The port number on which the services will be registeredm.</param>
		public EngineHost(int portNumber)
		{
			this.hostBinding = new NetTcpBinding (SecurityMode.None, false);
			this.hostBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;
			this.hostBinding.Security.Transport.ProtectionLevel = ProtectionLevel.None;
			
			this.portNumber     = portNumber;
			this.serviceManager = new RemotingServiceManager ();
			this.serviceHosts   = new List<ServiceHostMap> ();

			this.RegisterServiceHost (EngineHost.RemoteServiceManagerName, this.serviceManager, typeof (IRemoteServiceManager));
		}

		
		#region IDisposable Members
		
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (true);
		}
		
		#endregion

		/// <summary>
		/// Adds an engine to the engine host. Every engine should be hosted in its
		/// own app domain to provide isolation and the possibility to unload a given
		/// engine without shutting down the host.
		/// Every service defined by the engine is then wrapped to be callable through
		/// a cross application domain boundary.
		/// </summary>
		/// <param name="engine">The engine.</param>
		public void AddEngine(Engine engine)
		{
			this.serviceManager.AddEngine (engine);

			foreach (var item in engine.GetServiceRecords ())
			{
				System.Type               serviceType     = item.ServiceType;
				System.MarshalByRefObject serviceInstance = item.ServiceInstance;
				string                    serviceName     = item.UniqueName;

				object serviceAdapter = EngineHost.CreateRemotingServiceAdapter (serviceInstance, serviceType);

				System.Diagnostics.Debug.Assert (serviceAdapter != null);
				System.Diagnostics.Debug.Assert (serviceAdapter is Adapters.AbstractServiceAdapter);
				
				this.RegisterServiceHost (serviceName, serviceAdapter, serviceType);
			}
		}

		/// <summary>
		/// Creates the remoting service adapter. Instanciates an <see cref="Adapters.AbstractServiceAdapter"/>
		/// class which will marshal the calls to the real service.
		/// </summary>
		/// <param name="service">The real service.</param>
		/// <param name="serviceType">Type of the service (i.e. interface type).</param>
		/// <returns>The service adapter or <c>null</c>.</returns>
		private static object CreateRemotingServiceAdapter(System.MarshalByRefObject service, System.Type serviceType)
		{
			System.Diagnostics.Debug.Assert (System.Runtime.Remoting.RemotingServices.IsTransparentProxy (service));
			System.Diagnostics.Debug.Assert (serviceType.IsInterface);

			System.Type baseType = typeof (Adapters.AbstractServiceAdapter);

			var adapters = from t in baseType.Assembly.GetTypes ()
						   where t.IsClass && t.IsSubclassOf (baseType) && t.GetInterfaces ().Contains (serviceType)
						   select System.Activator.CreateInstance (t, new object[] { service });

			return adapters.FirstOrDefault ();
		}

		internal static System.Uri GetAddress(string machine, int portNumber, string serviceName)
		{
			System.Uri address;

			System.Uri.TryCreate (EngineHost.GetAddress (machine, portNumber), serviceName, out address);

			return address;
		}

		internal static System.Uri GetAddress(string machine, int portNumber)
		{
			return new System.Uri (string.Format (System.Globalization.CultureInfo.InvariantCulture, EngineHost.RemoteServiceManagerBaseUrl, machine, portNumber));
		}

		void RegisterServiceHost(string serviceName, object instance, System.Type type)
		{
			//	Make sure that the instance we unwrapped is really a service (i.e.
			//	it is tagged with the [ServiceBehavior] attribute).

			System.Diagnostics.Debug.Assert (instance.GetType ().HasServiceBehaviorAttribute ());

			System.Uri  address = EngineHost.GetAddress ("localhost", this.portNumber, serviceName);
			ServiceHost host    = new ServiceHost (instance, address);
 			
			host.AddServiceEndpoint (type, this.hostBinding, address);
			host.Open ();

			this.serviceHosts.Add (new ServiceHostMap ()
				{
					ServiceHost    = host,
					ServiceName    = serviceName,
					ServiceAdapter = instance,
				});
		}

		void Dispose(bool disposing)
		{
			if (disposing)
			{
				foreach (ServiceHostMap item in this.serviceHosts)
				{
					item.ServiceHost.Close ();

					System.IDisposable disposable = item.ServiceAdapter as System.IDisposable;

					if (disposable != null)
					{
						disposable.Dispose ();
					}
				}

				this.serviceHosts.Clear ();
			}
		}

		#region ServiceHostMap Structure

		struct ServiceHostMap
		{
			public ServiceHost						ServiceHost
			{
				get;
				set;
			}
			public string							ServiceName
			{
				get;
				set;
			}
			public object							ServiceAdapter
			{
				get;
				set;
			}
		}

		#endregion

		public static readonly string				RemoteServiceManagerBaseUrl = "net.tcp://{0}:{1}/";
		public static readonly string				RemoteServiceManagerName = "RemoteServiceManager";

		readonly NetTcpBinding						hostBinding;

		readonly int								portNumber;
		readonly RemotingServiceManager				serviceManager;
		readonly List<ServiceHostMap>				serviceHosts;
	}
}
