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

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.DocumentEditor.Dialogs
{
    /// <summary>
    /// Dialogue d'exportation d'un fichier ICO.
    /// </summary>
    public class ExportICO : Abstract
    {
        public ExportICO(DocumentEditor editor)
            : base(editor) { }

        public void Show(string filename)
        {
            //	Crée et montre la fenêtre du dialogue.
            if (this.window == null)
            {
                this.window = new Window(WindowFlags.HideFromTaskbar);
                this.WindowInit("ExportICO", 300, 220);
                this.window.PreventAutoClose = true;
                this.window.Owner = this.editor.Window;
                this.window.WindowCloseClicked += this.HandleWindowExportCloseClicked;

                Viewport panel = new Viewport(this.window.Root);
                panel.Name = "Panel";
                panel.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.TopAndBottom;
                panel.Margins = new Margins(10, 10, 10, 40);

                //	Boutons de fermeture.
                Button buttonOk = new Button(this.window.Root);
                buttonOk.PreferredWidth = 75;
                buttonOk.Text = Res.Strings.Dialog.ExportICO.Button.OK;
                buttonOk.ButtonStyle = ButtonStyle.DefaultAccept;
                buttonOk.Anchor = AnchorStyles.BottomRight;
                buttonOk.Margins = new Margins(0, 6 + 75 + 6, 0, 6);
                buttonOk.Clicked += this.HandleExportButtonOkClicked;
                buttonOk.TabIndex = 10;
                buttonOk.TabNavigationMode = TabNavigationMode.ActivateOnTab;
                ToolTip.Default.SetToolTip(buttonOk, Res.Strings.Dialog.ExportICO.Tooltip.OK);

                Button buttonCancel = new Button(this.window.Root);
                buttonCancel.PreferredWidth = 75;
                buttonCancel.Text = Res.Strings.Dialog.ExportICO.Button.Cancel;
                buttonCancel.ButtonStyle = ButtonStyle.DefaultCancel;
                buttonCancel.Anchor = AnchorStyles.BottomRight;
                buttonCancel.Margins = new Margins(0, 6, 0, 6);
                buttonCancel.Clicked += this.HandleExportButtonCancelClicked;
                buttonCancel.TabIndex = 11;
                buttonCancel.TabNavigationMode = TabNavigationMode.ActivateOnTab;
                ToolTip.Default.SetToolTip(
                    buttonCancel,
                    Res.Strings.Dialog.ExportICO.Tooltip.Cancel
                );
            }

            if (this.editor.HasCurrentDocument)
            {
                this.editor.CurrentDocument.Dialogs.BuildExportICO(this.window);
            }

            this.window.Text = string.Format(
                Res.Strings.Dialog.ExportICO.Title2,
                System.IO.Path.GetFileName(filename)
            );
            this.window.ShowDialog();
        }

        public override void Save()
        {
            //	Enregistre la position de la fenêtre du dialogue.
            this.WindowSave("ExportICO");
        }

        public override void Rebuild()
        {
            //	Reconstruit le dialogue.
            if (!this.editor.HasCurrentDocument)
                return;
            if (this.window == null)
                return;
            this.editor.CurrentDocument.Dialogs.BuildExportICO(this.window);
        }

        public void UpdatePages()
        {
            //	Met à jour le dialogue lorsque les pages ont changé.
            this.editor.CurrentDocument.Dialogs.UpdateExportICOPages();
        }

        private void HandleWindowExportCloseClicked(object sender)
        {
            this.CloseWindow();
        }

        private void HandleExportButtonCancelClicked(object sender, MessageEventArgs e)
        {
            this.CloseWindow();
        }

        private void HandleExportButtonOkClicked(object sender, MessageEventArgs e)
        {
            this.CloseWindow();

            string filename = System.IO.Path.Combine(
                this.editor.CurrentDocument.ExportDirectory,
                this.editor.CurrentDocument.ExportFilename
            );
            string err = this.editor.CurrentDocument.ExportICO(filename);
            this.editor.DialogError(err);
        }
    }
}
