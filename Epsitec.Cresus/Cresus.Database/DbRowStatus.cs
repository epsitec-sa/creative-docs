//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	public enum DbRowStatus
	{
		Live			= 0,				//	ligne standard
		Clean			= 1,				//	ligne propre, fra�chement cl�n�e
		
		TempLive		= 8,				//	ligne temporaire, standard
		TempClean		= 9,				//	ligne temporaire, propre
		
		//	Attention: tout ce qui a un code inf�rieur � DbRowStatus.Deleted est
		//	consid�r� comme "live", donc visible par les requ�tes normales :
		
		Deleted			= 64,				//	ligne supprim�e
	}
}
