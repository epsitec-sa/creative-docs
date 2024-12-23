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

using Epsitec.Common.Document;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Platform;

namespace Epsitec.Common.DocumentEditor
{
    /// <summary>
    /// La classe Application démarre l'éditeur de documents.
    /// </summary>
    public class Application : Epsitec.Common.Widgets.Application
    {
        public static void Start(string mode)
        {
            System.Diagnostics.Debug.Assert(mode != null);
            System.Diagnostics.Debug.Assert(mode.Length > 0);

            //	Evite de créer la liste des fontes complète maintenant si elle n'existe
            //	pas, puisque l'on veut tout d'abord être capable d'afficher le splash:
            Epsitec.Common.Text.TextContext.PostponeFullFontCollectionInitialization();

            Widget.Initialize();
            Application.mode = mode.Substring(0, 1);

            Res.Initialize();

            //	Il faut indiquer ci-après la date de diffusion du logiciel, qui doit
            //	être mise à jour chaque fois que l'on génère un nouveau CD :
            Common.Support.SerialAlgorithm.SetProductBuildDate(new System.DateTime(2013, 04, 19));
            Common.Support.SerialAlgorithm.SetProductGenerationNumber(84, 6); // Accepte CDID = 84

            Epsitec.Common.Document.Engine.Initialize();

            Common.Support.ImageProvider.Instance.EnableLongLifeCache = true;
            Common.Support.ImageProvider.Instance.PrefillManifestIconCache();

            switch (mode.Substring(1))
            {
                case "p":
                    Application.application = new Application(DocumentType.Pictogram);
                    break;
                default:
                    Application.application = new Application(DocumentType.Graphic);
                    break;
            }

            Window.RunEventLoop();
            Application.application.Dispose();
        }

        public Application(DocumentType type)
        {
            string windowTitle;
            switch (type)
            {
                case DocumentType.Graphic:
                    windowTitle = Res.Strings.Application.TitleDoc;
                    break;

                case DocumentType.Pictogram:
                    windowTitle = Res.Strings.Application.TitlePic;
                    break;

                case DocumentType.Text:
                    windowTitle = Res.Strings.Application.TitleTxt;
                    break;

                default:
                    windowTitle = Res.Strings.Application.TitleDoc;
                    break;
            }

            Window window = new Window(WindowFlags.Resizable, windowTitle: windowTitle);
            window.WindowClosed += this.HandleMainWindowClosed;

            this.editor = new DocumentEditor(
                type,
                this.CommandDispatcher,
                this.CommandContext,
                window
            );

            this.Window = window;

            // bl-net8-cross draganddrop
            // window.IsValidDropTarget = true;
            window.Icon = ImageProvider.Instance.GetImage(
                "manifest:Common.DocumentEditor/Images/Application.icon",
                null
            );
            window.AsyncNotification += this.HandleWindowAsyncNotification;
            window.WindowDragEntered += this.HandleMainWindowWindowDragEntered;
            window.WindowDragDropped += this.HandleMainWindowWindowDragDropped;

            if (type == DocumentType.Graphic)
            {
                window.Root.MinSize = new Size(410, 250);
            }
            else
            {
                window.Root.MinSize = new Size(430, 250);
            }

            var windowBounds = this.editor.GlobalSettings.MainWindowBounds;
            if (windowBounds.Location.IsZero)
            {
                // if the location is not set in GlobalSettings, we only set the size
                // the location will default to the screen center
                window.WindowSize = windowBounds.Size;
                window.CenterOnScreen();
            }
            else
            {
                window.WindowBounds = windowBounds;
            }
            window.IsFullScreen = this.editor.GlobalSettings.IsFullScreen;

            this.editor.PreferredSize = window.ClientSize;
            this.editor.Dock = DockStyle.Fill;
            this.editor.SetParent(window.Root);

            window.Show();
            window.MakeActive();

            this.editor.MakeReadyToRun();
        }

        public void HandleMainWindowClosed(object sender)
        {
            foreach (Window window in Window.GetAllLiveWindows())
            {
                window.Close();
            }
        }

        private void HandleWindowAsyncNotification(object sender)
        {
            this.editor.AsyncNotify();
        }

        public override string ShortWindowTitle
        {
            get { return this.editor.ShortWindowTitle; }
        }

        public override string ApplicationIdentifier
        {
            get { return "EpDocumentEditor"; }
        }

        public static string Mode
        {
            get { return Application.mode; }
        }

        public DocumentType Type
        {
            get { return this.editor.DocumentType; }
        }

        public static Application Current
        {
            get { return Application.application; }
        }

        protected override void ExecuteQuit(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            //	Evite que cette commande ne soit exécutée par Widgets.Application,
            //	car cela provoquerait la fin du programme, quelle que soit la
            //	réponse donnée par l'utilisateur au dialogue affiché par DocumentEditor.
        }

        private void HandleMainWindowWindowDragEntered(object sender, WindowDragEventArgs e)
        {
            string[] files = e.Data.ReadAsStringArray("FileDrop");
            if (files == null)
                return;
            e.AcceptDrop = false;

            if (files.Length > 0)
            {
                foreach (string file in files)
                {
                    if (
                        !Misc.IsExtension(file, this.DefaultExtension)
                        && !Misc.IsExtension(file, this.ModelExtension)
                        && !Misc.IsExtension(file, this.ColorsExtension)
                    )
                    {
                        return;
                    }
                }

                e.AcceptDrop = true;
            }
        }

        private void HandleMainWindowWindowDragDropped(object sender, WindowDragEventArgs e)
        {
            string[] files = e.Data.ReadAsStringArray("FileDrop");
            if (files == null)
                return;
            e.AcceptDrop = false;

            if (files.Length > 0)
            {
                foreach (string file in files)
                {
                    if (
                        !Misc.IsExtension(file, this.DefaultExtension)
                        && !Misc.IsExtension(file, this.ModelExtension)
                        && !Misc.IsExtension(file, this.ColorsExtension)
                    )
                    {
                        return;
                    }
                }

                e.AcceptDrop = true;

                foreach (string file in files)
                {
                    this.editor.Open(file);
                }
            }
        }

        protected string DefaultExtension
        {
            get
            {
                if (this.editor.DocumentType == DocumentType.Pictogram)
                {
                    return ".icon";
                }
                else
                {
                    return ".crdoc";
                }
            }
        }

        protected string ModelExtension
        {
            get
            {
                if (this.editor.DocumentType == DocumentType.Pictogram)
                {
                    return ".iconmod";
                }
                else
                {
                    return ".crmod";
                }
            }
        }

        protected string ColorsExtension
        {
            get { return ".crcolors"; }
        }

        private static Application application;
        private static string mode;

        private DocumentEditor editor;
    }
}
