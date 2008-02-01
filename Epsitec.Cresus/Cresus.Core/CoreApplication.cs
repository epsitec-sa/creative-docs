//	Copyright © 2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Widgets;

using System.Collections.Generic;

namespace Epsitec.Cresus.Core
{
	public class CoreApplication : Application
	{
		public CoreApplication()
		{
		}

		public override string ShortWindowTitle
		{
			get
			{
				return Res.Strings.ProductName;
			}
		}

		public void CreateUserInterface()
		{
			Window window = new Window ();

			window.Text = this.ShortWindowTitle;

			this.Window = window;
		}
	}
}
