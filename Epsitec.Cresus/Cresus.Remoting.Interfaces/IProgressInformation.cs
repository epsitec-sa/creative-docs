//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
		bool			HasFinished			{ get; }	//	true => op�ration termin�e
		
		System.TimeSpan	RunningDuration		{ get; }	//	dur�e d'ex�cution actuelle
		System.TimeSpan	ExpectedDuration	{ get; }	//	dur�e totale estim�e, ind�termin�e si < 0
		
		bool WaitForProgress(int minimum_progress, System.TimeSpan timeout);
	}
}
