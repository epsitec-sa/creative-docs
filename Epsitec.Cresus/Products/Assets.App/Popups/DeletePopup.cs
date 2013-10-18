//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Drawing;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class DeletePopup : AbstractPopup
	{
		public string							Question;

		protected override Size					DialogSize
		{
			get
			{
				return new Size (DeletePopup.dialogWidth, DeletePopup.dialogHeight);
			}
		}

		protected override void CreateUI()
		{
			this.CreateTitle (DeletePopup.titleHeight, this.Question);

			int y = (int) DeletePopup.margins;
			int dx = (DeletePopup.dialogWidth - DeletePopup.margins*2 - DeletePopup.buttonGap) /2;
			int dy = 24;

			this.CreateButton (DeletePopup.margins,                          y, dx, dy, "yes", "Oui");
			this.CreateButton (DeletePopup.margins+dx+DeletePopup.buttonGap, y, dx, dy, "no",  "Non");
		}


		private static readonly int margins      = 20;
		private static readonly int titleHeight  = 30;
		private static readonly int dialogWidth  = 260;
		private static readonly int dialogHeight = 100;
		private static readonly int buttonGap    = 10;
	}
}