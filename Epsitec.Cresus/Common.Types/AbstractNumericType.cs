//	Copyright � 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	public abstract class AbstractNumericType : AbstractType, INumericType
	{
		protected AbstractNumericType(string name, DecimalRange range) : base (name)
		{
			this.DefineRange (range);
		}

		#region INumType Members
		
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

		#endregion
		
		public void DefineRange(DecimalRange range)
		{
			this.Caption.SetValue (AbstractNumericType.RangeProperty, range);
		}

		public void DefinePreferredRange(DecimalRange range)
		{
			this.Caption.SetValue (AbstractNumericType.PreferredRangeProperty, range);
		}


		public static readonly DependencyProperty RangeProperty = DependencyProperty.RegisterAttached ("Range", typeof (DecimalRange), typeof (AbstractNumericType), new DependencyPropertyMetadata (DecimalRange.Empty));
		public static readonly DependencyProperty PreferredRangeProperty = DependencyProperty.RegisterAttached ("PreferredRange", typeof (DecimalRange), typeof (AbstractNumericType), new DependencyPropertyMetadata (DecimalRange.Empty));
	}
}
