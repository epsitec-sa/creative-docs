//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 15/04/2004

namespace Epsitec.Common.IO
{
	/// <summary>
	/// La classe Decompression supporte la décompression d'un stream
	/// avec détection automatique du format utilisé à la compression.
	/// </summary>
	public class Decompression : AbstractStream
	{
		protected Decompression(System.IO.Stream stream)
		{
			this.Stream = stream;
		}
		
		
		public static System.IO.Stream CreateStream(System.IO.Stream stream)
		{
			string name;
			return Decompression.CreateStream (stream, out name);
		}
		
		public static System.IO.Stream CreateStream(System.IO.Stream stream, out string name)
		{
			byte[] header = new byte[8];
			name = null;
			
			try
			{
				stream.Read (header, 0, 8);
				
				return Decompression.CreateStreamFromHeader (stream, header, out name);
			}
			catch (System.IO.IOException)
			{
			}
			
			return null;
		}
		
		public static System.IO.Stream CreateStreamFromHeader(System.IO.Stream stream, byte[] header, out string name)
		{
			name = null;
			
			if ((header.Length == 8) &&
				(header[0] == '$') &&
				(header[6] == 13) &&
				(header[7] == 10))
			{
				if ((header[1] == 'B') &&
					(header[2] == 'Z') &&
					(header[3] == 'I') &&
					(header[4] == 'P') &&
					(header[5] == '2'))
				{
					ICSharpCode.SharpZipLib.BZip2.BZip2InputStream decompressor = new ICSharpCode.SharpZipLib.BZip2.BZip2InputStream (stream);
					
					return new Decompression (decompressor);
				}
				
				if ((header[1] == 'Z') &&
					(header[2] == 'I') &&
					(header[3] == 'P') &&
					(header[4] == 32) &&
					(header[5] == 32))
				{
					ICSharpCode.SharpZipLib.Zip.ZipInputStream decompressor = new ICSharpCode.SharpZipLib.Zip.ZipInputStream (stream);
					ICSharpCode.SharpZipLib.Zip.ZipEntry       entry        = decompressor.GetNextEntry ();
					
					name = entry.Name;
					
					return new Decompression (decompressor);
				}
				
				if ((header[1] == 'Z') &&
					(header[2] == '1') &&
					(header[3] == '9') &&
					(header[4] == '5') &&
					(header[5] == '1'))
				{
					ICSharpCode.SharpZipLib.Zip.Compression.Streams.InflaterInputStream decompressor = new ICSharpCode.SharpZipLib.Zip.Compression.Streams.InflaterInputStream (stream);
					
					return new Decompression (decompressor);
				}
				
				if ((header[1] == 'G') &&
					(header[2] == 'Z') &&
					(header[3] == 'I') &&
					(header[4] == 'P') &&
					(header[5] == 32))
				{
					ICSharpCode.SharpZipLib.GZip.GZipInputStream decompressor = new ICSharpCode.SharpZipLib.GZip.GZipInputStream (stream);
					
					return new Decompression (decompressor);
				}
			}
			
			return null;
		}
		
		public static byte[] DecompressToArray(System.IO.Stream stream)
		{
			System.IO.Stream decompressor = Decompression.CreateStream (stream);
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			int    total = 0;
			byte[] data  = null;
			
			for (;;)
			{
				byte[] buffer = new byte[10000];
				int n = decompressor.Read (buffer, 0, buffer.Length);
					
				total += n;
					
				if (n < buffer.Length)
				{
					data = new byte[total];
					int copy_to = 0;
						
					foreach (byte[] chunk in list)
					{
						chunk.CopyTo (data, copy_to);
						copy_to += chunk.Length;
					}
					
					for (int i = 0; i < n; i++)
					{
						data[copy_to+i] = buffer[i];
					}
					
					list.Clear ();
					break;
				}
				
				list.Add (buffer);
			}
			
			decompressor.Close ();
			return data;
		}
	}
}
