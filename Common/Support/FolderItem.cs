/*
This file is part of CreativeDocs.

Copyright © 2003-2024, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland

CreativeDocs is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
any later version.

CreativeDocs is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/


using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Epsitec.Common.Support
{
    /// <summary>
    /// The <c>FolderItem</c> structure represents an item in a folder. This
    /// can be a folder, a file or a virtual folder (provided by the shell).
    /// </summary>
    public struct FolderItem : System.IEquatable<FolderItem>
    {
        public FolderItem(string path, FolderQueryMode mode)
        {
            this.fullPath = path;
            this.queryMode = mode;
        }

        //internal FolderItem(
        //    Common.Drawing.Image icon,
        //    FolderQueryMode queryMode,
        //    string displayName,
        //    string typeName,
        //    string fullPath,
        //    Platform.FolderItemHandle handle,
        //    Platform.FolderItemAttributes attributes
        //)
        //{
        //    this.icon = icon == null ? null : new FolderItemIcon(icon);
        //    this.queryMode = queryMode;
        //    this.displayName = displayName;
        //    this.typeName = typeName;
        //    this.fullPath = fullPath;
        //    this.handle = handle;
        //    this.attributes = attributes;

        //    if (
        //        (this.IsFolder)
        //        && (this.IsHidden == false)
        //        && (string.IsNullOrEmpty(this.fullPath) == false)
        //    )
        //    {
        //        //	For some yet unknown reasons, the attributes provided by the enumeration
        //        //	functions don't provide the correct 'Hidden' attribute in some cases; for
        //        //	instance, a .svn folder in a document library won't be marked as hidden.

        //        //	Make sure what the visibility of the directory really is:

        //        this.attributes = FolderItem.FixDirectoryHiddenAttribute(
        //            this.attributes,
        //            this.fullPath
        //        );
        //    }
        //}

        //private static Platform.FolderItemAttributes FixDirectoryHiddenAttribute(
        //    Platform.FolderItemAttributes attributes,
        //    string fullPath
        //)
        //{
        //    try
        //    {
        //        if (System.IO.Directory.Exists(fullPath))
        //        {
        //            var folderAttrib = new System.IO.DirectoryInfo(fullPath).Attributes;

        //            if ((folderAttrib & System.IO.FileAttributes.Hidden) != 0)
        //            {
        //                attributes |= Platform.FolderItemAttributes.Hidden;
        //            }
        //        }
        //    }
        //    catch { }

        //    return attributes;
        //}

        //internal FolderItem(Platform.FolderItemHandle handle)
        //{
        //    this.icon = null;
        //    this.queryMode = new FolderQueryMode();
        //    this.displayName = null;
        //    this.typeName = null;
        //    this.fullPath = null;
        //    this.handle = handle;
        //    this.attributes = Platform.FolderItemAttributes.None;
        //}

        /// <summary>
        /// Gets the icon for this item.
        /// </summary>
        /// <value>The icon or <c>null</c>.</value>
        public FolderItemIcon Icon
        {
            get
            {
                /*return this.icon;
                 * */
                throw new System.NotImplementedException();
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
                /*
                return this.displayName;
                */
                throw new System.NotImplementedException();
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
                /*
                return this.typeName;
                */
                throw new System.NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the full path for the item. Some virtual elements don't have a
        /// file path.
        /// </summary>
        /// <value>The full path.</value>
        public string FullPath
        {
            get { return this.fullPath; }
        }

        /// <summary>
        /// Gets a value indicating whether this item is purely virtual (i.e. it
        /// has no full path and only exists as a virtual construct, such as "My Computer").
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this item is purely virtual; otherwise, <c>false</c>.
        /// </value>
        public bool IsVirtual
        {
            get
            {
                /*
                return string.IsNullOrEmpty(this.fullPath);
                */
                throw new System.NotImplementedException();
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
                if (!Path.Exists(this.fullPath))
                {
                    return true;
                }
                if (!this.IsFolder)
                {
                    return false;
                }
                return !Directory.EnumerateFileSystemEntries(this.fullPath).Any();
            }
        }

        /// <summary>
        /// Gets a value indicating whether this item is valid.
        /// </summary>
        /// <value><c>true</c> if this item is valid; otherwise, <c>false</c>.</value>
        public bool IsValid
        {
            get
            {
                /*
                return this.displayName != null;
                */
                throw new System.NotImplementedException();
            }
        }

        /// <summary>
        /// Gets a value indicating whether this item is browsable. This is
        /// the case of the Internet.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this item is browsable; otherwise, <c>false</c>.
        /// </value>
        public bool IsBrowsable
        {
            get
            {
                /*
                return (this.attributes & Platform.FolderItemAttributes.Browsable) != 0;
                */
                throw new System.NotImplementedException();
            }
        }

        /// <summary>
        /// Gets a value indicating whether this item is a folder.
        /// </summary>
        /// <value><c>true</c> if this item is a folder; otherwise, <c>false</c>.</value>
        public bool IsFolder
        {
            get
            {
                FileAttributes attributes = File.GetAttributes(this.fullPath);
                return attributes.HasFlag(FileAttributes.Directory);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this item is compressed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this item is compressed; otherwise, <c>false</c>.
        /// </value>
        public bool IsCompressed
        {
            get
            {
                /*
                return (this.attributes & Platform.FolderItemAttributes.Compressed) != 0;
                */
                throw new System.NotImplementedException();
            }
        }

        /// <summary>
        /// Gets a value indicating whether this item is encrypted.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this item is encrypted; otherwise, <c>false</c>.
        /// </value>
        public bool IsEncrypted
        {
            get
            {
                /*
                return (this.attributes & Platform.FolderItemAttributes.Encrypted) != 0;
                */
                throw new System.NotImplementedException();
            }
        }

        /// <summary>
        /// Gets a value indicating whether this item is hidden.
        /// </summary>
        /// <value><c>true</c> if this item is hidden; otherwise, <c>false</c>.</value>
        public bool IsHidden
        {
            get
            {
                /*
                if ((this.attributes & Platform.FolderItemAttributes.Hidden) != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
                */
                throw new System.NotImplementedException();
            }
        }

        /// <summary>
        /// Gets a value indicating whether this item is a shortcut.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this item is a shortcut; otherwise, <c>false</c>.
        /// </value>
        public bool IsShortcut
        {
            get
            {
                /*
                return (this.attributes & Platform.FolderItemAttributes.Shortcut) != 0;
                */
                throw new System.NotImplementedException();
            }
        }

        /// <summary>
        /// Gets a value indicating whether this item is a web link.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this item is a web link; otherwise, <c>false</c>.
        /// </value>
        //public bool IsWebLink
        //{
        //    get { return (this.attributes & Platform.FolderItemAttributes.WebLink) != 0; }
        //}

        /// <summary>
        /// Gets a value indicating whether this item is read only.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this item is read only; otherwise, <c>false</c>.
        /// </value>
        public bool IsReadOnly
        {
            get
            {
                /*
                return (this.attributes & Platform.FolderItemAttributes.ReadOnly) != 0;
                */
                throw new System.NotImplementedException();
            }
        }

        /// <summary>
        /// Gets a value indicating whether this item is a drive.
        /// </summary>
        /// <value><c>true</c> if this item is a drive; otherwise, <c>false</c>.</value>
        public bool IsDrive
        {
            get
            {
                /*
                //	WIN32 specific code :

                if (
                    (this.fullPath != null)
                    && (this.fullPath.Length == 3)
                    && (this.fullPath[1] == ':')
                )
                {
                    return true;
                }
                else
                {
                    return false;
                }
                */
                throw new System.NotImplementedException();
            }
        }

        /// <summary>
        /// Gets a value indicating whether this item is file system node (a
        /// real folder or file).
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this item is a file system node; otherwise, <c>false</c>.
        /// </value>
        public bool IsFileSystemNode
        {
            get
            {
                /*
                return (this.attributes & Platform.FolderItemAttributes.FileSystemNode) != 0;
                */
                throw new System.NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the drive info.
        /// </summary>
        /// <value>The drive info or <c>null</c> if the item is not a drive.</value>
        public System.IO.DriveInfo DriveInfo
        {
            get
            {
                /*
                if (this.IsDrive)
                {
                    return new System.IO.DriveInfo(this.fullPath);
                }
                else
                {
                    return null;
                }
                */
                throw new System.NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the folder query mode used to get this item.
        /// </summary>
        /// <value>The query mode.</value>
        public FolderQueryMode QueryMode
        {
            get
            {
                /*
                return this.queryMode;
                */
                throw new System.NotImplementedException();
            }
        }

        public static readonly FolderItem Empty = new FolderItem();

        /// <summary>
        /// Gets a value indicating whether the user expects hidden files to show.
        /// </summary>
        /// <value><c>true</c> if hidden files should be shown; otherwise, <c>false</c>.</value>
        public static bool ShowHiddenFiles
        {
            get
            {
                /*
                int ticks = System.Environment.TickCount;
                int delta = System.Math.Abs(FolderItem.showHiddenFilesCacheTicks - ticks);

                if (delta > FolderItem.cacheTickLifeTime)
                {
                    //	Refresh the cached value only if it is older than 10 seconds;
                    //	we do this to avoid re-reading repeatedly the registry value
                    //	when ShowHiddenFiles gets called in a tight loop.

                    int value = (int)
                        Microsoft.Win32.Registry.GetValue(
                            @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced",
                            "Hidden",
                            2
                        );

                    FolderItem.showHiddenFilesCacheValue = (value == 1);
                    FolderItem.showHiddenFilesCacheTicks = ticks;
                }

                return FolderItem.showHiddenFilesCacheValue;
                */
                throw new System.NotImplementedException();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the user expects not to see file extensions.
        /// </summary>
        /// <value><c>true</c> if file extensions should be hidden; otherwise, <c>false</c>.</value>
        public static bool HideFileExtensions
        {
            get
            {
                /*
                int ticks = System.Environment.TickCount;
                int delta = System.Math.Abs(FolderItem.hideFileExtensionsCacheTicks - ticks);

                if (delta > FolderItem.cacheTickLifeTime)
                {
                    int value = (int)
                        Microsoft.Win32.Registry.GetValue(
                            @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced",
                            "HideFileExt",
                            2
                        );

                    FolderItem.hideFileExtensionsCacheValue = (value == 1);
                    FolderItem.hideFileExtensionsCacheTicks = ticks;
                }

                return FolderItem.hideFileExtensionsCacheValue;
                */
                throw new System.NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the short name of the file, with or without the extension, based
        /// on the user settings.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <returns>The short name of the file.</returns>
        public static string GetShortFileName(string path)
        {
            /*
            if (FolderItem.HideFileExtensions)
            {
                return System.IO.Path.GetFileNameWithoutExtension(path);
            }
            else
            {
                return System.IO.Path.GetFileName(path);
            }
            */
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Determines whether the specified path is a network path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        /// 	<c>true</c> if specified path is a network path; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNetworkPath(string path)
        {
            /*
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }

            if (path.StartsWith(@"\\"))
            {
                return true;
            }
            else
            {
                return false;
            }
            */
            throw new System.NotImplementedException();
        }

        #region IEquatable<FolderItem> Members

        public bool Equals(FolderItem other)
        {
            /*
            if (System.Object.ReferenceEquals(other, null))
            {
                return false;
            }

            if (this.handle == null)
            {
                return (this.handle == other.handle);
            }

            return this.handle.Equals(other.handle);
            */
            throw new System.NotImplementedException();
        }

        #endregion

        public static bool operator ==(FolderItem a, FolderItem b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(FolderItem a, FolderItem b)
        {
            return !a.Equals(b);
        }

        public override bool Equals(object obj)
        {
            if (obj is FolderItem)
            {
                return this.Equals((FolderItem)obj);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return this.handle == null ? 0 : this.handle.GetHashCode();
        }

        public override string ToString()
        {
            /*
            System.Text.StringBuilder buffer = new System.Text.StringBuilder();

            buffer.Append(this.DisplayName);
            buffer.AppendFormat(" ({0}) attr=", this.TypeName);
            buffer.Append(this.IsBrowsable ? "B" : "b");
            buffer.Append(this.IsFolder ? "F" : "f");
            buffer.Append(this.IsDrive ? "D" : "d");
            buffer.Append(this.IsCompressed ? "C" : "c");
            buffer.Append(this.IsEncrypted ? "E" : "e");
            buffer.Append(this.IsHidden ? "H" : "h");
            buffer.Append(this.IsShortcut ? "S" : "s");
            buffer.Append(this.IsReadOnly ? "R" : "r");
            buffer.Append(this.IsFileSystemNode ? "N" : "n");

            return buffer.ToString();
            */
            throw new System.NotImplementedException();
        }

        //internal Platform.FolderItemHandle Handle
        //{
        //    get { return this.handle; }
        //    set { this.handle = value; }
        //}

        private readonly FolderItemIcon icon;
        private readonly string displayName;
        private readonly string typeName;
        private readonly string fullPath;
        private readonly Platform.FolderItemAttributes attributes;
        private readonly FolderQueryMode queryMode;
        private Platform.FolderItemHandle handle;

        [System.ThreadStatic]
        private static bool hideFileExtensionsCacheValue;

        [System.ThreadStatic]
        private static bool showHiddenFilesCacheValue;

        [System.ThreadStatic]
        private static int hideFileExtensionsCacheTicks;

        [System.ThreadStatic]
        private static int showHiddenFilesCacheTicks;

        private const int cacheTickLifeTime = 10 * 1000;
    }
}
