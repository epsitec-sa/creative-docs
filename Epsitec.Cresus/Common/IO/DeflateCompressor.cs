//	Copyright © 2005-2013, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.IO
{
	/// <summary>
	/// La classe DeflateCompressor permet de réaliser une compression compatible
	/// avec ZLIB/DEFLATE (RFC 1951).
	/// </summary>
	public static class DeflateCompressor
	{
		public static byte[] Compress(byte[] data, int level)
		{
			var output     = new System.IO.MemoryStream ();
			var deflater   = new ICSharpCode.SharpZipLib.Zip.Compression.Deflater (level);
			var compressor = new ICSharpCode.SharpZipLib.Zip.Compression.Streams.DeflaterOutputStream (output, deflater);

			int chunkSize = 1024*1024;

			for (int pos = 0; pos < data.Length; pos += chunkSize)
			{
				var remaining = data.Length - pos;
				var blockSize = System.Math.Min (chunkSize, remaining);

				compressor.Write (data, pos, blockSize);
			}

			compressor.Flush ();
			compressor.Close ();
			
			return output.ToArray ();
		}
		
		public static byte[] Decompress(byte[] data)
		{
			System.IO.MemoryStream input = new System.IO.MemoryStream (data, 0, data.Length, false);
			System.IO.MemoryStream output = new System.IO.MemoryStream ();
			
			ICSharpCode.SharpZipLib.Zip.Compression.Streams.InflaterInputStream decompressor = new ICSharpCode.SharpZipLib.Zip.Compression.Streams.InflaterInputStream (input);
			
			byte[] buffer = new byte[1024*64];
			
			for (;;)
			{
				int read = decompressor.Read (buffer, 0, buffer.Length);
				
				if (read == 0)
				{
					break;
				}
				
				output.Write (buffer, 0, read);
			}
			
			decompressor.Close ();
			
			return output.ToArray ();
		}
	}
}
