using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.MyWidgets
{
	/// <summary>
	/// Widget permettant d'�diter un Caption.Type.
	/// </summary>
	public class TypeEditorNumeric : AbstractTypeEditor
	{
		public TypeEditorNumeric(Module module)
		{
			this.module = module;
			ResetBox group;

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
			this.CreateDecimalLabeled(Res.Strings.Viewers.Types.Numeric.Min, left, out group, out this.fieldMin);
			group.Dock = DockStyle.StackBegin;
			group.Margins = new Margins(0, 0, 0, 2);
			this.fieldMin.EditionAccepted += new EventHandler(this.HandleTextFieldChanged);

			this.CreateDecimalLabeled(Res.Strings.Viewers.Types.Numeric.Max, left, out group, out this.fieldMax);
			group.Dock = DockStyle.StackBegin;
			group.Margins = new Margins(0, 0, 0, 2);
			this.fieldMax.EditionAccepted += new EventHandler(this.HandleTextFieldChanged);

			this.CreateDecimalLabeled(Res.Strings.Viewers.Types.Numeric.Resol, left, out group, out this.fieldRes);
			group.Dock = DockStyle.StackBegin;
			group.Margins = new Margins(0, 0, 0, 0);
			this.fieldRes.EditionAccepted += new EventHandler(this.HandleTextFieldChanged);

			//	PreferredRange.
			this.CreateDecimalLabeled(Res.Strings.Viewers.Types.Numeric.PreferredMin, right, out group, out this.fieldPreferredMin);
			group.Dock = DockStyle.StackBegin;
			group.Margins = new Margins(0, 0, 0, 2);
			this.fieldPreferredMin.EditionAccepted += new EventHandler(this.HandleTextFieldChanged);

			this.CreateDecimalLabeled(Res.Strings.Viewers.Types.Numeric.PreferredMax, right, out group, out this.fieldPreferredMax);
			group.Dock = DockStyle.StackBegin;
			group.Margins = new Margins(0, 0, 0, 2);
			this.fieldPreferredMax.EditionAccepted += new EventHandler(this.HandleTextFieldChanged);

			this.CreateDecimalLabeled(Res.Strings.Viewers.Types.Numeric.PreferredResol, right, out group, out this.fieldPreferredRes);
			group.Dock = DockStyle.StackBegin;
			group.Margins = new Margins(0, 0, 0, 0);
			this.fieldPreferredRes.EditionAccepted += new EventHandler(this.HandleTextFieldChanged);

			//	Steps.
			this.CreateDecimalLabeled(Res.Strings.Viewers.Types.Numeric.SmallStep, left, out group, out this.fieldSmallStep);
			group.Dock = DockStyle.StackBegin;
			group.Margins = new Margins(0, 0, 10, 2);
			this.fieldSmallStep.EditionAccepted += new EventHandler(this.HandleTextFieldChanged);

			this.CreateDecimalLabeled(Res.Strings.Viewers.Types.Numeric.LargeStep, left, out group, out this.fieldLargeStep);
			group.Dock = DockStyle.StackBegin;
			group.Margins = new Margins(0, 0, 0, 0);
			this.fieldLargeStep.EditionAccepted += new EventHandler(this.HandleTextFieldChanged);
		}
		
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
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
			//	Retourne le texte du r�sum�.
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
			//	Met � jour le contenu de l'�diteur.
			this.ignoreChange = true;
			object value;

			value = this.structuredData.GetValue(Support.Res.Fields.ResourceNumericType.Range);
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

			value = this.structuredData.GetValue(Support.Res.Fields.ResourceNumericType.PreferredRange);
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

			value = this.structuredData.GetValue(Support.Res.Fields.ResourceNumericType.SmallStep);
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

			value = this.structuredData.GetValue(Support.Res.Fields.ResourceNumericType.LargeStep);
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
			//	Retourne le range par d�faut avec les valeurs min/max les plus grandes possibles.
			get
			{
				decimal min = decimal.MinValue;
				decimal max = decimal.MaxValue;
				decimal res = 0.01M;  // TODO: mettre une r�solution plus grande ?

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
					res = 0.01M;  // TODO: mettre une r�solution plus grande ?
				}

				return new DecimalRange(min, max, res);
			}
		}


		protected TextFieldEx					fieldMin;
		protected TextFieldEx					fieldMax;
		protected TextFieldEx					fieldRes;
		protected TextFieldEx					fieldPreferredMin;
		protected TextFieldEx					fieldPreferredMax;
		protected TextFieldEx					fieldPreferredRes;
		protected TextFieldEx					fieldSmallStep;
		protected TextFieldEx					fieldLargeStep;
	}
}
