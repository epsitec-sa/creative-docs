//	Copyright © 2007-2011, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Support.Internal
{
	/// <summary>
	/// The <c>ImageDocumentInfo</c> class provides information for images.
	/// </summary>
	internal class ImageDocumentInfo : IDocumentInfo
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ImageDocumentInfo"/> class
		/// for a specified image.
		/// </summary>
		/// <param name="imageData">The image data.</param>
		public ImageDocumentInfo(Drawing.ImageData imageData)
		{
			this.imageData = imageData;
		}

		#region IDocumentInfo Members

		/// <summary>
		/// Gets the description of the document as a formatted string.
		/// </summary>
		/// <returns>The description of the document.</returns>
		public string GetDescription()
		{
			if (this.imageData != null)
			{
				int dx = this.imageData.SourceWidth;
				int dy = this.imageData.SourceHeight;

				if (dx > 0 && dy > 0)
				{
					string format = this.imageData.GetFileFormat ().ToString ();
					return string.Format(Res.Strings.Image.Description.ToString (), dx, dy, format);
				}
			}

			return null;
		}

		/// <summary>
		/// Gets the thumbnail image of the document.
		/// </summary>
		/// <returns>The thumbnail image or <c>null</c>.</returns>
		public Drawing.Image GetThumbnail()
		{
			Drawing.Image image;

			lock (this.exclusion)
			{
				image = this.GetCachedThumbnail ();
			}

			if (image == null)
			{
				image = Drawing.Bitmap.FromImage (this.imageData.GetThumbnail ());
			}

			lock (this.exclusion)
			{
				if (image == null)
				{
					this.thumbnail = null;
				}
				else
				{
					this.thumbnail = new Types.Weak<Drawing.Image> (image);
				}
			}

			return image;
		}

		/// <summary>
		/// Asynchronously gets the image thumbnail of the document.
		/// </summary>
		/// <param name="callback">The callback which will be invoked with the
		/// thumbnail image, as soon as it will be available; this may happen
		/// synchronously when calling <c>GetAsyncThumbnail</c>, or later.</param>
		public void GetAsyncThumbnail(SimpleCallback<Drawing.Image> callback)
		{
			Drawing.Image image;

			lock (this.exclusion)
			{
				image = this.GetCachedThumbnail ();

				if (image == null)
				{
					if (this.callbackQueue == null)
					{
						this.callbackQueue = new Queue<SimpleCallback<Drawing.Image>> ();
						this.imageData.Changed += this.HandleImageChanged;

						image = Drawing.Bitmap.FromImage (this.imageData.GetAsyncThumbnail ());

						if (image != null)
						{
							this.thumbnail = new Types.Weak<Drawing.Image> (image);
						}
					}

					image = this.GetCachedThumbnail ();

					if (image == null)
					{
						this.callbackQueue.Enqueue (callback);
						callback = null;
					}
				}
			}

			if (image == null)
			{
				image = Drawing.Bitmap.FromImage (this.imageData.Thumbnail);
			}

			if (image != null)
			{
				this.NotifyCallbacks (image, false);
				
				if (callback != null)
				{
					callback (image);
				}
			}
		}

		#endregion

		/// <summary>
		/// Handles the <c>Changed</c> event of the image data source. This
		/// may happen when the requested thumbnail becomes ready.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="Epsitec.Common.Drawing.ImageDataEventArgs"/> instance containing the event data.</param>
		private void HandleImageChanged(object sender, Drawing.ImageDataEventArgs e)
		{
			Drawing.Image image = Drawing.Bitmap.FromImage (this.imageData.GetAsyncThumbnail ());
			
			//	The thumbnail image might still not be ready; but if it is, we no longer
			//	need to listen to any events :
			
			if (image != null)
			{
				this.NotifyCallbacks (image, true);
			}
		}

		private void NotifyCallbacks(Drawing.Image image, bool unlink)
		{
			SimpleCallback<Drawing.Image>[] callbacks = null;

			lock (this.exclusion)
			{
				this.thumbnail = new Types.Weak<Drawing.Image> (image);
				
				if (this.callbackQueue != null)
				{
					callbacks = this.callbackQueue.ToArray ();
				}

				if (unlink)
				{
					this.imageData.Changed -= this.HandleImageChanged;
				}
				
				this.callbackQueue = null;
			}

			if (callbacks != null)
			{
				foreach (SimpleCallback<Drawing.Image> callback in callbacks)
				{
					callback (image);
				}
			}
		}

		/// <summary>
		/// Gets the cached thumbnail image.
		/// </summary>
		/// <returns>The image or <c>null</c>.</returns>
		private Drawing.Image GetCachedThumbnail()
		{
			Drawing.Image image = null;

			if (this.thumbnail != null)
			{
				image = this.thumbnail.Target;

				if (image == null)
				{
					System.Diagnostics.Debug.WriteLine ("GC freed image thumbnail: image path is " + this.imageData.ImageFilePath);
				}
			}

			return image;
		}

		private readonly object					exclusion = new object ();
		private Queue<SimpleCallback<Drawing.Image>>	callbackQueue;
		private Drawing.ImageData						imageData;
		private Types.Weak<Drawing.Image>				thumbnail;
	}
}
