//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 15/04/2004

namespace Epsitec.Common.Support.IO
{
	/// <summary>
	/// La classe Compression donne accès aux divers streams comprimés
	/// (BZip2, GZip, Deflate = LZW & Huffman, ZIP).
	/// </summary>
	public class Compression : AbstractStream
	{
		protected Compression(System.IO.Stream stream)
		{
			this.Stream = stream;
		}
		
		
		public override void Close()
		{
			System.IO.Stream stream = this.Stream;
			
			if (stream is ICSharpCode.SharpZipLib.BZip2.BZip2OutputStream)
			{
				ICSharpCode.SharpZipLib.BZip2.BZip2OutputStream compressor = stream as ICSharpCode.SharpZipLib.BZip2.BZip2OutputStream;
				
				compressor.Flush ();
			}
			else if (stream is ICSharpCode.SharpZipLib.Zip.ZipOutputStream)
			{
				ICSharpCode.SharpZipLib.Zip.ZipOutputStream compressor = stream as ICSharpCode.SharpZipLib.Zip.ZipOutputStream;
				
				compressor.Flush ();
				compressor.Finish ();
			}
			else if (stream is ICSharpCode.SharpZipLib.Zip.Compression.Streams.DeflaterOutputStream)
			{
				ICSharpCode.SharpZipLib.Zip.Compression.Streams.DeflaterOutputStream compressor = stream as ICSharpCode.SharpZipLib.Zip.Compression.Streams.DeflaterOutputStream;
				
				compressor.Flush ();
				compressor.Finish ();
			}
			else if (stream is ICSharpCode.SharpZipLib.GZip.GZipOutputStream)
			{
				ICSharpCode.SharpZipLib.GZip.GZipOutputStream compressor = stream as ICSharpCode.SharpZipLib.GZip.GZipOutputStream;
				
				compressor.Flush ();
				compressor.Finish ();
			}
			
			base.Close ();
		}
		
		
		public static System.IO.Stream CreateBZip2Stream(System.IO.Stream stream)
		{
			byte[] header = new byte[] { (byte)'$', (byte)'B', (byte)'Z', (byte)'I', (byte)'P', (byte)'2', 13, 10 };
			stream.Write (header, 0, 8);
			
			ICSharpCode.SharpZipLib.BZip2.BZip2OutputStream compressor = new ICSharpCode.SharpZipLib.BZip2.BZip2OutputStream (stream);
			
			return new Compression (compressor);
		}
		
		public static System.IO.Stream CreateZipStream(System.IO.Stream stream, string name)
		{
			byte[] header = new byte[] { (byte)'$', (byte)'Z', (byte)'I', (byte)'P', 32, 32, 13, 10 };
			stream.Write (header, 0, 8);
			
			ICSharpCode.SharpZipLib.Zip.ZipOutputStream compressor = new ICSharpCode.SharpZipLib.Zip.ZipOutputStream (stream);
			ICSharpCode.SharpZipLib.Zip.ZipEntry entry = new ICSharpCode.SharpZipLib.Zip.ZipEntry (name);
			
			compressor.SetLevel (9);
			compressor.PutNextEntry (entry);
			
			return new Compression (compressor);
		}
		
		public static System.IO.Stream CreateDeflateStream(System.IO.Stream stream, int level)
		{
			byte[] header = new byte[] { (byte)'$', (byte)'Z', (byte)'1', (byte)'9', (byte)'5', (byte)'1', 13, 10 };
			stream.Write (header, 0, 8);
			
			ICSharpCode.SharpZipLib.Zip.Compression.Deflater deflater = new ICSharpCode.SharpZipLib.Zip.Compression.Deflater (level);
			ICSharpCode.SharpZipLib.Zip.Compression.Streams.DeflaterOutputStream compressor = new ICSharpCode.SharpZipLib.Zip.Compression.Streams.DeflaterOutputStream (stream, deflater);
			
			return new Compression (compressor);
		}
		
		public static System.IO.Stream CreateGZipStream(System.IO.Stream stream)
		{
			byte[] header = new byte[] { (byte)'$', (byte)'G', (byte)'Z', (byte)'I', (byte)'P', 32, 13, 10 };
			stream.Write (header, 0, 8);
			
			ICSharpCode.SharpZipLib.GZip.GZipOutputStream compressor = new ICSharpCode.SharpZipLib.GZip.GZipOutputStream (stream);
			
			return new Compression (compressor);
		}
	}
}
