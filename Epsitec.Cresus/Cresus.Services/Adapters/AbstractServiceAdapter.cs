//	Copyright © 2008-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Remoting;

namespace Epsitec.Cresus.Services.Adapters
{
	/// <summary>
	/// The <c>AbstractServiceAdapter</c> class is the base class which is used by all
	/// service adapters. A service adapter forwards a call across an app domain boundary
	/// to the real service, using .NET remoting. And it can be exposed through WCF.
	/// </summary>
	abstract class AbstractServiceAdapter : System.MarshalByRefObject
	{
		public override object InitializeLifetimeService()
		{
			//	By returning null, we explicitely state that we do not want to have
			//	automatic object recycling (see ILease and remoting).

			return null;
		}
	}

	abstract class AbstractServiceAdapter<T> : AbstractServiceAdapter, IRemoteService where T : IRemoteService
	{
		protected AbstractServiceAdapter(T target)
		{
			this.target = target;
		}


		#region IRemotingService Members

		/// <summary>
		/// Gets the service id which uniquely defines this service.
		/// </summary>
		/// <returns>The unique id for this service.</returns>
		public System.Guid GetServiceId()
		{
			return this.target.GetServiceId ();
		}

		#endregion


		protected readonly T target;
	}
}
