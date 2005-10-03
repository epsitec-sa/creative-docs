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
		private object							old_value;
		private object							new_value;
	}
}
