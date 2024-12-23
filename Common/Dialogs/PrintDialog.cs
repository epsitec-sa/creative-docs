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


namespace Epsitec.Common.Dialogs
{
    /// <summary>
    /// La classe Print présente le dialogue pour imprimer un document.
    /// </summary>
    public class PrintDialog
    {
        public PrintDialog()
        {
            this.dialog = new System.Windows.Forms.PrintDialog();

            this.dialog.AllowPrintToFile = true;
            this.dialog.AllowSelection = true;
            this.dialog.AllowSomePages = true;
            this.dialog.PrintToFile = false;
            this.dialog.ShowHelp = false;
            this.dialog.ShowNetwork = true;

            this.Document = new Printing.PrintDocument();
        }

        public bool AllowPrintToFile
        {
            get { return this.dialog.AllowPrintToFile; }
            set { this.dialog.AllowPrintToFile = value; }
        }

        public bool AllowFromPageToPage
        {
            get { return this.dialog.AllowSomePages; }
            set { this.dialog.AllowSomePages = value; }
        }

        public bool AllowSelectedPages
        {
            get { return this.dialog.AllowSelection; }
            set { this.dialog.AllowSelection = value; }
        }

        public bool PrintToFile
        {
            get { return this.dialog.PrintToFile; }
            set { this.dialog.PrintToFile = value; }
        }

        public Printing.PrintDocument Document
        {
            get { return this.document; }
            set
            {
                if (this.document != value)
                {
                    if (this.document != null)
                    {
                        this.Detach(this.document);
                    }

                    this.document = value;

                    if (this.document != null)
                    {
                        this.Attach(this.document);
                    }
                }
            }
        }

        public DialogResult Result
        {
            get { return this.result; }
        }

        #region IDialog Members
        public virtual void OpenDialog()
        {
            System.Windows.Forms.IWin32Window owner =
                this.owner == null ? null : this.owner.PlatformWindowObject;
            System.Windows.Forms.DialogResult result = this.dialog.ShowDialog(owner);

            switch (result)
            {
                case System.Windows.Forms.DialogResult.OK:
                case System.Windows.Forms.DialogResult.Yes:
                    this.result = DialogResult.Accept;
                    break;

                default:
                    this.result = DialogResult.Cancel;
                    break;
            }
        }

        public Common.Widgets.Window Owner
        {
            get { return this.owner; }
            set { this.owner = value; }
        }
        #endregion

        protected virtual void Attach(Printing.PrintDocument document)
        {
            this.dialog.Document = document.Object as System.Drawing.Printing.PrintDocument;
            this.dialog.PrinterSettings =
                document.PrinterSettings.Object as System.Drawing.Printing.PrinterSettings;

            document.PrinterChanged += this.HandleDocumentPrinterChanged;
        }

        protected virtual void Detach(Printing.PrintDocument document)
        {
            this.dialog.Document = null;
            this.dialog.PrinterSettings = null;

            document.PrinterChanged -= this.HandleDocumentPrinterChanged;
        }

        private void HandleDocumentPrinterChanged(object sender)
        {
            this.dialog.PrinterSettings =
                this.document.PrinterSettings.Object as System.Drawing.Printing.PrinterSettings;
        }

        protected Common.Widgets.Window owner;
        protected System.Windows.Forms.PrintDialog dialog;
        protected Printing.PrintDocument document;
        protected DialogResult result = DialogResult.None;
    }
}
