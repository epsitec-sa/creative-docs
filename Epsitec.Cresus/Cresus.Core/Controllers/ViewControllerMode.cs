//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

namespace Epsitec.Cresus.Core.Controllers
{
	/// <summary>
	/// The <c>ViewControllerMode</c> enumeration specifies which specific
	/// <see cref="EntityViewController"/> should be used.
	/// </summary>
	public enum ViewControllerMode
	{
		/// <summary>
		/// No controller specified.
		/// </summary>
		None,

		/// <summary>
		/// Use the summary view controller (read only).
		/// </summary>
		Summary,

		/// <summary>
		/// Use the edition view controller (read write).
		/// </summary>
		Edition,
	}
}
