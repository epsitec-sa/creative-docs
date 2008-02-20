//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.OpenType
{
	/// <summary>
	/// The <c>Support</c> class provides a few methods used to manipulate
	/// OpenType font tables.
	/// </summary>
	public sealed class Support
	{
		/// <summary>
		/// Reads a 16-bit integer value.
		/// </summary>
		/// <param name="data">The raw table data.</param>
		/// <param name="offset">The offset into the table data.</param>
		/// <returns>The 16-bit integer value.</returns>
		public static uint ReadInt16(byte[] data, int offset)
		{
			return (uint) (data[offset+0] << 8) | (data[offset+1]);
		}

		/// <summary>
		/// Reads a 32-bit integer value.
		/// </summary>
		/// <param name="data">The raw table data.</param>
		/// <param name="offset">The offset into the table data.</param>
		/// <returns>The 32-bit integer value.</returns>
		public static uint ReadInt32(byte[] data, int offset)
		{
			return (uint) (data[offset+0] << 24) | (uint) (data[offset+1] << 16) | (uint) (data[offset+2] << 8) | (data[offset+3]);
		}
	}
}
