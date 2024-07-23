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


namespace Epsitec.Common.Text
{
    /// <summary>
    /// L'interface IParagraphManager décrit les fonctions qu'un gestionnaire de
    /// paragraphe doit mettre à disposition.
    /// du texte.
    /// </summary>
    public interface IParagraphManager
    {
        string Name { get; }

        void AttachToParagraph(
            TextStory story,
            ICursor cursor,
            Properties.ManagedParagraphProperty property
        );
        void DetachFromParagraph(
            TextStory story,
            ICursor cursor,
            Properties.ManagedParagraphProperty property
        );
        void RefreshParagraph(
            TextStory story,
            ICursor cursor,
            Properties.ManagedParagraphProperty property
        );

        //	En attachant un gestionnaire de paragraphe à un paragraphe donné, il
        //	est possible de modifier le texte pour y ajouter des caractères marqués
        //	avec la propriété AutoTextProperty.
        //
        //	NB: Le curseur pointe toujours au début du paragraphe.
    }
}
