//	Copyright � 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets.Helpers
{
	/// <summary>
	/// VisualPropertyMetadataOptions.
	/// </summary>
	
	[System.Flags]
	
	public enum VisualPropertyMetadataOptions
	{
		None					= 0,
		
		AffectsMeasure			= 0x0001,
		AffectsArrange			= 0x0002,
		AffectsDisplay			= 0x0004,
		AffectsChildrenLayout	= 0x0008,
		
		InheritsValue			= 0x1000,		//	la valeur de la propri�t� peut �tre h�rit�e par des enfants
		ChangesSilently			= 0x2000,		//	les changements de la propri�t� ne g�n�rent pas d'�v�nement
	}
}
