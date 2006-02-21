//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// Property.
	/// </summary>
	public sealed class Property : System.IEquatable<Property>
	{
		private Property(string name, System.Type property_type, System.Type owner_type, PropertyMetadata metadata)
		{
			this.name = name;
			this.propertyType = property_type;
			this.ownerType = owner_type;
			this.defaultMetadata = metadata;
		}
		
		
		public string							Name
		{
			get
			{
				return this.name;
			}
		}
		public System.Type						OwnerType
		{
			get
			{
				return this.ownerType;
			}
		}
		public System.Type						PropertyType
		{
			get
			{
				return this.propertyType;
			}
		}
		public PropertyMetadata					DefaultMetadata
		{
			get
			{
				return this.defaultMetadata;
			}
		}
		public bool								IsAttached
		{
			get
			{
				return this.isAttached;
			}
		}
		
		public override int GetHashCode()
		{
			return this.name.GetHashCode () ^ this.ownerType.GetHashCode ();
		}
		public override bool Equals(object obj)
		{
			return this.Equals (obj as Property);
		}

		#region IEquatable<Property> Members
		public bool Equals(Property other)
		{
			if (other != null)
			{
				return (this.ownerType == other.ownerType)
					&& (this.name == other.name)
					&& (this.isAttached == other.isAttached);
			}

			return false;
		}
		#endregion

		public static ReadOnlyArray<Property> GetAllAttachedProperties()
		{
			if (Property.attachedPropertiesArray == null)
			{
				lock (Property.exclusion)
				{
					if (Property.attachedPropertiesArray == null)
					{
						Property.attachedPropertiesArray = Property.attachedPropertiesList.ToArray ();
					}
				}
			}
			
			return new ReadOnlyArray<Property> (Property.attachedPropertiesArray);
		}
		
		public void OverrideMetadata(System.Type type, PropertyMetadata metadata)
		{
			if (this.overriddenMetadata == null)
			{
				lock (this)
				{
					if (this.overriddenMetadata == null)
					{
						this.overriddenMetadata = new Dictionary<System.Type, PropertyMetadata> ();
					}
				}
			}
			
			this.overriddenMetadata[type] = metadata;
		}
		
		public PropertyMetadata GetMetadata(Object o)
		{
			return this.GetMetadata (o.GetType ());
		}
		public PropertyMetadata GetMetadata(System.Type type)
		{
			if (this.overriddenMetadata != null)
			{
				while (type != null)
				{
					if (this.overriddenMetadata.ContainsKey (type))
					{
						return this.overriddenMetadata[type];
					}
					
					type = type.BaseType;
				}
			}
			
			return this.DefaultMetadata;
		}
		
		public static Property Register(string name, System.Type property_type, System.Type owner_type)
		{
			return Property.Register (name, property_type, owner_type, new PropertyMetadata ());
		}
		public static Property Register(string name, System.Type property_type, System.Type owner_type, PropertyMetadata metadata)
		{
			Property dp = new Property (name, property_type, owner_type, metadata);
			
			Object.Register (dp);
			
			return dp;
		}
		
		public static Property RegisterAttached(string name, System.Type property_type, System.Type owner_type)
		{
			return Property.RegisterAttached (name, property_type, owner_type, new PropertyMetadata ());
		}
		public static Property RegisterAttached(string name, System.Type property_type, System.Type owner_type, PropertyMetadata metadata)
		{
			Property dp = new Property (name, property_type, owner_type, metadata);
			dp.isAttached = true;
			
			Object.Register (dp);

			lock (Property.exclusion)
			{
				Property.attachedPropertiesList.Add (dp);
				Property.attachedPropertiesArray = null;
			}
			
			return dp;
		}
		
		public static Property RegisterReadOnly(string name, System.Type property_type, System.Type owner_type)
		{
			return Property.Register (name, property_type, owner_type, new PropertyMetadata ());
		}
		public static Property RegisterReadOnly(string name, System.Type property_type, System.Type owner_type, PropertyMetadata metadata)
		{
			Property dp = new Property (name, property_type, owner_type, metadata);
			
			Object.Register (dp);
			
			return dp;
		}
		
		
		private string							name;
		private System.Type						propertyType;
		private System.Type						ownerType;
		private PropertyMetadata				defaultMetadata;
		private bool							isAttached;
		
		Dictionary<System.Type, PropertyMetadata>	overriddenMetadata;
		
		static object							exclusion = new object ();
		static List<Property>					attachedPropertiesList = new List<Property> ();
		static Property[]						attachedPropertiesArray;
	}
}
