//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Core.Controllers.CreationControllers
{
	/// <summary>
	/// The <c>CreationStatus</c> identifies the status of an entity. See also
	/// <see cref="CreationViewController&lt;T&gt;"/>.
	/// </summary>
	public enum CreationStatus
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
		/// The entity is in a ready state.
		/// </summary>
		Ready,
	}
}
