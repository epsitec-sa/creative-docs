using System;
using System.Collections.Generic;
using System.Text;

namespace Epsitec.Common.Support
{
	public struct FolderItem
	{
		internal FolderItem(Common.Drawing.Image icon, string displayName, string typeName, string fullPath, System.IntPtr handle)
		{
			this.icon = icon;
			this.displayName = displayName;
			this.typeName = typeName;
			this.fullPath = fullPath;
			this.systemItemReference = handle;
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

		internal System.IntPtr SystemItemReference
		{
			get
			{
				return this.systemItemReference;
			}
		}
		
		private Drawing.Image icon;
		private string displayName;
		private string typeName;
		private string fullPath;
		private System.IntPtr systemItemReference;
	}
}
