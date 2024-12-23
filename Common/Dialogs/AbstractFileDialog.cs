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


using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Dialogs.Helpers;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Dialogs
{
    /// <summary>
    /// Anciennement, cette classe créait un dialogue "maison" pour ouvrir ou enregistrer un
    /// fichier. Par la suite, il a été décidé de remplacer ces dialogues peu pratiques par
    /// de véritables dialogues Windows. Il en résulte une interface pas forcément idéale !
    /// Les anciens dialogues "maison" avaient une partie dédiée aux options, en bas.
    /// Ces options s'affichent désormais dans un 2ème dialogue spécifique, qui vient après
    /// le dialogue Windows standard (pour autant qu'il existe des options).
    /// </summary>
    public abstract class AbstractFileDialog
    {
        // ********************************************************************
        // TODO bl-net8-cross
        // - implement AbstractFileDialog (stub)
        // ********************************************************************

        public AbstractFileDialog()
        {
            this.filters = new FilterCollection(null);
            this.favorites = new List<string>();
        }

        public string Title
        {
            get { return this.title; }
            set { this.title = value; }
        }

        public DialogResult Result
        {
            //	Indique si le dialogue a été fermé avec 'ouvrir' ou 'annuler'.
            get { return this.result; }
        }

        public string InitialDirectory
        {
            //	Dossier initial.
            get { return this.initialDirectory; }
            set
            {
                this.isRedirected = false;
                FolderItem folder;

                if (value == "") // poste de travail ?
                {
                    folder = FileManager.GetFolderItem(
                        System.Environment.SpecialFolder.MyDocuments,
                        FolderQueryMode.NoIcons
                    );
                }
                else
                {
                    if (this.fileDialogType == FileDialogType.Save)
                    {
                        string oldPath = value;
                        string newPath = this.RedirectPath(oldPath);
                        this.isRedirected = oldPath != newPath;
                        value = newPath;
                    }

                    folder = FileManager.GetFolderItem(value, FolderQueryMode.NoIcons);

                    if (folder.IsEmpty)
                    {
                        folder = FileManager.GetFolderItem(
                            System.Environment.SpecialFolder.MyDocuments,
                            FolderQueryMode.NoIcons
                        );
                    }
                }

                this.initialDirectory = folder.FullPath;
            }
        }

        public string InitialFileName {
            //	Nom de fichier initial.
            get; set; }

        public FilterCollection Filters
        {
            get { return this.filters; }
        }

        public bool IsDirectoryRedirected
        {
            //	Indique si le dossier passé avec InitialDirectory a dû être
            //	redirigé de 'Exemples originaux' vers 'Mes exemples'.
            get { return this.isRedirected; }
        }

        public string FileName
        {
            //	Retourne le nom du fichier à ouvrir, ou null si l'utilisateur a choisi
            //	le bouton 'annuler'.
            get { return this.filename; }
        }

        public string[] FileNames
        {
            //	Retourne les noms des fichiers à ouvrir, ou null si l'utilisateur a choisi
            //	le bouton 'annuler'.
            get { return this.filenames; }
        }

        public DialogResult ShowDialog()
        {
            // bl-net8-cross cleanup
            //System.Windows.Forms.FileDialog dialog;
            NativeFileDialogSharp.DialogResult result;
            switch (this.fileDialogType)
            {
                case FileDialogType.Save:
                    //dialog = new System.Windows.Forms.SaveFileDialog()
                    //{
                    //    CheckFileExists = false,
                    //    CheckPathExists = false,
                    //};
                    result = NativeFileDialogSharp.Dialog.FileSave(
                        this.filters.FileDialogFilter,
                        this.InitialDirectory
                    );
                    break;

                default:
                    if (this.enableMultipleSelection)
                    {
                        result = NativeFileDialogSharp.Dialog.FileOpenMultiple(
                            this.filters.FileDialogFilter,
                            this.InitialDirectory
                        );
                    }
                    else
                    {
                        result = NativeFileDialogSharp.Dialog.FileOpen(
                            this.filters.FileDialogFilter,
                            this.InitialDirectory
                        );
                    }
                    //dialog = new System.Windows.Forms.OpenFileDialog()
                    //{
                    //    CheckFileExists = true,
                    //    CheckPathExists = true,
                    //    Multiselect = this.enableMultipleSelection,
                    //};
                    break;
            }
            // SETUP SOME OPTIONS

            //dialog.AutoUpgradeEnabled = true;
            //dialog.DereferenceLinks = true;
            //dialog.AddExtension = true;
            //dialog.RestoreDirectory = false;
            //dialog.ShowHelp = false;
            //dialog.ValidateNames = true;
            //dialog.Title = this.title;
            //dialog.InitialDirectory = this.InitialDirectory;
            //dialog.FileName = System.IO.Path.GetFileName(this.FileName);
            //dialog.Filter = this.filters.FileDialogFilter;

            //foreach (var favorite in this.favorites)
            //{
            //    var place = new System.Windows.Forms.FileDialogCustomPlace(favorite);
            //    dialog.CustomPlaces.Add(place);
            //}

            // SHOW THE DIALOG
            //var windowsResult = dialog.ShowDialog();

            // GET THE RESULT
            //this.result = AbstractFileDialog.ConvertWindowsResult(windowsResult);
            if (result.IsOk)
            {
                this.result = DialogResult.Accept;
            }
            if (result.IsCancelled)
            {
                this.result = DialogResult.Cancel;
            }

            if (result.IsOk)
            {
                this.filename = result.Path;
                this.filenames = result.Paths?.ToArray();

                this.InitialDirectory = System.IO.Path.GetDirectoryName(this.filename);
            }
            else
            {
                this.filename = null;
                this.filenames = null;
            }

            if (this.hasOptions && this.result == DialogResult.Accept)
            {
                return this.ShowOptionsDialog();
            }
            else
            {
                return this.result;
            }
        }

        protected virtual string RedirectPath(string path)
        {
            return path;
        }

        protected abstract IFavoritesSettings FavoritesSettings { get; }

        protected virtual string ActionButtonName
        {
            get
            {
                switch (this.fileDialogType)
                {
                    case FileDialogType.New:
                        return Epsitec.Common.Dialogs.Res.Strings.Dialog.File.Button.New.ToSimpleText();

                    case FileDialogType.Save:
                        return Epsitec.Common.Dialogs.Res.Strings.Dialog.File.Button.Save.ToSimpleText();

                    default:
                        return Epsitec.Common.Dialogs.Res.Strings.Dialog.File.Button.Open.ToSimpleText();
                }
            }
        }

        protected abstract Rectangle GetOwnerBounds();

        protected abstract void FavoritesAddApplicationFolders();
        protected void AddFavorite(string text, string icon, string path)
        {
            this.favorites.Add(path);
        }

        #region Options dialog
        private DialogResult ShowOptionsDialog()
        {
            //	Ouvre le dialogue des options, qui s'affiche après le dialogue Windows
            //	standard pour ouvrir/enregistrer.
            if (this.window == null)
            {
                this.CreateOptionsDialog();
            }

            this.result = DialogResult.Cancel;
            this.window.ShowDialog();
            return this.result;
        }

        private void CreateOptionsDialog()
        {
            //	Crée la fenêtre et tous les widgets pour peupler le dialogue.
            this.window = new Window();
            this.window.PreventAutoClose = true;
            this.window.Name = "Options";
            this.window.Text = this.title;
            this.window.Owner = this.owner;
            this.window.Icon = this.owner == null ? null : this.window.Owner.Icon;

            this.SetWindowGeometry(470, 140, false);

            this.window.WindowCloseClicked += this.HandleWindowCloseClicked;

            //	Dans l'ordre de bas en haut :
            this.CreateFooter();
            this.CreateOptionsUserInterface();
        }

        private void SetWindowGeometry(double dx, double dy, bool resizable)
        {
            this.window.ClientSize = new Size(dx, dy);

            dx = this.window.WindowSize.Width;
            dy = this.window.WindowSize.Height; // taille avec le cadre

            Rectangle cb = this.GetOwnerBounds();
            this.window.WindowBounds = new Rectangle(
                cb.Center.X - dx / 2,
                cb.Center.Y - dy / 2,
                dx,
                dy
            );

            this.window.Root.Padding = new Margins(8);
        }

        protected virtual void CreateOptionsUserInterface() { }

        private void CreateFooter()
        {
            //	Crée le pied du dialogue, avec les boutons 'ouvrir/enregistrer' et 'annuler'.
            var footer = new Widget(this.window.Root)
            {
                PreferredHeight = 22,
                Margins = new Margins(0, 0, 8, 0),
                Dock = DockStyle.Bottom,
                TabIndex = 6,
                TabNavigationMode = TabNavigationMode.ForwardTabPassive,
            };

            //	Dans l'ordre de droite à gauche:
            var buttonCancel = new Button(footer)
            {
                PreferredWidth = 75,
                Text = Epsitec.Common.Dialogs.Res.Strings.Dialog.Generic.Button.Cancel.ToString(),
                ButtonStyle = ButtonStyle.DefaultCancel,
                Dock = DockStyle.Right,
                Margins = new Margins(6, 0, 0, 0),
                TabIndex = 2,
                TabNavigationMode = TabNavigationMode.ActivateOnTab,
            };

            var buttonOk = new Button(footer)
            {
                PreferredWidth = 85,
                Text = this.ActionButtonName,
                ButtonStyle = ButtonStyle.DefaultAccept,
                Dock = DockStyle.Right,
                Margins = new Margins(6, 0, 0, 0),
                TabIndex = 1,
                TabNavigationMode = TabNavigationMode.ActivateOnTab,
            };

            buttonCancel.Clicked += this.HandleButtonCancelClicked;
            buttonOk.Clicked += this.HandleButtonOkClicked;
        }

        private void HandleButtonCancelClicked(object sender, MessageEventArgs e)
        {
            //	Bouton 'Annuler' cliqué.
            this.CloseWindow();
            this.result = DialogResult.Cancel;
        }

        private void HandleButtonOkClicked(object sender, MessageEventArgs e)
        {
            //	Bouton 'Ouvrir/Enregistrer' cliqué.
            this.CloseWindow();
            this.result = DialogResult.Accept;
        }

        private void HandleWindowCloseClicked(object sender)
        {
            //	Fenêtre fermée.
            this.CloseWindow();
        }

        private void CloseWindow()
        {
            this.window.Owner.MakeActive();
            this.window.Hide();
        }
        #endregion


        public static readonly string NewEmptyDocument = "#NewEmptyDocument#";

        private readonly FilterCollection filters;
        private readonly List<string> favorites;

        private DialogResult result;
        private string initialDirectory;
        private string filename;
        private string[] filenames;
        private bool isRedirected;

        protected Window window;
        protected Window owner;
        protected string title;
        protected bool enableNavigation;
        protected bool enableMultipleSelection;
        protected bool hasOptions;
        protected FileDialogType fileDialogType;
    }
}
