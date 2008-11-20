//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Remoting
{
	/// <summary>
	/// The <c>IRemoteService</c> interface must be implemented by every
	/// remote service which can be reached through the <see cref="IRemoteServiceManager"/>.
	/// </summary>
	public interface IRemoteService
	{
		/// <summary>
		/// Gets the service id which uniquely defines this service.
		/// </summary>
		/// <returns>The unique id for this service.</returns>
		System.Guid GetServiceId();
	}
}
