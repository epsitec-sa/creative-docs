//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Orchestrators;

namespace Epsitec.Cresus.Core.Library
{
	public abstract class ViewControllerComponent : CoreViewController, ICoreComponent<DataViewOrchestrator, ViewControllerComponent>
	{
		protected ViewControllerComponent(DataViewOrchestrator host, System.Type type, CoreApp app)
			: base (type.Name, app.FindActiveComponent<DataViewOrchestrator> ())
		{
			this.host = host;
		}


		#region ICoreComponent<DataViewOrchestrator, MainWindowComponent> Members

		public DataViewOrchestrator Host
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

		public bool CanExecuteSetupPhase()
		{
			System.Diagnostics.Debug.Assert (this.IsSetupPending);

			return true;
		}

		public void ExecuteSetupPhase()
		{
			this.wasSetupExecuted = true;
		}

		public System.IDisposable GetDisposable()
		{
			return this as System.IDisposable;
		}

		#endregion

		private readonly DataViewOrchestrator host;
		private bool wasSetupExecuted;
	}
}
