//	Copyright © 2003-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	public delegate void EventHandler(object sender);
	public delegate void EventHandler<TEventArgs>(object sender, TEventArgs e);
}
