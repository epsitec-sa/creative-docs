//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	public enum DbRowStatus
	{
		Live			= 0,				//	ligne standard
		Clean			= 1,				//	ligne propre, fra�chement cl�n�e
		Deleted			= 2,				//	ligne supprim�e
		
		TempLive		= 64,				//	ligne temporaire, standard
		TempClean		= 65,				//	ligne temporaire, propre
	}
}
