//	Copyright � 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
