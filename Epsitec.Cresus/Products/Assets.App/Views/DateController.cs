﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.Helpers;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	/// <summary>
	/// Contrôleur permettant de saisir une date avec plusieurs boutons pour faciliter
	/// la saisie, y compris un accès aux dates prédéfinies (début ou fin de l'année en
	/// cours par exemple) et au calendrier. Pour cela, on utilise les composants
	/// SimplePopup et CalendarPopup, affichés en tant que sous-popups.
	/// </summary>
	public class DateController
	{
		public DateController(DataAccessor accessor)
		{
			this.accessor = accessor;

			this.dateFieldController = new DateFieldController
			{
				Label      = null,
				LabelWidth = 0,
			};

			this.DateDescription = "Date";
			this.DateLabelWidth  = DateController.indent;
		}


		public string							DateDescription;
		public int								DateLabelWidth;
		public int								TabIndex;

		public System.DateTime?					Date
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
					this.UpdateDate ();
					this.UpdateButtons ();
				}
			}
		}


		public void CreateUI(Widget parent)
		{
			this.CreateToolbar (parent);
			this.CreateController (parent);

			this.UpdateButtons ();
		}


		private void CreateToolbar(Widget parent)
		{
			//	Crée la ligne des boutons [J] [M] [A] permettant de sélectionner
			//	la partie correspondante dans le textede la date.
			//	Les boutons sont positionnés horizontalement de façon à s'aligner
			//	au mieux sur le texte de la date dans le TextField.
			var line = new FrameBox
			{
				Parent          = parent,
				Dock            = DockStyle.Top,
				PreferredHeight = DateController.lineHeight,
				Margins         = new Margins (this.DateLabelWidth+(this.DateLabelWidth == 0 ? 0 : 10), 0, 0, 0),
			};

			this.dayButton   = this.CreatePartButton (line,  5, 13);
			this.monthButton = this.CreatePartButton (line, 19, 13);
			this.yearButton  = this.CreatePartButton (line, 33, 23);

			ToolTip.Default.SetToolTip (this.dayButton,   "Sélectionne le jour");
			ToolTip.Default.SetToolTip (this.monthButton, "Sélectionne le mois");
			ToolTip.Default.SetToolTip (this.yearButton,  "Sélectionne l'année");

			this.info = new StaticText
			{
				Parent           = line,
				ContentAlignment = ContentAlignment.MiddleLeft,
				TextBreakMode    = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
				Enable           = false,  // pour afficher en gris
				Dock             = DockStyle.Fill,
				Margins          = new Margins (72, 0, 0, 0),
			};

			this.dayButton.Clicked += delegate
			{
				this.SelectText (0, 2);  // sélectionne [31].03.2013
			};

			this.monthButton.Clicked += delegate
			{
				this.SelectText (3, 2);  // sélectionne 31.[03].2013
			};

			this.yearButton.Clicked += delegate
			{
				this.SelectText (6, 4);  // sélectionne 31.03.[2013]
			};
		}

		private IconButton CreatePartButton(Widget parent, int x, int dx)
		{
			return new IconButton
			{
				Parent          = parent,
				AutoFocus       = false,
				PreferredWidth  = dx,
				PreferredHeight = DateController.lineHeight,
				Anchor          = AnchorStyles.BottomLeft,
				Margins         = new Margins (x, 0, 0, 0),
			};
		}

		private IconButton CreateIconButton(Widget parent, string icon, int x)
		{
			return new IconButton
			{
				Parent          = parent,
				IconUri         = Misc.GetResourceIconUri (icon),
				AutoFocus       = false,
				PreferredWidth  = DateController.lineHeight,
				PreferredHeight = DateController.lineHeight,
				Anchor          = AnchorStyles.BottomLeft,
				Margins         = new Margins (x, 0, 0, 2),
			};
		}

		public void CreateController(Widget parent)
		{
			var footer = new FrameBox
			{
				Parent          = parent,
				Dock            = DockStyle.Top,
				PreferredHeight = DateController.lineHeight,
			};

			if (this.DateLabelWidth > 0)
			{
				new StaticText
				{
					Parent           = footer,
					Text             = this.DateDescription,
					ContentAlignment = ContentAlignment.MiddleRight,
					PreferredWidth   = this.DateLabelWidth,
					Dock             = DockStyle.Left,
					Margins          = new Margins (0, 10, 0, 0),
				};
			}

			var dateFrame = new FrameBox
			{
				Parent    = footer,
				Dock      = DockStyle.Fill,
				BackColor = ColorManager.WindowBackgroundColor,
			};

			this.dateFieldController.HideAdditionalButtons = true;
			this.dateFieldController.TabIndex = this.TabIndex;
			this.dateFieldController.CreateUI (dateFrame);
			this.dateFieldController.SetFocus ();

			this.beginButton      = this.CreateIconButton (dateFrame, "Date.CurrentYearBegin", (int) (72+DateController.lineHeight*2.5));
			this.nowButton        = this.CreateIconButton (dateFrame, "Date.Now",              (int) (72+DateController.lineHeight*3.5));
			this.endButton        = this.CreateIconButton (dateFrame, "Date.CurrentYearEnd",   (int) (72+DateController.lineHeight*4.5));
			this.predefinedButton = this.CreateIconButton (dateFrame, "Date.Predefined",       (int) (72+DateController.lineHeight*6.0));
			this.calendarButton   = this.CreateIconButton (dateFrame, "Date.Calendar",         (int) (72+DateController.lineHeight*7.0));
			this.deleteButton     = this.CreateIconButton (dateFrame, "Field.Clear",           (int) (72+DateController.lineHeight*8.5));

			ToolTip.Default.SetToolTip (this.beginButton,      "Début de l'année en cours");
			ToolTip.Default.SetToolTip (this.nowButton,        "Aujourd'hui");
			ToolTip.Default.SetToolTip (this.endButton,        "Fin de l'année en cours");
			ToolTip.Default.SetToolTip (this.predefinedButton, "Autre date à choix...");
			ToolTip.Default.SetToolTip (this.calendarButton,   "Calendrier...");
			ToolTip.Default.SetToolTip (this.deleteButton,     "Efface la date");

			this.beginButton.Clicked += delegate
			{
				this.Date = this.GetPredefinedDate (DateType.BeginCurrentYear);
				this.UpdateButtons ();
				this.dateFieldController.SetFocus ();
				this.OnDateChanged (this.date);
			};

			this.nowButton.Clicked += delegate
			{
				this.Date = this.GetPredefinedDate (DateType.Now);
				this.UpdateButtons ();
				this.dateFieldController.SetFocus ();
				this.OnDateChanged (this.date);
			};

			this.endButton.Clicked += delegate
			{
				this.Date = this.GetPredefinedDate (DateType.EndCurrentYear);
				this.UpdateButtons ();
				this.dateFieldController.SetFocus ();
				this.OnDateChanged (this.date);
			};

			this.predefinedButton.Clicked += delegate
			{
				this.ShowPredefinedPopup ();
			};

			this.dateFieldController.ValueEdited += delegate
			{
				this.Date = this.dateFieldController.Value;
				this.OnDateChanged (this.date);
			};

			this.dateFieldController.CursorChanged += delegate
			{
				this.UpdateButtons ();
			};

			this.deleteButton.Clicked += delegate
			{
				this.Date = null;
				this.UpdateButtons ();
				this.dateFieldController.SetFocus ();
				this.OnDateChanged (this.date);
			};

			this.calendarButton.Clicked += delegate
			{
				this.ShowCalendarPopup ();
			};
		}


		private void UpdateDate()
		{
			this.dateFieldController.Value = this.date;
		}

		private void UpdateButtons()
		{
			if (this.beginButton != null)
			{
				var part = this.dateFieldController.SelectedPart;

				this.dayButton.Enable   = this.Date != null;
				this.monthButton.Enable = this.Date != null;
				this.yearButton.Enable  = this.Date != null;

				this.UpdateButtonPart (this.dayButton,   part == DateFieldController.Part.Day);
				this.UpdateButtonPart (this.monthButton, part == DateFieldController.Part.Month);
				this.UpdateButtonPart (this.yearButton,  part == DateFieldController.Part.Year);

				this.UpdateButtonState (this.beginButton, this.Date == this.GetPredefinedDate (DateType.BeginCurrentYear));
				this.UpdateButtonState (this.nowButton,   this.Date == this.GetPredefinedDate (DateType.Now));
				this.UpdateButtonState (this.endButton,   this.Date == this.GetPredefinedDate (DateType.EndCurrentYear));

				this.deleteButton.Enable = this.Date != null;

				this.info.Text = this.date.ToFull ();
			}
		}

		private void UpdateButtonPart(IconButton button, bool activate)
		{
			if (activate)
			{
				button.IconUri = Misc.GetResourceIconUri ("Date.SelectedPart");
			}
			else
			{
				button.IconUri = Misc.GetResourceIconUri ("Date.UnselectedPart");
			}
		}

		private void UpdateButtonState(IconButton button, bool activate)
		{
			if (activate)
			{
				button.ButtonStyle = ButtonStyle.ActivableIcon;
				button.ActiveState = ActiveState.Yes;
			}
			else
			{
				button.ButtonStyle = ButtonStyle.ToolItem;
				button.ActiveState = ActiveState.No;
			}
		}


		private void SelectText(int start, int count)
		{
			this.dateFieldController.TextField.Focus ();
			this.dateFieldController.TextField.CursorFrom = start;
			this.dateFieldController.TextField.CursorTo   = start + count;
		}

		private void ShowPredefinedPopup()
		{
			var popup = new SimplePopup ()
			{
				SelectedItem = this.GetSelectedDate (this.date),
			};

			foreach (var type in this.DateTypes)
			{
				var text = this.GetPredefinedDescription (type);

				if (string.IsNullOrEmpty (text))
				{
					popup.Items.Add (null);
				}
				else
				{
					var date = this.GetPredefinedDate (type);
					var td = TypeConverters.DateToString (date);

					popup.Items.Add (string.Concat (td, " — ", text));
				}
			}

			popup.Create (this.predefinedButton, leftOrRight: true);

			popup.ItemClicked += delegate (object sender, int rank)
			{
				this.Date = this.GetPredefinedDate (rank);
				this.UpdateButtons ();
				this.dateFieldController.SetFocus ();
				this.OnDateChanged (this.date);
			};
		}

		private void ShowCalendarPopup()
		{
			var popup = new CalendarPopup ()
			{
				Date         = this.date.HasValue ? this.date.Value : Timestamp.Now.Date,
				SelectedDate = this.date,
			};

			popup.Create (this.calendarButton, leftOrRight: false);

			popup.DateChanged += delegate (object sender, System.DateTime date)
			{
				this.Date = date;
				this.UpdateButtons ();
				this.dateFieldController.SetFocus ();
				this.OnDateChanged (this.date);
			};
		}


		private string GetPredefinedDescription(DateType type)
		{
			switch (type)
			{
				case DateType.BeginMandat:
					return "Début du mandat";

				case DateType.EndMandat:
					return "Fin du mandat";

				case DateType.BeginPreviousYear:
					return "Début de l'année précédente";

				case DateType.EndPreviousYear:
					return "Fin de l'année précédente";

				case DateType.BeginCurrentYear:
					return "Début de l'année en cours";

				case DateType.EndCurrentYear:
					return "Fin de l'année en cours";

				case DateType.BeginNextYear:
					return "Début de l'année suivante";

				case DateType.EndNextYear:
					return "Fin de l'année suivante";

				case DateType.BeginPreviousMonth:
					return "Début du mois précédent";

				case DateType.EndPreviousMonth:
					return "Fin du mois précédent";

				case DateType.BeginCurrentMonth:
					return "Début du mois en cours";

				case DateType.EndCurrentMonth:
					return "Fin du mois en cours";

				case DateType.BeginNextMonth:
					return "Début du mois suivant";

				case DateType.EndNextMonth:
					return "Fin du mois suivant";

				case DateType.Now:
					return "Aujourd'hui";

				default:
					return null;
			}
		}

		private int GetSelectedDate(System.DateTime? date)
		{
			int sel = 0;

			if (date.HasValue)
			{
				foreach (var type in this.DateTypes)
				{
					if (this.GetPredefinedDate (type) == date.Value)
					{
						return sel;
					}

					sel++;
				}
			}

			return -1;
		}

		private System.DateTime GetPredefinedDate(int rank)
		{
			foreach (var type in this.DateTypes)
			{
				if (rank-- <= 0)
				{
					return this.GetPredefinedDate (type);
				}
			}

			return this.GetPredefinedDate (DateType.Now);
		}

		private System.DateTime GetPredefinedDate(DateType type)
		{
			var now = Timestamp.Now.Date;

			switch (type)
			{
				case DateType.BeginMandat:
					return this.accessor.Mandat.StartDate;

				case DateType.EndMandat:
					return this.accessor.Mandat.EndDate;

				case DateType.BeginPreviousYear:
					return new System.DateTime (now.Year-1, 1, 1);

				case DateType.EndPreviousYear:
					return new System.DateTime (now.Year-1, 12, 31);

				case DateType.BeginCurrentYear:
					return new System.DateTime (now.Year, 1, 1);

				case DateType.EndCurrentYear:
					return new System.DateTime (now.Year, 12, 31);

				case DateType.BeginNextYear:
					return new System.DateTime (now.Year+1, 1, 1);

				case DateType.EndNextYear:
					return new System.DateTime (now.Year+1, 12, 31);

				case DateType.BeginPreviousMonth:
					return new System.DateTime (now.Year, now.Month, 1).AddMonths (-1);

				case DateType.EndPreviousMonth:
					return new System.DateTime (now.Year, now.Month, 1).AddDays (-1);

				case DateType.BeginCurrentMonth:
					return new System.DateTime (now.Year, now.Month, 1);

				case DateType.EndCurrentMonth:
					return new System.DateTime (now.Year, now.Month, 1).AddMonths (1).AddDays (-1);

				case DateType.BeginNextMonth:
					return new System.DateTime (now.Year, now.Month, 1).AddMonths (1);

				case DateType.EndNextMonth:
					return new System.DateTime (now.Year, now.Month, 1).AddMonths (2).AddDays (-1);

				case DateType.Now:
					return now;

				default:
					return System.DateTime.MaxValue;
			}
		}

		private IEnumerable<DateType> DateTypes
		{
			get
			{
				yield return DateType.BeginMandat;
				yield return DateType.EndMandat;

				yield return DateType.Separator;
				yield return DateType.Separator;

				yield return DateType.BeginPreviousYear;
				yield return DateType.EndPreviousYear;

				yield return DateType.Separator;

				yield return DateType.BeginCurrentYear;
				yield return DateType.EndCurrentYear;

				yield return DateType.Separator;

				yield return DateType.BeginNextYear;
				yield return DateType.EndNextYear;

				yield return DateType.Separator;
				yield return DateType.Separator;

				yield return DateType.BeginPreviousMonth;
				yield return DateType.EndPreviousMonth;

				yield return DateType.Separator;

				yield return DateType.BeginCurrentMonth;
				yield return DateType.EndCurrentMonth;

				yield return DateType.Separator;

				yield return DateType.BeginNextMonth;
				yield return DateType.EndNextMonth;
			}
		}

		private enum DateType
		{
			Unknown,
			Separator,

			BeginMandat,
			EndMandat,

			BeginPreviousYear,
			EndPreviousYear,

			BeginCurrentYear,
			EndCurrentYear,

			BeginNextYear,
			EndNextYear,

			BeginPreviousMonth,
			EndPreviousMonth,

			BeginCurrentMonth,
			EndCurrentMonth,

			BeginNextMonth,
			EndNextMonth,

			Now,
		}


		#region Events handler
		private void OnDateChanged(System.DateTime? dateTime)
		{
			this.DateChanged.Raise (this, dateTime);
		}

		public event EventHandler<System.DateTime?> DateChanged;
		#endregion


		public const int controllerWidth  = DateController.widgetWidth;
		public const int controllerHeight = DateController.lineHeight*2 + 4;

		private const int indent          = 30;
		private const int widgetWidth     = DateFieldController.controllerWidth + (int) (AbstractFieldController.lineHeight*7.5) + 4;
		private const int lineHeight      = AbstractFieldController.lineHeight;


		private readonly DataAccessor						accessor;
		private readonly DateFieldController				dateFieldController;

		private IconButton									dayButton;
		private IconButton									monthButton;
		private IconButton									yearButton;
		private IconButton									beginButton;
		private IconButton									nowButton;
		private IconButton									endButton;
		private IconButton									predefinedButton;
		private IconButton									calendarButton;
		private IconButton									deleteButton;
		private StaticText									info;

		private System.DateTime?							date;
	}
}
