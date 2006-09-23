//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Types.AbstractType))]

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>AbstractType</c> class implements the basic type properties by
	/// storing them as attached properties in a <see cref="Caption"/>.
	/// </summary>
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

		/// <summary>
		/// Defines the default controller used to represent data of this type.
		/// </summary>
		/// <param name="controller">The controller.</param>
		/// <param name="controllerParameter">The controller parameter.</param>
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

			if ((caption != null) &&
				(this.SystemType != null))
			{
				AbstractType.SetSystemType (caption, this.SystemType.FullName);
			}
		}

		/// <summary>
		/// Gets the value of the <c>SystemTypeProperty</c> dependency property.
		/// </summary>
		/// <param name="caption">The caption to query.</param>
		/// <returns>The value or <c>null</c> if none is defined.</returns>
		public static string GetSystemType(Caption caption)
		{
			return (string) caption.GetValue (AbstractType.SytemTypeProperty);
		}

		/// <summary>
		/// Gets the value of the <c>CachedTypeProperty</c> dependency property.
		/// </summary>
		/// <param name="caption">The caption to query.</param>
		/// <returns>The value or <c>null</c> if none is defined.</returns>
		public static AbstractType GetCachedType(Caption caption)
		{
			return (AbstractType) caption.GetValue (AbstractType.CachedTypeProperty);
		}

		/// <summary>
		/// Gets the value of the <c>ComplexTypeProperty</c> dependency property.
		/// </summary>
		/// <param name="caption">The caption to query.</param>
		/// <returns>The value or <c>null</c> if none is defined.</returns>
		public static AbstractType GetComplexType(Caption caption)
		{
			return (AbstractType) caption.GetValue (AbstractType.ComplexTypeProperty);
		}

		/// <summary>
		/// Sets the value of the <c>SystemTypeProperty</c> dependency property.
		/// </summary>
		/// <param name="caption">The caption to modify.</param>
		/// <param name="value">The value.</param>
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

		/// <summary>
		/// Sets the value of the <c>CachedTypeProperty</c> dependency property.
		/// </summary>
		/// <param name="caption">The caption to modify.</param>
		/// <param name="value">The value.</param>
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

		/// <summary>
		/// Sets the value of the <c>ComplexTypeProperty</c> dependency property.
		/// </summary>
		/// <param name="caption">The caption to modify.</param>
		/// <param name="value">The value.</param>
		public static void SetComplexType(Caption caption, AbstractType value)
		{
			if (value == null)
			{
				caption.ClearValue (AbstractType.ComplexTypeProperty);
			}
			else
			{
				caption.SetValue (AbstractType.ComplexTypeProperty, value);
			}
		}

		public static readonly DependencyProperty DefaultControllerProperty = DependencyProperty.RegisterAttached ("DefaultController", typeof (string), typeof (AbstractType), new DependencyPropertyMetadata ("Numeric"));
		public static readonly DependencyProperty DefaultControllerParameterProperty = DependencyProperty.RegisterAttached ("DefaultControllerParameter", typeof (string), typeof (AbstractType), new DependencyPropertyMetadata ());
		public static readonly DependencyProperty SytemTypeProperty = DependencyProperty.RegisterAttached ("SystemType", typeof (string), typeof (AbstractType));
		public static readonly DependencyProperty CachedTypeProperty = DependencyProperty.RegisterAttached ("CachedType", typeof (AbstractType), typeof (AbstractType), new DependencyPropertyMetadata ().MakeNotSerializable ());
		public static readonly DependencyProperty ComplexTypeProperty = DependencyProperty.RegisterAttached ("ComplexType", typeof (AbstractType), typeof (AbstractType));
	}
}
