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
			this.groupMin.ResetButton.Name = "Min";
			this.groupMin.ResetButton.Clicked += new MessageEventHandler(this.HandleResetButtonClicked);
			this.fieldMin.EditionAccepted += new EventHandler(this.HandleTextFieldChanged);

			this.CreateDecimalLabeled(Res.Strings.Viewers.Types.Numeric.Max, left, out this.groupMax, out this.fieldMax);
			this.groupMax.Dock = DockStyle.StackBegin;
			this.groupMax.Margins = new Margins(0, 0, 0, 2);
			this.groupMax.ResetButton.Name = "Max";
			this.groupMax.ResetButton.Clicked += new MessageEventHandler(this.HandleResetButtonClicked);
			this.fieldMax.EditionAccepted += new EventHandler(this.HandleTextFieldChanged);

			this.CreateDecimalLabeled(Res.Strings.Viewers.Types.Numeric.Resol, left, out this.groupRes, out this.fieldRes);
			this.groupRes.Dock = DockStyle.StackBegin;
			this.groupRes.Margins = new Margins(0, 0, 0, 0);
			this.groupRes.ResetButton.Name = "Res";
			this.groupRes.ResetButton.Clicked += new MessageEventHandler(this.HandleResetButtonClicked);
			this.fieldRes.EditionAccepted += new EventHandler(this.HandleTextFieldChanged);

			//	PreferredRange.
			this.CreateDecimalLabeled(Res.Strings.Viewers.Types.Numeric.PreferredMin, right, out this.groupPreferredMin, out this.fieldPreferredMin);
			this.groupPreferredMin.Dock = DockStyle.StackBegin;
			this.groupPreferredMin.Margins = new Margins(0, 0, 0, 2);
			this.groupPreferredMin.ResetButton.Name = "PreferredMin";
			this.groupPreferredMin.ResetButton.Clicked += new MessageEventHandler(this.HandleResetButtonClicked);
			this.fieldPreferredMin.EditionAccepted += new EventHandler(this.HandleTextFieldChanged);

			this.CreateDecimalLabeled(Res.Strings.Viewers.Types.Numeric.PreferredMax, right, out this.groupPreferredMax, out this.fieldPreferredMax);
			this.groupPreferredMax.Dock = DockStyle.StackBegin;
			this.groupPreferredMax.Margins = new Margins(0, 0, 0, 2);
			this.groupPreferredMax.ResetButton.Name = "PreferredMax";
			this.groupPreferredMax.ResetButton.Clicked += new MessageEventHandler(this.HandleResetButtonClicked);
			this.fieldPreferredMax.EditionAccepted += new EventHandler(this.HandleTextFieldChanged);

			this.CreateDecimalLabeled(Res.Strings.Viewers.Types.Numeric.PreferredResol, right, out this.groupPreferredRes, out this.fieldPreferredRes);
			this.groupPreferredRes.Dock = DockStyle.StackBegin;
			this.groupPreferredRes.Margins = new Margins(0, 0, 0, 0);
			this.groupPreferredRes.ResetButton.Name = "PreferredRes";
			this.groupPreferredRes.ResetButton.Clicked += new MessageEventHandler(this.HandleResetButtonClicked);
			this.fieldPreferredRes.EditionAccepted += new EventHandler(this.HandleTextFieldChanged);

			//	Steps.
			this.CreateDecimalLabeled(Res.Strings.Viewers.Types.Numeric.SmallStep, left, out this.groupSmallStep, out this.fieldSmallStep);
			this.groupSmallStep.Dock = DockStyle.StackBegin;
			this.groupSmallStep.Margins = new Margins(0, 0, 10, 2);
			this.groupSmallStep.ResetButton.Name = "SmallStep";
			this.groupSmallStep.ResetButton.Clicked += new MessageEventHandler(this.HandleResetButtonClicked);
			this.fieldSmallStep.EditionAccepted += new EventHandler(this.HandleTextFieldChanged);

			this.CreateDecimalLabeled(Res.Strings.Viewers.Types.Numeric.LargeStep, left, out this.groupLargeStep, out this.fieldLargeStep);
			this.groupLargeStep.Dock = DockStyle.StackBegin;
			this.groupLargeStep.Margins = new Margins(0, 0, 0, 0);
			this.groupLargeStep.ResetButton.Name = "LargeStep";
			this.groupLargeStep.ResetButton.Clicked += new MessageEventHandler(this.HandleResetButtonClicked);
			this.fieldLargeStep.EditionAccepted += new EventHandler(this.HandleTextFieldChanged);
		}
		
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.groupMin.ResetButton.Clicked -= new MessageEventHandler(this.HandleResetButtonClicked);
				this.groupMax.ResetButton.Clicked -= new MessageEventHandler(this.HandleResetButtonClicked);
				this.groupRes.ResetButton.Clicked -= new MessageEventHandler(this.HandleResetButtonClicked);
				this.groupPreferredMin.ResetButton.Clicked -= new MessageEventHandler(this.HandleResetButtonClicked);
				this.groupPreferredMax.ResetButton.Clicked -= new MessageEventHandler(this.HandleResetButtonClicked);
				this.groupPreferredRes.ResetButton.Clicked -= new MessageEventHandler(this.HandleResetButtonClicked);
				this.groupSmallStep.ResetButton.Clicked -= new MessageEventHandler(this.HandleResetButtonClicked);
				this.groupLargeStep.ResetButton.Clicked -= new MessageEventHandler(this.HandleResetButtonClicked);
				
				this.fieldMin.EditionAccepted -= new EventHandler(this.HandleTextFieldChanged);
				this.fieldMax.EditionAccepted -= new EventHandler(this.HandleTextFieldChanged);
				this.fieldRes.EditionAccepted -= new EventHandler(this.HandleTextFieldChanged);
				this.fieldPreferredMin.EditionAccepted -= new EventHandler(this.HandleTextFieldChanged);
				this.fieldPreferredMax.EditionAccepted -= new EventHandler(this.HandleTextFieldChanged);
				this.fieldPreferredRes.EditionAccepted -= new EventHandler(this.HandleTextFieldChanged);
				this.fieldSmallStep.EditionAccepted -= new EventHandler(this.HandleTextFieldChanged);
				this.fieldLargeStep.EditionAccepted -= new EventHandler(this.HandleTextFieldChanged);
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

			return builder.ToString();
		}


		public override void UpdateContent()
		{
			//	Met à jour le contenu de l'éditeur.
			this.ignoreChange = true;
			object value;
			bool usesOriginalData;

			value = this.structuredData.GetValue(Support.Res.Fields.ResourceNumericType.Range, out usesOriginalData);
			Viewers.Abstract.ColorizeResetBox(this.groupMin, usesOriginalData);
			Viewers.Abstract.ColorizeResetBox(this.groupMax, usesOriginalData);
			Viewers.Abstract.ColorizeResetBox(this.groupRes, usesOriginalData);
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
			Viewers.Abstract.ColorizeResetBox(this.groupPreferredMin, usesOriginalData);
			Viewers.Abstract.ColorizeResetBox(this.groupPreferredMax, usesOriginalData);
			Viewers.Abstract.ColorizeResetBox(this.groupPreferredRes, usesOriginalData);
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
			Viewers.Abstract.ColorizeResetBox(this.groupSmallStep, usesOriginalData);
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
			Viewers.Abstract.ColorizeResetBox(this.groupLargeStep, usesOriginalData);
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

			if (button.Name == "Min")
			{
				this.ResetToOriginalValue(Support.Res.Fields.ResourceNumericType.Range);
			}

			if (button.Name == "Max")
			{
				this.ResetToOriginalValue(Support.Res.Fields.ResourceNumericType.Range);
			}

			if (button.Name == "Res")
			{
				this.ResetToOriginalValue(Support.Res.Fields.ResourceNumericType.Range);
			}

			if (button.Name == "PreferredMin")
			{
				this.ResetToOriginalValue(Support.Res.Fields.ResourceNumericType.PreferredRange);
			}

			if (button.Name == "PreferredMax")
			{
				this.ResetToOriginalValue(Support.Res.Fields.ResourceNumericType.PreferredRange);
			}

			if (button.Name == "PreferredRes")
			{
				this.ResetToOriginalValue(Support.Res.Fields.ResourceNumericType.PreferredRange);
			}

			if (button.Name == "SmallStep")
			{
				this.ResetToOriginalValue(Support.Res.Fields.ResourceNumericType.SmallStep);
			}

			if (button.Name == "LargeStep")
			{
				this.ResetToOriginalValue(Support.Res.Fields.ResourceNumericType.LargeStep);
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
	}
}
