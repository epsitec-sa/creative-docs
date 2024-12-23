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


using System.Runtime.InteropServices;

namespace Epsitec.Common.Support.Platform.Win32
{
    using Epsitec.Common.Widgets.Platform;
    using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

    internal static class ShellApi
    {
        public delegate int BrowseCallbackProc(
            System.IntPtr hwnd,
            uint uMsg,
            int lParam,
            int lpData
        );

        // Contains parameters for the SHBrowseForFolder function and receives information about the folder selected
        // by the user.
        [StructLayout(LayoutKind.Sequential)]
        public struct BROWSEINFO
        {
            public System.IntPtr hwndOwner; // Handle to the owner window for the dialog box.

            public System.IntPtr pidlRoot; // Pointer to an item identifier list (PIDL) specifying the

            // location of the root folder from which to start browsing.

            [MarshalAs(UnmanagedType.LPStr)] // Address of a buffer to receive the display name of the
            public string pszDisplayName; // folder selected by the user.

            [MarshalAs(UnmanagedType.LPStr)] // Address of a null-terminated string that is displayed
            public string lpszTitle; // above the tree view control in the dialog box.

            public uint ulFlags; // Flags specifying the options for the dialog box.

            [MarshalAs(UnmanagedType.FunctionPtr)] // Address of an application-defined function that the
            public BrowseCallbackProc lpfn; // dialog box calls when an event occurs.

            public int lParam; // Application-defined value that the dialog box passes to

            // the callback function

            public int iImage; // Variable to receive the image associated with the selected folder.
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct STRRETinternal
        {
            [FieldOffset(0)]
            public System.IntPtr pOleStr; // must be freed by caller of GetDisplayNameOf

            [FieldOffset(0)]
            public System.IntPtr pStr; // NOT USED

            [FieldOffset(0)]
            public uint uOffset; // Offset into SHITEMID

            [FieldOffset(0)]
            public System.IntPtr cStr; // Buffer to fill in (ANSI)
        }

        //		[StructLayout(LayoutKind.Explicit)]
        //		public struct STRRET
        //		{
        //			[FieldOffset(0)]
        //			public uint uType;						// One of the STRRET_* values
        //
        //			[FieldOffset(4)]
        //			public System.IntPtr pOleStr;						// must be freed by caller of GetDisplayNameOf
        //
        //			[FieldOffset(4)]
        //			public System.IntPtr pStr;							// NOT USED
        //
        //			[FieldOffset(4)]
        //			public uint uOffset;						// Offset into SHITEMID
        //
        //			[FieldOffset(4)]
        //			public System.IntPtr cStr;							// Buffer to fill in (ANSI)
        //		}

        [StructLayout(LayoutKind.Sequential)]
        public struct STRRET
        {
            public uint uType; // One of the STRRET_* values

            public STRRETinternal data;
        }

        // Contains information used by ShellExecuteEx
        [StructLayout(LayoutKind.Sequential)]
        public struct SHELLEXECUTEINFO
        {
            public uint cbSize; // Size of the structure, in bytes.
            public uint fMask; // Array of flags that indicate the content and validity of the

            // other structure members.
            public System.IntPtr hwnd; // PlatformWindow handle to any message boxes that the system might produce

            // while executing this function.
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpVerb; // string, referred to as a verb, that specifies the action to

            // be performed.
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpFile; // Address of a null-terminated string that specifies the name of

            // the file or object on which ShellExecuteEx will perform the
            // action specified by the lpVerb parameter.
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpParameters; // Address of a null-terminated string that contains the

            // application parameters.
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpDirectory; // Address of a null-terminated string that specifies the name of

            // the working directory.
            public int nShow; // Flags that specify how an application is to be shown when it

            // is opened.
            public System.IntPtr hInstApp; // If the function succeeds, it sets this member to a value

            // greater than 32.
            public System.IntPtr lpIDList; // Address of an ITEMIDLIST structure to contain an item identifier

            // list uniquely identifying the file to execute.
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpClass; // Address of a null-terminated string that specifies the name of

            // a file class or a globally unique identifier (GUID).
            public System.IntPtr hkeyClass; // Handle to the registry key for the file class.
            public uint dwHotKey; // Hot key to associate with the application.
            public System.IntPtr hIconMonitor; // Handle to the icon for the file class. OR Handle to the monitor

            // upon which the document is to be displayed.
            public System.IntPtr hProcess; // Handle to the newly started application.
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SHFILEINFO
        {
            public System.IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Win32Const.MAX_PATH)]
            public string szDisplayName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        // Contains information that the SHFileOperation function uses to perform file operations.
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SHFILEOPSTRUCT
        {
            public System.IntPtr hwnd; // PlatformWindow handle to the dialog box to display information about the

            // status of the file operation.
            public uint wFunc; // Value that indicates which operation to perform.
            public System.IntPtr pFrom; // Address of a buffer to specify one or more source file names.

            // These names must be fully qualified paths. Standard Microsoft®
            // MS-DOS® wild cards, such as "*", are permitted in the file-name
            // position. Although this member is declared as a null-terminated
            // string, it is used as a buffer to hold multiple file names. Each
            // file name must be terminated by a single NULL character. An
            // additional NULL character must be appended to the end of the
            // final name to indicate the end of pFrom.
            public System.IntPtr pTo; // Address of a buffer to contain the name of the destination file or

            // directory. This parameter must be set to NULL if it is not used.
            // Like pFrom, the pTo member is also a double-null terminated
            // string and is handled in much the same way.
            public ushort fFlags; // Flags that control the file operation.
            public int fAnyOperationsAborted; // Value that receives TRUE if the user aborted any file operations

            // before they were completed, or FALSE otherwise.
            public System.IntPtr hNameMappings; // A handle to a name mapping object containing the old and new

            // names of the renamed files. This member is used only if the
            // fFlags member includes the FOF_WANTMAPPINGHANDLE flag.
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpszProgressTitle; // Address of a string to use as the title of a progress dialog box.
            // This member is used only if fFlags includes the
            // FOF_SIMPLEPROGRESS flag.
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct APPBARDATA
        {
            public uint cbSize;
            public System.IntPtr hWnd;
            public uint uCallbackMessage;
            public uint uEdge;
            public RECT rc;
            public int lParam;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ComboBoxInfo
        {
            public int cbSize;
            public RECT rcItem;
            public RECT rcButton;
            public System.IntPtr stateButton;
            public System.IntPtr hwndCombo;
            public System.IntPtr hwndEdit;
            public System.IntPtr hwndList;
        }

        // IShellLink.Resolve fFlags
        [System.Flags]
        internal enum SLR_FLAGS
        {
            SLR_NO_UI = 0x0001,
            SLR_ANY_MATCH = 0x0002,
            SLR_UPDATE = 0x0004,
            SLR_NOUPDATE = 0x0008,
            SLR_NOSEARCH = 0x0010,
            SLR_NOTRACK = 0x0020,
            SLR_NOLINKINFO = 0x0040,
            SLR_INVOKE_MSI = 0x0080
        }

        // IShellLink.GetPath fFlags
        [System.Flags]
        internal enum SLGP_FLAGS
        {
            SLGP_SHORTPATH = 0x0001,
            SLGP_UNCPRIORITY = 0x0002,
            SLGP_RAWPATH = 0x0004
        }

        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct WIN32_FIND_DATAW
        {
            public int dwFileAttributes;
            public FILETIME ftCreationTime;
            public FILETIME ftLastAccessTime;
            public FILETIME ftLastWriteTime;
            public int nFileSizeHigh;
            public int nFileSizeLow;
            public int dwReserved0;
            public int dwReserved1;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Win32Const.MAX_PATH)]
            public string cFileName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Win32Const.MAX_SHORTPATH)]
            public string cAlternateFileName;
        }

        [System.Flags]
        internal enum StockIconOptions : uint
        {
            Small = 0x000000001, // Retrieve the small version of the icon, as specified by the SM_CXSMICON and SM_CYSMICON system metrics.
            ShellSize = 0x000000004, // Retrieve the shell-sized icons rather than the sizes specified by the system metrics.
            Handle = 0x000000100, // The hIcon member of the SHSTOCKICONINFO structure receives a handle to the specified icon.
            SystemIndex = 0x000004000, // The iSysImageImage member of the SHSTOCKICONINFO structure receives the index of the specified icon in the system imagelist.
            LinkOverlay = 0x000008000, // Add the link overlay to the file's icon.
            Selected = 0x000010000 // Blend the icon with the system highlight color.
        }

        //	See http://msdn.microsoft.com/en-us/library/bb762542(VS.85).aspx
        public enum StockIconIdentifier : uint
        {
            DocumentNotAssociated = 0, // document (blank page), no associated program
            DocumentAssociated = 1, // document with an associated program
            Application = 2, // generic application with no custom icon
            Folder = 3, // Folder (closed)
            FolderOpen = 4, // Folder (open)
            Drive525 = 5, // 5.25" floppy disk Drive
            Drive35 = 6, // 3.5" floppy disk Drive
            DriveRemove = 7, // removable Drive
            DriveFixed = 8, // Fixed (hard disk) Drive
            DriveNetwork = 9, // Network Drive
            DriveNetworkDisabled = 10, // disconnected Network Drive
            DriveCD = 11, // CD Drive
            DriveRAM = 12, // RAM disk Drive
            World = 13, // entire Network
            Server = 15, // a computer on the Network
            Printer = 16, // printer
            MyNetwork = 17, // My Network places
            Find = 22, // Find
            Help = 23, // Help
            Share = 28, // overlay for shared items
            Link = 29, // overlay for shortcuts to items
            SlowFile = 30, // overlay for slow items
            Recycler = 31, // empty recycle bin
            RecyclerFull = 32, // full recycle bin
            MediaCDAudio = 40, // Audio CD Media
            Lock = 47, // Security lock
            AutoList = 49, // AutoList
            PrinterNet = 50, // Network printer
            ServerShare = 51, // Server share
            PrinterFax = 52, // Fax printer
            PrinterFaxNet = 53, // Networked Fax Printer
            PrinterFile = 54, // Print to File
            Stack = 55, // Stack
            MediaSVCD = 56, // SVCD Media
            StuffedFolder = 57, // Folder containing other items
            DriveUnknown = 58, // Unknown Drive
            DriveDVD = 59, // DVD Drive
            MediaDVD = 60, // DVD Media
            MediaDVDRAM = 61, // DVD-RAM Media
            MediaDVDRW = 62, // DVD-RW Media
            MediaDVDR = 63, // DVD-R Media
            MediaDVDROM = 64, // DVD-ROM Media
            MediaCDAudioPlus = 65, // CD+ (Enhanced CD) Media
            MediaCDRW = 66, // CD-RW Media
            MediaCDR = 67, // CD-R Media
            MediaCDBurn = 68, // Burning CD
            MediaBlankCD = 69, // Blank CD Media
            MediaCDROM = 70, // CD-ROM Media
            AudioFiles = 71, // Audio Files
            ImageFiles = 72, // Image Files
            VideoFiles = 73, // Video Files
            MixedFiles = 74, // Mixed Files
            FolderBack = 75, // Folder back
            FolderFront = 76, // Folder front
            Shield = 77, // Security shield. Use for UAC prompts only.
            Warning = 78, // Warning
            Info = 79, // Informational
            Error = 80, // Error
            Key = 81, // Key / Secure
            Software = 82, // Software
            Rename = 83, // Rename
            Delete = 84, // Delete
            MediaAudioDVD = 85, // Audio DVD Media
            MediaMovieDVD = 86, // Movie DVD Media
            MediaEnhancedCD = 87, // Enhanced CD Media
            MediaEnhancedDVD = 88, // Enhanced DVD Media
            MediaHDDVD = 89, // HD-DVD Media
            MediaBluRay = 90, // BluRay Media
            MediaVCD = 91, // VCD Media
            MediaDVDPlusR = 92, // DVD+R Media
            MediaDVDPlusRW = 93, // DVD+RW Media
            DesktopPC = 94, // desktop computer
            MobilePC = 95, // mobile computer (laptop/notebook)
            Users = 96, // users
            MediaSmartMedia = 97, // Smart Media
            MediaCompactFlash = 98, // Compact Flash
            DeviceCellPhone = 99, // Cell phone
            DeviceCamera = 100, // Camera
            DeviceVideoCamera = 101, // Video camera
            DeviceAudioPlayer = 102, // Audio player
            NetworkConnect = 103, // Connect to Network
            Internet = 104, // InterNet
            ZipFile = 105, // ZIP File
            Settings = 106 // Settings
        }

        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct StockIconInfo
        {
            internal int StuctureSize;
            internal System.IntPtr Handle;
            internal int ImageIndex;
            internal int Identifier;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            internal string Path;
        }

        [DllImport(
            "Shell32.dll",
            CharSet = CharSet.Unicode,
            ExactSpelling = true,
            SetLastError = false
        )]
        internal static extern int SHGetStockIconInfo(
            StockIconIdentifier identifier,
            StockIconOptions flags,
            ref StockIconInfo info
        );

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern System.IntPtr ExtractIcon(
            System.IntPtr hInst,
            string lpszExeFileName,
            int nIconIndex
        );

        [DllImport("user32.dll")]
        public static extern bool DestroyIcon(System.IntPtr hIcon);

        [DllImport("user32.dll")]
        public static extern bool IsWindowEnabled(System.IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool EnableWindow(System.IntPtr hWnd, bool enable);

        [DllImport("user32.dll")]
        public static extern System.IntPtr GetFocus();

        [DllImport("user32.dll")]
        public static extern System.IntPtr SetFocus(System.IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern System.IntPtr GetDesktopWindow();

        [System.Flags]
        public enum SHGFI
        {
            SHGFI_ICON = 0x00000100,
            SHGFI_DISPLAYNAME = 0x00000200,
            SHGFI_TYPENAME = 0x00000400,
            SHGFI_ATTRIBUTES = 0x00000800,
            SHGFI_ICONLOCATION = 0x00001000,
            SHGFI_EXETYPE = 0x00002000,
            SHGFI_SYSICONINDEX = 0x00004000,
            SHGFI_LINKOVERLAY = 0x00008000,
            SHGFI_SELECTED = 0x00010000,
            SHGFI_ATTR_SPECIFIED = 0x00020000,
            SHGFI_LARGEICON = 0x00000000,
            SHGFI_SMALLICON = 0x00000001,
            SHGFI_OPENICON = 0x00000002,
            SHGFI_SHELLICONSIZE = 0x00000004,
            SHGFI_PIDL = 0x00000008,
            SHGFI_USEFILEATTRIBUTES = 0x00000010
        }

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        public static extern System.IntPtr SHGetFileInfo(
            string pszPath,
            uint dwFileAttributes,
            out SHFILEINFO psfi,
            int cbFileInfo,
            SHGFI uFlags
        );

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        public static extern System.IntPtr SHGetFileInfo(
            System.IntPtr pidl,
            uint dwFileAttributes,
            out SHFILEINFO psfi,
            int cbFileInfo,
            SHGFI uFlags
        );

        // Retrieves the path of a folder as an PIDL.
        [DllImport("shell32.dll")]
        public static extern uint SHGetFolderLocation(
            System.IntPtr hwndOwner, // Handle to the owner window.
            int nFolder, // A CSIDL value that identifies the folder to be located.
            System.IntPtr hToken, // Token that can be used to represent a particular user.
            uint dwReserved, // Reserved.
            out System.IntPtr ppidl
        ); // Address of a pointer to an item identifier list structure

        // Retrieves the path of a folder as an PIDL.
        [DllImport("shell32.dll")]
        public static extern uint SHGetSpecialFolderLocation(
            System.IntPtr hwndOwner, // Handle to the owner window.
            int nFolder, // A CSIDL value that identifies the folder to be located.
            out System.IntPtr ppidl
        ); // Address of a pointer to an item identifier list structure

        // specifying the folder's location relative to the root of the namespace
        // (the desktop).

        // Converts an item identifier list to a file system path.
        [DllImport("shell32.dll", CharSet = CharSet.Unicode, EntryPoint = "SHGetPathFromIDListW")]
        public static extern int SHGetPathFromIDList(
            System.IntPtr pidl, // Address of an item identifier list that specifies a file or directory location
            // relative to the root of the namespace (the desktop).
            System.Text.StringBuilder pszPath
        ); // Address of a buffer to receive the file system path.
        #region IL... functions usually found in SHELL32.DLL

        //	The following IL functions are documented to exist in SHELL32 starting with
        //	version 5.0, but I have found that on my Windows 2000 test environment, this
        //	is not the case; even trying to import the entry points through their ordinal
        //	does not work on Windows 2000. I therefore reimplemented the code myself.

        public static bool ILRemoveLastID(System.IntPtr pidl)
        {
            int len = ShellApi.ILGetSize(pidl);

            if (len > 2)
            {
                unsafe
                {
                    byte* ptr = (byte*)pidl.ToPointer();
                    byte* tail = ptr;

                    while (((ushort*)(ptr))[0] != 0)
                    {
                        tail = ptr;
                        ptr += ((ushort*)(ptr))[0];
                    }

                    tail[0] = 0;
                    tail[1] = 0;

                    return true;
                }
            }

            return false;
        }

        public static System.IntPtr ILCombine(
            System.IntPtr pidlAbsolute,
            System.IntPtr pidlRelative
        )
        {
            int l1 = ShellApi.ILGetSize(pidlAbsolute);
            int l2 = ShellApi.ILGetSize(pidlRelative);

            if (l1 >= 2)
            {
                l1 -= 2;
            }
            if (l2 >= 2)
            {
                l2 -= 2;
            }

            System.IntPtr p = Marshal.AllocCoTaskMem(l1 + l2 + 2);
            byte[] buffer = new byte[l1 + l2];

            if (p == System.IntPtr.Zero)
            {
                return p;
            }

            Marshal.WriteInt16(p, l1 + l2, 0);

            if (l1 > 0)
                Marshal.Copy(pidlAbsolute, buffer, 0, l1);
            if (l2 > 0)
                Marshal.Copy(pidlRelative, buffer, l1, l2);
            if (l1 + l2 > 0)
                Marshal.Copy(buffer, 0, p, l1 + l2);

            return p;
        }

        public static bool ILIsEqual(System.IntPtr pidlA, System.IntPtr pidlB)
        {
            int l1 = ShellApi.ILGetSize(pidlA);
            int l2 = ShellApi.ILGetSize(pidlB);

            if (l1 == l2)
            {
                if (l1 == 0)
                {
                    return true;
                }

                byte[] b1 = new byte[l1];
                byte[] b2 = new byte[l2];

                Marshal.Copy(pidlA, b1, 0, l1);
                Marshal.Copy(pidlB, b2, 0, l2);

                for (int i = 0; i < l1; i++)
                {
                    if (b1[i] != b2[i])
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        public static int ILGetSize(System.IntPtr pidl)
        {
            if (pidl == System.IntPtr.Zero)
            {
                return 0;
            }

            unsafe
            {
                byte* ptr = (byte*)pidl.ToPointer();
                int len = 2;

                while (((ushort*)(ptr))[0] != 0)
                {
                    len += ((ushort*)(ptr))[0];
                    ptr += ((ushort*)(ptr))[0];
                }

                return len;
            }
        }

        #endregion


        // Takes the CSIDL of a folder and returns the pathname.
        [DllImport("shell32.dll", CharSet = CharSet.Unicode, EntryPoint = "SHGetFolderPathW")]
        public static extern int SHGetFolderPath(
            System.IntPtr hwndOwner, // Handle to an owner window.
            int nFolder, // A CSIDL value that identifies the folder whose path is to be retrieved.
            System.IntPtr hToken, // An access token that can be used to represent a particular user.
            uint dwFlags, // Flags to specify which path is to be returned. It is used for cases where
            // the folder associated with a CSIDL may be moved or renamed by the user.
            System.Text.StringBuilder pszPath
        ); // Pointer to a null-terminated string which will receive the path.

        // Translates a Shell namespace object's display name into an item identifier list and returns the attributes
        // of the object. This function is the preferred method to convert a string to a pointer to an item
        // identifier list (PIDL).
        [DllImport("shell32.dll")]
        public static extern int SHParseDisplayName(
            [MarshalAs(UnmanagedType.LPWStr)] string pszName, // Pointer to a zero-terminated wide string that contains the display name
            // to parse.
            System.IntPtr pbc, // Optional bind context that controls the parsing operation. This parameter
            // is normally set to NULL.
            out System.IntPtr ppidl, // Address of a pointer to a variable of type ITEMIDLIST that receives the item
            // identifier list for the object.
            uint sfgaoIn, // ULONG value that specifies the attributes to query.
            out uint psfgaoOut
        ); // Pointer to a ULONG. On return, those attributes that are true for the

        // object and were requested in sfgaoIn will be set.


        // Retrieves the IShellFolder interface for the desktop folder, which is the root of the Shell's namespace.
        [DllImport("shell32.dll")]
        public static extern int SHGetDesktopFolder(out System.IntPtr ppshf); // Address that receives an IShellFolder interface pointer for the

        // desktop folder.

        // This function takes the fully-qualified pointer to an item identifier list (PIDL) of a namespace object,
        // and returns a specified interface pointer on the parent object.
        [DllImport("shell32.dll")]
        public static extern int SHBindToParent(
            System.IntPtr pidl, // The item's PIDL.
            [In] ref System.Guid riid, // The REFIID of one of the interfaces exposed by the item's parent object.
            out System.IntPtr ppv, // A pointer to the interface specified by riid. You must release the object when
            // you are finished.
            ref System.IntPtr ppidlLast
        ); // The item's PIDL relative to the parent folder. This PIDL can be used with many

        // of the methods supported by the parent folder's interfaces. If you set ppidlLast
        // to NULL, the PIDL will not be returned.

        // Accepts a STRRET structure returned by IShellFolder::GetDisplayNameOf that contains or points to a
        // string, and then returns that string as a BSTR.
        [DllImport("shlwapi.dll")]
        public static extern int StrRetToBSTR(
            ref STRRET pstr, // Pointer to a STRRET structure.
            System.IntPtr pidl, // Pointer to an ITEMIDLIST uniquely identifying a file object or subfolder relative
            // to the parent folder.
            [MarshalAs(UnmanagedType.BStr)]
                out string pbstr
        ); // Pointer to a variable of type BSTR that contains the converted string.

        // Takes a STRRET structure returned by IShellFolder::GetDisplayNameOf, converts it to a string, and
        // places the result in a buffer.
        [DllImport("shlwapi.dll")]
        public static extern int StrRetToBuf(
            ref STRRET pstr, // Pointer to the STRRET structure. When the function returns, this pointer will no
            // longer be valid.
            System.IntPtr pidl, // Pointer to the item's ITEMIDLIST structure.
            System.Text.StringBuilder pszBuf, // Buffer to hold the display name. It will be returned as a null-terminated
            // string. If cchBuf is too small, the name will be truncated to fit.
            uint cchBuf
        ); // Size of pszBuf, in characters. If cchBuf is too small, the string will be

        // truncated to fit.



        // Displays a dialog box that enables the user to select a Shell folder.
        [DllImport("shell32.dll")]
        public static extern System.IntPtr SHBrowseForFolder(ref BROWSEINFO lbpi); // Pointer to a BROWSEINFO structure that contains information used to display

        // the dialog box.

        // Performs an operation on a specified file.
        [DllImport("shell32.dll")]
        public static extern System.IntPtr ShellExecute(
            System.IntPtr hwnd, // Handle to a parent window.
            [MarshalAs(UnmanagedType.LPStr)] string lpOperation, // Pointer to a null-terminated string, referred to in this case as a verb,
            // that specifies the action to be performed.
            [MarshalAs(UnmanagedType.LPStr)]
                string lpFile, // Pointer to a null-terminated string that specifies the file or object on which
            // to execute the specified verb.
            [MarshalAs(UnmanagedType.LPStr)]
                string lpParameters, // If the lpFile parameter specifies an executable file, lpParameters is a pointer
            // to a null-terminated string that specifies the parameters to be passed
            // to the application.
            [MarshalAs(UnmanagedType.LPStr)]
                string lpDirectory, // Pointer to a null-terminated string that specifies the default directory.
            int nShowCmd
        ); // Flags that specify how an application is to be displayed when it is opened.

        // Performs an action on a file.
        [DllImport("shell32.dll")]
        public static extern int ShellExecuteEx(ref SHELLEXECUTEINFO lpExecInfo); // Address of a SHELLEXECUTEINFO structure that contains and receives

        // information about the application being executed.

        // Copies, moves, renames, or deletes a file system object.
        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        public static extern int SHFileOperation(ref SHFILEOPSTRUCT lpFileOp); // Address of an SHFILEOPSTRUCT structure that contains information

        // this function needs to carry out the specified operation. This
        // parameter must contain a valid value that is not NULL. You are
        // responsibile for validating the value. If you do not validate it,
        // you will experience unexpected results.

        // Notifies the system of an event that an application has performed. An application should use this function
        // if it performs an action that may affect the Shell.
        [DllImport("shell32.dll")]
        public static extern void SHChangeNotify(
            uint wEventId, // Describes the event that has occurred. the
            // ShellChangeNotificationEvents enum contains a list of options.
            uint uFlags, // Flags that indicate the meaning of the dwItem1 and dwItem2 parameters.
            System.IntPtr dwItem1, // First event-dependent value.
            System.IntPtr dwItem2
        ); // Second event-dependent value.

        public const uint SHARD_PIDL = 0x0001;
        public const uint SHARD_PATHW = 0x0003;

        // Adds a document to the Shell's list of recently used documents or clears all documents from the list.
        [DllImport("shell32.dll")]
        public static extern void SHAddToRecentDocs(
            uint uFlags, // Flag that indicates the meaning of the pv parameter.
            System.IntPtr pv
        ); // A pointer to either a null-terminated string with the path and file name

        // of the document, or a PIDL that identifies the document's file object.
        // Set this parameter to NULL to clear all documents from the list.
        [DllImport("shell32.dll")]
        public static extern void SHAddToRecentDocs(
            uint uFlags,
            [MarshalAs(UnmanagedType.LPWStr)] string pv
        );

        // Executes a command on a printer object.
        [DllImport("shell32.dll")]
        public static extern int SHInvokePrinterCommand(
            System.IntPtr hwnd, // Handle of the window that will be used as the parent of any windows
            // or dialog boxes that are created during the operation.
            uint uAction, // A value that determines the type of printer operation that will be
            // performed.
            [MarshalAs(UnmanagedType.LPWStr)]
                string lpBuf1, // Address of a null_terminated string that contains additional
            // information for the printer command.
            [MarshalAs(UnmanagedType.LPWStr)]
                string lpBuf2, // Address of a null-terminated string that contains additional
            // information for the printer command.
            int fModal
        ); //  value that determines whether SHInvokePrinterCommand should return

        // after initializing the command or wait until the command is completed.


        // Sends an appbar message to the system.
        [DllImport("shell32.dll")]
        public static extern uint SHAppBarMessage(
            uint dwMessage, // Appbar message value to send.
            ref APPBARDATA pData
        ); // Address of an APPBARDATA structure. The content of the structure

        // depends on the value set in the dwMessage parameter.


        // The RegisterWindowMessage function defines a new window message that is guaranteed to be unique throughout
        // the system. The message value can be used when sending or posting messages.
        [DllImport("user32.dll")]
        public static extern uint RegisterWindowMessage(
            [MarshalAs(UnmanagedType.LPTStr)] string lpstring
        ); // Pointer to a null-terminated string that specifies the message to be registered.

        // Instructs system edit controls to use AutoComplete to help complete URLs or
        // file system paths.
        [DllImport("shlwapi.dll")]
        public static extern int SHAutoComplete(System.IntPtr hwndEdit, uint dwFlags);

        [DllImport("User32.dll")]
        public static extern bool GetComboBoxInfo(System.IntPtr hwndCombo, ref ComboBoxInfo info);

        public static short GetHResultCode(int hr)
        {
            hr = hr & 0x0000ffff;
            return (short)hr;
        }

        public enum CSIDL
        {
            CSIDL_FLAG_CREATE = (0x8000), // Version 5.0. Combine this CSIDL with any of the following

            //CSIDLs to force the creation of the associated folder.
            CSIDL_ADMINTOOLS = (0x0030), // Version 5.0. The file system directory that is used to store

            // administrative tools for an individual user. The Microsoft
            // Management Console (MMC) will save customized consoles to
            // this directory, and it will roam with the user.
            CSIDL_ALTSTARTUP = (0x001d), // The file system directory that corresponds to the user's

            // nonlocalized Startup program group.
            CSIDL_APPDATA = (0x001a), // Version 4.71. The file system directory that serves as a

            // common repository for application-specific data. A typical
            // path is C:\Documents and Settings\username\Application Data.
            // This CSIDL is supported by the redistributable Shfolder.dll
            // for systems that do not have the Microsoft® Internet
            // Explorer 4.0 integrated Shell installed.
            CSIDL_BITBUCKET = (0x000a), // The virtual folder containing the objects in the user's

            // Recycle Bin.
            CSIDL_CDBURN_AREA = (0x003b), // Version 6.0. The file system directory acting as a staging

            // area for files waiting to be written to CD. A typical path
            // is C:\Documents and Settings\username\Local Settings\
            // Application Data\Microsoft\CD Burning.
            CSIDL_COMMON_ADMINTOOLS = (0x002f), // Version 5.0. The file system directory containing

            // administrative tools for all users of the computer.
            CSIDL_COMMON_ALTSTARTUP = (0x001e), // The file system directory that corresponds to the

            // nonlocalized Startup program group for all users. Valid only
            // for Microsoft Windows NT® systems.
            CSIDL_COMMON_APPDATA = (0x0023), // Version 5.0. The file system directory containing application

            // data for all users. A typical path is C:\Documents and
            // Settings\All Users\Application Data.
            CSIDL_COMMON_DESKTOPDIRECTORY = (0x0019), // The file system directory that contains files and folders

            // that appear on the desktop for all users. A typical path is
            // C:\Documents and Settings\All Users\Desktop. Valid only for
            // Windows NT systems.
            CSIDL_COMMON_DOCUMENTS = (0x002e), // The file system directory that contains documents that are

            // common to all users. A typical paths is C:\Documents and
            // Settings\All Users\Documents. Valid for Windows NT systems
            // and Microsoft Windows® 95 and Windows 98 systems with
            // Shfolder.dll installed.
            CSIDL_COMMON_FAVORITES = (0x001f), // The file system directory that serves as a common repository

            // for favorite items common to all users. Valid only for
            // Windows NT systems.
            CSIDL_COMMON_MUSIC = (0x0035), // Version 6.0. The file system directory that serves as a

            // repository for music files common to all users. A typical
            // path is C:\Documents and Settings\All Users\Documents\
            // My Music.
            CSIDL_COMMON_PICTURES = (0x0036), // Version 6.0. The file system directory that serves as a

            // repository for image files common to all users. A typical
            // path is C:\Documents and Settings\All Users\Documents\
            // My Pictures.
            CSIDL_COMMON_PROGRAMS = (0x0017), // The file system directory that contains the directories for

            // the common program groups that appear on the Start menu for
            // all users. A typical path is C:\Documents and Settings\
            // All Users\Start Menu\Programs. Valid only for Windows NT
            // systems.
            CSIDL_COMMON_STARTMENU = (0x0016), // The file system directory that contains the programs and

            // folders that appear on the Start menu for all users. A
            // typical path is C:\Documents and Settings\All Users\
            // Start Menu. Valid only for Windows NT systems.
            CSIDL_COMMON_STARTUP = (0x0018), // The file system directory that contains the programs that

            // appear in the Startup folder for all users. A typical path
            // is C:\Documents and Settings\All Users\Start Menu\Programs\
            // Startup. Valid only for Windows NT systems.
            CSIDL_COMMON_TEMPLATES = (0x002d), // The file system directory that contains the templates that

            // are available to all users. A typical path is C:\Documents
            // and Settings\All Users\Templates. Valid only for Windows
            // NT systems.
            CSIDL_COMMON_VIDEO = (0x0037), // Version 6.0. The file system directory that serves as a

            // repository for video files common to all users. A typical
            // path is C:\Documents and Settings\All Users\Documents\
            // My Videos.
            CSIDL_CONTROLS = (0x0003), // The virtual folder containing icons for the Control Panel

            // applications.
            CSIDL_COOKIES = (0x0021), // The file system directory that serves as a common repository

            // for Internet cookies. A typical path is C:\Documents and
            // Settings\username\Cookies.
            CSIDL_DESKTOP = (0x0000), // The virtual folder representing the Windows desktop, the root

            // of the namespace.
            CSIDL_DESKTOPDIRECTORY = (0x0010), // The file system directory used to physically store file

            // objects on the desktop (not to be confused with the desktop
            // folder itself). A typical path is C:\Documents and
            // Settings\username\Desktop.
            CSIDL_DRIVES = (0x0011), // The virtual folder representing My Computer, containing

            // everything on the local computer: storage devices, printers,
            // and Control Panel. The folder may also contain mapped
            // network drives.
            CSIDL_FAVORITES = (0x0006), // The file system directory that serves as a common repository

            // for the user's favorite items. A typical path is C:\Documents
            // and Settings\username\Favorites.
            CSIDL_FONTS = (0x0014), // A virtual folder containing fonts. A typical path is

            // C:\Windows\Fonts.
            CSIDL_HISTORY = (0x0022), // The file system directory that serves as a common repository

            // for Internet history items.
            CSIDL_INTERNET = (0x0001), // A virtual folder representing the Internet.
            CSIDL_INTERNET_CACHE = (0x0020), // Version 4.72. The file system directory that serves as a

            // common repository for temporary Internet files. A typical
            // path is C:\Documents and Settings\username\Local Settings\
            // Temporary Internet Files.
            CSIDL_LOCAL_APPDATA = (0x001c), // Version 5.0. The file system directory that serves as a data

            // repository for local (nonroaming) applications. A typical
            // path is C:\Documents and Settings\username\Local Settings\
            // Application Data.
            CSIDL_MYDOCUMENTS = (0x000c), // Version 6.0. The virtual folder representing the My Documents

            // desktop item. This should not be confused with
            // CSIDL_PERSONAL, which represents the file system folder that
            // physically stores the documents.
            CSIDL_MYMUSIC = (0x000d), // The file system directory that serves as a common repository

            // for music files. A typical path is C:\Documents and Settings
            // \User\My Documents\My Music.
            CSIDL_MYPICTURES = (0x0027), // Version 5.0. The file system directory that serves as a

            // common repository for image files. A typical path is
            // C:\Documents and Settings\username\My Documents\My Pictures.
            CSIDL_MYVIDEO = (0x000e), // Version 6.0. The file system directory that serves as a

            // common repository for video files. A typical path is
            // C:\Documents and Settings\username\My Documents\My Videos.
            CSIDL_NETHOOD = (0x0013), // A file system directory containing the link objects that may

            // exist in the My Network Places virtual folder. It is not the
            // same as CSIDL_NETWORK, which represents the network namespace
            // root. A typical path is C:\Documents and Settings\username\
            // NetHood.
            CSIDL_NETWORK = (0x0012), // A virtual folder representing Network Neighborhood, the root

            // of the network namespace hierarchy.
            CSIDL_PERSONAL = (0x0005), // The file system directory used to physically store a user's

            // common repository of documents. A typical path is
            // C:\Documents and Settings\username\My Documents. This should
            // be distinguished from the virtual My Documents folder in
            // the namespace, identified by CSIDL_MYDOCUMENTS.
            CSIDL_PRINTERS = (0x0004), // The virtual folder containing installed printers.
            CSIDL_PRINTHOOD = (0x001b), // The file system directory that contains the link objects that

            // can exist in the Printers virtual folder. A typical path is
            // C:\Documents and Settings\username\PrintHood.
            CSIDL_PROFILE = (0x0028), // Version 5.0. The user's profile folder. A typical path is

            // C:\Documents and Settings\username. Applications should not
            // create files or folders at this level; they should put their
            // data under the locations referred to by CSIDL_APPDATA or
            // CSIDL_LOCAL_APPDATA.
            CSIDL_PROFILES = (0x003e), // Version 6.0. The file system directory containing user

            // profile folders. A typical path is C:\Documents and Settings.
            CSIDL_PROGRAM_FILES = (0x0026), // Version 5.0. The Program Files folder. A typical path is

            // C:\Program Files.
            CSIDL_PROGRAM_FILES_COMMON = (0x002b), // Version 5.0. A folder for components that are shared across

            // applications. A typical path is C:\Program Files\Common.
            // Valid only for Windows NT, Windows 2000, and Windows XP
            // systems. Not valid for Windows Millennium Edition
            // (Windows Me).
            CSIDL_PROGRAMS = (0x0002), // The file system directory that contains the user's program

            // groups (which are themselves file system directories).
            // A typical path is C:\Documents and Settings\username\
            // Start Menu\Programs.
            CSIDL_RECENT = (0x0008), // The file system directory that contains shortcuts to the

            // user's most recently used documents. A typical path is
            // C:\Documents and Settings\username\My Recent Documents.
            // To create a shortcut in this folder, use SHAddToRecentDocs.
            // In addition to creating the shortcut, this function updates
            // the Shell's list of recent documents and adds the shortcut
            // to the My Recent Documents submenu of the Start menu.
            CSIDL_SENDTO = (0x0009), // The file system directory that contains Send To menu items.

            // A typical path is C:\Documents and Settings\username\SendTo.
            CSIDL_STARTMENU = (0x000b), // The file system directory containing Start menu items. A

            // typical path is C:\Documents and Settings\username\Start Menu.
            CSIDL_STARTUP = (0x0007), // The file system directory that corresponds to the user's

            // Startup program group. The system starts these programs
            // whenever any user logs onto Windows NT or starts Windows 95.
            // A typical path is C:\Documents and Settings\username\
            // Start Menu\Programs\Startup.
            CSIDL_SYSTEM = (0x0025), // Version 5.0. The Windows System folder. A typical path is

            // C:\Windows\System32.
            CSIDL_TEMPLATES = (0x0015), // The file system directory that serves as a common repository

            // for document templates. A typical path is C:\Documents
            // and Settings\username\Templates.
            CSIDL_WINDOWS = (0x0024), // Version 5.0. The Windows directory or SYSROOT. This
            // corresponds to the %windir% or %SYSTEMROOT% environment
            // variables. A typical path is C:\Windows.
        }

        public enum SHGFP_TYPE
        {
            SHGFP_TYPE_CURRENT = 0, // current value for user, verify it exists
            SHGFP_TYPE_DEFAULT = 1 // default value, may not exist
        }

        public enum SFGAO : uint
        {
            SFGAO_CANCOPY = 0x00000001, // Objects can be copied
            SFGAO_CANMOVE = 0x00000002, // Objects can be moved
            SFGAO_CANLINK = 0x00000004, // Objects can be linked
            SFGAO_STORAGE = 0x00000008, // supports BindToObject(IID_IStorage)
            SFGAO_CANRENAME = 0x00000010, // Objects can be renamed
            SFGAO_CANDELETE = 0x00000020, // Objects can be deleted
            SFGAO_HASPROPSHEET = 0x00000040, // Objects have property sheets
            SFGAO_DROPTARGET = 0x00000100, // Objects are drop target
            SFGAO_ENCRYPTED = 0x00002000, // object is encrypted (use alt color)
            SFGAO_ISSLOW = 0x00004000, // 'slow' object
            SFGAO_GHOSTED = 0x00008000, // ghosted icon
            SFGAO_LINK = 0x00010000, // Shortcut (link)
            SFGAO_SHARE = 0x00020000, // shared
            SFGAO_READONLY = 0x00040000, // read-only
            SFGAO_HIDDEN = 0x00080000, // hidden object
            SFGAO_NONENUMERATED = 0x00100000, // is a non-enumerated object
            SFGAO_NEWCONTENT = 0x00200000, // should show bold in explorer tree
            SFGAO_STREAM = 0x00400000, // supports BindToObject(IID_IStream)
            SFGAO_STORAGEANCESTOR = 0x00800000, // may contain children with SFGAO_STORAGE or SFGAO_STREAM
            SFGAO_VALIDATE = 0x01000000, // invalidate cached information
            SFGAO_REMOVABLE = 0x02000000, // is this removeable media?
            SFGAO_COMPRESSED = 0x04000000, // Object is compressed (use alt color)
            SFGAO_BROWSABLE = 0x08000000, // supports IShellFolder, but only implements CreateViewObject() (non-folder view)
            SFGAO_FILESYSANCESTOR = 0x10000000, // may contain children with SFGAO_FILESYSTEM
            SFGAO_FOLDER = 0x20000000, // support BindToObject(IID_IShellFolder)
            SFGAO_FILESYSTEM = 0x40000000, // is a win32 file system object (file/folder/root)
            SFGAO_HASSUBFOLDER = 0x80000000, // may contain children with SFGAO_FOLDER

            SFGAO_CAPABILITYMASK = 0x00000177, // This flag is a mask for the capability flags.
            SFGAO_DISPLAYATTRMASK = 0x000FC000, // This flag is a mask for the display attributes.
            SFGAO_STORAGECAPMASK = 0x70C50008, // This flag is a mask for determining storage capabilities, ie for open/save semantics
            SFGAO_CONTENTSMASK = 0x80000000, // This flag is a mask for the contents attributes.
        }

        public enum SHCONT
        {
            SHCONTF_FOLDERS = 0x0020, // only want folders enumerated (SFGAO_FOLDER)
            SHCONTF_NONFOLDERS = 0x0040, // include non folders
            SHCONTF_INCLUDEHIDDEN = 0x0080, // show items normally hidden
            SHCONTF_INIT_ON_FIRST_NEXT = 0x0100, // allow EnumObject() to return before validating enum
            SHCONTF_NETPRINTERSRCH = 0x0200, // hint that client is looking for printers
            SHCONTF_SHAREABLE = 0x0400, // hint that client is looking sharable resources (remote shares)
            SHCONTF_STORAGE = 0x0800, // include all items with accessible storage and their ancestors
        }

        public enum SHCIDS : uint
        {
            SHCIDS_ALLFIELDS = 0x80000000, // Compare all the information contained in the ITEMIDLIST

            // structure, not just the display names
            SHCIDS_CANONICALONLY = 0x10000000, // When comparing by name, compare the system names but not the

            // display names.
            SHCIDS_BITMASK = 0xFFFF0000,
            SHCIDS_COLUMNMASK = 0x0000FFFF
        }

        public enum SHGNO
        {
            SHGDN_NORMAL = 0x0000, // default (display purpose)
            SHGDN_INFOLDER = 0x0001, // displayed under a folder (relative)
            SHGDN_FOREDITING = 0x1000, // for in-place editing
            SHGDN_FORADDRESSBAR = 0x4000, // UI friendly parsing name (remove ugly stuff)
            SHGDN_FORPARSING = 0x8000 // parsing name for ParseDisplayName()
        }

        public enum STRRET_TYPE
        {
            STRRET_WSTR = 0x0000, // Use STRRET.pOleStr
            STRRET_OFFSET = 0x0001, // Use STRRET.uOffset to Ansi
            STRRET_CSTR = 0x0002 // Use STRRET.cStr
        }

        public enum PrinterActions
        {
            PRINTACTION_OPEN = 0, // The printer specified by the name in lpBuf1 will be opened.

            // lpBuf2 is ignored.
            PRINTACTION_PROPERTIES = 1, // The properties for the printer specified by the name in lpBuf1

            // will be displayed. lpBuf2 can either be NULL or specify.
            PRINTACTION_NETINSTALL = 2, // The network printer specified by the name in lpBuf1 will be

            // installed. lpBuf2 is ignored.
            PRINTACTION_NETINSTALLLINK = 3, // A shortcut to the network printer specified by the name in lpBuf1

            // will be created. lpBuf2 specifies the drive and path of the folder
            // in which the shortcut will be created. The network printer must
            // have already been installed on the local computer.
            PRINTACTION_TESTPAGE = 4, // A test page will be printed on the printer specified by the name

            // in lpBuf1. lpBuf2 is ignored.
            PRINTACTION_OPENNETPRN = 5, // The network printer specified by the name in lpBuf1 will be

            // opened. lpBuf2 is ignored.
            PRINTACTION_DOCUMENTDEFAULTS = 6, // Microsoft® Windows NT® only. The default document properties for

            // the printer specified by the name in lpBuf1 will be displayed.
            // lpBuf2 is ignored.
            PRINTACTION_SERVERPROPERTIES = 7 // Windows NT only. The properties for the server of the printer
            // specified by the name in lpBuf1 will be displayed. lpBuf2
            // is ignored.
        }
    }
}
