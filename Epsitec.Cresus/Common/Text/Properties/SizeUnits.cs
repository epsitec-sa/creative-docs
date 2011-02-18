//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
		PercentNotCombining			//	N * n% (ne se combine pas avec d'autres valeurs)
	}
}
