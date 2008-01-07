//	Copyright � 2005-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// L'�num�ration CombinationMode d�termine comment des propri�t�s peuvent
	/// �tre combin�es.
	/// </summary>
	public enum CombinationMode
	{
		Invalid				= 0,
		
		Combine,								//	combine deux propri�t�s pour en former une nouvelle
		Accumulate								//	accumule les propri�t�s (pas de combinaison)
	}
}
