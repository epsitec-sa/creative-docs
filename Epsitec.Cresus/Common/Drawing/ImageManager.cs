//	Copyright © 2007-2012, OPaC bright ideas, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Drawing
{
	public sealed class ImageManager : System.IDisposable
	{
		public ImageManager()
		{
			lock (ImageManager.globalExclusion)
			{
				ImageManager.runningEngines.Add (this);
				
				if (ImageManager.callbackQueue == null)
				{
					ImageManager.callbackQueue = new CallbackQueue ();
				}
				if (ImageManager.imageStore == null)
				{
					ImageManager.imageStore = new ImageStore ();
				}
			}

			this.RegisterProtocol ("file", Protocols.FileProtocol.ReadBytes);
			this.RegisterProtocol ("http", Protocols.HttpProtocol.ReadBytes);
			this.RegisterProtocol ("ftp", Protocols.FtpProtocol.ReadBytes);

			this.MemoryLimit = 100 * 1024*1024L;
		}

		public bool HasPendingWork
		{
			get
			{
				lock (this.localExclusion)
				{
					if ((this.pendingWorkQueue.Count > 0) ||
						(this.processingWorkQueue.Count > 0))
					{
						return true;
					}
					else
					{
						return false;
					}
				}
			}
		}

		public long GetTotalMemorySize()
		{
			ImageData[] images;
			long size = 0;

			lock (this.localExclusion)
			{
				images = this.imageList.ToArray ();
			}

			foreach (ImageData image in images)
			{
				size += image.MemorySize;
			}

			return size;
		}

		public int GetImageCount()
		{
			lock (this.localExclusion)
			{
				System.Diagnostics.Debug.Assert (this.imageList.Count == this.images.Count);

				return this.imageList.Count;
			}
		}

		public long MemoryLimit
		{
			get
			{
				return this.memoryPressureThreshold;
			}
			set
			{
				value = System.Math.Min (value, 500 * 1024*1024L);
				value = System.Math.Max (value,  10 * 1024*1024L);

				this.memoryPressureThreshold = (int) value;
				this.threadPool.DefineMemoryLimit (value * 2);
			}
		}

		public IEnumerable<ImageData> GetImageList()
		{
			ImageData[] images;

			lock (this.localExclusion)
			{
				images = this.imageList.ToArray ();
			}

			return images;
		}

		public void ForEachImage(System.Action<ImageData> action)
		{
			foreach (ImageData image in this.GetImageList ())
			{
				action (image);
			}
		}

		public ImageData AddImage(string imageFilePath, string imageId, System.DateTime imageFileDate)
		{
			string key = ImageManager.GetKey (imageFilePath, imageFileDate);

			lock (this.localExclusion)
			{
				ImageData image;

				if (this.images.TryGetValue (key, out image))
				{
					if (image.QueueStatus == QueueStatus.Discarded)
					{
						this.images.Remove (key);
						this.imageList.Remove (image);
					}
					else
					{
						return null;
					}
				}
				
				image = new ImageData (this, imageFilePath, imageId, imageFileDate);

				this.images.Add (key, image);
				this.imageList.Add (image);
				return image;
			}
		}

		public void RemoveImage(string imageFilePath, System.DateTime imageFileDate)
		{
			string key = ImageManager.GetKey (imageFilePath, imageFileDate);

			lock (this.localExclusion)
			{
				ImageData image;

				if (this.images.TryGetValue (key, out image))
				{
					this.images.Remove (key);
					this.imageList.Remove (image);
				}
			}
		}
		
		public ImageData FindImage(string imageFilePath, System.DateTime imageFileDate)
		{
			string key = ImageManager.GetKey (imageFilePath, imageFileDate);

			lock (this.localExclusion)
			{
				ImageData image;

				if (this.images.TryGetValue (key, out image))
				{
					return image;
				}
				else
				{
					return null;
				}
			}
		}

		public ImageData GetImage(string imageFilePath, string imageId, System.DateTime imageFileDate)
		{
			this.AddImage (imageFilePath, imageId, imageFileDate);
			return this.FindImage (imageFilePath, imageFileDate);
		}

		public ImageData GetImageFromFile(string path)
		{
			try
			{
				if (System.IO.File.Exists (path))
				{
					return this.GetImage (string.Concat ("file:", path), path, System.IO.File.GetLastWriteTimeUtc (path));
				}
			}
			catch (System.IO.IOException)
			{
			}

			return null;
		}

		public void Clear()
		{
			lock (this.localExclusion)
			{
				this.pendingWorkQueue.Clear ();
			}

			this.WaitProcessingQueueEmpty (-1);

			lock (this.localExclusion)
			{
				this.images.Clear ();
				this.imageList.Clear ();
			}
		}

		public bool WaitProcessingQueueEmpty(int timeout)
		{
			return this.WaitProcessingQueueEvent (timeout,
				delegate ()
				{
					lock (this.localExclusion)
					{
						return (this.processingWorkQueue.Count == 0);
					}
				}
			);
		}

		public bool WaitProcessingQueueEvent(int timeout, ConditionCallback condition)
		{
			System.Threading.AutoResetEvent wait;

			lock (this.localExclusion)
			{
				wait = new System.Threading.AutoResetEvent (false);
				this.waitProcessingQueueEvents.Add (wait);
			}

			bool ok = condition ();

			if (ok == false)
			{
				wait.WaitOne (timeout, false);
				ok = condition ();
			}

			lock (this.localExclusion)
			{
				this.waitProcessingQueueEvents.Remove (wait);
			}

			return ok;
		}

		public void RegisterProtocol(string protocol, ByteReaderCallback callback)
		{
			while (protocol.EndsWith (":"))
			{
				protocol = protocol.Substring (0, protocol.Length-1);
			}

			lock (this.localExclusion)
			{
				this.protocols[protocol] = callback;
			}
		}

		public void UnregisterProtocol(string protocol)
		{
			while (protocol.EndsWith (":"))
			{
				protocol = protocol.Substring (0, protocol.Length-1);
			}

			lock (this.localExclusion)
			{
				this.protocols.Remove (protocol);
			}
		}


		public void AddMemoryPressure(long size)
		{
			System.Threading.Interlocked.Add (ref this.totalMemoryPressure, size);

			int pressure = System.Threading.Interlocked.Add (ref this.localMemoryPressure, (int) size);

			if (pressure > this.memoryPressureThreshold)
			{
				bool addWorkerThread = false;

				if (this.isCacheTrimmingRequested == false)
				{
					lock (this.cacheExclusion)
					{
						if (this.isCacheTrimmingRequested == false)
						{
							this.isCacheTrimmingRequested = true;
							addWorkerThread = true;
						}
					}
				}

				if (addWorkerThread)
				{
					this.threadPool.QueueWorkItem (this.ProcessWorkQueueItem);
				}
			}
		}

		public void RemoveMemoryPressure(long size)
		{
			System.Threading.Interlocked.Add (ref this.totalMemoryPressure, -size);
		}

		#region IDisposable Members

		public void Dispose()
		{
			CallbackQueue callbackQueue = null;
			ImageStore imageStore = null;

			lock (ImageManager.globalExclusion)
			{
				ImageManager.runningEngines.Remove (this);

				if (ImageManager.runningEngines.Count == 0)
				{
					callbackQueue = ImageManager.callbackQueue;
					imageStore = ImageManager.imageStore;

					ImageManager.callbackQueue = null;
					ImageManager.imageStore = null;
				}
			}

			if (callbackQueue != null)
			{
				callbackQueue.Dispose ();
			}
			if (imageStore != null)
			{
				imageStore.Dispose ();
			}
			
			if (this.threadPool != null)
			{
				this.threadPool.Dispose ();
				this.threadPool = null;
			}
		}

		#endregion

		public void TrimCaches(CacheClearing mode, bool skipPending)
		{
			this.TrimCaches (mode, 0L, skipPending);
		}

		public void TrimCaches(CacheClearing mode, long targetMemoryPressure, bool skipPending)
		{
			lock (this.cacheExclusion)
			{
				if (mode == CacheClearing.Default)
				{
					mode = CacheClearing.ReleaseLargeBuffers;
				}

				ImageData[] images;

				lock (this.localExclusion)
				{
					if (mode == CacheClearing.ReleaseEverything)
					{
						images = this.imageList.ToArray ();
					}
					else
					{
						List<ImageData> filteredImageList = new List<ImageData> ();
						
						foreach (ImageData image in this.imageList)
						{
							if (image.GetCacheTimestamp (mode) > 0)
							{
								filteredImageList.Add (image);
							}
						}
						
						images = filteredImageList.ToArray ();
						
						System.Array.Sort (images,
							delegate (ImageData a, ImageData b)
							{
								long aTime = a.GetCacheTimestamp (mode);
								long bTime = b.GetCacheTimestamp (mode);
								return aTime.CompareTo (bTime);
							});
					}
				}

				foreach (ImageData image in images)
				{
					if (image.QueueStatus != QueueStatus.Working)
					{
						if ((skipPending == false) ||
							(image.QueueStatus == QueueStatus.Inactive))
						{
							if (mode == CacheClearing.ReleaseEverything)
							{
								ImageManager.ClearImageCache (CacheClearing.ReleaseLargeBuffers, image);
								ImageManager.ClearImageCache (CacheClearing.ReleaseMediumBuffers, image);
								ImageManager.ClearImageCache (CacheClearing.ReleaseSmallBuffers, image);
							}
							else
							{
								ImageManager.ClearImageCache (mode, image);
							}

							if (this.totalMemoryPressure <= targetMemoryPressure)
							{
								break;
							}
						}
					}
				}
			}
		}

		public static ImageManager Instance
		{
			get
			{
				return ImageManager.instance;
			}
		}

		public static void InitializeDefaultCache()
		{
			lock (ImageManager.globalExclusion)
			{
				if (ImageManager.instance == null)
				{
					ImageManager.instance = new ImageManager ();
				}
			}
		}

		public static void ShutDownDefaultCache()
		{
			ImageManager cache = null;

			lock (ImageManager.globalExclusion)
			{
				if (ImageManager.instance != null)
				{
					cache = ImageManager.instance;
					ImageManager.instance = null;
				}
			}

			if (cache != null)
			{
				cache.Dispose ();
			}
		}

		public static string GetKey(string imageFilePath, System.DateTime imageFileDate)
		{
			return string.Concat (imageFilePath.ToLowerInvariant (), "|", imageFileDate.Ticks.ToString (System.Globalization.CultureInfo.InvariantCulture));
		}

		public static string GetKeyPrefix(string imageFilePath)
		{
			return string.Concat (imageFilePath.ToLowerInvariant (), "|");
		}

		private static void ClearImageCache(CacheClearing mode, ImageData image)
		{
			image.ClearCache (mode);
		}

		/// <summary>
		/// Gets or sets the image size used by the cache for thumbnail images. The thumbnails
		/// are used to represent image miniatures.
		/// </summary>
		///
		/// <remarks>
		/// This property should be set only at application initialization.
		/// Excessive size can prevent the application from running properly, due to to
		/// excessive memory consumption. The default value is set to 128.
		/// Values are clipped to the range [64..512].
		/// </remarks>
		public int ThumbnailSize
		{
			get
			{
				return this.thumbnailSize;
			}
			set
			{
				value = System.Math.Min (value, 512);
				value = System.Math.Max (value,  64);
				
				this.thumbnailSize = value;
			}
		}

		/// <summary>
		/// Gets or sets the image size used by the cache for previewing images. The samples
		/// are used to display images in a document view. They should not be too large to
		/// avoid excessive memory pressure, nor be to low, to avoid poor display quality.
		/// </summary>
		///
		/// <remarks>
		/// This property should be called only at application initialization.
		/// Excessive size can prevent the application from running properly, due to to
		/// excessive memory consumption. The default value is set to 512.
		/// Values are clipped to the range [64..512].
		/// </remarks>
		public int SampleImageSize
		{
			get
			{
				return this.sampleImageSize;
			}
			set
			{
				value = System.Math.Min (value, 8192);
				value = System.Math.Max (value, 128);
				
				this.sampleImageSize = value;
			}
		}

		public static ImageStore ImageStore
		{
			get
			{
				return ImageManager.imageStore;
			}
		}

		internal byte[] ReadAllBytes(string path)
		{
			int pos = path.IndexOf (':');
			
			string protocol;
			string name;

			if (pos < 0)
			{
				protocol = "file";
				name = path;
			}
			else
			{
				protocol = path.Substring (0, pos);
				name = path.Substring (pos+1);
			}

			while (protocol.EndsWith (":"))
			{
				protocol = protocol.Substring (0, protocol.Length-1);
			}
			
			ByteReaderCallback callback = null;

			lock (this.localExclusion)
			{
				this.protocols.TryGetValue (protocol, out callback);
			}

			if (callback != null)
			{
				return callback (name);
			}
			else
			{
				return null;
			}
		}
		
		internal void Queue(IQueueable queueable, Callback callback)
		{
			lock (this.localExclusion)
			{
				if (this.processingWorkQueue.Contains (callback))
				{
					return;
				}
				
				if (this.pendingWorkQueue.Contains (callback))
				{
					this.pendingWorkQueue.Remove (callback);
					this.pendingWorkQueue.AddFirst (callback);
				}
				else
				{
					this.pendingWorkQueue.AddFirst (callback);
					queueable.ChangePendingCounter (1);
				}

				this.threadPool.QueueWorkItem (this.ProcessWorkQueueItem);
			}
		}


		public static void SyncInvoke(Callback callback)
		{
			Platform.Dispatcher.Invoke (callback);
		}

		public static void AsyncInvoke(Callback callback)
		{
			if (Platform.Dispatcher.InvokeRequired)
			{
				if (ImageManager.callbackQueue != null)
				{
					ImageManager.callbackQueue.Queue (
						delegate ()
						{
							Platform.Dispatcher.Invoke (callback);
						});
				}
			}
			else
			{
				callback ();
			}
		}

		private void ProcessWorkQueueItem()
		{
			Callback callback = null;
			IQueueable queueable = null;

			this.TrimCache ();

			lock (this.localExclusion)
			{
				if (this.pendingWorkQueue.Count > 0)
				{
					callback = this.pendingWorkQueue.First.Value;
					queueable = callback.Target as IQueueable;
					this.pendingWorkQueue.RemoveFirst ();
					this.processingWorkQueue.AddLast (callback);
					queueable.ChangeWorkingCounter (1);
					queueable.ChangePendingCounter (-1);
				}
			}

			if (callback != null)
			{
				try
				{
					System.Threading.Interlocked.Increment (ref this.runningThreadCount);

					if (queueable.QueueStatus == QueueStatus.Discarded)
					{
						//System.Diagnostics.Debug.WriteLine ("Dropping discarded work item");
					}
					else
					{
						callback ();
					}
				}
				finally
				{
					System.Threading.Interlocked.Decrement (ref this.runningThreadCount);
					
					lock (this.localExclusion)
					{
						this.processingWorkQueue.Remove (callback);
						queueable.ChangeWorkingCounter (-1);

						if (this.waitProcessingQueueEvents.Count > 0)
						{
							foreach (System.Threading.EventWaitHandle handle in this.waitProcessingQueueEvents)
							{
								handle.Set ();
							}
						}
					}
				}
			}
		}


		private void TrimCache()
		{
			if (this.isCacheTrimmingRequested)
			{
				lock (this.cacheExclusion)
				{
					if (this.isCacheTrimmingRequested)
					{
						this.localMemoryPressure = 0;

						System.Diagnostics.Process process = System.Diagnostics.Process.GetCurrentProcess ();

						System.Diagnostics.Debug.WriteLine ("Trimming... VM = "+(process.VirtualMemorySize64/(1024*1024L))+"MB");

						this.TrimCaches (CacheClearing.ReleaseLargeBuffers, this.memoryPressureThreshold, true);

						if (this.totalMemoryPressure > this.memoryPressureThreshold)
						{
							this.TrimCaches (CacheClearing.ReleaseMediumBuffers, this.memoryPressureThreshold, true);
						}
						if (this.totalMemoryPressure > this.memoryPressureThreshold)
						{
							this.TrimCaches (CacheClearing.ReleaseSmallBuffers, this.memoryPressureThreshold, true);
						}

						this.isCacheTrimmingRequested = false;
					}
				}
			}
		}

		private static ImageManager instance;
		private static object globalExclusion = new object ();
		private static List<ImageManager> runningEngines = new List<ImageManager> ();
		private static CallbackQueue callbackQueue;
		private static ImageStore imageStore;

		private object localExclusion = new object ();
		private object cacheExclusion = new object ();

		private long totalMemoryPressure;
		private int localMemoryPressure;

		private int memoryPressureThreshold;
		private int thumbnailSize = 128;
		private int sampleImageSize = 512;

		private bool isCacheTrimmingRequested;

		private CustomThreadPool threadPool = new CustomThreadPool ();
		private LinkedList<Callback> processingWorkQueue = new LinkedList<Callback> ();
		private LinkedList<Callback> pendingWorkQueue = new LinkedList<Callback> ();
		private Dictionary<string, ImageData> images = new Dictionary<string, ImageData> ();
		private Dictionary<string, ByteReaderCallback> protocols = new Dictionary<string, ByteReaderCallback> ();
		private List<ImageData> imageList = new List<ImageData> ();
		private List<System.Threading.EventWaitHandle> waitProcessingQueueEvents = new List<System.Threading.EventWaitHandle> ();
		private int runningThreadCount;
	}
}
