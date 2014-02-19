//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class GeneralSettingsView : AbstractSettingsView
	{
		public GeneralSettingsView(DataAccessor accessor, MainToolbar mainToolbar)
			: base (accessor, mainToolbar)
		{
		}


		public override void CreateUI(Widget parent)
		{
			new StaticText
			{
				Parent           = parent,
				Text             = "Ici viendront les réglages généraux...",
				ContentAlignment = ContentAlignment.MiddleCenter,
				Dock             = DockStyle.Fill,
			};
		}
	}
}
