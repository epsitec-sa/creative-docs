//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>TypeRosetta</c> class is the Rosetta Stone for everything which
	/// pertains to types. It is used to map between different type description
	/// mechanisms and well-known types, such as <see cref="T:System.Type"/>,
	/// <see cref="T:INamedType"/> or <see cref="T:IStructuredType"/>.
	/// </summary>
	public static class TypeRosetta
	{
		/// <summary>
		/// Gets the system type from type object.
		/// </summary>
		/// <param name="typeObject">The type object.</param>
		/// <returns>The system type.</returns>
		public static System.Type GetSystemTypeFromTypeObject(object typeObject)
		{
			System.Type sytemType = typeObject as System.Type;

			if (sytemType != null)
			{
				return sytemType;
			}

			INamedType namedType = typeObject as INamedType;

			if (namedType != null)
			{
				return namedType.SystemType;
			}

			DependencyProperty dependencyProperty = typeObject as DependencyProperty;

			if (dependencyProperty != null)
			{
				return dependencyProperty.PropertyType;
			}

			DependencyObjectType dependencyObjectType = typeObject as DependencyObjectType;

			if (dependencyObjectType != null)
			{
				return dependencyObjectType.SystemType;
			}
			
			//	The type object does not map to any known type description object.
			
			return null;
		}

		/// <summary>
		/// Gets the named type from type object.
		/// </summary>
		/// <param name="typeObject">The type object.</param>
		/// <returns>The named type.</returns>
		public static INamedType GetNamedTypeFromTypeObject(object typeObject)
		{
			INamedType namedType = typeObject as INamedType;

			if (namedType == null)
			{
				DependencyProperty dependencyProperty = typeObject as DependencyProperty;

				if (dependencyProperty != null)
				{
					namedType = dependencyProperty.DefaultMetadata.NamedType;
				}
				
				if (namedType == null)
				{
					System.Type systemType = TypeRosetta.GetSystemTypeFromTypeObject (typeObject);

					if (systemType == null)
					{
						//	No underlying System.Type exists for the specified type object.
						//	This is not a valid type object.

						throw new Exceptions.InvalidTypeObjectException (typeObject);
					}
					else
					{
						if (systemType.IsEnum)
						{
							namedType = EnumType.GetDefault (systemType);
						}
						else
						{
							if (systemType == typeof (int))
							{
								namedType = IntegerType.Default;
							}
							else if (systemType == typeof (long))
							{
								namedType = LongIntegerType.Default;
							}
							else if (systemType == typeof (double))
							{
								namedType = DoubleType.Default;
							}
							else if (systemType == typeof (void))
							{
								namedType = VoidType.Default;
							}
							else if (systemType == typeof (string))
							{
								namedType = StringType.Default;
							}
							else
							{
								namedType = new AutomaticNamedType (systemType);
							}
						}
					}
				}
			}

			System.Diagnostics.Debug.Assert (namedType != null);

			return namedType;
		}

		/// <summary>
		/// Gets the structured type from type object. The <see cref="T:IStructuredType"/> can
		/// then be used to query the type about its fields.
		/// </summary>
		/// <param name="type">The type object to convert.</param>
		/// <returns>The structured type compatible with the type object, or <c>null</c> if it
		/// canoot be derived from the type object.</returns>
		public static IStructuredType GetStructuredTypeFromTypeObject(object type)
		{
			IStructuredType structuredType = type as IStructuredType;

			if (structuredType != null)
			{
				return structuredType;
			}
			
			IStructuredTypeProvider structuredTypeProvider = type as IStructuredTypeProvider;

			if (structuredTypeProvider != null)
			{
				return structuredTypeProvider.GetStructuredType ();
			}

			return null;
		}
		
		/// <summary>
		/// Gets the type object from a value.
		/// </summary>
		/// <param name="value">The value to derive a type from.</param>
		/// <returns>The type object.</returns>
		public static object GetTypeObjectFromValue(object value)
		{
			if (value == null)
			{
				return null;
			}

			DependencyObject dependencyObject = value as DependencyObject;

			if (dependencyObject != null)
			{
				object type = TypeRosetta.GetTypeObject (dependencyObject);

				if (type != null)
				{
					return type;
				}
			}
			
			IStructuredTypeProvider structuredTypeProvider = value as IStructuredTypeProvider;

			if (structuredTypeProvider != null)
			{
				return structuredTypeProvider.GetStructuredType ();
			}
			
			if (dependencyObject != null)
			{
				return dependencyObject.ObjectType;
			}

			IStructuredData structuredData = value as IStructuredData;

			if (structuredData != null)
			{
				return new DynamicStructuredType (structuredData);
			}

			return value.GetType ();
		}

		/// <summary>
		/// Verifies the validity of the value (proper type and valid with respect
		/// to <see cref="T:IDataConstraint"/>).
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="typeObject">The expected type object (may not be <c>null</c>).</param>
		/// <returns>
		/// 	<c>true</c> if the value is compatible with the specified type, <c>false</c> otherwise.
		/// </returns>
		public static bool IsValidValue(object value, object typeObject)
		{
			if (typeObject == null)
			{
				throw new System.ArgumentNullException ("Null type specified");
			}
			
			INamedType      targetType       = TypeRosetta.GetNamedTypeFromTypeObject (typeObject);
			System.Type     targetSysType    = TypeRosetta.GetSystemTypeFromTypeObject (typeObject);
			IDataConstraint targetConstraint = targetType as IDataConstraint;

			System.Diagnostics.Debug.Assert (targetType != null);

			//	A DependencyProperty implements IDataConstraint too, so there is
			//	no need to check explicitely for dependency properties :
			
			if (targetConstraint != null)
			{
				return targetConstraint.IsValidValue (value);
			}

			if (value == null)
			{
				//	Only reference types can be set to null.

				//	TODO: check for nullable types too ?

				return targetSysType.IsClass;
			}
			else
			{
				return targetSysType.IsAssignableFrom (value.GetType ());
			}
		}

		/// <summary>
		/// Verifies the validity of the value (proper type and valid with respect
		/// to <see cref="T:IDataConstraint"/>).
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="targetType">The expected named type (may not be <c>null</c>).</param>
		/// <returns>
		/// 	<c>true</c> if the value is compatible with the specified type, <c>false</c> otherwise.
		/// </returns>
		public static bool IsValidValue(object value, INamedType targetType)
		{
			if (targetType == null)
			{
				throw new System.ArgumentNullException ("Null type specified");
			}

			System.Type     targetSysType    = targetType.SystemType;
			IDataConstraint targetConstraint = targetType as IDataConstraint;

			System.Diagnostics.Debug.Assert (targetType != null);

			if (targetConstraint != null)
			{
				return targetConstraint.IsValidValue (value);
			}

			if (value == null)
			{
				//	Only reference types can be set to null.

				//	TODO: check for nullable types too ?

				return targetSysType.IsClass;
			}
			else
			{
				return targetSysType.IsAssignableFrom (value.GetType ());
			}
		}


		/// <summary>
		/// Gets the type object with the specified name.
		/// </summary>
		/// <param name="name">The type object name.</param>
		/// <returns>The type object or <c>null</c> if no matching type object can be found.</returns>
		public static AbstractType GetTypeObject(string name)
		{
			TypeRosetta.InitializeNamedTypes ();

			AbstractType type;

			if (TypeRosetta.namedTypes.TryGetValue (name, out type))
			{
				return type;
			}
			
			return null;
		}

		public static AbstractType CreateTypeObject(Caption caption)
		{
			AbstractType type = null;
			
			switch (AbstractType.GetSystemType (caption))
			{
				case "System.Boolean":
					type = new BooleanType (caption);
					break;
				
				case "System.Decimal":
					type = new DecimalType (caption);
					break;
				
				case "System.Double":
					type = new DoubleType (caption);
					break;

				case "System.Int32":
					type = new IntegerType (caption);
					break;

				case "System.Int64":
					type = new LongIntegerType (caption);
					break;

				case "System.String":
					type = new StringType (caption);
					break;

				case "System.Void":
					type = new VoidType (caption);
					break;

			}
			
			return type;
		}

		private static void InitializeNamedTypes()
		{
			if (TypeRosetta.namedTypes == null)
			{
				lock (TypeRosetta.globalExclusion)
				{
					if (TypeRosetta.namedTypes == null)
					{
						Dictionary<string, AbstractType> dict = new Dictionary<string, AbstractType> ();

						TypeRosetta.InitializeDictionaryWithDefaultTypes (dict);
						
						TypeRosetta.namedTypes = dict;
					}
				}
			}
		}

		private static void InitializeDictionaryWithDefaultTypes(Dictionary<string, AbstractType> dict)
		{
			//	Fill the dictionary with the known default types. This defines the
			//	following basic .NET types :
			//
			//	- bool
			//	- decimal, double, int, long
			//	- string
			//	- void
			
			TypeRosetta.AddType (dict, BooleanType.Default);
			TypeRosetta.AddType (dict, DecimalType.Default);
			TypeRosetta.AddType (dict, DoubleType.Default);
			TypeRosetta.AddType (dict, IntegerType.Default);
			TypeRosetta.AddType (dict, LongIntegerType.Default);
			TypeRosetta.AddType (dict, StringType.Default);
			TypeRosetta.AddType (dict, VoidType.Default);
		}

		private static void AddType(Dictionary<string, AbstractType> dict, AbstractType type)
		{
			type.LockName ();
			dict.Add (type.Name, type);
		}

		#region AutomaticNamedType Class

		private class AutomaticNamedType : INamedType
		{
			public AutomaticNamedType(System.Type type)
			{
				this.type = type;
			}

			#region INamedType Members

			public string DefaultController
			{
				get
				{
					return null;
				}
			}

			public string DefaultControllerParameter
			{
				get
				{
					return null;
				}
			}

			#endregion
			
			#region ISystemType Members

			public System.Type SystemType
			{
				get
				{
					return this.type;
				}
			}

			#endregion

			#region INameCaption Members

			public Support.Druid CaptionId
			{
				get
				{
					return Support.Druid.Empty;
				}
			}

			#endregion

			#region IName Members

			public string Name
			{
				get
				{
					return this.type.FullName;
				}
			}

			#endregion

			private System.Type type;
		}
		
		#endregion

		#region Tools Class

		private class Properties : DependencyObject
		{
		}

		#endregion

		/// <summary>
		/// Gets the attached type object from the dependency object.
		/// </summary>
		/// <param name="obj">The dependency object.</param>
		/// <returns>The attached type object or <c>null</c> if none can be found.</returns>
		public static object GetTypeObject(DependencyObject obj)
		{
			return obj.GetValue (TypeRosetta.TypeObjectProperty);
		}

		/// <summary>
		/// Sets the attached type object to the dependency object.
		/// </summary>
		/// <param name="obj">The dependency object.</param>
		/// <param name="value">The type object to attach.</param>
		public static void SetTypeObject(DependencyObject obj, object value)
		{
			obj.SetValue (TypeRosetta.TypeObjectProperty, value);
		}

		/// <summary>
		/// Clears the attached type object from the dependency object.
		/// </summary>
		/// <param name="obj">The dependency object.</param>
		public static void ClearTypeObject(DependencyObject obj)
		{
			obj.ClearValue (TypeRosetta.TypeObjectProperty);
		}

		/// <summary>
		/// Verifies if the type implements the specified interface.
		/// </summary>
		/// <param name="systemType">Type to check.</param>
		/// <param name="interfaceType">Type of the interface to find.</param>
		/// <returns><c>true</c> if the interface is found; otherwise, <c>false</c>.</returns>
		public static bool DoesTypeImplementInterface(System.Type systemType, System.Type interfaceType)
		{
			if (systemType == interfaceType)
			{
				return true;
			}
			else
			{
				foreach (System.Type type in systemType.GetInterfaces ())
				{
					if (type == interfaceType)
					{
						return true;
					}
				}
			}

			return false;
		}


		/// <summary>
		/// Verifies if the type implements the specified generic interface.
		/// </summary>
		/// <param name="type">Type to check.</param>
		/// <param name="name">The name of the interface to find.</param>
		/// <returns><c>true</c> if the interface is found; otherwise, <c>false</c>.</returns>
		public static bool DoesTypeImplementGenericInterface(System.Type type, string name)
		{
			System.Type[] types = type.GetInterfaces ();

			name = string.Concat (name, "`");

			foreach (System.Type item in types)
			{
				if ((item.Name.StartsWith (name)) &&
					(item.IsGenericType))
				{
					return true;
				}
			}

			return false;
		}
		
		public static readonly DependencyProperty TypeObjectProperty = DependencyProperty.RegisterAttached ("TypeObject", typeof (object), typeof (TypeRosetta.Properties));

		private static object globalExclusion = new object ();

		private static Dictionary<string, AbstractType> namedTypes;
	}
}
