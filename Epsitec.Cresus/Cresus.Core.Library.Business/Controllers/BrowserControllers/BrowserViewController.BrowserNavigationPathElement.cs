//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Controllers.BrowserControllers;

using Epsitec.Cresus.Core.Orchestrators;
using Epsitec.Cresus.Core.Orchestrators.Navigation;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

[assembly: NavigationPathElementClass ("Browser", typeof (BrowserViewController.BrowserNavigationPathElement))]

namespace Epsitec.Cresus.Core.Controllers.BrowserControllers
{
	public partial class BrowserViewController
	{
		#region BrowserNavigationPathElement Class

		internal sealed class BrowserNavigationPathElement : NavigationPathElement
		{
			public BrowserNavigationPathElement(BrowserViewController controller, EntityKey entityKey)
			{
				this.dataSetName = controller.DataSetName;
				this.entityKey   = entityKey;
			}

			private BrowserNavigationPathElement (string dataSetName, string entityKey)
			{
				this.dataSetName = dataSetName;
				this.entityKey   = EntityKey.Parse (entityKey).Value;
			}

			private BrowserNavigationPathElement()
			{
			}

			public override bool Navigate(NavigationOrchestrator navigator)
			{
				var browserViewController = navigator.BrowserViewController;

				browserViewController.SelectDataSet (this.dataSetName);
				browserViewController.SetActiveEntityKey (this.entityKey);
				browserViewController.RefreshScrollList ();
				
				return browserViewController.ReselectActiveEntity ();
			}

			protected override string Serialize()
			{
				return string.Concat (BrowserNavigationPathElement.ClassIdPrefix, this.dataSetName, ".", this.entityKey.ToString ());
			}

			protected override NavigationPathElement Deserialize(string data)
			{
				if (data.StartsWith (BrowserNavigationPathElement.ClassIdPrefix))
				{
					var text = data.Substring (BrowserNavigationPathElement.ClassIdPrefix.Length);
					int pos  = text.IndexOf ('.');

					if (pos > 0)
					{
						return new BrowserNavigationPathElement (text.Substring (0, pos), text.Substring (pos+1));
					}
				}
				
				throw new System.FormatException (string.Format ("Invalid format; expected '{0}DataSet.EntityKey', got '{1}'", BrowserNavigationPathElement.ClassIdPrefix, data));
			}

			private const string				ClassIdPrefix = "Browser:";

			private readonly string				dataSetName;
			private readonly EntityKey			entityKey;
		}

		#endregion
	}
}