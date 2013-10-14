//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Drawing;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class NewEventPopup : AbstractPopup
	{
		public System.DateTime Date;

		protected override Size DialogSize
		{
			get
			{
				return this.GetDialogSize (7);
			}
		}

		protected override void CreateUI()
		{
			int dx = NewEventPopup.buttonWidth;
			int dy = NewEventPopup.buttonHeight;
			int x = NewEventPopup.margins;
			int y = (int) this.DialogSize.Height - NewEventPopup.margins - NewEventPopup.titleHeight - dy;

			this.CreateTitle (NewEventPopup.titleHeight, this.Title);

			this.CreateButton (x, y, dx, dy, "Entrée", "Entrée");
			y -= dy+NewEventPopup.buttonGap;
			this.CreateButton (x, y, dx, dy, "Modification", "Modification");
			y -= dy+NewEventPopup.buttonGap;
			this.CreateButton (x, y, dx, dy, "Réorganisation", "Réorganisation");
			y -= dy+NewEventPopup.buttonGap;
			this.CreateButton (x, y, dx, dy, "Augmentation", "Revalorisation");
			y -= dy+NewEventPopup.buttonGap;
			this.CreateButton (x, y, dx, dy, "Diminution", "Réévaluation");
			y -= dy+NewEventPopup.buttonGap;
			this.CreateButton (x, y, dx, dy, "AmortissementExtra", "Amortissement extraordinaire");
			y -= dy+NewEventPopup.buttonGap;
			this.CreateButton (x, y, dx, dy, "Sortie", "Sortie");
		}

		private Size GetDialogSize(int buttonCount)
		{
			int dx = NewEventPopup.margins*2 + NewEventPopup.buttonWidth;
			int dy = NewEventPopup.margins*2 + NewEventPopup.titleHeight + NewEventPopup.buttonHeight*buttonCount + NewEventPopup.buttonGap*(buttonCount-1);

			return new Size (dx, dy);
		}

		private string Title
		{
			get
			{
				return "Crée un événement le " + this.Date.ToString ("dd.MM.yyyy");
			}
		}


		private static readonly int margins      = 20;
		private static readonly int titleHeight  = 30;
		private static readonly int buttonWidth  = 180;
		private static readonly int buttonHeight = 30;
		private static readonly int buttonGap    = 5;
	}
}