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
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

[assembly: DependencyClass(typeof(VSplitView))]

namespace Epsitec.Common.Widgets
{
    public partial class VSplitView
    {
        /// <summary>
        /// The <c>DragProcess</c> class is used to manage the dragging of the splitter.
        /// </summary>
        private sealed class DragProcessor
        {
            public DragProcessor(VSplitView view, Widget source, MessageEventArgs e)
            {
                this.view = view;
                this.source = source;

                this.originalY = this.source.MapClientToRoot(e.Point).Y;
                this.originalH = this.Offset;

                this.source.PreProcessing += this.HandleSourcePreProcessing;

                this.view.separator.Visibility = true;

                e.Message.Captured = true;
            }

            private double Offset
            {
                get { return this.view.frame1Container.PreferredHeight; }
                set { this.view.Ratio = value / this.view.Client.Height; }
            }

            private void HandleSourcePreProcessing(object sender, MessageEventArgs e)
            {
                if (e.Message.IsMouseType)
                {
                    switch (e.Message.MessageType)
                    {
                        case MessageType.MouseUp:
                            this.DragEnd();
                            break;

                        case MessageType.MouseMove:
                            this.Drag(e.Point);
                            break;
                    }
                }

                e.Cancel = true;
            }

            private void Drag(Point point)
            {
                var currentY = this.source.MapClientToRoot(point).Y;

                this.Offset = System.Math.Max(0, this.originalH - currentY + this.originalY);
            }

            private void DragEnd()
            {
                this.source.PreProcessing -= this.HandleSourcePreProcessing;

                double hMin = this.view.CollapseThreshold;
                double hMax = this.view.Client.Height - hMin;

                if (this.Offset < hMin)
                {
                    this.CollapseFrame1();
                }
                else if (this.Offset > hMax)
                {
                    this.CollapseFrame2();
                }
                else
                {
                    this.view.dragButton.Visibility = false;
                }
            }

            private void CollapseFrame1()
            {
                this.Offset = 0;

                this.view.dragButton.Visibility = true;
                this.view.dragButton.Parent = this.view.scroller2.Parent;
                this.view.dragButton.Dock = DockStyle.Top;

                this.view.separator.Visibility = false;
            }

            private void CollapseFrame2()
            {
                this.Offset = this.view.Client.Height;

                this.view.dragButton.Visibility = true;
                this.view.dragButton.Parent = this.view.scroller1.Parent;
                this.view.dragButton.Dock = DockStyle.Bottom;

                this.view.separator.Visibility = false;
            }

            private readonly VSplitView view;
            private readonly Widget source;
            private readonly double originalY;
            private readonly double originalH;
        }
    }
}
