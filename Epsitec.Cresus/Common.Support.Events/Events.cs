//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	public delegate void EventHandler(object sender);
	public delegate void ArgEventHandler(object sender, object arg);
	public delegate void CancelEventHandler(object sender, CancelEventArgs e);
	public delegate void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e);
	
	public class EventArgs : System.EventArgs
	{
		public EventArgs()
		{
		}
	}
	
	public class CancelEventArgs : System.ComponentModel.CancelEventArgs
	{
		public CancelEventArgs()
		{
		}
	}
	
	public class PropertyChangedEventArgs : System.EventArgs
	{
		public PropertyChangedEventArgs(string property_name)
		{
			this.property_name = property_name;
		}
		
		
		public virtual string					PropertyName
		{
			get
			{
				return this.property_name;
			}
		}
		
		
		private string							property_name;
	}
}
