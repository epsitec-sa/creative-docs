//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	public delegate void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e);
	
	public class PropertyChangedEventArgs : System.EventArgs
	{
		public PropertyChangedEventArgs(Property property, object old_value, object new_value)
		{
			this.property = property;
			this.property_name = property.Name;
			this.old_value = old_value;
			this.new_value = new_value;
		}
		
		public PropertyChangedEventArgs(string property_name, object old_value, object new_value)
		{
			this.property_name = property_name;
			this.old_value = old_value;
			this.new_value = new_value;
		}
		
		
		public Property							Property
		{
			get
			{
				return this.property;
			}
		}
		
		public string							PropertyName
		{
			get
			{
				return this.property_name;
			}
		}
		
		public object							OldValue
		{
			get
			{
				return this.old_value;
			}
		}
		public object							NewValue
		{
			get
			{
				return this.new_value;
			}
		}
		
		
		private Property						property;
		private string							property_name;
		private object							old_value;
		private object							new_value;
	}
}
