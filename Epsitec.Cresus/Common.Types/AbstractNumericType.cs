//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	public abstract class AbstractNumericType : NamedDependencyObject, INamedType, INumericType, IDataConstraint
	{
		protected AbstractNumericType(string name, DecimalRange range) : base (name)
		{
			this.DefineRange (range);
		}
		
		#region INamedType Members
		
		public abstract System.Type				SystemType
		{
			get;
		}
		
		#endregion
		
		#region INumType Members
		
		public DecimalRange						Range
		{
			get
			{
				return (DecimalRange) this.GetValue (AbstractNumericType.RangeProperty);
			}
		}

		#endregion
		
		#region IDataConstraint Members
		
		public abstract bool IsValidValue(object value);
		
		#endregion

		protected void DefineRange(DecimalRange range)
		{
			this.SetLocalValue (AbstractNumericType.RangeProperty, range);
		}


		public static readonly DependencyProperty RangeProperty = DependencyProperty.RegisterReadOnly ("Range", typeof (DecimalRange), typeof (AbstractNumericType), new DependencyPropertyMetadata (DecimalRange.Empty));
	}
}
