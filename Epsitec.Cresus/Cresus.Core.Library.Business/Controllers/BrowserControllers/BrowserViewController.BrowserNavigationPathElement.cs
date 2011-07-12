//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Orchestrators;
using Epsitec.Cresus.Core.Orchestrators.Navigation;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.BrowserControllers
{
	public partial class BrowserViewController
	{
		#region BrowserNavigationPathElement Class

		private sealed class BrowserNavigationPathElement : NavigationPathElement
		{
			public BrowserNavigationPathElement(BrowserViewController controller, EntityKey entityKey)
			{
				this.dataSetName = controller.DataSetName;
				this.entityKey   = entityKey;
			}

			public override bool Navigate(NavigationOrchestrator navigator)
			{
				var browserViewController = navigator.BrowserViewController;

				browserViewController.SelectDataSet (this.dataSetName);
				browserViewController.SetActiveEntityKey (this.entityKey);
				browserViewController.RefreshScrollList ();
				
				return browserViewController.ReselectActiveEntity ();
			}

			public override string ToString()
			{
				return string.Concat ("<Browser:", this.dataSetName, ":", this.entityKey.RowKey.ToString (), ">");
			}


			private readonly string				dataSetName;
			private readonly EntityKey			entityKey;
		}

		#endregion
	}
}