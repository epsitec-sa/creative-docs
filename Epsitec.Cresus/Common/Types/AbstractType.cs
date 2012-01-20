//	Copyright © 2006-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Types.AbstractType))]

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>AbstractType</c> class implements the basic type properties by
	/// storing them as attached properties in a <see cref="Caption"/>.
	/// </summary>
	public abstract class AbstractType : NamedDependencyObject, INamedType, IDataConstraint, INullableType
	{
		protected AbstractType()
			: base ()
		{
		}

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

		protected AbstractType(Support.Druid druid)
			: base (druid)
		{
		}

		#region INamedType Members

		/// <summary>
		/// Gets the default controller used to represent data of this type.
		/// </summary>
		/// <value>The default controller.</value>
		public string DefaultController
		{
			get
			{
				return (string) this.Caption.GetValue (AbstractType.DefaultControllerProperty);
			}
		}

		/// <summary>
		/// Gets the parameter used with the default controller.
		/// </summary>
		/// <value>The default controller parameter.</value>
		public string DefaultControllerParameters
		{
			get
			{
				return (string) this.Caption.GetValue (AbstractType.DefaultControllerParametersProperty);
			}
		}

		#endregion
		
		#region ISystemType Members

		/// <summary>
		/// Gets the system type described by this object.
		/// </summary>
		/// <value>The system type described by this object.</value>
		public abstract System.Type				SystemType
		{
			get;
		}
		
		#endregion
		
		#region IDataConstraint Members

		/// <summary>
		/// Determines whether the specified value is valid according to the
		/// constraint.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// 	<c>true</c> if the specified value is valid; otherwise, <c>false</c>.
		/// </returns>
		public abstract bool IsValidValue(object value);
		
		#endregion

		#region INullableType Members

		/// <summary>
		/// Gets a value indicating whether this type may represent <c>null</c> values.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this type may represent <c>null</c> values; otherwise, <c>false</c>.
		/// </value>
		public bool IsNullable
		{
			get
			{
				return (bool) this.Caption.GetValue (AbstractType.IsNullableProperty);
			}
		}

		/// <summary>
		/// Gets a value indicating whether the value is null.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// 	<c>true</c> if the specified value represents the <c>null</c> value; otherwise, <c>false</c>.
		/// </returns>
		public virtual bool IsNullValue(object value)
		{
			return value == null;
		}

		#endregion

		/// <summary>
		/// Gets the type code for the type.
		/// </summary>
		/// <value>The type code.</value>
		public abstract TypeCode				TypeCode
		{
			get;
		}

		/// <summary>
		/// Gets the default value for this type.
		/// </summary>
		/// <value>The default value or <c>null</c>.</value>
		public virtual object					DefaultValue
		{
			get
			{
				return AbstractType.GetDefaultValue (this.Caption);
			}
		}

		/// <summary>
		/// Gets the sample value for this type.
		/// </summary>
		/// <value>The sample value or <c>null</c>.</value>
		public object							SampleValue
		{
			get
			{
				return AbstractType.GetSampleValue (this.Caption);
			}
		}

		/// <summary>
		/// Gets the module where this type is defined.
		/// </summary>
		/// <value>The module where this type is defined or <c>ResourceModuleId.Empty</c>.</value>
		public Support.ResourceModuleId			Module
		{
			get
			{
				if (this.IsCaptionDefined)
				{
					return Support.ResourceManager.GetSourceModule (this.Caption);
				}
				else
				{
					return Support.ResourceModuleId.Empty;
				}
			}
		}


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
			if (this.DefaultControllerParameters != controllerParameter)
			{
				this.Caption.SetValue (AbstractType.DefaultControllerParametersProperty, controllerParameter);
			}
		}

		/// <summary>
		/// Defines if this type is nullable or not.
		/// </summary>
		/// <param name="isNullable">If set to <c>true</c>, this type is nullable.</param>
		public void DefineIsNullable(bool isNullable)
		{
			if (this.IsNullable != isNullable)
			{
				if (isNullable)
				{
					this.Caption.SetValue (AbstractType.IsNullableProperty, true);
				}
				else
				{
					this.Caption.ClearValue (AbstractType.IsNullableProperty);
				}
			}
		}

		/// <summary>
		/// Defines the default value for this type.
		/// </summary>
		/// <param name="value">The value or <c>null</c>.</param>
		public void DefineDefaultValue(object value)
		{
			if (this.IsNullValue (value))
			{
				value = null;
			}
			
			System.Diagnostics.Debug.Assert ((value == null) || (this.IsValidValue (value)));
			AbstractType.SetDefaultValue (this.Caption, value);
		}

		/// <summary>
		/// Defines the sample value for this type.
		/// </summary>
		/// <param name="value">The value or <c>null</c>.</param>
		public void DefineSampleValue(object value)
		{
			if (this.IsNullValue (value))
			{
				value = null;
			}

			System.Diagnostics.Debug.Assert ((value == null) || (this.IsValidValue (value)));
			AbstractType.SetSampleValue (this.Caption, value);
		}

		protected override void OnCaptionDefined()
		{
			base.OnCaptionDefined ();

			Caption caption = this.Caption;

			if ((caption != null) &&
				(this.SystemType != null))
			{
				AbstractType.SetSystemType (caption, this.SystemType);
			}
		}

		/// <summary>
		/// Gets a partially qualified <see cref="System.Type"/> name.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>The name.</returns>
		public static string GetSystemTypeNameFromSystemType(System.Type type)
		{
			//	We don't use the fully qualified type name here, since this would be
			//	to verbose when serializing, and we hopefully don't need the versioning
			//	information either.

			string typeName     = type.FullName;
			string typeAssembly = type.Assembly.GetName ().Name;

			string name;

			if (typeAssembly == "mscorlib")
			{
				//	Types from the core .NET library won't be suffixed by an assembly
				//	name; thus, "int" will simply be represented as "System.Int32" :

				name = typeName;
			}
			else
			{
				//	Other types need the assembly reference :

				name = string.Concat (typeName, ", ", typeAssembly);
			}

			if (type.IsEnum)
			{
				return string.Concat ("E:", name);
			}
			if (type.IsClass)
			{
				return string.Concat ("C:", name);
			}
			if (type.IsValueType)
			{
				return string.Concat ("V:", name);
			}
			if (type.IsInterface)
			{
				return string.Concat ("I:", name);
			}

			throw new System.ArgumentException (string.Format ("Type {0} has an unsupported name", name));
		}

		/// <summary>
		/// Gets the <see cref="System.Type"/> from a partially qualified name.
		/// </summary>
		/// <param name="name">The partially qualified name.</param>
		/// <returns>The type.</returns>
		public static System.Type GetSystemTypeFromSystemTypeName(string name)
		{
			if ((name == null) ||
				(name.Length < 3) ||
				(name[1] != ':'))
			{
				throw new System.ArgumentException ("Invalid type name");
			}

			string prefix = name.Substring (0, 2);
			
			name = name.Substring (2);

			if (!name.Contains (", "))
			{
				//	A name which has no assembly specification refers to a type from
				//	the core .NET library :
				
				name = string.Concat (name, ", mscorlib");
			}

			System.Type type = TypeRosetta.GetSystemType (name);

			switch (prefix)
			{
				case "E:": break;
				case "C:": break;
				case "V:": break;
				case "I:": break;
				
				default:
					throw new System.ArgumentException (string.Format ("Type {0} has wrong prefix ({1})", name, prefix));
			}

			return type;
		}

		/// <summary>
		/// Gets the name of the system type family from the tagged system type name.
		/// </summary>
		/// <param name="name">The tagged system type name.</param>
		/// <returns>The <c>SystemTypeFamily</c>.</returns>
		public static SystemTypeFamily GetSystemTypeFamilyFromSystemTypeName(string name)
		{
			if ((name == null) ||
				(name.Length < 3) ||
				(name[1] != ':'))
			{
				throw new System.ArgumentException ("Invalid type name");
			}

			switch (name.Substring (0, 2))
			{
				case "E:": return SystemTypeFamily.Enum;
				case "C:": return SystemTypeFamily.Class;
				case "V:": return SystemTypeFamily.ValueType;
				case "I:": return SystemTypeFamily.Interface;
			}

			return SystemTypeFamily.Unknown;
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
		/// Gets the default value.
		/// </summary>
		/// <param name="caption">The caption.</param>
		/// <returns></returns>
		public static object GetDefaultValue(Caption caption)
		{
			object value = caption.GetValue (AbstractType.DefaultValueProperty);
			return value == null ? null : ((TypedObject) value).Value;
		}

		/// <summary>
		/// Gets the template value.
		/// </summary>
		/// <param name="caption">The caption.</param>
		/// <returns></returns>
		public static object GetSampleValue(Caption caption)
		{
			object value = caption.GetValue (AbstractType.SampleValueProperty);
			return value == null ? null : ((TypedObject) value).Value;
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
		/// <param name="typeName">The system type name as returned by <c>GetSystemTypeNameFromSystemType</c>.</param>
		public static void SetSystemType(Caption caption, string typeName)
		{
			if (string.IsNullOrEmpty (typeName))
			{
				caption.ClearValue (AbstractType.SytemTypeProperty);
			}
			else
			{
				caption.SetValue (AbstractType.SytemTypeProperty, typeName);
			}
		}

		/// <summary>
		/// Sets the value of the <c>SystemTypeProperty</c> dependency property.
		/// </summary>
		/// <param name="caption">The caption to modify.</param>
		/// <param name="type">The system type.</param>
		public static void SetSystemType(Caption caption, System.Type type)
		{
			if (type == null)
			{
				AbstractType.SetSystemType (caption, "");
			}
			else
			{
				AbstractType.SetSystemType (caption, AbstractType.GetSystemTypeNameFromSystemType (type));
			}
		}

		/// <summary>
		/// Sets the default value.
		/// </summary>
		/// <param name="caption">The caption.</param>
		/// <param name="value">The value.</param>
		public static void SetDefaultValue(Caption caption, object value)
		{
			if (value == null)
			{
				caption.ClearValue (AbstractType.DefaultValueProperty);
			}
			else
			{
				caption.SetValue (AbstractType.DefaultValueProperty, new TypedObject (value));
			}
		}

		/// <summary>
		/// Sets the template value.
		/// </summary>
		/// <param name="caption">The caption.</param>
		/// <param name="value">The value.</param>
		public static void SetSampleValue(Caption caption, object value)
		{
			if (value == null)
			{
				caption.ClearValue (AbstractType.SampleValueProperty);
			}
			else
			{
				caption.SetValue (AbstractType.SampleValueProperty, new TypedObject (value));
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

		/// <summary>
		/// Binds the complex type to the caption, if the complex type has no
		/// associated caption yet.
		/// </summary>
		/// <param name="caption">The caption.</param>
		public static void BindComplexTypeToCaption(Caption caption)
		{
			AbstractType type = AbstractType.GetComplexType (caption);

			if ((type != null) &&
				(type.IsCaptionDefined == false) &&
				(caption.Id.IsValid))
			{
				type.DefineCaption (caption);
			}
		}

		/// <summary>
		/// Checks that the complex type is properly bound to the caption.
		/// </summary>
		/// <param name="caption">The caption.</param>
		/// <returns><c>true</c> if the complex type is properly bound to the
		/// caption or if there is no complex type; otherwise, <c>false</c>.</returns>
		public static bool CheckComplexTypeBindingToCaption(Caption caption)
		{
			AbstractType type = AbstractType.GetComplexType (caption);

			if (type == null)
			{
				return true;
			}
			else
			{
				return type.IsCaptionDefined;
			}
		}

		
		public static readonly DependencyProperty DefaultControllerProperty = DependencyProperty.RegisterAttached ("DefaultController", typeof (string), typeof (AbstractType), new DependencyPropertyMetadata ("Numeric"));
		public static readonly DependencyProperty DefaultControllerParametersProperty = DependencyProperty.RegisterAttached ("DefaultControllerParameters", typeof (string), typeof (AbstractType), new DependencyPropertyMetadata ());
		public static readonly DependencyProperty SytemTypeProperty = DependencyProperty.RegisterAttached ("SystemType", typeof (string), typeof (AbstractType));
		public static readonly DependencyProperty CachedTypeProperty = DependencyProperty.RegisterAttached ("CachedType", typeof (AbstractType), typeof (AbstractType), new DependencyPropertyMetadata ().MakeNotSerializable ());
		public static readonly DependencyProperty ComplexTypeProperty = DependencyProperty.RegisterAttached ("ComplexType", typeof (AbstractType), typeof (AbstractType));
		public static readonly DependencyProperty IsNullableProperty = DependencyProperty.RegisterAttached ("IsNullable", typeof (bool), typeof (AbstractType), new DependencyPropertyMetadata (false));
		public static readonly DependencyProperty DefaultValueProperty = DependencyProperty.RegisterAttached ("DefaultValue", typeof (TypedObject), typeof (AbstractType));
		public static readonly DependencyProperty SampleValueProperty = DependencyProperty.RegisterAttached ("SampleValue", typeof (TypedObject), typeof (AbstractType));
	}
}
