//	Copyright © 2010-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

using Epsitec.Cresus.Core.Controllers.BrowserControllers;
using Epsitec.Cresus.Core.Metadata;
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
				this.dataSetCommandId = controller.DataSetMetadata.Command.Caption.Id;
				this.entityKey = entityKey;
			}

			private BrowserNavigationPathElement (Druid dataSetCaptionId, string entityKey)
			{
				this.dataSetCommandId = dataSetCaptionId;
				this.entityKey   = EntityKey.Parse (entityKey).Value;
			}

			private BrowserNavigationPathElement()
			{
			}

			public override bool Navigate(NavigationOrchestrator navigator)
			{
				var browserViewController = navigator.BrowserViewController;
				var dataSetMetadata = DataStoreMetadata.Current.FindDataSet (this.dataSetCommandId);

				browserViewController.SelectDataSet (dataSetMetadata);
				browserViewController.SelectEntity (this.entityKey);
				browserViewController.SelectActiveEntity ();

				return true;
			}

			protected override string Serialize()
			{
				var prefix = BrowserNavigationPathElement.ClassIdPrefix;
				var id = this.dataSetCommandId.ToCompactString ();
				var key = this.entityKey.ToString ();

				return string.Concat (prefix, id, ".", key);
			}

			protected override NavigationPathElement Deserialize(string data)
			{
				if (data.StartsWith (BrowserNavigationPathElement.ClassIdPrefix))
				{
					var text = data.Substring (BrowserNavigationPathElement.ClassIdPrefix.Length);
					int pos  = text.IndexOf ('.');

					if (pos > 0)
					{
						var commandId = Druid.Parse (text.Substring (0, pos));
						var entityKey = text.Substring (pos+1);
						return new BrowserNavigationPathElement (commandId, entityKey);
					}
				}

				throw new System.FormatException (string.Format ("Invalid format; expected '{0}<DataSet>.<EntityKey>', got '{1}'", BrowserNavigationPathElement.ClassIdPrefix, data));
			}

			private const string				ClassIdPrefix = "Browser:";

			private readonly Druid				dataSetCommandId;
			private readonly EntityKey			entityKey;
		}

		#endregion
	}
}