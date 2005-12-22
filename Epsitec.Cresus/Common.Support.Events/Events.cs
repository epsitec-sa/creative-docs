//	Copyright © 2003-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	public delegate void EventHandler(object sender);
	public delegate void ArgEventHandler(object sender, object arg);
	public delegate void CancelEventHandler(object sender, CancelEventArgs e);
	
	#region EventArgs Class
	public class EventArgs : System.EventArgs
	{
		public EventArgs()
		{
		}
	}
	#endregion
	
	#region CancelEventArgs Class
	public class CancelEventArgs : System.ComponentModel.CancelEventArgs
	{
		public CancelEventArgs()
		{
		}
	}
	#endregion
}
