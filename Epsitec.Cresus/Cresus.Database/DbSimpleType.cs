//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>DbSimpleType</c> enumeration lists the simplified types used to store
	/// data into the intermediate database layer.
	/// </summary>
	public enum DbSimpleType : byte
	{
		/// <summary>
		/// Not supported.
		/// </summary>
		Unsupported = 0,

		/// <summary>
		/// Null, type cannot be inferred.
		/// </summary>
		Null = 1,
		
		/// <summary>
		/// Numeric data (boolean, integer, real, time span, etc.).
		/// </summary>
		Decimal = 2,

		/// <summary>
		/// Unicode text.
		/// </summary>
		String = 3,

		/// <summary>
		/// Date only.
		/// </summary>
		Date = 4,

		/// <summary>
		/// Time only.
		/// </summary>
		Time = 5,

		/// <summary>
		/// Date and Time data with at least a 1ms resolution.
		/// </summary>
		DateTime = 6,

		/// <summary>
		/// Byte array.
		/// </summary>
		ByteArray = 7,
		
		/// <summary>
		/// Globally unique identifier, 128-bit value.
		/// </summary>
		Guid = 8,
	}
}
