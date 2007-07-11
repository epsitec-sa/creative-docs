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

			this.CreateComboLabeled(Res.Strings.Viewers.Types.DateTime.Resolution, left, out group, out this.fieldResol);
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
			this.CreateStringLabeled(Res.Strings.Viewers.Types.DateTime.DateMin, left, out this.groupMinDate, out this.fieldMinDate);
			this.groupMinDate.Dock = DockStyle.StackBegin;
			this.groupMinDate.Margins = new Margins(0, 0, 0, 2);
			this.fieldMinDate.TextChanged += new EventHandler(this.HandleTextFieldChanged);

			this.CreateStringLabeled(Res.Strings.Viewers.Types.DateTime.DateMax, left, out this.groupMaxDate, out this.fieldMaxDate);
			this.groupMaxDate.Dock = DockStyle.StackBegin;
			this.groupMaxDate.Margins = new Margins(0, 0, 0, 10);
			this.fieldMaxDate.TextChanged += new EventHandler(this.HandleTextFieldChanged);

			this.CreateStringLabeled(Res.Strings.Viewers.Types.DateTime.DateStep, left, out this.groupDateStep, out this.fieldDateStep);
			this.groupDateStep.Dock = DockStyle.StackBegin;
			this.groupDateStep.Margins = new Margins(0, 0, 0, 10);
			this.fieldDateStep.TextChanged += new EventHandler(this.HandleTextFieldChanged);

			//	Heure, à droite.
			this.CreateStringLabeled(Res.Strings.Viewers.Types.DateTime.TimeMin, right, out this.groupMinTime, out this.fieldMinTime);
			this.groupMinTime.Dock = DockStyle.StackBegin;
			this.groupMinTime.Margins = new Margins(0, 0, 20+10, 2);
			this.fieldMinTime.TextChanged += new EventHandler(this.HandleTextFieldChanged);

			this.CreateStringLabeled(Res.Strings.Viewers.Types.DateTime.TimeMax, right, out this.groupMaxTime, out this.fieldMaxTime);
			this.groupMaxTime.Dock = DockStyle.StackBegin;
			this.groupMaxTime.Margins = new Margins(0, 0, 0, 10);
			this.fieldMaxTime.TextChanged += new EventHandler(this.HandleTextFieldChanged);

			this.CreateStringLabeled(Res.Strings.Viewers.Types.DateTime.TimeStep, right, out this.groupTimeStep, out this.fieldTimeStep);
			this.groupTimeStep.Dock = DockStyle.StackBegin;
			this.groupTimeStep.Margins = new Margins(0, 0, 0, 0);
			this.fieldTimeStep.TextChanged += new EventHandler(this.HandleTextFieldChanged);

			//	Default et Sample, à gauche.
			this.CreateStringLabeled(Res.Strings.Viewers.Types.DateTime.Default, left, out this.groupDefault, out this.fieldDefault);
			this.groupDefault.Dock = DockStyle.StackBegin;
			this.groupDefault.Margins = new Margins(0, 0, 0, 2);
			this.fieldDefault.TextChanged += new EventHandler(this.HandleTextFieldChanged);

			this.CreateStringLabeled(Res.Strings.Viewers.Types.DateTime.Sample, left, out this.groupSample, out this.fieldSample);
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
			//	Retourne le texte du résumé.
			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			this.PutSummaryInitialise();
#if true
			object value;

			value = this.structuredData.GetValue(Support.Res.Fields.ResourceDateTimeType.Resolution);
			if (!UndefinedValue.IsUndefinedValue(value))
			{
				TimeResolution res = (TimeResolution) value;
				if (res != TimeResolution.Default)
				{
					this.PutSummaryValue(builder, Res.Strings.Viewers.Types.Summary.Resolution, TypeEditorDateTime.TimeResolutionToString(res));
				}
			}

			value = this.structuredData.GetValue(Support.Res.Fields.ResourceDateTimeType.MinimumDate);
			if (!UndefinedValue.IsUndefinedValue(value))
			{
				Date date = (Date) value;
				if (date != Date.Null)
				{
					this.PutSummaryValue(builder, Res.Strings.Viewers.Types.Summary.DateMin, TypeEditorDateTime.DateTimeToDateString(date.ToDateTime()));
				}
			}

			value = this.structuredData.GetValue(Support.Res.Fields.ResourceDateTimeType.MaximumDate);
			if (!UndefinedValue.IsUndefinedValue(value))
			{
				Date date = (Date) value;
				if (date != Date.Null)
				{
					this.PutSummaryValue(builder, Res.Strings.Viewers.Types.Summary.DateMax, TypeEditorDateTime.DateTimeToDateString(date.ToDateTime()));
				}
			}

			value = this.structuredData.GetValue(Support.Res.Fields.ResourceDateTimeType.MinimumTime);
			if (!UndefinedValue.IsUndefinedValue(value))
			{
				Time time = (Time) value;
				if (time != Time.Null)
				{
					this.PutSummaryValue(builder, Res.Strings.Viewers.Types.Summary.TimeMin, TypeEditorDateTime.DateTimeToTimeString(time.ToDateTime()));
				}
			}

			value = this.structuredData.GetValue(Support.Res.Fields.ResourceDateTimeType.MaximumTime);
			if (!UndefinedValue.IsUndefinedValue(value))
			{
				Time time = (Time) value;
				if (time != Time.Null)
				{
					this.PutSummaryValue(builder, Res.Strings.Viewers.Types.Summary.TimeMax, TypeEditorDateTime.DateTimeToTimeString(time.ToDateTime()));
				}
			}

			value = this.structuredData.GetValue(Support.Res.Fields.ResourceDateTimeType.DateStep);
			if (!UndefinedValue.IsUndefinedValue(value))
			{
				DateSpan step = (DateSpan) value;
				if (step != DateSpan.Zero)
				{
					this.PutSummaryValue(builder, Res.Strings.Viewers.Types.Summary.DateStep, TypeEditorDateTime.DateStepToString(step));
				}
			}

			value = this.structuredData.GetValue(Support.Res.Fields.ResourceDateTimeType.TimeStep);
			if (!UndefinedValue.IsUndefinedValue(value))
			{
				System.TimeSpan step = (System.TimeSpan) value;
				if (step != System.TimeSpan.Zero)
				{
					this.PutSummaryValue(builder, Res.Strings.Viewers.Types.Summary.TimeStep, TypeEditorDateTime.TimeSpanToString(step));
				}
			}

			this.PutSummaryDefaultAndSample(builder);

			return builder.ToString();
#else
			AbstractDateTimeType type = this.AbstractType as AbstractDateTimeType;

			if (type.Resolution != TimeResolution.Default)
			{
				this.PutSummaryValue(builder, Res.Strings.Viewers.Types.Summary.Resolution, TypeEditorDateTime.TimeResolutionToString(type.Resolution));
			}

			if (type.MinimumDate != Date.Null)
			{
				this.PutSummaryValue(builder, Res.Strings.Viewers.Types.Summary.DateMin, TypeEditorDateTime.DateTimeToDateString(type.MinimumDate.ToDateTime()));
			}

			if (type.MaximumDate != Date.Null)
			{
				this.PutSummaryValue(builder, Res.Strings.Viewers.Types.Summary.DateMax, TypeEditorDateTime.DateTimeToDateString(type.MaximumDate.ToDateTime()));
			}

			if (type.MinimumTime != Time.Null)
			{
				this.PutSummaryValue(builder, Res.Strings.Viewers.Types.Summary.TimeMin, TypeEditorDateTime.DateTimeToTimeString(type.MinimumTime.ToDateTime()));
			}

			if (type.MaximumTime != Time.Null)
			{
				this.PutSummaryValue(builder, Res.Strings.Viewers.Types.Summary.TimeMax, TypeEditorDateTime.DateTimeToTimeString(type.MaximumTime.ToDateTime()));
			}

			this.PutSummaryValue(builder, Res.Strings.Viewers.Types.Summary.DateStep, TypeEditorDateTime.DateStepToString(type.DateStep));
			this.PutSummaryValue(builder, Res.Strings.Viewers.Types.Summary.TimeStep, TypeEditorDateTime.TimeSpanToString(type.TimeStep));

			this.PutSummaryDefaultAndSample(builder, type);

			return builder.ToString();
#endif
		}

		protected override string TypeToString(object value)
		{
			if (value is Date)
			{
				Date date = (Date) value;
				return TypeEditorDateTime.DateTimeToDateString(date.ToDateTime());
			}
			else if (value is Time)
			{
				Time time = (Time) value;
				return TypeEditorDateTime.DateTimeToTimeString(time.ToDateTime());
			}
			else
			{
				System.DateTime dt = (System.DateTime) value;
				return TypeEditorDateTime.DateTimeToDateTimeString(dt);
			}
		}


		protected override void UpdateContent()
		{
			//	Met à jour le contenu de l'éditeur.
#if true
			this.ignoreChange = true;
			object value;

			bool showDate = true;
			bool showTime = true;

			if (this.typeCode == TypeCode.Date)
			{
				showTime = false;
			}

			if (this.typeCode == TypeCode.Time)
			{
				showDate = false;
			}

			this.groupMinDate.Visibility = showDate;
			this.groupMaxDate.Visibility = showDate;
			this.groupDateStep.Visibility = showDate;

			this.groupMinTime.Visibility = showTime;
			this.groupMaxTime.Visibility = showTime;
			this.groupTimeStep.Visibility = showTime;

			value = this.structuredData.GetValue(Support.Res.Fields.ResourceDateTimeType.Resolution);
			if (UndefinedValue.IsUndefinedValue(value))
			{
				this.fieldResol.Text = "";
			}
			else
			{
				TimeResolution res = (TimeResolution) value;
				if (res == TimeResolution.Default)
				{
					this.fieldResol.Text = "";
				}
				else
				{
					this.fieldResol.Text = TypeEditorDateTime.TimeResolutionToString(res);
				}
			}

			value = this.structuredData.GetValue(Support.Res.Fields.ResourceDateTimeType.MinimumDate);
			if (UndefinedValue.IsUndefinedValue(value))
			{
				this.fieldMinDate.Text = "";
			}
			else
			{
				TypeEditorDateTime.DateToField(this.fieldMinDate, (Date) value);
			}

			value = this.structuredData.GetValue(Support.Res.Fields.ResourceDateTimeType.MaximumDate);
			if (UndefinedValue.IsUndefinedValue(value))
			{
				this.fieldMaxDate.Text = "";
			}
			else
			{
				TypeEditorDateTime.DateToField(this.fieldMaxDate, (Date) value);
			}

			value = this.structuredData.GetValue(Support.Res.Fields.ResourceDateTimeType.MinimumTime);
			if (UndefinedValue.IsUndefinedValue(value))
			{
				this.fieldMinTime.Text = "";
			}
			else
			{
				TypeEditorDateTime.TimeToField(this.fieldMinTime, (Time) value);
			}

			value = this.structuredData.GetValue(Support.Res.Fields.ResourceDateTimeType.MaximumTime);
			if (UndefinedValue.IsUndefinedValue(value))
			{
				this.fieldMaxTime.Text = "";
			}
			else
			{
				TypeEditorDateTime.TimeToField(this.fieldMaxTime, (Time) value);
			}

			value = this.structuredData.GetValue(Support.Res.Fields.ResourceDateTimeType.DateStep);
			if (UndefinedValue.IsUndefinedValue(value))
			{
				this.fieldDateStep.Text = "";
			}
			else
			{
				TypeEditorDateTime.DateStepToField(this.fieldDateStep, (DateSpan) value);
			}

			value = this.structuredData.GetValue(Support.Res.Fields.ResourceDateTimeType.TimeStep);
			if (UndefinedValue.IsUndefinedValue(value))
			{
				this.fieldTimeStep.Text = "";
			}
			else
			{
				TypeEditorDateTime.TimeSpanToField(this.fieldTimeStep, (System.TimeSpan) value);
			}

			this.ignoreChange = false;
#else
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
#endif
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

				if (type is DateType)
				{
					Date date = (Date) value;
					field.Text = TypeEditorDateTime.DateTimeToDateString(date.ToDateTime());
				}
				else if (type is TimeType)
				{
					Time time = (Time) value;
					field.Text = TypeEditorDateTime.DateTimeToTimeString(time.ToDateTime());
				}
				else  // DateTime ?
				{
					System.DateTime dt = (System.DateTime) value;
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

		protected static void DateStepToField(TextField field, DateSpan ds)
		{
			if (ds == DateSpan.Zero)
			{
				field.Text = "";
			}
			else
			{
				field.Text = TypeEditorDateTime.DateStepToString(ds);
			}
		}

		protected static DateSpan FieldToDateStep(TextField field)
		{
			if (!string.IsNullOrEmpty(field.Text))
			{
				return TypeEditorDateTime.StringToDateStep(field.Text);
			}

			return DateSpan.Zero;
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

		protected static string DateStepToString(DateSpan ds)
		{
			return ds.ToString();
		}

		protected static DateSpan StringToDateStep(string text)
		{
			try
			{
				return DateSpan.Parse(text);
			}
			catch
			{
				return DateSpan.Zero;
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

#if true
			if (sender == this.fieldResol)
			{
				TimeResolution res = TypeEditorDateTime.StringToTimeResolution(this.fieldResol.Text);
				this.structuredData.SetValue(Support.Res.Fields.ResourceDateTimeType.Resolution, res);
			}

			if (sender == this.fieldMinDate)
			{
				Date minDate = TypeEditorDateTime.FieldToDate(this.fieldMinDate);
				this.structuredData.SetValue(Support.Res.Fields.ResourceDateTimeType.MinimumDate, minDate);
			}

			if (sender == this.fieldMaxDate)
			{
				Date maxDate = TypeEditorDateTime.FieldToDate(this.fieldMaxDate);
				this.structuredData.SetValue(Support.Res.Fields.ResourceDateTimeType.MaximumDate, maxDate);
			}

			if (sender == this.fieldMinTime)
			{
				Time minTime = TypeEditorDateTime.FieldToTime(this.fieldMinTime);
				this.structuredData.SetValue(Support.Res.Fields.ResourceDateTimeType.MinimumTime, minTime);
			}

			if (sender == this.fieldMaxTime)
			{
				Time maxTime = TypeEditorDateTime.FieldToTime(this.fieldMaxTime);
				this.structuredData.SetValue(Support.Res.Fields.ResourceDateTimeType.MaximumTime, maxTime);
			}

			if (sender == this.fieldDateStep)
			{
				DateSpan dateSpan = TypeEditorDateTime.FieldToDateStep(this.fieldDateStep);
				this.structuredData.SetValue(Support.Res.Fields.ResourceDateTimeType.DateStep, dateSpan);
			}

			if (sender == this.fieldTimeStep)
			{
				System.TimeSpan timeSpan = TypeEditorDateTime.FieldToTimeSpan(this.fieldTimeStep);
				this.structuredData.SetValue(Support.Res.Fields.ResourceDateTimeType.TimeStep, timeSpan);
			}

			if (sender == this.fieldDefault)
			{
				//	TODO:
			}

			if (sender == this.fieldSample)
			{
				//	TODO:
			}
			
			this.OnContentChanged();
			this.UpdateContent();
			this.module.AccessTypes2.SetLocalDirty();
#else
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
#endif
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
