//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.OpenType
{
	/// <summary>
	/// L'énumération FontScalingMode détermine comment les glyphes doivent
	/// être traités.
	/// </summary>
	
	[System.Flags] public enum FontScalingMode
	{
		Transparent			= 0x000,		//	ne change rien (mode transparent)
		
		PreventLigatures	= 0x001,		//	empêche l'emploi de ligatures
		PreventStretching	= 0x002,		//	empêche l'emploi du stretch excessif
	}
}
