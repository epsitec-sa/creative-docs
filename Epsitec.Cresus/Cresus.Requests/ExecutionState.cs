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
		ExecutedByClient	= 2,				//	ex�cution par le client OK
		SentToServer		= 3,				//	envoi au serveur OK
		ExecutedByServer	= 4,				//	ex�cution par le serveur OK
	}
	
	//	Les transitions suivantes sont possibles :
	//
	//		Pending -----> | -> ExecutedByClient -> SentToServer -> | -> ExecutedByServer
	//		               |                                        |
	//		Conflicting -> | -> Conflicting                         | -> Conflicting
}
