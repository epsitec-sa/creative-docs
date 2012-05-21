//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.IO
{
	/// <summary>
	/// La classe DeflateCompressor permet de réaliser une compression compatible
	/// avec ZLIB/DEFLATE (RFC 1951).
	/// </summary>
	public sealed class DeflateCompressor
	{
		private DeflateCompressor()
		{
		}
		
		
		public static byte[] Compress(byte[] data, int level)
		{
			System.IO.MemoryStream output = new System.IO.MemoryStream ();
			ICSharpCode.SharpZipLib.Zip.Compression.Deflater deflater = new ICSharpCode.SharpZipLib.Zip.Compression.Deflater (level);
			ICSharpCode.SharpZipLib.Zip.Compression.Streams.DeflaterOutputStream compressor = new ICSharpCode.SharpZipLib.Zip.Compression.Streams.DeflaterOutputStream (output, deflater);
			
			compressor.Write (data, 0, data.Length);
			compressor.Flush ();
			compressor.Close ();
			
			return output.ToArray ();
		}
		
		public static byte[] Decompress(byte[] data)
		{
			System.IO.MemoryStream input = new System.IO.MemoryStream (data, 0, data.Length, false);
			System.IO.MemoryStream output = new System.IO.MemoryStream ();
			
			ICSharpCode.SharpZipLib.Zip.Compression.Streams.InflaterInputStream decompressor = new ICSharpCode.SharpZipLib.Zip.Compression.Streams.InflaterInputStream (input);
			
			byte[] buffer = new byte[1024];
			
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
