//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Document
{
	public class ImageFileReference : System.IDisposable
	{
		public ImageFileReference(string path)
		{
			this.path = path;
		}
		
		~ImageFileReference()
		{
			this.Dispose (false);
		}
		
		public ImageFileReference(byte[] data, string entryName)
		{
			string ext = string.IsNullOrEmpty (entryName) ? "" : System.IO.Path.GetExtension (entryName);
			string dir = System.IO.Path.GetTempPath ();
			string name = "pdf-export-" + System.Guid.NewGuid ().ToString ("D");
			this.path = System.IO.Path.Combine (dir, name + ext);
			this.deleteOnDispose = true;
			System.IO.File.WriteAllBytes (this.path, data);
		}
		
		public string Path
		{
			get
			{
				return this.path;
			}
		}
		
		public void Dispose()
		{
			System.GC.SuppressFinalize (this);
			this.Dispose (true);
		}
		
		private void Dispose(bool disposing)
		{
			if ((this.deleteOnDispose) && (!this.disposeExecuted) && (System.IO.File.Exists (this.path)))
			{
				this.disposeExecuted = true;
				System.IO.File.Delete (this.path);
			}
		}
		
		private readonly string path;
		private readonly bool deleteOnDispose;
		private bool disposeExecuted;
	}
}
