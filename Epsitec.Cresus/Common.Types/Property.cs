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
			this.globalIndex = System.Threading.Interlocked.Increment (ref Property.globalPropertyCount);
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
		public int								GlobalIndex
		{
			get
			{
				return this.globalIndex;
			}
		}
		
		public override int GetHashCode()
		{
			return this.globalIndex;
		}
		public override bool Equals(object obj)
		{
			return this.Equals (obj as Property);
		}

		public bool IsValidType(object value)
		{
			if (value == null)
			{
				return false;
			}
			else
			{
				return this.PropertyType.IsAssignableFrom (value.GetType ());
			}
		}
		public bool IsValidValue(object value)
		{
			if (this.IsValidType (value))
			{
				PropertyMetadata metadata = this.DefaultMetadata;

				if (metadata.ValidateValue != null)
				{
					return metadata.ValidateValue (value);
				}
				else
				{
					return true;
				}
			}
			else
			{
				return false;
			}
		}

		public Property AddOwner(System.Type ownerType)
		{
			if (this.IsAttached)
			{
				throw new System.InvalidOperationException (string.Format ("Attached property {0} does not accept owner {1}", this.Name, ownerType));
			}

			if (this.additionalOwnerTypes == null)
			{
				lock (this)
				{
					if (this.additionalOwnerTypes == null)
					{
						this.additionalOwnerTypes = new List<System.Type> ();
					}
				}
			}

			if (this.additionalOwnerTypes.Contains (ownerType))
			{
				throw new System.ArgumentException (string.Format ("Property named {0} already has owner {1}", this.Name, ownerType));
			}
			
			this.additionalOwnerTypes.Add (ownerType);
			
			Object.Register (this, ownerType);
			
			return this;
		}
		public Property AddOwner(System.Type ownerType, PropertyMetadata metadata)
		{
			Property property = this.AddOwner (ownerType);
			
			property.OverrideMetadata (ownerType, metadata);
			
			return property;
		}
		
		public bool IsOwnedBy(System.Type ownerType)
		{
			if (this.ownerType == ownerType)
			{
				return true;
			}
			else if (this.additionalOwnerTypes != null)
			{
				return this.additionalOwnerTypes.Contains (ownerType);
			}
			else
			{
				return false;
			}
		}
		
		#region IEquatable<Property> Members
		public bool Equals(Property other)
		{
			if (other != null)
			{
				return this.globalIndex == other.globalIndex;
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
			
			Object.Register (dp, dp.OwnerType);
			
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
			
			Object.Register (dp, dp.OwnerType);

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
			
			Object.Register (dp, dp.OwnerType);
			
			return dp;
		}
		
		
		private string							name;
		private System.Type						propertyType;
		private System.Type						ownerType;
		private List<System.Type>				additionalOwnerTypes;
		private PropertyMetadata				defaultMetadata;
		private bool							isAttached;
		private int								globalIndex;
		
		Dictionary<System.Type, PropertyMetadata>	overriddenMetadata;
		
		static object							exclusion = new object ();
		static List<Property>					attachedPropertiesList = new List<Property> ();
		static Property[]						attachedPropertiesArray;
		static int								globalPropertyCount;
	}
}
