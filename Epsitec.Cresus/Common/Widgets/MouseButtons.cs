//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// L'énumération MouseButtons définit les boutons de la souris connus.
	/// Plusieurs boutons peuvent être combinés.
	/// </summary>
	[System.Flags] public enum MouseButtons
	{
		None			= 0,
		
		Left			= 0x00100000,
		Right			= 0x00200000,
		Middle			= 0x00400000,
		XButton1		= 0x00800000,
		XButton2		= 0x01000000
	}
}
