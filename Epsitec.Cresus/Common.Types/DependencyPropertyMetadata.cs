//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
		
		public DependencyPropertyMetadata(object defaultValue)
		{
			this.defaultValue = defaultValue;
		}
		public DependencyPropertyMetadata(object defaultValue, PropertyInvalidatedCallback propertyInvalidatedCallback)
		{
			this.defaultValue = defaultValue;
			this.propertyInvalidated = propertyInvalidatedCallback;
		}

		public DependencyPropertyMetadata(PropertyInvalidatedCallback propertyInvalidatedCallback)
		{
			this.propertyInvalidated = propertyInvalidatedCallback;
		}
		
		public DependencyPropertyMetadata(GetValueOverrideCallback getValueOverrideCallback)
		{
			this.getValueOverride = getValueOverrideCallback;
		}
		public DependencyPropertyMetadata(GetValueOverrideCallback getValueOverrideCallback, SetValueOverrideCallback setValueOverrideCallback)
		{
			this.getValueOverride = getValueOverrideCallback;
			this.setValueOverride = setValueOverrideCallback;
		}
		public DependencyPropertyMetadata(GetValueOverrideCallback getValueOverrideCallback, SetValueOverrideCallback setValueOverrideCallback, PropertyInvalidatedCallback propertyInvalidatedCallback)
		{
			this.getValueOverride = getValueOverrideCallback;
			this.setValueOverride = setValueOverrideCallback;
			this.propertyInvalidated = propertyInvalidatedCallback;
		}
		public DependencyPropertyMetadata(GetValueOverrideCallback getValueOverrideCallback, PropertyInvalidatedCallback propertyInvalidatedCallback)
		{
			this.getValueOverride = getValueOverrideCallback;
			this.propertyInvalidated = propertyInvalidatedCallback;
		}
		
		public DependencyPropertyMetadata(object defaultValue, GetValueOverrideCallback getValueOverrideCallback, SetValueOverrideCallback setValueOverrideCallback)
		{
			this.defaultValue     = defaultValue;
			this.getValueOverride = getValueOverrideCallback;
			this.setValueOverride = setValueOverrideCallback;
		}
		public DependencyPropertyMetadata(object defaultValue, GetValueOverrideCallback getValueOverrideCallback, PropertyInvalidatedCallback propertyInvalidatedCallback)
		{
			this.defaultValue        = defaultValue;
			this.getValueOverride    = getValueOverrideCallback;
			this.propertyInvalidated = propertyInvalidatedCallback;
		}
		public DependencyPropertyMetadata(object defaultValue, GetValueOverrideCallback getValueOverrideCallback)
		{
			this.defaultValue     = defaultValue;
			this.getValueOverride = getValueOverrideCallback;
		}
		public DependencyPropertyMetadata(object defaultValue, SetValueOverrideCallback setValueOverrideCallback)
		{
			this.defaultValue     = defaultValue;
			this.setValueOverride = setValueOverrideCallback;
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
				return this.filter != null;
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

		public void DefineFilter(System.Predicate<object> filter)
		{
			this.filter = filter;
		}
		
		public virtual bool FilterSerializableItem(object item)
		{
			if (this.filter == null)
			{
				return true;
			}
			else
			{
				return this.filter (item);
			}
		}
		public virtual IEnumerable<DependencyObject> FilterSerializableCollection(IEnumerable<DependencyObject> collection)
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

		internal bool NotifyPropertyInvalidated(DependencyObject sender, object oldValue, object newValue)
		{
			if (this.PropertyNotifiesChanges)
			{
				this.OnPropertyInvalidated (sender, oldValue, newValue);
				
				return true;
			}
			else
			{
				return false;
			}
		}
		
		protected virtual void OnPropertyInvalidated(DependencyObject sender, object oldValue, object newValue)
		{
			if (this.PropertyInvalidated != null)
			{
				this.PropertyInvalidated (sender, oldValue, newValue);
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
		private System.Predicate<object>		filter;
	}
}
