//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Requests
{
	/// <summary>
	/// L'�num�ration ExecutionState d�crit l'�tat d'une requ�te dans la queue
	/// d'ex�cution des requ�tes (voir ExecutionQueue/ExecutionEngine); utilis�
	/// dans une machine d'�tat.
	/// </summary>
	public enum ExecutionState
	{
		Pending,								//	en attente, ex�cution locale
		Conflicting,							//	en attente, conflit � r�soudre
		ExecutedByClient,						//	ex�cution par le client OK
		SentToServer,							//	envoi au serveur OK
		ExecutedByServer,						//	ex�cution par le serveur OK
	}
	
	//	Les transitions suivantes sont possibles :
	//
	//		Pending -----> | -> ExecutedByClient -> SentToServer -> | -> ExecutedByServer
	//		               |                                        |
	//		Conflicting -> | -> Conflicting                         | -> Conflicting
}
