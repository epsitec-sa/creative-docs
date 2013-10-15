//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Views;

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

			this.CreateDateUI ();

			this.CreateButton (x, y, dx, dy, "Entrée", "Entrée", "Entrée dans l'inventaire, acquisition");
			y -= dy+NewEventPopup.buttonGap;
			this.CreateButton (x, y, dx, dy, "Modification", "Modification", "Modification de diverses informations");
			y -= dy+NewEventPopup.buttonGap;
			this.CreateButton (x, y, dx, dy, "Réorganisation", "Réorganisation", "Modification pour MCH2");
			y -= dy+NewEventPopup.buttonGap;
			this.CreateButton (x, y, dx, dy, "Augmentation", "Revalorisation", "Augmentation de la valeur");
			y -= dy+NewEventPopup.buttonGap;
			this.CreateButton (x, y, dx, dy, "Diminution", "Réévaluation", "Baisse de la valeur");
			y -= dy+NewEventPopup.buttonGap;
			this.CreateButton (x, y, dx, dy, "AmortissementExtra", "Amortissement extraordinaire", "Amortissement manuel");
			y -= dy+NewEventPopup.buttonGap;
			this.CreateButton (x, y, dx, dy, "Sortie", "Sortie", "Sortie de l'inventaire, vente, vol, destruction, etc.");
		}

		private void CreateDateUI()
		{
			var frame = this.CreateTitleFrame (NewEventPopup.titleHeight);

			this.dateController = new DateFieldController
			{
				Label      = "Crée un événement le",
				LabelWidth = 110,
				Value      = this.Date,
			};

			this.dateController.CreateUI (frame);
			this.dateController.SetFocus ();

			this.dateController.ValueChanged += delegate
			{
				this.OnDateChanged (this.dateController.Value);
			};
		}

		private Size GetDialogSize(int buttonCount)
		{
			int dx = NewEventPopup.margins*2 + NewEventPopup.buttonWidth;
			int dy = NewEventPopup.margins*2 + NewEventPopup.titleHeight + NewEventPopup.buttonHeight*buttonCount + NewEventPopup.buttonGap*(buttonCount-1);

			return new Size (dx, dy);
		}


		#region Events handler
		private void OnDateChanged(System.DateTime? dateTime)
		{
			if (this.DateChanged != null)
			{
				this.DateChanged (this, dateTime);
			}
		}

		public delegate void DateChangedEventHandler(object sender, System.DateTime? dateTime);
		public event DateChangedEventHandler DateChanged;
		#endregion


		private static readonly int margins      = 20;
		private static readonly int titleHeight  = 30;
		private static readonly int buttonWidth  = 250;
		private static readonly int buttonHeight = 30;
		private static readonly int buttonGap    = 5;

		private DateFieldController dateController;
	}
}