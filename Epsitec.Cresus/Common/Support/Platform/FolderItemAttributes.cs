//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Support.Platform
{
	[System.Flags]
	internal enum FolderItemAttributes
	{
		None			= 0,

		Browsable		= 0x0001,
		CanCopy			= 0x0002,
		CanDelete		= 0x0004,
		CanMove			= 0x0008,
		CanRename		= 0x0010,
		Compressed		= 0x0020,
		Encrypted		= 0x0040,
		Hidden			= 0x0080,
		Shortcut		= 0x0100,
		ReadOnly		= 0x0200,
		Shared			= 0x0400,
		Folder			= 0x0800,
		FileSystemNode	= 0x1000,
		WebLink			= 0x2000
	}
}
