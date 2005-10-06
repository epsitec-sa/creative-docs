//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets.Helpers
{
	/// <summary>
	/// VisualPropertyFlags.
	/// </summary>
	
	[System.Flags]
	
	public enum VisualPropertyFlags
	{
		None					= 0,
		
		AffectsLayout			= 0x0001,
		AffectsParentLayout		= 0x0002,
		AffectsDisplay			= 0x0004,
		
		InheritsValue			= 0x1000,		//	la valeur de la propri�t� peut �tre h�rit�e par des enfants
	}
}
