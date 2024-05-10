//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Michael WALZ, Maintainer: Pierre ARNAUD

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
