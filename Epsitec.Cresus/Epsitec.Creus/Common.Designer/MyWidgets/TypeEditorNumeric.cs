using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.MyWidgets
{
	/// <summary>
	/// Widget permettant d'éditer un Caption.Type.
	/// </summary>
	public class TypeEditorNumeric : AbstractTypeEditor
	{
		public TypeEditorNumeric(Module module)
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

			//	Range.
			this.CreateDecimalLabeled(Res.Strings.Viewers.Types.Numeric.Min, left, out this.groupMin, out this.fieldMin);
			this.groupMin.Dock = DockStyle.StackBegin;
			this.groupMin.Margins = new Margins(0, 0, 0, 2);
			this.groupMin.ResetButton.Clicked += this.HandleResetButtonClicked;
			this.fieldMin.EditionAccepted += this.HandleTextFieldChanged;

			this.CreateDecimalLabeled(Res.Strings.Viewers.Types.Numeric.Max, left, out this.groupMax, out this.fieldMax);
			this.groupMax.Dock = DockStyle.StackBegin;
			this.groupMax.Margins = new Margins(0, 0, 0, 2);
			this.groupMax.ResetButton.Clicked += this.HandleResetButtonClicked;
			this.fieldMax.EditionAccepted += this.HandleTextFieldChanged;

			this.CreateDecimalLabeled(Res.Strings.Viewers.Types.Numeric.Resol, left, out this.groupRes, out this.fieldRes);
			this.groupRes.Dock = DockStyle.StackBegin;
			this.groupRes.Margins = new Margins(0, 0, 0, 0);
			this.groupRes.ResetButton.Clicked += this.HandleResetButtonClicked;
			this.fieldRes.EditionAccepted += this.HandleTextFieldChanged;

			//	PreferredRange.
			this.CreateDecimalLabeled(Res.Strings.Viewers.Types.Numeric.PreferredMin, right, out this.groupPreferredMin, out this.fieldPreferredMin);
			this.groupPreferredMin.Dock = DockStyle.StackBegin;
			this.groupPreferredMin.Margins = new Margins(0, 0, 0, 2);
			this.groupPreferredMin.ResetButton.Clicked += this.HandleResetButtonClicked;
			this.fieldPreferredMin.EditionAccepted += this.HandleTextFieldChanged;

			this.CreateDecimalLabeled(Res.Strings.Viewers.Types.Numeric.PreferredMax, right, out this.groupPreferredMax, out this.fieldPreferredMax);
			this.groupPreferredMax.Dock = DockStyle.StackBegin;
			this.groupPreferredMax.Margins = new Margins(0, 0, 0, 2);
			this.groupPreferredMax.ResetButton.Clicked += this.HandleResetButtonClicked;
			this.fieldPreferredMax.EditionAccepted += this.HandleTextFieldChanged;

			this.CreateDecimalLabeled(Res.Strings.Viewers.Types.Numeric.PreferredResol, right, out this.groupPreferredRes, out this.fieldPreferredRes);
			this.groupPreferredRes.Dock = DockStyle.StackBegin;
			this.groupPreferredRes.Margins = new Margins(0, 0, 0, 0);
			this.groupPreferredRes.ResetButton.Clicked += this.HandleResetButtonClicked;
			this.fieldPreferredRes.EditionAccepted += this.HandleTextFieldChanged;

			//	Steps.
			this.CreateDecimalLabeled(Res.Strings.Viewers.Types.Numeric.SmallStep, left, out this.groupSmallStep, out this.fieldSmallStep);
			this.groupSmallStep.Dock = DockStyle.StackBegin;
			this.groupSmallStep.Margins = new Margins(0, 0, 10, 2);
			this.groupSmallStep.ResetButton.Clicked += this.HandleResetButtonClicked;
			this.fieldSmallStep.EditionAccepted += this.HandleTextFieldChanged;

			this.CreateDecimalLabeled(Res.Strings.Viewers.Types.Numeric.LargeStep, left, out this.groupLargeStep, out this.fieldLargeStep);
			this.groupLargeStep.Dock = DockStyle.StackBegin;
			this.groupLargeStep.Margins = new Margins(0, 0, 0, 0);
			this.groupLargeStep.ResetButton.Clicked += this.HandleResetButtonClicked;
			this.fieldLargeStep.EditionAccepted += this.HandleTextFieldChanged;

			//	Valeur par défaut.
			this.CreateDecimalLabeled(Res.Strings.Viewers.Types.Numeric.Default, left, out this.groupDefault, out this.fieldDefault);
			this.groupDefault.Dock = DockStyle.StackBegin;
			this.groupDefault.Margins = new Margins(0, 0, 10, 0);
			this.groupDefault.ResetButton.Clicked += this.HandleResetButtonClicked;
			this.fieldDefault.EditionAccepted += this.HandleTextFieldChanged;
		}
		
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.groupMin.ResetButton.Clicked -= this.HandleResetButtonClicked;
				this.groupMax.ResetButton.Clicked -= this.HandleResetButtonClicked;
				this.groupRes.ResetButton.Clicked -= this.HandleResetButtonClicked;
				this.groupPreferredMin.ResetButton.Clicked -= this.HandleResetButtonClicked;
				this.groupPreferredMax.ResetButton.Clicked -= this.HandleResetButtonClicked;
				this.groupPreferredRes.ResetButton.Clicked -= this.HandleResetButtonClicked;
				this.groupSmallStep.ResetButton.Clicked -= this.HandleResetButtonClicked;
				this.groupLargeStep.ResetButton.Clicked -= this.HandleResetButtonClicked;
				this.groupDefault.ResetButton.Clicked -= this.HandleResetButtonClicked;
				
				this.fieldMin.EditionAccepted -= this.HandleTextFieldChanged;
				this.fieldMax.EditionAccepted -= this.HandleTextFieldChanged;
				this.fieldRes.EditionAccepted -= this.HandleTextFieldChanged;
				this.fieldPreferredMin.EditionAccepted -= this.HandleTextFieldChanged;
				this.fieldPreferredMax.EditionAccepted -= this.HandleTextFieldChanged;
				this.fieldPreferredRes.EditionAccepted -= this.HandleTextFieldChanged;
				this.fieldSmallStep.EditionAccepted -= this.HandleTextFieldChanged;
				this.fieldLargeStep.EditionAccepted -= this.HandleTextFieldChanged;
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

			value = this.structuredData.GetValue(Support.Res.Fields.ResourceNumericType.Range);
			if (!UndefinedValue.IsUndefinedValue(value))
			{
				DecimalRange range = (DecimalRange) value;
				if (!range.IsEmpty)
				{
					this.PutSummaryValue(builder, Res.Strings.Viewers.Types.Summary.Min, range.Minimum.ToString());
					this.PutSummaryValue(builder, Res.Strings.Viewers.Types.Summary.Max, range.Maximum.ToString());
					this.PutSummaryValue(builder, Res.Strings.Viewers.Types.Summary.Resolution, range.Resolution.ToString());
				}
			}

			value = this.structuredData.GetValue(Support.Res.Fields.ResourceNumericType.PreferredRange);
			if (!UndefinedValue.IsUndefinedValue(value))
			{
				DecimalRange range = (DecimalRange) value;
				if (!range.IsEmpty)
				{
					this.PutSummaryValue(builder, Res.Strings.Viewers.Types.Summary.Min, range.Minimum.ToString());
					this.PutSummaryValue(builder, Res.Strings.Viewers.Types.Summary.Max, range.Maximum.ToString());
					this.PutSummaryValue(builder, Res.Strings.Viewers.Types.Summary.Resolution, range.Resolution.ToString());
				}
			}

			value = this.structuredData.GetValue(Support.Res.Fields.ResourceNumericType.SmallStep);
			if (!UndefinedValue.IsUndefinedValue(value))
			{
				decimal step = (decimal) value;
				if (step != 0M)
				{
					this.PutSummaryValue(builder, Res.Strings.Viewers.Types.Summary.SmallStep, step.ToString());
				}
			}

			value = this.structuredData.GetValue(Support.Res.Fields.ResourceNumericType.LargeStep);
			if (!UndefinedValue.IsUndefinedValue(value))
			{
				decimal step = (decimal) value;
				if (step != 0M)
				{
					this.PutSummaryValue(builder, Res.Strings.Viewers.Types.Summary.BigStep, step.ToString());
				}
			}

			value = this.structuredData.GetValue(Support.Res.Fields.ResourceBaseType.DefaultValue);
			if (!UndefinedValue.IsUndefinedValue(value))
			{
				this.PutSummaryValue(builder, Res.Strings.Viewers.Types.Summary.Default, value.ToString());
			}

			return builder.ToString();
		}


		public override void UpdateContent()
		{
			//	Met à jour le contenu de l'éditeur.
			this.ignoreChange = true;
			object value;
			bool usesOriginalData;

			CultureMapSource source = this.module.AccessTypes.GetCultureMapSource(this.cultureMap);

			value = this.structuredData.GetValue(Support.Res.Fields.ResourceNumericType.Range, out usesOriginalData);
			this.ColorizeResetBox(this.groupMin, source, usesOriginalData);
			this.ColorizeResetBox(this.groupMax, source, usesOriginalData);
			this.ColorizeResetBox(this.groupRes, source, usesOriginalData);
			if (UndefinedValue.IsUndefinedValue(value))
			{
				this.fieldMin.Text = "";
				this.fieldMax.Text = "";
				this.fieldRes.Text = "";
			}
			else
			{
				DecimalRange range = (DecimalRange) value;
				if (range.IsEmpty)
				{
					this.fieldMin.Text = "";
					this.fieldMax.Text = "";
					this.fieldRes.Text = "";
				}
				else
				{
					this.SetDecimal(this.fieldMin, range.Minimum);
					this.SetDecimal(this.fieldMax, range.Maximum);
					this.SetDecimal(this.fieldRes, range.Resolution);
				}
			}

			value = this.structuredData.GetValue(Support.Res.Fields.ResourceNumericType.PreferredRange, out usesOriginalData);
			this.ColorizeResetBox(this.groupPreferredMin, source, usesOriginalData);
			this.ColorizeResetBox(this.groupPreferredMax, source, usesOriginalData);
			this.ColorizeResetBox(this.groupPreferredRes, source, usesOriginalData);
			if (UndefinedValue.IsUndefinedValue(value))
			{
				this.fieldPreferredMin.Text = "";
				this.fieldPreferredMax.Text = "";
				this.fieldPreferredRes.Text = "";
			}
			else
			{
				DecimalRange range = (DecimalRange) value;
				if (range.IsEmpty)
				{
					this.fieldPreferredMin.Text = "";
					this.fieldPreferredMax.Text = "";
					this.fieldPreferredRes.Text = "";
				}
				else
				{
					this.SetDecimal(this.fieldPreferredMin, range.Minimum);
					this.SetDecimal(this.fieldPreferredMax, range.Maximum);
					this.SetDecimal(this.fieldPreferredRes, range.Resolution);
				}
			}

			value = this.structuredData.GetValue(Support.Res.Fields.ResourceNumericType.SmallStep, out usesOriginalData);
			this.ColorizeResetBox(this.groupSmallStep, source, usesOriginalData);
			if (UndefinedValue.IsUndefinedValue(value))
			{
				this.fieldSmallStep.Text = "";
			}
			else
			{
				decimal step = (decimal) value;
				if (step == 0M)
				{
					this.fieldSmallStep.Text = "";
				}
				else
				{
					this.SetDecimal(this.fieldSmallStep, step);
				}
			}

			value = this.structuredData.GetValue(Support.Res.Fields.ResourceNumericType.LargeStep, out usesOriginalData);
			this.ColorizeResetBox(this.groupLargeStep, source, usesOriginalData);
			if (UndefinedValue.IsUndefinedValue(value))
			{
				this.fieldLargeStep.Text = "";
			}
			else
			{
				decimal step = (decimal) value;
				if (step == 0M)
				{
					this.fieldLargeStep.Text = "";
				}
				else
				{
					this.SetDecimal(this.fieldLargeStep, step);
				}
			}
			
			value = this.structuredData.GetValue(Support.Res.Fields.ResourceBaseType.DefaultValue, out usesOriginalData);
			this.ColorizeResetBox(this.groupDefault, source, usesOriginalData);
			if (UndefinedValue.IsUndefinedValue(value))
			{
				this.fieldDefault.Text = "";
			}
			else
			{
				decimal def;
				switch (this.typeCode)
				{
					case TypeCode.Decimal:
						def = (decimal) value;
						break;
					case TypeCode.Double:
						def = (decimal) (double) value;
						break;
					case TypeCode.Integer:
						def = (decimal) (int) value;
						break;
					case TypeCode.LongInteger:
						def = (decimal) (long) value;
						break;
					default:
						throw new System.NotImplementedException ();
				}
				
				this.SetDecimal(this.fieldDefault, def);
			}
			
			this.ignoreChange = false;
		}


		private void HandleTextFieldChanged(object sender)
		{
			if (this.ignoreChange)
			{
				return;
			}

			//	Range.
			if (sender == this.fieldMin || sender == this.fieldMax || sender == this.fieldRes)
			{
				DecimalRange range = this.DefaultRange;
				object value = this.structuredData.GetValue(Support.Res.Fields.ResourceNumericType.Range);
				if (!UndefinedValue.IsUndefinedValue(value))
				{
					range = (DecimalRange) value;
					if (range.IsEmpty)
					{
						range = this.DefaultRange;
					}
				}

				if (sender == this.fieldMin)
				{
					range = new DecimalRange(this.GetDecimal(this.fieldMin), range.Maximum, range.Resolution);
				}

				if (sender == this.fieldMax)
				{
					range = new DecimalRange(range.Minimum, this.GetDecimal(this.fieldMax), range.Resolution);
				}

				if (sender == this.fieldRes)
				{
					range = new DecimalRange(range.Minimum, range.Maximum, this.GetDecimal(this.fieldRes));
				}

				this.structuredData.SetValue(Support.Res.Fields.ResourceNumericType.Range, range);
			}

			//	PreferredRange.
			if (sender == this.fieldPreferredMin || sender == this.fieldPreferredMax || sender == this.fieldPreferredRes)
			{
				DecimalRange preferredRange = this.DefaultRange;
				object value = this.structuredData.GetValue(Support.Res.Fields.ResourceNumericType.PreferredRange);
				if (!UndefinedValue.IsUndefinedValue(value))
				{
					preferredRange = (DecimalRange) value;
					if (preferredRange.IsEmpty)
					{
						preferredRange = this.DefaultRange;
					}
				}

				if (sender == this.fieldPreferredMin)
				{
					preferredRange = new DecimalRange(this.GetDecimal(this.fieldPreferredMin), preferredRange.Maximum, preferredRange.Resolution);
				}

				if (sender == this.fieldPreferredMax)
				{
					preferredRange = new DecimalRange(preferredRange.Minimum, this.GetDecimal(this.fieldPreferredMax), preferredRange.Resolution);
				}

				if (sender == this.fieldPreferredRes)
				{
					preferredRange = new DecimalRange(preferredRange.Minimum, preferredRange.Maximum, this.GetDecimal(this.fieldPreferredRes));
				}

				this.structuredData.SetValue(Support.Res.Fields.ResourceNumericType.PreferredRange, preferredRange);
			}

			//	Steps.
			if (sender == this.fieldSmallStep)
			{
				decimal smallStep = this.GetDecimal(this.fieldSmallStep);
				this.structuredData.SetValue(Support.Res.Fields.ResourceNumericType.SmallStep, smallStep);
			}

			if (sender == this.fieldLargeStep)
			{
				decimal largeStep = this.GetDecimal(this.fieldLargeStep);
				this.structuredData.SetValue(Support.Res.Fields.ResourceNumericType.LargeStep, largeStep);
			}

			if (sender == this.fieldDefault)
			{
				decimal? def = this.GetDecimalOrNull(this.fieldDefault);
				if (def.HasValue)
				{
					switch (this.typeCode)
					{
						case TypeCode.Decimal:
							this.structuredData.SetValue (Support.Res.Fields.ResourceBaseType.DefaultValue, def.Value);
							break;
						case TypeCode.Double:
							this.structuredData.SetValue (Support.Res.Fields.ResourceBaseType.DefaultValue, (double) def.Value);
							break;
						case TypeCode.Integer:
							this.structuredData.SetValue (Support.Res.Fields.ResourceBaseType.DefaultValue, (int) def.Value);
							break;
						case TypeCode.LongInteger:
							this.structuredData.SetValue (Support.Res.Fields.ResourceBaseType.DefaultValue, (long) def.Value);
							break;
						default:
							throw new System.NotImplementedException ();
					}
				}
				else
				{
					this.structuredData.SetValue (Support.Res.Fields.ResourceBaseType.DefaultValue, UndefinedValue.Value);
				}
			}

			this.OnContentChanged();
			this.UpdateContent();
			this.module.AccessTypes.SetLocalDirty();
		}

		protected DecimalRange DefaultRange
		{
			//	Retourne le range par défaut avec les valeurs min/max les plus grandes possibles.
			get
			{
				decimal min = decimal.MinValue;
				decimal max = decimal.MaxValue;
				decimal res = 0.01M;  // TODO: mettre une résolution plus grande ?

				if (this.typeCode == TypeCode.Integer)
				{
					min = (decimal) int.MinValue;
					max = (decimal) int.MaxValue;
					res = 1M;
				}

				if (this.typeCode == TypeCode.LongInteger)
				{
					min = (decimal) System.Int64.MinValue;
					max = (decimal) System.Int64.MaxValue;
					res = 1M;
				}

				if (this.typeCode == TypeCode.Double)
				{
					//min = (decimal) double.MinValue;  // trop grand !
					//max = (decimal) double.MaxValue;
					res = 0.01M;  // TODO: mettre une résolution plus grande ?
				}

				return new DecimalRange(min, max, res);
			}
		}

		private void HandleResetButtonClicked(object sender, MessageEventArgs e)
		{
			AbstractButton button = sender as AbstractButton;

			if (button == this.groupMin.ResetButton)
			{
				this.ResetToOriginalValue(Support.Res.Fields.ResourceNumericType.Range);
			}

			if (button == this.groupMax.ResetButton)
			{
				this.ResetToOriginalValue(Support.Res.Fields.ResourceNumericType.Range);
			}

			if (button == this.groupRes.ResetButton)
			{
				this.ResetToOriginalValue(Support.Res.Fields.ResourceNumericType.Range);
			}

			if (button == this.groupPreferredMin.ResetButton)
			{
				this.ResetToOriginalValue(Support.Res.Fields.ResourceNumericType.PreferredRange);
			}

			if (button == this.groupPreferredMax.ResetButton)
			{
				this.ResetToOriginalValue(Support.Res.Fields.ResourceNumericType.PreferredRange);
			}

			if (button == this.groupPreferredRes.ResetButton)
			{
				this.ResetToOriginalValue(Support.Res.Fields.ResourceNumericType.PreferredRange);
			}

			if (button == this.groupSmallStep.ResetButton)
			{
				this.ResetToOriginalValue(Support.Res.Fields.ResourceNumericType.SmallStep);
			}

			if (button == this.groupLargeStep.ResetButton)
			{
				this.ResetToOriginalValue(Support.Res.Fields.ResourceNumericType.LargeStep);
			}

			if (button == this.groupDefault.ResetButton)
			{
				this.ResetToOriginalValue(Support.Res.Fields.ResourceBaseType.DefaultValue);
			}

			this.OnContentChanged();
			this.UpdateContent();
			this.module.AccessTypes.SetLocalDirty();
		}


		protected ResetBox						groupMin;
		protected TextFieldEx					fieldMin;
		protected ResetBox						groupMax;
		protected TextFieldEx					fieldMax;
		protected ResetBox						groupRes;
		protected TextFieldEx					fieldRes;
		protected ResetBox						groupPreferredMin;
		protected TextFieldEx					fieldPreferredMin;
		protected ResetBox						groupPreferredMax;
		protected TextFieldEx					fieldPreferredMax;
		protected ResetBox						groupPreferredRes;
		protected TextFieldEx					fieldPreferredRes;
		protected ResetBox						groupSmallStep;
		protected TextFieldEx					fieldSmallStep;
		protected ResetBox						groupLargeStep;
		protected TextFieldEx					fieldLargeStep;
		protected ResetBox						groupDefault;
		protected TextFieldEx					fieldDefault;
	}
}
