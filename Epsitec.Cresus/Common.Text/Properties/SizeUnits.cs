//	Copyright � 2005-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// L'�num�ration SizeUnits d�termine les unit�s � utiliser pour d�crire
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
