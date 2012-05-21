//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Core.Library
{
	public interface ICoreComponent<THost, TComponent> : ICoreComponent
		where THost : class, ICoreComponentHost<TComponent>
		where TComponent : class, ICoreComponent
	{
		THost Host
		{
			get;
		}

		bool IsSetupPending
		{
			get;
		}

		bool CanExecuteSetupPhase();

		void ExecuteSetupPhase();

		System.IDisposable GetDisposable();
	}
}
