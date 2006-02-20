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
			System.Diagnostics.Debug.Assert (property.OwnerType == this.SystemType);
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
		
		public static ObjectType FromSystemType(System.Type type)
		{
			lock (ObjectType.types)
			{
				return ObjectType.FromSystemTypeLocked (type);
			}
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
				
				return this_type;
			}
		}
		#endregion


		private ObjectType						baseType;
		private System.Type						systemType;
		private List<Property>					localStandardProperties = new List<Property> ();
		private List<Property>					localAttachedProperties = new List<Property> ();
		private Property[]						standardPropertiesArray;
		private Property[]						attachedPropertiesArray;
		private Dictionary<string, Property>	lookup;

		static Dictionary<System.Type, ObjectType> types = new Dictionary<System.Type, ObjectType> ();
	}
}
