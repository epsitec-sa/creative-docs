//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Requests
{
	/// <summary>
	/// L'énumération ExecutionState décrit l'état d'une requête dans la queue
	/// d'exécution des requêtes (voir ExecutionQueue/ExecutionEngine); utilisé
	/// dans une machine d'état.
	/// </summary>
	public enum ExecutionState : short
	{
		Pending				= 0,				//	en attente, exécution locale
		Conflicting			= 1,				//	en attente, conflit à résoudre
		ConflictResolved	= 2,				//	en attente, conflit résolu
		ExecutedByClient	= 3,				//	exécution par le client OK
		SentToServer		= 4,				//	envoi au serveur OK
		ExecutedByServer	= 5,				//	exécution par le serveur OK
	}
	
	//	Les transitions suivantes sont possibles :
	//
	//		Pending ----------> | -> ExecutedByClient -> SentToServer -> | -> ExecutedByServer
	//		                    |                                        |
	//		ConflictResolved -> | -> Conflicting -> ConflictResolved     | -> Conflicting -> ConflictResolved
}
