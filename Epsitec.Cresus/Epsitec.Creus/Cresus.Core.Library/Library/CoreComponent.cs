//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Library
{
	/// <summary>
	/// The <c>CoreComponent</c> generic class is the base class for every component which
	/// implements <see cref="ICoreComponent"/> and which can be attached to an <see cref="ICoreComponentHost"/>.
	/// </summary>
	/// <typeparam name="THost"></typeparam>
	/// <typeparam name="TComponent">The type of the component.</typeparam>
	public abstract class CoreComponent<THost, TComponent> : ICoreComponent<THost, TComponent>
		where THost : class, ICoreComponentHost<TComponent>
		where TComponent : class, ICoreComponent
	{
		protected CoreComponent(THost host)
		{
			this.host= host;
		}

		#region ICoreComponent<THost, TComponent> Implementation

		public THost Host
		{
			get
			{
				return this.host;
			}
		}

		public bool IsSetupPending
		{
			get
			{
				return this.wasSetupExecuted == false;
			}
		}


		public virtual bool CanExecuteSetupPhase()
		{
			System.Diagnostics.Debug.Assert (this.IsSetupPending);

			return true;
		}

		public virtual void ExecuteSetupPhase()
		{
			this.wasSetupExecuted = true;
		}

		public virtual System.IDisposable GetDisposable()
		{
			return this as System.IDisposable;
		}

		#endregion

		private readonly THost host;
		private bool wasSetupExecuted;
	}
}
