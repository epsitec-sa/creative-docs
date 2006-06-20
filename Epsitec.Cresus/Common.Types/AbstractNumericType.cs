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

		public string DefaultController
		{
			get
			{
				return (string) this.GetValue (AbstractNumericType.DefaultControllerProperty);
			}
		}

		public string DefaultControllerParameter
		{
			get
			{
				return (string) this.GetValue (AbstractNumericType.DefaultControllerParameterProperty);
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
			this.SetValue (AbstractNumericType.RangeProperty, range);
		}

		public void DefinePreferredRange(DecimalRange range)
		{
			this.SetValue (AbstractNumericType.PreferredRangeProperty, range);
		}

		public void DefineDefaultController(string controller, string controllerParameter)
		{
			if (this.DefaultController != controller)
			{
				this.SetValue (AbstractNumericType.DefaultControllerProperty, controller);
			}
			if (this.DefaultControllerParameter != controllerParameter)
			{
				this.SetValue (AbstractNumericType.DefaultControllerParameterProperty, controllerParameter);
			}
		}


		public static readonly DependencyProperty DefaultControllerProperty = DependencyProperty.RegisterReadOnly ("DefaultController", typeof (string), typeof (AbstractNumericType), new DependencyPropertyMetadata ("Numeric"));
		public static readonly DependencyProperty DefaultControllerParameterProperty = DependencyProperty.RegisterReadOnly ("DefaultControllerParameter", typeof (string), typeof (AbstractNumericType), new DependencyPropertyMetadata ());
		public static readonly DependencyProperty RangeProperty = DependencyProperty.RegisterReadOnly ("Range", typeof (DecimalRange), typeof (AbstractNumericType), new DependencyPropertyMetadata (DecimalRange.Empty));
		public static readonly DependencyProperty PreferredRangeProperty = DependencyProperty.RegisterReadOnly ("PreferredRange", typeof (DecimalRange), typeof (AbstractNumericType), new DependencyPropertyMetadata (DecimalRange.Empty));
	}
}
