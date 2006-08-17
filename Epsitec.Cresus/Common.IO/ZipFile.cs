//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.IO
{
	public class ZipFile
	{
		public ZipFile()
		{
			this.entries = new List<Entry> ();
		}

		public int CompressionLevel
		{
			get
			{
				return this.level;
			}
			set
			{
				this.level = value;
			}
		}

		public IEnumerable<string> EntryNames
		{
			get
			{
				foreach (Entry entry in this.entries)
				{
					if (entry.IsDirectory)
					{
						//	Skip directory entries...
					}
					else
					{
						yield return entry.Name;
					}
				}
			}
		}

		public IEnumerable<string> DirectoryNames
		{
			get
			{
				foreach (Entry entry in this.entries)
				{
					if (entry.IsDirectory)
					{
						yield return entry.Name;
					}
				}
			}
		}
		
		public void LoadFile(string name)
		{
			using (System.IO.Stream stream = System.IO.File.OpenRead (name))
			{
				this.LoadFile (stream);
			}
		}
		
		public void LoadFile(System.IO.Stream stream)
		{
			this.entries.Clear ();

			ICSharpCode.SharpZipLib.Zip.ZipInputStream zip = new ICSharpCode.SharpZipLib.Zip.ZipInputStream (stream);
			ICSharpCode.SharpZipLib.Zip.ZipEntry entry;
			
			while ((entry = zip.GetNextEntry ()) != null)
			{
				if (entry.IsDirectory == false)
				{
					byte[] data   = new byte[entry.Size];
					int    size   = data.Length;
					int    offset = 0;
					
					while (true)
					{
						int length = zip.Read (data, offset, size);

						offset += length;
						size   -= length;

						if (length == 0)
						{
							break;
						}
					}

					System.Diagnostics.Debug.Assert (size == 0);
					
					this.entries.Add (new Entry (entry.Name, data, entry.DateTime));
				}
				else
				{
					this.entries.Add (new Entry (entry.Name, entry.DateTime));
				}
			}
			
			zip.Close ();
		}

		public void SaveFile(string name)
		{
			using (System.IO.Stream stream = System.IO.File.Create (name))
			{
				this.SaveFile (stream);
			}
		}

		public void SaveFile(System.IO.Stream stream)
		{
			this.entries.Sort (delegate (Entry a, Entry b) { return string.Compare (a.Name, b.Name); });
			ICSharpCode.SharpZipLib.Checksums.Crc32 crc = new ICSharpCode.SharpZipLib.Checksums.Crc32 ();
			ICSharpCode.SharpZipLib.Zip.ZipOutputStream zip = new ICSharpCode.SharpZipLib.Zip.ZipOutputStream (stream);

			zip.SetLevel (this.level);

			foreach (Entry entry in this.entries)
			{
				this.WriteEntry (zip, entry, crc);
			}
			
			zip.Finish ();
			zip.Close ();
		}


		public void AddEntry(string name, byte[] data)
		{
			System.Diagnostics.Debug.Assert (string.IsNullOrEmpty (name) == false);
			System.Diagnostics.Debug.Assert (name.Contains ("\\") == false);
			System.Diagnostics.Debug.Assert (name.StartsWith ("/") == false);
			System.Diagnostics.Debug.Assert (name.EndsWith ("/") == false);

			this.AddDirectory (System.IO.Path.GetDirectoryName (name));
			
			this.entries.RemoveAll (delegate (Entry entry) { return (entry.Name == name); });
			this.entries.Add (new Entry (name, data, System.DateTime.Now));
		}

		public void AddDirectory(string name)
		{
			if (string.IsNullOrEmpty (name))
			{
				return;
			}
			
			System.Diagnostics.Debug.Assert (name.Contains ("\\") == false);
			System.Diagnostics.Debug.Assert (name.StartsWith ("/") == false);

			if (name.EndsWith ("/") == false)
			{
				name = name + "/";
			}
			
			if (this.entries.Exists (delegate (Entry entry) { return entry.IsDirectory && entry.Name == name; }))
			{
				//	OK, found.
			}
			else
			{
				this.entries.Add (new Entry (name, System.DateTime.Now));
			}
		}

		public void RemoveEntry(string name)
		{
			System.Diagnostics.Debug.Assert (string.IsNullOrEmpty (name) == false);
			System.Diagnostics.Debug.Assert (name.Contains ("\\") == false);
			System.Diagnostics.Debug.Assert (name.StartsWith ("/") == false);
			System.Diagnostics.Debug.Assert (name.EndsWith ("/") == false);
			
			this.entries.RemoveAll (delegate (Entry entry) { return entry.Name == name; });
		}

		
		
		private void WriteEntry(ICSharpCode.SharpZipLib.Zip.ZipOutputStream zip, Entry entry, ICSharpCode.SharpZipLib.Checksums.Crc32 crc)
		{
			ICSharpCode.SharpZipLib.Zip.ZipEntry e = new ICSharpCode.SharpZipLib.Zip.ZipEntry (entry.Name);

			if (entry.IsDirectory)
			{
				e.DateTime = entry.DateTime;
				
				zip.PutNextEntry (e);
			}
			else
			{
				crc.Reset ();
				crc.Update (entry.Data);

				e.Crc = crc.Value;
				e.DateTime = entry.DateTime;
				e.Size = entry.Data.Length;

				zip.PutNextEntry (e);
				zip.Write (entry.Data, 0, entry.Data.Length);
			}
		}

		
		private struct Entry
		{
			public Entry(string fileName, byte[] data, System.DateTime date)
			{
				this.name = fileName;
				this.data = data;
				this.date = date;
			}

			public Entry(string directoryName, System.DateTime date)
			{
				this.name = directoryName;
				this.data = null;
				this.date = date;
			}

			public string Name
			{
				get
				{
					return this.name;
				}
			}

			public byte[] Data
			{
				get
				{
					return this.data;
				}
				set
				{
					this.data = value;
				}
			}

			public System.DateTime DateTime
			{
				get
				{
					return this.date;
				}
			}

			public bool IsDirectory
			{
				get
				{
					return this.data == null;
				}
			}

			private string name;
			private byte[] data;
			private System.DateTime date;
		}

		int level = 5;
		List<Entry> entries;
	}
}
