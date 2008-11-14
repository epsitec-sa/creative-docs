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
	public class EngineHost : System.MarshalByRefObject, System.IDisposable
	{
		public EngineHost(int portNumber)
		{
			this.hostBinding = new NetTcpBinding (SecurityMode.None, false);
			this.hostBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;
			this.hostBinding.Security.Transport.ProtectionLevel = ProtectionLevel.None;
			
			this.portNumber = portNumber;
			this.serviceManager = new RemotingServiceManager ();
			this.serviceHosts = new List<ServiceHostMap> ();

			this.RegisterServiceHost<IRemoteServiceManager> (EngineHost.RemoteServiceManagerName, this.serviceManager);
		}

		
		#region IDisposable Members
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (true);
		}
		#endregion

		public void AddEngine(Engine engine)
		{
			this.serviceManager.AddEngine (engine);

			foreach (var item in engine.GetServiceRecords ())
			{
				object instance = EngineHost.WrapRemotingService (item.ServiceInstance, item.ServiceType);

				System.Diagnostics.Debug.Assert (instance.GetType ().HasServiceBehaviorAttribute ());
				
				this.RegisterServiceHost (item.UniqueName, instance, item.ServiceType);
			}
		}

		private static object WrapRemotingService(System.MarshalByRefObject service, System.Type serviceType)
		{
			System.Diagnostics.Debug.Assert (System.Runtime.Remoting.RemotingServices.IsTransparentProxy (service));
			System.Diagnostics.Debug.Assert (serviceType.IsInterface);

			System.Type baseType = typeof (Adapters.AbstractServiceAdapter);

			var wrapper = from t in baseType.Assembly.GetTypes ()
						  where t.IsSubclassOf (baseType) && t.GetInterfaces ().Contains (serviceType)
						  select System.Activator.CreateInstance (t, new object[] { service });

			return wrapper.FirstOrDefault ();
		}

		public static System.Uri GetAddress(string machine, int portNumber, string serviceName)
		{
			System.Uri address;

			System.Uri.TryCreate (EngineHost.GetAddress (machine, portNumber), serviceName, out address);

			return address;
		}

		public static System.Uri GetAddress(string machine, int portNumber)
		{
			return new System.Uri (string.Format (System.Globalization.CultureInfo.InvariantCulture, EngineHost.RemoteServiceManagerBaseUrl, machine, portNumber));
		}

		private void RegisterServiceHost<T>(string serviceName, T instance) where T : class
		{
			this.RegisterServiceHost (serviceName, instance, typeof (T));
		}

		private void RegisterServiceHost(string serviceName, object instance, System.Type type)
		{
			System.Uri address = this.GetLocalAddress (serviceName);
			ServiceHost host = new ServiceHost (instance, address);
 			host.AddServiceEndpoint (type, this.hostBinding, address);
			host.Open ();

			this.serviceHosts.Add (new ServiceHostMap ()
				{
					ServiceHost = host,
					ServiceName = serviceName,
					Instance = instance,
				});
		}

		private System.Uri GetLocalAddress(string serviceName)
		{
			return EngineHost.GetAddress ("localhost", this.portNumber, serviceName);
		}
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				foreach (ServiceHostMap item in this.serviceHosts)
				{
					item.ServiceHost.Close ();

					System.IDisposable disposable = item.Instance as System.IDisposable;

					if (disposable != null)
					{
						disposable.Dispose ();
					}
				}

				this.serviceHosts.Clear ();
			}
		}

		private struct ServiceHostMap
		{
			public ServiceHost ServiceHost
			{
				get;
				set;
			}
			public string ServiceName
			{
				get;
				set;
			}
			public object Instance
			{
				get;
				set;
			}
		}

		public static readonly string				RemoteServiceManagerBaseUrl = "net.tcp://{0}:{1}/";
		public static readonly string				RemoteServiceManagerName = "RemoteServiceManager";

		readonly NetTcpBinding						hostBinding;

		readonly int								portNumber;
		readonly RemotingServiceManager				serviceManager;
		readonly List<ServiceHostMap>				serviceHosts;
	}
}
