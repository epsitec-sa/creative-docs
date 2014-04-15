//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Settings;
using Epsitec.Cresus.Assets.Server.Helpers;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class DateFieldController : AbstractFieldController
	{
		public DateFieldController(DataAccessor accessor)
			: base (accessor)
		{
			this.DateRangeCategory = DateRangeCategory.Free;
		}


		public DateRangeCategory				DateRangeCategory;

		public System.DateTime?					Value
		{
			get
			{
				return this.value;
			}
			set
			{
				if (this.value != value)
				{
					this.value = value;

					if (this.textField != null)
					{
						if (this.ignoreChanges.IsZero)
						{
							using (this.ignoreChanges.Enter ())
							{
								this.textField.Text = this.ConvDateToString (this.value);
								this.textField.SelectAll ();
							}
						}
					}

					this.AdjustHint ();
				}
			}
		}

		public TextField						TextField
		{
			get
			{
				return this.textField;
			}
		}

		private void UpdateValue()
		{
			using (this.ignoreChanges.Enter ())
			{
				this.textField.Text = this.ConvDateToString (this.value);
				this.textField.SelectAll ();
			}
		}

		protected override void ClearValue()
		{
			this.Value = null;
			this.AdjustHint ();
			this.UpdateButtons ();
			this.OnValueEdited (this.Field);
		}

		protected override void UpdatePropertyState()
		{
			base.UpdatePropertyState ();

			AbstractFieldController.UpdateTextField (this.textField, this.propertyState, this.isReadOnly);
			this.UpdateButtons ();
		}


		public override void CreateUI(Widget parent)
		{
			base.CreateUI (parent);

			this.textField = new TextField
			{
				Parent          = this.frameBox,
				TextDisplayMode = TextFieldDisplayMode.ActiveHint,
				PreferredWidth  = DateFieldController.fieldWidth,
				PreferredHeight = AbstractFieldController.lineHeight,
				Dock            = DockStyle.Left,
				TabIndex        = this.TabIndex,
				Text            = this.ConvDateToString (this.value),
			};

			this.minusButton      = this.CreateIconButton (this.frameBox, "Date.Minus",            false);
			this.plusButton       = this.CreateIconButton (this.frameBox, "Date.Plus",             false);
			this.beginButton      = this.CreateIconButton (this.frameBox, "Date.CurrentYearBegin", true);
			this.nowButton        = this.CreateIconButton (this.frameBox, "Date.Now",              false);
			this.endButton        = this.CreateIconButton (this.frameBox, "Date.CurrentYearEnd",   false);
			this.predefinedButton = this.CreateIconButton (this.frameBox, "Date.Predefined",       true);
			this.calendarButton   = this.CreateIconButton (this.frameBox, "Date.Calendar",         false);
			this.deleteButton     = this.CreateIconButton (this.frameBox, "Field.Delete",          true);

			ToolTip.Default.SetToolTip (this.beginButton,      "Début de l'année en cours");
			ToolTip.Default.SetToolTip (this.nowButton,        "Aujourd'hui");
			ToolTip.Default.SetToolTip (this.endButton,        "Fin de l'année en cours");
			ToolTip.Default.SetToolTip (this.predefinedButton, "Autre date à choix...");
			ToolTip.Default.SetToolTip (this.calendarButton,   "Calendrier...");
			ToolTip.Default.SetToolTip (this.deleteButton,     "Efface la date");

			//	Connexion des événements.
			this.textField.PreProcessing += delegate (object sender, MessageEventArgs e)
			{
				if (e.Message.MessageType == MessageType.KeyDown)
				{
					if (e.Message.KeyCode == KeyCode.ArrowDown)
					{
						this.AddValue (-1);
					}
					else if (e.Message.KeyCode == KeyCode.ArrowUp)
					{
						this.AddValue (1);
					}
				}
			};

			this.textField.TextChanged += delegate
			{
				if (this.ignoreChanges.IsZero)
				{
					using (this.ignoreChanges.Enter ())
					{
						this.Value = this.ConvStringToDate (this.textField.Text);
						this.AdjustHint ();
						this.UpdateButtons ();
						this.OnValueEdited (this.Field);
					}
				}
			};

			this.textField.CursorChanged += delegate
			{
				this.UpdateButtons ();
				this.OnCursorChanged ();
			};

			this.textField.SelectionChanged += delegate
			{
				this.UpdateButtons ();
				this.OnCursorChanged ();
			};

			this.textField.KeyboardFocusChanged += delegate (object sender, DependencyPropertyChangedEventArgs e)
			{
				bool focused = (bool) e.NewValue;

				if (focused)  // pris le focus ?
				{
					this.SetFocus ();
					this.AdjustHint ();
				}
				else  // perdu le focus ?
				{
					this.UpdateValue ();
					this.ClearHint ();
				}
			};

			this.minusButton.Clicked += delegate
			{
				this.AddValue (-1);
			};

			this.plusButton.Clicked += delegate
			{
				this.AddValue (1);
			};

			this.beginButton.Clicked += delegate
			{
				this.Value = this.GetPredefinedDate (DateType.BeginCurrentYear);
				this.UpdateButtons ();
				this.SetFocus ();
				this.OnValueEdited (this.Field);
			};

			this.nowButton.Clicked += delegate
			{
				this.Value = this.GetPredefinedDate (DateType.Now);
				this.UpdateButtons ();
				this.SetFocus ();
				this.OnValueEdited (this.Field);
			};

			this.endButton.Clicked += delegate
			{
				this.Value = this.GetPredefinedDate (DateType.EndCurrentYear);
				this.UpdateButtons ();
				this.SetFocus ();
				this.OnValueEdited (this.Field);
			};

			this.predefinedButton.Clicked += delegate
			{
				this.ShowPredefinedPopup ();
			};

			this.calendarButton.Clicked += delegate
			{
				this.ShowCalendarPopup ();
			};

			this.deleteButton.Clicked += delegate
			{
				this.textField.Text = null;
				this.UpdateButtons ();
				this.SetFocus ();
				this.OnValueEdited (this.Field);
			};
		}

		public override void SetFocus()
		{
			this.textField.SelectAll ();
			this.textField.Focus ();

			base.SetFocus ();
		}


		private IconButton CreateIconButton(Widget parent, string icon, bool leftMargin)
		{
			return new IconButton
			{
				Parent        = parent,
				IconUri       = Misc.GetResourceIconUri (icon),
				AutoFocus     = false,
				Dock          = DockStyle.Left,
				PreferredSize = new Size (AbstractFieldController.lineHeight, AbstractFieldController.lineHeight),
				Margins         = new Margins (leftMargin ? AbstractFieldController.lineHeight/2 : 0, 0, 0, 0),
			};
		}

		private void UpdateButtons()
		{
			if (this.minusButton == null)
			{
				return;
			}

			var part = this.SelectedPart;

			this.minusButton.Enable = !this.IsReadOnly && part != Part.Unknown;
			this.plusButton .Enable = !this.IsReadOnly && part != Part.Unknown;

			switch (part)
			{
				case Part.Day:
					ToolTip.Default.SetToolTip (this.minusButton, "Jour précédent");
					ToolTip.Default.SetToolTip (this.plusButton,  "Jour suivant");
					break;

				case Part.Month:
					ToolTip.Default.SetToolTip (this.minusButton, "Mois précédent");
					ToolTip.Default.SetToolTip (this.plusButton,  "Mois suivant");
					break;

				case Part.Year:
					ToolTip.Default.SetToolTip (this.minusButton, "Année précédente");
					ToolTip.Default.SetToolTip (this.plusButton,  "Année suivante");
					break;
			}

			this.UpdateButtonState (this.beginButton, this.Value == this.GetPredefinedDate (DateType.BeginCurrentYear));
			this.UpdateButtonState (this.nowButton,   this.Value == this.GetPredefinedDate (DateType.Now));
			this.UpdateButtonState (this.endButton,   this.Value == this.GetPredefinedDate (DateType.EndCurrentYear));

			this.beginButton     .Enable = !this.IsReadOnly;
			this.nowButton       .Enable = !this.IsReadOnly;
			this.endButton       .Enable = !this.IsReadOnly;
			this.predefinedButton.Enable = !this.IsReadOnly;
			this.calendarButton  .Enable = !this.IsReadOnly;
			this.deleteButton    .Enable = !this.IsReadOnly && !string.IsNullOrEmpty (this.textField.Text);
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


		private void ShowPredefinedPopup()
		{
			var popup = new SimplePopup ()
			{
				SelectedItem = this.GetSelectedDate (this.value),
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
				this.Value = this.GetPredefinedDate (rank);
				this.UpdateButtons ();
				this.SetFocus ();
				this.OnValueEdited (this.Field);
			};
		}

		private void ShowCalendarPopup()
		{
			var popup = new CalendarPopup ()
			{
				Date         = this.value.HasValue ? this.value.Value : Timestamp.Now.Date,
				SelectedDate = this.value,
			};

			popup.Create (this.calendarButton, leftOrRight: false);

			popup.DateChanged += delegate (object sender, System.DateTime date)
			{
				this.Value = date;
				this.UpdateButtons ();
				this.SetFocus ();
				this.OnValueEdited (this.Field);
			};
		}


		private string GetPredefinedDescription(DateType type)
		{
			switch (type)
			{
				case DateType.BeginMandat:
					return "Début du mandat";

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

				yield return DateType.Separator;
				yield return DateType.Separator;

				yield return DateType.BeginPreviousYear;

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


		private void AddValue(int value)
		{
			var part = this.SelectedPart;

			switch (part)
			{
				case Part.Day:
					this.AddDays (value);
					break;

				case Part.Month:
					this.AddMonths (value);
					break;

				default:
					this.AddYears (value);
					break;
			}

			this.SelectPart (part);
			this.UpdateButtons ();
		}

		private void AddYears(int years)
		{
			if (this.value.HasValue)
			{
				this.Value = this.value.Value.AddYears (years);
				this.textField.Focus ();
				this.OnValueEdited (this.Field);
			}
		}

		private void AddMonths(int days)
		{
			if (this.value.HasValue)
			{
				this.Value = this.value.Value.AddMonths (days);
				this.textField.Focus ();
				this.OnValueEdited (this.Field);
			}
		}

		private void AddDays(int days)
		{
			if (this.value.HasValue)
			{
				this.Value = this.value.Value.AddDays (days);
				this.textField.Focus ();
				this.OnValueEdited (this.Field);
			}
		}

		public Part SelectedPart
		{
			//	Détermine la partie de la date actuellement en édition.
			get
			{
				var text = this.textField.Text;

				if (!string.IsNullOrEmpty (text))
				{
					int f = System.Math.Min (this.textField.CursorFrom, this.textField.CursorTo);
					int t = System.Math.Max (this.textField.CursorFrom, this.textField.CursorTo);

					if (f == 0 && t == this.textField.Text.Length)
					{
						return Part.Year;
					}

					text = text.Replace (' ', '.');
					text = text.Replace ('/', '.');
					text = text.Replace (',', '.');
					text = text.Replace ('-', '.');

					var words = text.Split ('.');

					int part = 0;
					int i = 0;
					foreach (var word in words)
					{
						i += word.Length+1;

						if (this.textField.Cursor < i)
						{
							switch (part)
							{
								case 0:
									return Part.Day;

								case 1:
									return Part.Month;

								case 2:
									return Part.Year;
							}
						}

						part++;
					}
				}

				return Part.Unknown;
			}
		}

		private void SelectPart(Part part)
		{
			//	Sélectionne une partie du texte en édition.
			if (this.textField.Text.Length == 10)  // jj.mm.aaaa ?
			{
				switch (part)
				{
					case Part.Day:
						this.textField.CursorFrom = 0;
						this.textField.CursorTo   = 2;  // [31].03.2015
						break;

					case Part.Month:
						this.textField.CursorFrom = 3;
						this.textField.CursorTo   = 3+2;  // 31.[03].2015
						break;

					case Part.Year:
						this.textField.CursorFrom = 6;
						this.textField.CursorTo   = 6+4;  // 31.03.[2015]
						break;
				}
			}
		}

		public enum Part
		{
			Unknown,
			Day,
			Month,
			Year,
		}


		private void ClearHint()
		{
			//	Supprime le texte 'hint' de la date en édition.
			this.textField.HintText = null;
		}

		private void AdjustHint()
		{
			//	Ajuste le texte 'hint' de la date en édition.
			if (this.textField != null)
			{
				string hint = this.ConvDateToString (this.ConvStringToDate (this.textField.Text));

				if (string.IsNullOrEmpty (hint))
				{
					hint = this.ConvDateToString (this.DefaultDate);
				}

				this.textField.HintText = DateFieldController.AdjustHintDate (this.textField.FormattedText, hint);
			}
		}

		private static string AdjustHintDate(FormattedText entered, FormattedText hint)
		{
			//	Ajuste le texte 'hint' en fonction du texte entré, pour une date.
			//
			//	entered = "5"     hint = "05.03.2012" out = "5.03.2012"
			//	entered = "5."    hint = "05.03.2012" out = "5.03.2012"
			//	entered = "5.3"   hint = "05.03.2012" out = "5.3.2012"
			//	entered = "5.3."  hint = "05.03.2012" out = "5.3.2012"
			//	entered = "5.3.2" hint = "05.03.2012" out = "5.3.2"
			//	entered = "5 3"   hint = "05.03.2012" out = "5 3.2012"

			if (entered.IsNullOrEmpty () || hint.IsNullOrEmpty ())
			{
				return hint.ToSimpleText ();
			}

			//	Décompose le texte 'entered', en mots et en séparateurs.
			var brut = entered.ToSimpleText ();

			var we = new List<string> ();
			var se = new List<string> ();

			int j = 0;
			bool n = true;
			for (int i = 0; i <= brut.Length; i++)
			{
				bool isDigit;

				if (i < brut.Length)
				{
					isDigit = brut[i] >= '0' && brut[i] <= '9';
				}
				else
				{
					isDigit = !n;
				}

				if (n && !isDigit)
				{
					we.Add (brut.Substring (j, i-j));
					j = i;
					n = false;
				}

				if (!n && isDigit)
				{
					se.Add (brut.Substring (j, i-j));
					j = i;
					n = true;
				}
			}

			//	Décompose le texte 'hint', en mots.
			const char sep = '.';
			var wh = hint.ToSimpleText ().Split (sep);

			int count = System.Math.Min (we.Count, wh.Length);
			for (int i = 0; i < count; i++)
			{
				if (!string.IsNullOrEmpty (we[i]))
				{
					wh[i] = we[i];
				}
			}

			//	Recompose la chaîne finale.
			var builder = new System.Text.StringBuilder ();

			for (int i = 0; i < wh.Length; i++)
			{
				builder.Append (wh[i]);

				if (i < wh.Length-1)
				{
					if (i < se.Count)
					{
						builder.Append (se[i]);
					}
					else
					{
						builder.Append (sep);
					}
				}
			}

			return builder.ToString ();
		}

		private System.DateTime? DefaultDate
		{
			get
			{
				switch (this.DateRangeCategory)
				{
					case DateRangeCategory.Mandat:
						return LocalSettings.DefaultMandatDate;

					case DateRangeCategory.Free:
						return LocalSettings.DefaultFreeDate;

					default:
						throw new System.InvalidOperationException (string.Format ("Unsupported DateRangeCategory {0}", this.DateRangeCategory));
				}
			}
		}


		private string ConvDateToString(System.DateTime? value)
		{
			return TypeConverters.DateToString (value);
		}

		private System.DateTime? ConvStringToDate(string text)
		{
			System.DateTime? date;

			switch (this.DateRangeCategory)
			{
				case DateRangeCategory.Mandat:
					date = TypeConverters.ParseDate (text, LocalSettings.DefaultMandatDate, this.accessor.Mandat.StartDate, null);

					if (date.HasValue)
					{
						LocalSettings.DefaultMandatDate = date.Value;
					}
					break;

				case DateRangeCategory.Free:
					date = TypeConverters.ParseDate (text, LocalSettings.DefaultFreeDate, null, null);

					if (date.HasValue)
					{
						LocalSettings.DefaultFreeDate = date.Value;
					}
					break;

				default:
					throw new System.InvalidOperationException (string.Format ("Unsupported DateRangeCategory {0}", this.DateRangeCategory));
			}

			return date;
		}


		#region Events handler
		protected void OnCursorChanged()
		{
			this.CursorChanged.Raise (this);
		}

		public event EventHandler CursorChanged;
		#endregion


		private const int fieldWidth      = 70;
		public  const int controllerWidth = DateFieldController.fieldWidth + AbstractFieldController.lineHeight*2;


		private TextField						textField;
		private IconButton						minusButton;
		private IconButton						plusButton;
		private IconButton						beginButton;
		private IconButton						nowButton;
		private IconButton						endButton;
		private IconButton						predefinedButton;
		private IconButton						calendarButton;
		private IconButton						deleteButton;
		private System.DateTime?				value;
	}
}
