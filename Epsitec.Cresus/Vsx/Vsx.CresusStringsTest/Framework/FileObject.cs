//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Roger VUISTINER

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec
{
	public class FileObject : IEquatable<FileObject>, IDisposable
	{
		public FileObject(string path)
		{
			this.Path = System.IO.Path.GetFullPath (path);
		}

		~FileObject()
		{
			this.Dispose (false);
		}

		#region Object overrides

		public override int GetHashCode()
		{
			return this.Path.ToLower ().GetHashCode ();
		}

		public override bool Equals(object obj)
		{
			return this.Equals (obj as FileObject);
		}

		public override string ToString()
		{
			return this.Path;
		}

		#endregion

		#region IEquatable<FileObject> Members

		public bool Equals(FileObject other)
		{
			if (this == other)
			{
				return true;
			}
			if (other == null)
			{
				return false;
			}
			return string.Compare (this.Path, other.Path, StringComparison.InvariantCultureIgnoreCase) == 0;
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			this.Dispose (true);
			GC.SuppressFinalize (this);
		}

		#endregion

		public string Path
		{
			get;
			private set;
		}

		public bool Exists
		{
			get
			{
				return System.IO.File.Exists (this.Path);
			}
		}

		public bool IsEmpty
		{
			get
			{
				var fileInfo = this.FileInfo;
				return !fileInfo.Exists || fileInfo.Length == 0;
			}
		}

		public System.IO.FileInfo FileInfo
		{
			get
			{
				return new System.IO.FileInfo (this.Path);
			}
		}

		public void Clear()
		{
			System.IO.File.WriteAllBytes (this.Path, new byte[0]);
		}

		public void Delete()
		{
			System.IO.File.Delete (this.Path);
		}

		public void CopyTo(FileObject other)
		{
			if (!this.Equals (other))
			{
				System.IO.File.WriteAllBytes (other.Path, System.IO.File.ReadAllBytes (this.Path));
			}
		}

		protected virtual void Dispose(bool disposing)
		{
		}
	}
}
