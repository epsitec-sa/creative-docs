//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	public class DependencyPropertyChangedEventArgs : System.EventArgs
	{
		public DependencyPropertyChangedEventArgs(DependencyProperty property, object oldValue, object newValue)
		{
			this.property = property;
			this.propertyName = property.Name;
			this.oldValue = oldValue;
			this.newValue = newValue;
		}

		public DependencyPropertyChangedEventArgs(string propertyName, object oldValue, object newValue)
		{
			this.propertyName = propertyName;
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
		
		public DependencyPropertyChangedEventArgs(string propertyName)
		{
			this.propertyName = propertyName;
			this.oldValue = null;
			this.newValue = null;
		}
		
		public DependencyProperty				Property
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
				return this.propertyName;
			}
		}
		
		public object							OldValue
		{
			get
			{
				return this.oldValue;
			}
		}
		public object							NewValue
		{
			get
			{
				return this.newValue;
			}
		}
		
		
		private DependencyProperty				property;
		private string							propertyName;
		private object							oldValue;
		private object							newValue;
	}
}
