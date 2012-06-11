//	Copyright © 2012, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	public delegate void EventHandler<TSender, TEventArgs>(TSender sender, TEventArgs e)
		where TEventArgs : System.EventArgs;
}
