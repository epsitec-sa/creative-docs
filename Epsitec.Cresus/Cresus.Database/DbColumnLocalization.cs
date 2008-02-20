//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>DbColumnLocalization</c> enumeration specifies if a column can
	/// store localized data or not.
	/// </summary>
	public enum DbColumnLocalization : byte
	{
		/// <summary>
		/// No localization has been defined; this is the unknown state.
		/// </summary>
		Unknown=0,

		/// <summary>
		/// The column may not store localized data.
		/// </summary>
		None=1,

		/// <summary>
		/// The column may contain localized data.
		/// </summary>
		Localized=2,
	}
}
