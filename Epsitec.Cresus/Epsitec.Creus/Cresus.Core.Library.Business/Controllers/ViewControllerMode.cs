//	Copyright © 2010-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
		None = 0,

		/// <summary>
		/// Use the summary view controller (read only).
		/// </summary>
		Summary = 1,

		/// <summary>
		/// Use the edition view controller (read write).
		/// </summary>
		Edition = 2,

		/// <summary>
		/// Use the creation view controller (special case, read only based on dummy bootstrap entity).
		/// </summary>
		Creation = 3,

		/// <summary>
		/// Same as <see cref="Creation"/> if the entity is not sufficiently defined; otherwise
		/// same as <see cref="Summary"/>.
		/// </summary>
		CreationOrSummary = 4,

		/// <summary>
		/// Same as <see cref="Creation"/> if the entity is not sufficiently defined; otherwise
		/// same as <see cref="Edition"/>.
		/// </summary>
		CreationOrEdition = 5,
	}
}