//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
