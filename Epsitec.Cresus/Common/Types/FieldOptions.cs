//	Copyright © 2007-2012, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>FieldOptions</c> enumeration defines a set of options which can
	/// be attached to a field.
	/// See <see cref="StructuredTypeField"/>.
	/// </summary>
	[DesignerVisible]
	[System.Flags]
	public enum FieldOptions : byte
	{
		/// <summary>
		/// There is no option defined for this field.
		/// </summary>
		None=0x00,

		/// <summary>
		/// The field is nullable, which basically means that it can be empty,
		/// i.e. have no value at all.
		/// </summary>
		Nullable=0x01,

		/// <summary>
		/// The field relation points to a private (i.e. not shared) target.
		/// </summary>
		PrivateRelation=0x02,

		/// <summary>
		/// The field should be used to generate an ascending index (0..n).
		/// </summary>
		IndexAscending=0x04,
		
		/// <summary>
		/// The field should be used to generate a descending index (n..0).
		/// </summary>
		IndexDescending=0x08,

		/// <summary>
		/// The field is virtual: it has no direct database backing and will
		/// not be persisted as is.
		/// </summary>
		Virtual=0x10,
	}
}
