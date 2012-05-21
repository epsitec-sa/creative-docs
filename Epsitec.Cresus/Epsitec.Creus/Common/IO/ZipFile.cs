//	Copyright © 2006-2012, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

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
		/// Gets the name of the ZIP file at load time.
		/// </summary>
		/// <value>The name of the ZIP file at load time.</value>
		public string LoadFileName
		{
			get
			{
				return this.loadFileName;
			}
			set
			{
				this.loadFileName = value;
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
					this.loadFileName = name;
					return this.TryLoadFile (stream, loadPredicate);
				}
			}
			catch (System.IO.IOException)
			{
			}

			return false;
		}


		/// <summary>
		/// Tries to load the specified ZIP file. Only the entries for which the predicate
		/// returns true will be effectively loaded.
		/// </summary>
		/// <param name="stream">The file stream.</param>
		/// <param name="loadPredicate">The load predicate which must return true if an
		/// entry is to be loaded.</param>
		/// <returns>
		/// 	<c>true</c> on success; otherwise, <c>false</c>.
		/// </returns>
		public bool TryLoadFile(System.IO.Stream stream, LoadPredicate loadPredicate)
		{
			try
			{
				long position = stream.Position;

				int b1 = stream.ReadByte ();
				int b2 = stream.ReadByte ();
				int b3 = stream.ReadByte ();
				int b4 = stream.ReadByte ();
				
				stream.Position = position;

				if ((b1 == 'P') &&
					(b2 == 'K') &&
					(b3 == 3) &&
					(b4 == 4))
				{
					this.LoadFile (stream, loadPredicate);

					return true;
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
				this.loadFileName = name;
				this.LoadFile (stream, delegate (string entryName) { return true; });
			}
		}

		/// <summary>
		/// Loads the specified ZIP stream.
		/// </summary>
		/// <param name="stream">The file stream.</param>
		/// <param name="loadPredicate">The load predicate (or <c>null</c> to load everything).</param>
		/// <returns><c>true</c> if the ZIP file could be loaded; otherwise, <c>false</c>.</returns>
		public bool LoadFile(System.IO.Stream stream, LoadPredicate loadPredicate = null)
		{
			this.entries.Clear ();

			ICSharpCode.SharpZipLib.Zip.ZipInputStream zip = new ICSharpCode.SharpZipLib.Zip.ZipInputStream (stream);
			ICSharpCode.SharpZipLib.Zip.ZipEntry entry;

			try
			{
				while ((entry = zip.GetNextEntry ()) != null)
				{
					if (entry.IsDirectory == false)
					{
						if ((loadPredicate == null) || (loadPredicate (entry.Name)))
						{
							byte[] data = ZipFile.ReadEntry (zip, entry);
							this.entries.Add (new Entry (entry.Name, data, entry.DateTime, (int) entry.ZipFileIndex, entry.CompressionMethod == ICSharpCode.SharpZipLib.Zip.CompressionMethod.Deflated));
						}
					}
					else
					{
						this.entries.Add (new Entry (entry.Name, entry.DateTime, entry.Size, (int) entry.ZipFileIndex, entry.CompressionMethod == ICSharpCode.SharpZipLib.Zip.CompressionMethod.Deflated));
					}
				}
			}
			catch (System.Exception ex)
			{
				System.Diagnostics.Debug.WriteLine ("ZipFile.LoadFile failed: " + ex.Message);

				//	Error while reading the ZIP file : we won't crash, but return an empty entry list.
				
				this.entries.Clear ();
				
				return false;
			}
			finally
			{
				zip.Close ();
			}

			return true;
		}

		/// <summary>
		/// Loads the specified ZIP from an embedded resource.
		/// </summary>
		/// <param name="assembly">The assembly.</param>
		/// <param name="path">The embedded resource path.</param>
		/// <param name="loadPredicate">The load predicate (or <c>null</c> to load everything).</param>
		/// <returns>
		///   <c>true</c> if the ZIP file could be loaded; otherwise, <c>false</c>.
		/// </returns>
		public bool LoadFileFromResources(System.Reflection.Assembly assembly, string path, LoadPredicate loadPredicate = null)
		{
			using (System.IO.Stream stream = assembly.GetManifestResourceStream (path))
			{
				return this.LoadFile (stream, loadPredicate);
			}
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
			this.entries.Sort
				(
					delegate (Entry a, Entry b)
					{
						if (a.Priority == b.Priority)
						{
							return string.Compare (a.Name, b.Name);
						}
						else if (a.Priority < b.Priority)
						{
							return -1;
						}
						else
						{
							return 1;
						}
					}
				);
			
			ICSharpCode.SharpZipLib.Zip.ZipOutputStream zip = new ICSharpCode.SharpZipLib.Zip.ZipOutputStream (stream);

			zip.SetLevel (this.level);

			foreach (Entry entry in this.entries)
			{
				this.WriteEntry (zip, entry);
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
			this.AddEntry (name, data, System.DateTime.Now, true);
		}

		/// <summary>
		/// Adds a file entry to the ZIP file.
		/// </summary>
		/// <param name="name">The file entry name. It can include a relative path using
		/// exclusively forward slashes as separators.</param>
		/// <param name="data">The data.</param>
		/// <param name="compress">If set to <c>true</c>, the data will be compressed.</param>
		public void AddEntry(string name, byte[] data, bool compress)
		{
			this.AddEntry (name, data, System.DateTime.Now, compress);
		}

		/// <summary>
		/// Adds a file entry to the ZIP file.
		/// </summary>
		/// <param name="name">The file entry name. It can include a relative path using
		/// exclusively forward slashes as separators.</param>
		/// <param name="data">The data.</param>
		/// <param name="priority">The file entry priority.</param>
		public void AddEntry(string name, byte[] data, int priority)
		{
			this.AddEntry (name, data, System.DateTime.Now, priority, true);
		}

		/// <summary>
		/// Adds a file entry to the ZIP file.
		/// </summary>
		/// <param name="name">The file entry name. It can include a relative path using
		/// exclusively forward slashes as separators.</param>
		/// <param name="data">The data.</param>
		/// <param name="dateTime">The creation date time.</param>
		/// <param name="compress">If set to <c>true</c>, the data will be compressed.</param>
		public void AddEntry(string name, byte[] data, System.DateTime dateTime, bool compress)
		{
			this.AddEntry (name, data, dateTime, 1000000, compress);
		}

		/// <summary>
		/// Adds a file entry to the ZIP file.
		/// </summary>
		/// <param name="name">The file entry name. It can include a relative path using
		/// exclusively forward slashes as separators.</param>
		/// <param name="data">The data.</param>
		/// <param name="dateTime">The creation date time.</param>
		/// <param name="priority">The file entry priority.</param>
		/// <param name="compress">If set to <c>true</c>, the data will be compressed.</param>
		public void AddEntry(string name, byte[] data, System.DateTime dateTime, int priority, bool compress)
		{
			System.Diagnostics.Debug.Assert (string.IsNullOrEmpty (name) == false);
			System.Diagnostics.Debug.Assert (name.Contains ("\\") == false);
			System.Diagnostics.Debug.Assert (name.StartsWith ("/") == false);
			System.Diagnostics.Debug.Assert (name.EndsWith ("/") == false);

			this.AddDirectory (System.IO.Path.GetDirectoryName (name).Replace ('\\', '/'));
			
			this.entries.RemoveAll (delegate (Entry entry) { return (entry.Name == name); });
			this.entries.Add (new Entry (name, data, dateTime, priority, compress));
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
				this.entries.Add (new Entry (name, System.DateTime.Now, 0, 0, true));
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



		public static string DecompressTextFile(System.IO.Stream stream, System.Text.Encoding encoding = null)
		{
			if (encoding == null)
			{
				encoding = System.Text.Encoding.Default;
			}

			var zipFile = new Epsitec.Common.IO.ZipFile ();

			zipFile.LoadFile (stream);

			return encoding.GetString (zipFile.Entries.First ().Data);
		}

		public static string DecompressTextFile(System.Reflection.Assembly assembly, string resourcePath, System.Text.Encoding encoding = null)
		{
			if (encoding == null)
			{
				encoding = System.Text.Encoding.Default;
			}

			var zipFile = new Epsitec.Common.IO.ZipFile ();
			
			zipFile.LoadFileFromResources (assembly, resourcePath);
			
			return encoding.GetString (zipFile.Entries.First ().Data);
		}


		private static byte[] ReadEntry(ICSharpCode.SharpZipLib.Zip.ZipInputStream zip, ICSharpCode.SharpZipLib.Zip.ZipEntry entry)
		{
			byte[] data;
			
			if (entry.Size < 0)
			{
				int total = 0;
				List<byte[]> buffers = new List<byte[]> ();

				while (true)
				{
					byte[] readBuffer = new byte[64*1024];

					int read = ZipFile.ReadBytes (zip, readBuffer);

					if (read < readBuffer.Length)
					{
						byte[] bigBuffer = new byte[total+read];
						int offset = 0;
						foreach (var buffer in buffers)
						{
							buffer.CopyTo (bigBuffer, offset);
							offset += buffer.Length;
						}
						System.Array.Copy (readBuffer, 0, bigBuffer, offset, read);
						return bigBuffer;
					}

					buffers.Add (readBuffer);
					total += read;
				}
			}
			else
			{
				data = new byte[entry.Size];
				ZipFile.ReadBytes (zip, data);
			}

			return data;
		}

		private static int ReadBytes(ICSharpCode.SharpZipLib.Zip.ZipInputStream zip, byte[] data)
		{
			int size   = data.Length;
			int offset = 0;

			while (size > 0)
			{
				int length = zip.Read (data, offset, size);

				offset += length;
				size   -= length;

				if (length == 0)
				{
					break;
				}
			}

			return offset;
		}

		private void WriteEntry(ICSharpCode.SharpZipLib.Zip.ZipOutputStream zip, Entry entry)
		{
			ICSharpCode.SharpZipLib.Zip.ZipEntry e = new ICSharpCode.SharpZipLib.Zip.ZipEntry (entry.Name);

			if (entry.IsDirectory)
			{
				e.DateTime = entry.DateTime;
				
				zip.PutNextEntry (e);
			}
			else
			{
				e.DateTime          = entry.DateTime;
				e.Size              = entry.Data.Length;
				e.CompressionMethod = entry.IsCompressed ? ICSharpCode.SharpZipLib.Zip.CompressionMethod.Deflated : ICSharpCode.SharpZipLib.Zip.CompressionMethod.Stored;

				zip.PutNextEntry (e);
				zip.Write (entry.Data, 0, entry.Data.Length);
			}
		}

		#region Entry Structure

		/// <summary>
		/// The <c>Entry</c> structure maps a file name to its data and date/time
		/// stamp.
		/// </summary>
		public struct Entry
		{
			internal Entry(string fileName, byte[] data, System.DateTime date, int priority, bool isCompressed)
			{
				this.name = fileName;
				this.data = data;
				this.date = date;
				this.size = data == null ? 0 : data.Length;
				this.isCompressed = isCompressed;
				this.priority = priority;
			}

			internal Entry(string directoryName, System.DateTime date, long size, int priority, bool isCompressed)
			{
				this.name = directoryName;
				this.data = null;
				this.date = date;
				this.size = size;
				this.isCompressed = isCompressed;
				this.priority = priority;
			}

			/// <summary>
			/// Gets the full name of the entry.
			/// </summary>
			/// <value>The full name.</value>
			public string						Name
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
			public byte[]						Data
			{
				get
				{
					return this.data;
				}
			}

			/// <summary>
			/// Gets the size of the uncompressed data.
			/// </summary>
			/// <value>The size of the uncompressed data.</value>
			public long							Size
			{
				get
				{
					return this.size;
				}
			}

			/// <summary>
			/// Gets the date and time associated with the entry.
			/// </summary>
			/// <value>The date and time.</value>
			public System.DateTime				DateTime
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
			public bool							IsDirectory
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
			public bool							IsEmpty
			{
				get
				{
					return this.name == null;
				}
			}

			/// <summary>
			/// Gets or sets a value indicating whether this entry contains compressed data.
			/// </summary>
			/// <value>
			/// 	<c>true</c> if this entry contains compressed data; otherwise, <c>false</c>.
			/// </value>
			public bool							IsCompressed
			{
				get
				{
					return this.isCompressed;
				}
			}

			/// <summary>
			/// Gets or sets the priority for this entry. A small value represents a
			/// higher priority (it usually maps to the ZIP entry index).
			/// </summary>
			/// <value>The priority.</value>
			public int							Priority
			{
				get
				{
					return this.priority;
				}
			}

			/// <summary>
			/// The default empty entry.
			/// </summary>
			public static Entry Empty = new Entry ();

			private readonly string				name;
			private readonly byte[]				data;
			private readonly long				size;
			private readonly System.DateTime	date;
			private readonly bool				isCompressed;
			private readonly int				priority;
		}

		#endregion

		private int								level = 5;
		private readonly List<Entry>			entries;
		private string							loadFileName;
	}
}
