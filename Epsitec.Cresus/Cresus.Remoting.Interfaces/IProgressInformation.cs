//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Remoting
{
	/// <summary>
	/// L'interface IProgressInformation permet d'obtenir des informations sur
	/// le progrès d'une opération de longue durée.
	/// </summary>
	public interface IProgressInformation
	{
		int				ProgressPercent		{ get; }	//	avancement 0..100, -1 = indéterminé
		bool			HasFinished			{ get; }	//	true => opération terminée
		
		System.TimeSpan	RunningDuration		{ get; }	//	durée d'exécution actuelle
		System.TimeSpan	ExpectedDuration	{ get; }	//	durée totale estimée, indéterminée si < 0
		
		bool WaitForProgress(int minimum_progress, System.TimeSpan timeout);
	}
}
