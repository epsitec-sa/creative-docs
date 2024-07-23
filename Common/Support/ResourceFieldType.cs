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


namespace Epsitec.Common.Support
{
    /// <summary>
    /// L'énumération ResourceFieldType définit les divers type de champs
    /// qu'une ressource peut contenir.
    /// </summary>
    public enum ResourceFieldType
    {
        None, //	champ n'existe pas
        Data, //	champ contient des données (string)
        Binary, //	champ contient des données binaires
        Bundle, //	champ contient un bundle
        List, //	champ contient une liste (de bundles)
    }
}
