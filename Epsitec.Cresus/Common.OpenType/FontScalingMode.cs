//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.OpenType
{
	/// <summary>
	/// L'�num�ration FontScalingMode d�termine comment les glyphes doivent
	/// �tre trait�s.
	/// </summary>
	
	[System.Flags] public enum FontScalingMode
	{
		Transparent			= 0x000,		//	ne change rien (mode transparent)
		
		PreventLigatures	= 0x001,		//	emp�che l'emploi de ligatures
		PreventStretching	= 0x002,		//	emp�che l'emploi du stretch excessif
	}
}
