//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	public delegate void EventHandler(object sender);
	public delegate void CancelEventHandler(object sender, CancelEventArgs e);
	
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
}
