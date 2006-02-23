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

		internal void AddDerivedType(System.Type derivedType)
		{
			if (this.derivedTypes == null)
			{
				lock (this)
				{
					if (this.derivedTypes == null)
					{
						this.derivedTypes = new List<System.Type> ();
					}
				}
			}
			
			this.derivedTypes.Add (derivedType);

			//	If the base type overrides the metadata for this property, then
			//	we have to inherit the same metadata :
			
			System.Type baseType = derivedType.BaseType;

			if (this.overriddenMetadata != null)
			{
				if ((this.overriddenMetadata.ContainsKey (baseType)) &&
					(this.overriddenMetadata.ContainsKey (derivedType) == false))
				{
					this.overriddenMetadata[derivedType] = this.overriddenMetadata[baseType];
				}
			}
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
		public bool IsReferencedBy(System.Type referrerType)
		{
			if (this.IsOwnedBy (referrerType))
			{
				return true;
			}
			else if (this.derivedTypes != null)
			{
				foreach (System.Type type in this.derivedTypes)
				{
					if (type == referrerType)
					{
						return true;
					}
				}
			}
			
			return false;
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
			ObjectType.FromSystemType (type);
			
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
				if (this.overriddenMetadata.ContainsKey (type))
				{
					return this.overriddenMetadata[type];
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
		private List<System.Type>				derivedTypes;
		private PropertyMetadata				defaultMetadata;
		private bool							isAttached;
		private int								globalIndex;
		private int								lastPropertyCount;
		
		Dictionary<System.Type, PropertyMetadata>	overriddenMetadata;
		
		static object							exclusion = new object ();
		static List<Property>					attachedPropertiesList = new List<Property> ();
		static Property[]						attachedPropertiesArray;
		static int								globalPropertyCount;
	}
}
