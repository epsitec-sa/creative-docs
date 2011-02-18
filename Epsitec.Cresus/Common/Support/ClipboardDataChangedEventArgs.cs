//	Copyright © 2009, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>ClipboardDataChangedEventArgs</c> class stores the new clipboard
	/// data after the system clipboard was changed.
	/// </summary>
	public class ClipboardDataChangedEventArgs : EventArgs
	{
		public ClipboardDataChangedEventArgs(ClipboardReadData data)
		{
			this.data = data;
		}


		public ClipboardReadData Data
		{
			get
			{
				return this.data;
			}
		}

		
		private readonly ClipboardReadData data;
	}
}
