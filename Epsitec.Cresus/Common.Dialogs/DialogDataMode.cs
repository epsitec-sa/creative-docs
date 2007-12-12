//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// The <c>DialogDataMode</c> enumeration defines the modes which can be
	/// used with the data represented by <see cref="DialogData"/>.
	/// </summary>
	public enum DialogDataMode
	{
		/// <summary>
		/// The isolated mode does not reflect data changes until the dialog
		/// is validated.
		/// </summary>
		Isolated,

		/// <summary>
		/// The real-time mode updates the data on the fly, as it is being
		/// edited in the dialog.
		/// </summary>
		RealTime
	}
}
