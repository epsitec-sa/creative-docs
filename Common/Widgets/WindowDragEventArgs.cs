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
    /// <summary>
    /// The <c>WindowDragEventArgs</c> class describes an event which is produced
    /// while <c>WinForms</c> drag and drop is happening.
    /// </summary>
    public sealed class WindowDragEventArgs : Support.EventArgs
    {
        /// <summary>
        /// Gets the associated clipboard data.
        /// </summary>
        /// <value>The associated clipboard data.</value>
        public Support.ClipboardReadData Data
        {
            //get { return Support.Clipboard.CreateReadDataFromIDataObject(this.original.Data); }
            get
            {
                throw new System.NotImplementedException();
                return new Support.ClipboardReadData();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the target is accepting the drop.
        /// </summary>
        /// <value><c>true</c> if the target is accepting the drop; otherwise, <c>false</c>.</value>
        public bool AcceptDrop
        {
            /*
            get { return (this.original.Effect & System.Windows.Forms.DragDropEffects.Copy) != 0; }
            set
            {
                if (this.AcceptDrop != value)
                {
                    if (value)
                    {
                        this.original.Effect = System.Windows.Forms.DragDropEffects.Copy;
                    }
                    else
                    {
                        this.original.Effect = System.Windows.Forms.DragDropEffects.None;
                    }
                }
            }
            */
            get
            {
                throw new System.NotImplementedException();
                return true;
            }
            set { throw new System.NotImplementedException(); }
        }

        //private readonly System.Windows.Forms.DragEventArgs original;
    }
}
