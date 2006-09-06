using System;
using System.Collections.Generic;
using System.Text;

namespace Epsitec.Common.Support
{
	public struct FolderItem
	{
		internal FolderItem(Common.Drawing.Image icon, string displayName, string typeName, string fullPath, Platform.FolderItemHandle handle)
		{
			this.icon = icon;
			this.displayName = displayName;
			this.typeName = typeName;
			this.fullPath = fullPath;
			this.handle = handle;
		}
		
		public Drawing.Image Icon
		{
			get
			{
				return this.icon;
			}
		}

		public string DisplayName
		{
			get
			{
				return this.displayName;
			}
		}

		public string TypeName
		{
			get
			{
				return this.typeName;
			}
		}

		public string FullPath
		{
			get
			{
				return this.fullPath;
			}
		}

		public bool IsVirtual
		{
			get
			{
				return string.IsNullOrEmpty (this.fullPath);
			}
		}

		internal Platform.FolderItemHandle Handle
		{
			get
			{
				return this.handle;
			}
		}
		
		private Drawing.Image icon;
		private string displayName;
		private string typeName;
		private string fullPath;
		private Platform.FolderItemHandle handle;
	}
}
