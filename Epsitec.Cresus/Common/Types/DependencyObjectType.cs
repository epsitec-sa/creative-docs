//	Copyright © 2005-2011, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>DependencyObjectType</c> is a higher level version of the
	/// <see cref="T:System.Type"/> class.
	/// </summary>
	public sealed class DependencyObjectType : IStructuredType, IStructuredTypeProvider
	{
		private DependencyObjectType(System.Type systemType, DependencyObjectType baseType)
		{
			this.systemType = systemType;
			this.baseType   = baseType;

			DependencyObjectType type = this.baseType;
			
			//	Register this type as a derived type on all standard properties
			//	defined by the base classes :
			
			while (type != null)
			{
				if (type.localStandardProperties != null)
				{
					foreach (DependencyProperty property in type.localStandardProperties)
					{
						property.AddDerivedType (this.systemType);
					}
				}
				else if (type.standardPropertiesArray != null)
				{
					foreach (DependencyProperty property in type.standardPropertiesArray)
					{
						property.AddDerivedType (this.systemType);
					}
					
					break;
				}
				
				type = type.baseType;
			}
		}
		
		public DependencyObjectType				BaseType
		{
			get
			{
				return this.baseType;
			}
		}
		public System.Type						SystemType
		{
			get
			{
				return this.systemType;
			}
		}
		public string							Name
		{
			get
			{
				return this.systemType.Name;
			}
		}
		public bool								IsStaticClass
		{
			get
			{
				return this.systemType.IsAbstract && this.systemType.IsSealed;
			}
		}
		
		public bool IsObjectInstanceOfType(DependencyObject o)
		{
			if (o == null)
			{
				throw new System.ArgumentNullException ();
			}
			
			if (o.ObjectType == this)
			{
				return true;
			}
			if (o.ObjectType.IsSubclassOf (this))
			{
				return true;
			}
			
			return false;
		}
		public bool IsSubclassOf(DependencyObjectType type)
		{
			DependencyObjectType baseType = this.BaseType;
			
			if (baseType == type)
			{
				return true;
			}
			
			if ((baseType != null) &&
				(baseType.IsSubclassOf (type)))
			{
				return true;
			}
			
			return false;
		}
		
		public void Register(DependencyProperty property)
		{
			System.Diagnostics.Debug.Assert (property.IsOwnedBy (this.SystemType));
			System.Diagnostics.Debug.Assert (this.localStandardProperties != null);
			System.Diagnostics.Debug.Assert (this.localAttachedProperties != null);

			if (property.IsAttached)
			{
				this.localAttachedProperties.Add (property);
			}
			else
			{
				if (this.IsStaticClass)
                {
					//	Static classes may not define properties, unless they are attached
					//	properties.
					throw new Exceptions.WrongBaseTypeException (this.systemType);
                }

				this.localStandardProperties.Add (property);
			}
		}
		
		public DependencyObject CreateEmptyObject()
		{
			if (this.allocator == null)
			{
				lock (this)
				{
					if (this.allocator == null)
					{
						this.allocator = Support.DynamicCodeFactory.CreateAllocator<DependencyObject> (this.systemType);
					}
				}
			}
			
			//	The allocator invokes a dynamically generated piece of IL code
			//	which does simply 'new' the associated type. With .NET 2.0, this
			//	is 40 times faster than the equivalent System.Activator call; it
			//	takes less than 0.5 μs on a 3GHz Pentium-D system.
			
			return this.allocator ();
		}

		public Collections.ReadOnlyList<DependencyProperty> GetProperties()
		{
			if (this.standardPropertiesArray == null)
			{
				this.BuildPropertyList ();
			}

			return new Collections.ReadOnlyList<DependencyProperty> (this.standardPropertiesArray);
		}
		public Collections.ReadOnlyList<DependencyProperty> GetAttachedProperties()
		{
			if (this.attachedPropertiesArray == null)
			{
				this.BuildPropertyList ();
			}

			return new Collections.ReadOnlyList<DependencyProperty> (this.attachedPropertiesArray);
		}

		public DependencyProperty GetProperty(string name)
		{
			if (this.lookup == null)
			{
				this.BuildPropertyList ();
			}

			DependencyProperty property;

			if (this.lookup.TryGetValue (name, out property))
			{
				return property;
			}
			
			//	TODO: trouver une propriété attachée qui conviendrait
			
			return null;
		}

		public static DependencyObjectType FromType<T>()
			where T : DependencyObject
		{
			return DependencyObjectType.FromSystemType (typeof (T));
		}

		public static DependencyObjectType FromSystemType(System.Type type)
		{
			lock (DependencyObjectType.types)
			{
				return DependencyObjectType.FromSystemTypeLocked (type);
			}
		}

		internal static DependencyObjectType SetupFromSystemType(System.Type type)
		{
			DependencyObjectType objectType = DependencyObjectType.FromSystemType (type);

			if (objectType.initialized == false)
			{
				lock (objectType)
				{
					if (objectType.initialized == false)
					{
						objectType.InitializeLocked ();
						objectType.initialized = true;
					}
				}
			}
			
			return objectType;
		}

		#region IStructuredType Members

		StructuredTypeField IStructuredType.GetField(string fieldId)
		{
			DependencyProperty property  = this.GetProperty (fieldId);
			INamedType         namedType = TypeRosetta.GetNamedTypeFromTypeObject (property);
			Support.Druid      captionId = property == null ? Support.Druid.Empty : property.CaptionId;

			return new StructuredTypeField (fieldId, namedType, captionId);
		}
		
		IEnumerable<string> IStructuredType.GetFieldIds()
		{
			Collections.ReadOnlyList<DependencyProperty> properties = this.GetProperties ();
			string[] names = new string[properties.Count];

			for (int i = 0; i < properties.Count; i++)
			{
				names[i] = properties[i].Name;
			}

			return names;
		}

		StructuredTypeClass IStructuredType.GetClass()
		{
			return StructuredTypeClass.None;
		}

		#endregion

		#region IStructuredTypeProvider Members

		IStructuredType IStructuredTypeProvider.GetStructuredType()
		{
			return this;
		}

		#endregion
		
		#region Private Methods
		private void BuildPropertyList()
		{
			lock (this)
			{
				if (this.standardPropertiesArray == null)
				{
					ICollection<DependencyProperty> baseProperties;

					if (this.BaseType == null)
					{
						baseProperties = new DependencyProperty[0];
					}
					else
					{
						baseProperties = this.BaseType.GetProperties ();
					}

					DependencyProperty[] allProperties  = new DependencyProperty[baseProperties.Count + this.localStandardProperties.Count];

					baseProperties.CopyTo (allProperties, 0);
					this.localStandardProperties.CopyTo (allProperties, baseProperties.Count);

					this.standardPropertiesArray = allProperties;
					this.attachedPropertiesArray = this.localAttachedProperties.ToArray ();
					
					this.localStandardProperties = null;
					this.localAttachedProperties = null;

					System.Array.Sort (this.standardPropertiesArray);
					System.Array.Sort (this.attachedPropertiesArray);

					this.lookup = new Dictionary<string, DependencyProperty> (this.standardPropertiesArray.Length);

					foreach (DependencyProperty property in this.standardPropertiesArray)
					{
						this.lookup[property.Name] = property;
					}
					foreach (DependencyProperty property in this.attachedPropertiesArray)
					{
						this.lookup[property.Name] = property;
					}
				}
			}
		}
		
		private void ExecuteTypeStaticConstructor()
		{
			System.Reflection.FieldInfo[] infos = this.systemType.GetFields (System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);

			if (infos.Length > 0)
			{
				for (int i = 0; i < infos.Length; i++)
				{
					if (infos[i].FieldType == typeof (DependencyProperty))
					{
						DependencyObjectType.typeStaticConstructorExecuting++;
						
						try
						{
//							System.Diagnostics.Debug.WriteLine (string.Format ("Initialized type {0}", this.Name));
							infos[i].GetValue (null);
							break;
						}
						finally
						{
							DependencyObjectType.typeStaticConstructorExecuting--;
							Serialization.DependencyClassManager.ExecutePendingInitializationCode ();
						}
					}
				}
			}
		}

		internal static bool IsExecutingStaticConstructor
		{
			get
			{
				return DependencyObjectType.typeStaticConstructorExecuting > 0;
			}
		}

		private static DependencyObjectType FromSystemTypeLocked(System.Type systemType)
		{
			if (systemType == null)
			{
				return null;
			}

			DependencyObjectType objectType;
			
			if (DependencyObjectType.types.TryGetValue (systemType, out objectType))
			{
				return objectType;
			}
			else if (systemType == typeof (DependencyObject))
			{
				DependencyObjectType thisType = new DependencyObjectType (systemType, null);
				
				DependencyObjectType.types[systemType] = thisType;

				thisType.ExecuteTypeStaticConstructor ();
				
				return thisType;
			}
			else if (systemType == typeof (object))
			{
				return null;
			}
			else
			{
				DependencyObjectType baseType = DependencyObjectType.FromSystemTypeLocked (systemType.BaseType);
				
				if (baseType == null)
				{
					if ((systemType.IsAbstract) &&
						(systemType.IsSealed))
					{
						//	Sealed & abstract means 'static class' : OK to have a static class registered with a
						//	DependencyObjectType.
					}
					else
					{
						throw new Exceptions.WrongBaseTypeException (systemType);
					}
				}
				
				DependencyObjectType thisType = new DependencyObjectType (systemType, baseType);
				
				DependencyObjectType.types[systemType] = thisType;

				thisType.ExecuteTypeStaticConstructor ();
				
				return thisType;
			}
		}

		private void InitializeLocked()
		{
			
		}
		#endregion

		[System.ThreadStatic]
		private static int typeStaticConstructorExecuting;

		private static readonly Dictionary<System.Type, DependencyObjectType> types = new Dictionary<System.Type, DependencyObjectType> ();

		
		private readonly DependencyObjectType			baseType;
		private readonly System.Type					systemType;
		private List<DependencyProperty>				localStandardProperties = new List<DependencyProperty> ();
		private List<DependencyProperty>				localAttachedProperties = new List<DependencyProperty> ();
		private DependencyProperty[]					standardPropertiesArray;
		private DependencyProperty[]					attachedPropertiesArray;
		private Dictionary<string, DependencyProperty>	lookup;
		private bool									initialized;
		private Support.Allocator<DependencyObject>		allocator;
	}
}
