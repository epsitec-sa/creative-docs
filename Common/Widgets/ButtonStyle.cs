/*
This file is part of CreativeDocs.

Copyright © 2003-2024, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland

CreativeDocs is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
any later version.

CreativeDocs is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/


namespace Epsitec.Common.Widgets
{
    public enum ButtonStyle
    {
        None, // rien
        Flat, // pas de cadre, ni de relief
        Normal, // bouton normal
        Scroller, // bouton pour Scroller
        Slider, // bouton pour Slider
        Combo, // bouton pour TextFieldCombo
        UpDown, // bouton pour TextFieldUpDown
        ExListLeft, // bouton pour TextFieldExList, à gauche
        ExListMiddle, // bouton pour TextFieldExList, au milieu
        ExListRight, // bouton pour TextFieldExList, à droite (cf Combo)
        Tab, // bouton pour TabButton
        Icon, // bouton pour une icône
        ActivableIcon, // bouton pour une icône activable
        ToolItem, // bouton pour barre d'icône
        ComboItem, // bouton pour menu-combo
        ListItem, // bouton pour liste
        HeaderSlider, // bouton pour modifier une largeur de colonne
        Confirmation, // bouton pour confirmation (ConfirmationButton)

        DefaultAccept, // bouton pour accepter un choix dans un dialogue (OK)
        DefaultCancel, // bouton pour refuser un choix dans un dialogue (Cancel)
        DefaultAcceptAndCancel, // bouton unique pour accepter ou refuser un choix dans un dialogue
    }
}
