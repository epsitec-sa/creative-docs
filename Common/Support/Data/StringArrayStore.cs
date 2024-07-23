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
    /// Summary description for StringArrayStore.
    /// </summary>
    public class StringArrayStore : ITextArrayStore
    {
        public StringArrayStore(string[] array)
        {
            this.array = array;
        }

        public string this[int index]
        {
            get { return this.array[index]; }
            set
            {
                if (this.array[index] != value)
                {
                    this.array[index] = value;
                    this.OnStoreContentsChanged();
                }
            }
        }

        public int Count
        {
            get { return this.array.Length; }
        }

        #region ITextArrayStore Members
        public int GetRowCount()
        {
            return this.array.Length;
        }

        public int GetColumnCount()
        {
            return 1;
        }

        public string GetCellText(int row, int column)
        {
            if (column == 0)
            {
                return this.array[row];
            }

            return null;
        }

        public void SetCellText(int row, int column, string value)
        {
            if (column == 0)
            {
                this.array[row] = value;
            }
            else
            {
                throw new System.NotSupportedException(
                    string.Format("Cannot set cell in column {0}.", column)
                );
            }
        }

        public void InsertRows(int row, int num)
        {
            throw new System.NotSupportedException(string.Format("Cannot insert rows."));
        }

        public void RemoveRows(int row, int num)
        {
            throw new System.NotSupportedException(string.Format("Cannot remove rows."));
        }

        public void MoveRow(int row, int distance)
        {
            throw new System.NotSupportedException(string.Format("Cannot move row."));
        }

        public bool CheckSetRow(int row)
        {
            return (row >= 0) && (row < this.array.Length);
        }

        public bool CheckInsertRows(int row, int num)
        {
            return false;
        }

        public bool CheckRemoveRows(int row, int num)
        {
            return false;
        }

        public bool CheckMoveRow(int row, int distance)
        {
            return false;
        }

        public bool CheckEnabledCell(int row, int column)
        {
            return true;
        }

        public event Support.EventHandler StoreContentsChanged;
        #endregion

        protected virtual void OnStoreContentsChanged()
        {
            if (this.StoreContentsChanged != null)
            {
                this.StoreContentsChanged(this);
            }
        }

        protected string[] array;
    }
}
