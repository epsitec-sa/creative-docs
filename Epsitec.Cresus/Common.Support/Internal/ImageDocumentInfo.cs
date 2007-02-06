//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Support.Internal
{
	internal class ImageDocumentInfo : IDocumentInfo
	{
		public ImageDocumentInfo(Drawing.ImageData image)
		{
			this.image = image;
		}

		#region IDocumentInfo Members

		public string GetDescription()
		{
			if (this.image != null)
			{
				string format = this.image.GetFileFormat ().ToString ();
				string size = string.Format ("{0} x {1}", this.image.SourceWidth, this.image.SourceHeight);

				return string.Concat (format, "<br/>", size);
			}

			return "";
		}

		public Drawing.Image GetThumbnail()
		{
			Drawing.Image image;

			lock (this.exclusion)
			{
				image = this.GetCachedThumbnail ();
			}

			if (image == null)
			{
				image = Drawing.Bitmap.FromImage (this.image.GetThumbnail ());
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
						this.image.Changed += this.HandleImageChanged;

						image = Drawing.Bitmap.FromImage (this.image.GetAsyncThumbnail ());

						if (image != null)
						{
							this.thumbnail = new Types.Weak<Drawing.Image> (image);
						}
					}

					image = this.GetCachedThumbnail ();

					if (image == null)
					{
						this.callbackQueue.Enqueue (callback);
					}
				}
			}

			if (image != null)
			{
				callback (image);
			}
		}

		#endregion

		private void HandleImageChanged(object sender, Drawing.ImageDataEventArgs e)
		{
			Drawing.Image image = Drawing.Bitmap.FromImage (this.image.GetAsyncThumbnail ());
			
			if (image != null)
			{
				SimpleCallback<Drawing.Image>[] callbacks;

				lock (this.exclusion)
				{
					this.thumbnail = new Types.Weak<Drawing.Image> (image);
					callbacks = this.callbackQueue.ToArray ();
					this.callbackQueue = null;
					this.image.Changed -= this.HandleImageChanged;
				}

				foreach (SimpleCallback<Drawing.Image> callback in callbacks)
				{
					callback (image);
				}
			}
		}

		private Drawing.Image GetCachedThumbnail()
		{
			Drawing.Image image = null;

			if (this.thumbnail != null)
			{
				image = this.thumbnail.Target;
			}

			return image;
		}

		private object exclusion = new object ();
		Queue<SimpleCallback<Drawing.Image>> callbackQueue;
		Drawing.ImageData image;
		Types.Weak<Drawing.Image> thumbnail;
	}
}
