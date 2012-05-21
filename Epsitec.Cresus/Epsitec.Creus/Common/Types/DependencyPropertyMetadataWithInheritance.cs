//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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

		public DependencyPropertyMetadataWithInheritance(object defaultValue) : base (defaultValue)
		{
		}
		public DependencyPropertyMetadataWithInheritance(object defaultValue, PropertyInvalidatedCallback propertyInvalidatedCallback) : base (defaultValue, propertyInvalidatedCallback)
		{
		}

		public DependencyPropertyMetadataWithInheritance(GetValueOverrideCallback getValueOverrideCallback) : base (getValueOverrideCallback)
		{
		}
		public DependencyPropertyMetadataWithInheritance(GetValueOverrideCallback getValueOverrideCallback, SetValueOverrideCallback setValueOverrideCallback) : base (getValueOverrideCallback, setValueOverrideCallback)
		{
		}
		public DependencyPropertyMetadataWithInheritance(GetValueOverrideCallback getValueOverrideCallback, SetValueOverrideCallback setValueOverrideCallback, PropertyInvalidatedCallback propertyInvalidatedCallback) : base (getValueOverrideCallback, setValueOverrideCallback, propertyInvalidatedCallback)
		{
		}
		public DependencyPropertyMetadataWithInheritance(GetValueOverrideCallback getValueOverrideCallback, PropertyInvalidatedCallback propertyInvalidatedCallback) : base (getValueOverrideCallback, propertyInvalidatedCallback)
		{
		}
		
		public DependencyPropertyMetadataWithInheritance(object defaultValue, GetValueOverrideCallback getValueOverrideCallback, SetValueOverrideCallback setValueOverrideCallback) : base (defaultValue, getValueOverrideCallback, setValueOverrideCallback)
		{
		}
		public DependencyPropertyMetadataWithInheritance(object defaultValue, GetValueOverrideCallback getValueOverrideCallback, PropertyInvalidatedCallback propertyInvalidatedCallback) : base (defaultValue, getValueOverrideCallback, propertyInvalidatedCallback)
		{
		}
		public DependencyPropertyMetadataWithInheritance(object defaultValue, GetValueOverrideCallback getValueOverrideCallback) : base (defaultValue, getValueOverrideCallback)
		{
		}
		public DependencyPropertyMetadataWithInheritance(object defaultValue, SetValueOverrideCallback setValueOverrideCallback) : base (defaultValue, setValueOverrideCallback)
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
