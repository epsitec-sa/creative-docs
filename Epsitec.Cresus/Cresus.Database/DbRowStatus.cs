//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	public enum DbRowStatus
	{
		Live			= 0,				//	fiche standard
		Clean			= 1,				//	fiche propre, fraîchement archivée
		Archive			= 2,				//	fiche archive
		Deleted			= 3,				//	fiche supprimée
	}
	
	[System.Flags]
	public enum DbRowSearchMode
	{
		All				= Live | Clean | Archive | Deleted,
		
		Live			= (1 << DbRowStatus.Live),
		Clean			= (1 << DbRowStatus.Clean),
		Archive			= (1 << DbRowStatus.Archive),
		Deleted			= (1 << DbRowStatus.Deleted),
		
		LiveOrClean		= Live | Clean,
		NotDeleted		= Live | Clean | Archive,
	}
}
