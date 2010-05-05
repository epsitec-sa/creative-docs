//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Core.Controllers
{
	public interface ICoreViewControllerHost
	{
		void NotifyDisposing(CoreViewController controller);
	}
}
