//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	public enum DbRowStatus
	{
		Live			= 0,				//	fiche standard
		Copied			= 1,				//	fiche propre, fraîchement copiée
		ArchiveCopy		= 2,				//	fiche archive
		Deleted			= 3,				//	fiche supprimée
	}
}
