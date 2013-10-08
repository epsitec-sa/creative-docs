//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Views;

namespace Epsitec.Cresus.Assets.App
{
	public class AssetsUI
	{
		public void CreateUI(Widget parent)
		{
			var mainView = new MainView ();
			mainView.CreateUI (parent);
		}
	}
}
