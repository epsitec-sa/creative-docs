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
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Dialogs.Helpers
{
    /// <summary>
    /// The <c>MessageBuilder</c> class is used to create pieces of messages
    /// displayed in message boxes.
    /// </summary>
    public static class MessageBuilder
    {
        public static Widget CreateIconAndText(string iconUri, FormattedText message)
        {
            Widget container = new Widget();

            if (string.IsNullOrEmpty(iconUri))
            {
                MessageBuilder.CreateTextOnly(container, message);
            }
            else
            {
                MessageBuilder.CreateIconAndText(container, iconUri, message);
            }

            return container;
        }

        public static string GetIconUri(DialogIcon icon)
        {
            switch (icon)
            {
                case DialogIcon.Warning:
                case DialogIcon.Question:
                    return string.Concat("manifest:Common/Dialogs/Images/", icon, ".icon");

                default:
                    return null;
            }
        }

        public static FormattedText FormatMessage(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }
            else
            {
                text = text.Replace("\r", "");
                text = TextLayout.ConvertToTaggedText(text);

                return text;
            }
        }

        public static string GetDialogTitle(Window owner)
        {
            Application application = Widgets.Helpers.VisualTree.GetApplication(owner);

            if (application == null)
            {
                return Res.Strings.Dialog.Generic.Title.ToSimpleText();
            }
            else
            {
                return application.ShortWindowTitle;
            }
        }

        private static void CreateTextOnly(Widget container, FormattedText message)
        {
            var widgetText = new StaticText() { Parent = container, FormattedText = message };

            double minWidth = System.Math.Min(
                400,
                widgetText.TextLayout.GetSingleLineSize().Width + 4
            );

            widgetText.TextLayout.LayoutSize = new Drawing.Size(minWidth, TextLayout.Infinite);

            double width = widgetText.TextLayout.LayoutSize.Width;
            double height = System.Math.Ceiling(widgetText.TextLayout.TotalRectangle.Height + 4);

            widgetText.TextBreakMode = Drawing.TextBreakMode.Hyphenate;
            widgetText.Anchor = AnchorStyles.All;

            container.PreferredSize = new Drawing.Size(width, height);
        }

        private static void CreateIconAndText(
            Widget container,
            string iconUri,
            FormattedText message
        )
        {
            var widgetText = new StaticText() { Parent = container, FormattedText = message, };

            var widgetIcon = new StaticImage()
            {
                Parent = container,
                ImageName = iconUri,
                PreferredSize = new Drawing.Size(48, 48),
                Anchor = AnchorStyles.TopLeft,
                Margins = new Drawing.Margins(0, 0, 0, 0)
            };

            double minWidth = System.Math.Min(
                400,
                widgetText.TextLayout.GetSingleLineSize().Width + 4
            );

            widgetText.TextLayout.LayoutSize = new Drawing.Size(minWidth, TextLayout.Infinite);

            double width = widgetText.TextLayout.LayoutSize.Width;
            double height = System.Math.Ceiling(widgetText.TextLayout.TotalRectangle.Height + 4);

            widgetText.TextBreakMode = Drawing.TextBreakMode.Hyphenate;
            widgetText.Anchor = AnchorStyles.All;
            widgetText.Margins = new Drawing.Margins(widgetIcon.PreferredWidth + 8, 0, 0, 0);

            container.PreferredSize = new Drawing.Size(
                widgetIcon.PreferredWidth + 8 + width,
                System.Math.Max(widgetIcon.PreferredHeight, height)
            );
        }
    }
}
