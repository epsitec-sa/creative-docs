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

		public virtual string DefaultController
		{
			get
			{
				return "Numeric";
			}
		}

		public virtual string DefaultControllerParameter
		{
			get
			{
				return null;
			}
		}

		#endregion
		
		#region ISystemType Members
		
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
		
		public DecimalRange						PreferredRange
		{
			get
			{
				return (DecimalRange) this.GetValue (AbstractNumericType.PreferredRangeProperty);
			}
		}

		#endregion
		
		#region IDataConstraint Members
		
		public abstract bool IsValidValue(object value);
		
		#endregion

		public void DefineRange(DecimalRange range)
		{
			this.SetLocalValue (AbstractNumericType.RangeProperty, range);
		}

		public void DefinePreferredRange(DecimalRange range)
		{
			this.SetLocalValue (AbstractNumericType.PreferredRangeProperty, range);
		}


		public static readonly DependencyProperty RangeProperty = DependencyProperty.RegisterReadOnly ("Range", typeof (DecimalRange), typeof (AbstractNumericType), new DependencyPropertyMetadata (DecimalRange.Empty));
		public static readonly DependencyProperty PreferredRangeProperty = DependencyProperty.RegisterReadOnly ("PreferredRange", typeof (DecimalRange), typeof (AbstractNumericType), new DependencyPropertyMetadata (DecimalRange.Empty));
	}
}
