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
	public class CalendarPopup : AbstractPopup
	{
		public System.DateTime					Date;
		public System.DateTime?					SelectedDate;

		protected override Size					DialogSize
		{
			get
			{
				return new Size (CalendarPopup.dialogWidth, CalendarPopup.dialogHeight);
			}
		}

		public override void CreateUI()
		{
			this.CreateTitle ("Calendrier");

			this.CreateCalendarUI ();
			this.CreateCloseButton ();
		}

		private void CreateCalendarUI()
		{
			this.calendarFrame = this.CreateFrame (CalendarPopup.margins, CalendarPopup.margins, CalendarController.RequiredWidth, CalendarController.RequiredHeight);

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

		public event EventHandler<System.DateTime> DateChanged;
		#endregion


		private static readonly int margins      = 10;
		private static readonly int dialogWidth  = CalendarController.RequiredWidth  + CalendarPopup.margins*2;
		private static readonly int dialogHeight = AbstractPopup.TitleHeight + CalendarController.RequiredHeight + CalendarPopup.margins*2;

		private FrameBox						calendarFrame;
		private CalendarController				calendarController;
	}
}