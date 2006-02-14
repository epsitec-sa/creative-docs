//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// L'énumération ModifierKeys définit les touches super-shift connues.
	/// Plusieurs touches super-shift peuvent être combinées.
	/// </summary>
	[System.Flags] public enum ModifierKeys
	{
		None			= 0,
		
		Shift			= 0x00010000,
		Control			= 0x00020000,
		Alt				= 0x00040000,
	}
}
