//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	public delegate void SimpleCallback();
	public delegate void EventHandler(object sender);
	public delegate void EventHandler<TEventArgs>(object sender, TEventArgs e);
}
