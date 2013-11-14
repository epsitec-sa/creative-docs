//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class DatePopup : AbstractPopup
	{
		public System.DateTime?					Date;

		protected override Size					DialogSize
		{
			get
			{
				return new Size (DatePopup.dialogWidth, DatePopup.dialogHeight);
			}
		}

		protected override void CreateUI()
		{
			this.CreateTitle (DatePopup.titleHeight, "Choix d'une date");

			var r1 = this.CreateRadio (DatePopup.margins, 70, DatePopup.dialogWidth-DatePopup.margins*2, 20, "final", "Etat final");
			var r2 = this.CreateRadio (DatePopup.margins, 50, DatePopup.dialogWidth-DatePopup.margins*2, 20, "date", "Etat en date du :");

			this.CreateDateUI ();
			this.CreateCloseButton ();

			if (this.Date.HasValue)
			{
				r2.ActiveState = ActiveState.Yes;
			}
			else
			{
				r1.ActiveState = ActiveState.Yes;
				this.dateFrame.Enable = false;
			}

			r1.ActiveStateChanged += delegate
			{
				this.Date = null;
				this.dateFrame.Enable = false;
				this.OnDateChanged (this.Date);
			};

			r2.ActiveStateChanged += delegate
			{
				this.Date = this.dateController.Value;
				this.dateFrame.Enable = true;
				this.OnDateChanged (this.Date);
			};
		}

		private void CreateDateUI()
		{
			this.dateFrame = this.CreateFrame (DatePopup.margins, 20, DatePopup.dialogWidth-DatePopup.margins*2, 2+17+2);
			this.dateFrame.BackColor = ColorManager.WindowBackgroundColor;

			this.dateController = new DateFieldController
			{
				Label      = "Date",
				LabelWidth = 40,
				Value      = this.Date.HasValue ? this.Date : new Timestamp (System.DateTime.Now, 0).Date,
			};

			this.dateController.HideAdditionalButtons = true;
			this.dateController.CreateUI (this.dateFrame);
			this.dateController.SetFocus ();

			this.dateController.ValueEdited += delegate
			{
				this.Date = this.dateController.Value;
				this.OnDateChanged (this.dateController.Value);
			};
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


		private static readonly int titleHeight  = 25;
		private static readonly int margins      = 20;
		private static readonly int dialogWidth  = 200;
		private static readonly int dialogHeight = 130;

		private FrameBox dateFrame;
		private DateFieldController dateController;
	}
}