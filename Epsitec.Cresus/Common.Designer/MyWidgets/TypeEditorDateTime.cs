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
			this.groupResol.ResetButton.Clicked += this.HandleResetButtonClicked;
			this.fieldResol.TextChanged += this.HandleTextFieldChanged;
			this.fieldResol.Items.Add(Res.Strings.Viewers.Types.DateTime.Res.Milliseconds);
			this.fieldResol.Items.Add(Res.Strings.Viewers.Types.DateTime.Res.Seconds);
			this.fieldResol.Items.Add(Res.Strings.Viewers.Types.DateTime.Res.Minutes);
			this.fieldResol.Items.Add(Res.Strings.Viewers.Types.DateTime.Res.Hours);
			this.fieldResol.Items.Add(Res.Strings.Viewers.Types.DateTime.Res.Days);
			this.fieldResol.Items.Add(Res.Strings.Viewers.Types.DateTime.Res.Weeks);
			this.fieldResol.Items.Add(Res.Strings.Viewers.Types.DateTime.Res.Months);
			this.fieldResol.Items.Add(Res.Strings.Viewers.Types.DateTime.Res.Years);

			//	Valeur par défaut.
			this.CreateDecimalLabeled(Res.Strings.Viewers.Types.DateTime.Default, right, out this.groupDefault, out this.fieldDefault);
			this.groupDefault.Dock = DockStyle.StackBegin;
			this.groupDefault.Margins = new Margins(0, 0, 0, 10);
			this.groupDefault.ResetButton.Clicked += this.HandleResetButtonClicked;
			this.fieldDefault.EditionAccepted += this.HandleTextFieldChanged;

			//	Date, à gauche.
			this.CreateStringLabeled(Res.Strings.Viewers.Types.DateTime.DateMin, left, out this.groupMinDate, out this.fieldMinDate);
			this.groupMinDate.Dock = DockStyle.StackBegin;
			this.groupMinDate.Margins = new Margins(0, 0, 0, 2);
			this.groupMinDate.ResetButton.Clicked += this.HandleResetButtonClicked;
			this.fieldMinDate.EditionAccepted += this.HandleTextFieldChanged;

			this.CreateStringLabeled(Res.Strings.Viewers.Types.DateTime.DateMax, left, out this.groupMaxDate, out this.fieldMaxDate);
			this.groupMaxDate.Dock = DockStyle.StackBegin;
			this.groupMaxDate.Margins = new Margins(0, 0, 0, 10);
			this.groupMaxDate.ResetButton.Clicked += this.HandleResetButtonClicked;
			this.fieldMaxDate.EditionAccepted += this.HandleTextFieldChanged;

			this.CreateStringLabeled(Res.Strings.Viewers.Types.DateTime.DateStep, left, out this.groupDateStep, out this.fieldDateStep);
			this.groupDateStep.Dock = DockStyle.StackBegin;
			this.groupDateStep.Margins = new Margins(0, 0, 0, 10);
			this.groupDateStep.ResetButton.Clicked += this.HandleResetButtonClicked;
			this.fieldDateStep.EditionAccepted += this.HandleTextFieldChanged;

			//	Heure, à droite.
			this.CreateStringLabeled(Res.Strings.Viewers.Types.DateTime.TimeMin, right, out this.groupMinTime, out this.fieldMinTime);
			this.groupMinTime.Dock = DockStyle.StackBegin;
			this.groupMinTime.Margins = new Margins(0, 0, 20+10, 2);
			this.groupMinTime.ResetButton.Clicked += this.HandleResetButtonClicked;
			this.fieldMinTime.EditionAccepted += this.HandleTextFieldChanged;

			this.CreateStringLabeled(Res.Strings.Viewers.Types.DateTime.TimeMax, right, out this.groupMaxTime, out this.fieldMaxTime);
			this.groupMaxTime.Dock = DockStyle.StackBegin;
			this.groupMaxTime.Margins = new Margins(0, 0, 0, 10);
			this.groupMaxTime.ResetButton.Clicked += this.HandleResetButtonClicked;
			this.fieldMaxTime.EditionAccepted += this.HandleTextFieldChanged;

			this.CreateStringLabeled(Res.Strings.Viewers.Types.DateTime.TimeStep, right, out this.groupTimeStep, out this.fieldTimeStep);
			this.groupTimeStep.Dock = DockStyle.StackBegin;
			this.groupTimeStep.Margins = new Margins(0, 0, 0, 10);
			this.groupTimeStep.ResetButton.Clicked += this.HandleResetButtonClicked;
			this.fieldTimeStep.EditionAccepted += this.HandleTextFieldChanged;
		}

		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.fieldResol.TextChanged -= this.HandleTextFieldChanged;

				this.groupMinDate.ResetButton.Clicked -= this.HandleResetButtonClicked;
				this.groupMaxDate.ResetButton.Clicked -= this.HandleResetButtonClicked;
				this.groupDateStep.ResetButton.Clicked -= this.HandleResetButtonClicked;
				this.groupMinTime.ResetButton.Clicked -= this.HandleResetButtonClicked;
				this.groupMaxTime.ResetButton.Clicked -= this.HandleResetButtonClicked;
				this.groupTimeStep.ResetButton.Clicked -= this.HandleResetButtonClicked;
				this.groupDefault.ResetButton.Clicked -= this.HandleResetButtonClicked;

				this.fieldMinDate.EditionAccepted -= this.HandleTextFieldChanged;
				this.fieldMaxDate.EditionAccepted -= this.HandleTextFieldChanged;
				this.fieldDateStep.EditionAccepted -= this.HandleTextFieldChanged;
				this.fieldMinTime.EditionAccepted -= this.HandleTextFieldChanged;
				this.fieldMaxTime.EditionAccepted -= this.HandleTextFieldChanged;
				this.fieldTimeStep.EditionAccepted -= this.HandleTextFieldChanged;
				this.fieldDefault.EditionAccepted -= this.HandleTextFieldChanged;
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
				if (!date.IsNull)
				{
					this.PutSummaryValue(builder, Res.Strings.Viewers.Types.Summary.DateMin, TypeEditorDateTime.DateTimeToDateString(date.ToDateTime()));
				}
			}

			value = this.structuredData.GetValue(Support.Res.Fields.ResourceDateTimeType.MaximumDate);
			if (!UndefinedValue.IsUndefinedValue(value))
			{
				Date date = (Date) value;
				if (!date.IsNull)
				{
					this.PutSummaryValue(builder, Res.Strings.Viewers.Types.Summary.DateMax, TypeEditorDateTime.DateTimeToDateString(date.ToDateTime()));
				}
			}

			value = this.structuredData.GetValue(Support.Res.Fields.ResourceDateTimeType.MinimumTime);
			if (!UndefinedValue.IsUndefinedValue(value))
			{
				Time time = (Time) value;
				if (!time.IsNull)
				{
					this.PutSummaryValue(builder, Res.Strings.Viewers.Types.Summary.TimeMin, TypeEditorDateTime.DateTimeToTimeString(time.ToDateTime()));
				}
			}

			value = this.structuredData.GetValue(Support.Res.Fields.ResourceDateTimeType.MaximumTime);
			if (!UndefinedValue.IsUndefinedValue(value))
			{
				Time time = (Time) value;
				if (!time.IsNull)
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

			value = this.structuredData.GetValue(Support.Res.Fields.ResourceBaseType.DefaultValue);
			if (!UndefinedValue.IsUndefinedValue(value))
			{
				this.PutSummaryValue(builder, Res.Strings.Viewers.Types.Summary.Default, this.TypeToString(value));
			}

			return builder.ToString();
		}

		protected string GetString(object value)
		{
			if (this.typeCode == TypeCode.Date)
			{
				Date date = (Date) value;
				return TypeEditorDateTime.DateTimeToDateString(date.ToDateTime());
			}

			if (this.typeCode == TypeCode.Time)
			{
				Time time = (Time) value;
				return TypeEditorDateTime.DateTimeToTimeString(time.ToDateTime());
			}

			if (this.typeCode == TypeCode.DateTime)
			{
				System.DateTime dt = (System.DateTime) value;
				return TypeEditorDateTime.DateTimeToDateTimeString(dt);
			}

			return null;
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

			CultureMapSource source = this.module.AccessTypes.GetCultureMapSource(this.cultureMap);

			value = this.structuredData.GetValue(Support.Res.Fields.ResourceDateTimeType.Resolution, out usesOriginalData);
			this.ColorizeResetBox(this.groupResol, source, usesOriginalData);
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
			this.ColorizeResetBox(this.groupMinDate, source, usesOriginalData);
			if (UndefinedValue.IsUndefinedValue(value))
			{
				this.fieldMinDate.Text = "";
			}
			else
			{
				TypeEditorDateTime.DateToField(this.fieldMinDate, (Date) value);
			}

			value = this.structuredData.GetValue(Support.Res.Fields.ResourceDateTimeType.MaximumDate, out usesOriginalData);
			this.ColorizeResetBox(this.groupMaxDate, source, usesOriginalData);
			if (UndefinedValue.IsUndefinedValue(value))
			{
				this.fieldMaxDate.Text = "";
			}
			else
			{
				TypeEditorDateTime.DateToField(this.fieldMaxDate, (Date) value);
			}

			value = this.structuredData.GetValue(Support.Res.Fields.ResourceDateTimeType.MinimumTime, out usesOriginalData);
			this.ColorizeResetBox(this.groupMinTime, source, usesOriginalData);
			if (UndefinedValue.IsUndefinedValue(value))
			{
				this.fieldMinTime.Text = "";
			}
			else
			{
				TypeEditorDateTime.TimeToField(this.fieldMinTime, (Time) value);
			}

			value = this.structuredData.GetValue(Support.Res.Fields.ResourceDateTimeType.MaximumTime, out usesOriginalData);
			this.ColorizeResetBox(this.groupMaxTime, source, usesOriginalData);
			if (UndefinedValue.IsUndefinedValue(value))
			{
				this.fieldMaxTime.Text = "";
			}
			else
			{
				TypeEditorDateTime.TimeToField(this.fieldMaxTime, (Time) value);
			}

			value = this.structuredData.GetValue(Support.Res.Fields.ResourceDateTimeType.DateStep, out usesOriginalData);
			this.ColorizeResetBox(this.groupDateStep, source, usesOriginalData);
			if (UndefinedValue.IsUndefinedValue(value))
			{
				this.fieldDateStep.Text = "";
			}
			else
			{
				TypeEditorDateTime.DateStepToField(this.fieldDateStep, (DateSpan) value);
			}

			value = this.structuredData.GetValue(Support.Res.Fields.ResourceDateTimeType.TimeStep, out usesOriginalData);
			this.ColorizeResetBox(this.groupTimeStep, source, usesOriginalData);
			if (UndefinedValue.IsUndefinedValue(value))
			{
				this.fieldTimeStep.Text = "";
			}
			else
			{
				TypeEditorDateTime.TimeSpanToField(this.fieldTimeStep, (System.TimeSpan) value);
			}

			value = this.structuredData.GetValue(Support.Res.Fields.ResourceBaseType.DefaultValue, out usesOriginalData);
			this.ColorizeResetBox(this.groupDefault, source, usesOriginalData);
			if (UndefinedValue.IsUndefinedValue(value))
			{
				this.fieldDefault.Text = "";
			}
			else
			{
				this.fieldDefault.Text = this.GetString(value);
			}

			this.ignoreChange = false;
		}


		protected static TimeResolution StringToTimeResolution(string text)
		{
			if (text == Res.Strings.Viewers.Types.DateTime.Res.Milliseconds)  return TimeResolution.Milliseconds;
			if (text == Res.Strings.Viewers.Types.DateTime.Res.Seconds)       return TimeResolution.Seconds;
			if (text == Res.Strings.Viewers.Types.DateTime.Res.Minutes)       return TimeResolution.Minutes;
			if (text == Res.Strings.Viewers.Types.DateTime.Res.Hours)         return TimeResolution.Hours;
			if (text == Res.Strings.Viewers.Types.DateTime.Res.Days)          return TimeResolution.Days;
			if (text == Res.Strings.Viewers.Types.DateTime.Res.Weeks)         return TimeResolution.Weeks;
			if (text == Res.Strings.Viewers.Types.DateTime.Res.Months)        return TimeResolution.Months;
			if (text == Res.Strings.Viewers.Types.DateTime.Res.Years)         return TimeResolution.Years;
			return TimeResolution.Default;
		}

		protected static string TimeResolutionToString(TimeResolution resol)
		{
			switch (resol)
			{
				case TimeResolution.Milliseconds:  return Res.Strings.Viewers.Types.DateTime.Res.Milliseconds;
				case TimeResolution.Seconds:       return Res.Strings.Viewers.Types.DateTime.Res.Seconds;
				case TimeResolution.Minutes:       return Res.Strings.Viewers.Types.DateTime.Res.Minutes;
				case TimeResolution.Hours:         return Res.Strings.Viewers.Types.DateTime.Res.Hours;
				case TimeResolution.Days:          return Res.Strings.Viewers.Types.DateTime.Res.Days;
				case TimeResolution.Weeks:         return Res.Strings.Viewers.Types.DateTime.Res.Weeks;
				case TimeResolution.Months:        return Res.Strings.Viewers.Types.DateTime.Res.Months;
				case TimeResolution.Years:         return Res.Strings.Viewers.Types.DateTime.Res.Years;
				default:                           return "";
			}
		}


		protected static void DateToField(TextFieldEx field, Date date)
		{
			if (date.IsNull)
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
			if (time.IsNull)
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
				System.DateTime? dt = TypeEditorDateTime.StringToDateTime(field.Text);
				if (dt.HasValue)
				{
					return new Date(dt.Value);
				}
			}

			return Date.Null;
		}

		protected static Time FieldToTime(TextFieldEx field)
		{
			if (!string.IsNullOrEmpty(field.Text))
			{
				System.TimeSpan? ts = TypeEditorDateTime.StringToTimeSpan(field.Text);
				if ((ts.HasValue) &&
					(ts.Value.TotalMilliseconds >= 0) &&
					(ts.Value.TotalDays < 1))
				{
					return new Time(ts.Value.Ticks);
				}
			}

			return Time.Null;
		}

		protected static System.DateTime? FieldToDateTime(TextFieldEx field)
		{
			if (!string.IsNullOrEmpty(field.Text))
			{
				return TypeEditorDateTime.StringToDateTime(field.Text);
			}

			return null;
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
				DateSpan? ds = TypeEditorDateTime.StringToDateStep(field.Text);
				if (ds.HasValue)
				{
					return ds.Value;
				}
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
				System.TimeSpan? ts = TypeEditorDateTime.StringToTimeSpan(field.Text);
				if (ts.HasValue)
				{
					return ts.Value;
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

		protected static System.DateTime? StringToDateTime(string text)
		{
			System.DateTime dt;
			if (System.DateTime.TryParse(text, System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.AssumeLocal|System.Globalization.DateTimeStyles.NoCurrentDateDefault, out dt))
			{
				return dt;
			}
			else
			{
				return null;
			}
		}

		protected static string DateStepToString(DateSpan ds)
		{
			return ds.ToString();
		}

		protected static DateSpan? StringToDateStep(string text)
		{
			try
			{
				return DateSpan.Parse(text);
			}
			catch
			{
				return null;
			}
		}

		protected static string TimeSpanToString(System.TimeSpan ts)
		{
			return ts.ToString();
		}

		protected static System.TimeSpan? StringToTimeSpan(string text)
		{
			System.TimeSpan ts;
			if (System.TimeSpan.TryParse(text, out ts))
			{
				return ts;
			}
			else
			{
				return null;
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

			if (sender == this.fieldDefault)
			{
				object value = UndefinedValue.Value;

				if (this.typeCode == TypeCode.Date)
				{
					Date def = TypeEditorDateTime.FieldToDate(this.fieldDefault);
					
					if (!def.IsNull)
					{
						value = def;
					}
				}
				else if (this.typeCode == TypeCode.Time)
				{
					Time def = TypeEditorDateTime.FieldToTime(this.fieldDefault);

					if (!def.IsNull)
					{
						value = def;
					}
				}
				else if (this.typeCode == TypeCode.DateTime)
				{
					System.DateTime? def = TypeEditorDateTime.FieldToDateTime(this.fieldDefault);

					if (def.HasValue)
					{
						value = def.Value;
					}
				}

				this.structuredData.SetValue (Support.Res.Fields.ResourceBaseType.DefaultValue, value);
			}

			this.OnContentChanged();
			this.UpdateContent();
			this.module.AccessTypes.SetLocalDirty();
		}

		private void HandleResetButtonClicked(object sender, MessageEventArgs e)
		{
			AbstractButton button = sender as AbstractButton;

			if (button == this.groupResol.ResetButton)
			{
				this.ResetToOriginalValue(Support.Res.Fields.ResourceDateTimeType.Resolution);
			}

			if (button == this.groupMinDate.ResetButton)
			{
				this.ResetToOriginalValue(Support.Res.Fields.ResourceDateTimeType.MinimumDate);
			}

			if (button == this.groupMaxDate.ResetButton)
			{
				this.ResetToOriginalValue(Support.Res.Fields.ResourceDateTimeType.MaximumDate);
			}

			if (button == this.groupDateStep.ResetButton)
			{
				this.ResetToOriginalValue(Support.Res.Fields.ResourceDateTimeType.DateStep);
			}

			if (button == this.groupMinTime.ResetButton)
			{
				this.ResetToOriginalValue(Support.Res.Fields.ResourceDateTimeType.MinimumTime);
			}

			if (button == this.groupMaxTime.ResetButton)
			{
				this.ResetToOriginalValue(Support.Res.Fields.ResourceDateTimeType.MaximumTime);
			}

			if (button == this.groupTimeStep.ResetButton)
			{
				this.ResetToOriginalValue(Support.Res.Fields.ResourceDateTimeType.TimeStep);
			}

			if (button == this.groupDefault.ResetButton)
			{
				this.ResetToOriginalValue(Support.Res.Fields.ResourceBaseType.DefaultValue);
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

		protected ResetBox						groupDefault;
		protected TextFieldEx					fieldDefault;
	}
}
