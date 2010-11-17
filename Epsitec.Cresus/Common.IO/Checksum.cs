//	Copyright © 2004-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Security.Cryptography;

namespace Epsitec.Common.IO
{
	public delegate void ChecksumCallback(IChecksum checksum);

	/// <summary>
	/// La classe Checksum permet d'accéder aux divers algorithmes de calcul
	/// de ... checksums sur des données binaires. Voir IChecksum.
	/// </summary>
	public static class Checksum
	{
		public static int ComputeCrc32(byte[] data)
		{
			return Checksum.ComputeCrc32 (engine => engine.Update (data));
		}

		public static int ComputeCrc32(ChecksumCallback callback)
		{
			if (Checksum.sharedCrc32 == null)
			{
				Checksum.sharedCrc32 = Checksum.CreateCrc32 ();
			}

			Checksum.sharedCrc32.Reset ();

			callback (Checksum.sharedCrc32);

			return (int) (Checksum.sharedCrc32.Value & 0x7fffffff);
		}

		public static int ComputeAdler32(byte[] data, int length = 0)
		{
			if (length == 0)
            {
				length = data.Length;
            }
			else
			{
				length = System.Math.Min (length, data.Length);
			}

			return Checksum.ComputeAdler32 (engine => engine.Update (data, 0, length));
		}

		public static int ComputeAdler32(ChecksumCallback callback)
		{
			if (Checksum.sharedAdler32 == null)
			{
				Checksum.sharedAdler32 = Checksum.CreateAdler32 ();
			}

			Checksum.sharedAdler32.Reset ();
			
			callback (Checksum.sharedAdler32);
			
			return (int) (Checksum.sharedAdler32.Value & 0x7fffffff);
		}

		public static string ComputeMd5Hash(byte[] data)
		{
			if (Checksum.sharedMd5 == null)
			{
				Checksum.sharedMd5 = new MD5CryptoServiceProvider ();
			}

			byte[] hash = Checksum.sharedMd5.ComputeHash (data);
			return Ascii85.Encode (hash, outputMarks: false);
		}
		
		
		public static IChecksum CreateCrc32()
		{
			return new Crc32Wrapper ();
		}
		
		public static IChecksum CreateAdler32()
		{
			return new Adler32Wrapper ();
		}


		#region ChecksumWrapper Class

		private abstract class ChecksumWrapper : IChecksum
		{
			protected ChecksumWrapper(ICSharpCode.SharpZipLib.Checksums.IChecksum checksum)
			{
				this.checksum = checksum;
			}
			
			
			public long							Value
			{
				get
				{
					return this.checksum.Value;
				}
			}

			
			public void Reset()
			{
				this.checksum.Reset ();
			}

			public void Update(int byteValue)
			{
				if ((byteValue < 0) ||
					(byteValue > 255))
				{
					throw new System.ArgumentOutOfRangeException ("byte_value", byteValue, "The value must be in the 0..255 range.");
				}
				
				this.checksum.Update (byteValue);
			}
			
			public void Update(byte[] buffer)
			{
				if ((buffer != null) &&
					(buffer.Length > 0))
				{
					this.checksum.Update (buffer);
				}
			}

			public void Update(byte[] buffer, int offset, int length)
			{
				if (length > 0)
				{
					this.checksum.Update (buffer, offset, length);
				}
			}
			
			
			public void UpdateValue(string value)
			{
				if (!string.IsNullOrEmpty (value))
				{
					this.Update (System.Text.Encoding.UTF8.GetBytes (value));
				}
			}
			
			public void UpdateValue(string[] values)
			{
				if (values != null)
				{
					foreach (string s in values)
					{
						this.UpdateValue (s);
					}
				}
			}
			
			public void UpdateValue(int value)
			{
				this.buffer[0] = (byte) (value >> 24);
				this.buffer[1] = (byte) (value >> 16);
				this.buffer[2] = (byte) (value >>  8);
				this.buffer[3] = (byte) (value >>  0);
				
				this.Update (this.buffer, 0, 4);
			}
			
			public void UpdateValue(short value)
			{
				this.buffer[0] = (byte) (value >>  8);
				this.buffer[1] = (byte) (value >>  0);
				
				this.Update (this.buffer, 0, 2);
			}
			
			public void UpdateValue(double value)
			{
				this.doubles[0] = value;
				
				int count = System.Buffer.ByteLength (this.doubles);
				
				System.Buffer.BlockCopy (this.doubles, 0, this.buffer, 0, count);
				
				this.Update (buffer, 0, count);
			}
			
			public void UpdateValue(bool value)
			{
				this.Update (value ? 1 : 0);
			}
			
			
			ICSharpCode.SharpZipLib.Checksums.IChecksum checksum;
			
			private double[]					doubles = new double[1];
			private byte[]						buffer = new byte[32];
		}

		#endregion

		#region Crc32Wrapper Class

		private class Crc32Wrapper : ChecksumWrapper
		{
			public Crc32Wrapper()
				: base (new ICSharpCode.SharpZipLib.Checksums.Crc32 ())
			{
			}
		}

		#endregion

		#region Adler32Wrapper

		private class Adler32Wrapper : ChecksumWrapper
		{
			public Adler32Wrapper()
				: base (new ICSharpCode.SharpZipLib.Checksums.Adler32 ())
			{
			}
		}

		#endregion

		[System.ThreadStatic]
		private static IChecksum sharedCrc32;
		
		[System.ThreadStatic]
		private static IChecksum sharedAdler32;

		[System.ThreadStatic]
		private static MD5CryptoServiceProvider sharedMd5;
	}
}
