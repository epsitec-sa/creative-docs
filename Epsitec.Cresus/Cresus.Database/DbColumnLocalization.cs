//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>DbColumnLocalization</c> enumeration specifies if a column can
	/// store localized data or not.
	/// </summary>
	public enum DbColumnLocalization : byte
	{
		/// <summary>
		/// The column may not store localized data.
		/// </summary>
		None=0,

		/// <summary>
		/// The column may contain localized data.
		/// </summary>
		Localized=1,
	}
}
