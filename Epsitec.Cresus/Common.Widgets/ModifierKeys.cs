//	Copyright � 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// L'�num�ration ModifierKeys d�finit les touches super-shift connues.
	/// Plusieurs touches super-shift peuvent �tre combin�es.
	/// </summary>
	[System.Flags] public enum ModifierKeys
	{
		None			= 0,
		
		Shift			= 0x00010000,
		Control			= 0x00020000,
		Alt				= 0x00040000,
	}
}
