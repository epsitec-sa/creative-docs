//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Remoting;

using System.Collections.Generic;

namespace Epsitec.Cresus.Services
{
	public class Kernel : System.MarshalByRefObject, IKernel
	{
		public Kernel(EngineHost engineHost)
		{
			this.engines = new Dictionary<System.Guid, Engine> ();
			this.engineHost = engineHost;
			this.engineHost.RegisterService ("kernel.soap", this);
		}

		public void AddEngine(Engine engine)
		{
			this.engines.Add (engine.DatabaseId, engine);
		}

		#region IKernel Members

		public KernelDatabaseInfo[] GetDatabaseInfos()
		{
			return new KernelDatabaseInfo[0];
		}

		public IRemotingService GetRemotingService(System.Guid databaseId, System.Guid serviceId)
		{
			foreach (var engine in this.engines)
			{
				if (engine.Key == databaseId)
				{
					IRemotingService service = engine.Value.GetService (serviceId);

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
			}

			return null;
		}

		#endregion

		readonly Dictionary<System.Guid, Engine> engines;
		readonly EngineHost engineHost;
	}
}
