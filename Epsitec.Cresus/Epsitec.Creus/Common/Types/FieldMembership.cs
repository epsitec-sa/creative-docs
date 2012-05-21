//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
		/// The field is redefined locally (e.g. override of an expression
		/// defined by a locally included interface).
		/// </summary>
		LocalOverride=2,
	}
}
