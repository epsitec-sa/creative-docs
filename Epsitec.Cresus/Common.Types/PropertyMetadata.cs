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
				return this.get_value_override_callback;
			}
			set
			{
				this.get_value_override_callback = value;
			}
		}
		
		public SetValueOverrideCallback			SetValueOverride
		{
			get
			{
				return this.set_value_override_callback;
			}
			set
			{
				this.set_value_override_callback = value;
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
		
		
		private object							default_value;
		private GetValueOverrideCallback		get_value_override_callback;
		private SetValueOverrideCallback		set_value_override_callback;
		private PropertyInvalidatedCallback		property_invalidated;
	}
}
