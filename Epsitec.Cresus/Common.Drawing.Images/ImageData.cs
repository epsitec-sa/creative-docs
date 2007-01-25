//	Copyright © 2007, OPaC bright ideas, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

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
		}

		internal QueueStatus QueueStatus
		{
			get
			{
				if (this.workingCounter > 0)
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
					if (this.imageData != null)
					{
						size += this.imageData.Length;
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

		public Opac.FreeImage.FileFormat GetFileFormat()
		{
			if (this.fileFormat == Opac.FreeImage.FileFormat.Unknown)
			{
				this.LoadImageData ();
			}
			
			return this.fileFormat;
		}
		
		public Opac.FreeImage.Image GetAsyncThumbnail()
		{
			Opac.FreeImage.Image thumbnail;

			thumbnail = this.Thumbnail;

			if (thumbnail == null)
			{
				this.engine.Queue (this, this.CreateThumbnail);
			}

			return thumbnail;
		}

		public Opac.FreeImage.Image GetAsyncSampleImage()
		{
			Opac.FreeImage.Image sampleImage;

			sampleImage = this.SampleImage;

			if (sampleImage == null)
			{
				this.engine.Queue (this, this.CreateThumbnail);
			}

			return sampleImage;
		}

		public Opac.FreeImage.Image GetThumbnail()
		{
			Opac.FreeImage.Image thumbnail;

			thumbnail = this.Thumbnail;

			if (thumbnail == null)
			{
				this.SyncCreateThumbnail ();
				thumbnail = this.Thumbnail;
			}

			return thumbnail;
		}

		public Opac.FreeImage.Image GetSampleImage()
		{
			Opac.FreeImage.Image sampleImage;

			sampleImage = this.SampleImage;

			if (sampleImage == null)
			{
				this.SyncCreateThumbnail ();
				sampleImage = this.SampleImage;
			}

			return sampleImage;
		}

		internal Opac.FreeImage.Image Thumbnail
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
			set
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

		internal Opac.FreeImage.Image SampleImage
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
			set
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

		private static int GetMemorySize(Opac.FreeImage.Image image)
		{
			int dx = image.GetPitch ();
			int dy = image.GetHeight ();

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

				Opac.FreeImage.Image image = null;

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
				using (Opac.FreeImage.Memory memory = new Opac.FreeImage.Memory (this.compressedThumbnail))
				{
					this.Thumbnail = memory.Load ();
				}
			}
		}

		private void DecompressSampleImage()
		{
			if ((this.compressedSampleImage != null) &&
				(this.sampleImage == null))
			{
				using (Opac.FreeImage.Memory memory = new Opac.FreeImage.Memory (this.compressedSampleImage))
				{
					this.SampleImage = memory.Load ();
				}
			}
		}
		
		private void CompressThumbnail()
		{
			if ((this.compressedThumbnail == null) &&
				(this.thumbnail != null))
			{
				this.compressedThumbnail = this.thumbnail.SaveToMemory (Opac.FreeImage.FileFormat.Jpeg, Opac.FreeImage.LoadSaveMode.JpegQualityNormal);

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
				this.compressedSampleImage = this.sampleImage.SaveToMemory (Opac.FreeImage.FileFormat.Jpeg, Opac.FreeImage.LoadSaveMode.JpegQualityNormal);

				if (this.compressedSampleImage != null)
				{
					this.engine.AddMemoryPressure (this.compressedSampleImage.Length);
				}
			}
		}

		private void ReleaseImageData()
		{
			if (this.imageData != null)
			{
				this.engine.RemoveMemoryPressure (this.imageData.Length);
				this.imageData.Dispose ();
				this.imageData = null;
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

			try
			{
				Opac.FreeImage.Image sampleImage = this.SampleImage;
				
				if (sampleImage == null)
				{
					byte[] cachedSampleImageData = ImageManager.ImageStore.LoadImageData (this.imageId, this.imageFileDate, this.engine.SampleImageSize, out this.width, out this.height);

					System.Diagnostics.Debug.Assert (this.compressedSampleImage == null);

					if ((cachedSampleImageData == null) ||
						(cachedSampleImageData.Length == 0))
					{
						this.LoadImageData ();

						if ((this.imageData == null) ||
							(this.imageData.IsValid == false))
						{
							return;
						}

						using (Opac.FreeImage.Image fullImage = this.imageData.Load ())
						{
							if (fullImage != null)
							{
								fullImage.Information = "FULL:" + this.imageFilePath + "/" + System.Environment.TickCount;
								
								this.width = fullImage.GetWidth ();
								this.height	= fullImage.GetHeight ();
								
								int dx;
								int dy;

								this.GetReducedImageSize (this.engine.SampleImageSize, out dx, out dy);

								sampleImage = fullImage.Rescale (dx, dy, Opac.FreeImage.Filter.Bilinear);
							}
						}

						if (sampleImage != null)
						{
							this.compressedSampleImage = ImageManager.ImageStore.SaveImageData (this.imageId, this.imageFileDate, this.engine.SampleImageSize, this.width, this.height, sampleImage);
							
							if (this.compressedSampleImage != null)
							{
								this.engine.AddMemoryPressure (this.compressedSampleImage.Length);
							}
						}
					}
					else
					{
						using (Opac.FreeImage.Memory memory = new Opac.FreeImage.Memory (cachedSampleImageData))
						{
							sampleImage = memory.Load ();
							this.compressedSampleImage = cachedSampleImageData;

							if (this.compressedSampleImage != null)
							{
								this.engine.AddMemoryPressure (this.compressedSampleImage.Length);
							}
						}
					}

					if (sampleImage == null)
					{
						return;
					}

					this.SampleImage = sampleImage;
				}

				Opac.FreeImage.Image thumbnail = this.Thumbnail;

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
						using (Opac.FreeImage.Memory memory = new Opac.FreeImage.Memory (cachedThumbnailData))
						{
							thumbnail = memory.Load ();
						}
						
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
			if (this.imageData == null)
			{
				byte[] data = null;

				try
				{
					data = this.engine.ReadAllBytes (this.imageFilePath);
				}
				catch (System.OutOfMemoryException)
				{
					System.Diagnostics.Debug.WriteLine ("Out of memory condition");
				}

				if (data != null)
				{
					lock (this.exclusion)
					{
						if (this.imageData == null)
						{
							this.imageData = new Opac.FreeImage.Memory (data);
						}
					}
					
					this.engine.AddMemoryPressure (data.Length);
				}
			}

			lock (this.exclusion)
			{
				if (this.imageData != null)
				{
					this.fileFormat = this.imageData.GetFileFormat ();
					this.imageDataTimestamp = System.DateTime.Now.Ticks;
				}
				else
				{
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

			Opac.FreeImage.Image image;

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

		private object exclusion = new object ();
		private string imageFilePath;
		private string imageId;
		private System.DateTime imageFileDate;
		private ImageManager engine;
		
		private int pendingCounter;
		private int workingCounter;

		private long imageDataTimestamp;
		private long thumbnailTimestamp;
		private long sampleImageTimestamp;

		private Opac.FreeImage.Image thumbnail;
		private Opac.FreeImage.Image sampleImage;
		private Opac.FreeImage.Memory imageData;
		private Opac.FreeImage.FileFormat fileFormat;
		private int width;
		private int height;

		private byte[] compressedThumbnail;
		private byte[] compressedSampleImage;
	}
}
