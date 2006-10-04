//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Types.AbstractNumericType))]

namespace Epsitec.Common.Types
{
	public abstract class AbstractNumericType : AbstractType, INumericType
	{
		protected AbstractNumericType(string name, DecimalRange range)
			: base (name)
		{
			this.DefineRange (range);
		}

		protected AbstractNumericType(Caption caption)
			: base (caption)
		{
		}

		#region INumericType Members
		
		public DecimalRange						Range
		{
			get
			{
				return (DecimalRange) this.Caption.GetValue (AbstractNumericType.RangeProperty);
			}
		}
		
		public DecimalRange						PreferredRange
		{
			get
			{
				return (DecimalRange) this.Caption.GetValue (AbstractNumericType.PreferredRangeProperty);
			}
		}

		public decimal							SmallStep
		{
			get
			{
				return (decimal) this.Caption.GetValue (AbstractNumericType.SmallStepProperty);
			}
		}

		public decimal							LargeStep
		{
			get
			{
				return (decimal) this.Caption.GetValue (AbstractNumericType.LargeStepProperty);
			}
		}

		#endregion
		
		public void DefineRange(DecimalRange range)
		{
			if (range.IsEmpty)
			{
				this.Caption.ClearValue (AbstractNumericType.RangeProperty);
			}
			else
			{
				this.Caption.SetValue (AbstractNumericType.RangeProperty, range);
			}
		}

		public void DefinePreferredRange(DecimalRange range)
		{
			if (range.IsEmpty)
			{
				this.Caption.ClearValue (AbstractNumericType.PreferredRangeProperty);
			}
			else
			{
				this.Caption.SetValue (AbstractNumericType.PreferredRangeProperty, range);
			}
		}

		public void DefineSmallStep(decimal value)
		{
			if (value == 0)
			{
				this.Caption.ClearValue (AbstractNumericType.SmallStepProperty);
			}
			else
			{
				this.Caption.SetValue (AbstractNumericType.SmallStepProperty, value);
			}
		}

		public void DefineLargeStep(decimal value)
		{
			if (value == 0)
			{
				this.Caption.ClearValue (AbstractNumericType.LargeStepProperty);
			}
			else
			{
				this.Caption.SetValue (AbstractNumericType.LargeStepProperty, value);
			}
		}

		public static readonly DependencyProperty RangeProperty = DependencyProperty.RegisterAttached ("Range", typeof (DecimalRange), typeof (AbstractNumericType), new DependencyPropertyMetadata (DecimalRange.Empty));
		public static readonly DependencyProperty PreferredRangeProperty = DependencyProperty.RegisterAttached ("PreferredRange", typeof (DecimalRange), typeof (AbstractNumericType), new DependencyPropertyMetadata (DecimalRange.Empty));
		public static readonly DependencyProperty SmallStepProperty = DependencyProperty.RegisterAttached ("SmallStep", typeof (decimal), typeof (AbstractNumericType), new DependencyPropertyMetadata (0M));
		public static readonly DependencyProperty LargeStepProperty = DependencyProperty.RegisterAttached ("LargeStep", typeof (decimal), typeof (AbstractNumericType), new DependencyPropertyMetadata (0M));
	}
}
