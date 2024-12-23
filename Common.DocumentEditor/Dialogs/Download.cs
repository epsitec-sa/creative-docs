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
    /// Dialogue "Télécharger une mise à jour".
    /// </summary>
    public class Download : Abstract
    {
        public Download(DocumentEditor editor)
            : base(editor) { }

        public void SetInfo(string version, string url)
        {
            //	Spécifie les informations pour la mise à jour.
            if (version.EndsWith(".0"))
            {
                version = version.Substring(0, version.Length - 2);
            }
            this.version = version;
            this.url = url;
        }

        public override void Show()
        {
            /*
            //	Crée et montre la fenêtre du dialogue.
            if (this.window == null)
            {
                this.window = new PlatformWindow();
                this.window.MakeFixedSizeWindow();
                this.window.MakeSecondaryWindow();
                this.WindowInit("Download", 250, 120);
                this.window.Text = Res.Strings.Dialog.Download.Title;
                this.window.PreventAutoClose = true;
                this.window.Owner = this.editor.PlatformWindow;
                this.window.Icon = this.editor.PlatformWindow.Icon;
                this.window.WindowCloseClicked += this.HandleWindowDownloadCloseClicked;

                StaticText title = new StaticText(this.window.Root);
                if (string.IsNullOrEmpty(this.url))
                {
                    title.Text = Res.Strings.Dialog.Download.UpToDate;
                }
                else
                {
                    title.Text = Res.Strings.Dialog.Download.Available;
                }
                title.Dock = DockStyle.Top;
                title.Margins = new Margins(6, 6, 6, 0);
                title.HypertextClicked += this.HandleLinkHypertextClicked;

                string chip = "<list type=\"fix\" width=\"1.5\"/>";

                string current = string.Format(
                    Res.Strings.Dialog.Download.Actual,
                    About.GetVersion()
                );
                StaticText actual = new StaticText(this.window.Root);
                actual.Text = chip + current;
                actual.Dock = DockStyle.Top;
                actual.Margins = new Margins(6, 6, 8, 0);

                string text;
                if (string.IsNullOrEmpty(this.url))
                {
                    text = Res.Strings.Dialog.Download.Nothing;
                }
                else
                {
                    string link = string.Format(Res.Strings.Dialog.Download.Link, this.version);
                    text = string.Format("<a href=\"{0}\">{1}</a>", this.url, link);
                }
                StaticText url = new StaticText(this.window.Root);
                url.Text = chip + text;
                url.Dock = DockStyle.Top;
                url.Margins = new Margins(6, 6, 0, 0);
                url.HypertextClicked += HandleLinkHypertextClicked;

                //	Bouton de fermeture.
                Button buttonClose = new Button(this.window.Root);
                buttonClose.PreferredWidth = 75;
                buttonClose.Text = Res.Strings.Dialog.Button.Close;
                buttonClose.ButtonStyle = ButtonStyle.DefaultAcceptAndCancel;
                buttonClose.Anchor = AnchorStyles.BottomRight;
                buttonClose.Margins = new Margins(0, 6, 0, 6);
                buttonClose.Clicked += this.HandleDownloadButtonCloseClicked;
                buttonClose.TabIndex = 1000;
                buttonClose.TabNavigationMode = TabNavigationMode.ActivateOnTab;
                ToolTip.Default.SetToolTip(buttonClose, Res.Strings.Dialog.Tooltip.Close);
            }

            this.window.ShowDialog();
            */
        }

        public override void Save()
        {
            //	Enregistre la position de la fenêtre du dialogue.
            this.WindowSave("Download");
        }

        private void HandleLinkHypertextClicked(object sender, MessageEventArgs e)
        {
            Widget widget = sender as Widget;
            System.Diagnostics.Process.Start(widget.Hypertext);
        }

        private void HandleWindowDownloadCloseClicked(object sender)
        {
            this.CloseWindow();
        }

        private void HandleDownloadButtonCloseClicked(object sender, MessageEventArgs e)
        {
            this.CloseWindow();
        }

        protected string version;
        protected string url;
    }
}
