//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Support;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Search.Data;
using Epsitec.Cresus.Compta.Fields.Controllers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Search.Controllers
{
	/// <summary>
	/// Ce contrôleur gère la saisie d'une période comprise entre deux dates.
	/// </summary>
	public class TemporalController
	{
		public TemporalController(AbstractController controller, TemporalData data)
		{
			this.controller = controller;
			this.data       = data;

			this.compta          = this.controller.ComptaEntity;
			this.dataAccessor    = this.controller.DataAccessor;
			this.businessContext = this.controller.BusinessContext;

			this.ignoreChanges = new SafeCounter ();
			this.descriptionBestFitWidths = new Dictionary<TemporalDataDuration, int> ();
		}


		public void SearchClear()
		{
			this.data.Clear ();
			this.UpdateButtons ();
		}


		public FrameBox CreateUI(FrameBox parent, System.Action searchStartAction)
		{
			this.searchStartAction = searchStartAction;

			this.mainFrame = new FrameBox
			{
				Parent  = parent,
				Dock    = DockStyle.Fill,
				//?Padding = new Margins (5),
			};

			this.CreateMainUI ();
			this.UpdateButtons ();

			return this.mainFrame;
		}

		private void CreateMainUI()
		{
			this.editionFrame = new FrameBox
			{
				Parent          = this.mainFrame,
				PreferredHeight = 20,
				Dock            = DockStyle.Left,
				Margins         = new Margins (0, 10, 0, 0),
			};

			this.staticFrame = UIBuilder.CreatePseudoCombo (this.mainFrame, out this.staticDates, out this.menuButton);

			{
				var label = new StaticText
				{
					Parent         = this.editionFrame,
					Text           = "Du",
					Dock           = DockStyle.Left,
					Margins        = new Margins (0, 10, 0, 0),
				};
				label.PreferredWidth = label.GetBestFitSize ().Width;

				var initialDate = Converters.DateToString (this.data.BeginDate);
				this.beginDateController = UIBuilder.CreateDateField (this.controller, this.editionFrame, initialDate, "Date initiale incluse", this.ValidateDate, this.DateChanged);
				this.beginDateController.Box.Dock = DockStyle.Left;
			}

			{
				var label = new StaticText
				{
					Parent         = this.editionFrame,
					Text           = "Au",
					Dock           = DockStyle.Left,
					Margins        = new Margins (10, 10, 0, 0),
				};
				label.PreferredWidth = label.GetBestFitSize ().Width;

				var initialDate = Converters.DateToString (this.data.EndDate);
				this.endDateController = UIBuilder.CreateDateField (this.controller, this.editionFrame, initialDate, "Date finale incluse", this.ValidateDate, this.DateChanged);
				this.endDateController.Box.Dock = DockStyle.Left;
			}

			this.editionInfo = new StaticText
			{
				Parent         = this.editionFrame,
				TextBreakMode  = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
				PreferredWidth = 60,
				Dock           = DockStyle.Left,
				Margins        = new Margins (10, 0, 0, 0),
			};

			this.dateSlider = new HSlider
			{
				Parent          = this.mainFrame,
				UseArrowGlyphs  = true,
				PreferredWidth  = 100,
				PreferredHeight = 20-2,
				Dock            = DockStyle.Left,
				Margins         = new Margins (3, 0, 1, 1),
			};

			this.nowButton = new Button
			{
				Parent          = this.mainFrame,
				FormattedText   = "Auj.",
				ButtonStyle     = ButtonStyle.Icon,
				AutoFocus       = false,
				PreferredHeight = 20,
				Dock            = DockStyle.Left,
				Margins         = new Margins (3, 1, 0, 0),
			};
			this.nowButton.PreferredWidth = this.nowButton.GetBestFitSize ().Width;

			ToolTip.Default.SetToolTip (this.dateSlider, "Choix de la période");
			ToolTip.Default.SetToolTip (this.menuButton, "Choix de la période");
			ToolTip.Default.SetToolTip (this.nowButton,  "Période incluant aujourd'hui");

			this.warningIcon = new StaticText
			{
				Parent         = this.mainFrame,
				Text           = UIBuilder.GetIconTag ("Warning"),
				PreferredWidth = 20,
				Dock           = DockStyle.Left,
				Margins         = new Margins (2, 0, 0, 0),
			};

			var durationLabel = new StaticText
			{
				Parent          = this.mainFrame,
				FormattedText   = "Durée",
				PreferredHeight = 20,
				Dock            = DockStyle.Left,
				Margins         = new Margins (10, 10, 0, 0),
			};
			durationLabel.PreferredWidth = durationLabel.GetBestFitSize ().Width;

			this.durationField = new TextFieldCombo
			{
				Parent          = this.mainFrame,
				PreferredWidth  = 100,
				PreferredHeight = 20,
				MenuButtonWidth = UIBuilder.ComboButtonWidth,
				IsReadOnly      = true,
				Dock            = DockStyle.Left,
			};

			TemporalController.InitTemporalDataDurationCombo (this.durationField);

			//	Connexion des événements.
			this.durationField.SelectedItemChanged += delegate
			{
				this.data.Duration = TemporalController.TemporalDataDurationToType (this.durationField.FormattedText);
				this.data.SetDate (this.data.BeginDate);
				this.UpdateButtons ();
				this.searchStartAction ();
			};

			this.dateSlider.ValueChanged += delegate
			{
				if (this.ignoreChanges.IsZero)
				{
					int sel = (int) this.dateSlider.Value;
					var dr = this.DateRanges.ToArray ();
					if (sel >= 0 && sel < dr.Length)
					{
						this.data.SetDate (dr[sel].BeginDate);
						this.UpdateButtons ();
						this.searchStartAction ();
					}
				}
			};

			this.nowButton.Clicked += delegate
			{
				this.data.SetDate (Date.Today);
				this.UpdateButtons ();
				this.searchStartAction ();
			};

			this.staticDates.Clicked += delegate
			{
				this.ShowMenu (this.staticFrame);
			};

			this.menuButton.Clicked += delegate
			{
				this.ShowMenu (this.staticFrame);
			};
		}

		private void ValidateDate(EditionData data)
		{
			Validators.ValidateDate (data, emptyAccepted: true);
		}

		private void DateChanged(int line, ColumnType columnType)
		{
			this.data.BeginDate = Converters.ParseDate (this.beginDateController.EditionData.Text);
			this.data.EndDate   = Converters.ParseDate (this.endDateController.EditionData.Text);

			this.UpdateInfos ();
			this.searchStartAction ();
		}


		public void UpdateContent()
		{
			this.UpdateButtons ();
		}

		private void UpdateButtons()
		{
			using (this.ignoreChanges.Enter ())
			{
				this.editionFrame.Visibility =  this.EditionEnable;
				this.staticFrame.Visibility  = !this.EditionEnable;
				this.dateSlider.Visibility   = !this.EditionEnable;
				this.nowButton.Visibility    = !this.EditionEnable;
				this.menuButton.Visibility   = !this.EditionEnable;

				this.durationField.FormattedText = TemporalController.TemporalDataDurationToString (this.data.Duration);

				this.beginDateController.EditionData.Text = Converters.DateToString (this.data.BeginDate);
				this.beginDateController.EditionDataToWidget ();
				this.beginDateController.Validate ();

				this.endDateController.EditionData.Text = Converters.DateToString (this.data.EndDate);
				this.endDateController.EditionDataToWidget ();
				this.endDateController.Validate ();

				this.nowButton.Enable = !Dates.DateInRange (Date.Today, this.data.BeginDate, this.data.EndDate);

				if (!this.EditionEnable)
				{
					var dr = this.DateRanges.ToArray ();
					int n = dr.Length;
					int sel = 0;

					if (this.data.BeginDate < dr.First ().BeginDate)
					{
						sel = 0;
					}
					else if (this.data.BeginDate > dr.Last ().BeginDate)
					{
						sel = n-1;
					}
					else
					{
						for (int i = 0; i < n; i++)
						{
							if (this.data.BeginDate == dr[i].BeginDate)
							{
								sel = i;
								break;
							}
						}
					}

					this.dateSlider.MinValue    = (decimal) 0;
					this.dateSlider.MaxValue    = (decimal) n-1;
					this.dateSlider.Resolution  = (decimal) 1;
					this.dateSlider.SmallChange = (decimal) 1;
					this.dateSlider.LargeChange = (decimal) 2;
					this.dateSlider.Value       = (decimal) sel;
				}
			}

			this.UpdateInfos ();
		}

		private void UpdateInfos()
		{
			this.staticFrame.PreferredWidth = this.DescriptionBestFitWidth + UIBuilder.ComboButtonWidth;
			this.staticDates.FormattedText = Dates.GetDescription (this.data.BeginDate, this.data.EndDate);
			this.editionInfo.FormattedText = this.NumberOfDays;

			var error = this.ErrorDescription;

			if (error.IsNullOrEmpty)
			{
				this.warningIcon.Visibility = false;
			}
			else
			{
				this.warningIcon.Visibility = true;
				ToolTip.Default.SetToolTip (this.warningIcon, error);
			}
		}


		#region Best fit width code
		private int DescriptionBestFitWidth
		{
			//	Retourne la largeur nécessaire pour afficher une période.
			get
			{
				int width;

				//	Le dictionnaire agit comme un cache.
				if (!this.descriptionBestFitWidths.TryGetValue (this.data.Duration, out width))
				{
					width = TemporalController.GetDescriptionBestFitWidth (this.data.Duration);
					this.descriptionBestFitWidths.Add (this.data.Duration, width);
				}

				return width;
			}
		}

		private static int GetDescriptionBestFitWidth(TemporalDataDuration duration)
		{
			//	Retourne la largeur nécessaire pour afficher une période.
			var textLayout = new TextLayout ();

			int max = 0;
			foreach (var dr in TemporalController.GetDateRangeSamples (duration))
			{
				textLayout.FormattedText = Dates.GetDescription (dr.BeginDate, dr.EndDate);
				int width = (int) textLayout.GetSingleLineSize ().Width;
				max = System.Math.Max (max, width);
			}

			if (max == 0)
			{
				return 100;
			}
			else
			{
				return max + 10;
			}
		}

		private static IEnumerable<DateRange> GetDateRangeSamples(TemporalDataDuration duration)
		{
			//	Retourne suffisemment d'échantillons pour représenter la plus grande largeur à utiliser pour une durée.
			Date date1, date2;

			switch (duration)
			{
				case TemporalDataDuration.Daily:
					for (int i = 0; i < 7; i++)
					{
						date1 = new Date (2012, 1, i+1);
						date2 = date1;
						yield return new DateRange (date1, date2);
					}
					break;

				case TemporalDataDuration.Weekly:
					date1 = new Date (2012, 5, 7);
					date2 = new Date (2012, 5, 13);
					yield return new DateRange (date1, date2);
					break;

				case TemporalDataDuration.Monthly:
					for (int i = 0; i < 12; i++)
					{
						date1 = new Date (2012, i+1, 1);
						date2 = Dates.AddDays (Dates.AddMonths(date1, 1), -1);
						yield return new DateRange (date1, date2);
					}
					break;

				case TemporalDataDuration.Quarterly:
					for (int i = 0; i < 4; i++)
					{
						date1 = new Date (2012, i+1, 1);
						date2 = Dates.AddDays (Dates.AddMonths(date1, 3), -1);
						yield return new DateRange (date1, date2);
					}
					break;

				case TemporalDataDuration.Biannual:
					for (int i = 0; i < 2; i++)
					{
						date1 = new Date (2012, i+1, 1);
						date2 = Dates.AddDays (Dates.AddMonths(date1, 6), -1);
						yield return new DateRange (date1, date2);
					}
					break;

				case TemporalDataDuration.Annual:
					date1 = new Date (2012, 1, 1);
					date2 = new Date (2012, 12, 31);
					yield return new DateRange (date1, date2);
					break;
			}
		}
		#endregion


		private FormattedText NumberOfDays
		{
			get
			{
				if (this.data.BeginDate.HasValue && this.data.EndDate.HasValue)
				{
					int n = Dates.NumberOfDays (this.data.EndDate.Value, this.data.BeginDate.Value) + 1;

					if (n <= 0)
					{
						return "(0 jour)";
					}
					else if (n == 1)
					{
						return "(1 jour)";
					}
					else
					{
						return string.Format ("({0} jours)", n.ToString ());
					}
				}
				else
				{
					return FormattedText.Empty;
				}
			}
		}

		private FormattedText ErrorDescription
		{
			get
			{
				var période = this.controller.MainWindowController.Période;

				if (Dates.DateInRange (this.data.BeginDate, période.DateDébut, période.DateFin) &&
					Dates.DateInRange (this.data.EndDate,   période.DateDébut, période.DateFin))
				{
					return FormattedText.Empty;
				}
				else
				{
					return FormattedText.Concat ("La période choisie déborde de la période comptable");
				}
			}
		}


		private bool EditionEnable
		{
			get
			{
				return this.data.Duration == TemporalDataDuration.Other;
			}
		}


		#region TemporalDataDuration helpers
		private static void InitTemporalDataDurationCombo(TextFieldCombo combo)
		{
			combo.Items.Clear ();

			foreach (var type in TemporalController.TemporalDataDurations)
			{
				combo.Items.Add (TemporalController.TemporalDataDurationToString (type));
			}
		}

		private static TemporalDataDuration TemporalDataDurationToType(FormattedText text)
		{
			foreach (var type in TemporalController.TemporalDataDurations)
			{
				if (TemporalController.TemporalDataDurationToString (type) == text)
				{
					return type;
				}
			}

			return TemporalDataDuration.Unknown;
		}

		private static FormattedText TemporalDataDurationToString(TemporalDataDuration duration)
		{
			//	Texte affiché après "Durée".
			switch (duration)
			{
				case TemporalDataDuration.Daily:
					return "Journalière";

				case TemporalDataDuration.Weekly:
					return "Hebdomadaire";

				case TemporalDataDuration.Monthly:
					return "Mensuelle";

				case TemporalDataDuration.Quarterly:
					return "Trimestrielle";

				case TemporalDataDuration.Biannual:
					return "Semestrielle";

				case TemporalDataDuration.Annual:
					return "Annuelle";

				case TemporalDataDuration.Other:
					return "Quelconque";

				default:
					return "?";
			}
		}

		private static IEnumerable<TemporalDataDuration> TemporalDataDurations
		{
			get
			{
				yield return TemporalDataDuration.Other;
				yield return TemporalDataDuration.Daily;
				yield return TemporalDataDuration.Weekly;
				yield return TemporalDataDuration.Monthly;
				yield return TemporalDataDuration.Quarterly;
				yield return TemporalDataDuration.Biannual;
				yield return TemporalDataDuration.Annual;
			}
		}
		#endregion


		private void ShowMenu(Widget parentButton)
		{
			//	Affiche le menu permettant de choisir la période.
			var menu = new VMenu ();

			var dr = this.DateRanges.ToArray ();

			int first = 0;
			int count = dr.Length;
			int max = 20;  // limite arbitraire, au-delà de laquelle le menu est considéré comme trop long

			if (count > max)  // menu trop long ?
			{
				int sel = -1;

				for (int i = 0; i < dr.Length; i++)
				{
					var dateRange = dr[i];
					bool select = dateRange.BeginDate == this.data.BeginDate;

					if (select)
					{
						sel = i;
						break;
					}
				}

				if (sel == -1)
				{
					if (this.data.BeginDate > dr.First ().BeginDate)
					{
						first = dr.Length-max;
					}
				}
				else
				{
					first = System.Math.Min (sel+max/2, dr.Length-1);
					first = System.Math.Max (first-max+1, 0);
				}

				count = System.Math.Min (dr.Length - first, max);
			}

			//	Ajoute la première case ?
			if (first > 0)
			{
				this.AddToMenu (menu, dr, 0);
				menu.Items.Add (new MenuSeparator ());
			}

			//	Ajoute les cases intermédiaires (20 au maximum).
			for (int i = first; i < first+count; i++)
			{
				this.AddToMenu (menu, dr, i);
			}

			//	Ajoute la dernière case ?
			if (first+count < dr.Length)
			{
				menu.Items.Add (new MenuSeparator ());
				this.AddToMenu (menu, dr, dr.Length-1);
			}

			if (menu.Items.Any ())
			{
				TextFieldCombo.AdjustComboSize (parentButton, menu, false);

				menu.Host = parentButton.Window;
				menu.ShowAsComboList (parentButton, Point.Zero, parentButton);
			}
		}

		private void AddToMenu(VMenu menu, DateRange[] dr, int i)
		{
			//	Ajoute une case dans le menu permettant de choisir la période.
			var dateRange = dr[i];
			bool select = dateRange.BeginDate == this.data.BeginDate;

			var item = new MenuItem ()
			{
				IconUri       = UIBuilder.GetRadioStateIconUri (select),
				FormattedText = this.GetDateRangeDescription (dateRange),
				TabIndex      = i,
			};

			item.Clicked += delegate
			{
				this.data.BeginDate = dr[item.TabIndex].BeginDate;
				this.data.EndDate   = dr[item.TabIndex].EndDate;
				this.UpdateButtons ();
				this.searchStartAction ();
			};

			menu.Items.Add (item);
		}


		private FormattedText GetDateRangeDescription(DateRange dateRange)
		{
			var desc = Dates.GetDescription (dateRange.BeginDate, dateRange.EndDate);
			var rank = FormattedText.Empty;

			if (this.data.Duration == TemporalDataDuration.Monthly)
			{
				rank = dateRange.BeginDate.Month.ToString ("00");  // 01..12
			}
			else if (this.data.Duration == TemporalDataDuration.Quarterly)
			{
				rank = ((dateRange.BeginDate.Month-1)/3+1).ToString ("0");  // 1..4
			}
			else if (this.data.Duration == TemporalDataDuration.Biannual)
			{
				rank = ((dateRange.BeginDate.Month-1)/6+1).ToString ("0");  // 1..2
			}

			if (!rank.IsNullOrEmpty)
			{
				desc = FormattedText.Concat (rank.ApplyBold (), ": ", desc);
			}

			return desc;
		}

		private IEnumerable<DateRange> DateRanges
		{
			//	Retourne la liste des intervalles faisant partie de la période comptable en cours.
			get
			{
				var période = this.controller.MainWindowController.Période;

				var temp = new TemporalData ();
				this.data.CopyTo (temp);
				temp.BeginDate = période.DateDébut;
				temp.SetDate (temp.BeginDate);

				do
				{
					yield return new DateRange (temp.BeginDate.Value, temp.EndDate.Value);

					temp.BeginDate = Dates.AddDays (temp.EndDate.Value, 1);
					temp.SetDate (temp.BeginDate);
				}
				while (temp.BeginDate <= période.DateFin);
			}
		}

		private class DateRange
		{
			//	Cette petite classe représente simplement un intervalle de dates (de x à y),
			//	où les 2 dates sont inclues.
			public DateRange(Date beginDate, Date endDate)
			{
				this.BeginDate = beginDate;
				this.EndDate   = endDate;
			}

			public Date BeginDate
			{
				get;
				private set;
			}

			public Date EndDate
			{
				get;
				private set;
			}
		}


		private readonly AbstractController				controller;
		private readonly ComptaEntity					compta;
		private readonly BusinessContext				businessContext;
		private readonly AbstractDataAccessor			dataAccessor;
		private readonly TemporalData					data;
		private readonly SafeCounter					ignoreChanges;
		private readonly Dictionary<TemporalDataDuration, int> descriptionBestFitWidths;

		private System.Action							searchStartAction;
		private FrameBox								mainFrame;
		private FrameBox								editionFrame;
		private TextFieldCombo							durationField;
		private HSlider									dateSlider;
		private Button									nowButton;
		private DateFieldController						beginDateController;
		private DateFieldController						endDateController;
		private FrameBox								staticFrame;
		private StaticText								staticDates;
		private GlyphButton								menuButton;
		private StaticText								editionInfo;
		private StaticText								warningIcon;
	}
}
