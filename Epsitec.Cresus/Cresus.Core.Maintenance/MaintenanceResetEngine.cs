//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Business;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Maintenance
{
	public sealed class MaintenanceResetEngine
	{
		public MaintenanceResetEngine()
		{
			this.Reset ();
		}

		private void Reset()
		{
			Epsitec.Cresus.Core.CoreData.ForceDatabaseCreationRequest = true;

			using (var app = new MaintenanceApp ())
			{
				app.CoreData.InitializeDatabase ();
			}

			Epsitec.Cresus.Core.CoreData.ForceDatabaseCreationRequest = false;
		}
	}
}
