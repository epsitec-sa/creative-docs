//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// Property.
	/// </summary>
	public sealed class Property
	{
		private Property(string name, System.Type property_type, System.Type owner_type, PropertyMetadata metadata)
		{
			this.name = name;
			this.property_type = property_type;
			this.owner_type = owner_type;
			this.default_metadata = metadata;
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
				return this.owner_type;
			}
		}
		
		public System.Type						PropertyType
		{
			get
			{
				return this.property_type;
			}
		}
		
		public PropertyMetadata					DefaultMetadata
		{
			get
			{
				return this.default_metadata;
			}
		}
		
		
		public override int GetHashCode()
		{
			return this.name.GetHashCode () ^ this.owner_type.GetHashCode ();
		}
		
		public override bool Equals(object obj)
		{
			Property property = obj as Property;
			
			if (property != null)
			{
				return (this.owner_type == property.owner_type)
					&& (this.name == property.name);
			}
			
			return false;
		}

		
		public static Property Register(string name, System.Type property_type, System.Type owner_type)
		{
			return Property.Register (name, property_type, owner_type, new PropertyMetadata ());
		}
		
		public static Property Register(string name, System.Type property_type, System.Type owner_type, PropertyMetadata metadata)
		{
			Property dp = new Property (name, property_type, owner_type, metadata);
			
			Object.Register (dp);
			
			return dp;
		}
		
		
		private string							name;
		private System.Type						property_type;
		private System.Type						owner_type;
		private PropertyMetadata				default_metadata;
	}
}
