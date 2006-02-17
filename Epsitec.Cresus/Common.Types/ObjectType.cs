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
			this.system_type = system_type;
			this.base_type   = base_type;
		}
		
		
		public ObjectType						BaseType
		{
			get
			{
				return this.base_type;
			}
		}
		public System.Type						SystemType
		{
			get
			{
				return this.system_type;
			}
		}
		public string							Name
		{
			get
			{
				return this.system_type.Name;
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
			System.Diagnostics.Debug.Assert (this.local_properties != null);
			
			this.local_properties.Add (property);
		}
		
		public ReadOnlyArray<Property> GetProperties()
		{
			if (this.all_properties == null)
			{
				this.BuildPropertyList ();
			}
			
			return new ReadOnlyArray<Property> (this.all_properties);
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
				if (this.all_properties == null)
				{
					ICollection<Property> base_properties;

					if (this.BaseType == null)
					{
						base_properties = new Property[0];
					}
					else
					{
						base_properties = this.BaseType.GetProperties ();
					}

					Property[] all_properties  = new Property[base_properties.Count + this.local_properties.Count];

					base_properties.CopyTo (all_properties, 0);
					this.local_properties.CopyTo (all_properties, base_properties.Count);

					this.all_properties   = all_properties;
					this.local_properties = null;

					this.lookup = new Dictionary<string, Property> (this.all_properties.Length);

					foreach (Property property in this.all_properties)
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


		private ObjectType						base_type;
		private System.Type						system_type;
		private List<Property>					local_properties = new List<Property> ();
		private Property[]						all_properties;
		private Dictionary<string, Property>	lookup;

		static Dictionary<System.Type, ObjectType> types = new Dictionary<System.Type, ObjectType> ();
	}
}
