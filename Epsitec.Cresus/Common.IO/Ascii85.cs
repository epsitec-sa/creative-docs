//	Jeff Atwood, http://www.codinghorror.com/blog/archives/000410.html
//	Based on C code from http://www.stillhq.com/cgi-bin/cvsweb/ascii85/
//	Wikipedia article at http://en.wikipedia.org/wiki/Ascii85

using System;
using System.Text;
using System.IO;

namespace Epsitec.Common.IO
{
	/// <summary>
	/// C# implementation of ASCII85 encoding. 
	/// </summary>
	public class Ascii85
	{
		/// <summary>
		/// Prefix mark that identifies an encoded ASCII85 string, traditionally '<~'
		/// </summary>
		public string PrefixMark = "<~";
		
		/// <summary>
		/// Suffix mark that identifies an encoded ASCII85 string, traditionally '~>'
		/// </summary>
		public string SuffixMark = "~>";
		
		/// <summary>
		/// Maximum line length for encoded ASCII85 string; 
		/// set to zero for one unbroken line.
		/// </summary>
		public int LineLength = 75;
		
		/// <summary>
		/// Add the Prefix and Suffix marks when encoding, and enforce their presence for decoding
		/// </summary>
		public bool EnforceMarks = true;

		/// <summary>
		/// Decodes an ASCII85 encoded string into the original binary data
		/// </summary>
		/// <param name="s">ASCII85 encoded string</param>
		/// <returns>byte array of decoded binary data</returns>
		public byte[] Decode(string s)
		{
			if (EnforceMarks)
			{
				if (!s.StartsWith (PrefixMark) | !s.EndsWith (SuffixMark))
				{
					throw new System.FormatException ("ASCII85 encoded data should begin with '" + PrefixMark + "' and end with '" + SuffixMark + "'");
				}
			}

			// strip prefix and suffix if present
			if (s.StartsWith (PrefixMark))
			{
				s = s.Substring (PrefixMark.Length);
			}
			if (s.EndsWith (SuffixMark))
			{
				s = s.Substring (0, s.Length - SuffixMark.Length);
			}

			MemoryStream ms = new MemoryStream ();
			int count = 0;
			bool processChar = false;

			foreach (char c in s)
			{
				switch (c)
				{
					case 'z':
						if (count != 0)
						{
							throw new System.FormatException ("The character 'z' is invalid inside an ASCII85 block.");
						}
						this.decodedBlock[0] = 0;
						this.decodedBlock[1] = 0;
						this.decodedBlock[2] = 0;
						this.decodedBlock[3] = 0;
						ms.Write (this.decodedBlock, 0, Ascii85.decodedBlockLength);
						processChar = false;
						break;
					case ' ':
					case '\n':
					case '\r':
					case '\t':
					case '\0':
					case '\f':
					case '\b':
						processChar = false;
						break;
					default:
						if (c < '!' || c > 'u')
						{
							throw new System.FormatException ("Bad character '" + c + "' found. ASCII85 only allows characters '!' to 'u'.");
						}
						processChar = true;
						break;
				}

				if (processChar)
				{
					this.tuple += ((uint) (c - Ascii85.asciiOffset) * pow85[count]);
					count++;
					if (count == Ascii85.encodedBlockLength)
					{
						DecodeBlock ();
						ms.Write (this.decodedBlock, 0, Ascii85.decodedBlockLength);
						this.tuple = 0;
						count = 0;
					}
				}
			}

			// if we have some bytes left over at the end..
			if (count != 0)
			{
				if (count == 1)
				{
					throw new System.FormatException ("The last block of ASCII85 data cannot be a single byte.");
				}
				count--;
				this.tuple += pow85[count];
				DecodeBlock (count);
				for (int i = 0; i < count; i++)
				{
					ms.WriteByte (this.decodedBlock[i]);
				}
			}

			return ms.ToArray ();
		}

		/// <summary>
		/// Encodes binary data into a plaintext ASCII85 format string
		/// </summary>
		/// <param name="ba">binary data to encode</param>
		/// <returns>ASCII85 encoded string</returns>
		public string Encode(byte[] ba)
		{
			StringBuilder sb = new StringBuilder ((int) (ba.Length * (Ascii85.encodedBlockLength/Ascii85.decodedBlockLength)));
			this.linePos = 0;

			if (EnforceMarks)
			{
				AppendString (sb, PrefixMark);
			}

			int count = 0;
			this.tuple = 0;
			foreach (byte b in ba)
			{
				if (count >= Ascii85.decodedBlockLength - 1)
				{
					this.tuple |= b;
					if (this.tuple == 0)
					{
						AppendChar (sb, 'z');
					}
					else
					{
						EncodeBlock (sb);
					}
					this.tuple = 0;
					count = 0;
				}
				else
				{
					this.tuple |= (uint) (b << (24 - (count * 8)));
					count++;
				}
			}

			// if we have some bytes left over at the end..
			if (count > 0)
			{
				EncodeBlock (count + 1, sb);
			}

			if (EnforceMarks)
			{
				AppendString (sb, SuffixMark);
			}
			return sb.ToString ();
		}

		#region Private Methods

		private void EncodeBlock(StringBuilder sb)
		{
			EncodeBlock (Ascii85.encodedBlockLength, sb);
		}

		private void EncodeBlock(int count, StringBuilder sb)
		{
			for (int i = Ascii85.encodedBlockLength - 1; i >= 0; i--)
			{
				this.encodedBlock[i] = (byte) ((this.tuple % 85) + Ascii85.asciiOffset);
				this.tuple /= 85;
			}

			for (int i = 0; i < count; i++)
			{
				char c = (char) this.encodedBlock[i];
				AppendChar (sb, c);
			}

		}

		private void DecodeBlock()
		{
			DecodeBlock (Ascii85.decodedBlockLength);
		}

		private void DecodeBlock(int bytes)
		{
			for (int i = 0; i < bytes; i++)
			{
				this.decodedBlock[i] = (byte) (this.tuple >> 24 - (i * 8));
			}
		}

		private void AppendString(StringBuilder sb, string s)
		{
			if (LineLength > 0 && (this.linePos + s.Length > LineLength))
			{
				this.linePos = 0;
				sb.Append ('\n');
			}
			else
			{
				this.linePos += s.Length;
			}
			sb.Append (s);
		}

		private void AppendChar(StringBuilder sb, char c)
		{
			sb.Append (c);
			this.linePos++;
			if (LineLength > 0 && (this.linePos >= LineLength))
			{
				this.linePos = 0;
				sb.Append ('\n');
			}
		}

		#endregion

		private const int asciiOffset = 33;
		private const int encodedBlockLength = 5;
		private const int decodedBlockLength = 4;
		private byte[] encodedBlock = new byte[Ascii85.encodedBlockLength];
		private byte[] decodedBlock = new byte[Ascii85.decodedBlockLength];
		private uint tuple = 0;
		private int linePos = 0;

		private readonly uint[] pow85 = { 85*85*85*85, 85*85*85, 85*85, 85, 1 };
	}
}