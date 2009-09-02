//	Copyright © 2004-2009, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>ClipboardDataFormat</c> enumeration defines all data formats which
	/// are supported by the <see cref="Clipboard"/> class.
	/// </summary>
	public enum ClipboardDataFormat
	{
		/// <summary>
		/// No data.
		/// </summary>
		None,

		/// <summary>
		/// Unsupported data format.
		/// </summary>
		Unsupported,

		/// <summary>
		/// Text data format.
		/// </summary>
		Text,

		/// <summary>
		/// Image data format.
		/// </summary>
		Image,

		/// <summary>
		/// HTML data format (using the Microsoft HTML fragment representation).
		/// </summary>
		MicrosoftHtml
	}
}
