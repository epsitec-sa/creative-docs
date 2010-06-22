//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers
{
	/// <summary>
	/// The <c>BrowserSettingsController</c> class manages the settings associated
	/// with a <see cref="BrowserViewController"/>, such as the search criteria.
	/// </summary>
	class BrowserSettingsController : CoreViewController
	{
		public BrowserSettingsController(string name, BrowserViewController browser)
			: base (name)
		{
			this.browser = browser;
			this.browser.CurrentChanged += this.HandleBrowserCurrentChanged;
		}

		public override IEnumerable<CoreController> GetSubControllers()
		{
			yield break;
		}

		public override void CreateUI(Widget container)
		{
			this.CreateUIMainIcon (container);
		}

		private void CreateUIMainIcon(Widget container)
		{
			this.iconBackground = new LayeredIconButton
			{
				Parent = container,
				PreferredSize = new Size (28, 28),
				PreferredIconSize = new Size (24, 24),
				Dock = DockStyle.Left,
				IconUri = Misc.GetResourceIconUri ("Edition.NewRecord"),
			};
		}

		
		private void HandleBrowserCurrentChanged(object sender)
		{
			this.UpdateStaticTextIcon ();
		}

		private void UpdateStaticTextIcon()
		{
			this.iconBackground.ClearOverlays ();
			this.iconBackground.AddOverlay (Misc.GetResourceIconUri ("Base.Customers"), new Size (20, 20));
		}

		private readonly BrowserViewController browser;

		private StaticImage iconOverlay;
		private LayeredIconButton iconBackground;
	}
}
