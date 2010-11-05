//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// L'énumération <c>WidgetPaintState</c> détermine quels effets graphiques
	/// utiliser quand un <c>Widget</c> est peint.
	/// </summary>
	[System.Flags]
	public enum WidgetPaintState : uint
	{
		None				= 0x00000000,				//	=> neutre
		
		ActiveYes			= 0x00000001,				//	=> mode ActiveState.Yes
		ActiveMaybe			= 0x00000002,				//	=> mode ActiveState.Maybe
		
		Enabled				= 0x00010000,				//	=> reçoit des événements
		Focused				= 0x00020000,				//	=> reçoit les événements clavier
		Entered				= 0x00040000,				//	=> contient la souris
		Selected			= 0x00080000,				//	=> sélectionné
		Engaged				= 0x00100000,				//	=> pression en cours
		Error				= 0x00200000,				//	=> signale une erreur
		ThreeState			= 0x00400000,				//	=> accepte 3 états
		UndefinedLanguage	= 0x00800000,				//	=> langue indéfinie

		InheritedFocus		= 0x01000000,				//	=> le parent est Focused
		InheritedEnter		= 0x02000000,				//	=> l'enfant est Entered
	}
}