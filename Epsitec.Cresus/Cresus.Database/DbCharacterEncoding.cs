//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>DbCharacterEncoding</c> enumeration defines well known character
	/// encodings such as Unicode or ASCII.
	/// </summary>
	public enum DbCharacterEncoding : byte
	{
		/// <summary>
		/// Unknown encoding mode.
		/// </summary>
		Unknown = 0,
		
		/// <summary>
		/// Unicode encoding.
		/// </summary>
		Unicode = 1,

		/// <summary>
		/// ASCII encoding.
		/// </summary>
		Ascii = 2,
	}
}
