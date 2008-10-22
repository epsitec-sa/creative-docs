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
	public class EngineHost : System.MarshalByRefObject, System.IDisposable
	{
		public EngineHost(int port_number)
		{
			this.port_number    = port_number;
			this.services       = new List<AbstractServiceEngine> ();
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
		
		public void AddService(AbstractServiceEngine engine)
		{
			this.services.Add (engine);
		}
		
		public void RegisterChannel()
		{
			System.Collections.Hashtable setup = new System.Collections.Hashtable ();
			
			setup["port"] = this.port_number;
			
			SoapServerFormatterSinkProvider sink_provider = new SoapServerFormatterSinkProvider ();
			sink_provider.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;
			
			HttpChannel channel = new HttpChannel (setup, null, sink_provider);
			ChannelServices.RegisterChannel (channel);
			this.isChannelRegistered = true;
		}

		public void RegisterService(AbstractServiceEngine service)
		{
			if (this.isChannelRegistered == false)
			{
				this.RegisterChannel ();
			}

			string int_name = service.ServiceName;
			string pub_name = string.Concat (int_name, "Service", ".soap");

			if (System.Runtime.Remoting.RemotingServices.IsTransparentProxy (service))
			{
				Remoting.IConnectionService asConnectionService = service as Remoting.IConnectionService;
				Remoting.IOperatorService asOperatorService = service as Remoting.IOperatorService;
				Remoting.IReplicationService asReplicationService = service as Remoting.IReplicationService;
				Remoting.IRequestExecutionService asRequestExecutionService = service as Remoting.IRequestExecutionService;

				if (asConnectionService != null)
				{
					RemotingServices.Marshal (new Adapters.ConnectionServiceAdapter (asConnectionService), pub_name);
				}
				else if (asOperatorService != null)
				{
					RemotingServices.Marshal (new Adapters.OperatorServiceAdapter (asOperatorService), pub_name);
				}
				else if (asReplicationService != null)
				{
					RemotingServices.Marshal (new Adapters.ReplicationServiceAdapter (asReplicationService), pub_name);
				}
				else if (asRequestExecutionService != null)
				{
					RemotingServices.Marshal (new Adapters.RequestExecutionServiceAdapter (asRequestExecutionService), pub_name);
				}
				else
				{
					throw new System.InvalidOperationException ();
				}
			}
			else
			{
				RemotingServices.Marshal (service, pub_name);
			}
		}
		
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.services.Count > 0)
				{
					AbstractServiceEngine[] array = this.services.ToArray ();
					
					for (int i = 0; i < array.Length; i++)
					{
						array[i].Dispose ();
					}
					
					this.services.Clear ();
				}
			}
		}
		
		private int								port_number;
		private List<AbstractServiceEngine>		services;
		private bool							isChannelRegistered;
	}
}
