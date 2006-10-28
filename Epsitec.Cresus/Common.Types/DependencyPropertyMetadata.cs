//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// DependencyPropertyMetadata.
	/// </summary>
	public class DependencyPropertyMetadata : ICaption
	{
		public DependencyPropertyMetadata()
		{
		}
		
		public DependencyPropertyMetadata(object default_value)
		{
			this.defaultValue = default_value;
		}
		public DependencyPropertyMetadata(object default_value, PropertyInvalidatedCallback property_invalidated_callback)
		{
			this.defaultValue = default_value;
			this.propertyInvalidated = property_invalidated_callback;
		}

		public DependencyPropertyMetadata(PropertyInvalidatedCallback property_invalidated_callback)
		{
			this.propertyInvalidated = property_invalidated_callback;
		}
		
		public DependencyPropertyMetadata(GetValueOverrideCallback get_value_override_callback)
		{
			this.getValueOverride = get_value_override_callback;
		}
		public DependencyPropertyMetadata(GetValueOverrideCallback get_value_override_callback, SetValueOverrideCallback set_value_override_callback)
		{
			this.getValueOverride = get_value_override_callback;
			this.setValueOverride = set_value_override_callback;
		}
		public DependencyPropertyMetadata(GetValueOverrideCallback get_value_override_callback, SetValueOverrideCallback set_value_override_callback, PropertyInvalidatedCallback property_invalidated_callback)
		{
			this.getValueOverride = get_value_override_callback;
			this.setValueOverride = set_value_override_callback;
			this.propertyInvalidated = property_invalidated_callback;
		}
		public DependencyPropertyMetadata(GetValueOverrideCallback get_value_override_callback, PropertyInvalidatedCallback property_invalidated_callback)
		{
			this.getValueOverride = get_value_override_callback;
			this.propertyInvalidated = property_invalidated_callback;
		}
		
		public DependencyPropertyMetadata(object default_value, GetValueOverrideCallback get_value_override_callback, SetValueOverrideCallback set_value_override_callback)
		{
			this.defaultValue     = default_value;
			this.getValueOverride = get_value_override_callback;
			this.setValueOverride = set_value_override_callback;
		}
		public DependencyPropertyMetadata(object default_value, GetValueOverrideCallback get_value_override_callback, PropertyInvalidatedCallback property_invalidated_callback)
		{
			this.defaultValue        = default_value;
			this.getValueOverride    = get_value_override_callback;
			this.propertyInvalidated = property_invalidated_callback;
		}
		public DependencyPropertyMetadata(object default_value, GetValueOverrideCallback get_value_override_callback)
		{
			this.defaultValue     = default_value;
			this.getValueOverride = get_value_override_callback;
		}
		public DependencyPropertyMetadata(object default_value, SetValueOverrideCallback set_value_override_callback)
		{
			this.defaultValue     = default_value;
			this.setValueOverride = set_value_override_callback;
		}
		
		
		public object							DefaultValue
		{
			get
			{
				return this.defaultValue;
			}
		}
		public Support.Druid					CaptionId
		{
			get
			{
				return this.captionId;
			}
		}
		public INamedType						NamedType
		{
			get
			{
				return this.namedType;
			}
		}
		
		public GetValueOverrideCallback			GetValueOverride
		{
			get
			{
				return this.getValueOverride;
			}
			set
			{
				this.getValueOverride = value;
			}
		}
		public SetValueOverrideCallback			SetValueOverride
		{
			get
			{
				return this.setValueOverride;
			}
			set
			{
				this.setValueOverride = value;
			}
		}
		public ValidateValueCallback			ValidateValue
		{
			get
			{
				return this.validateValueCallback;
			}
			set
			{
				this.validateValueCallback = value;
			}
		}
		public CoerceValueCallback				CoerceValue
		{
			get
			{
				return this.coerceValueCallback;
			}
			set
			{
				this.coerceValueCallback = value;
			}
		}
		
		public PropertyInvalidatedCallback		PropertyInvalidated
		{
			get
			{
				return this.propertyInvalidated;
			}
			set
			{
				this.propertyInvalidated = value;
			}
		}
		public virtual bool						InheritsValue
		{
			get
			{
				return false;
			}
		}

		public virtual bool						HasSerializationFilter
		{
			get
			{
				return false;
			}
		}
		public bool								CanSerializeReadOnly
		{
			get
			{
				return this.canSerializeReadOnly;
			}
			set
			{
				this.canSerializeReadOnly = value;
			}
		}
		public bool								CanSerializeReadWrite
		{
			get
			{
				return this.canSerializeReadWrite;
			}
			set
			{
				this.canSerializeReadWrite = value;
			}
		}
		public virtual bool						PropertyNotifiesChanges
		{
			get
			{
				return true;
			}
		}


		public DependencyPropertyMetadata MakeReadOnlySerializable()
		{
			this.canSerializeReadOnly = true;
			return this;
		}
		public DependencyPropertyMetadata MakeNotSerializable()
		{
			this.canSerializeReadOnly = false;
			this.canSerializeReadWrite = false;
			return this;
		}
		public DependencyPropertyMetadata DefineCaptionId(Support.Druid value)
		{
			this.captionId = value;
			return this;
		}
		public DependencyPropertyMetadata DefineNamedType(INamedType type)
		{
			this.namedType = type;
			return this;
		}

		public void DefineDefaultValue(object value)
		{
			this.defaultValue = value;
		}
		
		public virtual bool FilterSerializableItem(DependencyObject item)
		{
			return true;
		}
		public virtual IEnumerable<DependencyObject> FilterSerializableCollection(IEnumerable<DependencyObject> collection, DependencyProperty property)
		{
			//	Skip items in collection which may not be serialized.
			
			if (this.HasSerializationFilter)
			{
				return this.ReallyFilterSerializableCollection (collection);
			}
			else
			{
				return collection;
			}
		}
		
		public virtual object CreateDefaultValue()
		{
			object value     = this.DefaultValue;
			System.ICloneable cloneable = value as System.ICloneable;

			return (cloneable == null) ? value : cloneable.Clone ();
		}

		public DependencyPropertyMetadata Clone()
		{
			return this.CloneCopyToNewObject (this.CloneNewObject ());
		}

		protected virtual DependencyPropertyMetadata CloneCopyToNewObject(DependencyPropertyMetadata copy)
		{
			copy.defaultValue = this.defaultValue;
			copy.getValueOverride = this.getValueOverride;
			copy.setValueOverride = this.setValueOverride;
			copy.propertyInvalidated = this.propertyInvalidated;
			copy.validateValueCallback = this.validateValueCallback;
			copy.coerceValueCallback = this.coerceValueCallback;
			copy.canSerializeReadOnly = this.canSerializeReadOnly;
			copy.canSerializeReadWrite = this.canSerializeReadWrite;
			copy.captionId = this.captionId;
			copy.namedType = this.namedType;

			return copy;
		}

		protected virtual DependencyPropertyMetadata CloneNewObject()
		{
			return new DependencyPropertyMetadata ();
		}

		#region ICaption Members

		Support.Druid ICaption.CaptionId
		{
			get
			{
				return this.CaptionId;
			}
		}

		#endregion

		internal bool NotifyPropertyInvalidated(DependencyObject sender, object old_value, object new_value)
		{
			if (this.PropertyNotifiesChanges)
			{
				this.OnPropertyInvalidated (sender, old_value, new_value);
				
				return true;
			}
			else
			{
				return false;
			}
		}
		
		protected virtual void OnPropertyInvalidated(DependencyObject sender, object old_value, object new_value)
		{
			if (this.PropertyInvalidated != null)
			{
				this.PropertyInvalidated (sender, old_value, new_value);
			}
		}

		protected IEnumerable<DependencyObject> ReallyFilterSerializableCollection(IEnumerable<DependencyObject> collection)
		{
			foreach (Types.DependencyObject item in collection)
			{
				if (this.FilterSerializableItem (item))
				{
					yield return item;
				}
			}
		}
		
		
		private object							defaultValue;
		private GetValueOverrideCallback		getValueOverride;
		private SetValueOverrideCallback		setValueOverride;
		private PropertyInvalidatedCallback		propertyInvalidated;
		private ValidateValueCallback			validateValueCallback;
		private CoerceValueCallback				coerceValueCallback;
		private bool							canSerializeReadOnly;
		private bool							canSerializeReadWrite = true;
		private Support.Druid					captionId;
		private INamedType						namedType;
	}
}
