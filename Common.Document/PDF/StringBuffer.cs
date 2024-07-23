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


namespace Epsitec.Common.Document.PDF
{
    /// <summary>
    /// The <c>StringBuffer</c> class is used as a replacement of <see cref="System.StringBuilder"/>
    /// where the amount of memory used might be high; if the threshold is exceeded, the string
    /// will be stored in a backing file.
    /// </summary>
    public sealed class StringBuffer : System.IDisposable
    {
        public StringBuffer()
        {
            this.buffer = new System.Text.StringBuilder();
        }

        ~StringBuffer()
        {
            this.Dispose(false);
        }

        public bool InMemory
        {
            get { return this.path == null; }
        }

        public int Length
        {
            get { return this.length; }
        }

        public bool EndsWithWhitespace
        {
            get { return this.endsWithWhitespace; }
        }

        public void Append(string text)
        {
            if (this.length < 0)
            {
                throw new System.ObjectDisposedException("StringBuffer");
            }

            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            this.length += text.Length;

            if (text.Length > StringBuffer.Threshold)
            {
                this.FlushBuffer();
                this.Emit(text);

                this.endsWithWhitespace = char.IsWhiteSpace(text[text.Length - 1]);
            }
            else
            {
                this.buffer.Append(text);

                this.endsWithWhitespace = char.IsWhiteSpace(text[text.Length - 1]);

                if (this.length > StringBuffer.Threshold)
                {
                    this.FlushBuffer();
                }
            }
        }

        public void AppendNewLine()
        {
            this.Append("\r\n");
        }

        public override string ToString()
        {
            if (this.path != null)
            {
                this.FlushBuffer();

                int count = this.length;
                byte[] buffer = new byte[count];

                this.stream.Seek(0, System.IO.SeekOrigin.Begin);
                this.stream.Read(buffer, 0, count);

                return System.Text.Encoding.Default.GetString(buffer);
            }
            else
            {
                return this.buffer.ToString();
            }
        }

        public System.IO.Stream GetStream()
        {
            this.FlushBuffer();
            this.stream.Seek(0, System.IO.SeekOrigin.Begin);

            return this.stream;
        }

        public void CloseStream(System.IO.Stream stream)
        {
            System.Diagnostics.Debug.Assert(this.stream == stream);

            //	Dispose the string buffer; we won't use it anymore after this point.

            this.Dispose();
        }

        #region IDisposable Members

        public void Dispose()
        {
            System.GC.SuppressFinalize(this);
            this.Dispose(true);
        }

        #endregion

        private void Dispose(bool disposing)
        {
            if ((this.path != null) && (System.IO.File.Exists(this.path)))
            {
                if (this.stream != null)
                {
                    this.stream.Close();
                }

                System.IO.File.Delete(this.path);

                this.path = null;
                this.stream = null;
            }

            this.length = -1;
        }

        private void FlushBuffer()
        {
            if (this.path == null)
            {
                this.path = System.IO.Path.GetTempFileName();
                this.stream = System.IO.File.Open(this.path, System.IO.FileMode.Open);
            }

            if (this.buffer.Length > 0)
            {
                this.stream.Seek(0, System.IO.SeekOrigin.End);
                this.Emit(this.buffer.ToString());
            }

            this.stream.Flush();
        }

        private void Emit(string text)
        {
            byte[] data = System.Text.Encoding.Default.GetBytes(text);
            this.stream.Write(data, 0, data.Length);
            this.buffer.Length = 0;
        }

        private const int Threshold = 1024 * 4;

        private readonly System.Text.StringBuilder buffer;

        private System.IO.Stream stream;
        private int length;
        private string path;
        private bool endsWithWhitespace;
    }
}
