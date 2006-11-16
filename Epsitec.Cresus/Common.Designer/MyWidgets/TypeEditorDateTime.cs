using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.MyWidgets
{
	/// <summary>
	/// Widget permettant d'éditer un Caption.Type.
	/// </summary>
	public class TypeEditorDateTime : AbstractTypeEditor
	{
		public TypeEditorDateTime()
		{
			Widget group;

			Widget band = new Widget(this);
			band.TabIndex = this.tabIndex++;
			band.TabNavigation = TabNavigationMode.ForwardTabPassive;
			band.Dock = DockStyle.StackBegin;
			band.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;

			Widget left = new Widget(band);
			left.TabIndex = this.tabIndex++;
			left.TabNavigation = TabNavigationMode.ForwardTabPassive;
			left.Dock = DockStyle.Fill;

			Widget right = new Widget(band);
			right.TabIndex = this.tabIndex++;
			right.TabNavigation = TabNavigationMode.ForwardTabPassive;
			right.Dock = DockStyle.Fill;

			this.CreateComboLabeled("Résolution", left, out group, out this.fieldResol);
			group.Dock = DockStyle.StackBegin;
			group.Margins = new Margins(0, 0, 0, 10);
			this.fieldResol.TextChanged += new EventHandler(this.HandleTextFieldChanged);
			this.fieldResol.Items.Add("Milliseconds");
			this.fieldResol.Items.Add("Seconds");
			this.fieldResol.Items.Add("Minutes");
			this.fieldResol.Items.Add("Hours");
			this.fieldResol.Items.Add("Days");
			this.fieldResol.Items.Add("Weeks");
			this.fieldResol.Items.Add("Months");
			this.fieldResol.Items.Add("Years");

			//	Date, à gauche.
			this.CreateStringLabeled("Date minimale", left, out this.groupMinDate, out this.fieldMinDate);
			this.groupMinDate.Dock = DockStyle.StackBegin;
			this.groupMinDate.Margins = new Margins(0, 0, 0, 2);
			this.fieldMinDate.TextChanged += new EventHandler(this.HandleTextFieldChanged);

			this.CreateStringLabeled("Date maximale", left, out this.groupMaxDate, out this.fieldMaxDate);
			this.groupMaxDate.Dock = DockStyle.StackBegin;
			this.groupMaxDate.Margins = new Margins(0, 0, 0, 10);
			this.fieldMaxDate.TextChanged += new EventHandler(this.HandleTextFieldChanged);

			this.CreateStringLabeled("Pas pour la date", left, out this.groupDateStep, out this.fieldDateStep);
			this.groupDateStep.Dock = DockStyle.StackBegin;
			this.groupDateStep.Margins = new Margins(0, 0, 0, 0);
			this.fieldDateStep.TextChanged += new EventHandler(this.HandleTextFieldChanged);

			//	Heure, à droite.
			this.CreateStringLabeled("Heure minimale", right, out this.groupMinTime, out this.fieldMinTime);
			this.groupMinTime.Dock = DockStyle.StackBegin;
			this.groupMinTime.Margins = new Margins(0, 0, 20+10, 2);
			this.fieldMinTime.TextChanged += new EventHandler(this.HandleTextFieldChanged);

			this.CreateStringLabeled("Heure maximale", right, out this.groupMaxTime, out this.fieldMaxTime);
			this.groupMaxTime.Dock = DockStyle.StackBegin;
			this.groupMaxTime.Margins = new Margins(0, 0, 0, 10);
			this.fieldMaxTime.TextChanged += new EventHandler(this.HandleTextFieldChanged);

			this.CreateStringLabeled("Pas pour l'heure", right, out this.groupTimeStep, out this.fieldTimeStep);
			this.groupTimeStep.Dock = DockStyle.StackBegin;
			this.groupTimeStep.Margins = new Margins(0, 0, 0, 0);
			this.fieldTimeStep.TextChanged += new EventHandler(this.HandleTextFieldChanged);
		}

		public TypeEditorDateTime(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.fieldResol.TextChanged -= new EventHandler(this.HandleTextFieldChanged);

				this.fieldMinDate.TextChanged -= new EventHandler(this.HandleTextFieldChanged);
				this.fieldMaxDate.TextChanged -= new EventHandler(this.HandleTextFieldChanged);
				this.fieldDateStep.TextChanged -= new EventHandler(this.HandleTextFieldChanged);
				
				this.fieldMinTime.TextChanged -= new EventHandler(this.HandleTextFieldChanged);
				this.fieldMaxTime.TextChanged -= new EventHandler(this.HandleTextFieldChanged);
				this.fieldTimeStep.TextChanged -= new EventHandler(this.HandleTextFieldChanged);
			}
			
			base.Dispose(disposing);
		}


		public override string GetSummary()
		{
			//	Retourne le texte du résumé.
			System.Text.StringBuilder builder = new System.Text.StringBuilder();

			AbstractDateTimeType type = this.AbstractType as AbstractDateTimeType;

			if (type.Resolution != TimeResolution.Default)
			{
				builder.Append("Résolution = ");
				builder.Append(TypeEditorDateTime.Convert(type.Resolution));
			}

			if (type.MinimumDate != Date.Null)
			{
				this.PutSummaryLegend(builder, "Date min = ");
				builder.Append(TypeEditorDateTime.ToDate(type.MinimumDate.ToDateTime()));
			}

			if (type.MaximumDate != Date.Null)
			{
				this.PutSummaryLegend(builder, "Date max = ");
				builder.Append(TypeEditorDateTime.ToDate(type.MaximumDate.ToDateTime()));
			}

			if (type.MinimumTime != Time.Null)
			{
				this.PutSummaryLegend(builder, "Heure min = ");
				builder.Append(TypeEditorDateTime.ToTime(type.MinimumTime.ToDateTime()));
			}

			if (type.MaximumTime != Time.Null)
			{
				this.PutSummaryLegend(builder, "Heure max = ");
				builder.Append(TypeEditorDateTime.ToTime(type.MaximumTime.ToDateTime()));
			}

			this.PutSummaryLegend(builder, "Pas date = ");
			builder.Append(TypeEditorDateTime.ToDateStep(type.DateStep));

			this.PutSummaryLegend(builder, "Pas heure = ");
			builder.Append(TypeEditorDateTime.ToTimeSpan(type.TimeStep));

			return builder.ToString();
		}

		protected void PutSummaryLegend(System.Text.StringBuilder builder, string legend)
		{
			if (builder.Length > 0)
			{
				builder.Append(", ");
			}

			builder.Append(legend);
		}


		protected override void UpdateContent()
		{
			//	Met à jour le contenu de l'éditeur.
			AbstractDateTimeType type = this.AbstractType as AbstractDateTimeType;

			bool showDate = true;
			bool showTime = true;

			if (type is DateType)
			{
				showTime = false;
			}

			if (type is TimeType)
			{
				showDate = false;
			}

			this.groupMinDate.Visibility = showDate;
			this.groupMaxDate.Visibility = showDate;
			this.groupDateStep.Visibility = showDate;

			this.groupMinTime.Visibility = showTime;
			this.groupMaxTime.Visibility = showTime;
			this.groupTimeStep.Visibility = showTime;

			this.ignoreChange = true;
			this.fieldResol.Text = TypeEditorDateTime.Convert(type.Resolution);
			TypeEditorDateTime.ToDate(this.fieldMinDate, type.MinimumDate);
			TypeEditorDateTime.ToDate(this.fieldMaxDate, type.MaximumDate);
			TypeEditorDateTime.ToTime(this.fieldMinTime, type.MinimumTime);
			TypeEditorDateTime.ToTime(this.fieldMaxTime, type.MaximumTime);
			TypeEditorDateTime.ToDateStep(this.fieldDateStep, type.DateStep);
			TypeEditorDateTime.ToTimeSpan(this.fieldTimeStep, type.TimeStep);
			this.ignoreChange = false;
		}


		protected static TimeResolution Convert(string text)
		{
			switch (text)
			{
				case "Milliseconds":  return TimeResolution.Milliseconds;
				case "Seconds":       return TimeResolution.Seconds;
				case "Minutes":       return TimeResolution.Minutes;
				case "Hours":         return TimeResolution.Hours;
				case "Days":          return TimeResolution.Days;
				case "Weeks":         return TimeResolution.Weeks;
				case "Months":        return TimeResolution.Months;
				case "Years":         return TimeResolution.Years;
				default:              return TimeResolution.Default;
			}
		}

		protected static string Convert(TimeResolution resol)
		{
			switch (resol)
			{
				case TimeResolution.Milliseconds:  return "Milliseconds";
				case TimeResolution.Seconds:       return "Seconds";
				case TimeResolution.Minutes:       return "Minutes";
				case TimeResolution.Hours:         return "Hours";
				case TimeResolution.Days:          return "Days";
				case TimeResolution.Weeks:         return "Weeks";
				case TimeResolution.Months:        return "Months";
				case TimeResolution.Years:         return "Years";
				default:                           return "";
			}
		}

		protected static void ToDate(TextField field, Date date)
		{
			if (date == Date.Null)
			{
				field.Text = "";
			}
			else
			{
				field.Text = TypeEditorDateTime.ToDate(date.ToDateTime());
			}
		}

		protected static void ToTime(TextField field, Time time)
		{
			if (time == Time.Null)
			{
				field.Text = "";
			}
			else
			{
				field.Text = TypeEditorDateTime.ToTime(time.ToDateTime());
			}
		}

		protected static Date ToDate(TextField field)
		{
			if (!string.IsNullOrEmpty(field.Text))
			{
				System.DateTime dt = TypeEditorDateTime.ToDateTime(field.Text);
				if (dt != System.DateTime.MinValue)
				{
					return new Date(dt);
				}
			}

			return Date.Null;
		}

		protected static Time ToTime(TextField field)
		{
			if (!string.IsNullOrEmpty(field.Text))
			{
				System.DateTime dt = TypeEditorDateTime.ToDateTime(field.Text);
				if (dt != System.DateTime.MinValue)
				{
					return new Time(dt);
				}
			}

			return Time.Null;
		}

		protected static void ToDateStep(TextField field, DateStep ds)
		{
			if (ds == new DateStep(0, 0, 0))
			{
				field.Text = "";
			}
			else
			{
				field.Text = TypeEditorDateTime.ToDateStep(ds);
			}
		}

		protected static DateStep ToDateStep(TextField field)
		{
			if (!string.IsNullOrEmpty(field.Text))
			{
				return TypeEditorDateTime.ToDateStep(field.Text);
			}

			return new DateStep(0, 0, 0);
		}

		protected static void ToTimeSpan(TextField field, System.TimeSpan ts)
		{
			if (ts == System.TimeSpan.Zero)
			{
				field.Text = "";
			}
			else
			{
				field.Text = TypeEditorDateTime.ToTimeSpan(ts);
			}
		}

		protected static System.TimeSpan ToTimeSpan(TextField field)
		{
			if (!string.IsNullOrEmpty(field.Text))
			{
				System.TimeSpan ts = TypeEditorDateTime.ToTimeSpan(field.Text);
				if (ts != System.TimeSpan.Zero)
				{
					return ts;
				}
			}

			return System.TimeSpan.Zero;
		}

		protected static string ToDate(System.DateTime dt)
		{
			//	(d) Short date: 4/17/2006
			return dt.ToString("d", System.Globalization.CultureInfo.CurrentCulture);
		}

		protected static string ToTime(System.DateTime dt)
		{
			//	(T) Long time: 14:22:48
			return dt.ToString("T", System.Globalization.CultureInfo.CurrentCulture);
		}

		protected static string ToDateTime(System.DateTime dt)
		{
			//	(G) General date/long time: 17.04.2006 14:22:48
			return dt.ToString("G", System.Globalization.CultureInfo.CurrentCulture);
		}

		protected static System.DateTime ToDateTime(string text)
		{
			System.DateTime dt;
			if (System.DateTime.TryParse(text, System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.AssumeLocal|System.Globalization.DateTimeStyles.NoCurrentDateDefault, out dt))
			{
				return dt;
			}
			else
			{
				return System.DateTime.MinValue;
			}
		}

		protected static string ToDateStep(DateStep ds)
		{
			return ds.ToString();
		}

		protected static DateStep ToDateStep(string text)
		{
			try
			{
				return DateStep.Parse(text);
			}
			catch
			{
				return new DateStep(0, 0, 0);
			}
		}

		protected static string ToTimeSpan(System.TimeSpan ts)
		{
			return ts.ToString();
		}

		protected static System.TimeSpan ToTimeSpan(string text)
		{
			System.TimeSpan ts;
			if (System.TimeSpan.TryParse(text, out ts))
			{
				return ts;
			}
			else
			{
				return System.TimeSpan.Zero;
			}
		}


		private void HandleTextFieldChanged(object sender)
		{
			if (this.ignoreChange)
			{
				return;
			}

			//	[Note1] On demande le type avec un ResourceAccess.GetField.
			AbstractDateTimeType type = this.AbstractType as AbstractDateTimeType;

			if (sender == this.fieldResol)
			{
				type.DefineResolution(TypeEditorDateTime.Convert(this.fieldResol.Text));
			}

			if (sender == this.fieldMinDate)
			{
				type.DefineMinimumDate(TypeEditorDateTime.ToDate(this.fieldMinDate));
			}

			if (sender == this.fieldMaxDate)
			{
				type.DefineMaximumDate(TypeEditorDateTime.ToDate(this.fieldMaxDate));
			}

			if (sender == this.fieldMinTime)
			{
				type.DefineMinimumTime(TypeEditorDateTime.ToTime(this.fieldMinTime));
			}

			if (sender == this.fieldMaxTime)
			{
				type.DefineMaximumTime(TypeEditorDateTime.ToTime(this.fieldMaxTime));
			}

			if (sender == this.fieldDateStep)
			{
				type.DefineDateStep(TypeEditorDateTime.ToDateStep(this.fieldDateStep));
			}

			if (sender == this.fieldTimeStep)
			{
				type.DefineTimeStep(TypeEditorDateTime.ToTimeSpan(this.fieldTimeStep));
			}

			//	[Note1] Cet appel va provoquer le ResourceAccess.SetField.
			this.OnContentChanged();
		}


		protected TextFieldCombo				fieldResol;

		protected Widget						groupMinDate;
		protected TextField						fieldMinDate;
		protected Widget						groupMaxDate;
		protected TextField						fieldMaxDate;
		protected Widget						groupDateStep;
		protected TextField						fieldDateStep;

		protected Widget						groupMinTime;
		protected TextField						fieldMinTime;
		protected Widget						groupMaxTime;
		protected TextField						fieldMaxTime;
		protected Widget						groupTimeStep;
		protected TextField						fieldTimeStep;
	}
}
