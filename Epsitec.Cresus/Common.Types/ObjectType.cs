//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

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
		
		
		public bool IsInstanceOfType(Object o)
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
		
		public Property[] GetProperties()
		{
			if (this.all_properties == null)
			{
				lock (this)
				{
					if (this.all_properties == null)
					{
						Property[] base_properties = this.BaseType == null ? new Property[0] : this.BaseType.GetProperties ();
						Property[] all_properties  = new Property[base_properties.Length + this.local_properties.Count];
						
						base_properties.CopyTo (all_properties, 0);
						this.local_properties.CopyTo (all_properties, base_properties.Length);
						
						this.all_properties   = all_properties;
						this.local_properties = null;
					}
				}
			}
			
			return this.all_properties;
		}
		
		
		public static ObjectType FromSystemType(System.Type type)
		{
			lock (ObjectType.types)
			{
				return ObjectType.FromSystemTypeLocked (type);
			}
		}
		
		
		private static ObjectType FromSystemTypeLocked(System.Type system_type)
		{
			if (system_type == null)
			{
				return null;
			}
			else if (ObjectType.types.Contains (system_type))
			{
				return ObjectType.types[system_type] as ObjectType;
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
		
		
		private ObjectType						base_type;
		private System.Type						system_type;
		private System.Collections.ArrayList	local_properties = new System.Collections.ArrayList ();
		private Property[]						all_properties;
		
		static System.Collections.Hashtable		types = new System.Collections.Hashtable ();
	}
}
