using NUnit.Framework;

namespace Epsitec.Common.Support
{
	[TestFixture] public class IOTest
	{
		[Test]
		public void CheckBZip2Stream()
		{
			byte[] buffer = System.Text.Encoding.UTF8.GetBytes (IOTest.SampleText);
			
			System.IO.MemoryStream target = new System.IO.MemoryStream ();
			System.IO.Stream   compressor = IO.Compression.CreateBZip2Stream (target);
			
			compressor.Write (buffer, 0, buffer.Length);
			compressor.Close ();
			
			byte[] data = target.ToArray ();
			long length = data.Length;
			
			System.Console.Out.WriteLine ("Source size : {0} bytes", buffer.Length);
			System.Console.Out.WriteLine ("Compressed size : {0} bytes, using BZIP2", length);
			
			System.IO.MemoryStream source = new System.IO.MemoryStream (data);
			System.IO.Stream decompressor = IO.Decompression.CreateStream (source);
			
			byte[] read = new byte[buffer.Length];
			
			decompressor.Read (read, 0, buffer.Length);
			decompressor.Close ();
			
			for (int i = 0; i < buffer.Length; i++)
			{
				Assert.AreEqual (buffer[i], read[i], string.Format ("offset {0}: {1} != {2}", i, (char) buffer[i], (char) read[i]));
			}
		}

		[Test]
		public void CheckDeflateStreamLevel1()
		{
			byte[] buffer = System.Text.Encoding.UTF8.GetBytes (IOTest.SampleText);
			
			System.IO.MemoryStream target = new System.IO.MemoryStream ();
			System.IO.Stream   compressor = IO.Compression.CreateDeflateStream (target, 1);
			
			compressor.Write (buffer, 0, buffer.Length);
			compressor.Close ();
			
			byte[] data = target.ToArray ();
			long length = data.Length;
			
			System.Console.Out.WriteLine ("Compressed size : {0} bytes, using DEFLATE-1", length);
			
			System.IO.MemoryStream source = new System.IO.MemoryStream (data);
			System.IO.Stream decompressor = IO.Decompression.CreateStream (source);
			
			byte[] read = new byte[buffer.Length];
			
			decompressor.Read (read, 0, buffer.Length);
			decompressor.Close ();
			
			for (int i = 0; i < buffer.Length; i++)
			{
				Assert.AreEqual (buffer[i], read[i], string.Format ("offset {0}: {1} != {2}", i, (char) buffer[i], (char) read[i]));
			}
		}

		[Test]
		public void CheckDeflateStreamLevel9()
		{
			byte[] buffer = System.Text.Encoding.UTF8.GetBytes (IOTest.SampleText);
			
			System.IO.MemoryStream target = new System.IO.MemoryStream ();
			System.IO.Stream   compressor = IO.Compression.CreateDeflateStream (target, 9);
			
			compressor.Write (buffer, 0, buffer.Length);
			compressor.Close ();
			
			byte[] data = target.ToArray ();
			long length = data.Length;
			
			System.Console.Out.WriteLine ("Compressed size : {0} bytes, using DEFLATE-9", length);
			
			System.IO.MemoryStream source = new System.IO.MemoryStream (data);
			System.IO.Stream decompressor = IO.Decompression.CreateStream (source);
			
			byte[] read = new byte[buffer.Length];
			
			decompressor.Read (read, 0, buffer.Length);
			decompressor.Close ();
			
			for (int i = 0; i < buffer.Length; i++)
			{
				Assert.AreEqual (buffer[i], read[i], string.Format ("offset {0}: {1} != {2}", i, (char) buffer[i], (char) read[i]));
			}
		}

		[Test]
		public void CheckGZipStream()
		{
			byte[] buffer = System.Text.Encoding.UTF8.GetBytes (IOTest.SampleText);
			
			System.IO.MemoryStream target = new System.IO.MemoryStream ();
			System.IO.Stream   compressor = IO.Compression.CreateGZipStream (target);
			
			compressor.Write (buffer, 0, buffer.Length);
			compressor.Close ();
			
			byte[] data = target.ToArray ();
			long length = data.Length;
			
			System.Console.Out.WriteLine ("Compressed size : {0} bytes, using GZIP", length);
			
			System.IO.MemoryStream source = new System.IO.MemoryStream (data);
			System.IO.Stream decompressor = IO.Decompression.CreateStream (source);
			
			byte[] read = new byte[buffer.Length];
			
			decompressor.Read (read, 0, buffer.Length);
			decompressor.Close ();
			
			for (int i = 0; i < buffer.Length; i++)
			{
				Assert.AreEqual (buffer[i], read[i], string.Format ("offset {0}: {1} != {2}", i, (char) buffer[i], (char) read[i]));
			}
		}

		[Test]
		public void CheckChecksumCRC()
		{
			IO.IChecksum checksum = IO.Checksum.CreateCrc32 ();
			byte[] buffer = System.Text.Encoding.UTF8.GetBytes (IOTest.SampleText);
			
			Assert.AreEqual (0, checksum.Value);
			
			checksum.Reset ();
			Assert.AreEqual (0, checksum.Value);
			checksum.Update (buffer);
			
			long value_0 = checksum.Value;
			
			byte b10 = buffer[10];
			
			buffer[10] = buffer[20];
			buffer[20] = b10;
			
			checksum.Reset ();
			Assert.AreEqual (0, checksum.Value);
			checksum.Update (buffer);
			
			long value_1 = checksum.Value;
			
			System.Console.Out.WriteLine ();
			System.Console.Out.WriteLine ("CRC32 : {0:X}, after byte swap {1:X}", value_0, value_1);
			
			Assert.IsTrue (value_0 != value_1);
		}

		[Test]
		public void CheckChecksumAdler()
		{
			IO.IChecksum checksum = IO.Checksum.CreateAdler32 ();
			byte[] buffer = System.Text.Encoding.UTF8.GetBytes (IOTest.SampleText);
			
			Assert.AreEqual (1, checksum.Value);
			
			checksum.Reset ();
			Assert.AreEqual (1, checksum.Value);
			checksum.Update (buffer);
			
			long value_0 = checksum.Value;
			
			byte b10 = buffer[10];
			
			buffer[10] = buffer[20];
			buffer[20] = b10;
			
			checksum.Reset ();
			Assert.AreEqual (1, checksum.Value);
			checksum.Update (buffer);
			
			long value_1 = checksum.Value;
			
			System.Console.Out.WriteLine ("Adler : {0:X}, after byte swap {1:X}", value_0, value_1);
			
			Assert.IsTrue (value_0 != value_1);
		}

		[Test]
		public void CheckChecksumMd5()
		{
			byte[] buffer = System.Text.Encoding.UTF8.GetBytes (IOTest.SampleText);

			string value_0 = IO.Checksum.ComputeMd5Hash (buffer);

			byte b10 = buffer[10];

			buffer[10] = buffer[20];
			buffer[20] = b10;

			string value_1 = IO.Checksum.ComputeMd5Hash (buffer);

			System.Console.Out.WriteLine ();
			System.Console.Out.WriteLine ("MD5 before: {0}\n" +
										  "MD5 after:  {1}", value_0, value_1);

			Assert.IsTrue (value_0 != value_1);
		}

		[Test]
		public void CheckDeflateCompressor()
		{
			byte[] data = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 1, 2, 3, 1, 2, 3, 5, 6, 7, 8, 1, 2, 3, 3, 4, 5, 2, 3, 7, 8, 1, 2, 3, 1, 2, 3, 5, 6, 7, 8, 1, 2, 3 };
			
			byte[] compressed = IO.DeflateCompressor.Compress (data, 9);
			byte[] decompressed = IO.DeflateCompressor.Decompress (compressed);
			
			System.Console.Out.WriteLine ("Raw data length: {0}, compressed: {1}, decompressed: {2}", data.Length, compressed.Length, decompressed.Length);
			
			Assert.IsTrue (Types.Comparer.Equal (data, decompressed), "Original Data != Decompressed Data");
		}

		[Test]
		public void CheckZipFile()
		{
			IO.ZipFile file = new IO.ZipFile ();

			file.AddEntry ("file 1.txt", System.Text.Encoding.UTF8.GetBytes ("Hello, simple file.\r\nAt the root of the ZIP archive.\r\n"));
			file.AddEntry ("images/i1.bin", new byte[] { 0x1, 0x2, 0x3, 0x4, 0x5, 0x1, 0x2, 0x3, 0x4, 0x5 });
			file.AddEntry ("images/i2.bin", new byte[] { 0x1, 0x2, 0x3, 0x4, 0x5, 0x1, 0x2, 0x3, 0x4, 0x5 });
			file.AddEntry ("file 2.txt", System.Text.Encoding.UTF8.GetBytes ("Other, simple file.\r\nAt the root of the ZIP archive.\r\n"));
			file.AddEntry ("text/more.txt", System.Text.Encoding.UTF8.GetBytes ("More simple text in a subfolder in the ZIP archive.\r\n"));
			file.AddDirectory ("empty folder");

			file.SaveFile ("t1.zip");

			IO.ZipFile read = new IO.ZipFile ();

			read.LoadFile ("t1.zip");

			string[] entryNames = Types.Collection.ToArray (read.EntryNames);
			string[] directoryNames = Types.Collection.ToArray (read.DirectoryNames);

			Assert.AreEqual (5, entryNames.Length);
			Assert.AreEqual ("file 1.txt", entryNames[0]);
			Assert.AreEqual ("file 2.txt", entryNames[1]);
			Assert.AreEqual ("images/i1.bin", entryNames[2]);
			Assert.AreEqual ("images/i2.bin", entryNames[3]);
			Assert.AreEqual ("text/more.txt", entryNames[4]);

			Assert.AreEqual (3, directoryNames.Length);
			Assert.AreEqual ("empty folder/", directoryNames[0]);
			Assert.AreEqual ("images/", directoryNames[1]);
			Assert.AreEqual ("text/", directoryNames[2]);

			Assert.IsTrue (new System.TimeSpan (System.Math.Abs (file["file 2.txt"].DateTime.Ticks - read["file 2.txt"].DateTime.Ticks)).Seconds <= 1);
			Assert.AreEqual (file["file 2.txt"].Data.Length, read["file 2.txt"].Data.Length);
			Assert.IsTrue (read["foo"].IsEmpty);
			Assert.AreEqual ("Hello, simple file.\r\nAt the root of the ZIP archive.\r\n", System.Text.Encoding.UTF8.GetString (read["file 1.txt"].Data));
			
			read.RemoveEntry ("file 2.txt");
			read.RemoveEntry ("images/i1.bin");

			read.SaveFile ("t2.zip");

			Assert.IsTrue (read.TryLoadFile ("t2.zip"));
			Assert.IsFalse (read.TryLoadFile ("t3.zip"));

			System.IO.File.WriteAllBytes ("t2.zip", new byte[] { 0, 1, 2 });
			
			Assert.IsFalse (read.TryLoadFile ("t2.zip"));
		}

		[Test]
		public void CheckZipStream()
		{
			byte[] buffer = System.Text.Encoding.UTF8.GetBytes (IOTest.SampleText);

			System.IO.MemoryStream target = new System.IO.MemoryStream ();
			System.IO.Stream compressor = IO.Compression.CreateZipStream (target, "test");

			compressor.Write (buffer, 0, buffer.Length);
			compressor.Close ();

			byte[] data = target.ToArray ();
			long length = data.Length;

			System.Console.Out.WriteLine ("Compressed size : {0} bytes, using ZIP", length);

			System.IO.MemoryStream source = new System.IO.MemoryStream (data);
			System.IO.Stream decompressor = IO.Decompression.CreateStream (source);

			byte[] read = new byte[buffer.Length];

			decompressor.Read (read, 0, buffer.Length);
			decompressor.Close ();

			for (int i = 0; i < buffer.Length; i++)
			{
				Assert.AreEqual (buffer[i], read[i], string.Format ("offset {0}: {1} != {2}", i, (char) buffer[i], (char) read[i]));
			}
		}


		private static string SampleText
		{
			get
			{
				return "Ceci est un petit essai qui devrait permettre de comparer la qualité de la "
					 + "compression de divers algorithmes, appliqués à un morceau de texte relativement "
					 + "simple. L'utilisation de tags <italique>XML</italique> dans un tel <gras>texte</gras> "
					 + "est quelque peu <italique>artificielle</italique>, mais c'est un <gras>test</gras>, "
					 + "alors que veut-on de plus&#160;? Voici donc le XML alibi&#160;:\n"
					 + "<root>\n"
					+ "  <element name='abc'>\n"
					+ "    <node arg='1'/>\n"
					+ "    <node arg='2'/>\n"
					+ "    <node arg='3'/>\n"
					+ "    <node arg='4'/>\n"
					+ "    <node arg='5'/>\n"
					+ "    <node arg='6'/>\n"
					+ "  </element>\n"
					+ "  <element name='xyz'>\n"
					+ "    <node arg='3'/>\n"
					+ "    <node arg='6'/>\n"
					+ "    <node arg='8'/>\n"
					+ "    <node arg='10'/>\n"
					+ "  </element>\n"
					+ "</root>\n";
			}
		}
	}
}
