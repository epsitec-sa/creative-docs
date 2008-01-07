//	Copyright © 2007-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>FieldMembership</c> enumeration specifies where a field is
	/// originally defined. Do not change the values, as they are used in
	/// the serialization of <see cref="StructuredTypeField"/>.
	/// </summary>
	[DesignerVisible]
	public enum FieldMembership : byte
	{
		/// <summary>
		/// The field is defined locally.
		/// </summary>
		Local=0,

		/// <summary>
		/// The field is inherited from some parent or base class.
		/// </summary>
		Inherited=1,

		/// <summary>
		/// The field is redefined locally.
		/// </summary>
		LocalOverride=2
	}
}
