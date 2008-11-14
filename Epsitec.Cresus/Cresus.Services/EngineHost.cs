//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Remoting;

using System.Collections.Generic;
using System.ServiceModel;
using System.Net.Security;

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
				object instance = EngineHost.WrapRemotingService (item.ServiceInstance);
				this.RegisterServiceHost (item.UniqueName, instance, item.ServiceType);
			}
		}

		private static object WrapRemotingService(System.MarshalByRefObject service)
		{
			if (System.Runtime.Remoting.RemotingServices.IsTransparentProxy (service))
			{
				Remoting.IConnectionService asConnectionService = service as Remoting.IConnectionService;
				Remoting.IOperatorService asOperatorService = service as Remoting.IOperatorService;
				Remoting.IReplicationService asReplicationService = service as Remoting.IReplicationService;
				Remoting.IRequestExecutionService asRequestExecutionService = service as Remoting.IRequestExecutionService;

				if (asConnectionService != null)
				{
					return new Adapters.ConnectionServiceAdapter (asConnectionService);
				}
				else if (asOperatorService != null)
				{
					return new Adapters.OperatorServiceAdapter (asOperatorService);
				}
				else if (asReplicationService != null)
				{
					return new Adapters.ReplicationServiceAdapter (asReplicationService);
				}
				else if (asRequestExecutionService != null)
				{
					return new Adapters.RequestExecutionServiceAdapter (asRequestExecutionService);
				}
				else
				{
					throw new System.InvalidOperationException ();
				}
			}
			else
			{
				return service;
			}
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
