//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Cresus.Assets
{
	public class AssetsApplication : Application
	{
		public AssetsApplication()
		{
			UI.SetApplication (this);

			Epsitec.Common.Widgets.Adorners.Factory.SetActive ("LookRoyale");
		}
		
		public override string ShortWindowTitle
		{
			get
			{
//				string version = VersionChecker.Format ("#.#.###", GraphUpdate.GetInstalledVersion ());
//				string mode    = GraphSerial.LicensingFriendlyName;
				return string.Format (Res.Strings.ProductName.ToSimpleText () /*, mode, version */);
			}
		}
		
		public override string ApplicationIdentifier
		{
			get
			{
				return "CresusAssetsMainEp";
			}
		}

	}
}
