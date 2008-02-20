//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
	}
}
