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


using Epsitec.Common.Types;

[assembly: DependencyClass(typeof(Epsitec.Common.Widgets.Behaviors.CopyPasteBehavior))]

namespace Epsitec.Common.Widgets.Behaviors
{
    /// <summary>
    /// La classe CopyPasteBehavior gère le copier & coller.
    /// </summary>
    public sealed class CopyPasteBehavior : DependencyObject
    {
        // ********************************************************************
        // TODO bl-net8-cross clipboard
        // - implement CopyPasteBehavior (stub)
        // ********************************************************************
        public CopyPasteBehavior(Widget host)
        {
            this.host = host;
        }

        public Widget Host
        {
            get { return this.host; }
        }

        public bool IsRichTextEnabled
        {
            get
            {
                //	To enable rich text pasting, the widget must have the RichTextEnabled
                //	property attached to it and set to true !

                return (bool)this.host.GetValue(CopyPasteBehavior.RichTextEnabledProperty);
            }
        }

        public bool ProcessMessage(Message message, Drawing.Point pos)
        {
            if (this.host.IsEnabled)
            {
                if (message.MessageType == MessageType.KeyPress)
                {
                    if (message.IsControlPressed)
                    {
                        switch (message.KeyCode)
                        {
                            case KeyCode.AlphaC:
                                if (this.ProcessCopy())
                                {
                                    message.Consumer = this.host;
                                }
                                break;

                            case KeyCode.AlphaV:
                                if (this.ProcessPaste())
                                {
                                    message.Consumer = this.host;
                                }
                                break;

                            case KeyCode.AlphaX:
                                if (this.ProcessCopy() && this.ProcessDelete())
                                {
                                    message.Consumer = this.host;
                                }
                                break;

                            case KeyCode.AlphaA:
                                if (this.ProcessSelectAll())
                                {
                                    message.Consumer = this.host;
                                }
                                break;
                        }
                    }
                }
            }

            return message.Handled;
        }

        public bool ProcessCopy()
        {
            /*
            AbstractTextField text = this.host as AbstractTextField;

            if (text != null)
            {
                string value = text.Selection;

                if (value == "")
                {
                    value = text.Text;
                }

                Support.ClipboardWriteData data = new Support.ClipboardWriteData();

                data.WriteTextLayout(value);
                data.WriteHtmlFragment(value);
                Support.Clipboard.SetData(data);

                return true;
            }

            return false;
            */
            return true;
        }

        public bool ProcessPaste()
        {
            /*
            AbstractTextField text = this.host as AbstractTextField;

            if (text != null)
            {
                Support.ClipboardReadData data = Support.Clipboard.GetData();

                string html = null;

                if (this.IsRichTextEnabled)
                {
                    string textLayout = data.ReadTextLayout();

                    if (textLayout != null)
                    {
                        html = textLayout;
                    }
                    else
                    {
                        html = data.ReadHtmlFragment();

                        if (html != null)
                        {
                            html = Support.Clipboard.ConvertHtmlToSimpleXml(html);
                        }
                        else
                        {
                            html = TextLayout.ConvertToTaggedText(data.ReadText());
                        }
                    }
                }
                else
                {
                    html = TextLayout.ConvertToTaggedText(data.ReadText());
                }

                if (html != null)
                {
                    if (text.TextFieldStyle != TextFieldStyle.Multiline)
                    {
                        html = html.Replace("<br/>", " ");
                    }

                    text.Selection = html;
                    text.SimulateEdited();
                    return true;
                }
            }

            return false;
            */
            return true;
        }

        public bool ProcessDelete()
        {
            AbstractTextField text = this.host as AbstractTextField;

            if (text != null)
            {
                string value = text.Selection;

                if (value == "")
                {
                    text.SelectAll();
                }

                text.TextNavigator.DeleteSelection();
                text.SimulateEdited();

                return true;
            }

            return false;
        }

        public bool ProcessSelectAll()
        {
            AbstractTextField text = this.host as AbstractTextField;

            if (text != null)
            {
                text.SelectAll();
                return true;
            }

            return false;
        }

        public static readonly DependencyProperty RichTextEnabledProperty =
            DependencyProperty.RegisterAttached(
                "RichTextEnabled",
                typeof(bool),
                typeof(CopyPasteBehavior),
                new DependencyPropertyMetadata(false)
            );

        readonly Widget host;
    }
}
