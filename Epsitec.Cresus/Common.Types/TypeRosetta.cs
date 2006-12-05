//	Copyright � 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
			if (typeObject == null)
			{
				return null;
			}
			
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
							if (systemType == typeof (bool))
							{
								namedType = BooleanType.Default;
							}
							else if (systemType == typeof (int))
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
							else if (systemType == typeof (decimal))
							{
								namedType = DecimalType.Default;
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
								IStructuredType structuredType = typeObject as IStructuredType;

								if (structuredType ==  null)
								{
									namedType = new AutomaticNamedType (systemType);
								}
								else
								{
									namedType = new AutomaticStructuredNamedType (systemType, structuredType);
								}
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
			if (typeObject is StructuredTypeField)
			{
				StructuredTypeField field = (StructuredTypeField) typeObject;
				return TypeRosetta.IsValidValue (value, field.Type);
			}
			
			if (UnknownValue.IsUnknownValue (value))
			{
				return false;
			}
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
			if (UnknownValue.IsUnknownValue (value))
			{
				return false;
			}
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
				INullableType nullable = targetType as INullableType;

				if (nullable != null)
				{
					//	If the type implements INullable, it specifies support
					//	for null values explicitely :
					
					return nullable.IsNullable;
				}
				else
				{
					//	Only reference types can be set to null.
					
					return targetSysType.IsClass;
				}
			}
			else
			{
				return targetSysType.IsAssignableFrom (value.GetType ());
			}
		}

		public static bool IsValidValue(object value, StructuredTypeField field)
		{
			if (UnknownValue.IsUnknownValue (value))
			{
				return false;
			}
			if (field.IsEmpty)
			{
				throw new System.ArgumentNullException ("Empty field specified");
			}

			switch (field.Relation)
			{
				case Relation.None:
				case Relation.Reference:
				case Relation.Bijective:
					return TypeRosetta.IsValidValue (value, field.Type);

				case Relation.Collection:
					return TypeRosetta.IsValidValueForCollectionOfType (value, field.Type);

				default:
					throw new System.NotImplementedException (string.Format ("Support for Relation.{0} not implemented", field.Relation));
			}
		}

		public static bool IsValidValueForCollectionOfType(object value, INamedType namedType)
		{
			System.Type     typeOfItems    = namedType.SystemType;
			IStructuredType structuredType = namedType as IStructuredType;

			if (structuredType != null)
			{
				typeOfItems = typeof (StructuredData);
			}
			
			if (typeOfItems == null)
			{
				throw new System.NotSupportedException (string.Format ("Unsupported null system type in collection validation for {0}", namedType.Name));
			}

			if (! TypeRosetta.IsValidValueForCollectionOfType (value, typeOfItems))
			{
				return false;
			}
			else if (structuredType == null)
			{
				return true;
			}
			else
			{
				//	The value is of the right collection type, but maybe its
				//	content is not valid ? Just check one item to probe for
				//	this hypothesis :

				System.Collections.IEnumerable enumerable = value as System.Collections.IEnumerable;

				if (enumerable != null)
				{
					System.Collections.IEnumerator enumerator = enumerable.GetEnumerator ();
					
					if (enumerator.MoveNext ())
					{
						StructuredData item = enumerator.Current as StructuredData;
						
						if (item != null)
						{
							return StructuredType.HaveCompatibleFields (item.StructuredType, structuredType);
						}
					}
				}
				
				return true;
			}
		}
		
		public static bool IsValidValueForCollectionOfType(object value, System.Type typeOfItems)
		{
			if (value == null)
			{
				return false;
			}

			System.Type typeOfValue = value.GetType ();
			System.Type interfaceType;

			if ((TypeRosetta.DoesTypeImplementGenericInterface (typeOfValue, typeof (IList<>), out interfaceType)) ||
				(TypeRosetta.DoesTypeImplementGenericInterface (typeOfValue, typeof (ICollection<>), out interfaceType)))
			{
				System.Type[] genericArguments = interfaceType.GetGenericArguments ();
				System.Diagnostics.Debug.Assert (genericArguments.Length == 1);

				return typeOfItems.IsAssignableFrom (genericArguments[0]);
			}

			if (TypeRosetta.DoesTypeImplementInterface (typeOfValue, typeof (System.Collections.IList)))
			{
				return true;
			}
			
			return false;
		}


		/// <summary>
		/// Gets the type object with the specified name.
		/// </summary>
		/// <param name="name">The type object name.</param>
		/// <returns>The type object or <c>null</c> if no matching type object can be found.</returns>
		public static AbstractType GetTypeObject(string name)
		{
			TypeRosetta.InitializeKnownTypes ();

			lock (TypeRosetta.knownTypes)
			{
				foreach (AbstractType type in TypeRosetta.knownTypes.Values)
				{
					if (type.Name == name)
					{
						return type;
					}
				}
			}

			return null;
		}

		/// <summary>
		/// Gets the type object with the specified type id.
		/// </summary>
		/// <param name="typeId">The type object DRUID.</param>
		/// <returns>The type object or <c>null</c> if no matching type object can be found.</returns>
		public static AbstractType GetTypeObject(Support.Druid typeId)
		{
			TypeRosetta.InitializeKnownTypes ();

			lock (TypeRosetta.knownTypes)
			{
				AbstractType type;

				if (TypeRosetta.knownTypes.TryGetValue (typeId, out type))
				{
					return type;
				}
				else
				{
					return null;
				}
			}
		}

		/// <summary>
		/// Gets the type object described by the specified caption. This
		/// relies on an internal cache to speed up accesses.
		/// </summary>
		/// <param name="caption">The caption defining a type object.</param>
		/// <returns>The type object or <c>null</c> if no matching type object can be found.</returns>
		public static AbstractType GetTypeObject(Caption caption)
		{
			if (caption == null)
			{
				throw new System.ArgumentNullException ("caption");
			}

			AbstractType type = AbstractType.GetCachedType (caption);

			if (type != null)
			{
				return type;
			}

			type = TypeRosetta.GetTypeObject (caption.Id);

			if (type == null)
			{
				type = TypeRosetta.CreateTypeObject (caption);

				System.Diagnostics.Debug.Assert ((type == null) || (TypeRosetta.knownTypes.ContainsKey (caption.Id)));
			}

			if (type != null)
			{
				AbstractType.SetCachedType (caption, type);
			}
			
			return type;
		}

		/// <summary>
		/// Creates the type object for the specified type id.
		/// </summary>
		/// <param name="druid">The type object DRUID.</param>
		/// <returns>The type object or <c>null</c> if the type object cannot be created.</returns>
		public static AbstractType CreateTypeObject(Support.Druid druid)
		{
			Caption caption = Support.Resources.DefaultManager.GetCaption (druid);
			
			return TypeRosetta.CreateTypeObject (caption);
		}

		/// <summary>
		/// Creates the type object for the specified caption.
		/// </summary>
		/// <param name="caption">The caption used to store the type object definition.</param>
		/// <returns>
		/// The type object or <c>null</c> if the type object cannot be created.
		/// </returns>
		public static AbstractType CreateTypeObject(Caption caption)
		{
			TypeRosetta.InitializeKnownTypes ();

			if (caption == null)
			{
				throw new System.ArgumentNullException ("caption");
			}

			lock (TypeRosetta.knownTypes)
			{
				try
				{
					TypeRosetta.creatingTypeObject++;
					return TypeRosetta.LockedCreateTypeObject (caption);
				}
				finally
				{
					TypeRosetta.creatingTypeObject--;
				}
			}
		}

		#region Low level type management code

		private static AbstractType LockedCreateTypeObject(Caption caption)
		{
			Support.Druid typeId = caption.Id;
			AbstractType  type   = null;
			
			string systemTypeName = AbstractType.GetSystemType (caption);

			if (string.IsNullOrEmpty (systemTypeName))
			{
				//	This is not a simple type, based on one of the well known .NET
				//	types. The type definition was serialized and attached to the
				//	caption as a complex object type :

				type = AbstractType.GetComplexType (caption);

				//	Re-bind the caption with the type, when needed (e.g. if the
				//	serialization of the complex object type does not keep track
				//	of the associated caption) :

				if (type.IsCaptionDefined == false)
				{
					type.DefineCaption (caption);
				}
			}
			else
			{
				string simpleTypeName = (systemTypeName.Length > 2) && (systemTypeName[1] == ':') ? systemTypeName.Substring (2) : systemTypeName;

				switch (simpleTypeName)
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

					case "System.Byte[]":
						type = new BinaryType (caption);
						break;

					case "System.DateTime":
						type = new DateTimeType (caption);
						break;

					case "Epsitec.Common.Types.Date, Common.Types":
						type = new DateType (caption);
						break;

					case "Epsitec.Common.Types.Time, Common.Types":
						type = new TimeType (caption);
						break;

					case "System.Collections.IEnumerable":
						type = new CollectionType (caption);
						break;
					
					default:
						type = TypeRosetta.CreateSystemTypeBasedTypeObject (systemTypeName, caption);
						break;
				}
			}

			if ((type != null) &&
				(typeId.IsValid))
			{
				//	Check if the type is already known. If not, record a reference to
				//	it so that we can then access it by its DRUID or by its name :

				if (TypeRosetta.knownTypes.ContainsKey (typeId) == false)
				{
					TypeRosetta.knownTypes[typeId] = type;
				}
			}

			TypeRosetta.ExecuteFixUps ();
			
			return type;
		}

		private static AbstractType CreateSystemTypeBasedTypeObject(string systemTypeName, Caption caption)
		{
			System.Type systemType = AbstractType.GetSystemTypeFromSystemTypeName (systemTypeName);

			if (systemType == null)
			{
				//	System.Type cannot be resolved in the current context (a required assembly
				//	might be missing).

				SystemTypeFamily family = AbstractType.GetSystemTypeFamilyFromSystemTypeName (systemTypeName);

				if (family == SystemTypeFamily.Enum)
				{
					return new EnumType (typeof (UnresolvedEnum), caption);
				}

				return null;
			}

			if (systemType.IsEnum)
			{
				return new EnumType (systemType, caption);
			}
			
			return null;
		}

		internal static void InitializeKnownTypes()
		{
			if (TypeRosetta.knownTypes == null)
			{
				lock (TypeRosetta.globalExclusion)
				{
					if (TypeRosetta.knownTypes == null)
					{
						Dictionary<Support.Druid, AbstractType> dict = new Dictionary<Support.Druid, AbstractType> ();

						TypeRosetta.knownTypes = dict;
						
						TypeRosetta.InitializeDictionaryWithDefaultTypes (dict);
					}
				}
			}
		}

		private static void InitializeDictionaryWithDefaultTypes(Dictionary<Support.Druid, AbstractType> dict)
		{
			//	Fill the dictionary with the known default types. This defines the
			//	following basic .NET types :
			//
			//	- bool
			//	- decimal, double, int, long
			//	- string
			//	- void
			
			TypeRosetta.AddType (BooleanType.Default);
			TypeRosetta.AddType (DecimalType.Default);
			TypeRosetta.AddType (DoubleType.Default);
			TypeRosetta.AddType (IntegerType.Default);
			TypeRosetta.AddType (LongIntegerType.Default);
			TypeRosetta.AddType (StringType.Default);
			TypeRosetta.AddType (VoidType.Default);
		}

		private static void AddType(AbstractType type)
		{
			type.Lock ();
		}

		#endregion

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

			#region ICaption Members

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

		#region AutomaticNamedType Class

		private class AutomaticStructuredNamedType : AutomaticNamedType, IStructuredType
		{
			public AutomaticStructuredNamedType(System.Type type, IStructuredType structuredType)
				: base (type)
			{
				this.structuredType = structuredType;
			}

			#region IStructuredType Members

			public IEnumerable<string> GetFieldIds()
			{
				return this.structuredType.GetFieldIds ();
			}

			public StructuredTypeField GetField(string fieldId)
			{
				return this.structuredType.GetField (fieldId);
			}

			#endregion

			private IStructuredType structuredType;
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
		/// <param name="systemType">Type of the system.</param>
		/// <param name="genericInterfaceType">Type of the generic interface.</param>
		/// <param name="interfaceType">Real type of the interface.</param>
		/// <returns>
		/// 	<c>true</c> if the interface is found; otherwise, <c>false</c>.
		/// </returns>
		public static bool DoesTypeImplementGenericInterface(System.Type systemType, System.Type genericInterfaceType, out System.Type interfaceType)
		{
			if ((systemType.IsGenericType) &&
				(systemType.IsGenericTypeDefinition == false) &&
				(systemType.GetGenericTypeDefinition () == genericInterfaceType))
			{
				interfaceType = genericInterfaceType;
				return true;
			}
			else
			{
				foreach (System.Type type in systemType.GetInterfaces ())
				{
					if ((type.IsGenericType) &&
						(type.IsGenericTypeDefinition == false) &&
						(type.GetGenericTypeDefinition () == genericInterfaceType))
					{
						interfaceType = type;
						return true;
					}
				}
			}
			
			interfaceType = null;
			return false;
		}


		/// <summary>
		/// Verifies if the type implements the specified generic interface.
		/// </summary>
		/// <param name="type">Type to check.</param>
		/// <param name="genericInterfaceType">Type of the generic interface.</param>
		/// <returns>
		/// 	<c>true</c> if the interface is found; otherwise, <c>false</c>.
		/// </returns>
		public static bool DoesTypeImplementGenericInterface(System.Type type, System.Type genericInterfaceType)
		{
			System.Type interfaceType;
			return TypeRosetta.DoesTypeImplementGenericInterface (type, genericInterfaceType, out interfaceType);
		}

		/// <summary>
		/// Queues a fix-up callback which must be called immediately after type
		/// creation.
		/// </summary>
		/// <param name="callback">The callback.</param>
		internal static void QueueFixUp(Support.SimpleCallback callback)
		{
			if (TypeRosetta.creatingTypeObject > 0)
			{
				TypeRosetta.fixUpQueue.Enqueue (callback);
			}
			else
			{
				try
				{
					TypeRosetta.creatingTypeObject++;
					
					callback ();
				}
				finally
				{
					TypeRosetta.creatingTypeObject--;
				}
			}
		}

		/// <summary>
		/// Executes the fix-ups queued by <c>QueueFixUp</c>.
		/// </summary>
		private static void ExecuteFixUps()
		{
			while (TypeRosetta.fixUpQueue.Count > 0)
			{
				Support.SimpleCallback callback = TypeRosetta.fixUpQueue.Dequeue ();
				callback ();
			}
		}

		public static readonly DependencyProperty TypeObjectProperty = DependencyProperty.RegisterAttached ("TypeObject", typeof (object), typeof (TypeRosetta.Properties));

		private static object globalExclusion = new object ();

		private static Dictionary<Support.Druid, AbstractType> knownTypes;
		private static Queue<Support.SimpleCallback> fixUpQueue = new Queue<Epsitec.Common.Support.SimpleCallback> ();
		private static int creatingTypeObject;
	}
}
