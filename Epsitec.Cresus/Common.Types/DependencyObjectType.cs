//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>DependencyObjectType</c> is a higher level version of the
	/// <see cref="T:System.Type"/> class.
	/// </summary>
	public sealed class DependencyObjectType : IStructuredType
	{
		private DependencyObjectType(System.Type system_type, DependencyObjectType base_type)
		{
			this.systemType = system_type;
			this.baseType   = base_type;

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
			DependencyObjectType base_type = this.BaseType;
			
			if (base_type == type)
			{
				return true;
			}
			
			if ((base_type != null) &&
				(base_type.IsSubclassOf (type)))
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
						this.BuildDynamicAllocator ();
					}
				}
			}
			
			//	The allocator invokes a dynamically generated piece of IL code
			//	which does simply 'new' the associated type. With .NET 2.0, this
			//	is 40 times faster than the equivalent System.Activator call; it
			//	takes less than 0.5 μs on a 3GHz Pentium-D system.
			
			return this.allocator ();
		}

		public ReadOnlyArray<DependencyProperty> GetProperties()
		{
			if (this.standardPropertiesArray == null)
			{
				this.BuildPropertyList ();
			}

			return new ReadOnlyArray<DependencyProperty> (this.standardPropertiesArray);
		}
		public ReadOnlyArray<DependencyProperty> GetAttachedProperties()
		{
			if (this.attachedPropertiesArray == null)
			{
				this.BuildPropertyList ();
			}

			return new ReadOnlyArray<DependencyProperty> (this.attachedPropertiesArray);
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

		object IStructuredType.GetFieldTypeObject(string name)
		{
			return this.GetProperty (name);
		}

		string[] IStructuredType.GetFieldNames()
		{
			ReadOnlyArray<DependencyProperty> properties = this.GetProperties ();
			string[] names = new string[properties.Count];

			for (int i = 0; i < properties.Count; i++)
			{
				names[i] = properties[i].Name;
			}

			return names;
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
		private void BuildDynamicAllocator()
		{
			//	Create a small piece of dynamic code which does simply "new T()"
			//	for the underlying system type. This code relies on lightweight
			//	code generation and results in a very fast dynamic allocator.
			
			System.Reflection.Module module = typeof (DependencyObjectType).Module;
			System.Reflection.Emit.DynamicMethod dynamicMethod = new System.Reflection.Emit.DynamicMethod ("DynamicAllocator", this.systemType, System.Type.EmptyTypes, module, true);
			System.Reflection.Emit.ILGenerator ilGen = dynamicMethod.GetILGenerator ();

			ilGen.Emit (System.Reflection.Emit.OpCodes.Nop);
			ilGen.Emit (System.Reflection.Emit.OpCodes.Newobj, this.systemType.GetConstructor (System.Type.EmptyTypes));
			ilGen.Emit (System.Reflection.Emit.OpCodes.Ret);

			this.allocator = (Allocator) dynamicMethod.CreateDelegate (typeof (Allocator));
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
//						System.Diagnostics.Debug.WriteLine (string.Format ("Initialized type {0}", this.Name));
						infos[i].GetValue (null);
						break;
					}
				}
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
				DependencyObjectType this_type = new DependencyObjectType (systemType, null);
				
				DependencyObjectType.types[systemType] = this_type;

				this_type.ExecuteTypeStaticConstructor ();
				
				return this_type;
			}
			else if (systemType == typeof (System.Object))
			{
				return null;
			}
			else
			{
				DependencyObjectType base_type = DependencyObjectType.FromSystemTypeLocked (systemType.BaseType);
				
				if (base_type == null)
				{
					throw new Exceptions.WrongBaseTypeException (systemType);
				}
				
				DependencyObjectType this_type = new DependencyObjectType (systemType, base_type);
				
				DependencyObjectType.types[systemType] = this_type;

				this_type.ExecuteTypeStaticConstructor ();
				
				return this_type;
			}
		}

		private void InitializeLocked()
		{
			
		}
		#endregion

		private delegate DependencyObject Allocator();

		private DependencyObjectType			baseType;
		private System.Type						systemType;
		private List<DependencyProperty>		localStandardProperties = new List<DependencyProperty> ();
		private List<DependencyProperty>		localAttachedProperties = new List<DependencyProperty> ();
		private DependencyProperty[]			standardPropertiesArray;
		private DependencyProperty[]			attachedPropertiesArray;
		private Dictionary<string, DependencyProperty>	lookup;
		private bool							initialized;
		private Allocator						allocator;

		static Dictionary<System.Type, DependencyObjectType> types = new Dictionary<System.Type, DependencyObjectType> ();
	}
}
