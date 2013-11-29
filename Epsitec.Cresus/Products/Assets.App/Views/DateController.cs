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

			this.radios = new Dictionary<DateType, RadioButton> ();
		}


		public System.DateTime? Date
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
					this.Update ();
				}
			}
		}


		public void CreateUI(Widget parent)
		{
			this.CreateLine (parent, "Mandat",           DateType.BeginMandat,       DateType.EndMandat);
			this.CreateLine (parent, "Année précédente", DateType.BeginPreviousYear, DateType.EndPreviousYear);
			this.CreateLine (parent, "Année courante",   DateType.BeginCurrentYear,  DateType.EndCurrentYear);
			this.CreateLine (parent, "Année suivante",   DateType.BeginNextYear,     DateType.EndNextYear);
			this.CreateLine (parent, "",                 DateType.Now,               DateType.Unknown);

			this.CreatePrefix (parent);
			this.CreateController (parent);
			this.Update ();
		}

		public void Update()
		{
			if (this.radios.Count != 0)
			{
				foreach (DateType type in System.Enum.GetValues (typeof (DateType)))
				{
					if (type != DateType.Unknown)
					{
						bool selected = this.date == this.GetPredefinedDate (type);
						this.radios[type].ActiveState = selected ? ActiveState.Yes : ActiveState.No;
					}
				}

				this.dateFieldController.Value = this.date;
			}
		}


		private void CreateLine(Widget parent, string label, DateType i1, DateType i2)
		{
			var line = new FrameBox
			{
				Parent          = parent,
				Dock            = DockStyle.Top,
				PreferredHeight = DateController.radioHeight,
			};

			new StaticText
			{
				Parent           = line,
				Text             = label,
				ContentAlignment = ContentAlignment.MiddleRight,
				PreferredWidth   = DateController.ColumnWidth1,
				PreferredHeight  = DateController.radioHeight,
				Dock             = DockStyle.Left,
				Margins          = new Margins (0, 10, 0, 0),
			};

			this.CreateRadio (line, i1, (i2 == DateType.Unknown) ? DateController.ColumnWidth2+DateController.ColumnWidth3 : DateController.ColumnWidth2);
			this.CreateRadio (line, i2, DateController.ColumnWidth3);
		}

		private void CreateRadio(Widget parent, DateType type, int width)
		{
			if (type == DateType.Unknown)
			{
				return;
			}

			var radio = new RadioButton
			{
				Parent          = parent,
				Text            = this.GetPredefinedDescription (type),
				Dock            = DockStyle.Left,
				PreferredWidth  = width,
				PreferredHeight = DateController.radioHeight,
			};

			radio.Clicked += delegate
			{
				this.Date = this.GetPredefinedDate (type);
				this.dateFieldController.SetFocus ();
				this.OnDateChanged (this.date);
			};

			this.radios[type] = radio;
		}

		private void CreatePrefix(Widget parent)
		{
			//	Crée la ligne des boutons [J] [M] [A] permettant de sélectionner
			//	la partie correspondante dans le textede la date.
			//	Les boutons sont positionnés horizontalement de façon à s'aligner
			//	au mieux sur le texte de la date dans le TextField.
			const int h = 17;

			var line = new FrameBox
			{
				Parent          = parent,
				Dock            = DockStyle.Top,
				PreferredHeight = h,
				Margins         = new Margins (DateController.ColumnWidth1+15, 0, 10, 0),
			};

			var dayButton = new Button
			{
				Parent          = line,
				Text            = "J",
				ButtonStyle     = ButtonStyle.ToolItem,
				Dock            = DockStyle.Left,
				PreferredWidth  = 13,
				PreferredHeight = h,
				Margins         = new Margins (0, 1, 0, 0),
			};

			var monthButton = new Button
			{
				Parent          = line,
				Text            = "M",
				ButtonStyle     = ButtonStyle.ToolItem,
				Dock            = DockStyle.Left,
				PreferredWidth  = 13,
				PreferredHeight = h,
				Margins         = new Margins (0, 1, 0, 0),
			};

			var yearButton = new Button
			{
				Parent          = line,
				Text            = "A",
				ButtonStyle     = ButtonStyle.ToolItem,
				Dock            = DockStyle.Left,
				PreferredWidth  = 23,
				PreferredHeight = h,
				Margins         = new Margins (0, 1, 0, 0),
			};

			dayButton.Clicked += delegate
			{
				this.Select (0, 2);  // sélectionne [31].03.2013
			};

			monthButton.Clicked += delegate
			{
				this.Select (3, 2);  // sélectionne 31.[03].2013
			};

			yearButton.Clicked += delegate
			{
				this.Select (6, 4);  // sélectionne 31.03.[2013]
			};
		}

		private void CreateController(Widget parent)
		{
			var footer = new FrameBox
			{
				Parent          = parent,
				Dock            = DockStyle.Top,
				PreferredHeight = DateController.dateHeight,
			};

			new StaticText
			{
				Parent           = footer,
				Text             = "Date",
				ContentAlignment = ContentAlignment.MiddleRight,
				PreferredWidth   = DateController.ColumnWidth1,
				Dock             = DockStyle.Left,
				Margins          = new Margins (0, 10, 0, 0),
			};

			var dateFrame = new FrameBox
			{
				Parent         = footer,
				PreferredWidth = 100,
				Dock           = DockStyle.Left,
				BackColor      = ColorManager.WindowBackgroundColor,
			};

			this.dateFieldController.HideAdditionalButtons = true;
			this.dateFieldController.CreateUI (dateFrame);
			this.dateFieldController.SetFocus ();

			this.dateFieldController.ValueEdited += delegate
			{
				this.Date = this.dateFieldController.Value;
				this.OnDateChanged (this.date);
			};
		}


		private void Select(int start, int count)
		{
			this.dateFieldController.TextField.Focus ();
			this.dateFieldController.TextField.CursorFrom = start;
			this.dateFieldController.TextField.CursorTo   = start + count;
		}


		private string GetPredefinedDescription(DateType type)
		{
			switch (type)
			{
				case DateType.BeginMandat:
				case DateType.BeginPreviousYear:
				case DateType.BeginCurrentYear:
				case DateType.BeginNextYear:
					return "Début";

				case DateType.EndMandat:
				case DateType.EndPreviousYear:
				case DateType.EndCurrentYear:
				case DateType.EndNextYear:
					return "Fin";

				case DateType.Now:
					return "Aujourd'hui";

				default:
					return null;
			}
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


		public const int ColumnWidth1     = 90;
		public const int ColumnWidth2     = 60;
		public const int ColumnWidth3     = 60;
		public const int ControllerWidth  = DateController.ColumnWidth1 + DateController.ColumnWidth2 + DateController.ColumnWidth3;
		public const int ControllerHeight = DateController.radioHeight*5 + 10 + 17 + DateController.dateHeight;

		private const int radioHeight = 17;
		private const int dateHeight  = 2+17+2;


		private readonly DataAccessor						accessor;
		private readonly Dictionary<DateType, RadioButton>	radios;
		private readonly DateFieldController				dateFieldController;

		private System.DateTime?							date;
	}
}
