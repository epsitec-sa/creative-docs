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

namespace Epsitec.Common.Widgets
{
    public class WindowTitle : FrameBox
    {
        public WindowTitle()
        {
            this.PreferredHeight = 92;
            this.Dock = DockStyle.Top;
            this.BackColor = Color.FromBrightness(1);
        }

        protected override void ProcessMessage(Message message, Point pos)
        {
            switch (message.MessageType)
            {
                case MessageType.MouseDown:
                    this.Window.StartWindowManagerOperation(
                        Epsitec.Common.Widgets.Platform.WindowManagerOperation.MoveWindow
                    );
                    break;
            }
        }

        protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
        {
            var adorner = Epsitec.Common.Widgets.Adorners.Factory.Active;

            graphics.Color = this.BackColor;
            graphics.AddFilledRectangle(this.Client.Bounds);
            graphics.RenderSolid();

            graphics.Color = adorner.ColorBorder;
            graphics.LineWidth = 1.0;
            graphics.LineCap = CapStyle.Butt;
            graphics.AddLine(0, 0.5, this.Client.Size.Width, 0.5);
            graphics.RenderSolid();

            var font = Font.DefaultFont;
            var size = 14.0;

            graphics.Color = adorner.ColorCaption;
            graphics.PaintText(24, 32, this.Text, font, size);
        }
    }
}
