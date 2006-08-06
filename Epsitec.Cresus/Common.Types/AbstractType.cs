//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	public abstract class AbstractType : NamedDependencyObject, INamedType, IDataConstraint
	{
		protected AbstractType(string name)
			: base (name)
		{
		}

		protected AbstractType(string name, string controller, string controllerParameter)
			: base (name)
		{
			this.DefineDefaultController (controller, controllerParameter);
		}

		#region INamedType Members

		public string DefaultController
		{
			get
			{
				return (string) this.Caption.GetValue (AbstractType.DefaultControllerProperty);
			}
		}

		public string DefaultControllerParameter
		{
			get
			{
				return (string) this.Caption.GetValue (AbstractType.DefaultControllerParameterProperty);
			}
		}

		#endregion
		
		#region ISystemType Members
		
		public abstract System.Type				SystemType
		{
			get;
		}
		
		#endregion
		
		#region IDataConstraint Members
		
		public abstract bool IsValidValue(object value);
		
		#endregion

		public void DefineDefaultController(string controller, string controllerParameter)
		{
			if (this.DefaultController != controller)
			{
				this.Caption.SetValue (AbstractType.DefaultControllerProperty, controller);
			}
			if (this.DefaultControllerParameter != controllerParameter)
			{
				this.Caption.SetValue (AbstractType.DefaultControllerParameterProperty, controllerParameter);
			}
		}


		public static readonly DependencyProperty DefaultControllerProperty = DependencyProperty.RegisterAttached ("DefaultController", typeof (string), typeof (AbstractType), new DependencyPropertyMetadata ("Numeric"));
		public static readonly DependencyProperty DefaultControllerParameterProperty = DependencyProperty.RegisterAttached ("DefaultControllerParameter", typeof (string), typeof (AbstractType), new DependencyPropertyMetadata ());
	}
}
