//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	public enum ButtonStyle
	{
		None,							// rien
		Flat,							// pas de cadre, ni de relief
		Normal,							// bouton normal
		Scroller,						// bouton pour Scroller
		Slider,							// bouton pour Slider
		Combo,							// bouton pour TextFieldCombo
		UpDown,							// bouton pour TextFieldUpDown
		ExListLeft,						// bouton pour TextFieldExList, à gauche
		ExListMiddle,					// bouton pour TextFieldExList, au milieu
		ExListRight,					// bouton pour TextFieldExList, à droite (cf Combo)
		Tab,							// bouton pour TabButton
		Icon,							// bouton pour une icône
		ActivableIcon,					// bouton pour une icône activable
		ToolItem,						// bouton pour barre d'icône
		ComboItem,						// bouton pour menu-combo
		ListItem,						// bouton pour liste
		HeaderSlider,					// bouton pour modifier une largeur de colonne
		Confirmation,					// bouton pour confirmation (ConfirmationButton)
		
		DefaultAccept,					// bouton pour accepter un choix dans un dialogue (OK)
		DefaultCancel,					// bouton pour refuser un choix dans un dialogue (Cancel)
		DefaultAcceptAndCancel,			// bouton unique pour accepter ou refuser un choix dans un dialogue
	}
}
