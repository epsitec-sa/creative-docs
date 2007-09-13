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
		public TypeEditorNumeric()
		{
			FrameBox group;

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
			this.fieldMin.TextChanged += new EventHandler(this.HandleTextFieldChanged);

			this.CreateDecimalLabeled(Res.Strings.Viewers.Types.Numeric.Max, left, out group, out this.fieldMax);
			group.Dock = DockStyle.StackBegin;
			group.Margins = new Margins(0, 0, 0, 2);
			this.fieldMax.TextChanged += new EventHandler(this.HandleTextFieldChanged);

			this.CreateDecimalLabeled(Res.Strings.Viewers.Types.Numeric.Resol, left, out group, out this.fieldRes);
			group.Dock = DockStyle.StackBegin;
			group.Margins = new Margins(0, 0, 0, 0);
			this.fieldRes.TextChanged += new EventHandler(this.HandleTextFieldChanged);

			//	PreferredRange.
			this.CreateDecimalLabeled(Res.Strings.Viewers.Types.Numeric.PreferredMin, right, out group, out this.fieldPreferredMin);
			group.Dock = DockStyle.StackBegin;
			group.Margins = new Margins(0, 0, 0, 2);
			this.fieldPreferredMin.TextChanged += new EventHandler(this.HandleTextFieldChanged);

			this.CreateDecimalLabeled(Res.Strings.Viewers.Types.Numeric.PreferredMax, right, out group, out this.fieldPreferredMax);
			group.Dock = DockStyle.StackBegin;
			group.Margins = new Margins(0, 0, 0, 2);
			this.fieldPreferredMax.TextChanged += new EventHandler(this.HandleTextFieldChanged);

			this.CreateDecimalLabeled(Res.Strings.Viewers.Types.Numeric.PreferredResol, right, out group, out this.fieldPreferredRes);
			group.Dock = DockStyle.StackBegin;
			group.Margins = new Margins(0, 0, 0, 0);
			this.fieldPreferredRes.TextChanged += new EventHandler(this.HandleTextFieldChanged);

			//	Steps.
			this.CreateDecimalLabeled(Res.Strings.Viewers.Types.Numeric.SmallStep, left, out group, out this.fieldSmallStep);
			group.Dock = DockStyle.StackBegin;
			group.Margins = new Margins(0, 0, 10, 2);
			this.fieldSmallStep.TextChanged += new EventHandler(this.HandleTextFieldChanged);

			this.CreateDecimalLabeled(Res.Strings.Viewers.Types.Numeric.LargeStep, left, out group, out this.fieldLargeStep);
			group.Dock = DockStyle.StackBegin;
			group.Margins = new Margins(0, 0, 0, 0);
			this.fieldLargeStep.TextChanged += new EventHandler(this.HandleTextFieldChanged);

			//	Default et Sample.
#if false
			this.CreateDecimalLabeled(Res.Strings.Viewers.Types.Numeric.Default, left, out group, out this.fieldDefault);
			group.Dock = DockStyle.StackBegin;
			group.Margins = new Margins(0, 0, 10, 2);
			this.fieldDefault.TextChanged += new EventHandler(this.HandleTextFieldChanged);

			this.CreateDecimalLabeled(Res.Strings.Viewers.Types.Numeric.Sample, left, out group, out this.fieldSample);
			group.Dock = DockStyle.StackBegin;
			group.Margins = new Margins(0, 0, 0, 0);
			this.fieldSample.TextChanged += new EventHandler(this.HandleTextFieldChanged);
#endif
		}
		
		public TypeEditorNumeric(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.fieldMin.TextChanged -= new EventHandler(this.HandleTextFieldChanged);
				this.fieldMax.TextChanged -= new EventHandler(this.HandleTextFieldChanged);
				this.fieldRes.TextChanged -= new EventHandler(this.HandleTextFieldChanged);

				this.fieldPreferredMin.TextChanged -= new EventHandler(this.HandleTextFieldChanged);
				this.fieldPreferredMax.TextChanged -= new EventHandler(this.HandleTextFieldChanged);
				this.fieldPreferredRes.TextChanged -= new EventHandler(this.HandleTextFieldChanged);

				this.fieldSmallStep.TextChanged -= new EventHandler(this.HandleTextFieldChanged);
				this.fieldLargeStep.TextChanged -= new EventHandler(this.HandleTextFieldChanged);

#if false
				this.fieldDefault.TextChanged -= new EventHandler(this.HandleTextFieldChanged);
				this.fieldSample.TextChanged -= new EventHandler(this.HandleTextFieldChanged);
#endif
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

			this.PutSummaryDefaultAndSample(builder);
#else
			AbstractNumericType type = this.AbstractType as AbstractNumericType;
			string text;

			if (!type.Range.IsEmpty)
			{
				this.PutSummaryValue(builder, Res.Strings.Viewers.Types.Summary.Min, type.Range.Minimum.ToString());
				this.PutSummaryValue(builder, Res.Strings.Viewers.Types.Summary.Max, type.Range.Maximum.ToString());
				this.PutSummaryValue(builder, Res.Strings.Viewers.Types.Summary.Resolution, type.Range.Resolution.ToString());
			}

			if (!type.PreferredRange.IsEmpty)
			{
				this.PutSummaryValue(builder, Res.Strings.Viewers.Types.Summary.Min, type.PreferredRange.Minimum.ToString());
				this.PutSummaryValue(builder, Res.Strings.Viewers.Types.Summary.Max, type.PreferredRange.Maximum.ToString());
				this.PutSummaryValue(builder, Res.Strings.Viewers.Types.Summary.Resolution, type.PreferredRange.Resolution.ToString());
			}

			this.PutSummaryValue(builder, Res.Strings.Viewers.Types.Summary.SmallStep, type.SmallStep.ToString());
			this.PutSummaryValue(builder, Res.Strings.Viewers.Types.Summary.BigStep, type.LargeStep.ToString());

			this.PutSummaryDefaultAndSample(builder, type);

			if (type.UseCompactStorage)
			{
				this.PutSummarySeparator(builder, 2);
				this.PutSummaryValue(builder, Res.Strings.Viewers.Types.Summary.Compact);
			}
#endif

			return builder.ToString();
		}


		protected override void UpdateContent()
		{
			//	Met à jour le contenu de l'éditeur.
#if true
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
#else
			AbstractNumericType type = this.AbstractType as AbstractNumericType;

			this.ignoreChange = true;

			if (type.Range.IsEmpty)
			{
				this.fieldMin.Text = "";
				this.fieldMax.Text = "";
				this.fieldRes.Text = "";
			}
			else
			{
				this.SetDecimal(this.fieldMin, type.Range.Minimum);
				this.SetDecimal(this.fieldMax, type.Range.Maximum);
				this.SetDecimal(this.fieldRes, type.Range.Resolution);
			}

			if (type.PreferredRange.IsEmpty)
			{
				this.fieldPreferredMin.Text = "";
				this.fieldPreferredMax.Text = "";
				this.fieldPreferredRes.Text = "";
			}
			else
			{
				this.SetDecimal(this.fieldPreferredMin, type.PreferredRange.Minimum);
				this.SetDecimal(this.fieldPreferredMax, type.PreferredRange.Maximum);
				this.SetDecimal(this.fieldPreferredRes, type.PreferredRange.Resolution);
			}

			this.SetDecimal(this.fieldSmallStep, type.SmallStep);
			this.SetDecimal(this.fieldLargeStep, type.LargeStep);

			this.SetDecimalObject(this.fieldDefault, type.DefaultValue);
			this.SetDecimalObject(this.fieldSample, type.SampleValue);

			this.checkCompactStorage.ActiveState = type.UseCompactStorage ? ActiveState.Yes : ActiveState.No;

			this.ignoreChange = false;
#endif
		}


		private void HandleTextFieldChanged(object sender)
		{
			if (this.ignoreChange)
			{
				return;
			}

#if true
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
				//	TODO:
			}

			if (sender == this.fieldSample)
			{
				//	TODO:
			}
			
			this.OnContentChanged();
			this.UpdateContent();
			this.module.AccessTypes.SetLocalDirty();
#else
			//	[Note1] On demande le type avec un ResourceAccess.GetField.
			AbstractNumericType type = this.AbstractType as AbstractNumericType;

			decimal min = type.Range.Minimum;
			decimal max = type.Range.Maximum;
			decimal res = type.Range.Resolution;

			decimal pmin = type.PreferredRange.Minimum;
			decimal pmax = type.PreferredRange.Maximum;
			decimal pres = type.PreferredRange.Resolution;

			//	Range.
			if (sender == this.fieldMin)
			{
				min = this.GetDecimal(this.fieldMin);
			}

			if (sender == this.fieldMax)
			{
				max = this.GetDecimal(this.fieldMax);
			}

			if (sender == this.fieldRes)
			{
				res = this.GetDecimal(this.fieldRes);
			}

			//	PreferredRange.
			if (sender == this.fieldPreferredMin)
			{
				pmin = this.GetDecimal(this.fieldPreferredMin);
			}

			if (sender == this.fieldPreferredMax)
			{
				pmax = this.GetDecimal(this.fieldPreferredMax);
			}

			if (sender == this.fieldPreferredRes)
			{
				pres = this.GetDecimal(this.fieldPreferredRes);
			}

			//	Steps.
			if (sender == this.fieldSmallStep)
			{
				type.DefineSmallStep(this.GetDecimal(this.fieldSmallStep));
			}

			if (sender == this.fieldLargeStep)
			{
				type.DefineLargeStep(this.GetDecimal(this.fieldLargeStep));
			}

			if (sender == this.fieldDefault)
			{
				type.DefineDefaultValue(this.GetDecimalObject(this.fieldDefault, type.SystemType));
			}

			if (sender == this.fieldSample)
			{
				type.DefineSampleValue(this.GetDecimalObject(this.fieldSample, type.SystemType));
			}

			if (min == 0 && max == 0 && res == 0)
			{
				type.DefineRange(new DecimalRange());
			}
			else
			{
				type.DefineRange(new DecimalRange(min, max, res));
			}

			if (pmin == 0 && pmax == 0 && pres == 0)
			{
				type.DefinePreferredRange(new DecimalRange());
			}
			else
			{
				type.DefinePreferredRange(new DecimalRange(pmin, pmax, pres));
			}

			//	[Note1] Cet appel va provoquer le ResourceAccess.SetField.
			this.OnContentChanged();
#endif
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


		protected TextField						fieldMin;
		protected TextField						fieldMax;
		protected TextField						fieldRes;
		protected TextField						fieldPreferredMin;
		protected TextField						fieldPreferredMax;
		protected TextField						fieldPreferredRes;
		protected TextField						fieldSmallStep;
		protected TextField						fieldLargeStep;
		protected TextField						fieldDefault;
		protected TextField						fieldSample;
	}
}
