//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Engine.Pdf
{
	/// <summary>
	/// The <c>ImageFileReference</c> class is used to reference a data file, which may
	/// be automatically deleted when it is no longer needed.
	/// </summary>
	public class ImageFileReference : System.IDisposable
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ImageFileReference"/> class,
		/// pointing to an existing file.
		/// </summary>
		/// <param name="path">The path.</param>
		public ImageFileReference(string path)
		{
			this.path = path;
		}
		
		~ImageFileReference()
		{
			this.Dispose (false);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ImageFileReference"/> class,
		/// pointing to a temporary file, created on the fly based on the provided data.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <param name="entryName">Name of the entry.</param>
		public ImageFileReference(byte[] data, string entryName)
		{
			string ext  = string.IsNullOrEmpty (entryName) ? "" : System.IO.Path.GetExtension (entryName);
			string dir  = System.IO.Path.GetTempPath ();
			string name = "pdf-export-" + System.Guid.NewGuid ().ToString ("D");
			
			this.path = System.IO.Path.Combine (dir, name + ext);
			this.deleteOnDispose = true;
			
			//	Save the data to a temporary file, which will be deleted when this
			//	reference is disposed.
			
			System.IO.File.WriteAllBytes (this.path, data);
		}

		/// <summary>
		/// Gets the path to the file.
		/// </summary>
		public string						Path
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
