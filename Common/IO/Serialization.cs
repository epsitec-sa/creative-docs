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


using System.Runtime.Serialization.Formatters.Binary;

namespace Epsitec.Common.IO
{
    /// <summary>
    /// The <c>Serialization</c> class is a thin wrapper above .NET's own
    /// object serializaion/deserialization mechanisms, which uses compresed
    /// byte arrays for its storage needs.
    /// </summary>
    public static class Serialization
    {
        public static byte[] SerializeAndCompressToMemory(object o, Compressor compressor)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            System.IO.MemoryStream memory = new System.IO.MemoryStream();
            System.IO.Stream compressed = Compression.CreateStream(memory, compressor);

            formatter.Serialize(compressed, o);

            compressed.Close();
            memory.Close();

            return memory.ToArray();
        }

        public static object DeserializeAndDecompressFromMemory(byte[] buffer)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            System.IO.MemoryStream memory = new System.IO.MemoryStream(
                buffer,
                0,
                buffer.Length,
                false,
                false
            );
            System.IO.Stream decompressed = Decompression.CreateStream(memory);

            object o = formatter.Deserialize(decompressed);

            decompressed.Close();
            memory.Close();

            return o;
        }

        /// <summary>
        /// Deserializes the instance and decompress it from a byte array already
        /// loaded in memory.
        /// </summary>
        /// <typeparam name="T">The expected type (or base type).</typeparam>
        /// <param name="buffer">The buffer.</param>
        /// <returns>The deserialized instance or <c>null</c>.</returns>
        public static T DeserializeAndDecompressFromMemory<T>(byte[] buffer)
            where T : class
        {
            if (buffer == null)
            {
                return null;
            }
            else
            {
                return Serialization.DeserializeAndDecompressFromMemory(buffer) as T;
            }
        }

        public static byte[] SerializeToMemory(object o)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            System.IO.MemoryStream memory = new System.IO.MemoryStream();

            formatter.Serialize(memory, o);

            memory.Close();

            return memory.ToArray();
        }

        public static object DeserializeFromMemory(byte[] buffer)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            System.IO.MemoryStream memory = new System.IO.MemoryStream(
                buffer,
                0,
                buffer.Length,
                false,
                false
            );

            object o = formatter.Deserialize(memory);

            memory.Close();

            return o;
        }

        public static T DeserializeFromMemory<T>(byte[] buffer)
            where T : class
        {
            if (buffer == null)
            {
                return null;
            }
            else
            {
                return Serialization.DeserializeFromMemory(buffer) as T;
            }
        }
    }
}
