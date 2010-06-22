//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers
{
	class BrowserSettingsController : CoreViewController
	{
		public BrowserSettingsController(string name, BrowserViewController browser)
			: base (name)
		{
			this.browser = browser;
		}

		public override IEnumerable<CoreController> GetSubControllers()
		{
			yield break;
		}

		public override void CreateUI(Common.Widgets.Widget container)
		{
			container.BackColor = Common.Drawing.Color.FromRgb (0.9, 0.9, 1.0);
		}

		private readonly BrowserViewController browser;
	}
}
