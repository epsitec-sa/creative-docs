//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 15/04/2004

namespace Epsitec.Common.Support.IO
{
	/// <summary>
	/// La classe Checksum permet d'accéder aux divers algorithmes de calcul
	/// de ... checksums sur des données binaires. Voir IChecksum.
	/// </summary>
	public class Checksum
	{
		private Checksum()
		{
		}
		
		
		public static IChecksum CreateCrc32()
		{
			return new Crc32Wrapper ();
		}
		
		public static IChecksum CreateAdler32()
		{
			return new Adler32Wrapper ();
		}
		
		
		protected class IChecksumWrapper : IChecksum
		{
			protected IChecksumWrapper(ICSharpCode.SharpZipLib.Checksums.IChecksum checksum)
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
			
			
			ICSharpCode.SharpZipLib.Checksums.IChecksum checksum;
		}
		
		
		protected class Crc32Wrapper : IChecksumWrapper
		{
			public Crc32Wrapper() : base (new ICSharpCode.SharpZipLib.Checksums.Crc32 ())
			{
			}
		}
		
		protected class Adler32Wrapper : IChecksumWrapper
		{
			public Adler32Wrapper() : base (new ICSharpCode.SharpZipLib.Checksums.Adler32 ())
			{
			}
		}
	}
}
