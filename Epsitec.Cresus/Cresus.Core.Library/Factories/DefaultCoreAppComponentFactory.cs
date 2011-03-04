//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Epsitec.Cresus.Core.Factories
{
	public abstract class DefaultCoreAppComponentFactory<T> : ICoreAppComponentFactory
		where T : CoreAppComponent
	{
		#region ICoreComponentFactory<CoreApp,CoreAppComponent> Members

		public virtual bool CanCreate(CoreApp host)
		{
			return true;
		}

		public virtual CoreAppComponent Create(CoreApp host)
		{
			var args = new object[] { host };
			var binding = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
			return System.Activator.CreateInstance (typeof (T), binding, null, args, null) as CoreAppComponent;
		}

		System.Type ICoreComponentFactory<CoreApp, CoreAppComponent>.GetComponentType()
		{
			return typeof (T);
		}

		#endregion
	}
}
