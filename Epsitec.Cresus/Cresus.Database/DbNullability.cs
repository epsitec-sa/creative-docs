//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>DbNullability</c> enumeration defines the different nullable
	/// modes supported by the database. Basically, this is <c>Yes</c> and <c>No</c>.
	/// </summary>
	public enum DbNullability : byte
	{
		/// <summary>
		/// Undefined nullability.
		/// </summary>
		Undefined,
		
		/// <summary>
		/// Does not accept <c>null</c> as a valid value.
		/// </summary>
		No,
		
		/// <summary>
		/// Accepts <c>null</c> as a valid value.
		/// </summary>
		Yes,
	}
}
