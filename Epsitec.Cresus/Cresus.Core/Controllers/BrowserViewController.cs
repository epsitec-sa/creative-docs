//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers
{
	class BrowserViewController : CoreViewController
	{
		public BrowserViewController(string name)
			: base (name)
		{
		}

		public override IEnumerable<CoreController> GetSubControllers()
		{
			yield break;
		}

		public override void CreateUI(Widget container)
		{
			new StaticText ()
			{
				Parent = container,
				BackColor = Color.FromBrightness (1),
				Dock = DockStyle.Fill,
				Text = "Ici viendra la future <i>liste de gauche</i> Crésus.",
				TextBreakMode = TextBreakMode.Hyphenate,
				ContentAlignment = ContentAlignment.MiddleLeft,
			};
		}
	}
}
