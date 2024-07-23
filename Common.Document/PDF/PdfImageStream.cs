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


using System.Xml.Linq;

namespace Epsitec.Common.Document.PDF
{
    public class PdfImageStream : System.IDisposable
    {
        public PdfImageStream(string code, System.IO.Stream stream, int length, string path = null)
        {
            this.code = code;
            this.stream = stream;
            this.length = length;
            this.path = path;
        }

        public PdfImageStream(string code, StringBuffer data)
        {
            this.code = code;
            this.path = System.IO.Path.GetTempFileName();

            System.IO.File.WriteAllText(this.path, data.ToString(), System.Text.Encoding.Default);

            this.stream = System.IO.File.OpenRead(this.path);
            this.length = (int)this.stream.Length;
        }

        public string Code
        {
            get { return this.code; }
        }

        public System.IO.Stream Stream
        {
            get { return this.stream; }
        }

        public int StreamLength
        {
            get { return this.length; }
        }

        public static XElement ToXml(PdfImageStream item)
        {
            if (item == null)
            {
                return new XElement("pdfImageStream", new XAttribute("null", "true"));
            }
            else
            {
                return new XElement(
                    "pdfImageStream",
                    new XAttribute("code", item.code),
                    new XAttribute("path", item.path)
                );
            }
        }

        public static PdfImageStream FromXml(XElement root)
        {
            var xml = root.Element("pdfImageStream");

            if ((xml == null) || ((string)xml.Attribute("null") == "true"))
            {
                return null;
            }

            var code = (string)xml.Attribute("code");
            var path = (string)xml.Attribute("path");
            var stream = System.IO.File.OpenRead(path);
            var length = (int)stream.Length;

            return new PdfImageStream(code, stream, (int)length, path);
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (this.stream != null)
            {
                this.stream.Dispose();
            }

            if (this.path != null)
            {
                System.IO.File.Delete(this.path);
            }
        }

        #endregion


        private readonly string code;
        private readonly System.IO.Stream stream;
        private readonly int length;
        private readonly string path;
    }
}
