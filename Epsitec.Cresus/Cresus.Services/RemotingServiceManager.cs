//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Remoting;

using System.Collections.Generic;

namespace Epsitec.Cresus.Services
{
	internal sealed class RemotingServiceManager : System.MarshalByRefObject, IRemoteServiceManager, System.IDisposable
	{
		public RemotingServiceManager()
		{
			this.engines = new Dictionary<System.Guid, Engine> ();
		}

		public void AddEngine(Engine engine)
		{
			this.engines.Add (engine.DatabaseId, engine);
		}

		public override object InitializeLifetimeService()
		{
			//	En retournant null ici, on garantit que le service ne sera jamais
			//	recyclé (sinon, après un temps défini par ILease, l'objet est retiré
			//	de la table des objets joignables par "remoting").

			return null;
		}

		#region IKernel Members

		public KernelDatabaseInfo[] GetDatabaseInfos()
		{
			List<KernelDatabaseInfo> infos = new List<KernelDatabaseInfo> ();

			foreach (var engine in this.engines)
			{
				infos.Add (new KernelDatabaseInfo (engine.Key, engine.Value.DatabaseName));
			}

			return new KernelDatabaseInfo[0];
		}

		public IRemoteService GetRemoteService(System.Guid databaseId, System.Guid serviceId)
		{
			foreach (var engine in this.engines)
			{
				if (engine.Key == databaseId)
				{
					return RemotingServiceManager.WrapRemotingService (engine.Value.GetService (serviceId));
				}
			}

			return null;
		}

		private static IRemoteService WrapRemotingService(IRemoteService service)
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

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			foreach (var item in this.engines)
			{
				item.Value.Dispose ();
			}

			this.engines.Clear ();
		}

		#endregion

		readonly Dictionary<System.Guid, Engine> engines;
	}
}
