//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>FieldMembership</c> enumeration specifies where a field is
	/// originally defined.
	/// See <see cref="StructuredTypeField"/>.
	/// </summary>
	[DesignerVisible]
	public enum FieldMembership
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
		/// Ths field is inherited through interface implementation.
		/// </summary>
		Interface=2,
	}
}
