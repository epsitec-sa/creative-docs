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


namespace Epsitec.Common.IO
{
    /// <summary>
    /// La classe Compression donne accès aux divers streams comprimés
    /// (BZip2, GZip, Deflate = LZW & Huffman, ZIP).
    /// </summary>
    public class Compression : AbstractStream
    {
        protected Compression(System.IO.Stream stream)
            : base(stream) { }

        public override void Close()
        {
            System.IO.Stream stream = this.Stream;

            if (stream is ICSharpCode.SharpZipLib.BZip2.BZip2OutputStream)
            {
                ICSharpCode.SharpZipLib.BZip2.BZip2OutputStream compressor =
                    stream as ICSharpCode.SharpZipLib.BZip2.BZip2OutputStream;

                compressor.Flush();
            }
            else if (stream is ICSharpCode.SharpZipLib.Zip.ZipOutputStream)
            {
                ICSharpCode.SharpZipLib.Zip.ZipOutputStream compressor =
                    stream as ICSharpCode.SharpZipLib.Zip.ZipOutputStream;

                compressor.Flush();
                compressor.Finish();
            }
            else if (stream is ICSharpCode.SharpZipLib.Zip.Compression.Streams.DeflaterOutputStream)
            {
                ICSharpCode.SharpZipLib.Zip.Compression.Streams.DeflaterOutputStream compressor =
                    stream as ICSharpCode.SharpZipLib.Zip.Compression.Streams.DeflaterOutputStream;

                compressor.Flush();
                compressor.Finish();
            }
            else if (stream is ICSharpCode.SharpZipLib.GZip.GZipOutputStream)
            {
                ICSharpCode.SharpZipLib.GZip.GZipOutputStream compressor =
                    stream as ICSharpCode.SharpZipLib.GZip.GZipOutputStream;

                compressor.Flush();
                compressor.Finish();
            }

            base.Close();
        }

        public static void GZipCompressFile(string inputFilePath, string outputFilePath)
        {
            using (var inputStream = System.IO.File.OpenRead(inputFilePath))
            using (var outputStream = System.IO.File.Create(outputFilePath))
            using (
                var compressedStream = new System.IO.Compression.GZipStream(
                    outputStream,
                    System.IO.Compression.CompressionMode.Compress
                )
            )
            {
                inputStream.CopyTo(compressedStream);
            }
        }

        public static void GZipDecompressFile(string inputFilePath, string outputFilePath)
        {
            using (var inputStream = System.IO.File.OpenRead(inputFilePath))
            using (var outputStream = System.IO.File.Create(outputFilePath))
            using (
                var decompressedStream = new System.IO.Compression.GZipStream(
                    inputStream,
                    System.IO.Compression.CompressionMode.Decompress
                )
            )
            {
                decompressedStream.CopyTo(outputStream);
            }
        }

        public static System.IO.Stream CreateTransparentStream(System.IO.Stream stream)
        {
            byte[] header = new byte[]
            {
                (byte)'$',
                (byte)'N',
                (byte)'O',
                (byte)'N',
                (byte)'E',
                32,
                13,
                10
            };
            stream.Write(header, 0, 8);

            return stream;
        }

        public static System.IO.Stream CreateBZip2Stream(System.IO.Stream stream)
        {
            byte[] header = new byte[]
            {
                (byte)'$',
                (byte)'B',
                (byte)'Z',
                (byte)'I',
                (byte)'P',
                (byte)'2',
                13,
                10
            };
            stream.Write(header, 0, 8);

            ICSharpCode.SharpZipLib.BZip2.BZip2OutputStream compressor =
                new ICSharpCode.SharpZipLib.BZip2.BZip2OutputStream(stream);

            return new Compression(compressor);
        }

        public static System.IO.Stream CreateZipStream(System.IO.Stream stream, string name)
        {
            byte[] header = new byte[]
            {
                (byte)'$',
                (byte)'Z',
                (byte)'I',
                (byte)'P',
                32,
                32,
                13,
                10
            };
            stream.Write(header, 0, 8);

            ICSharpCode.SharpZipLib.Zip.ZipOutputStream compressor =
                new ICSharpCode.SharpZipLib.Zip.ZipOutputStream(stream);
            ICSharpCode.SharpZipLib.Zip.ZipEntry entry = new ICSharpCode.SharpZipLib.Zip.ZipEntry(
                name
            );

            compressor.SetLevel(9);
            compressor.PutNextEntry(entry);

            return new Compression(compressor);
        }

        public static System.IO.Stream CreateDeflateStream(System.IO.Stream stream, int level)
        {
            byte[] header = new byte[]
            {
                (byte)'$',
                (byte)'Z',
                (byte)'1',
                (byte)'9',
                (byte)'5',
                (byte)'1',
                13,
                10
            };
            stream.Write(header, 0, 8);

            ICSharpCode.SharpZipLib.Zip.Compression.Deflater deflater =
                new ICSharpCode.SharpZipLib.Zip.Compression.Deflater(level);
            ICSharpCode.SharpZipLib.Zip.Compression.Streams.DeflaterOutputStream compressor =
                new ICSharpCode.SharpZipLib.Zip.Compression.Streams.DeflaterOutputStream(
                    stream,
                    deflater
                );

            return new Compression(compressor);
        }

        public static System.IO.Stream CreateGZipStream(System.IO.Stream stream)
        {
            byte[] header = new byte[]
            {
                (byte)'$',
                (byte)'G',
                (byte)'Z',
                (byte)'I',
                (byte)'P',
                32,
                13,
                10
            };
            stream.Write(header, 0, 8);

            ICSharpCode.SharpZipLib.GZip.GZipOutputStream compressor =
                new ICSharpCode.SharpZipLib.GZip.GZipOutputStream(stream);

            return new Compression(compressor);
        }

        public static System.IO.Stream CreateStream(System.IO.Stream stream, Compressor compressor)
        {
            switch (compressor)
            {
                case Compressor.None:
                    return Compression.CreateTransparentStream(stream);
                case Compressor.BZip2:
                    return Compression.CreateBZip2Stream(stream);
                case Compressor.Zip:
                    return Compression.CreateZipStream(stream, "stream");
                case Compressor.DeflateQuick:
                    return Compression.CreateDeflateStream(stream, 0);
                case Compressor.DeflateCompact:
                    return Compression.CreateDeflateStream(stream, 9);
                case Compressor.GZip:
                    return Compression.CreateGZipStream(stream);
            }

            throw new System.ArgumentException(
                string.Format("Compressor {0} not recognized.", compressor),
                "compressor"
            );
        }
    }
}
