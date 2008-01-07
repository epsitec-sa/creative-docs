//	Copyright � 2004-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Requests
{
	/// <summary>
	/// L'�num�ration OrchestratorState d�crit l'�tat de l'Orchestrator.
	/// </summary>
	public enum OrchestratorState
	{
		Ready				= 0,				//	pr�t, rien � faire pour le moment
		Conflicting			= 1,				//	pr�t, en attente de la r�solution d'un conflit
		Processing			= 2,				//	en ex�cution
	}
}
