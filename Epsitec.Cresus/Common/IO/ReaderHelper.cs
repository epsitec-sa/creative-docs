//	Copyright © 2005-2011, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.IO
{
	/// <summary>
	/// The <c>ReaderHelper</c> class provides several support functions used
	/// to read from files and from streams.
	/// </summary>
	public static class ReaderHelper
	{
		/// <summary>
		/// Reads the specified stream until the end of the stream has been reached
		/// or the expected amount of data has been read.
		/// </summary>
		/// <param name="stream">The stream to read from.</param>
		/// <param name="buffer">The buffer where to store the data.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="length">The length to read.</param>
		/// <returns>The number of bytes read.</returns>
		public static int Read(System.IO.Stream stream, byte[] buffer, int offset, int length)
		{
			if (length == 0)
			{
				return 0;
			}
			
			int read  = stream.Read (buffer, offset, length);
			int total = read;
			
			while (total < length)
			{
				//	Il se peut que le Read n'ait pas retourné tout ce
				//	qui lui a été demandé, sans pour autant que la fin
				//	ait été atteinte (par ex. lors de décompression).
				
				//	Dans ce cas, il faut tenter de lire la suite par
				//	petits morceaux :
				
				read = stream.Read (buffer, offset + total, length - total);
				
				if (read == 0)
				{
					break;
				}
				
				total += read;
			}
			
			return total;
		}

		/// <summary>
		/// Splits the source text into non empty lines.
		/// </summary>
		/// <param name="source">The source text.</param>
		/// <returns>A collection of non empty lines.</returns>
		public static IEnumerable<string> SplitLines(string source)
		{
			int pos = 0;
			int start = 0;

			bool newLine = true;

			while (pos < source.Length)
			{
				char c = source[pos++];

				if ((c == '\n') ||
					(c == '\r'))
				{
					if (newLine)
					{
						start = pos;
						continue;
					}
					else
					{
						yield return source.Substring (start, pos-start-1);

						newLine = true;
						start = pos;
					}
				}
				else
				{
					newLine = false;
				}
			}
		}

		/// <summary>
		/// Reads the text from the stream, decoding it with the specified encoding.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <param name="encoding">The encoding.</param>
		/// <returns>The text.</returns>
		public static string ReadText(System.IO.Stream stream, System.Text.Encoding encoding)
		{
			int  length = (int) (stream.Length - stream.Position);
			byte[] data = new byte[length];

			int count = stream.Read (data, 0, length);

			if (count != length)
			{
				throw new System.InvalidOperationException ("Read did not return expected byte count");
			}

			string text = encoding.GetString (data);
			return text;
		}
	}
}
