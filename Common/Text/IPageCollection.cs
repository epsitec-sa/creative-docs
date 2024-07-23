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
    /// L'interface IPageCollection permet de d'obtenir des informations sur
    /// des pages d'un document.
    /// </summary>
    public interface IPageCollection
    {
        int GetPageCount();

        string GetPageLabel(int page);
        PageFlags GetPageFlags(int page);

        void SetPageProperty(int page, string key, string value);
        string GetPageProperty(int page, string key);
        void ClearPageProperties(int page);
    }

    /// <summary>
    /// Le bitset PageFlags permet d'indiquer si une page est paire/impaire et
    /// s'il s'agit de la première page d'un document/d'une section.
    /// </summary>

    [System.Flags]
    public enum PageFlags
    {
        None = 0,

        Even = 1,
        Odd = 2,

        First = 0x10,
    }
}
