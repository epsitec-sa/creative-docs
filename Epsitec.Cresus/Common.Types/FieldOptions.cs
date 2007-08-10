//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>FieldOptions</c> enumeration ...
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
	}
}
