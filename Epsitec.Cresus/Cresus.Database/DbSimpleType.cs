//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>DbSimpleType</c> enumeration lists the simplified types used to store
	/// data into the intermediate database layer.
	/// </summary>
	public enum DbSimpleType
	{
		/// <summary>
		/// Not supported.
		/// </summary>
		Unsupported,

		/// <summary>
		/// Null, type cannot be inferred.
		/// </summary>
		Null,
		
		/// <summary>
		/// Numeric data (boolean, integer, real, time span, etc.).
		/// </summary>
		Decimal,

		/// <summary>
		/// Unicode text.
		/// </summary>
		String,

		/// <summary>
		/// Date only.
		/// </summary>
		Date,

		/// <summary>
		/// Time only.
		/// </summary>
		Time,

		/// <summary>
		/// Date and Time data with at least a 1ms resolution.
		/// </summary>
		DateTime,

		/// <summary>
		/// Byte array.
		/// </summary>
		ByteArray,
		
		/// <summary>
		/// Globally unique identifier, 128-bit value.
		/// </summary>
		Guid,
	}
}
