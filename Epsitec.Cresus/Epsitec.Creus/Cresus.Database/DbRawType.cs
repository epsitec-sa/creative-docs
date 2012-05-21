//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>DbRawType</c> enumeration lists all possible low level types
	/// which can be used with an ADO.NET provider.
	/// </summary>
	public enum DbRawType : byte
	{
		/// <summary>
		/// Unknown or unsupported type.
		/// </summary>
		Unknown = 0,
		
		/// <summary>
		/// Null, type cannot be inferred.
		/// </summary>
		Null = 1,
		
		/// <summary>
		/// Boolean (1-bit value).
		/// </summary>
		Boolean = 2,
		
		/// <summary>
		/// Integer (16-bit value).
		/// </summary>
		Int16 = 3,
		
		/// <summary>
		/// Integer (32-bit value).
		/// </summary>
		Int32 = 4,
		
		/// <summary>
		/// Integer (64-bit value).
		/// </summary>
		Int64 = 5,
		
		/// <summary>
		/// Small decimal value (9 digits integer part + 9 digits fractional part).
		/// </summary>
		SmallDecimal = 6,

		/// <summary>
		/// Large decimal value (15 digits integer part + 3 digits fractional part).
		/// </summary>
		LargeDecimal = 7,
		
		/// <summary>
		/// Date only.
		/// </summary>
		Date = 8,
		
		/// <summary>
		/// Time only.
		/// </summary>
		Time = 9,
		
		/// <summary>
		/// Date and Time data with at least a 1ms resolution.
		/// </summary>
		DateTime = 10,

		/// <summary>
		/// Unicode text.
		/// </summary>
		String = 11,
		
		/// <summary>
		/// Byte array.
		/// </summary>
		ByteArray = 12,

		/// <summary>
		/// Globally unique identifier, 128-bit value.
		/// </summary>
		Guid = 13,
	}
}
