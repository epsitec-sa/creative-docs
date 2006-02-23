//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La classe ObjectType représente une version de plus haut niveau de
	/// System.Type.
	/// </summary>
	public sealed class ObjectType
	{
		private ObjectType(System.Type system_type, ObjectType base_type)
		{
			this.systemType = system_type;
			this.baseType   = base_type;

			ObjectType type = this.baseType;
			
			//	Register this type as a derived type on all standard properties
			//	defined by the base classes :
			
			while (type != null)
			{
				if (type.localStandardProperties != null)
				{
					foreach (Property property in type.localStandardProperties)
					{
						property.AddDerivedType (this.systemType);
					}
				}
				else if (type.standardPropertiesArray != null)
				{
					foreach (Property property in type.standardPropertiesArray)
					{
						property.AddDerivedType (this.systemType);
					}
					
					break;
				}
				
				type = type.baseType;
			}
		}
		
		public ObjectType						BaseType
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
		
		public bool IsObjectInstanceOfType(Object o)
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
		public bool IsSubclassOf(ObjectType type)
		{
			ObjectType base_type = this.BaseType;
			
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
		
		public void Register(Property property)
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

		public ReadOnlyArray<Property> GetProperties()
		{
			if (this.standardPropertiesArray == null)
			{
				this.BuildPropertyList ();
			}

			return new ReadOnlyArray<Property> (this.standardPropertiesArray);
		}
		public ReadOnlyArray<Property> GetAttachedProperties()
		{
			if (this.attachedPropertiesArray == null)
			{
				this.BuildPropertyList ();
			}

			return new ReadOnlyArray<Property> (this.attachedPropertiesArray);
		}

		public Property GetProperty(string name)
		{
			if (this.lookup == null)
			{
				this.BuildPropertyList ();
			}

			if (this.lookup.ContainsKey (name))
			{
				return this.lookup[name];
			}
			
			//	TODO: trouver une propriété attachée qui conviendrait
			
			return null;
		}
		public Property GetProperty(Property property)
		{
			if (this.lookup == null)
			{
				this.BuildPropertyList ();
			}

			for (int i = 0; i < this.standardPropertiesArray.Length; i++)
			{
				if (this.standardPropertiesArray[i] == property)
				{
					return property;
				}
			}
			
			return null;
		}

		public static ObjectType FromSystemType(System.Type type)
		{
			lock (ObjectType.types)
			{
				return ObjectType.FromSystemTypeLocked (type);
			}
		}

		internal static ObjectType SetupFromSystemType(System.Type type)
		{
			ObjectType objectType = ObjectType.FromSystemType (type);

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

		#region Private Methods
		private void BuildPropertyList()
		{
			lock (this)
			{
				if (this.standardPropertiesArray == null)
				{
					ICollection<Property> baseProperties;

					if (this.BaseType == null)
					{
						baseProperties = new Property[0];
					}
					else
					{
						baseProperties = this.BaseType.GetProperties ();
					}

					Property[] allProperties  = new Property[baseProperties.Count + this.localStandardProperties.Count];

					baseProperties.CopyTo (allProperties, 0);
					this.localStandardProperties.CopyTo (allProperties, baseProperties.Count);

					this.standardPropertiesArray = allProperties;
					this.attachedPropertiesArray = this.localAttachedProperties.ToArray ();
					
					this.localStandardProperties = null;
					this.localAttachedProperties = null;

					this.lookup = new Dictionary<string, Property> (this.standardPropertiesArray.Length);

					foreach (Property property in this.standardPropertiesArray)
					{
						this.lookup[property.Name] = property;
					}
					foreach (Property property in this.attachedPropertiesArray)
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
					if (infos[i].FieldType == typeof (Property))
					{
						System.Diagnostics.Debug.WriteLine (string.Format ("Initialized type {0}", this.Name));
						infos[i].GetValue (null);
						break;
					}
				}
			}
		}

		private static ObjectType FromSystemTypeLocked(System.Type system_type)
		{
			if (system_type == null)
			{
				return null;
			}
			else if (ObjectType.types.ContainsKey (system_type))
			{
				return ObjectType.types[system_type];
			}
			else if (system_type == typeof (Object))
			{
				ObjectType this_type = new ObjectType (system_type, null);
				
				ObjectType.types[system_type] = this_type;

				this_type.ExecuteTypeStaticConstructor ();
				
				return this_type;
			}
			else
			{
				ObjectType base_type = ObjectType.FromSystemTypeLocked (system_type.BaseType);
				
				if (base_type == null)
				{
					return null;
				}
				
				ObjectType this_type = new ObjectType (system_type, base_type);
				
				ObjectType.types[system_type] = this_type;

				this_type.ExecuteTypeStaticConstructor ();
				
				return this_type;
			}
		}

		private void InitializeLocked()
		{
			
		}
		#endregion


		private ObjectType						baseType;
		private System.Type						systemType;
		private List<Property>					localStandardProperties = new List<Property> ();
		private List<Property>					localAttachedProperties = new List<Property> ();
		private Property[]						standardPropertiesArray;
		private Property[]						attachedPropertiesArray;
		private Dictionary<string, Property>	lookup;
		private bool							initialized;

		static Dictionary<System.Type, ObjectType> types = new Dictionary<System.Type, ObjectType> ();
	}
}
