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

namespace Epsitec.Common.IO
{
    public sealed class ByteStream : System.IO.Stream
    {
        // bl-net8-cross IGNOREFILE
        // those NotImplementedException where originally here, they are not stubs
        public ByteStream(long length, IEnumerable<byte> source)
        {
            this.length = length;
            this.source = source;

            this.enumerator = this.source.GetEnumerator();
        }

        public override bool CanRead
        {
            get { return true; }
        }
        public override bool CanSeek
        {
            get { return false; }
        }
        public override bool CanTimeout
        {
            get { return false; }
        }
        public override bool CanWrite
        {
            get { return false; }
        }
        public override long Length
        {
            get { return this.length; }
        }
        public override long Position
        {
            get { return this.pos; }
            set { throw new System.NotImplementedException(); }
        }

        public override void Flush() { }

        public override long Seek(long offset, System.IO.SeekOrigin origin)
        {
            throw new System.NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new System.NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new System.NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int read = 0;

            while ((count > 0) && (this.enumerator.MoveNext()))
            {
                buffer[offset++] = this.enumerator.Current;
                this.pos++;
                count--;
                read++;
            }

            return read;
        }

        public override void Close()
        {
            base.Close();
            this.enumerator.Dispose();
        }

        private readonly long length;
        private long pos;
        private readonly IEnumerable<byte> source;
        private readonly IEnumerator<byte> enumerator;
    }
}
