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
using System.Linq;

namespace Epsitec.Common.BigList.Processors
{
    public sealed class KeyDownProcessor : EventProcessor
    {
        private KeyDownProcessor(IEventProcessorHost host)
        {
            this.host = host;
            this.policy = this.host.GetPolicy<KeyDownProcessorPolicy>();
            this.scrollingProcessor = this.host as IScrollingProcessor;
        }

        public static bool Attach(IEventProcessorHost host, Message message, Point pos)
        {
            if (host.EventProcessors.OfType<KeyDownProcessor>().Any())
            {
                return false;
            }

            if ((message.IsAltPressed) || (message.IsShiftPressed))
            {
                return false;
            }

            var proc = new KeyDownProcessor(host);

            proc.host.Add(proc);
            proc.Process(message, pos);

            return true;
        }

        protected override bool Process(Message message, Point pos)
        {
            switch (message.MessageType)
            {
                case MessageType.KeyDown:
                    return this.ProcessKeyDown(message);
            }

            return false;
        }

        private bool ProcessKeyDown(Message message)
        {
            if ((message.IsAltPressed) || (message.IsShiftPressed))
            {
                return false;
            }

            if (this.scrollingProcessor == null)
            {
                return false;
            }

            if (message.IsControlPressed)
            {
                switch (this.policy.PassiveScrollMode)
                {
                    case ScrollMode.MoveActive:
                    case ScrollMode.MoveFocus:
                    case ScrollMode.MoveVisible:
                        return this.ProcessScroll(
                            message.KeyCodeOnly,
                            this.policy.PassiveScrollMode
                        );
                }

                return false;
            }

            return this.ProcessScroll(message.KeyCodeOnly, ScrollMode.MoveActiveAndSelect);
        }

        private bool ProcessScroll(KeyCode code, ScrollMode scrollMode)
        {
            switch (code)
            {
                case KeyCode.Home:
                    this.scrollingProcessor.Scroll(
                        new Point(0, 1),
                        ScrollUnit.Document,
                        scrollMode
                    );
                    return true;

                case KeyCode.End:
                    this.scrollingProcessor.Scroll(
                        new Point(0, -1),
                        ScrollUnit.Document,
                        scrollMode
                    );
                    return true;

                case KeyCode.ArrowUp:
                    this.scrollingProcessor.Scroll(new Point(0, 1), ScrollUnit.Line, scrollMode);
                    return true;

                case KeyCode.ArrowDown:
                    this.scrollingProcessor.Scroll(new Point(0, -1), ScrollUnit.Line, scrollMode);
                    return true;

                case KeyCode.PageUp:
                    this.scrollingProcessor.Scroll(new Point(0, 1), ScrollUnit.Page, scrollMode);
                    return true;

                case KeyCode.PageDown:
                    this.scrollingProcessor.Scroll(new Point(0, -1), ScrollUnit.Page, scrollMode);
                    return true;
            }

            return false;
        }

        private readonly IEventProcessorHost host;
        private readonly IScrollingProcessor scrollingProcessor;
        private readonly KeyDownProcessorPolicy policy;
    }
}
