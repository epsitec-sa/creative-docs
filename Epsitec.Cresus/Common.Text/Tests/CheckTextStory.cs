//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Tests
{
	/// <summary>
	/// Vérifie le bon fonctionnement de la classe TextStory.
	/// </summary>
	public sealed class CheckTextStory
	{
		public static void RunTests()
		{
			TextStory story1 = new TextStory ();
			TextStory story2 = new TextStory ();
			TextStory story3 = new TextStory ();
			
			int cursor_id = story1.NewCursor ();
			
			ulong[] text = new ulong[6000];
			
			for (int i = 0; i < text.Length; i += 4)
			{
				text[i+0] = 'A';
				text[i+1] = 'b';
				text[i+2] = 'c';
				text[i+3] = ' ';
			}
			
			System.Diagnostics.Trace.WriteLine ("Generating 8MB of text.");
			while (story1.TextLength < 1*1000*1000)
			{
				story1.InsertText (cursor_id, text);
			}
			
			System.Diagnostics.Trace.WriteLine ("Saving file.");
			using (System.IO.FileStream file = new System.IO.FileStream (@"c:\test.text", System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write))
			{
				story1.WriteRawText (file);
			}
			System.Diagnostics.Trace.WriteLine ("Saving file, done.");
			
			
			System.Diagnostics.Trace.WriteLine ("Loading file.");
			using (System.IO.FileStream file = new System.IO.FileStream (@"c:\test.text", System.IO.FileMode.Open, System.IO.FileAccess.Read))
			{
				story2.ReadRawText (file);
			}
			System.Diagnostics.Trace.WriteLine ("Loading file, done.");
			
			Debug.Assert.IsTrue (story1.TextLength == story2.TextLength);
			
			
			System.Diagnostics.Trace.WriteLine ("Saving compressed file.");
			using (System.IO.FileStream file = new System.IO.FileStream (@"c:\test.compressed", System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write))
			{
				using (System.IO.Stream stream = Common.IO.Compression.CreateDeflateStream (file, 1))
				{
					story1.WriteRawText (stream);
					stream.Flush ();
					file.SetLength (file.Position);
				}
			}
			System.Diagnostics.Trace.WriteLine ("Saving compressed file, done.");
			
			System.Diagnostics.Trace.WriteLine ("Loading compressed file.");
			using (System.IO.FileStream file = new System.IO.FileStream (@"c:\test.compressed", System.IO.FileMode.Open, System.IO.FileAccess.Read))
			{
				using (System.IO.Stream stream = Common.IO.Decompression.CreateStream (file))
				{
					story3.ReadRawText (stream);
				}
			}
			System.Diagnostics.Trace.WriteLine ("Loading compressed file, done.");
			
			
#if false
			using (System.IO.FileStream input = new System.IO.FileStream (@"c:\source.txt", System.IO.FileMode.Open, System.IO.FileAccess.Read))
			{
				using (System.IO.FileStream file = new System.IO.FileStream (@"c:\deflate-1.bin", System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write))
				{
					using (System.IO.Stream stream = Common.IO.Compression.CreateDeflateStream (file, 1))
					{
						byte[] data = new byte[1000];
						for (;;)
						{
							int read = input.Read (data, 0, 1000);
							if (read == 0) break;
							stream.Write (data, 0, read);
						}
					}
				}
			}
			
			using (System.IO.FileStream input = new System.IO.FileStream (@"c:\source.txt", System.IO.FileMode.Open, System.IO.FileAccess.Read))
			{
				using (System.IO.FileStream file = new System.IO.FileStream (@"c:\deflate-9.bin", System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write))
				{
					using (System.IO.Stream stream = Common.IO.Compression.CreateDeflateStream (file, 9))
					{
						byte[] data = new byte[1000];
						for (;;)
						{
							int read = input.Read (data, 0, 1000);
							if (read == 0) break;
							stream.Write (data, 0, read);
						}
					}
				}
			}
			
			using (System.IO.FileStream input = new System.IO.FileStream (@"c:\source.txt", System.IO.FileMode.Open, System.IO.FileAccess.Read))
			{
				using (System.IO.FileStream file = new System.IO.FileStream (@"c:\bzip2.bin", System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write))
				{
					using (System.IO.Stream stream = Common.IO.Compression.CreateBZip2Stream (file))
					{
						byte[] data = new byte[1000];
						for (;;)
						{
							int read = input.Read (data, 0, 1000);
							if (read == 0) break;
							stream.Write (data, 0, read);
						}
					}
				}
			}
			
			using (System.IO.FileStream input = new System.IO.FileStream (@"c:\source.txt", System.IO.FileMode.Open, System.IO.FileAccess.Read))
			{
				using (System.IO.FileStream file = new System.IO.FileStream (@"c:\gzip.bin", System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write))
				{
					using (System.IO.Stream stream = Common.IO.Compression.CreateGZipStream (file))
					{
						byte[] data = new byte[1000];
						for (;;)
						{
							int read = input.Read (data, 0, 1000);
							if (read == 0) break;
							stream.Write (data, 0, read);
						}
					}
				}
			}
			
			using (System.IO.FileStream input = new System.IO.FileStream (@"c:\source.txt", System.IO.FileMode.Open, System.IO.FileAccess.Read))
			{
				using (System.IO.FileStream file = new System.IO.FileStream (@"c:\zip.bin", System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write))
				{
					using (System.IO.Stream stream = Common.IO.Compression.CreateZipStream (file, "Z"))
					{
						byte[] data = new byte[1000];
						for (;;)
						{
							int read = input.Read (data, 0, 1000);
							if (read == 0) break;
							stream.Write (data, 0, read);
						}
					}
				}
			}
#endif
		}
	}
}
