//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Support.CodeGeneration
{
	/// <summary>
	/// The <c>CodeAccessibility</c> enumeration lists all possible code
	/// accessibility types (such as <c>abstract</c>, <c>virtual</c> or
	/// <c>static</c>).
	/// </summary>
	public enum CodeAccessibility : byte
	{
		/// <summary>
		/// The code item has its default accessibility.
		/// </summary>
		Default,

		/// <summary>
		/// The code item is declared final, which means it cannot be overridden
		/// by any other method or property.
		/// </summary>
		Final,

		/// <summary>
		/// The code item is declared <c>abstract</c>.
		/// </summary>
		Abstract,
		
		/// <summary>
		/// The code item is declared <c>virtual</c>.
		/// </summary>
		Virtual,
		
		/// <summary>
		/// The code item is declared overridden (with the <c>override</c> keyword).
		/// </summary>
		Override,

		/// <summary>
		/// The code item is declared <c>static</c>.
		/// </summary>
		Static,
		
		/// <summary>
		/// The code item is declared constant (with the <c>const</c> keyword).
		/// </summary>
		Constant,
		
		/// <summary>
		/// The code item is declared <c>sealed</c>. This only applies to classes.
		/// </summary>
		Sealed,
	}
}
