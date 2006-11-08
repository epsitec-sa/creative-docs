//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>DbIdClass</c> enumeration identifies the class into which a <c>DbId</c>
	/// belongs.
	/// </summary>
	public enum DbIdClass : byte
	{
		/// <summary>
		/// This is a standard identifier.
		/// </summary>
		Standard,

		/// <summary>
		/// This is a temporary identifier.
		/// </summary>
		Temporary,

		/// <summary>
		/// This is an invalid identifier.
		/// </summary>
		Invalid
	}
}
