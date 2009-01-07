//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Remoting;

using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace Epsitec.Cresus.Services
{
	[ServiceBehavior (InstanceContextMode=InstanceContextMode.Single, IncludeExceptionDetailInFaults=true)]
	sealed class RemotingServiceManager : System.MarshalByRefObject, IRemoteServiceManager, System.IDisposable
	{
		internal RemotingServiceManager()
		{
			this.engines = new Dictionary<System.Guid, Engine> ();
		}

		/// <summary>
		/// Adds the engine to the list of known engines. Every engine is
		/// associated with a unique ID which is used to identify the app
		/// domain in which the engine is running.
		/// </summary>
		/// <param name="engine">The engine.</param>
		internal void AddEngine(Engine engine)
		{
			this.engines.Add (engine.DatabaseId, engine);

			int appDomainId = this.engines.Count;
			
			//	The engine gets associated with an app domain ID; this is required
			//	to identify operations on a per engine basis (see OperationManager).
			
			engine.SetAppDomainId (appDomainId);
		}

		/// <summary>
		/// Gets the engines hosted by this remoting service manager. The engines
		/// are sorted alphabetically using the database name.
		/// </summary>
		/// <value>The engines.</value>
		public IEnumerable<Engine> Engines
		{
			get
			{
				var engines = from engine in this.engines.Values
							  orderby engine.DatabaseName, engine.DatabaseId
							  select engine;

				return engines;
			}
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
