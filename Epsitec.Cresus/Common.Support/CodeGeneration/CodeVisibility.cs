//	Copyright © 2007-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Support.CodeGeneration
{
	/// <summary>
	/// The <c>CodeVisibility</c> enumeration lists all possible code access
	/// visibilities defined by the C# language.
	/// </summary>
	public enum CodeVisibility : byte
	{
		/// <summary>
		/// The code has no specific access visibility.
		/// </summary>
		None,

		/// <summary>
		/// The code item is declared <c>public</c>.
		/// </summary>
		Public,
		
		/// <summary>
		/// The code item is declared <c>internal</c>.
		/// </summary>
		Internal,
		
		/// <summary>
		/// The code item is declared <c>protected</c>.
		/// </summary>
		Protected,
		
		/// <summary>
		/// The code item is declared <c>private</c>.
		/// </summary>
		Private
	}
}
