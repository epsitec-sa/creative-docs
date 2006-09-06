using System;
using System.Collections.Generic;
using System.Text;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>FolderItem</c> structure represents an item in a folder. This
	/// can be a folder, a file or a virtual folder (provided by the shell).
	/// </summary>
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

		/// <summary>
		/// Gets the icon for this item.
		/// </summary>
		/// <value>The icon or <c>null</c>.</value>
		public Drawing.Image Icon
		{
			get
			{
				return this.icon;
			}
		}

		/// <summary>
		/// Gets the display name for the item.
		/// </summary>
		/// <value>The display name.</value>
		public string DisplayName
		{
			get
			{
				return this.displayName;
			}
		}

		/// <summary>
		/// Gets the type name for the item (e.g. "Folder", "Text File", ...).
		/// </summary>
		/// <value>The type name.</value>
		public string TypeName
		{
			get
			{
				return this.typeName;
			}
		}

		/// <summary>
		/// Gets the full path for the item. Some virtual elements don't have a
		/// file path.
		/// </summary>
		/// <value>The full path.</value>
		public string FullPath
		{
			get
			{
				return this.fullPath;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance is purely virtual (i.e. it
		/// has no full path and only exists as a virtual construct, such as "My Computer").
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this item is purely virtual; otherwise, <c>false</c>.
		/// </value>
		public bool IsVirtual
		{
			get
			{
				return string.IsNullOrEmpty (this.fullPath);
			}
		}

		/// <summary>
		/// Gets a value indicating whether this item is empty.
		/// </summary>
		/// <value><c>true</c> if this item is empty; otherwise, <c>false</c>.</value>
		public bool IsEmpty
		{
			get
			{
				return this.displayName == null;
			}
		}

		public static readonly FolderItem Empty = new FolderItem ();

		internal Platform.FolderItemHandle Handle
		{
			get
			{
				return this.handle;
			}
			set
			{
				this.handle = value;
			}
		}
		
		private Drawing.Image icon;
		private string displayName;
		private string typeName;
		private string fullPath;
		private Platform.FolderItemHandle handle;
	}
}
