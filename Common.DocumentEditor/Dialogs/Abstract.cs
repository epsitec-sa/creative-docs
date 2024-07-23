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
using Epsitec.Common.Widgets.Platform;

namespace Epsitec.Common.DocumentEditor.Dialogs
{
    using GlobalSettings = Common.Document.Settings.GlobalSettings;

    /// <summary>
    /// Classe de base.
    /// </summary>
    public abstract class Abstract
    {
        public Abstract(DocumentEditor editor)
        {
            this.editor = editor;
            this.globalSettings = editor.GlobalSettings;
            this.window = null;
        }

        public virtual void Show()
        {
            //	Crée et montre la fenêtre du dialogue.
        }

        public virtual void Hide()
        {
            //	Cache la fenêtre du dialogue.
            if (this.window != null)
            {
                this.window.Hide();
            }
        }

        public virtual void Save()
        {
            //	Enregistre la position de la fenêtre du dialogue.
        }

        public virtual void Rebuild()
        {
            //	Reconstruit le dialogue.
        }

        protected void WindowInit(string name, double dx, double dy)
        {
            //	Initialise la fenêtre, à partir de la taille intérieure,
            //	c'est-à-dire sans le cadre.
            this.WindowInit(name, dx, dy, false);
        }

        protected void WindowInit(string name, double dx, double dy, bool resizable)
        {
            this.window.ClientSize = new Size(dx, dy);
            dx = this.window.WindowSize.Width;
            dy = this.window.WindowSize.Height; // taille avec le cadre

            Point location;
            Size size;
            if (this.globalSettings.GetWindowBounds(name, out location, out size))
            {
                if (resizable)
                {
                    this.window.ClientSize = size;
                    dx = this.window.WindowSize.Width;
                    dy = this.window.WindowSize.Height; // taille avec le cadre
                }

                Rectangle rect = new Rectangle(location, new Size(dx, dy));
                rect = ScreenInfo.FitIntoWorkingArea(rect);
                this.window.WindowBounds = rect;
            }
            else
            {
                Rectangle cb = this.CurrentBounds;
                Rectangle rect = new Rectangle(cb.Center.X - dx / 2, cb.Center.Y - dy / 2, dx, dy);
                this.window.WindowBounds = rect;
            }
        }

        protected void WindowSave(string name)
        {
            //	Sauve la fenêtre.
            if (this.window == null)
                return;
            this.globalSettings.SetWindowBounds(
                name,
                this.window.WindowLocation,
                this.window.ClientSize
            );
        }

        protected Rectangle CurrentBounds
        {
            //	Donne les frontières de l'application.
            get
            {
                if (this.editor.Window == null)
                {
                    return this.globalSettings.MainWindowBounds;
                }
                else
                {
                    return new Rectangle(
                        this.editor.Window.WindowLocation,
                        this.editor.Window.WindowSize
                    );
                }
            }
        }

        protected virtual void CloseWindow()
        {
            //	Ferme la fenêtre du dialogue.
            this.editor.Window.MakeActive();
            this.window.Hide();
            this.OnClosed();
        }

        protected virtual void OnClosed()
        {
            if (this.Closed != null)
            {
                this.Closed(this);
            }
        }

        public event Epsitec.Common.Support.EventHandler Closed;

        protected DocumentEditor editor;
        protected GlobalSettings globalSettings;
        protected Window window;
    }
}
