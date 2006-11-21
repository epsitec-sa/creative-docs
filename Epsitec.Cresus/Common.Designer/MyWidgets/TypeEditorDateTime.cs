using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.MyWidgets
{
	/// <summary>
	/// Widget permettant d'�diter un Caption.Type.
	/// </summary>
	public class TypeEditorDateTime : AbstractTypeEditor
	{
		public TypeEditorDateTime()
		{
			Widget group;

			Widget band = new Widget(this);
			band.TabIndex = this.tabIndex++;
			band.TabNavigationMode = TabNavigationMode.ForwardTabPassive;
			band.Dock = DockStyle.StackBegin;
			band.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;

			Widget left = new Widget(band);
			left.TabIndex = this.tabIndex++;
			left.TabNavigationMode = TabNavigationMode.ForwardTabPassive;
			left.Dock = DockStyle.Fill;

			Widget right = new Widget(band);
			right.TabIndex = this.tabIndex++;
			right.TabNavigationMode = TabNavigationMode.ForwardTabPassive;
			right.Dock = DockStyle.Fill;

			this.CreateComboLabeled("R�solution", left, out group, out this.fieldResol);
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

			//	Date, � gauche.
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
			this.groupDateStep.Margins = new Margins(0, 0, 0, 10);
			this.fieldDateStep.TextChanged += new EventHandler(this.HandleTextFieldChanged);

			//	Heure, � droite.
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

			//	Default et Sample, � gauche.
			this.CreateStringLabeled("Valeur par d�faut", left, out this.groupDefault, out this.fieldDefault);
			this.groupDefault.Dock = DockStyle.StackBegin;
			this.groupDefault.Margins = new Margins(0, 0, 0, 2);
			this.fieldDefault.TextChanged += new EventHandler(this.HandleTextFieldChanged);

			this.CreateStringLabeled("Exemple de valeur", left, out this.groupSample, out this.fieldSample);
			this.groupSample.Dock = DockStyle.StackBegin;
			this.groupSample.Margins = new Margins(0, 0, 0, 0);
			this.fieldSample.TextChanged += new EventHandler(this.HandleTextFieldChanged);
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

				this.fieldDefault.TextChanged -= new EventHandler(this.HandleTextFieldChanged);
				this.fieldSample.TextChanged -= new EventHandler(this.HandleTextFieldChanged);
			}
			
			base.Dispose(disposing);
		}


		public override string GetSummary()
		{
			//	Retourne le texte du r�sum�.
			System.Text.StringBuilder builder = new System.Text.StringBuilder();

			AbstractDateTimeType type = this.AbstractType as AbstractDateTimeType;

			if (type.Resolution != TimeResolution.Default)
			{
				builder.Append("R�solution = ");
				builder.Append(TypeEditorDateTime.TimeResolutionToString(type.Resolution));
			}

			if (type.MinimumDate != Date.Null)
			{
				this.PutSummaryLegend(builder, "Date min = ");
				builder.Append(TypeEditorDateTime.DateTimeToDateString(type.MinimumDate.ToDateTime()));
			}

			if (type.MaximumDate != Date.Null)
			{
				this.PutSummaryLegend(builder, "Date max = ");
				builder.Append(TypeEditorDateTime.DateTimeToDateString(type.MaximumDate.ToDateTime()));
			}

			if (type.MinimumTime != Time.Null)
			{
				this.PutSummaryLegend(builder, "Heure min = ");
				builder.Append(TypeEditorDateTime.DateTimeToTimeString(type.MinimumTime.ToDateTime()));
			}

			if (type.MaximumTime != Time.Null)
			{
				this.PutSummaryLegend(builder, "Heure max = ");
				builder.Append(TypeEditorDateTime.DateTimeToTimeString(type.MaximumTime.ToDateTime()));
			}

			this.PutSummaryLegend(builder, "Pas date = ");
			builder.Append(TypeEditorDateTime.DateStepToString(type.DateStep));

			this.PutSummaryLegend(builder, "Pas heure = ");
			builder.Append(TypeEditorDateTime.TimeSpanToString(type.TimeStep));

			this.PutSummaryDefaultAndSample(builder, type);

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
			//	Met � jour le contenu de l'�diteur.
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

			this.fieldResol.Text = TypeEditorDateTime.TimeResolutionToString(type.Resolution);

			TypeEditorDateTime.DateToField(this.fieldMinDate, type.MinimumDate);
			TypeEditorDateTime.DateToField(this.fieldMaxDate, type.MaximumDate);
			TypeEditorDateTime.TimeToField(this.fieldMinTime, type.MinimumTime);
			TypeEditorDateTime.TimeToField(this.fieldMaxTime, type.MaximumTime);

			TypeEditorDateTime.DateStepToField(this.fieldDateStep, type.DateStep);
			TypeEditorDateTime.TimeSpanToField(this.fieldTimeStep, type.TimeStep);

			this.ObjectToField(this.fieldDefault, type.DefaultValue);
			this.ObjectToField(this.fieldSample, type.SampleValue);

			this.ignoreChange = false;
		}


		protected static TimeResolution StringToTimeResolution(string text)
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

		protected static string TimeResolutionToString(TimeResolution resol)
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

		protected void ObjectToField(TextField field, object value)
		{
			if (value == null)
			{
				field.Text = "";
			}
			else
			{
				AbstractDateTimeType type = this.AbstractType as AbstractDateTimeType;
				System.DateTime dt = (System.DateTime) value;

				if (type is DateType)
				{
					field.Text = TypeEditorDateTime.DateTimeToDateString(dt);
				}
				else if (type is TimeType)
				{
					field.Text = TypeEditorDateTime.DateTimeToTimeString(dt);
				}
				else  // DateTime ?
				{
					field.Text = TypeEditorDateTime.DateTimeToDateTimeString(dt);
				}
			}
		}

		protected object FieldToObject(TextField field)
		{
			if (string.IsNullOrEmpty(field.Text))
			{
				return null;
			}
			else
			{
				AbstractDateTimeType type = this.AbstractType as AbstractDateTimeType;

				if (type is DateType)
				{
					return TypeEditorDateTime.FieldToDate(field);
				}
				else if (type is TimeType)
				{
					return TypeEditorDateTime.FieldToTime(field);
				}
				else  // DateTime ?
				{
					return TypeEditorDateTime.FieldToDateTime(field);
				}
			}
		}

		protected static void DateToField(TextField field, Date date)
		{
			if (date == Date.Null)
			{
				field.Text = "";
			}
			else
			{
				field.Text = TypeEditorDateTime.DateTimeToDateString(date.ToDateTime());
			}
		}

		protected static void TimeToField(TextField field, Time time)
		{
			if (time == Time.Null)
			{
				field.Text = "";
			}
			else
			{
				field.Text = TypeEditorDateTime.DateTimeToTimeString(time.ToDateTime());
			}
		}

		protected static Date FieldToDate(TextField field)
		{
			if (!string.IsNullOrEmpty(field.Text))
			{
				System.DateTime dt = TypeEditorDateTime.StringToDateTime(field.Text);
				if (dt != System.DateTime.MinValue)
				{
					return new Date(dt);
				}
			}

			return Date.Null;
		}

		protected static Time FieldToTime(TextField field)
		{
			if (!string.IsNullOrEmpty(field.Text))
			{
				System.DateTime dt = TypeEditorDateTime.StringToDateTime(field.Text);
				if (dt != System.DateTime.MinValue)
				{
					return new Time(dt);
				}
			}

			return Time.Null;
		}

		protected static System.DateTime FieldToDateTime(TextField field)
		{
			if (!string.IsNullOrEmpty(field.Text))
			{
				return TypeEditorDateTime.StringToDateTime(field.Text);
			}

			return System.DateTime.MinValue;
		}

		protected static void DateStepToField(TextField field, DateStep ds)
		{
			if (ds == new DateStep(0, 0, 0))
			{
				field.Text = "";
			}
			else
			{
				field.Text = TypeEditorDateTime.DateStepToString(ds);
			}
		}

		protected static DateStep FieldToDateStep(TextField field)
		{
			if (!string.IsNullOrEmpty(field.Text))
			{
				return TypeEditorDateTime.StringToDateStep(field.Text);
			}

			return new DateStep(0, 0, 0);
		}

		protected static void TimeSpanToField(TextField field, System.TimeSpan ts)
		{
			if (ts == System.TimeSpan.Zero)
			{
				field.Text = "";
			}
			else
			{
				field.Text = TypeEditorDateTime.TimeSpanToString(ts);
			}
		}

		protected static System.TimeSpan FieldToTimeSpan(TextField field)
		{
			if (!string.IsNullOrEmpty(field.Text))
			{
				System.TimeSpan ts = TypeEditorDateTime.StringToTimeSpan(field.Text);
				if (ts != System.TimeSpan.Zero)
				{
					return ts;
				}
			}

			return System.TimeSpan.Zero;
		}

		protected static string DateTimeToDateString(System.DateTime dt)
		{
			//	(d) Short date: 4/17/2006
			return dt.ToString("d", System.Globalization.CultureInfo.CurrentCulture);
		}

		protected static string DateTimeToTimeString(System.DateTime dt)
		{
			//	(T) Long time: 14:22:48
			return dt.ToString("T", System.Globalization.CultureInfo.CurrentCulture);
		}

		protected static string DateTimeToDateTimeString(System.DateTime dt)
		{
			//	(G) General date/long time: 17.04.2006 14:22:48
			return dt.ToString("G", System.Globalization.CultureInfo.CurrentCulture);
		}

		protected static System.DateTime StringToDateTime(string text)
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

		protected static string DateStepToString(DateStep ds)
		{
			return ds.ToString();
		}

		protected static DateStep StringToDateStep(string text)
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

		protected static string TimeSpanToString(System.TimeSpan ts)
		{
			return ts.ToString();
		}

		protected static System.TimeSpan StringToTimeSpan(string text)
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
				type.DefineResolution(TypeEditorDateTime.StringToTimeResolution(this.fieldResol.Text));
			}

			if (sender == this.fieldMinDate)
			{
				type.DefineMinimumDate(TypeEditorDateTime.FieldToDate(this.fieldMinDate));
			}

			if (sender == this.fieldMaxDate)
			{
				type.DefineMaximumDate(TypeEditorDateTime.FieldToDate(this.fieldMaxDate));
			}

			if (sender == this.fieldMinTime)
			{
				type.DefineMinimumTime(TypeEditorDateTime.FieldToTime(this.fieldMinTime));
			}

			if (sender == this.fieldMaxTime)
			{
				type.DefineMaximumTime(TypeEditorDateTime.FieldToTime(this.fieldMaxTime));
			}

			if (sender == this.fieldDateStep)
			{
				type.DefineDateStep(TypeEditorDateTime.FieldToDateStep(this.fieldDateStep));
			}

			if (sender == this.fieldTimeStep)
			{
				type.DefineTimeStep(TypeEditorDateTime.FieldToTimeSpan(this.fieldTimeStep));
			}

			if (sender == this.fieldDefault)
			{
				type.DefineDefaultValue(this.FieldToObject(this.fieldDefault));
			}

			if (sender == this.fieldSample)
			{
				type.DefineSampleValue(this.FieldToObject(this.fieldSample));
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

		protected Widget						groupDefault;
		protected TextField						fieldDefault;
		protected Widget						groupSample;
		protected TextField						fieldSample;
	}
}
