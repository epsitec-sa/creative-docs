//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
		AffectsTextLayout		= 0x0010,
		
		InheritsValue			= 0x1000,		//	la valeur de la propriété peut être héritée par des enfants
		ChangesSilently			= 0x2000,		//	les changements de la propriété ne génèrent pas d'événement
	}
}
