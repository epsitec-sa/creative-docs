//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Requests
{
	/// <summary>
	/// L'énumération OrchestratorState décrit l'état de l'Orchestrator.
	/// </summary>
	public enum OrchestratorState
	{
		Ready				= 0,				//	prêt, rien à faire pour le moment
		Conflicting			= 1,				//	prêt, en attente de la résolution d'un conflit
		Processing			= 2,				//	en exécution
	}
}
