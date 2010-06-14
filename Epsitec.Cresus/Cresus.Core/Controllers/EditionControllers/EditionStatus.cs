//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	/// <summary>
	/// The <c>EditionStatus</c> identifies the status of an entity. See also
	/// <see cref="EditionViewController&lt;T&gt;"/>.
	/// </summary>
	public enum EditionStatus
	{
		/// <summary>
		/// Unknown status.
		/// </summary>
		Unknown,

		/// <summary>
		/// The entity is empty.
		/// </summary>
		Empty,

		/// <summary>
		/// The entity is in a valid state.
		/// </summary>
		Valid,
		
		/// <summary>
		/// The entity is in an invalid state.
		/// </summary>
		Invalid,
	}
}
