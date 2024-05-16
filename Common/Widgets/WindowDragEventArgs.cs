//	Copyright © 2003-2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

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
