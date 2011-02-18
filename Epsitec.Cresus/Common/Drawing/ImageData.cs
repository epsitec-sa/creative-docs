//	Copyright © 2007-2008, OPaC bright ideas, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing.Platform;

using System.Collections.Generic;

namespace Epsitec.Common.Drawing
{
	public class ImageData : IQueueable, System.IDisposable
	{
		public ImageData(ImageManager engine, string path, string imageId, System.DateTime date)
		{
			this.engine = engine;
			this.imageFilePath = path;
			this.imageId = imageId;
			this.imageFileDate = date;
			this.fileFormat = null;
		}

		internal QueueStatus QueueStatus
		{
			get
			{
				if (this.discarded)
				{
					return QueueStatus.Discarded;
				}
				else if (this.workingCounter > 0)
				{
					return QueueStatus.Working;
				}
				else if (this.pendingCounter > 0)
				{
					return QueueStatus.Pending;
				}
				else
				{
					return QueueStatus.Inactive;
				}
			}
		}
		
		public string ImageFilePath
		{
			get
			{
				return this.imageFilePath;
			}
		}

		public string ImageId
		{
			get
			{
				return this.imageId;
			}
		}

		public string ImageFileName
		{
			get
			{
				return System.IO.Path.GetFileName (this.ImageFilePath);
			}
		}
		
		public long MemorySize
		{
			get
			{
				lock (this.exclusion)
				{
					long size = 0;

					if (this.thumbnail != null)
					{
						size += this.thumbnail.MemorySize;
					}
					if (this.sampleImage != null)
					{
						size += this.sampleImage.MemorySize;
					}
					if (this.compressedThumbnail != null)
					{
						size += this.compressedThumbnail.Length;
					}
					if (this.compressedSampleImage != null)
					{
						size += this.compressedSampleImage.Length;
					}

					return size;
				}
			}
		}

		public int SourceWidth
		{
			get
			{
				return this.width;
			}
		}

		public int SourceHeight
		{
			get
			{
				return this.height;
			}
		}

		public void Discard()
		{
			this.discarded = true;
		}

		public BitmapFileFormat GetFileFormat()
		{
			if (this.fileFormat == null)
			{
				this.LoadImageData ();
			}
			
			return this.fileFormat;
		}

		public NativeBitmap GetAsyncThumbnail()
		{
			NativeBitmap thumbnail;

			thumbnail = this.Thumbnail;

			if (thumbnail == null)
			{
				this.engine.Queue (this, this.CreateThumbnail);
			}

			return thumbnail;
		}

		public NativeBitmap GetAsyncSampleImage()
		{
			NativeBitmap sampleImage;

			sampleImage = this.SampleImage;

			if (sampleImage == null)
			{
				this.engine.Queue (this, this.CreateThumbnail);
			}

			return sampleImage;
		}

		public NativeBitmap GetThumbnail()
		{
			NativeBitmap thumbnail;

			thumbnail = this.Thumbnail;

			if (thumbnail == null)
			{
				this.SyncCreateThumbnail ();
				thumbnail = this.Thumbnail;
			}

			return thumbnail;
		}

		public NativeBitmap GetSampleImage()
		{
			NativeBitmap sampleImage;

			sampleImage = this.SampleImage;

			if (sampleImage == null)
			{
				this.SyncCreateThumbnail ();
				sampleImage = this.SampleImage;
			}

			return sampleImage;
		}

		public NativeBitmap Thumbnail
		{
			get
			{
				lock (this.exclusion)
				{
					this.DecompressThumbnail ();
					
					if (this.thumbnail != null)
					{
						this.thumbnailTimestamp = System.DateTime.Now.Ticks;
					}

					return this.thumbnail;
				}
			}
			internal set
			{
				lock (this.exclusion)
				{
					if (this.thumbnail != null)
					{
						this.engine.RemoveMemoryPressure (ImageData.GetMemorySize (this.thumbnail));
						this.thumbnail.Information = this.thumbnail.Information + "/dead " + System.Environment.TickCount;
					}

					this.thumbnail = value;

					if (this.thumbnail != null)
					{
						this.thumbnail.Information = "THUMB:" + this.imageFilePath + "/" + System.Environment.TickCount;
						this.engine.AddMemoryPressure (ImageData.GetMemorySize (this.thumbnail));
						this.thumbnailTimestamp = System.DateTime.Now.Ticks;
					}
					else
					{
						this.thumbnailTimestamp = 0;
					}
				}
			}
		}

		public NativeBitmap SampleImage
		{
			get
			{
				lock (this.exclusion)
				{
					this.DecompressSampleImage ();

					if (this.sampleImage != null)
					{
						this.sampleImageTimestamp = System.DateTime.Now.Ticks;
					}

					return this.sampleImage;
				}
			}
			internal set
			{
				lock (this.exclusion)
				{
					if (this.sampleImage != null)
					{
						this.engine.RemoveMemoryPressure (ImageData.GetMemorySize (this.sampleImage));
						this.sampleImage.Information = this.sampleImage.Information + "/dead " + System.Environment.TickCount;
					}

					this.sampleImage = value;

					if (this.sampleImage != null)
					{
						this.sampleImage.Information = "SAMPLE:" + this.imageFilePath + "/" + System.Environment.TickCount;
						this.engine.AddMemoryPressure (ImageData.GetMemorySize (this.sampleImage));
						this.sampleImageTimestamp = System.DateTime.Now.Ticks;
					}
					else
					{
						this.sampleImageTimestamp = 0;
					}
				}
			}
		}

		private static int GetMemorySize(NativeBitmap image)
		{
			int dx = image.Pitch;
			int dy = image.Height;

			return dx * dy;
		}

		internal long GetCacheTimestamp(CacheClearing mode)
		{
			lock (this.exclusion)
			{
				switch (mode)
				{
					case CacheClearing.ReleaseLargeBuffers:
						return this.imageDataTimestamp;

					case CacheClearing.ReleaseSmallBuffers:
						return this.thumbnailTimestamp;
					
					case CacheClearing.ReleaseMediumBuffers:
						return this.sampleImageTimestamp;
					
					default:
						return 0;
				}
			}
		}

		internal void ClearCache(CacheClearing mode)
		{
			lock (this.exclusion)
			{
				if (this.QueueStatus == QueueStatus.Working)
				{
					return;
				}

				NativeBitmap image = null;

				switch (mode)
				{
					case CacheClearing.ReleaseSmallBuffers:
						image = this.thumbnail;
						
						this.CompressThumbnail ();
						this.Thumbnail = null;
						
						if (image != null)
						{
							image.Dispose ();
						}
						break;
					
					case CacheClearing.ReleaseMediumBuffers:
						image = this.sampleImage;

						this.CompressSampleImage ();
						this.SampleImage = null;
						
						if (image != null)
						{
							image.Dispose ();
						}
						break;
					
					case CacheClearing.ReleaseLargeBuffers:
						this.ReleaseImageData ();
						break;
				}
			}
		}

		private void DecompressThumbnail()
		{
			if ((this.compressedThumbnail != null) &&
				(this.thumbnail == null))
			{
				this.Thumbnail = NativeBitmap.Load (this.compressedThumbnail);
			}
		}

		private void DecompressSampleImage()
		{
			if ((this.compressedSampleImage != null) &&
				(this.sampleImage == null))
			{
				this.SampleImage = NativeBitmap.Load (this.compressedSampleImage);
			}
		}
		
		private void CompressThumbnail()
		{
			if ((this.compressedThumbnail == null) &&
				(this.thumbnail != null))
			{
				this.compressedThumbnail = this.thumbnail.SaveToMemory (new BitmapFileFormat () { Type = BitmapFileType.Jpeg, Quality = 75 });

				if (this.compressedThumbnail != null)
				{
					this.engine.AddMemoryPressure (this.compressedThumbnail.Length);
				}
			}
		}

		private void CompressSampleImage()
		{
			if ((this.compressedSampleImage == null) &&
				(this.sampleImage != null))
			{
				this.compressedSampleImage = this.sampleImage.SaveToMemory (new BitmapFileFormat () { Type = BitmapFileType.Jpeg, Quality = 75 });

				if (this.compressedSampleImage != null)
				{
					this.engine.AddMemoryPressure (this.compressedSampleImage.Length);
				}
			}
		}

		private void ReleaseImageData()
		{
			if (this.fullImage != null)
			{
				this.engine.RemoveMemoryPressure (this.fullImage.ByteCount);
				this.fullImage.Dispose ();
				this.fullImage = null;
				this.imageDataTimestamp = 0;
			}
		}

		private void SyncCreateThumbnail()
		{
			lock (this.exclusion)
			{
				this.CreateThumbnail ();
			}
		}

		private void CreateThumbnail()
		{
			if (string.IsNullOrEmpty (this.imageFilePath))
			{
				return;
			}
			
			if (ImageManager.ImageStore == null)
			{
				return;
			}

			try
			{
				NativeBitmap sampleImage = this.SampleImage;
				
				if (sampleImage == null)
				{
					byte[] cachedSampleImageData = ImageManager.ImageStore.LoadImageData (this.imageId, this.imageFileDate, this.engine.SampleImageSize, out this.width, out this.height);

					System.Diagnostics.Debug.Assert (this.compressedSampleImage == null);

					if ((cachedSampleImageData == null) ||
						(cachedSampleImageData.Length == 0))
					{
						this.LoadImageData ();

						if ((this.fullImage == null) ||
							(this.fullImage.IsValid == false))
						{
							return;
						}

						this.fullImage.Information = "FULL:" + this.imageFilePath + "/" + System.Environment.TickCount;
						
						this.width  = this.fullImage.Width;
						this.height	= this.fullImage.Height;
						
						int dx;
						int dy;

						this.GetReducedImageSize (this.engine.SampleImageSize, out dx, out dy);

						sampleImage = this.fullImage.Rescale (dx, dy);

						if (sampleImage != null)
						{
							if (ImageManager.ImageStore != null)
							{
								this.compressedSampleImage = ImageManager.ImageStore.SaveImageData (this.imageId, this.imageFileDate, this.engine.SampleImageSize, this.width, this.height, sampleImage);
							}
							
							if (this.compressedSampleImage != null)
							{
								this.engine.AddMemoryPressure (this.compressedSampleImage.Length);
							}
						}
					}
					else
					{
						sampleImage = NativeBitmap.Load (cachedSampleImageData);
						this.compressedSampleImage = cachedSampleImageData;

						if (this.compressedSampleImage != null)
						{
							this.engine.AddMemoryPressure (this.compressedSampleImage.Length);
						}
					}

					if (sampleImage == null)
					{
						return;
					}

					this.SampleImage = sampleImage;
				}

				NativeBitmap thumbnail = this.Thumbnail;

				if (thumbnail == null)
				{
					byte[] cachedThumbnailData = ImageManager.ImageStore.LoadImageData (this.imageId, this.imageFileDate, this.engine.ThumbnailSize, out this.width, out this.height);

					System.Diagnostics.Debug.Assert (this.compressedThumbnail == null);
					
					if ((cachedThumbnailData == null) ||
						(cachedThumbnailData.Length == 0))
					{
						thumbnail = sampleImage.MakeThumbnail (this.engine.ThumbnailSize);

						if (thumbnail != null)
						{
							this.compressedThumbnail = ImageManager.ImageStore.SaveImageData (this.imageId, this.imageFileDate, this.engine.ThumbnailSize, this.width, this.height, thumbnail);
							
							if (this.compressedThumbnail != null)
							{
								this.engine.AddMemoryPressure (this.compressedThumbnail.Length);
							}
						}
					}
					else
					{
						thumbnail = NativeBitmap.Load (cachedThumbnailData);
						
						this.compressedThumbnail = cachedThumbnailData;
						
						if (this.compressedThumbnail != null)
						{
							this.engine.AddMemoryPressure (this.compressedThumbnail.Length);
						}
					}

					this.Thumbnail = thumbnail;
				}

				this.NotifyChanged (new ImageDataEventArgs ());
			}
			catch
			{
			}
		}

		private void GetReducedImageSize(int sampleSize, out int dx, out int dy)
		{
			dx = this.width;
			dy = this.height;

			double totalPixels = System.Math.Sqrt (dx * dy);
			double smallPixels = sampleSize;

			if (totalPixels > smallPixels)
			{
				double scale = smallPixels / totalPixels;

				dx = System.Math.Max (1, (int) System.Math.Round (dx * scale));
				dy = System.Math.Max (1, (int) System.Math.Round (dy * scale));
			}
		}

		private void LoadImageData()
		{
			lock (this.exclusion)
			{
				byte[] data = this.engine.ReadAllBytes (this.imageFilePath);
				NativeBitmap image = NativeBitmap.Load (data, this.imageFilePath);

				if ((image != null) &&
					(image.IsValid))
				{
					this.fullImage  = image;
					this.fileFormat = image.FileFormat;
					this.imageDataTimestamp = System.DateTime.Now.Ticks;
					this.engine.AddMemoryPressure (this.fullImage.ByteCount);
				}
				else
				{
					this.fullImage  = null;
					this.fileFormat = new BitmapFileFormat ();
					this.imageDataTimestamp = 0;
				}
			}
		}

		private void NotifyChanged(ImageDataEventArgs e)
		{
			ImageManager.AsyncInvoke (
				delegate ()
				{
					this.OnChanged (e);
				});
		}

		private void OnChanged(ImageDataEventArgs e)
		{
			if (this.Changed != null)
			{
				this.Changed (this, e);
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			this.ReleaseImageData ();

			NativeBitmap image;

			image = this.thumbnail;

			if (image != null)
			{
				this.Thumbnail = null;
				image.Dispose ();
			}

			image = this.sampleImage;

			if (image != null)
			{
				this.SampleImage = null;
				image.Dispose ();
			}

			if (this.compressedThumbnail != null)
			{
				this.engine.RemoveMemoryPressure (this.compressedThumbnail.Length);
				this.compressedThumbnail = null;
			}

			if (this.compressedSampleImage != null)
			{
				this.engine.RemoveMemoryPressure (this.compressedSampleImage.Length);
				this.compressedSampleImage = null;
			}
		}

		#endregion

		#region IQueueable Members

		QueueStatus IQueueable.QueueStatus
		{
			get
			{
				return this.QueueStatus;
			}
		}

		void IQueueable.ChangePendingCounter(int change)
		{
			QueueStatus oldStatus = this.QueueStatus;

			if (change > 0)
			{
				if (System.Threading.Interlocked.Increment (ref this.pendingCounter) > 1)
				{
					System.Diagnostics.Debug.WriteLine ("Should not happen: counter > 1");
				}
			}
			else if (change < 0)
			{
				System.Threading.Interlocked.Decrement (ref this.pendingCounter);
			}

			QueueStatus newStatus = this.QueueStatus;

			if (oldStatus != newStatus)
			{
				this.OnQueueStatusChanged (oldStatus, newStatus);
			}
		}

		void IQueueable.ChangeWorkingCounter(int change)
		{
			QueueStatus oldStatus = this.QueueStatus;

			if (change > 0)
			{
				if (System.Threading.Interlocked.Increment (ref this.workingCounter) > 1)
				{
					System.Diagnostics.Debug.WriteLine ("Should not happen: counter > 1");
				}
			}
			else if (change < 0)
			{
				System.Threading.Interlocked.Decrement (ref this.workingCounter);
			}

			QueueStatus newStatus = this.QueueStatus;

			if (oldStatus != newStatus)
			{
				this.OnQueueStatusChanged (oldStatus, newStatus);
			}
		}

		#endregion

		private void OnQueueStatusChanged(QueueStatus oldStatus, QueueStatus newStatus)
		{
			if ((oldStatus == QueueStatus.Working) &&
				(newStatus == QueueStatus.Pending))
			{
				System.Diagnostics.Debug.WriteLine ("Should not happen: working->pending");
			}
			if ((oldStatus == QueueStatus.Inactive) &&
				(newStatus == QueueStatus.Working))
			{
				System.Diagnostics.Debug.WriteLine ("Should not happen: inactive->working");
			}
		}

		public event System.EventHandler<ImageDataEventArgs> Changed;

		private readonly object exclusion = new object ();
		private string imageFilePath;
		private string imageId;
		private System.DateTime imageFileDate;
		private ImageManager engine;
		
		private int pendingCounter;
		private int workingCounter;

		private long imageDataTimestamp;
		private long thumbnailTimestamp;
		private long sampleImageTimestamp;

		private NativeBitmap thumbnail;
		private NativeBitmap sampleImage;
		private NativeBitmap fullImage;
		private BitmapFileFormat fileFormat;
		private int width;
		private int height;
		private bool discarded;

		private byte[] compressedThumbnail;
		private byte[] compressedSampleImage;
	}
}
