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
using Epsitec.Common.BigList;
using Epsitec.Common.BigList.Processors;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Layouts;
using Epsitec.Common.Widgets.Platform;

[assembly: DependencyClass(typeof(ItemListColumnHeaderView))]

namespace Epsitec.Common.BigList
{
    public partial class ItemListColumnHeaderView
    {
        private class ViewEventProcessor
            : IEventProcessorHost,
                IDetectionProcessor,
                ISelectionProcessor,
                IDraggingProcessor
        {
            public ViewEventProcessor(ItemListColumnHeaderView view)
            {
                this.view = view;
            }

            public bool ProcessMessage(Message message, Point pos)
            {
                if (this.view.Columns.Any() == false)
                {
                    return false;
                }

                if (this.processor != null)
                {
                    if (this.processor.ProcessMessage(message, pos))
                    {
                        return true;
                    }
                }

                switch (message.MessageType)
                {
                    case MessageType.MouseEnter:
                    case MessageType.MouseHover:
                    case MessageType.MouseLeave:
                        break;

                    case MessageType.MouseDown:
                        return this.ProcessMouseDown(message, pos);

                    case MessageType.MouseUp:
                        break;

                    case MessageType.MouseMove:
                        break;
                }

                return false;
            }

            public void PaintOverlay(Graphics graphics, Rectangle clipRect)
            {
                if (this.processor != null)
                {
                    this.processor.PaintOverlay(graphics, clipRect);
                }
            }

            private bool ProcessMouseDown(Message message, Point pos)
            {
                var drag = this.DetectDrag(pos).ToArray();

                return MouseDragProcessor.Attach(this, message, pos, drag)
                    || MouseDownProcessor.Attach(this, this.view.Client.Bounds, message, pos);
            }

            #region IEventProcessorHost Members

            IEnumerable<IEventProcessor> IEventProcessorHost.EventProcessors
            {
                get
                {
                    if (this.processor != null)
                    {
                        yield return this.processor;
                    }
                }
            }

            void IEventProcessorHost.Add(IEventProcessor processor)
            {
                this.processor = processor;
            }

            void IEventProcessorHost.Remove(IEventProcessor processor)
            {
                if (this.processor == processor)
                {
                    this.processor = null;
                }
            }

            TPolicy IEventProcessorHost.GetPolicy<TPolicy>()
            {
                return this.view.GetPolicy<TPolicy>();
            }

            #endregion

            #region IDetectionProcessor Members

            public int Detect(Point pos)
            {
                var column = this.view.Columns.DetectColumn(pos);

                if (column == null)
                {
                    return -1;
                }
                else
                {
                    return column.Index;
                }
            }

            #endregion

            #region ISelectionProcessor Members

            public bool IsSelected(int index)
            {
                return false;
            }

            public void Select(int index, ItemSelection selection)
            {
                var column = this.view.Columns.GetColumn(index);

                if (column == null)
                {
                    return;
                }

                switch (selection)
                {
                    case ItemSelection.Activate:
                        break;

                    case ItemSelection.Focus:
                        break;

                    case ItemSelection.Select:
                    case ItemSelection.Deselect:
                        break;

                    case ItemSelection.Toggle:
                        this.view.SelectSortColumn(column);
                        this.view.Invalidate();
                        break;
                }
            }

            #endregion

            #region IDraggingProcessor Members

            public IEnumerable<MouseDragFrame> DetectDrag(Point pos)
            {
                foreach (var column in this.view.Columns)
                {
                    var det = 2.0;
                    var def = column.Layout.Definition;
                    var left = def.ActualOffset;
                    var right = left + def.ActualWidth;
                    var rect = this.view.GetColumnBounds(column);

                    var elasticity = def.Width.IsAbsolute
                        ? MouseDragElasticity.None
                        : MouseDragElasticity.Stretch;

                    if ((pos.X >= left - det) && (pos.X <= left + det))
                    {
                        yield return new MouseDragFrame(
                            column.Index,
                            GripId.EdgeLeft,
                            rect,
                            MouseDragDirection.Horizontal,
                            Rectangle.FromPoints(0, rect.Bottom, rect.Right, rect.Top),
                            elasticity
                        );
                    }

                    if ((pos.X >= right - det) && (pos.X <= right + det))
                    {
                        var minWidth = def.MinWidth;
                        var maxWidth = def.MaxWidth;

                        yield return new MouseDragFrame(
                            column.Index,
                            GripId.EdgeRight,
                            rect,
                            MouseDragDirection.Horizontal,
                            Rectangle.FromPoints(
                                rect.Left + minWidth,
                                rect.Bottom,
                                System.Math.Min(this.view.Client.Width, rect.Left + maxWidth),
                                rect.Top
                            ),
                            elasticity
                        );
                    }
                }
            }

            void IDraggingProcessor.ApplyDrag(
                MouseDragFrame originalFrame,
                MouseDragFrame currentFrame
            )
            {
                if (originalFrame.Grip == GripId.EdgeRight)
                {
                    var column = this.view.Columns.GetColumn(originalFrame.Index);

                    column.Layout.Definition.Width = new GridLength(
                        currentFrame.Bounds.Width,
                        GridUnitType.Absolute
                    );

                    this.view.RefreshColumnLayout();
                }
            }

            #endregion

            private readonly ItemListColumnHeaderView view;
            private IEventProcessor processor;
        }
    }
}
