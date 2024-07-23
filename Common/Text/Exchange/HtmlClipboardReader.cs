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


namespace Epsitec.Common.Text.Exchange
{
    public static class NativeHtmlClipboardReader
    {
        // ********************************************************************
        // TODO bl-net8-cross clipboard
        // - implement NativeHtmlClipboardReader (stub)
        // ********************************************************************
        public static string Read()
        {
            /*
            byte[] clipboardBytes =
                Epsitec.Common.Support.Platform.Win32.Clipboard.ReadHtmlFormat();

            if (clipboardBytes == null)
            {
                return string.Empty;
            }

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            for (int i = 0; i < 200; i++)
            {
                sb.Append((char)clipboardBytes[i]);
            }

            string startText = sb.ToString();

            const string startHtml = "StartHTML:";
            int startHtmlIndex = startText.IndexOf(startHtml);
            int index = 0;

            if (startHtmlIndex > 0)
            {
                startHtmlIndex += startHtml.Length;
                string htmlIndexValue = startText.Substring(startHtmlIndex, 15); // 15 == bonne reserve pour la longueur de l'offset
                index = Misc.ParseInt(htmlIndexValue);
            }

            return System.Text.Encoding.UTF8.GetString(
                clipboardBytes,
                index,
                clipboardBytes.Length - index
            );
            */
            return null;
        }
    }
}
