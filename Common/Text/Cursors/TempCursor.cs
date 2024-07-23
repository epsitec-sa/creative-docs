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


namespace Epsitec.Common.Text.Cursors
{
    /// <summary>
    /// La classe TempCursor décrit un curseur temporaire (réservé à un usage
    /// interne, les modifications d'un curseur temporaire ne sont pas sauvées
    /// pour le undo/redo).
    /// </summary>
    public class TempCursor : ICursor
    {
        public TempCursor() { }

        #region ICursor Members
        public int CursorId
        {
            get { return this.cursorId; }
            set { this.cursorId = value; }
        }

        public virtual CursorAttachment Attachment
        {
            get { return CursorAttachment.Temporary; }
        }

        public virtual int Direction
        {
            get { return 0; }
            set
            {
                //				System.Diagnostics.Debug.Assert (value == 0);
            }
        }

        public virtual void Clear() { }
        #endregion

        private Internal.CursorId cursorId;
    }
}
