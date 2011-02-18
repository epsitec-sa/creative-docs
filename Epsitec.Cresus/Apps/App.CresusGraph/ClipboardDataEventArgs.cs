//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

namespace Epsitec.Cresus.Graph
{
	/// <summary>
	/// The <c>ClipboardDataEventArgs</c> class represents the data stored in the
	/// clipboard, associated with a <c>ClipboardDataChanged</c> event.
	/// </summary>
	public class ClipboardDataEventArgs : EventArgs
	{
		public ClipboardDataEventArgs(string text)
		{
			this.text = text;
			this.format = ClipboardDataFormat.Text;
		}


		public string Text
		{
			get
			{
				return this.text;
			}
		}

		public ClipboardDataFormat Format
		{
			get
			{
				return this.format;
			}
		}

		
		private readonly string text;
		private readonly ClipboardDataFormat format;
	}
}
