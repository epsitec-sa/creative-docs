//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>DbColumnLocalisation</c> enumeration specifies if a column can
	/// store localised data.
	/// </summary>
	public enum DbColumnLocalisation : byte
	{
		/// <summary>
		/// The column may not store localised data.
		/// </summary>
		None=0,

		/// <summary>
		/// The column contains the default variant of the localised data.
		/// </summary>
		Default=1,

		/// <summary>
		/// The column contains a language specific variant of the localised data.
		/// </summary>
		Localised=2
	}
}
