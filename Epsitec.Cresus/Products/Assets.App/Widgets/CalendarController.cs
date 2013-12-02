//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	/// <summary>
	/// Contrôleur permettant de choisir une date dans un calendrier. Trois mois sont
	/// affichés (mois précédent, courant et suivant), et des boutons permettent de
	/// reculer ou d'avancer dans le temps.
	/// </summary>
	public class CalendarController
	{
		public System.DateTime Date
		{
			get
			{
				return this.date;
			}
			set
			{
				if (this.date != value)
				{
					this.date = value;
					this.UpdateCalendar ();
				}
			}
		}

		public System.DateTime? SelectedDate
		{
			get
			{
				return this.selectedDate;
			}
			set
			{
				if (this.selectedDate != value)
				{
					this.selectedDate = value;
					this.UpdateCalendar ();
				}
			}
		}


		public void CreateUI(Widget parent)
		{
			var frame = new FrameBox
			{
				Parent = parent,
				Dock   = DockStyle.Fill,
			};

			this.prevButton = new GlyphButton
			{
				Parent          = frame,
				GlyphShape      = GlyphShape.TriangleLeft,
				ButtonStyle     = ButtonStyle.ToolItem,
				PreferredWidth  = CalendarController.ButtonWidth,
				PreferredHeight = Calendar.RequiredHeight,
				Dock            = DockStyle.Left,
			};

			this.prevCalendar = new Calendar
			{
				Parent          = frame,
				PreferredWidth  = Calendar.RequiredWidth,
				PreferredHeight = Calendar.RequiredHeight,
				Dock            = DockStyle.Left,
			};

			this.currCalendar = new Calendar
			{
				Parent          = frame,
				PreferredWidth  = Calendar.RequiredWidth,
				PreferredHeight = Calendar.RequiredHeight,
				Dock            = DockStyle.Left,
				Margins         = new Margins (CalendarController.CalendarMargins, CalendarController.CalendarMargins, 0, 0),
			};

			this.nextCalendar = new Calendar
			{
				Parent          = frame,
				PreferredWidth  = Calendar.RequiredWidth,
				PreferredHeight = Calendar.RequiredHeight,
				Dock            = DockStyle.Left,
			};

			this.nextButton = new GlyphButton
			{
				Parent          = frame,
				GlyphShape      = GlyphShape.TriangleRight,
				ButtonStyle     = ButtonStyle.ToolItem,
				PreferredWidth  = CalendarController.ButtonWidth,
				PreferredHeight = Calendar.RequiredHeight,
				Dock            = DockStyle.Left,
			};

			//	Connexion des événements.
			this.prevButton.Clicked += delegate
			{
				this.date = this.date.AddMonths (-1);
				this.UpdateCalendar ();
			};

			this.nextButton.Clicked += delegate
			{
				this.date = this.date.AddMonths (1);
				this.UpdateCalendar ();
			};

			this.prevCalendar.DateChanged += delegate (object sender, System.DateTime date)
			{
				this.OnDateChanged (date);
			};

			this.currCalendar.DateChanged += delegate (object sender, System.DateTime date)
			{
				this.OnDateChanged (date);
			};

			this.nextCalendar.DateChanged += delegate (object sender, System.DateTime date)
			{
				this.OnDateChanged (date);
			};

			this.UpdateCalendar ();
		}

		private void UpdateCalendar()
		{
			if (this.currCalendar != null)
			{
				this.prevCalendar.Date         = this.date.AddMonths (-1);
				this.prevCalendar.SelectedDate = this.selectedDate;

				this.currCalendar.Date         = this.date;
				this.currCalendar.SelectedDate = this.selectedDate;

				this.nextCalendar.Date         = this.date.AddMonths (1);
				this.nextCalendar.SelectedDate = this.selectedDate;
			}
		}


		#region Events handler
		private void OnDateChanged(System.DateTime dateTime)
		{
			this.DateChanged.Raise (this, dateTime);
		}

		public event EventHandler<System.DateTime> DateChanged;
		#endregion


		public static readonly int RequiredWidth  = Calendar.RequiredWidth*3 + CalendarController.CalendarMargins*2 + CalendarController.ButtonWidth*2;
		public static readonly int RequiredHeight = Calendar.RequiredHeight;
		private const int ButtonWidth             = 20;
		private const int CalendarMargins         = 20;

		private GlyphButton						prevButton;
		private GlyphButton						nextButton;
		private Calendar						prevCalendar;
		private Calendar						currCalendar;
		private Calendar						nextCalendar;

		private System.DateTime					date;
		private System.DateTime?				selectedDate;
	}
}