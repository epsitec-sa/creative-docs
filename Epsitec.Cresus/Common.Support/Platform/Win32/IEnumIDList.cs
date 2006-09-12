//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Runtime.InteropServices;

namespace Epsitec.Common.Support.Platform.Win32
{
	[ComImport]
	[Guid("000214F2-0000-0000-C000-000000000046")]
	[InterfaceType (ComInterfaceType.InterfaceIsIUnknown)]
	
	internal interface IEnumIDList
	{
		[PreserveSig]
		int Next(int count, out System.IntPtr elementPidl, out uint fetched);
		[PreserveSig]
		void Skip(int count);
		[PreserveSig]
		void Reset();
		[PreserveSig]
		void Clone(ref IEnumIDList enumIDList);
	}
}
