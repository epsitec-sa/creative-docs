//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	[System.Flags]
	public enum DbRowSearchMode
	{
		All				= Live | Copied | ArchiveCopy | Deleted,
		
		Live			= (1 << DbRowStatus.Live),
		Copied			= (1 << DbRowStatus.Copied),
		ArchiveCopy		= (1 << DbRowStatus.ArchiveCopy),
		Deleted			= (1 << DbRowStatus.Deleted),
		
		LiveActive		= Live | Copied,
		LiveAll			= Live | Copied | ArchiveCopy,
	}
}
