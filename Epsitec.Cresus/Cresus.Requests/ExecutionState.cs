//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Requests
{
	/// <summary>
	/// L'énumération ExecutionState décrit l'état d'une requête dans la queue
	/// d'exécution des requêtes (voir ExecutionQueue/ExecutionEngine); utilisé
	/// dans une machine d'état.
	/// </summary>
	public enum ExecutionState
	{
		Pending,								//	en attente, exécution locale
		Conflicting,							//	en attente, conflit à résoudre
		ExecutedByClient,						//	exécution par le client OK
		SentToServer,							//	envoi au serveur OK
		ExecutedByServer,						//	exécution par le serveur OK
	}
	
	//	Les transitions suivantes sont possibles :
	//
	//		Pending -----> | -> ExecutedByClient -> SentToServer -> | -> ExecutedByServer
	//		               |                                        |
	//		Conflicting -> | -> Conflicting                         | -> Conflicting
}
