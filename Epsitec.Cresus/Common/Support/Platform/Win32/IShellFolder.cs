//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Runtime.InteropServices;

namespace Epsitec.Common.Support.Platform.Win32
{
	[ComImport]
	[InterfaceType (ComInterfaceType.InterfaceIsIUnknown)]
	[Guid ("000214E6-0000-0000-C000-000000000046")]
	
	internal interface IShellFolder
	{
		// Translates a file object's or folder's display name into an item identifier list.
		// Return value: error code, if any
		[PreserveSig]
		int ParseDisplayName(
			System.IntPtr hwnd,				// Optional window handle
			System.IntPtr pbc,					// Optional bind context that controls the parsing operation. This parameter is 
			// normally set to NULL. 
			[MarshalAs (UnmanagedType.LPWStr)] 
			string pszDisplayName,		// Null-terminated UNICODE string with the display name.
		   ref uint pchEaten,		// Pointer to a ULONG value that receives the number of characters of the 
			// display name that was parsed.
		   out System.IntPtr ppidl,			// Pointer to an ITEMIDLIST pointer that receives the item identifier list for 
			// the object.
			ref uint pdwAttributes);	// Optional parameter that can be used to query for file attributes.
		// this can be values from the SFGAO enum

		// Allows a client to determine the contents of a folder by creating an item identifier enumeration object 
		// and returning its IEnumIDList interface.
		// Return value: error code, if any
		[PreserveSig]
		int EnumObjects(
			System.IntPtr hwnd,				// If user input is required to perform the enumeration, this window handle 
			// should be used by the enumeration object as the parent window to take 
			// user input.
			int grfFlags,				// Flags indicating which items to include in the enumeration. For a list 
			// of possible values, see the SHCONTF enum. 
			out System.IntPtr ppenumIDList);	// Address that receives a pointer to the IEnumIDList interface of the 
		// enumeration object created by this method. 

		// Retrieves an IShellFolder object for a subfolder.
		// Return value: error code, if any
		[PreserveSig]
		int BindToObject(
			System.IntPtr pidl,				// Address of an ITEMIDLIST structure (PIDL) that identifies the subfolder.
			System.IntPtr pbc,					// Optional address of an IBindCtx interface on a bind context object to be 
			// used during this operation.
			[In]
			ref System.Guid riid,				// Identifier of the interface to return. 
		   out System.IntPtr ppv);			// Address that receives the interface pointer.

		// Requests a pointer to an object's storage interface. 
		// Return value: error code, if any
		[PreserveSig]
		int BindToStorage(
			System.IntPtr pidl,				// Address of an ITEMIDLIST structure that identifies the subfolder relative 
			// to its parent folder. 
			System.IntPtr pbc,					// Optional address of an IBindCtx interface on a bind context object to be 
			// used during this operation.
			[In]
			ref System.Guid riid,				// Interface identifier (IID) of the requested storage interface.
		   out System.IntPtr ppv);			// Address that receives the interface pointer specified by riid.

		// Determines the relative order of two file objects or folders, given their item identifier lists.
		// Return value: If this method is successful, the CODE field of the HRESULT contains one of the following 
		// values (the code can be retrived using the helper function GetHResultCode):
		// Negative A negative return value indicates that the first item should precede the second (pidl1 < pidl2). 
		// Positive A positive return value indicates that the first item should follow the second (pidl1 > pidl2). 
		// Zero A return value of zero indicates that the two items are the same (pidl1 = pidl2). 
		[PreserveSig]
		int CompareIDs(
			int lParam,				// Value that specifies how the comparison should be performed. The lower 
			// sixteen bits of lParam define the sorting rule. The upper sixteen bits of 
			// lParam are used for flags that modify the sorting rule. values can be from 
			// the SHCIDS enum
			System.IntPtr pidl1,				// Pointer to the first item's ITEMIDLIST structure.
			System.IntPtr pidl2);				// Pointer to the second item's ITEMIDLIST structure.

		// Requests an object that can be used to obtain information from or interact with a folder object.
		// Return value: error code, if any
		[PreserveSig]
		int CreateViewObject(
			System.IntPtr hwndOwner,			// Handle to the owner window.
			[In]
			ref System.Guid riid,				// Identifier of the requested interface. 
		   out System.IntPtr ppv);			// Address of a pointer to the requested interface. 

		// Retrieves the attributes of one or more file objects or subfolders. 
		// Return value: error code, if any
		[PreserveSig]
		int GetAttributesOf(
			uint cidl,				// Number of file objects from which to retrieve attributes. 

			[MarshalAs (UnmanagedType.LPArray, SizeParamIndex=0)]
			System.IntPtr[] apidl,				// Address of an array of pointers to ITEMIDLIST structures, each of which 
			// uniquely identifies a file object relative to the parent folder.
		   ref uint rgfInOut);		// Address of a single ULONG value that, on entry, contains the attributes that 
		// the caller is requesting. On exit, this value contains the requested 
		// attributes that are common to all of the specified objects. this value can
		// be from the SFGAO enum

		// Retrieves an OLE interface that can be used to carry out actions on the specified file objects or folders.
		// Return value: error code, if any
		[PreserveSig]
		int GetUIObjectOf(
			System.IntPtr hwndOwner,			// Handle to the owner window that the client should specify if it displays 
			// a dialog box or message box.
			uint cidl,				// Number of file objects or subfolders specified in the apidl parameter. 
			System.IntPtr[] apidl,				// Address of an array of pointers to ITEMIDLIST structures, each of which 
			// uniquely identifies a file object or subfolder relative to the parent folder.
			[In]
			ref System.Guid riid,				// Identifier of the COM interface object to return.
		   ref uint rgfReserved,		// Reserved. 
		   out System.IntPtr ppv);			// Pointer to the requested interface.

		// Retrieves the display name for the specified file object or subfolder. 
		// Return value: error code, if any
		[PreserveSig]
		int GetDisplayNameOf(
			System.IntPtr pidl,				// Address of an ITEMIDLIST structure (PIDL) that uniquely identifies the file 
			// object or subfolder relative to the parent folder. 
			uint uFlags,				// Flags used to request the type of display name to return. For a list of 
			// possible values, see the SHGNO enum. 
			out ShellApi.STRRET pName);			// Address of a STRRET structure in which to return the display name.

		// Sets the display name of a file object or subfolder, changing the item identifier in the process.
		// Return value: error code, if any
		[PreserveSig]
		int SetNameOf(
			System.IntPtr hwnd,				// Handle to the owner window of any dialog or message boxes that the client 
			// displays.
			System.IntPtr pidl,				// Pointer to an ITEMIDLIST structure that uniquely identifies the file object
			// or subfolder relative to the parent folder. 
			[MarshalAs (UnmanagedType.LPWStr)] 
			string pszName,				// Pointer to a null-terminated string that specifies the new display name. 
		   uint uFlags,				// Flags indicating the type of name specified by the lpszName parameter. For 
			// a list of possible values, see the description of the SHGNO enum. 
		   out System.IntPtr ppidlOut);		// Address of a pointer to an ITEMIDLIST structure which receives the new ITEMIDLIST. 
	}
}