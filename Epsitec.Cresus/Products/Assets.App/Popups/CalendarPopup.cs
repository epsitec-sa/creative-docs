//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Widgets;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Popup permettant de choisir une date dans un calendrier affichant trois mois.
	/// </summary>
	public class CalendarPopup : AbstractPopup
	{
		private System.DateTime					Date;
		private System.DateTime?				SelectedDate;

		protected override Size					DialogSize
		{
			get
			{
				return new Size (CalendarPopup.dialogWidth, CalendarPopup.dialogHeight);
			}
		}

		protected override void CreateUI()
		{
			this.CreateTitle (Res.Strings.Popup.Calendar.Title.ToString ());

			this.CreateCalendarUI ();
			this.CreateCloseButton ();
		}

		private void CreateCalendarUI()
		{
			this.calendarFrame = this.CreateFrame (CalendarPopup.margins, CalendarPopup.margins, CalendarController.requiredWidth, CalendarController.requiredHeight);

			this.calendarController = new CalendarController ()
			{
				Date         = this.Date,
				SelectedDate = this.SelectedDate,
			};

			this.calendarController.CreateUI (this.calendarFrame);

			this.calendarController.DateChanged += delegate (object sender, System.DateTime date)
			{
				this.Date = date;
				this.OnDateChanged (this.Date);
				this.ClosePopup ();
			};
		}


		#region Events handler
		private void OnDateChanged(System.DateTime dateTime)
		{
			this.DateChanged.Raise (this, dateTime);
		}

		private event EventHandler<System.DateTime> DateChanged;
		#endregion


		#region Helpers
		public static void Show(Widget target, System.DateTime date, System.DateTime? selectedDate, System.Action<System.DateTime> action)
		{
			//	Affiche le Popup pour choisir une date dans un calendrier.
			var popup = new CalendarPopup ()
			{
				Date         = date,
				SelectedDate = selectedDate,
			};

			popup.Create (target, leftOrRight: true);

			popup.DateChanged += delegate (object sender, System.DateTime d)
			{
				action (d);
			};
		}
		#endregion

	
		private const int margins      = 10;
		private const int dialogWidth  = CalendarController.requiredWidth  + CalendarPopup.margins*2;
		private const int dialogHeight = AbstractPopup.titleHeight + CalendarController.requiredHeight + CalendarPopup.margins*2;

		private FrameBox						calendarFrame;
		private CalendarController				calendarController;
	}
}