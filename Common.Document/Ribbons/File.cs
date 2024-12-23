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

namespace Epsitec.Common.Document.Ribbons
{
    /// <summary>
    /// La classe File correspond au menu fichiers.
    /// </summary>
    public class File : Abstract
    {
        public File(DocumentType type, InstallType install, DebugMode debugMode)
            : base(type, install, debugMode)
        {
            this.Title = Res.Strings.Action.FileMain;
            this.PreferredWidth = 8 + 22 * 1.5 * 4 + 4 + 22 * 3;

            this.buttonSave = this.CreateIconButton("Save", "Large");
            this.buttonPrint = this.CreateIconButton("Print", "Large");

            double dx = this.buttonSave.PreferredWidth;
            double dy = this.buttonSave.PreferredHeight;

            this.groupNew = new Widget(this);
            this.buttonLastModels = this.CreateMenuButton(
                "",
                Res.Strings.Action.LastModels,
                this.HandleLastModelsPressed
            );
            this.buttonLastModels.ContentAlignment = ContentAlignment.BottomCenter;
            this.buttonLastModels.GlyphSize = new Size(dx * 1.5, dy * 0.5);
            this.buttonLastModels.Anchor = AnchorStyles.All;
            this.buttonLastModels.SetParent(this.groupNew);
            this.buttonNew = this.CreateIconButton("New", "Large");
            this.buttonNew.ButtonStyle = ButtonStyle.ComboItem;
            this.buttonNew.PreferredSize = new Size(dx * 1.5, dy * 1.5);
            this.buttonNew.Dock = DockStyle.Top;
            this.buttonNew.SetParent(this.groupNew);

            this.groupOpen = new Widget(this);
            this.buttonLastFiles = this.CreateMenuButton(
                "",
                Res.Strings.Action.LastFiles,
                this.HandleLastFilesPressed
            );
            this.buttonLastFiles.ContentAlignment = ContentAlignment.BottomCenter;
            this.buttonLastFiles.GlyphSize = new Size(dx * 1.5, dy * 0.5);
            this.buttonLastFiles.Anchor = AnchorStyles.All;
            this.buttonLastFiles.SetParent(this.groupOpen);
            this.buttonOpen = this.CreateIconButton("Open", "Large");
            this.buttonOpen.ButtonStyle = ButtonStyle.ComboItem;
            this.buttonOpen.PreferredSize = new Size(dx * 1.5, dy * 1.5);
            this.buttonOpen.Dock = DockStyle.Top;
            this.buttonOpen.SetParent(this.groupOpen);

            this.buttonExport = this.CreateIconButton("Export");
            this.buttonQuickExport = this.CreateIconButton("QuickExport");
            this.buttonSaveAs = this.CreateIconButton("SaveAs");
            this.buttonSaveModel = this.CreateIconButton("SaveModel");
            this.buttonCloseAll = this.CreateIconButton("CloseAll");

            //			this.UpdateClientGeometry();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) { }

            base.Dispose(disposing);
        }

        protected override void UpdateClientGeometry()
        {
            //	Met à jour la géométrie.
            base.UpdateClientGeometry();

            if (this.buttonNew == null)
                return;

            double dx = this.buttonSave.PreferredWidth;
            double dy = this.buttonSave.PreferredHeight;

            Rectangle rect = this.UsefulZone;
            rect.Width = dx * 1.5;
            rect.Height = dy * 2.0;
            this.groupNew.SetManualBounds(rect);
            rect.Offset(dx * 1.5, 0);
            this.groupOpen.SetManualBounds(rect);
            rect.Offset(dx * 1.5, 0);

            rect.Height = dy * 1.5;
            rect.Offset(0, dy * 0.5);
            this.buttonSave.SetManualBounds(rect);
            rect.Offset(dx * 1.5, 0);
            this.buttonPrint.SetManualBounds(rect);

            rect = this.UsefulZone;
            rect.Width = dx;
            rect.Height = dy;
            rect.Offset(dx * 1.5 * 4 + 4, dy + 5);
            this.buttonExport.SetManualBounds(rect);
            rect.Offset(dx, 0);
            this.buttonQuickExport.SetManualBounds(rect);

            rect = this.UsefulZone;
            rect.Width = dx;
            rect.Height = dy;
            rect.Offset(dx * 1.5 * 4 + 4, 0);
            this.buttonSaveAs.SetManualBounds(rect);
            rect.Offset(dx, 0);
            this.buttonSaveModel.SetManualBounds(rect);
            rect.Offset(dx, 0);
            this.buttonCloseAll.SetManualBounds(rect);
        }

        private void HandleLastModelsPressed(object sender, MessageEventArgs e)
        {
            //	Bouton pour ouvrir le menu des derniers modèles cliqué.
            GlyphButton button = sender as GlyphButton;
            if (button == null)
                return;
            VMenu menu = this.BuildLastModelsMenu();
            if (menu == null)
                return;
            menu.Host = this;
            menu.MinWidth = button.ActualWidth;
            TextFieldCombo.AdjustComboSize(button, menu, false);
            menu.ShowAsComboList(button, Point.Zero, button);
        }

        private void HandleLastFilesPressed(object sender, MessageEventArgs e)
        {
            //	Bouton pour ouvrir le menu des derniers fichiers cliqué.
            GlyphButton button = sender as GlyphButton;
            if (button == null)
                return;
            VMenu menu = this.BuildLastFilenamesMenu();
            if (menu == null)
                return;
            menu.Host = this;
            menu.MinWidth = button.ActualWidth;
            TextFieldCombo.AdjustComboSize(button, menu, false);
            menu.ShowAsComboList(button, Point.Zero, button);
        }

        protected VMenu BuildLastModelsMenu()
        {
            //	Construit le sous-menu des derniers modèles ouverts.
            int total = this.globalSettings.LastModelCount;
            if (total == 0)
                return null;

            VMenu menu = new VMenu();

            for (int i = 0; i < total; i++)
            {
                string cmd = "LastModel";
                string filename = string.Format(
                    "{0} {1}",
                    (i + 1) % 10,
                    this.globalSettings.LastModelGetShort(i)
                );
                string name = this.globalSettings.LastModelGet(i);
                Misc.CreateStructuredCommandWithName(cmd);
                MenuItem item = new MenuItem(cmd, "", filename, "", name);

                string tooltip = this.globalSettings.LastModelGet(i);
                if (tooltip != Epsitec.Common.Dialogs.AbstractFileDialog.NewEmptyDocument)
                {
                    ToolTip.Default.SetToolTip(item, tooltip);
                }

                menu.Items.Add(item);
            }

            menu.AdjustSize();
            return menu;
        }

        protected VMenu BuildLastFilenamesMenu()
        {
            //	Construit le sous-menu des derniers fichiers ouverts.
            int total = this.globalSettings.LastFilenameCount;
            if (total == 0)
                return null;

            VMenu menu = new VMenu();

            for (int i = 0; i < total; i++)
            {
                string cmd = "LastFile";
                string filename = string.Format(
                    "{0} {1}",
                    (i + 1) % 10,
                    this.globalSettings.LastFilenameGetShort(i)
                );
                string name = this.globalSettings.LastFilenameGet(i);
                Misc.CreateStructuredCommandWithName(cmd);
                MenuItem item = new MenuItem(cmd, "", filename, "", name);
                ToolTip.Default.SetToolTip(item, this.globalSettings.LastFilenameGet(i));
                menu.Items.Add(item);
            }

            menu.AdjustSize();
            return menu;
        }

        protected Widget groupNew;
        protected IconButton buttonNew;
        protected GlyphButton buttonLastModels;

        protected Widget groupOpen;
        protected IconButton buttonOpen;
        protected GlyphButton buttonLastFiles;

        protected IconButton buttonSave;
        protected IconButton buttonPrint;
        protected IconButton buttonExport;
        protected IconButton buttonQuickExport;
        protected IconButton buttonCloseAll;
        protected IconButton buttonSaveAs;
        protected IconButton buttonSaveModel;
    }
}
