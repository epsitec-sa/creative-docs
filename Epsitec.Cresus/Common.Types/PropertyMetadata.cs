//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// PropertyMetadata.
	/// </summary>
	public class PropertyMetadata
	{
		public PropertyMetadata()
		{
		}
		
		public PropertyMetadata(object default_value)
		{
			this.default_value = default_value;
		}
		
		public PropertyMetadata(object default_value, PropertyInvalidatedCallback property_invalidated_callback)
		{
			this.default_value = default_value;
			this.property_invalidated = property_invalidated_callback;
		}
		
		public PropertyMetadata(object default_value, GetValueOverrideCallback get_value_override_callback)
		{
			this.default_value = default_value;
			this.get_value_override = get_value_override_callback;
		}
		
		
		public object							DefaultValue
		{
			get
			{
				return this.default_value;
			}
		}
		
		public GetValueOverrideCallback			GetValueOverride
		{
			get
			{
				return this.get_value_override;
			}
			set
			{
				this.get_value_override = value;
			}
		}
		
		public SetValueOverrideCallback			SetValueOverride
		{
			get
			{
				return this.set_value_override;
			}
			set
			{
				this.set_value_override = value;
			}
		}
		
		public PropertyInvalidatedCallback		PropertyInvalidated
		{
			get
			{
				return this.property_invalidated;
			}
			set
			{
				this.property_invalidated = value;
			}
		}
		
		
		internal void NotifyPropertyInvalidated(Object sender, object old_value, object new_value)
		{
			this.OnPropertyInvalidated (sender, old_value, new_value);
		}
		
		
		protected virtual void OnPropertyInvalidated(Object sender, object old_value, object new_value)
		{
			if (this.PropertyInvalidated != null)
			{
				this.PropertyInvalidated (sender, old_value, new_value);
			}
		}
		
		
		private object							default_value;
		private GetValueOverrideCallback		get_value_override;
		private SetValueOverrideCallback		set_value_override;
		private PropertyInvalidatedCallback		property_invalidated;
	}
}
