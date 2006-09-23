//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Types.AbstractType))]

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

		protected AbstractType(Caption caption)
			: base (caption)
		{
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

		protected override void OnCaptionDefined()
		{
			base.OnCaptionDefined ();

			Caption caption = this.Caption;

			if (caption != null)
			{
				AbstractType.SetSystemType (caption, this.SystemType.FullName);
			}
		}

		public static string GetSystemType(Caption caption)
		{
			return (string) caption.GetValue (AbstractType.SytemTypeProperty);
		}
		
		public static AbstractType GetCachedType(Caption caption)
		{
			return (AbstractType) caption.GetValue (AbstractType.CachedTypeProperty);
		}

		public static void SetSystemType(Caption caption, string value)
		{
			if (string.IsNullOrEmpty (value))
			{
				caption.ClearValue (AbstractType.SytemTypeProperty);
			}
			else
			{
				caption.SetValue (AbstractType.SytemTypeProperty, value);
			}
		}

		public static void SetCachedType(Caption caption, AbstractType value)
		{
			if (value == null)
			{
				caption.ClearValue (AbstractType.CachedTypeProperty);
			}
			else
			{
				caption.SetValue (AbstractType.CachedTypeProperty, value);
			}
		}

		public static readonly DependencyProperty DefaultControllerProperty = DependencyProperty.RegisterAttached ("DefaultController", typeof (string), typeof (AbstractType), new DependencyPropertyMetadata ("Numeric"));
		public static readonly DependencyProperty DefaultControllerParameterProperty = DependencyProperty.RegisterAttached ("DefaultControllerParameter", typeof (string), typeof (AbstractType), new DependencyPropertyMetadata ());
		public static readonly DependencyProperty SytemTypeProperty = DependencyProperty.RegisterAttached ("SystemType", typeof (string), typeof (AbstractType));
		public static readonly DependencyProperty CachedTypeProperty = DependencyProperty.RegisterAttached ("CachedType", typeof (AbstractType), typeof (AbstractType), new DependencyPropertyMetadata ().MakeNotSerializable ());
	}
}
