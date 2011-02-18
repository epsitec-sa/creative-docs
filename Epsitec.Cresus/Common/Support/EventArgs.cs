//	Copyright © 2003-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	/// <summary>
	///	The <c>EventArgs</c> class is the base class used to provide event
	/// handlers with arguments.
	/// </summary>
	public class EventArgs : System.EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EventArgs"/> class.
		/// </summary>
		public EventArgs()
		{
		}


		public static readonly new EventArgs Empty = new EventArgs ();
	}
}
