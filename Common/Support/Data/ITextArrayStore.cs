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


namespace Epsitec.Common.Support.Data
{
    /// <summary>
    /// L'interface ITextArrayStore permet d'accéder à des textes stockés dans
    /// une table bidimensionnelle, en lecture et en écriture.
    /// </summary>
    public interface ITextArrayStore
    {
        int GetRowCount();
        int GetColumnCount();

        string GetCellText(int row, int column);
        void SetCellText(int row, int column, string value);
        void InsertRows(int row, int num);
        void RemoveRows(int row, int num);
        void MoveRow(int row, int distance);

        bool CheckSetRow(int row);
        bool CheckInsertRows(int row, int num);
        bool CheckRemoveRows(int row, int num);
        bool CheckMoveRow(int row, int distance);
        bool CheckEnabledCell(int row, int column);

        event Support.EventHandler StoreContentsChanged;
    }
}
