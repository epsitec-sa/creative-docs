//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
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
			this.DateLabelWidth  = DateController.Indent;
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
				PreferredHeight = DateController.LineHeight,
				Margins         = new Margins (this.DateLabelWidth+(this.DateLabelWidth == 0 ? 0 : 10), 0, 0, 0),
				BackColor       = ColorManager.WindowBackgroundColor,
			};

			this.dayButton   = this.CreatePartButton (line, "Date.SelectDay",    5, 13);
			this.monthButton = this.CreatePartButton (line, "Date.SelectMonth", 19, 13);
			this.yearButton  = this.CreatePartButton (line, "Date.SelectYear",  33, 23);

			this.beginButton = this.CreateIconButton (line, "Date.CurrentYearBegin", DateController.LineHeight*2);
			this.nowButton   = this.CreateIconButton (line, "Date.Now",              DateController.LineHeight*1);
			this.endButton   = this.CreateIconButton (line, "Date.CurrentYearEnd",   DateController.LineHeight*0);

			ToolTip.Default.SetToolTip (this.dayButton,   "Sélectionne le jour");
			ToolTip.Default.SetToolTip (this.monthButton, "Sélectionne le mois");
			ToolTip.Default.SetToolTip (this.yearButton,  "Sélectionne l'année");

			ToolTip.Default.SetToolTip (this.beginButton, "Début de l'année en cours");
			ToolTip.Default.SetToolTip (this.nowButton,   "Aujourd'hui");
			ToolTip.Default.SetToolTip (this.endButton,   "Fin de l'année en cours");

			this.dayButton.Clicked += delegate
			{
				this.Select (0, 2);  // sélectionne [31].03.2013
			};

			this.monthButton.Clicked += delegate
			{
				this.Select (3, 2);  // sélectionne 31.[03].2013
			};

			this.yearButton.Clicked += delegate
			{
				this.Select (6, 4);  // sélectionne 31.03.[2013]
			};

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
		}

		private IconButton CreatePartButton(Widget parent, string icon, int x, int dx)
		{
			return new IconButton
			{
				Parent          = parent,
				IconUri         = AbstractCommandToolbar.GetResourceIconUri (icon),
				AutoFocus       = false,
				PreferredWidth  = dx,
				PreferredHeight = DateController.LineHeight-2,
				Anchor          = AnchorStyles.BottomLeft,
				Margins         = new Margins (x, 0, 0, 0),
			};
		}

		private IconButton CreateIconButton(Widget parent, string icon, int x)
		{
			return new IconButton
			{
				Parent          = parent,
				IconUri         = AbstractCommandToolbar.GetResourceIconUri (icon),
				AutoFocus       = false,
				PreferredWidth  = DateController.LineHeight,
				PreferredHeight = DateController.LineHeight,
				Anchor          = AnchorStyles.BottomRight,
				Margins         = new Margins (0, x, 0, 0),
			};
		}

		private void CreateController(Widget parent)
		{
			var footer = new FrameBox
			{
				Parent          = parent,
				Dock            = DockStyle.Top,
				PreferredHeight = DateController.LineHeight,
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
				Parent         = footer,
				PreferredWidth = DateController.WidgetWidth,
				Dock           = DockStyle.Fill,
				BackColor      = ColorManager.WindowBackgroundColor,
			};

			this.dateFieldController.HideAdditionalButtons = true;
			this.dateFieldController.TabIndex = this.TabIndex;
			this.dateFieldController.CreateUI (dateFrame);
			this.dateFieldController.SetFocus ();

			this.deleteButton = this.CreateIconButton (dateFrame, "Field.Clear", DateController.LineHeight*0);
			ToolTip.Default.SetToolTip (this.deleteButton, "Efface la date");

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

				this.UpdateButtonState (this.dayButton,   part == DateFieldController.Part.Day);
				this.UpdateButtonState (this.monthButton, part == DateFieldController.Part.Month);
				this.UpdateButtonState (this.yearButton,  part == DateFieldController.Part.Year);

				this.UpdateButtonState (this.beginButton, this.Date == this.GetPredefinedDate (DateType.BeginCurrentYear));
				this.UpdateButtonState (this.nowButton,   this.Date == this.GetPredefinedDate (DateType.Now));
				this.UpdateButtonState (this.endButton,   this.Date == this.GetPredefinedDate (DateType.EndCurrentYear));

				this.deleteButton.Enable = this.Date != null;
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


		private void Select(int start, int count)
		{
			this.dateFieldController.TextField.Focus ();
			this.dateFieldController.TextField.CursorFrom = start;
			this.dateFieldController.TextField.CursorTo   = start + count;
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

				case DateType.Now:
					return now;

				default:
					return System.DateTime.MaxValue;
			}
		}

		private enum DateType
		{
			Unknown,

			BeginMandat,
			EndMandat,

			BeginPreviousYear,
			EndPreviousYear,

			BeginCurrentYear,
			EndCurrentYear,

			BeginNextYear,
			EndNextYear,

			Now,
		}


		#region Events handler
		private void OnDateChanged(System.DateTime? dateTime)
		{
			this.DateChanged.Raise (this, dateTime);
		}

		public event EventHandler<System.DateTime?> DateChanged;
		#endregion


		public const int ControllerWidth  = DateController.WidgetWidth;
		public const int ControllerHeight = DateController.LineHeight * 2;

		private const int Indent          = 30;
		private const int WidgetWidth     = 142;
		private const int LineHeight      = 2+17+2;


		private readonly DataAccessor						accessor;
		private readonly DateFieldController				dateFieldController;

		private IconButton									dayButton;
		private IconButton									monthButton;
		private IconButton									yearButton;
		private IconButton									beginButton;
		private IconButton									nowButton;
		private IconButton									endButton;
		private IconButton									deleteButton;
		private System.DateTime?							date;
	}
}
