//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Requests
{
	/// <summary>
	/// L'�num�ration ExecutionState d�crit l'�tat d'une requ�te dans la queue
	/// d'ex�cution des requ�tes (voir ExecutionQueue/ExecutionEngine); utilis�
	/// dans une machine d'�tat.
	/// </summary>
	public enum ExecutionState : short
	{
		Pending				= 0,				//	en attente, ex�cution locale
		Conflicting			= 1,				//	en attente, conflit � r�soudre
		ConflictResolved	= 2,				//	en attente, conflit r�solu
		ExecutedByClient	= 3,				//	ex�cution par le client OK
		SentToServer		= 4,				//	envoi au serveur OK
		ExecutedByServer	= 5,				//	ex�cution par le serveur OK
	}
	
	//	Les transitions suivantes sont possibles :
	//
	//		Pending ----------> | -> ExecutedByClient -> SentToServer -> | -> ExecutedByServer
	//		                    |                                        |
	//		ConflictResolved -> | -> Conflicting -> ConflictResolved     | -> Conflicting -> ConflictResolved
}
