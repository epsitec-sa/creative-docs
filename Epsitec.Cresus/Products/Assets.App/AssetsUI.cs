//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.Server.NaiveEngine;

namespace Epsitec.Cresus.Assets.App
{
	public class AssetsUI
	{
		public AssetsUI(DataMandat mandat)
		{
			this.mandat = mandat;
		}

		public void CreateUI(Widget parent)
		{
			var mainView = new MainView (this.mandat);
			mainView.CreateUI (parent);
		}


		private readonly DataMandat mandat;
	}
}
