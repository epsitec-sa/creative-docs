//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// DependencyPropertyMetadataWithInheritance.
	/// </summary>
	public class DependencyPropertyMetadataWithInheritance : DependencyPropertyMetadata
	{
		public DependencyPropertyMetadataWithInheritance() : base ()
		{
		}

		public DependencyPropertyMetadataWithInheritance(object default_value) : base (default_value)
		{
		}
		public DependencyPropertyMetadataWithInheritance(object default_value, PropertyInvalidatedCallback property_invalidated_callback) : base (default_value, property_invalidated_callback)
		{
		}

		public DependencyPropertyMetadataWithInheritance(GetValueOverrideCallback get_value_override_callback) : base (get_value_override_callback)
		{
		}
		public DependencyPropertyMetadataWithInheritance(GetValueOverrideCallback get_value_override_callback, SetValueOverrideCallback set_value_override_callback) : base (get_value_override_callback, set_value_override_callback)
		{
		}
		public DependencyPropertyMetadataWithInheritance(GetValueOverrideCallback get_value_override_callback, SetValueOverrideCallback set_value_override_callback, PropertyInvalidatedCallback property_invalidated_callback) : base (get_value_override_callback, set_value_override_callback, property_invalidated_callback)
		{
		}
		public DependencyPropertyMetadataWithInheritance(GetValueOverrideCallback get_value_override_callback, PropertyInvalidatedCallback property_invalidated_callback) : base (get_value_override_callback, property_invalidated_callback)
		{
		}
		
		public DependencyPropertyMetadataWithInheritance(object default_value, GetValueOverrideCallback get_value_override_callback, SetValueOverrideCallback set_value_override_callback) : base (default_value, get_value_override_callback, set_value_override_callback)
		{
		}
		public DependencyPropertyMetadataWithInheritance(object default_value, GetValueOverrideCallback get_value_override_callback, PropertyInvalidatedCallback property_invalidated_callback) : base (default_value, get_value_override_callback, property_invalidated_callback)
		{
		}
		public DependencyPropertyMetadataWithInheritance(object default_value, GetValueOverrideCallback get_value_override_callback) : base (default_value, get_value_override_callback)
		{
		}
		public DependencyPropertyMetadataWithInheritance(object default_value, SetValueOverrideCallback set_value_override_callback) : base (default_value, set_value_override_callback)
		{
		}
		
		public override bool					InheritsValue
		{
			get
			{
				return true;
			}
		}

		protected override DependencyPropertyMetadata CloneNewObject()
		{
			return new DependencyPropertyMetadataWithInheritance ();
		}
	}
}
