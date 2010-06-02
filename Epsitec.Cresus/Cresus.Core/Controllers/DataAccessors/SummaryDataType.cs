//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Core.Controllers.DataAccessors
{
	/// <summary>
	/// The <c>SummaryDataType</c> defines the type of a <see cref="SummaryData"/> object.
	/// </summary>
	public enum SummaryDataType
	{
		/// <summary>
		/// Undefined type.
		/// </summary>
		Undefined,

		/// <summary>
		/// Simple item: represents plain vanilla fields of an entity.
		/// </summary>
		SimpleItem,

		/// <summary>
		/// Empty item: used as a template to create collection items.
		/// </summary>
		EmptyItem,

		/// <summary>
		/// Collection item: automatically created by a collection accessor, based on
		/// a template (usually an empty item).
		/// </summary>
		CollectionItem,
	}
}
