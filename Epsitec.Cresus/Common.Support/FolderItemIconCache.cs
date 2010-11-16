//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>FolderItemIconCache</c> is used to store and retrieve small images.
	/// When an image is added to the cache, duplicates are avoided (images with
	/// same pixels will produce the same id).
	/// </summary>
	internal sealed class FolderItemIconCache
	{
		private FolderItemIconCache()
		{
		}

		/// <summary>
		/// Adds the specified image to the cache. This internally increments
		/// a counter associated with the image; every call to <c>Add</c>
		/// should have a matching call to <c>Release</c>.
		/// </summary>
		/// <param name="image">The image.</param>
		/// <returns>The id associed with the image.</returns>
		public long Add(Drawing.Image image)
		{
			long id = -1;

			//	Beware: the GC can release any Item at any time asynchronously by
			//	calling Release, so be very cautious in this method !
			
			byte[] sourceData = image.BitmapImage.GetRawBitmapBytes ();
			int    sourceCrc  = IO.Checksum.ComputeCrc32 (delegate (IO.IChecksum checksum) { checksum.Update (sourceData); });

			Item[] items;
			Item matchingItem = new Item ();
			
			lock (this.images)
			{
				items = new Item[this.images.Count];
				this.images.Values.CopyTo (items, 0);
			}
			
			for (int j = 0; j < items.Length; j++)
			{
				Item item = items[j];
				
				if (item.crc == sourceCrc)
				{
					byte[] cacheData = item.image.BitmapImage.GetRawBitmapBytes ();

					if ((cacheData != null) &&
						(cacheData.Length == sourceData.Length))
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
							id           = item.id;
							matchingItem = item;
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
					item.id    = this.nextIndex++;
					item.crc   = sourceCrc;
					item.count = 1;

					this.images[item.id] = item;

					return item.id;
				}
				else
				{
					Item item;

					if (this.images.TryGetValue (id, out item))
					{
						item.count++;
					}
					else
					{
						item       = matchingItem;
						item.count = 1;
					}

					this.images[item.id] = item;

					return item.id;
				}
			}
		}

		/// <summary>
		/// Releases the specified image based on its id.
		/// </summary>
		/// <param name="id">The image id.</param>
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

		/// <summary>
		/// Resolves the specified image id and returns the corresponding
		/// image.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns>The image or <c>null</c> if the id cannot be resolved.</returns>
		public Drawing.Image Resolve(long id)
		{
			lock (this.images)
			{
				Item item;
				this.images.TryGetValue (id, out item);
				return item.image;
			}
		}

		#region Private Item Structure
		
		private struct Item
		{
			public Drawing.Image image;
			public int crc;
			public long id;
			public int count;
		}

		#endregion

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
		private long nextIndex = (System.DateTime.UtcNow.Ticks - 632935616148146251)/10000;	//	>0 and time [ms] dependent to avoid accidental reuse of ids
	}
}
