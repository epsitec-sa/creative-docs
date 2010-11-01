//	Copyright © 2007-2008, OPaC bright ideas, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing.Platform;

using System.Collections.Generic;

namespace Epsitec.Common.Drawing
{
	/// <summary>
	/// The <c>ImageStore</c> class implements a persistent image cache, where
	/// images are uniquely identified by their source path, last modification
	/// date and total pixel count.
	/// </summary>
	public class ImageStore
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ImageStore"/> class.
		/// </summary>
		public ImageStore()
		{
			this.mutex = new System.Threading.Mutex (false, ImageStore.GetMutexName ());
			this.records = new List<ImageRecord> ();
			this.cacheDir = System.IO.Path.Combine (System.Environment.GetFolderPath (System.Environment.SpecialFolder.LocalApplicationData), @"OPaC bright ideas\ImageManager.Cache");

			if (!System.IO.Directory.Exists (this.cacheDir))
			{
				System.IO.Directory.CreateDirectory (this.cacheDir);
			}
		}

		/// <summary>
		/// Loads the image data from the cache, if it exists.
		/// </summary>
		/// <param name="path">The image source path.</param>
		/// <param name="date">The image modification date.</param>
		/// <param name="pixels">The image pixel count.</param>
		/// <param name="sourceWidth">Width of the original source image.</param>
		/// <param name="sourceHeight">Height of the original source image.</param>
		/// <returns>
		/// The image data (JPEG encoded) or <c>null</c> if it cannot be found
		/// in the cache.
		/// </returns>
		public byte[] LoadImageData(string path, System.DateTime date, int pixels, out int sourceWidth, out int sourceHeight)
		{
			string cacheFilePath;
			int cachePixels;

			if (this.FindCachedImage (path, date, pixels, out cacheFilePath, out cachePixels, out sourceWidth, out sourceHeight))
			{
				if (cachePixels == pixels)
				{
					try
					{
						if (System.IO.File.Exists (cacheFilePath))
						{
							return System.IO.File.ReadAllBytes (cacheFilePath);
						}
					}
					catch (System.IO.IOException)
					{
						return null;
					}
				}
			}
			
			return null;
		}
		
		/// <summary>
		/// Saves the image data to the cache.
		/// </summary>
		/// <param name="path">The image source path.</param>
		/// <param name="date">The image modification date.</param>
		/// <param name="pixels">The image pixel count.</param>
		/// <param name="sourceWidth">Width of the original source image.</param>
		/// <param name="sourceHeight">Height of the original source image.</param>
		/// <param name="data">The image data (JPEG encoded).</param>
		public void SaveImageData(string path, System.DateTime date, int pixels, int sourceWidth, int sourceHeight, byte[] data, string extension)
		{
			string hash = ImageStore.GetPathHash (path);

			lock (this.records)
			{
				this.LoadRecords ();

				foreach (ImageRecord record in this.records)
				{
					if ((record.PathHash == hash) &&
						(record.Date == date) &&
						(record.Pixels == pixels))
					{
						string cacheFilePath = System.IO.Path.Combine (this.cacheDir, record.CacheFileName);

						this.mutex.WaitOne ();

						try
						{
							System.IO.File.WriteAllBytes (cacheFilePath, data);
						}
						finally
						{
							this.mutex.ReleaseMutex ();
						}

						return;
					}
				}

				this.UpdateRecords (
					delegate ()
					{
						string cacheFilePath;
						string cacheFileName;

						do
						{
							cacheFileName = System.IO.Path.GetFileNameWithoutExtension (System.IO.Path.GetRandomFileName ()) + extension;
							cacheFilePath = System.IO.Path.Combine (this.cacheDir, cacheFileName);
						}
						while (System.IO.File.Exists (cacheFilePath));

						this.records.Add (new ImageRecord (hash, date, cacheFileName, pixels, sourceWidth, sourceHeight));
						System.IO.File.WriteAllBytes (cacheFilePath, data);

						return true;
					});
			}
		}

		public byte[] SaveImageData(string path, System.DateTime date, int pixels, int sourceWidth, int sourceHeight, NativeBitmap image)
		{
			NativeBitmap temp = null;
			string extension = null;
			byte[] memory = null;

			if ((image.BitsPerPixel > 8) &&
				(image.IsTransparent == false))
			{
				memory = image.SaveToMemory (new BitmapFileFormat () { Type = BitmapFileType.Jpeg, Quality = 90 });
				extension = ".jpg";
			}
			else if (image.BitsPerPixel <= 8)
			{
				memory = image.SaveToMemory (new BitmapFileFormat () { Type = BitmapFileType.Gif });
				extension = ".gif";
			}
			
			if ((memory == null) &&
				(image.BitsPerPixel == 32))
			{
				memory = image.SaveToMemory (new BitmapFileFormat () { Type = BitmapFileType.Png });
				extension = ".png";
			}
			if (memory == null)
			{
				temp = image.ConvertToRgb24 ();
				memory = image.SaveToMemory (new BitmapFileFormat () { Type = BitmapFileType.Jpeg, Quality = 90 });
				extension = ".jpg";
			}

			this.SaveImageData (path, date, pixels, sourceWidth, sourceHeight, memory, extension);

			if (temp != null)
			{
				temp.Dispose ();
			}

			return memory;
		}

		/// <summary>
		/// Scrubs the cache, removing entries which are to old and trim the total
		/// size in order not to exceed the default maximum disk size.
		/// </summary>
		public void ScrubCache()
		{
			this.ScrubCache (System.TimeSpan.FromDays (ImageStore.maximumDiskCacheAgeInDays), ImageStore.maximumDiskCacheSize);
		}

		/// <summary>
		/// Scrubs the cache, removing entries which are to old and trim the total
		/// size in order not to exceed the specified maximum disk size.
		/// </summary>
		/// <param name="maximumAge">The maximum allowed image age.</param>
		/// <param name="maximumSize">The maximum allowed disk size.</param>
		public void ScrubCache(System.TimeSpan maximumAge, long maximumSize)
		{
			int age = (int) (maximumAge.TotalHours);

			lock (this.records)
			{
				this.UpdateRecords (
					delegate ()
					{
						return this.RemoveAll (
							delegate (FileRecord file)
							{
								if (file.AgeInHours > age)
								{
									return true;
								}
								else if (this.totalCacheLength > maximumSize)
								{
									return true;
								}
								else
								{
									return false;
								}
							});
					});
			}
		}

		internal void Dispose()
		{
		}

		private bool FindCachedImage(string path, System.DateTime date, int pixels, out string cacheFilePath, out int cachePixels, out int sourceWidth, out int sourceHeight)
		{
			string hash = ImageStore.GetPathHash (path);

			cacheFilePath = null;
			cachePixels = 0;
			sourceWidth = 0;
			sourceHeight = 0;

			lock (this.records)
			{
				this.LoadRecords ();

				int pixelDelta = int.MaxValue;

				foreach (ImageRecord record in this.records)
				{
					if ((record.PathHash == hash) &&
						(record.Date == date))
					{
						int delta = System.Math.Abs (record.Pixels - pixels);

						if (delta < pixelDelta)
						{
							cacheFilePath = record.CacheFileName;
							cachePixels = record.Pixels;
							sourceWidth = record.SourceWidth;
							sourceHeight = record.SourceHeight;
							pixelDelta = delta;
						}
					}
				}
			}

			if (cacheFilePath == null)
			{
				return false;
			}
			else
			{
				cacheFilePath = System.IO.Path.Combine (this.cacheDir, cacheFilePath);
				return true;
			}
		}

		private static string GetMutexName()
		{
			return string.Concat (@"Global\OPaC.ImageManager.CacheLock.", System.Environment.UserName);
		}

		private static string GetPathHash(string path)
		{
			System.Text.StringBuilder output = new System.Text.StringBuilder ();

			foreach (char c in path)
			{
				switch (c)
				{
					case '\\':
						output.Append ('/');
						break;
					
					case ';':
					case ',':
						output.Append ('-');
						break;
					
					default:
						output.Append (c);
						break;
				}
			}

			return output.ToString ();
		}

		private string GetCacheJournalPath()
		{
			return System.IO.Path.Combine (this.cacheDir, "cache.data");
		}

		private void LoadRecords()
		{
			if (this.records.Count == 0)
			{
				if (System.IO.File.Exists (this.GetCacheJournalPath ()))
				{
					this.mutex.WaitOne ();

					string[] source;

					try
					{
						source = System.IO.File.ReadAllLines (this.GetCacheJournalPath ());
					}
					finally
					{
						this.mutex.ReleaseMutex ();
					}

					this.FillRecords (source);
				}
			}
		}

		private void FillRecords(string[] source)
		{
			this.records.Clear ();

			foreach (string item in source)
			{
				try
				{
					ImageRecord record = new ImageRecord (item);
					this.records.Add (record);
				}
				catch
				{
				}
			}
		}

		private void UpdateRecords(ActionCallback callback)
		{
			this.mutex.WaitOne ();

			try
			{
				if (System.IO.File.Exists (this.GetCacheJournalPath ()))
				{
					this.FillRecords (System.IO.File.ReadAllLines (this.GetCacheJournalPath ()));
				}

				if (callback ())
				{
					List<string> source = new List<string> ();

					foreach (ImageRecord record in this.records)
					{
						source.Add (record.ToString ());
					}

					System.IO.File.WriteAllLines (this.GetCacheJournalPath (), source.ToArray ());
				}
			}
			finally
			{
				this.mutex.ReleaseMutex ();
			}
		}

		private bool RemoveAll(System.Predicate<FileRecord> condition)
		{
			List<FileRecord> files = this.FindAllCacheFiles ();
			Dictionary<string, ImageRecord> images = new Dictionary<string, ImageRecord> ();

			foreach (ImageRecord image in this.records)
			{
				images[image.CacheFileName] = image;
			}
			
			int changes = 0;

			foreach (FileRecord file in files)
			{
				images.Remove (file.Name);

				if (condition (file))
				{
					this.records.RemoveAll (
						delegate (ImageRecord image)
						{
							if (image.CacheFileName == file.Name)
							{
								changes++;
								return true;
							}
							else
							{
								return false;
							}
						});

					try
					{
						this.totalCacheLength -= file.Length;
						System.IO.File.Delete (System.IO.Path.Combine (this.cacheDir, file.Name));
					}
					catch
					{
					}
				}
			}

			if (images.Count > 0)
			{
				foreach (string name in images.Keys)
				{
					int index = this.records.FindIndex (
						delegate (ImageRecord image)
						{
							if (image.CacheFileName == name)
							{
								return true;
							}
							else
							{
								return false;
							}
						});

					if (index >= 0)
					{
						this.records.RemoveAt (index);
						changes++;
					}
				}
			}

			return changes > 0;
		}

		private List<FileRecord> FindAllCacheFiles()
		{
			List<FileRecord> files = new List<FileRecord> ();
			long nowTicks = System.DateTime.UtcNow.Ticks;
			long totalLength = 0;

			foreach (string name in System.IO.Directory.GetFiles (this.cacheDir, "*.jpg"))
			{
				System.IO.FileInfo file = new System.IO.FileInfo (name);

				totalLength += file.Length;
				
				files.Add (new FileRecord (file, nowTicks));
			}

			files.Sort ();

			this.totalCacheLength = totalLength;
			
			return files;
		}

		#region FileRecord Structure

		private struct FileRecord : System.IComparable<FileRecord>
		{
			public FileRecord(System.IO.FileInfo info, long nowTicks)
			{
				this.name = System.IO.Path.GetFileName (info.FullName);
				this.length = info.Length;
				this.age = (int) ((nowTicks - info.LastAccessTimeUtc.Ticks) / (10000000L));
			}

			public string Name
			{
				get
				{
					return this.name;
				}
			}

			public long Length
			{
				get
				{
					return this.length;
				}
			}

			public int AgeInHours
			{
				get
				{
					return this.age / 3600;
				}
			}

			#region IComparable<FileRecord> Members

			public int CompareTo(FileRecord other)
			{
				if (this.age > other.age)
				{
					return -1;
				}
				else if (this.age == other.age)
				{
					return 0;
				}
				else
				{
					return 1;
				}
			}

			#endregion
			
			private string name;
			private long length;
			private int age;
		}

		#endregion

		#region ImageRecord Structure

		private struct ImageRecord
		{
			public ImageRecord(string pathHash, System.DateTime date, string cacheFileName, int pixels, int sourceWidth, int sourceHeight)
			{
				this.pathHash = pathHash;
				this.date = date;
				this.cacheFileName = cacheFileName;
				this.pixels = pixels;
				this.extra = null;
				this.sourceWidth = sourceWidth;
				this.sourceHeight = sourceHeight;
			}

			public ImageRecord(string source)
			{
				string[] values = source.Split (';');

				this.pathHash = values[0];
				this.date = new System.DateTime (long.Parse (values[1], System.Globalization.CultureInfo.InvariantCulture), System.DateTimeKind.Utc);
				this.cacheFileName = values[2];
				this.pixels = int.Parse (values[3], System.Globalization.CultureInfo.InvariantCulture);
				this.sourceWidth = int.Parse (values[4], System.Globalization.CultureInfo.InvariantCulture);
				this.sourceHeight = int.Parse (values[5], System.Globalization.CultureInfo.InvariantCulture);
				this.extra = values.Length > 6 ? string.Join (";", values, 4, values.Length-4) : null;
			}


			public string PathHash
			{
				get
				{
					return this.pathHash;
				}
			}

			public System.DateTime Date
			{
				get
				{
					return this.date;
				}
			}

			public string CacheFileName
			{
				get
				{
					return this.cacheFileName;
				}
			}

			public int SourceWidth
			{
				get
				{
					return this.sourceWidth;
				}
			}

			public int SourceHeight
			{
				get
				{
					return this.sourceHeight;
				}
			}

			public int Pixels
			{
				get
				{
					return this.pixels;
				}
			}
			
			public override string ToString()
			{
				string[] values = new string[6];
				
				values[0] = this.pathHash;
				values[1] = this.date.Ticks.ToString (System.Globalization.CultureInfo.InvariantCulture);
				values[2] = this.cacheFileName;
				values[3] = this.pixels.ToString (System.Globalization.CultureInfo.InvariantCulture);
				values[4] = this.sourceWidth.ToString (System.Globalization.CultureInfo.InvariantCulture);
				values[5] = this.sourceHeight.ToString (System.Globalization.CultureInfo.InvariantCulture);

				if (this.extra == null)
				{
					return string.Join (";", values);
				}
				else
				{
					return string.Concat (string.Join (";", values), ";", this.extra);
				}
			}


			private string pathHash;
			private System.DateTime date;
			private string cacheFileName;
			private int pixels;
			private int sourceWidth;
			private int sourceHeight;
			private string extra;
		}

		#endregion

		private delegate bool ActionCallback();

		private const int maximumDiskCacheAgeInDays = 30;
		private const long maximumDiskCacheSize = 50*1024*1024L;

		private string cacheDir;
		private System.Threading.Mutex mutex;
		private List<ImageRecord> records;
		private long totalCacheLength;
	}
}
