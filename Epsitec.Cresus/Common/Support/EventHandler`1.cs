//	Copyright © 2003-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	public delegate void EventHandler<TEventArgs>(object sender, TEventArgs e)
		where TEventArgs : System.EventArgs;
}
