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


namespace Epsitec.Common.Widgets.Behaviors
{
    /// <summary>
    /// La classe DragBehavior implémente le comportement de dragging, à savoir
    /// détecter la condition de début de drag, gérer le déplacement et terminer
    /// le drag.
    /// </summary>
    public sealed class DragBehavior
    {
        public DragBehavior(
            IDragBehaviorHost host,
            Widget widget,
            bool isRelative = false,
            bool isZeroBased = false
        )
        {
            this.host = host;
            this.widget = widget;
            this.isRelative = isRelative;
            this.isZeroBased = isZeroBased;
        }

        public DragBehavior(Widget widget, bool isRelative = false, bool isZeroBased = false)
            : this(widget as IDragBehaviorHost, widget, isRelative, isZeroBased) { }

        public IDragBehaviorHost Host
        {
            get { return this.host; }
        }

        public Widget Widget
        {
            get { return this.widget; }
        }

        public bool IsDragging
        {
            get { return this.isDragging; }
        }

        public bool ProcessMessage(Message message, Drawing.Point pos)
        {
            if (this.widget.IsEnabled)
            {
                switch (message.MessageType)
                {
                    case MessageType.MouseDown:
                        if (this.isDragging)
                        {
                            message.Captured = true;
                            message.Consumer = this.widget;
                            message.Swallowed = true;
                        }
                        else
                        {
                            if (message.Button == MouseButtons.Left && message.ButtonDownCount == 1)
                            {
                                this.StartDragging(message, pos);
                                this.HandleDragging(message, pos);
                            }
                        }
                        break;

                    case MessageType.MouseUp:
                        if (message.Button == MouseButtons.Left)
                        {
                            this.StopDragging(message, pos);
                        }
                        break;

                    case MessageType.MouseMove:
                        if (Message.CurrentState.Buttons == MouseButtons.Left)
                        {
                            this.HandleDragging(message, pos);
                        }
                        break;

                    case MessageType.KeyDown:
                    case MessageType.KeyUp:
                        if (
                            message.KeyCode == KeyCode.AltKey
                            || message.KeyCode == KeyCode.ShiftKey
                            || message.KeyCode == KeyCode.ControlKey
                        )
                        {
                            this.HandleDragging(message, pos);
                        }
                        break;
                }
            }

            return message.Handled;
        }

        private void StartDragging(Message message, Drawing.Point pos)
        {
            if (this.isRelative)
            {
                if (this.host.OnDragBegin(pos) == false)
                {
                    return;
                }

                this.dragOffset = pos - this.host.DragLocation;
            }
            else
            {
                if (this.host.OnDragBegin(message.Cursor) == false)
                {
                    return;
                }

                this.dragOffset = message.Cursor - this.host.DragLocation;
            }

            message.Captured = true;
            message.Consumer = this.widget;

            this.isDragging = true;
        }

        private void StopDragging(Message message, Drawing.Point pos)
        {
            if (this.isDragging)
            {
                message.Consumer = this.widget;
                //?message.Swallowed = true;

                this.isDragging = false;

                this.host.OnDragEnd();
            }
        }

        private void HandleDragging(Message message, Drawing.Point pos)
        {
            if (this.isDragging)
            {
                message.Consumer = this.widget;

                if (this.isRelative)
                {
                    Drawing.Point oldPos = this.host.DragLocation;
                    Drawing.Point newPos = this.isZeroBased ? pos : pos - this.dragOffset;

                    this.host.OnDragging(new DragEventArgs(oldPos, newPos));
                }
                else
                {
                    Drawing.Point oldPos = this.host.DragLocation;
                    Drawing.Point newPos = this.isZeroBased
                        ? message.Cursor
                        : message.Cursor - this.dragOffset;

                    this.host.OnDragging(new DragEventArgs(oldPos, newPos));
                }
            }
        }

        private readonly IDragBehaviorHost host;
        private readonly Widget widget;

        private bool isDragging;
        private bool isRelative;
        private bool isZeroBased;

        private Drawing.Point dragOffset;
    }
}
