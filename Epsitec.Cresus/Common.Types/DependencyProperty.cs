//	Copyright © 2005-2011, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>DependencyProperty</c> class stores runtime information about a property
	/// defined for a <see cref="DependencyObject"/>. The concept is very similar to what
	/// Microsoft implemented for WPF.
	/// </summary>
	public sealed class DependencyProperty : System.IEquatable<DependencyProperty>, System.IComparable<DependencyProperty>, IDataConstraint, IName
	{
		private DependencyProperty(string name, System.Type propertyType, System.Type ownerType, DependencyPropertyMetadata metadata, DpType dpType = DpType.Standard)
		{
			if (name == null)
			{
				throw new System.ArgumentNullException ("Property needs a name");
			}

			if (DependencyProperty.validNameRegex.IsMatch (name) == false)
			{
				throw new System.ArgumentException (string.Format ("'{0}' is not a valid property name", name));
			}
			
			this.name = name;
			this.propertyType = propertyType;
			this.ownerType = ownerType;
			this.defaultMetadata = metadata;
			this.globalIndex = System.Threading.Interlocked.Increment (ref DependencyProperty.globalPropertyCount);
			this.isPropertyDerivedFromDependencyObject = typeof (DependencyObject).IsAssignableFrom (this.propertyType);
			this.isAttached = dpType == DpType.Attached;
			this.isReadOnly = dpType == DpType.ReadOnly;

			this.isPropertyAnICollectionOfDependencyObject = TypeRosetta.DoesTypeImplementInterface (this.propertyType, typeof (ICollection<DependencyObject>));
			this.isPropertyAnICollectionOfString = TypeRosetta.DoesTypeImplementInterface (this.propertyType, typeof (ICollection<string>));
			this.isPropertyAnICollectionOfAny = TypeRosetta.DoesTypeImplementGenericInterface (this.propertyType, typeof (ICollection<>));

			if ((this.isPropertyAnICollectionOfDependencyObject == false) &&
				(this.isPropertyAnICollectionOfString == false) &&
				(this.isPropertyAnICollectionOfAny == false))
			{
				if (TypeRosetta.DoesTypeImplementInterface (this.propertyType, typeof (System.Collections.ICollection)))
				{
					throw new System.ArgumentException (string.Format ("Property {0} uses unsupported collection type {1}", name, this.propertyType.Name));
				}
			}

			if ((this.defaultMetadata.InheritsValue) &&
				(this.propertyType == typeof (bool)))
			{
				//	This is a special property which can fit into a bitset-based
				//	inherited property cache.

				lock (DependencyProperty.exclusion)
				{
					if (DependencyProperty.bitsetBasedCachedInheritedPropertyCount < DependencyProperty.bitsetBasedCachedInheritedProperties.Length)
					{
						int index = DependencyProperty.bitsetBasedCachedInheritedPropertyCount++;
						DependencyProperty.bitsetBasedCachedInheritedProperties[index] = this;
						this.inheritedPropertyCacheMask = (1 << index);
					}
				}
			}

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
		
		public bool								IsPropertyTypeAnICollectionOfDependencyObject
		{
			get
			{
				return this.isPropertyAnICollectionOfDependencyObject;
			}
		}
		
		public bool								IsPropertyTypeAnICollectionOfString
		{
			get
			{
				return this.isPropertyAnICollectionOfString;
			}
		}

		public bool								IsPropertyTypeAnICollectionOfAny
		{
			get
			{
				return this.isPropertyAnICollectionOfAny;
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
		
		public bool								HasTypeConverter
		{
			get
			{
				this.SetupTypeConverterIfNeeded ();

				return this.hasTypeConverter;
			}
		}

		public Support.Druid					CaptionId
		{
			get
			{
				return this.captionId;
			}
		}
		
		internal int							InheritedPropertyCacheMask
		{
			get
			{
				return this.inheritedPropertyCacheMask;
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
				return ! this.PropertyType.IsValueType;
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

		#region IComparable<DependencyProperty> Members

		public int CompareTo(DependencyProperty other)
		{
			if (other == null)
			{
				return 1;
			}
			
			if (other.globalIndex == this.globalIndex)
			{
				return 0;
			}

			int compare = this.name.CompareTo (other.name);

			if (compare == 0)
			{
				compare = this.propertyType.Name.CompareTo (other.propertyType.Name);

				if (compare == 0)
				{
					compare = this.ownerType.Name.CompareTo (other.ownerType.Name);
				}
			}

			return compare;
		}

		#endregion

		#region IDataConstraint Members

		bool IDataConstraint.IsValidValue(object value)
		{
			return this.IsValidValue (value);
		}

		#endregion

		#region IName Members

		string IName.Name
		{
			get
			{
				return this.Name;
			}
		}

		#endregion
		
		public static Collections.ReadOnlyList<DependencyProperty> GetAllAttachedProperties()
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

			return new Collections.ReadOnlyList<DependencyProperty> (DependencyProperty.attachedPropertiesArray);
		}
		
		internal static DependencyProperty GetInheritedPropertyFromCacheMask(int mask)
		{
			if (mask == 0)
			{
				return null;
			}
			if (mask < 0)
			{
				return null;
			}
			
			int shift = 1;
			
			for (int i = 0; i < DependencyProperty.bitsetBasedCachedInheritedPropertyCount; i++)
			{
				if (shift == mask)
				{
					return DependencyProperty.bitsetBasedCachedInheritedProperties[i];
				}
				
				shift = shift << 1;
			}
			
			return null;
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

		/// <summary>
		/// Defines the caption id associated with this dependency property.
		/// </summary>
		/// <param name="captionId">The caption id.</param>
		public void DefineCaptionId(Support.Druid captionId)
		{
			if ((this.captionId == captionId) ||
				(this.captionId.IsEmpty))
			{
				this.captionId = captionId;
			}
			else
			{
				throw new System.InvalidOperationException ("CaptionId may not be redefined");
			}
		}
		
		public DependencyPropertyMetadata GetMetadata(DependencyObject o)
		{
			return this.GetMetadata (o.GetType ());
		}
		public DependencyPropertyMetadata GetMetadata(System.Type type)
		{
			if (this.overriddenMetadata != null)
			{
				DependencyPropertyMetadata metadata;

				while (type != typeof (DependencyObject))
				{
					if (this.overriddenMetadata.TryGetValue (type, out metadata))
					{
						return metadata;
					}
					
					type = type.BaseType;
				}
			}
			
			return this.DefaultMetadata;
		}


		/// <summary>
		/// Converts the object value to a <c>string</c>.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="context">The conversion context.</param>
		/// <returns>A <c>string</c> which represents the object value.</returns>
		public string ConvertToString(object value, IContextResolver context)
		{
			if (value == null)
			{
				return null;
			}

			this.SetupTypeConverterIfNeeded ();

			System.Diagnostics.Debug.Assert (this.typeConverter != null);
			System.Diagnostics.Debug.Assert (TypeRosetta.IsValidValue (value, this.propertyType));
			
			return this.typeConverter.ConvertToString (value, context);
		}

		/// <summary>
		/// Converts the <c>string</c> back to an object value.
		/// </summary>
		/// <param name="value">The <c>string</c> value.</param>
		/// <param name="context">The conversion context.</param>
		/// <returns>An object value compatible with the property type.</returns>
		public object ConvertFromString(string value, IContextResolver context)
		{
			if (value == null)
			{
				return null;
			}

			this.SetupTypeConverterIfNeeded ();

			System.Diagnostics.Debug.Assert (this.typeConverter != null);
			
			return this.typeConverter.ConvertFromString (value, context);
		}

		/// <summary>
		/// Defines a specific serialization converter for this property.
		/// </summary>
		/// <param name="serializationConverter">The serialization converter.</param>
		public void DefineSerializationConverter(ISerializationConverter serializationConverter)
		{
			System.Diagnostics.Debug.Assert (serializationConverter != null);

			this.typeConverter = serializationConverter;
			this.typeConverterOk = true;
			this.hasTypeConverter = true;
		}


		private void SetupTypeConverterIfNeeded()
		{
			if (this.typeConverterOk == false)
			{
				lock (this)
				{
					if (this.typeConverterOk == false)
					{
						ISerializationConverter converter = null;
						
						if (Serialization.DependencyClassManager.IsReady)
						{
							converter = Serialization.DependencyClassManager.Current.FindSerializationConverter (this.propertyType);

							if (this.typeConverter == null)
							{
								this.typeConverter = converter;
							}
						}
						
						if (this.typeConverter == null)
						{
							this.typeConverter = InvariantConverter.GetSerializationConverter (this.propertyType);
						}

						this.hasTypeConverter = (converter != null);
						this.typeConverterOk  = true;
					}
				}
			}
		}


		public static DependencyProperty Register(string name, System.Type propertyType, System.Type ownerType)
		{
			return DependencyProperty.Register (name, propertyType, ownerType, new DependencyPropertyMetadata ());
		}
		
		public static DependencyProperty Register(string name, System.Type propertyType, System.Type ownerType, DependencyPropertyMetadata metadata)
		{
			DependencyProperty dp = new DependencyProperty (name, propertyType, ownerType, metadata);
			
			DependencyObject.Register (dp, dp.OwnerType);
			
			return dp;
		}

		
		public static DependencyProperty RegisterAttached(string name, System.Type propertyType, System.Type ownerType)
		{
			return DependencyProperty.RegisterAttached (name, propertyType, ownerType, new DependencyPropertyMetadata ());
		}
		
		public static DependencyProperty RegisterAttached(string name, System.Type propertyType, System.Type ownerType, DependencyPropertyMetadata metadata)
		{
			DependencyProperty dp = new DependencyProperty (name, propertyType, ownerType, metadata, DpType.Attached);
			
			DependencyObject.Register (dp, dp.OwnerType);

			lock (DependencyProperty.exclusion)
			{
				DependencyProperty.attachedPropertiesList.Add (dp);
				DependencyProperty.attachedPropertiesArray = null;
			}
			
			return dp;
		}

		
		public static DependencyProperty RegisterReadOnly(string name, System.Type propertyType, System.Type ownerType)
		{
			return DependencyProperty.RegisterReadOnly (name, propertyType, ownerType, new DependencyPropertyMetadata ());
		}
		
		public static DependencyProperty RegisterReadOnly(string name, System.Type propertyType, System.Type ownerType, DependencyPropertyMetadata metadata)
		{
			DependencyProperty dp = new DependencyProperty (name, propertyType, ownerType, metadata, DpType.ReadOnly);

			DependencyObject.Register (dp, dp.OwnerType);
			
			return dp;
		}


		private enum DpType
		{
			Standard,
			ReadOnly,
			Attached,
		}

		static DependencyProperty()
		{
			DependencyProperty.validNameRegex = new System.Text.RegularExpressions.Regex ("^[a-zA-Z][_a-zA-Z0-9]*$");
		}

		private readonly string						name;
		private readonly System.Type				propertyType;
		private readonly System.Type				ownerType;
		private readonly DependencyPropertyMetadata	defaultMetadata;
		private readonly bool						isAttached;
		private readonly bool						isPropertyDerivedFromDependencyObject;
		private readonly bool						isPropertyAnICollectionOfDependencyObject;
		private readonly bool						isPropertyAnICollectionOfString;
		private readonly bool						isPropertyAnICollectionOfAny;
		private readonly bool						isReadOnly;
		private readonly int						globalIndex;
		private readonly int						inheritedPropertyCacheMask;

		private List<System.Type>					additionalOwnerTypes;
		private List<System.Type>					derivedTypes;
		
		private bool								hasTypeConverter;
		private bool								typeConverterOk;
		private ISerializationConverter				typeConverter;
		private Support.Druid						captionId;
		
		private Dictionary<System.Type, DependencyPropertyMetadata>	overriddenMetadata;

		private static readonly object						exclusion = new object ();
		private static readonly List<DependencyProperty>	attachedPropertiesList = new List<DependencyProperty> ();
		private static DependencyProperty[]					attachedPropertiesArray;
		private static int									globalPropertyCount;
		private static readonly DependencyProperty[]		bitsetBasedCachedInheritedProperties = new DependencyProperty[InheritedPropertyCache.MaskBits];
		private static int									bitsetBasedCachedInheritedPropertyCount;
		private static System.Text.RegularExpressions.Regex validNameRegex;
	}
}