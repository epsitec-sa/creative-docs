//	Copyright © 2010-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Core.Controllers.DataAccessors
{
	/// <summary>
	/// The <c>TileDataType</c> defines the type of a <see cref="TileDataItem"/> object.
	/// </summary>
	public enum TileDataType
	{
		/// <summary>
		/// Undefined type.
		/// </summary>
		Undefined,

		/// <summary>
		/// Simple item: represents plain vanilla (read-only) fields of an entity.
		/// </summary>
		SimpleItem,

		/// <summary>
		/// Editable item: represents editable fields of an entity.
		/// </summary>
		EditableItem,

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
