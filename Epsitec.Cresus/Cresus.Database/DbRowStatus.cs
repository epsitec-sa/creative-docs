//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	public enum DbRowStatus
	{
		Live			= 0,				//	fiche standard
		Clean			= 1,				//	fiche propre, fra�chement archiv�e
		Archive			= 2,				//	fiche archive
		
		//	Attention: tout ce qui a un code inf�rieur � DbRowStatus.Deleted est
		//	consid�r� comme "actif", donc visible par les requ�tes normales :
		
		Deleted			= 64,				//	ligne supprim�e
	}
}
