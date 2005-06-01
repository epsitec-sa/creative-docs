//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// L'énumération SizeUnits détermine les unités à utiliser pour décrire
	/// la taille d'une fonte, etc.
	/// </summary>
	public enum SizeUnits : byte
	{
		None,
		
		Points,						//	n [pt]
		Millimeters,				//	n [mm]
		Inches,						//	n [in]
		
		DeltaPoints,				//	N +/- n [pt]
		DeltaMillimeters,			//	N +/- n [mm]
		DeltaInches,				//	N +/- n [in]
		
		Percent,					//	N * n%
	}
}
