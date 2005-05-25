//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// L'énumération CombinationMode détermine comment des propriétés peuvent
	/// être combinées.
	/// </summary>
	public enum CombinationMode
	{
		Invalid				= 0,
		
		Combine,								//	combine deux propriétés pour en former une nouvelle
		Accumulate								//	accumule les propriétés (pas de combinaison)
	}
}
