//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

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
			BinaryFormatter        formatter  = new BinaryFormatter ();
			System.IO.MemoryStream memory     = new System.IO.MemoryStream ();
			System.IO.Stream       compressed = Compression.CreateStream (memory, compressor);
			
			formatter.Serialize (compressed, o);
			
			compressed.Close ();
			memory.Close ();
			
			return memory.ToArray ();
		}
		
		public static object DeserializeAndDecompressFromMemory(byte[] buffer)
		{
			BinaryFormatter        formatter    = new BinaryFormatter ();
			System.IO.MemoryStream memory       = new System.IO.MemoryStream(buffer, 0, buffer.Length, false, false);
			System.IO.Stream       decompressed = Decompression.CreateStream (memory);
			
			object o = formatter.Deserialize (decompressed);
			
			decompressed.Close ();
			memory.Close ();
			
			return o;
		}
	}
}
