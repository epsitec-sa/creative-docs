//	Copyright � 2006-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// L'�num�ration <c>WidgetPaintState</c> d�termine quels effets graphiques
	/// utiliser quand un <c>Widget</c> est peint.
	/// </summary>
	[System.Flags]
	public enum WidgetPaintState : uint
	{
		None			= 0x00000000,				//	=> neutre
		
		ActiveYes		= 0x00000001,				//	=> mode ActiveState.Yes
		ActiveMaybe		= 0x00000002,				//	=> mode ActiveState.Maybe
		
		Enabled			= 0x00010000,				//	=> re�oit des �v�nements
		Focused			= 0x00020000,				//	=> re�oit les �v�nements clavier
		Entered			= 0x00040000,				//	=> contient la souris
		Selected		= 0x00080000,				//	=> s�lectionn�
		Engaged			= 0x00100000,				//	=> pression en cours
		Error			= 0x00200000,				//	=> signale une erreur
		ThreeState		= 0x00400000,				//	=> accepte 3 �tats
	}
}