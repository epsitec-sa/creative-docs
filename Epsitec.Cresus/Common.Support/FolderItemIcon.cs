//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>FolderItemIcon</c> class encapsulates an icon and provides both
	/// a direct access to the underlying image bitmap and to an image name
	/// which can be used by <see cref="Epsitec.Common.Widgets.TextLayout"/>.
	/// </summary>
	public sealed class FolderItemIcon : System.IDisposable
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FolderItemIcon"/> class
		/// based on a specified image; internally, this associates the image to
		/// an id using the <see cref="FolderItemIconCache"/>.
		/// </summary>
		/// <param name="image">The image.</param>
		public FolderItemIcon(Drawing.Image image)
		{
			this.id = FolderItemIconCache.Instance.Add (image);
		}

		~FolderItemIcon()
		{
			this.Dispose (false);
		}

		/// <summary>
		/// Gets the image.
		/// </summary>
		/// <value>The image.</value>
		public Drawing.Image					Image
		{
			get
			{
				return FolderItemIconCache.Instance.Resolve (this.id);
			}
		}

		/// <summary>
		/// Gets the name of the image which can be used by the &lt;img&gt;
		/// tag in rich text.
		/// </summary>
		/// <value>The name of the image.</value>
		public string							ImageName
		{
			get
			{
				return string.Format (System.Globalization.CultureInfo.InvariantCulture, "foldericon:{0}", this.id);
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}

		#endregion

		private void Dispose(bool disposing)
		{
			FolderItemIconCache.Instance.Release (this.id);
			this.id = -1;
		}

		private long							id;
	}
}
