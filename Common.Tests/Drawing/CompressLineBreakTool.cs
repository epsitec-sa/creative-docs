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

using NUnit.Framework;

namespace Epsitec.Common.Tests.Drawing
{
    [TestFixture]
    public class CompressLineBreakTool
    {
        [Test]
        public void CompressLineBreak()
        {
            System.IO.FileStream stream = new System.IO.FileStream(
                @"Resources\LineBreak-4.1.0.txt",
                System.IO.FileMode.Open,
                System.IO.FileAccess.Read,
                System.IO.FileShare.Read
            );
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            stream.Close();

            System.IO.FileStream target = new System.IO.FileStream(
                @"Resources\LineBreak-4.1.0.compressed",
                System.IO.FileMode.Create,
                System.IO.FileAccess.Write,
                System.IO.FileShare.None
            );
            System.IO.Stream compressor = IO.Compression.CreateDeflateStream(target, 9);

            compressor.Write(buffer, 0, buffer.Length);
            compressor.Close();
            target.Close();
        }
    }
}
