//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
		
		string path;
		string extension;
		System.IO.FileAttributes attributes;
	}
}
