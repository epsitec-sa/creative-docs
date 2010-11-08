﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.BrowserControllers
{
	public partial class BrowserViewController
	{
		#region BrowserNavigationPathElement Class

		private class BrowserNavigationPathElement : Epsitec.Cresus.Core.Orchestrators.Navigation.NavigationPathElement
		{
			public BrowserNavigationPathElement(BrowserViewController controller, EntityKey entityKey)
			{
				this.dataSetName = controller.DataSetName;
				this.entityKey   = entityKey;
			}

			public override bool Navigate(Orchestrators.NavigationOrchestrator navigator)
			{
				var browserViewController = navigator.BrowserViewController;

				browserViewController.SelectDataSet (this.dataSetName);
				browserViewController.SetActiveEntityKey (this.entityKey);
				browserViewController.RefreshScrollList ();
				
				//	Make sure we re-select the active entity by clearing it first:
				browserViewController.Orchestrator.ClearActiveEntity ();

				return browserViewController.SelectActiveEntity ();
			}

			public override string ToString()
			{
				return string.Concat ("<Browser:", this.dataSetName, ":", this.entityKey.RowKey.ToString (), ">");
			}


			private readonly string dataSetName;
			private readonly EntityKey entityKey;
		}

		#endregion
	}
}