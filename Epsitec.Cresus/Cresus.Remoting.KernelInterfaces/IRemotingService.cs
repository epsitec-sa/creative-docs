//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Remoting
{
	public interface IRemotingService
	{
		System.Guid GetServiceId();
		string GetServiceName();
	}
}
