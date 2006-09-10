//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.IO
{
	public delegate void ChecksumCallback(IChecksum checksum);

	/// <summary>
	/// La classe Checksum permet d'accéder aux divers algorithmes de calcul
	/// de ... checksums sur des données binaires. Voir IChecksum.
	/// </summary>
	public class Checksum
	{
		private Checksum()
		{
		}

		public static long ComputeCrc32(ChecksumCallback callback)
		{
			if (Checksum.sharedCrc32 == null)
			{
				Checksum.sharedCrc32 = Checksum.CreateCrc32 ();
			}

			Checksum.sharedCrc32.Reset ();
			callback (Checksum.sharedCrc32);
			return Checksum.sharedCrc32.Value;
		}

		public static long ComputeAdler32(ChecksumCallback callback)
		{
			if (Checksum.sharedAdler32 == null)
			{
				Checksum.sharedAdler32 = Checksum.CreateAdler32 ();
			}

			Checksum.sharedAdler32.Reset ();
			callback (Checksum.sharedAdler32);
			return Checksum.sharedAdler32.Value;
		}
		
		
		public static IChecksum CreateCrc32()
		{
			return new Crc32Wrapper ();
		}
		
		public static IChecksum CreateAdler32()
		{
			return new Adler32Wrapper ();
		}
		
		
		protected class ChecksumWrapper : IChecksum
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

			public void Update(int byte_value)
			{
				if ((byte_value < 0) ||
					(byte_value > 255))
				{
					throw new System.ArgumentOutOfRangeException ("byte_value", byte_value, "The value must be in the 0..255 range.");
				}
				
				this.checksum.Update (byte_value);
			}
			
			public void Update(byte[] buffer)
			{
				this.checksum.Update (buffer);
			}

			public void Update(byte[] buffer, int offset, int length)
			{
				this.checksum.Update (buffer, offset, length);
			}
			
			
			public void UpdateValue(string value)
			{
				if (value != null)
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
		
		
		protected class Crc32Wrapper : ChecksumWrapper
		{
			public Crc32Wrapper() : base (new ICSharpCode.SharpZipLib.Checksums.Crc32 ())
			{
			}
		}
		
		protected class Adler32Wrapper : ChecksumWrapper
		{
			public Adler32Wrapper() : base (new ICSharpCode.SharpZipLib.Checksums.Adler32 ())
			{
			}
		}

		[System.ThreadStatic]
		private static IChecksum sharedCrc32;
		
		[System.ThreadStatic]
		private static IChecksum sharedAdler32;
	}
}
