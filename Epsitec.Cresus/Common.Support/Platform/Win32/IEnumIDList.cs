using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;

namespace Epsitec.Common.Support.Platform.Win32
{
	[ComImport]
	[Guid("000214F2-0000-0000-C000-000000000046")]
	[InterfaceType (ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IEnumIDList
	{
		[PreserveSig]
		int Next(int count, out IntPtr elementPidl, out uint fetched);
		[PreserveSig]
		void Skip(int count);
		[PreserveSig]
		void Reset();
		[PreserveSig]
		void Clone(ref IEnumIDList enumIDList);
	}
}
