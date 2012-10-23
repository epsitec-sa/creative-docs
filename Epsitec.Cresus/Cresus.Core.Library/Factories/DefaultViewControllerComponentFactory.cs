//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Orchestrators;

using System.Reflection;

namespace Epsitec.Cresus.Core.Factories
{
	public abstract class DefaultViewControllerComponentFactory<T> : IViewControllerComponentFactory
			where T : ViewControllerComponent
	{
		#region ICoreComponentFactory<MainWindowController, ViewControllerComponent> Members

		public virtual bool CanCreate(DataViewOrchestrator host)
		{
			return true;
		}

		public virtual bool ShouldCreate(DataViewOrchestrator host)
		{
			return true;
		}

		public virtual ViewControllerComponent Create(DataViewOrchestrator host)
		{
			var args = new object[] { host };
			var binding = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
			return System.Activator.CreateInstance (typeof (T), binding, null, args, null) as ViewControllerComponent;
		}

		System.Type ICoreComponentFactory<DataViewOrchestrator, ViewControllerComponent>.GetComponentType()
		{
			return typeof (T);
		}

		#endregion
	}
}
