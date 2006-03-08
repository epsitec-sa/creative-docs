//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// DependencyPropertyMetadata.
	/// </summary>
	public class DependencyPropertyMetadata
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


		public virtual object CreateDefaultValue()
		{
			object value     = this.DefaultValue;
			System.ICloneable cloneable = value as System.ICloneable;

			return (cloneable == null) ? value : cloneable.Clone ();
		}
		public virtual object FindInheritedValue(DependencyObject o, DependencyProperty property)
		{
			DependencyObject parent = DependencyObjectTree.FindParentDefiningProperty (o, property);
			
			if (parent == null)
			{
				return this.CreateDefaultValue ();
			}
			else
			{
				return parent.GetValue (property);
			}
		}

		internal void NotifyPropertyInvalidated(DependencyObject sender, object old_value, object new_value)
		{
			this.OnPropertyInvalidated (sender, old_value, new_value);
		}
		
		protected virtual void OnPropertyInvalidated(DependencyObject sender, object old_value, object new_value)
		{
			if (this.PropertyInvalidated != null)
			{
				this.PropertyInvalidated (sender, old_value, new_value);
			}
		}
		
		
		private object							defaultValue = UndefinedValue.Instance;
		private GetValueOverrideCallback		getValueOverride;
		private SetValueOverrideCallback		setValueOverride;
		private PropertyInvalidatedCallback		propertyInvalidated;
		private ValidateValueCallback			validateValueCallback;
		private CoerceValueCallback				coerceValueCallback;
	}
}
