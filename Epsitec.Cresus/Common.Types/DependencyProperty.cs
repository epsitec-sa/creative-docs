//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// DependencyProperty.
	/// </summary>
	public sealed class DependencyProperty : System.IEquatable<DependencyProperty>
	{
		private DependencyProperty(string name, System.Type property_type, System.Type owner_type, DependencyPropertyMetadata metadata)
		{
			this.name = name;
			this.propertyType = property_type;
			this.ownerType = owner_type;
			this.defaultMetadata = metadata;
			this.globalIndex = System.Threading.Interlocked.Increment (ref DependencyProperty.globalPropertyCount);
			this.isPropertyDerivedFromDependencyObject = typeof (DependencyObject).IsAssignableFrom (this.propertyType);
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
		public DependencyPropertyMetadata		DefaultMetadata
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
		public bool								IsPropertyTypeDerivedFromDependencyObject
		{
			get
			{
				return this.isPropertyDerivedFromDependencyObject;
			}
		}
		public bool								IsReadOnly
		{
			get
			{
				return this.isReadOnly;
			}
		}
		public bool								IsReadWrite
		{
			get
			{
				return this.isReadOnly == false;
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
			return this.Equals (obj as DependencyProperty);
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
				DependencyPropertyMetadata metadata = this.DefaultMetadata;

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

		public DependencyProperty AddOwner(System.Type ownerType)
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
				throw new System.ArgumentException (string.Format ("DependencyProperty named {0} already has owner {1}", this.Name, ownerType));
			}
			
			this.additionalOwnerTypes.Add (ownerType);
			
			DependencyObject.Register (this, ownerType);
			
			return this;
		}
		public DependencyProperty AddOwner(System.Type ownerType, DependencyPropertyMetadata metadata)
		{
			DependencyProperty property = this.AddOwner (ownerType);
			
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
		
		#region IEquatable<DependencyProperty> Members
		public bool Equals(DependencyProperty other)
		{
			if (other != null)
			{
				return this.globalIndex == other.globalIndex;
			}

			return false;
		}
		#endregion

		public static ReadOnlyArray<DependencyProperty> GetAllAttachedProperties()
		{
			if (DependencyProperty.attachedPropertiesArray == null)
			{
				lock (DependencyProperty.exclusion)
				{
					if (DependencyProperty.attachedPropertiesArray == null)
					{
						DependencyProperty.attachedPropertiesArray = DependencyProperty.attachedPropertiesList.ToArray ();
					}
				}
			}
			
			return new ReadOnlyArray<DependencyProperty> (DependencyProperty.attachedPropertiesArray);
		}
		
		public void OverrideMetadata(System.Type type, DependencyPropertyMetadata metadata)
		{
			DependencyObjectType.FromSystemType (type);
			
			if (this.overriddenMetadata == null)
			{
				lock (this)
				{
					if (this.overriddenMetadata == null)
					{
						this.overriddenMetadata = new Dictionary<System.Type, DependencyPropertyMetadata> ();
					}
				}
			}
			
			this.overriddenMetadata[type] = metadata;
		}
		
		public DependencyPropertyMetadata GetMetadata(DependencyObject o)
		{
			return this.GetMetadata (o.GetType ());
		}
		public DependencyPropertyMetadata GetMetadata(System.Type type)
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
		
		public static DependencyProperty Register(string name, System.Type property_type, System.Type owner_type)
		{
			return DependencyProperty.Register (name, property_type, owner_type, new DependencyPropertyMetadata ());
		}
		public static DependencyProperty Register(string name, System.Type property_type, System.Type owner_type, DependencyPropertyMetadata metadata)
		{
			DependencyProperty dp = new DependencyProperty (name, property_type, owner_type, metadata);
			
			DependencyObject.Register (dp, dp.OwnerType);
			
			return dp;
		}
		
		public static DependencyProperty RegisterAttached(string name, System.Type property_type, System.Type owner_type)
		{
			return DependencyProperty.RegisterAttached (name, property_type, owner_type, new DependencyPropertyMetadata ());
		}
		public static DependencyProperty RegisterAttached(string name, System.Type property_type, System.Type owner_type, DependencyPropertyMetadata metadata)
		{
			DependencyProperty dp = new DependencyProperty (name, property_type, owner_type, metadata);
			dp.isAttached = true;
			
			DependencyObject.Register (dp, dp.OwnerType);

			lock (DependencyProperty.exclusion)
			{
				DependencyProperty.attachedPropertiesList.Add (dp);
				DependencyProperty.attachedPropertiesArray = null;
			}
			
			return dp;
		}
		
		public static DependencyProperty RegisterReadOnly(string name, System.Type property_type, System.Type owner_type)
		{
			return DependencyProperty.RegisterReadOnly (name, property_type, owner_type, new DependencyPropertyMetadata ());
		}
		public static DependencyProperty RegisterReadOnly(string name, System.Type property_type, System.Type owner_type, DependencyPropertyMetadata metadata)
		{
			DependencyProperty dp = new DependencyProperty (name, property_type, owner_type, metadata);

			dp.isReadOnly = true;
			
			DependencyObject.Register (dp, dp.OwnerType);
			
			return dp;
		}
		
		
		private string							name;
		private System.Type						propertyType;
		private System.Type						ownerType;
		private List<System.Type>				additionalOwnerTypes;
		private List<System.Type>				derivedTypes;
		private DependencyPropertyMetadata		defaultMetadata;
		private bool							isAttached;
		private bool							isPropertyDerivedFromDependencyObject;
		private bool							isReadOnly;
		private int								globalIndex;
		
		Dictionary<System.Type, DependencyPropertyMetadata>	overriddenMetadata;
		
		static object							exclusion = new object ();
		static List<DependencyProperty>			attachedPropertiesList = new List<DependencyProperty> ();
		static DependencyProperty[]				attachedPropertiesArray;
		static int								globalPropertyCount;
	}
}
