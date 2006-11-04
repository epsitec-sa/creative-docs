//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>DbRawType</c> enumeration lists all possible low level types
	/// which can be used with an ADO.NET provider.
	/// </summary>
	public enum DbRawType
	{
		/// <summary>
		/// Unsupported type.
		/// </summary>
		Unsupported,
		
		/// <summary>
		/// Unknown type.
		/// </summary>
		Unknown = Unsupported,
		
		/// <summary>
		/// Null, type cannot be inferred.
		/// </summary>
		Null,
		
		/// <summary>
		/// Boolean (1-bit value).
		/// </summary>
		Boolean,
		
		/// <summary>
		/// Integer (16-bit value).
		/// </summary>
		Int16,
		
		/// <summary>
		/// Integer (32-bit value).
		/// </summary>
		Int32,
		
		/// <summary>
		/// Integer (64-bit value).
		/// </summary>
		Int64,
		
		/// <summary>
		/// Small decimal value (9 digits integer part + 9 digits fractional part).
		/// </summary>
		SmallDecimal,

		/// <summary>
		/// Large decimal value (15 digits integer part + 3 digits fractional part).
		/// </summary>
		LargeDecimal,
		
		
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
		/// Unicode text.
		/// </summary>
		String,
		
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
