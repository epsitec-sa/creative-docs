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
		public TypeEditorDateTime(Module module)
		{
			this.module = module;

			FrameBox band = new FrameBox(this);
			band.TabIndex = this.tabIndex++;
			band.Dock = DockStyle.StackBegin;
			band.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;

			FrameBox left = new FrameBox(band);
			left.TabIndex = this.tabIndex++;
			left.Dock = DockStyle.Fill;

			FrameBox right = new FrameBox(band);
			right.TabIndex = this.tabIndex++;
			right.Dock = DockStyle.Fill;

			this.CreateComboLabeled(Res.Strings.Viewers.Types.DateTime.Resolution, left, out this.groupResol, out this.fieldResol);
			this.groupResol.Dock = DockStyle.StackBegin;
			this.groupResol.Margins = new Margins(0, 0, 0, 10);
			this.groupResol.ResetButton.Name = "Resol";
			this.groupResol.ResetButton.Clicked += new MessageEventHandler(this.HandleResetButtonClicked);
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
			this.groupMinDate.ResetButton.Name = "MinDate";
			this.groupMinDate.ResetButton.Clicked += new MessageEventHandler(this.HandleResetButtonClicked);
			this.fieldMinDate.EditionAccepted += new EventHandler(this.HandleTextFieldChanged);

			this.CreateStringLabeled(Res.Strings.Viewers.Types.DateTime.DateMax, left, out this.groupMaxDate, out this.fieldMaxDate);
			this.groupMaxDate.Dock = DockStyle.StackBegin;
			this.groupMaxDate.Margins = new Margins(0, 0, 0, 10);
			this.groupMaxDate.ResetButton.Name = "MaxDate";
			this.groupMaxDate.ResetButton.Clicked += new MessageEventHandler(this.HandleResetButtonClicked);
			this.fieldMaxDate.EditionAccepted += new EventHandler(this.HandleTextFieldChanged);

			this.CreateStringLabeled(Res.Strings.Viewers.Types.DateTime.DateStep, left, out this.groupDateStep, out this.fieldDateStep);
			this.groupDateStep.Dock = DockStyle.StackBegin;
			this.groupDateStep.Margins = new Margins(0, 0, 0, 10);
			this.groupDateStep.ResetButton.Name = "DateStep";
			this.groupDateStep.ResetButton.Clicked += new MessageEventHandler(this.HandleResetButtonClicked);
			this.fieldDateStep.EditionAccepted += new EventHandler(this.HandleTextFieldChanged);

			//	Heure, à droite.
			this.CreateStringLabeled(Res.Strings.Viewers.Types.DateTime.TimeMin, right, out this.groupMinTime, out this.fieldMinTime);
			this.groupMinTime.Dock = DockStyle.StackBegin;
			this.groupMinTime.Margins = new Margins(0, 0, 20+10, 2);
			this.groupMinTime.ResetButton.Name = "MinTime";
			this.groupMinTime.ResetButton.Clicked += new MessageEventHandler(this.HandleResetButtonClicked);
			this.fieldMinTime.EditionAccepted += new EventHandler(this.HandleTextFieldChanged);

			this.CreateStringLabeled(Res.Strings.Viewers.Types.DateTime.TimeMax, right, out this.groupMaxTime, out this.fieldMaxTime);
			this.groupMaxTime.Dock = DockStyle.StackBegin;
			this.groupMaxTime.Margins = new Margins(0, 0, 0, 10);
			this.groupMaxTime.ResetButton.Name = "MaxTime";
			this.groupMaxTime.ResetButton.Clicked += new MessageEventHandler(this.HandleResetButtonClicked);
			this.fieldMaxTime.EditionAccepted += new EventHandler(this.HandleTextFieldChanged);

			this.CreateStringLabeled(Res.Strings.Viewers.Types.DateTime.TimeStep, right, out this.groupTimeStep, out this.fieldTimeStep);
			this.groupTimeStep.Dock = DockStyle.StackBegin;
			this.groupTimeStep.Margins = new Margins(0, 0, 0, 10);
			this.groupTimeStep.ResetButton.Name = "TimeStep";
			this.groupTimeStep.ResetButton.Clicked += new MessageEventHandler(this.HandleResetButtonClicked);
			this.fieldTimeStep.EditionAccepted += new EventHandler(this.HandleTextFieldChanged);
		}

		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.fieldResol.TextChanged -= new EventHandler(this.HandleTextFieldChanged);

				this.groupMinDate.ResetButton.Clicked -= new MessageEventHandler(this.HandleResetButtonClicked);
				this.groupMaxDate.ResetButton.Clicked -= new MessageEventHandler(this.HandleResetButtonClicked);
				this.groupDateStep.ResetButton.Clicked -= new MessageEventHandler(this.HandleResetButtonClicked);
				this.groupMinTime.ResetButton.Clicked -= new MessageEventHandler(this.HandleResetButtonClicked);
				this.groupMaxTime.ResetButton.Clicked -= new MessageEventHandler(this.HandleResetButtonClicked);
				this.groupTimeStep.ResetButton.Clicked -= new MessageEventHandler(this.HandleResetButtonClicked);

				this.fieldMinDate.EditionAccepted -= new EventHandler(this.HandleTextFieldChanged);
				this.fieldMaxDate.EditionAccepted -= new EventHandler(this.HandleTextFieldChanged);
				this.fieldDateStep.EditionAccepted -= new EventHandler(this.HandleTextFieldChanged);
				this.fieldMinTime.EditionAccepted -= new EventHandler(this.HandleTextFieldChanged);
				this.fieldMaxTime.EditionAccepted -= new EventHandler(this.HandleTextFieldChanged);
				this.fieldTimeStep.EditionAccepted -= new EventHandler(this.HandleTextFieldChanged);
			}
			
			base.Dispose(disposing);
		}


		public override string GetSummary()
		{
			//	Retourne le texte du résumé.
			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			this.PutSummaryInitialise();
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

			return builder.ToString();
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


		public override void UpdateContent()
		{
			//	Met à jour le contenu de l'éditeur.
			this.ignoreChange = true;
			object value;
			bool usesOriginalData;

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

			value = this.structuredData.GetValue(Support.Res.Fields.ResourceDateTimeType.Resolution, out usesOriginalData);
			Viewers.Abstract.ColorizeResetBox(this.groupResol, usesOriginalData);
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

			value = this.structuredData.GetValue(Support.Res.Fields.ResourceDateTimeType.MinimumDate, out usesOriginalData);
			Viewers.Abstract.ColorizeResetBox(this.groupMinDate, usesOriginalData);
			if (UndefinedValue.IsUndefinedValue(value))
			{
				this.fieldMinDate.Text = "";
			}
			else
			{
				TypeEditorDateTime.DateToField(this.fieldMinDate, (Date) value);
			}

			value = this.structuredData.GetValue(Support.Res.Fields.ResourceDateTimeType.MaximumDate, out usesOriginalData);
			Viewers.Abstract.ColorizeResetBox(this.groupMaxDate, usesOriginalData);
			if (UndefinedValue.IsUndefinedValue(value))
			{
				this.fieldMaxDate.Text = "";
			}
			else
			{
				TypeEditorDateTime.DateToField(this.fieldMaxDate, (Date) value);
			}

			value = this.structuredData.GetValue(Support.Res.Fields.ResourceDateTimeType.MinimumTime, out usesOriginalData);
			Viewers.Abstract.ColorizeResetBox(this.groupMinTime, usesOriginalData);
			if (UndefinedValue.IsUndefinedValue(value))
			{
				this.fieldMinTime.Text = "";
			}
			else
			{
				TypeEditorDateTime.TimeToField(this.fieldMinTime, (Time) value);
			}

			value = this.structuredData.GetValue(Support.Res.Fields.ResourceDateTimeType.MaximumTime, out usesOriginalData);
			Viewers.Abstract.ColorizeResetBox(this.groupMaxTime, usesOriginalData);
			if (UndefinedValue.IsUndefinedValue(value))
			{
				this.fieldMaxTime.Text = "";
			}
			else
			{
				TypeEditorDateTime.TimeToField(this.fieldMaxTime, (Time) value);
			}

			value = this.structuredData.GetValue(Support.Res.Fields.ResourceDateTimeType.DateStep, out usesOriginalData);
			Viewers.Abstract.ColorizeResetBox(this.groupDateStep, usesOriginalData);
			if (UndefinedValue.IsUndefinedValue(value))
			{
				this.fieldDateStep.Text = "";
			}
			else
			{
				TypeEditorDateTime.DateStepToField(this.fieldDateStep, (DateSpan) value);
			}

			value = this.structuredData.GetValue(Support.Res.Fields.ResourceDateTimeType.TimeStep, out usesOriginalData);
			Viewers.Abstract.ColorizeResetBox(this.groupTimeStep, usesOriginalData);
			if (UndefinedValue.IsUndefinedValue(value))
			{
				this.fieldTimeStep.Text = "";
			}
			else
			{
				TypeEditorDateTime.TimeSpanToField(this.fieldTimeStep, (System.TimeSpan) value);
			}

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


		protected static void DateToField(TextFieldEx field, Date date)
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

		protected static void TimeToField(TextFieldEx field, Time time)
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

		protected static Date FieldToDate(TextFieldEx field)
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

		protected static Time FieldToTime(TextFieldEx field)
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

		protected static System.DateTime FieldToDateTime(TextFieldEx field)
		{
			if (!string.IsNullOrEmpty(field.Text))
			{
				return TypeEditorDateTime.StringToDateTime(field.Text);
			}

			return System.DateTime.MinValue;
		}

		protected static void DateStepToField(TextFieldEx field, DateSpan ds)
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

		protected static DateSpan FieldToDateStep(TextFieldEx field)
		{
			if (!string.IsNullOrEmpty(field.Text))
			{
				return TypeEditorDateTime.StringToDateStep(field.Text);
			}

			return DateSpan.Zero;
		}

		protected static void TimeSpanToField(TextFieldEx field, System.TimeSpan ts)
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

		protected static System.TimeSpan FieldToTimeSpan(TextFieldEx field)
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

			this.OnContentChanged();
			this.UpdateContent();
			this.module.AccessTypes.SetLocalDirty();
		}

		private void HandleResetButtonClicked(object sender, MessageEventArgs e)
		{
			AbstractButton button = sender as AbstractButton;

			if (button.Name == "Resol")
			{
				this.ResetToOriginalValue(Support.Res.Fields.ResourceDateTimeType.Resolution);
			}

			if (button.Name == "MinDate")
			{
				this.ResetToOriginalValue(Support.Res.Fields.ResourceDateTimeType.MinimumDate);
			}

			if (button.Name == "MaxDate")
			{
				this.ResetToOriginalValue(Support.Res.Fields.ResourceDateTimeType.MaximumDate);
			}

			if (button.Name == "DateStep")
			{
				this.ResetToOriginalValue(Support.Res.Fields.ResourceDateTimeType.DateStep);
			}

			if (button.Name == "MinTime")
			{
				this.ResetToOriginalValue(Support.Res.Fields.ResourceDateTimeType.MinimumTime);
			}

			if (button.Name == "MaxTime")
			{
				this.ResetToOriginalValue(Support.Res.Fields.ResourceDateTimeType.MaximumTime);
			}

			if (button.Name == "TimeStep")
			{
				this.ResetToOriginalValue(Support.Res.Fields.ResourceDateTimeType.TimeStep);
			}

			this.OnContentChanged();
			this.UpdateContent();
			this.module.AccessTypes.SetLocalDirty();
		}


		protected ResetBox						groupResol;
		protected TextFieldCombo				fieldResol;

		protected ResetBox						groupMinDate;
		protected TextFieldEx					fieldMinDate;
		protected ResetBox						groupMaxDate;
		protected TextFieldEx					fieldMaxDate;
		protected ResetBox						groupDateStep;
		protected TextFieldEx					fieldDateStep;

		protected ResetBox						groupMinTime;
		protected TextFieldEx					fieldMinTime;
		protected ResetBox						groupMaxTime;
		protected TextFieldEx					fieldMaxTime;
		protected ResetBox						groupTimeStep;
		protected TextFieldEx					fieldTimeStep;
	}
}
