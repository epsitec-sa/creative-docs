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

			this.radios = new RadioButton[DateController.maxRadios];
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
			this.CreateLine (parent, "Exercice",         0, 8);
			this.CreateLine (parent, "Année précédente", 1, 2);
			this.CreateLine (parent, "Année courante",   3, 5);
			this.CreateLine (parent, "Année suivante",   6, 7);
			this.CreateLine (parent, "",                 4, -1);

			this.CreatePrefix (parent);
			this.CreateController (parent);
			this.Update ();
		}

		public void Update()
		{
			if (this.radios[0] != null)
			{
				for (int i=0; i<DateController.maxRadios; i++)
				{
					bool selected = this.date == this.GetPredefinedDate (i);
					this.radios[i].ActiveState = selected ? ActiveState.Yes : ActiveState.No;
				}

				this.dateFieldController.Value = this.date;
			}
		}


		private void CreateLine(Widget parent, string label, int i1, int i2)
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

			this.CreateRadio (line, i1, (i2 == -1) ? DateController.ColumnWidth2+DateController.ColumnWidth3 : DateController.ColumnWidth2);
			this.CreateRadio (line, i2, DateController.ColumnWidth3);
		}

		private void CreateRadio(Widget parent, int index, int width)
		{
			if (index == -1)
			{
				return;
			}

			var radio = new RadioButton
			{
				Parent          = parent,
				Text            = this.GetPredefinedDescription (index),
				Dock            = DockStyle.Left,
				PreferredWidth  = width,
				PreferredHeight = DateController.radioHeight,
			};

			radio.Clicked += delegate
			{
				this.Date = this.GetPredefinedDate (index);
				this.dateFieldController.SetFocus ();
				this.OnDateChanged (this.date);
			};

			this.radios[index] = radio;
		}

		private void CreatePrefix(Widget parent)
		{
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
				this.Select (0, 2);
			};

			monthButton.Clicked += delegate
			{
				this.Select (3, 2);
			};

			yearButton.Clicked += delegate
			{
				this.Select (6, 4);
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


		private string GetPredefinedDescription(int index)
		{
			switch (index)
			{
				case 0:
					return "Début";

				case 1:
					return "Début";

				case 2:
					return "Fin";

				case 3:
					return "Début";

				case 4:
					return "Aujourd'hui";

				case 5:
					return "Fin";

				case 6:
					return "Début";

				case 7:
					return "Fin";

				case 8:
					return "Fin";

				default:
					return null;
			}
		}

		private System.DateTime GetPredefinedDate(int index)
		{
			var now = Timestamp.Now.Date;

			switch (index)
			{
				case 0:
					return this.accessor.Mandat.StartDate;

				case 1:
					return new System.DateTime (now.Year-1, 1, 1);

				case 2:
					return new System.DateTime (now.Year-1, 12, 31);

				case 3:
					return new System.DateTime (now.Year, 1, 1);

				case 4:
					return now;

				case 5:
					return new System.DateTime (now.Year, 12, 31);

				case 6:
					return new System.DateTime (now.Year+1, 1, 1);

				case 7:
					return new System.DateTime (now.Year+1, 12, 31);

				case 8:
					return this.accessor.Mandat.EndDate;

				default:
					return System.DateTime.MaxValue;
			}
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

		private const int maxRadios   = 9;
		private const int radioHeight = 17;
		private const int dateHeight  = 2+17+2;


		private readonly DataAccessor			accessor;
		private readonly RadioButton[]			radios;
		private readonly DateFieldController	dateFieldController;

		private System.DateTime?				date;
	}
}
