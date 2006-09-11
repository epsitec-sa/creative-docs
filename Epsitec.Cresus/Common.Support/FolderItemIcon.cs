//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Support
{
	public sealed class FolderItemIcon : System.IDisposable
	{
		public FolderItemIcon(Drawing.Image image)
		{
			this.id = FolderItemIconCache.Instance.Add (image);
		}

		~FolderItemIcon()
		{
			this.Dispose (false);
		}

		public Drawing.Image					Image
		{
			get
			{
				return FolderItemIconCache.Instance.Resolve (this.id);
			}
		}

		public string							ImageName
		{
			get
			{
				return string.Format (System.Globalization.CultureInfo.InvariantCulture, "icon:{0}", this.id);
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
