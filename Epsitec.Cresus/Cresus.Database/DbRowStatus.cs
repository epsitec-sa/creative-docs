//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	public enum DbRowStatus
	{
		Live			= 0,				//	fiche standard
		Clean			= 1,				//	fiche propre, fraîchement archivée
		Archive			= 2,				//	fiche archive
		
		//	Attention: tout ce qui a un code inférieur à DbRowStatus.Deleted est
		//	considéré comme "actif", donc visible par les requêtes normales :
		
		Deleted			= 64,				//	ligne supprimée
	}
}
