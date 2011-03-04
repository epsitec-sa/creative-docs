//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Factories
{
	public sealed class DialogCentralFactory : ICoreAppComponentFactory
	{
		#region ICoreComponentFactory<CoreApp,CoreAppComponent> Members

		bool ICoreComponentFactory<CoreApp, CoreAppComponent>.CanCreate(CoreApp host)
		{
			return true;
		}

		CoreAppComponent ICoreComponentFactory<CoreApp, CoreAppComponent>.Create(CoreApp host)
		{
			return new DialogCentral (host);
		}

		System.Type ICoreComponentFactory<CoreApp, CoreAppComponent>.GetComponentType()
		{
			return typeof (DialogCentral);
		}

		#endregion
	}
}
