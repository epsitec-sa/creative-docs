//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.IO
{
	public delegate bool LoadPredicate(string entryName);
	
	/// <summary>
	/// The <c>ZipFile</c> class manages ZIP file loading and saving. The contents
	/// of the ZIP file can be accessed as individual entries, each stored as a byte
	/// array.
	/// </summary>
	public sealed class ZipFile
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ZipFile"/> class.
		/// </summary>
		public ZipFile()
		{
			this.entries = new List<Entry> ();
		}


		/// <summary>
		/// Gets or sets the compression level.
		/// </summary>
		/// <value>The compression level (<c>0</c> = store only, <c>5</c> = default, <c>9</c> = best compression).</value>
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

		/// <summary>
		/// Enumerates the entry names.
		/// </summary>
		/// <value>The entry names.</value>
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

		/// <summary>
		/// Enumerates the directory names.
		/// </summary>
		/// <value>The directory names.</value>
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

		/// <summary>
		/// Enumerates the entries.
		/// </summary>
		/// <value>The entries.</value>
		public IEnumerable<Entry> Entries
		{
			get
			{
				return this.entries;
			}
		}

		/// <summary>
		/// Gets the <see cref="Entry"/> with the specified name.
		/// </summary>
		/// <value>The <see cref="Entry"/> with the specified name.</value>
		public Entry this[string name]
		{
			get
			{
				return this.entries.Find (delegate (Entry entry) { return entry.Name == name; });
			}
		}


		/// <summary>
		/// Tries to load the specified ZIP file.
		/// </summary>
		/// <param name="name">The file name.</param>
		/// <returns><c>true</c> on success; otherwise, <c>false</c>.</returns>
		public bool TryLoadFile(string name)
		{
			return this.TryLoadFile (name, delegate (string entryName) { return true; });
		}

		/// <summary>
		/// Tries to load the specified ZIP file. Only the entries for which the predicate
		/// returns true will be effectively loaded.
		/// </summary>
		/// <param name="name">The file name.</param>
		/// <param name="loadPredicate">The load predicate which must return true if an
		/// entry is to be loaded.</param>
		/// <returns><c>true</c> on success; otherwise, <c>false</c>.</returns>
		public bool TryLoadFile(string name, LoadPredicate loadPredicate)
		{
			try
			{
				using (System.IO.Stream stream = System.IO.File.OpenRead (name))
				{
					int b1 = stream.ReadByte ();
					int b2 = stream.ReadByte ();
					int b3 = stream.ReadByte ();
					int b4 = stream.ReadByte ();
					
					stream.Position = 0;

					if ((b1 == 'P') &&
						(b2 == 'K') &&
						(b3 == 3) &&
						(b4 == 4))
					{
						this.LoadFile (stream, loadPredicate);

						return true;
					}
				}

			}
			catch (System.IO.IOException)
			{
			}
			
			return false;
		}

		/// <summary>
		/// Loads the specified ZIP file.
		/// </summary>
		/// <param name="name">The file name.</param>
		public void LoadFile(string name)
		{
			using (System.IO.Stream stream = System.IO.File.OpenRead (name))
			{
				this.LoadFile (stream, delegate (string entryName) { return true; });
			}
		}

		/// <summary>
		/// Loads the specified ZIP stream.
		/// </summary>
		/// <param name="stream">The file stream.</param>
		public void LoadFile(System.IO.Stream stream, LoadPredicate loadPredicate)
		{
			this.entries.Clear ();

			ICSharpCode.SharpZipLib.Zip.ZipInputStream zip = new ICSharpCode.SharpZipLib.Zip.ZipInputStream (stream);
			ICSharpCode.SharpZipLib.Zip.ZipEntry entry;
			
			while ((entry = zip.GetNextEntry ()) != null)
			{
				if (entry.IsDirectory == false)
				{
					if (loadPredicate (entry.Name))
					{
						byte[] data   = new byte[entry.Size];
						int size   = data.Length;
						int offset = 0;

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
				}
				else
				{
					this.entries.Add (new Entry (entry.Name, entry.DateTime));
				}
			}
			
			zip.Close ();
		}

		/// <summary>
		/// Saves to the specified ZIP file.
		/// </summary>
		/// <param name="name">The file name.</param>
		public void SaveFile(string name)
		{
			using (System.IO.Stream stream = System.IO.File.Create (name))
			{
				this.SaveFile (stream);
			}
		}

		/// <summary>
		/// Saves to the specified ZIP stream.
		/// </summary>
		/// <param name="stream">The stream.</param>
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


		/// <summary>
		/// Adds a file entry to the ZIP file.
		/// </summary>
		/// <param name="name">The file entry name. It can include a relative path using
		/// exclusively forward slashes as separators.</param>
		/// <param name="data">The data.</param>
		public void AddEntry(string name, byte[] data)
		{
			this.AddEntry (name, data, System.DateTime.Now);
		}

		/// <summary>
		/// Adds a file entry to the ZIP file.
		/// </summary>
		/// <param name="name">The file entry name. It can include a relative path using
		/// exclusively forward slashes as separators.</param>
		/// <param name="data">The data.</param>
		/// <param name="dateTime">The creation date time.</param>
		public void AddEntry(string name, byte[] data, System.DateTime dateTime)
		{
			System.Diagnostics.Debug.Assert (string.IsNullOrEmpty (name) == false);
			System.Diagnostics.Debug.Assert (name.Contains ("\\") == false);
			System.Diagnostics.Debug.Assert (name.StartsWith ("/") == false);
			System.Diagnostics.Debug.Assert (name.EndsWith ("/") == false);

			this.AddDirectory (System.IO.Path.GetDirectoryName (name));
			
			this.entries.RemoveAll (delegate (Entry entry) { return (entry.Name == name); });
			this.entries.Add (new Entry (name, data, dateTime));
		}

		/// <summary>
		/// Adds an empty directory to the ZIP file. This is done automatically when
		/// adding a file entry for which there is not yet a directory entry.
		/// </summary>
		/// <param name="name">The directory entry name.</param>
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

		/// <summary>
		/// Removes the file entry.
		/// </summary>
		/// <param name="name">The file entry name.</param>
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

		
		/// <summary>
		/// The <c>Entry</c> structure maps a file name to its data and date/time
		/// stamp.
		/// </summary>
		public struct Entry
		{
			internal Entry(string fileName, byte[] data, System.DateTime date)
			{
				this.name = fileName;
				this.data = data;
				this.date = date;
			}

			internal Entry(string directoryName, System.DateTime date)
			{
				this.name = directoryName;
				this.data = null;
				this.date = date;
			}

			/// <summary>
			/// Gets the full name of the entry.
			/// </summary>
			/// <value>The full name.</value>
			public string Name
			{
				get
				{
					return this.name;
				}
			}

			/// <summary>
			/// Gets or sets the data of the entry.
			/// </summary>
			/// <value>The data.</value>
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

			/// <summary>
			/// Gets the date and time associated with the entry.
			/// </summary>
			/// <value>The date and time.</value>
			public System.DateTime DateTime
			{
				get
				{
					return this.date;
				}
			}

			/// <summary>
			/// Gets a value indicating whether this entry is a directory.
			/// </summary>
			/// <value>
			/// 	<c>true</c> if this entry is a directory; otherwise, <c>false</c>.
			/// </value>
			public bool IsDirectory
			{
				get
				{
					return this.data == null;
				}
			}

			/// <summary>
			/// Gets a value indicating whether this entry is empty.
			/// </summary>
			/// <value><c>true</c> if this entry is empty; otherwise, <c>false</c>.</value>
			public bool IsEmpty
			{
				get
				{
					return this.name == null;
				}
			}

			/// <summary>
			/// The default empty entry.
			/// </summary>
			public static Entry Empty = new Entry ();

			private string name;
			private byte[] data;
			private System.DateTime date;
		}

		private int level = 5;
		private List<Entry> entries;
	}
}
