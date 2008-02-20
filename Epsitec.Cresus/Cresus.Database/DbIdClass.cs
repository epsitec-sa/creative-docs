//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
