//	Copyright � 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Remoting
{
	/// <summary>
	/// L'interface IProgressInformation permet d'obtenir des informations sur
	/// le progr�s d'une op�ration de longue dur�e.
	/// </summary>
	public interface IProgressInformation
	{
		int				ProgressPercent		{ get; }	//	avancement 0..100, -1 = ind�termin�
		ProgressStatus	ProgressStatus		{ get; }	//	�tat (en cours d'ex�cution, termin�, annul�, �chou�)
		int				CurrentStep			{ get; }	//	# de l'�tape en cours (0..n)
		int				LastStep			{ get; }	//	# de la derni�re �tape, -1 = ind�termin�
		
		System.TimeSpan	RunningDuration		{ get; }	//	dur�e d'ex�cution actuelle
		System.TimeSpan	ExpectedDuration	{ get; }	//	dur�e totale estim�e, ind�termin�e si < 0
		
		bool WaitForProgress(int minimum_progress, System.TimeSpan timeout);
	}
}
