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
    /// Dialogue des informations sur le document.
    /// </summary>
    public class Infos : Abstract
    {
        public Infos(DocumentEditor editor)
            : base(editor) { }

        public override void Show()
        {
            //	Crée et montre la fenêtre du dialogue.
            if (this.window == null)
            {
                this.window = new Window(WindowFlags.HideFromTaskbar | WindowFlags.Resizable);
                this.window.PreventAutoClose = true;
                this.WindowInit("Infos", 300, 250, true);
                this.window.Text = Res.Strings.Dialog.Infos.Title;
                this.window.Owner = this.editor.Window;
                this.window.Icon = Support.ImageProvider.Instance.GetImageFromManifestResource(
                    "Epsitec.Common.DocumentEditor.Images.Application.icon",
                    this.GetType().Assembly
                );
                this.window.WindowCloseClicked += this.HandleWindowInfosCloseClicked;
                this.window.Root.MinSize = new Size(160, 100);

                ResizeKnob resize = new ResizeKnob(this.window.Root);
                resize.Anchor = AnchorStyles.BottomRight;
                resize.Margins = new Margins(0, 0, 0, 0);
                ToolTip.Default.SetToolTip(resize, Res.Strings.Dialog.Tooltip.Resize);

                TextFieldMulti multi = new TextFieldMulti(this.window.Root);
                multi.Name = "Infos";
                multi.IsReadOnly = true;
                multi.MaxLength = 100000;
                multi.Dock = DockStyle.Fill;
                multi.Margins = new Margins(6, 6, 6, 34);

                //	Bouton de fermeture.
                Button buttonClose = new Button(this.window.Root);
                buttonClose.PreferredWidth = 75;
                buttonClose.Text = Res.Strings.Dialog.Button.Close;
                buttonClose.ButtonStyle = ButtonStyle.DefaultAcceptAndCancel;
                buttonClose.Anchor = AnchorStyles.BottomRight;
                buttonClose.Margins = new Margins(0, 6, 0, 6);
                buttonClose.Clicked += this.HandleInfosButtonCloseClicked;
                buttonClose.TabIndex = 1000;
                buttonClose.TabNavigationMode = TabNavigationMode.ActivateOnTab;
                ToolTip.Default.SetToolTip(buttonClose, Res.Strings.Dialog.Tooltip.Close);
            }

            this.window.Show();
            this.editor.CurrentDocument.Dialogs.BuildInfos(this.window);
        }

        public override void Save()
        {
            //	Enregistre la position de la fenêtre du dialogue.
            this.WindowSave("Infos");
        }

        public override void Rebuild()
        {
            //	Reconstruit le dialogue.
            if (!this.editor.HasCurrentDocument)
                return;
            if (this.window == null)
                return;
            this.editor.CurrentDocument.Dialogs.BuildInfos(this.window);
        }

        private void HandleWindowInfosCloseClicked(object sender)
        {
            this.CloseWindow();
        }

        private void HandleInfosButtonCloseClicked(object sender, MessageEventArgs e)
        {
            this.CloseWindow();
        }
    }
}
