//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.BrowserControllers
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
			this.browser.DataSetSelected += this.HandleBrowserDataSetSelected;
		}

		public override IEnumerable<CoreController> GetSubControllers()
		{
			yield break;
		}

		public override void CreateUI(Widget container)
		{
			this.CreateUINewItemIconButton (container);
		}

		private void CreateUINewItemIconButton(Widget container)
		{
			this.newItemIconButton = new LayeredIconButton
			{
				Name = "CreateNewItem",
				Parent = container,
				PreferredSize = new Size (28, 28),
				PreferredIconSize = new Size (24, 24),
				Dock = DockStyle.Left,
				IconUri = Misc.GetResourceIconUri ("Edition.NewRecord"),
			};

			this.newItemIconButton.Clicked += this.HandleNewItemClicked;
		}

		
		private void HandleBrowserDataSetSelected(object sender)
		{
			this.UpdateNewItemIconButton ();
		}

		private void HandleNewItemClicked(object sender, MessageEventArgs e)
		{
			this.browser.CreateNewItem ();
		}

		private void UpdateNewItemIconButton()
		{
			string name = "Base." + this.browser.DataSetName;

			this.newItemIconButton.ClearOverlays ();
			this.newItemIconButton.AddOverlay (Misc.GetResourceIconUri (name), new Size (20, 20));
		}

		private readonly BrowserViewController browser;

		private LayeredIconButton newItemIconButton;
	}
}
