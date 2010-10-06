//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.PlugIns;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.WorkflowDesigner
{
	[PlugIn ("WorkflowDesigner", "1.0")]
	public class WorkflowDesignerPlugIn : ICorePlugIn
	{
		public WorkflowDesignerPlugIn(PlugInFactory factory)
		{
			this.factory = factory;
			this.application = this.factory.Application;

			this.application.CreatedUI       += sender => System.Diagnostics.Debug.WriteLine ("EVENT: Application finished creating the UI");
			this.application.ShutdownStarted += sender => System.Diagnostics.Debug.WriteLine ("EVENT: Application shutting down");
		}

		#region IDisposable Members

		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}

		#endregion

		protected virtual void Dispose(bool disposing)
		{
		}


		private readonly PlugInFactory factory;
		private readonly CoreApplication application;
	}
}
