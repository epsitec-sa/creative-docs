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


using System;
using static SDL2.SDL;

namespace Epsitec.Common.Widgets.Platform
{
    /// <summary>
    /// La classe ScreenInfo fournit les informations au sujet d'un écran.
    /// </summary>
    public class ScreenInfo
    {
        public ScreenInfo(int displayIndex)
        {
            this.displayIndex = displayIndex;
        }

        /// <summary>
        /// Retourne le rectangle correspondant à l'écran par rapport à la surface
        /// de travail globale.
        /// </summary>
        public Drawing.Rectangle Bounds
        {
            get
            {
                SDL_Rect bounds;
                SDL_GetDisplayBounds(this.displayIndex, out bounds);
                return new Drawing.Rectangle(bounds.x, bounds.y, bounds.w, bounds.h);
            }
        }

        /// <summary>
        /// Retourne le rectangle correspondant à la surface de travail sur l'écran
        /// par rapport à la surface de travail globale. L'espace pris par la barre
        /// des tâches est automatiquement enlevé.
        /// </summary>
        public Drawing.Rectangle WorkingArea
        {
            get
            {
                SDL_Rect bounds;
                SDL_GetDisplayUsableBounds(this.displayIndex, out bounds);
                return new Drawing.Rectangle(bounds.x, bounds.y, bounds.w, bounds.h);
            }
        }

        /// <summary>
        /// Indique s'il s'agit de l'écran principal (celui où il y a la barre des
        /// tâches).
        /// </summary>
        public bool IsPrimary
        {
            get { return this.displayIndex == 0; }
        }

        public string DeviceName
        {
            get { return SDL_GetDisplayName(this.displayIndex); }
        }

        /// <summary>
        /// Retourne le rectangle correspondant à la surface de travail globale. Cette
        /// surface peut avoir une origine négative...
        /// </summary>
        public static Drawing.Rectangle GlobalArea
        {
            get { throw new NotImplementedException(); }
            /*            get
                        {
                            int ox = System.Windows.Forms.SystemInformation.VirtualScreen.Left;
                            int oy =
                                (int)ScreenInfo.PrimaryHeight
                                - System.Windows.Forms.SystemInformation.VirtualScreen.Bottom;
                            int dx = System.Windows.Forms.SystemInformation.VirtualScreen.Width;
                            int dy = System.Windows.Forms.SystemInformation.VirtualScreen.Height;
            
                            return new Drawing.Rectangle(ox, oy, dx, dy);
                        }
            */
        }

        /// <summary>
        /// Construit la table de tous les écrans disponibles.
        /// </summary>
        public static ScreenInfo[] AllScreens
        {
            get
            {
                int numDisplay = SDL_GetNumVideoDisplays();
                if (numDisplay < 1)
                {
                    throw new InvalidOperationException(SDL_GetError());
                }
                ScreenInfo[] infos = new ScreenInfo[numDisplay];

                for (int i = 0; i < numDisplay; i++)
                {
                    infos[i] = new ScreenInfo(i);
                }

                return infos;
            }
        }

        /// <summary>
        /// Trouve l'écran qui se trouve au point indiqué.
        /// </summary>
        /// <param name="point">position absolue</param>
        /// <returns>écran trouvé</returns>
        public static ScreenInfo Find(Drawing.Point point)
        {
            // use SDL_GetPointDisplayIndex when the sdl library is updated to >= 2.24.0
            return MainScreen;
        }

        /// <summary>
        /// Trouve l'écran qui est le plus recouvert par le rectangle indiqué.
        /// </summary>
        /// <param name="rect">rectangle à tester</param>
        /// <returns>écran trouvé</returns>
        public static ScreenInfo Find(Drawing.Rectangle rect)
        {
            // use SDL_GetRectDisplayIndex when the sdl library is updated to >= 2.24.0
            return MainScreen;
        }

        /// <summary>
        /// Fits the specified rectangle into the working area. This can be used
        /// to make a window fully visible.
        /// </summary>
        /// <param name="rect">The rectangle to fit.</param>
        /// <returns>The adjusted rectangle.</returns>
        public static Drawing.Rectangle FitIntoWorkingArea(Drawing.Rectangle rect)
        {
            ScreenInfo si = Find(rect.Center);
            Drawing.Rectangle area = si.WorkingArea;

            rect.Width = System.Math.Min(rect.Width, area.Width);
            rect.Height = System.Math.Min(rect.Height, area.Height);

            if (rect.Left < area.Left) // dépasse à gauche ?
            {
                rect.Offset(area.Left - rect.Left, 0);
            }

            if (rect.Right > area.Right) // dépasse à droite ?
            {
                rect.Offset(area.Right - rect.Right, 0);
            }

            if (rect.Bottom < area.Bottom) // dépasse en bas ?
            {
                rect.Offset(0, area.Bottom - rect.Bottom);
            }

            if (rect.Top > area.Top) // dépasse en haut ?
            {
                rect.Offset(0, area.Top - rect.Top);
            }

            return rect;
        }

        public static readonly ScreenInfo MainScreen = new ScreenInfo(0); // the main display is always at index 0

        private int displayIndex;
    }
}
