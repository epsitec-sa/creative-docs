//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Support
{
	internal sealed class FolderItemIconCache
	{
		public FolderItemIconCache()
		{
		}

		public long Add(Drawing.Image image)
		{
			long id = -1;
			
			byte[] sourceData = image.BitmapImage.GetRawBitmapBytes ();
			long   sourceCrc  = IO.Checksum.ComputeCrc32 (delegate (IO.IChecksum checksum) { checksum.Update (sourceData); });
			
			foreach (Item item in this.images.Values)
			{
				if (item.crc == sourceCrc)
				{
					byte[] cacheData = item.image.BitmapImage.GetRawBitmapBytes ();

					if (cacheData.Length == sourceData.Length)
					{
						bool ok = true;
						
						for (int i = 0; i < sourceData.Length; i++)
						{
							if (cacheData[i] != sourceData[i])
							{
								ok = false;
								break;
							}
						}

						if (ok)
						{
							id = item.id;
							break;
						}
					}
				}
			}

			lock (this.images)
			{
				if (id < 0)
				{
					Item item = new Item ();

					item.image = image;
					item.id = this.nextIndex++;
					item.crc   = sourceCrc;
					item.count = 1;

					this.images[item.id] = item;

					return item.id;
				}
				else
				{
					Item item = this.images[id];

					item.count++;

					this.images[item.id] = item;

					return item.id;
				}
			}
		}

		public void Release(long id)
		{
			System.Diagnostics.Debug.Assert (id >= 0);
			System.Diagnostics.Debug.Assert (this.images.ContainsKey (id));
			
			lock (this.images)
			{
				Item item = this.images[id];

				item.count--;

				if (item.count == 0)
				{
					this.images.Remove (id);
				}
				else
				{
					this.images[id] = item;
				}
			}
		}

		public Drawing.Image Resolve(long id)
		{
			lock (this.images)
			{
				Item item;
				this.images.TryGetValue (id, out item);
				return item.image;
			}
		}

		struct Item
		{
			public Drawing.Image image;
			public long crc;
			public long id;
			public int count;
		}


		public static FolderItemIconCache Instance
		{
			get
			{
				lock (FolderItemIconCache.exclusion)
				{
					if (FolderItemIconCache.instance == null)
					{
						FolderItemIconCache.instance = new FolderItemIconCache ();
					}
					
					return FolderItemIconCache.instance;
				}
			}
		}

		private static object exclusion = new object ();
		private static FolderItemIconCache instance;

		private Dictionary<long, Item> images = new Dictionary<long,Item> ();
		private long nextIndex;
	}
}
