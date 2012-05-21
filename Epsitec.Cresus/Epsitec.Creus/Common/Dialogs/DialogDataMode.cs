//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
		RealTime,

		/// <summary>
		/// The transparent mode does not track changes and binds the dialog
		/// directly to the provided data. Use this if the user interface must
		/// reflect external changes in the provided data.
		/// </summary>
		Transparent,

		/// <summary>
		/// The search mode considers all data typed in by the user as defining
		/// a search template; the original data will never be altered.
		/// </summary>
		Search,
	}
}
