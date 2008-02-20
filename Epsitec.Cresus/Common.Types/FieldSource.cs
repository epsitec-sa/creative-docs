//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>FieldSource</c> enumeration specifies how a field is
	/// defined (it is either a value or an expression).
	/// See <see cref="StructuredTypeField"/>.
	/// </summary>
	[DesignerVisible]
	public enum FieldSource : byte
	{
		/// <summary>
		/// The field holds a value.
		/// </summary>
		Value=0,

		/// <summary>
		/// The field is the result of the evaluation of an expression.
		/// </summary>
		Expression=1,
	}
}
