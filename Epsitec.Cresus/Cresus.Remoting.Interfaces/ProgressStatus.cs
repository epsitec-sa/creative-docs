//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Remoting
{
	/// <summary>
	/// Summary description for ProgressStatus.
	/// </summary>
	public enum ProgressStatus
	{
		None,
		
		Running		= 1,		//	en cours d'ex�cution
		
		Succeeded	= 2,		//	termin�, l'op�ration a �t� couronn�e de succ�s
		Cancelled	= 3,		//	termin�, l'op�ration a �t� annul�e
		Failed		= 4,		//	termin�, l'op�ration a �chou�
	}
}
