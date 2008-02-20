//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Runtime.InteropServices;

namespace Epsitec.Common.Support.Platform.Win32
{
	internal static class ShellGuids
	{
		public static System.Guid IID_IMalloc					= new System.Guid ("{00000002-0000-0000-C000-000000000046}");
		public static System.Guid IID_IShellFolder				= new System.Guid ("{000214E6-0000-0000-C000-000000000046}");
		public static System.Guid IID_IShellLink				= new System.Guid ("{000214F9-0000-0000-C000-000000000046}");
		public static System.Guid IID_IFolderFilterSite			= new System.Guid ("{C0A651F5-B48B-11d2-B5ED-006097C686F6}");
		public static System.Guid IID_IFolderFilter				= new System.Guid ("{9CC22886-DC8E-11d2-B1D0-00C04F8EEB3E}");

		//	AutoComplete System.Guids :
		
		public static System.Guid IID_IAutoCompList				= new System.Guid ("{00BB2760-6A77-11D0-A535-00C04FD7D062}");
		public static System.Guid IID_IObjMgr					= new System.Guid ("{00BB2761-6A77-11D0-A535-00C04FD7D062}");
		public static System.Guid IID_IACList					= new System.Guid ("{77A130B0-94FD-11D0-A544-00C04FD7D062}");
		public static System.Guid IID_IACList2					= new System.Guid ("{470141A0-5186-11D2-BBB6-0060977B464C}");
		public static System.Guid IID_ICurrentWorkingDirectory	= new System.Guid ("{91956D21-9276-11D1-921A-006097DF5BD4}");
		
		public static System.Guid CLSID_AutoComplete			= new System.Guid ("{00BB2763-6A77-11D0-A535-00C04FD7D062}");
		public static System.Guid CLSID_ACLHistory				= new System.Guid ("{00BB2764-6A77-11D0-A535-00C04FD7D062}");
		public static System.Guid CLSID_ACListISF				= new System.Guid ("{03C036F1-A186-11D0-824A-00AA005B4383}");
		public static System.Guid CLSID_ACLMRU					= new System.Guid ("{6756A641-dE71-11D0-831B-00AA005B4383}");
		public static System.Guid CLSID_ACLMulti				= new System.Guid ("{00BB2765-6A77-11D0-A535-00C04FD7D062}");
		public static System.Guid CLSID_ACLCustomMRU			= new System.Guid ("{6935DB93-21E8-4CCC-BEB9-9fE3C77A297A}");

	}
}