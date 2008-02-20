//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>FieldRelation</c> enumeration defines what relation binds a
	/// field from one structure with other structures.
	/// See <see cref="StructuredTypeField"/>.
	/// </summary>
	[DesignerVisible]
	public enum FieldRelation : byte
	{
		/// <summary>
		/// There is no relation defined for this field.
		/// </summary>
		None=0,

		/// <summary>
		/// The field defines a reference (pointer) to another structure.
		/// </summary>
		Reference=1,

		/// <summary>
		/// The field defines a collection of items.
		/// </summary>
		Collection=3,
	}
}
