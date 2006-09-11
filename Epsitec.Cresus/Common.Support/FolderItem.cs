//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>FolderItem</c> structure represents an item in a folder. This
	/// can be a folder, a file or a virtual folder (provided by the shell).
	/// </summary>
	public struct FolderItem : System.IEquatable<FolderItem>
	{
		internal FolderItem(Common.Drawing.Image icon, string displayName, string typeName, string fullPath, Platform.FolderItemHandle handle, Platform.FolderItemAttributes attributes)
		{
			this.icon = icon == null ? null : new FolderItemIcon (icon);
			this.displayName = displayName;
			this.typeName = typeName;
			this.fullPath = fullPath;
			this.handle = handle;
			this.attributes = attributes;
		}

		/// <summary>
		/// Gets the icon for this item.
		/// </summary>
		/// <value>The icon or <c>null</c>.</value>
		public FolderItemIcon					Icon
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
		public string							DisplayName
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
		public string							TypeName
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
		public string							FullPath
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
		public bool								IsVirtual
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
		public bool								IsEmpty
		{
			get
			{
				return this.displayName == null;
			}
		}

		public bool								IsBrowsable
		{
			get
			{
				return (this.attributes & Platform.FolderItemAttributes.Browsable) != 0;
			}
		}

		public bool								IsFolder
		{
			get
			{
				return (this.attributes & Platform.FolderItemAttributes.Folder) != 0;
			}
		}


		public bool								IsCompressed
		{
			get
			{
				return (this.attributes & Platform.FolderItemAttributes.Compressed) != 0;
			}
		}

		public bool								IsEncrypted
		{
			get
			{
				return (this.attributes & Platform.FolderItemAttributes.Encrypted) != 0;
			}
		}

		public bool								IsHidden
		{
			get
			{
				return (this.attributes & Platform.FolderItemAttributes.Hidden) != 0;
			}
		}

		public bool								IsShortcut
		{
			get
			{
				return (this.attributes & Platform.FolderItemAttributes.Shortcut) != 0;
			}
		}

		public bool								IsReadOnly
		{
			get
			{
				return (this.attributes & Platform.FolderItemAttributes.ReadOnly) != 0;
			}
		}

		public bool								IsDrive
		{
			get
			{
				//	WIN32 specific code :

				if ((this.fullPath != null) &&
					(this.fullPath.Length == 3) &&
					(this.fullPath[1] == ':'))
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		public bool								IsFileSystemNode
		{
			get
			{
				return (this.attributes & Platform.FolderItemAttributes.FileSystemNode) != 0;
			}
		}

		public System.IO.DriveInfo				DriveInfo
		{
			get
			{
				if (this.IsDrive)
				{
					return new System.IO.DriveInfo (this.fullPath);
				}
				else
				{
					return null;
				}
			}
		}

		
		public static readonly FolderItem		Empty = new FolderItem ();

		public static bool						ShowHiddenFiles
		{
			get
			{
				int value = (int) Microsoft.Win32.Registry.GetValue (@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "Hidden", 2);

				return value == 1;
			}
		}
		
		#region IEquatable<FolderItem> Members

		public bool Equals(FolderItem other)
		{
			if (System.Object.ReferenceEquals (other, null))
			{
				return false;
			}

			if (this.handle == null)
			{
				return (this.handle == other.handle);
			}

			return this.handle.Equals (other.handle);
		}

		#endregion

		public static bool operator==(FolderItem a, FolderItem b)
		{
			return a.Equals (b);
		}

		public static bool operator!=(FolderItem a, FolderItem b)
		{
			return !a.Equals (b);
		}

		public override bool Equals(object obj)
		{
			if (obj is FolderItem)
			{
				return this.Equals ((FolderItem) obj);
			}
			else
			{
				return false;
			}
		}

		public override int GetHashCode()
		{
			return this.handle == null ? 0 : this.handle.GetHashCode ();
		}

		public override string ToString()
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();

			buffer.Append (this.DisplayName);
			buffer.AppendFormat (" ({0}) attr=", this.TypeName);
			buffer.Append (this.IsBrowsable ? "B" : "b");
			buffer.Append (this.IsFolder ? "F" : "f");
			buffer.Append (this.IsDrive ? "D" : "d");
			buffer.Append (this.IsCompressed ? "C" : "c");
			buffer.Append (this.IsEncrypted ? "E" : "e");
			buffer.Append (this.IsHidden ? "H" : "h");
			buffer.Append (this.IsShortcut ? "S" : "s");
			buffer.Append (this.IsReadOnly ? "R" : "r");
			buffer.Append (this.IsFileSystemNode ? "N" : "n");

			return buffer.ToString ();
		}
		
		
		internal Platform.FolderItemHandle		Handle
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
		
		private FolderItemIcon					icon;
		private string							displayName;
		private string							typeName;
		private string							fullPath;
		private Platform.FolderItemHandle		handle;
		private Platform.FolderItemAttributes	attributes;
	}
}
