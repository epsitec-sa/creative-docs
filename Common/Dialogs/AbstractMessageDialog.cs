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
    /// La classe AbstractMessageDialog sert de base aux dialogues présentant
    /// un message avec des boutons de style oui/non.
    /// </summary>
    public abstract class AbstractMessageDialog : AbstractDialog
    {
        public AbstractMessageDialog() { }

        internal void HideCancelButton()
        {
            this.hideCancel = true;
        }

        public static void LayoutButtons(double width, params Widgets.Button[] buttons)
        {
            if (buttons.Length > 0)
            {
                double totalWidth = 0;

                for (int i = 0; i < buttons.Length; i++)
                {
                    if (buttons[i] != null)
                    {
                        totalWidth += buttons[i].ActualWidth;
                    }
                }

                totalWidth += (buttons.Length - 1) * 8;

                if (totalWidth < width)
                {
                    double x = System.Math.Floor((width - totalWidth) / 2);

                    for (int i = 0; i < buttons.Length; i++)
                    {
                        if (buttons[i] != null)
                        {
                            buttons[i]
                                .SetManualBounds(
                                    new Drawing.Rectangle(
                                        x,
                                        buttons[i].ActualLocation.Y,
                                        buttons[i].ActualWidth,
                                        buttons[i].ActualHeight
                                    )
                                );

                            x += buttons[i].ActualWidth;
                            x += 8;
                        }
                    }
                }
            }
        }

        protected bool hideCancel;
    }
}
