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
    /// L'énumération TextStyleClass définit les catégories auxquelles les
    /// styles TextStyle peuvent appartenir.
    /// </summary>
    public enum TextStyleClass
    {
        Invalid = 0, //	pas valide

        Abstract = 1, //	style abstrait, sert uniquement de base aux autres
        Paragraph = 2, //	style de paragraphe, appliqué au paragraphe entier
        Text = 3, //	style de texte, appliqué à un passage de texte local
        Symbol = 4, //	style de symboles, appliqué à un caractère unique

        MetaProperty = 5, //	style se comportant comme une propriété
    }
}
