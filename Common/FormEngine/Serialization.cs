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

namespace Epsitec.Common.FormEngine
{
    /// <summary>
    /// Procédures de sérialisation et désérialisation de masques de saisie.
    /// </summary>
    public static class Serialization
    {
        public static string SerializeForm(FormDescription form)
        {
            //	Retourne la chaîne qui contient la sérialisation d'un masque de saisie.
            return form.Serialize();
        }

        public static FormDescription DeserializeForm(string xml)
        {
            //	Retourne le masque de saisie désérialisé à partir d'une chaîne.
            FormDescription form = new FormDescription();
            form.Deserialize(xml);
            return form;
        }
    }
}
