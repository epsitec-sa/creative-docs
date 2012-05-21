//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>DbColumnClass</c> enumeration specifies what class of data a
	/// column stores.
	/// </summary>
	public enum DbColumnClass : byte
	{
		/// <summary>
		/// The column contains data.
		/// </summary>
		Data				= 0,

		/// <summary>
		/// The column contains a key ID.
		/// </summary>
		KeyId				= 1,

		/// <summary>
		/// The column contains a reference to a key ID.
		/// </summary>
		RefId				= 3,

		/// <summary>
		/// The column contains an internal reference which is not really handled
		/// like a foreign key.
		/// </summary>
		RefInternal			= 4,

		Virtual				= 5,
	}
}
