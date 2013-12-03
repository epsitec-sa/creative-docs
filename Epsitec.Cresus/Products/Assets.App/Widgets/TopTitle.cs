//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	public class TopTitle : StaticText
	{
		public TopTitle()
		{
			this.Dock             = DockStyle.Top;
			this.PreferredHeight  = TopTitle.height;
			this.ContentAlignment = ContentAlignment.MiddleCenter;
			this.BackColor        = ColorManager.WindowBackgroundColor;
		}


		public void SetTitle(string title)
		{
			if (string.IsNullOrEmpty (title))
			{
				this.ClearTitle ();
			}
			else
			{
				this.Text = title;
				this.TextLayout.DefaultFontSize = 20.0;
			}
		}

		public void ClearTitle()
		{
			this.Text = null;
		}


		public const int height = 30;
	}
}