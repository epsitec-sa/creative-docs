//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Remoting;

using System.Collections.Generic;
using System.ServiceModel;

namespace Epsitec.Cresus.Services
{
	[ServiceBehavior (InstanceContextMode=InstanceContextMode.Single, IncludeExceptionDetailInFaults=true)]
	internal sealed class RemotingServiceManager : System.MarshalByRefObject, IRemoteServiceManager, System.IDisposable
	{
		public RemotingServiceManager()
		{
			this.engines = new Dictionary<System.Guid, Engine> ();
		}

		public void AddEngine(Engine engine)
		{
			this.engines.Add (engine.DatabaseId, engine);
			engine.SetAppDomainId (this.engines.Count);
		}

		public override object InitializeLifetimeService()
		{
			//	En retournant null ici, on garantit que le service ne sera jamais
			//	recyclé (sinon, après un temps défini par ILease, l'objet est retiré
			//	de la table des objets joignables par "remoting").

			return null;
		}

		#region IRemoteServiceManager Members

		public KernelDatabaseInfo[] GetDatabaseInfos()
		{
			List<KernelDatabaseInfo> infos = new List<KernelDatabaseInfo> ();

			foreach (var engine in this.engines)
			{
				infos.Add (new KernelDatabaseInfo (engine.Key, engine.Value.DatabaseName));
			}

			return infos.ToArray ();
		}

		public string GetRemoteServiceEndpointAddress(System.Guid databaseId, System.Guid serviceId)
		{
			foreach (var engine in this.engines)
			{
				if (engine.Key == databaseId)
				{
					return engine.Value.GetServiceUniqueName (serviceId);
				}
			}

			return null;
		}

		public void CancelOperation(long operationId)
		{
			foreach (var engine in this.engines)
			{
				AbstractOperation operation = engine.Value.GetOperation (operationId);
				
				if (operation != null)
				{
					operation.CancelOperation ();
					break;
				}
			}
		}

		public void CancelOperationAsync(long operationId, out ProgressInformation cancelProgressInformation)
		{
			foreach (var engine in this.engines)
			{
				AbstractOperation operation = engine.Value.GetOperation (operationId);

				if (operation != null)
				{
					AbstractOperation cancelOp = operation.CancelOperationAsync ();

					if (cancelOp == null)
					{
						cancelProgressInformation = ProgressInformation.Immediate;
					}
					else
					{
						cancelProgressInformation = cancelOp.GetProgressInformation ();
					}

					return;
				}
			}

			cancelProgressInformation = ProgressInformation.Immediate;
		}

		public bool WaitForProgress(long operationId, int minimumProgress, System.TimeSpan timeout)
		{
			foreach (var engine in this.engines)
			{
				AbstractOperation operation = engine.Value.GetOperation (operationId);

				if (operation != null)
				{
					return operation.WaitForProgress (minimumProgress, timeout);
				}
			}

			return true;
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
