//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	public struct FileFilterInfo
	{
		public FileFilterInfo(string path, System.IO.FileAttributes attributes)
		{
			this.path = path;
			this.extension = string.IsNullOrEmpty (path) ? null : System.IO.Path.GetExtension (path).ToLowerInvariant ();
			this.attributes = attributes;

			if ((path.Length == 3) &&
				(path[1] == ':'))
			{
				this.attributes |= System.IO.FileAttributes.Directory;
			}
		}

		public string Path
		{
			get
			{
				return this.path;
			}
		}

		public string LowerCaseExtension
		{
			get
			{
				return this.extension;
			}
		}

		public System.IO.FileAttributes Attributes
		{
			get
			{
				return this.attributes;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this.path == null;
			}
		}

		public static readonly FileFilterInfo Empty = new FileFilterInfo ();

		private string path;
		private string extension;
		private System.IO.FileAttributes attributes;
	}
}
