//	Copyright © 2006-2011, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

//	This file is based on work from Jeff Atwood :
//	Jeff Atwood, http://www.codinghorror.com/blog/archives/000410.html
//	Based on C code from http://www.stillhq.com/cgi-bin/cvsweb/ascii85/
//	Wikipedia article at http://en.wikipedia.org/wiki/Ascii85

namespace Epsitec.Common.IO
{
	/// <summary>
	/// C# implementation of ASCII85 encoding. 
	/// </summary>
	public static class Ascii85
	{
		/// <summary>
		/// Encodes binary data into a plaintext ASCII85 format string.
		/// </summary>
		/// <param name="data">Binary data to encode.</param>
		/// <param name="outputMarks">if set to <c>true</c> output marks.</param>
		/// <returns>ASCII85 encoded string.</returns>
		public static string Encode(byte[] data, bool outputMarks = true)
		{
			Engine engine = new Engine ()
			{
				EnforceMarks = outputMarks
			};

			return engine.Encode (data);
		}

		/// <summary>
		/// Decodes an ASCII85 encoded string into the original binary data.
		/// </summary>
		/// <param name="value">ASCII85 encoded string.</param>
		/// <param name="enforceMarks">if set to <c>true</c> enforce marks.</param>
		/// <returns>Byte array of decoded binary data.</returns>
		public static byte[] Decode(string value, bool enforceMarks = true)
		{
			Engine engine = new Engine ()
			{
				EnforceMarks = enforceMarks
			};
			
			return engine.Decode (value);
		}


		public static string MapEncodedStringToXmlTransparent(string data)
		{
			if (data == null)
			{
				return null;
			}
			
			System.Text.StringBuilder buffer = new System.Text.StringBuilder (data.Length);

			foreach (char c in data)
			{
				char output;
				
				switch (c)
				{
					case '\'':	output = 'v';	break;
					case '\"':	output = 'w';	break;
					case '<':	output = '{';	break;
					case '>':	output = '}';	break;
					case '&':	output = '|';	break;
					
					case 'v':
					case 'w':
					case 'x':
					case 'y':
					case '{':
					case '|':
					case '}':
					case '~':
						throw new System.FormatException ("Invalid ASCII 85 character found");

					default:
						output = c;
						break;
				}

				buffer.Append (output);
			}
			
			return buffer.ToString ();
		}

		public static string MapXmlTransparentToEncodedString(string data)
		{
			if (data == null)
			{
				return null;
			}

			System.Text.StringBuilder buffer = new System.Text.StringBuilder (data.Length);

			foreach (char c in data)
			{
				char output;
				
				switch (c)
				{
					case 'v':	output = '\'';	break;
					case 'w':	output = '\"';	break;
					case '{':	output = '<';	break;
					case '}':	output = '>';	break;
					case '|':	output = '&';	break;

					case 'x':
					case 'y':
					case '\'':
					case '\"':
					case '<':
					case '>':
					case '&':
					case '~':
						throw new System.FormatException ("Invalid XML-transparent ASCII 85 character found");

					default:
						output = c;
						break;
				}

				buffer.Append (output);
			}
			
			return buffer.ToString ();
		}

		public static string EncodeGuid(System.Guid guid)
		{
			return Ascii85.Encode (guid.ToByteArray (), outputMarks: false);
		}

		public static System.Guid DecodeGuid(string value)
		{
			return new System.Guid (Ascii85.Decode (value, enforceMarks: false));
		}

		public static System.Guid? DecodeGuidOrNull(string value)
		{
			if (string.IsNullOrEmpty (value))
			{
				return null;
			}

			byte[] data =Ascii85.Decode (value, enforceMarks: false);

			if (data.Length != 16)
			{
				return null;
			}

			return new System.Guid (data);
		}

		#region Engine Class

		public sealed class Engine
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
				if (this.EnforceMarks)
				{
					if (!s.StartsWith (this.PrefixMark) | !s.EndsWith (this.SuffixMark))
					{
						throw new System.FormatException ("ASCII85 encoded data should begin with '" + this.PrefixMark + "' and end with '" + this.SuffixMark + "'");
					}
				}

				// strip prefix and suffix if present
				if (s.StartsWith (this.PrefixMark))
				{
					s = s.Substring (this.PrefixMark.Length);
				}
				if (s.EndsWith (this.SuffixMark))
				{
					s = s.Substring (0, s.Length - this.SuffixMark.Length);
				}

				System.IO.MemoryStream ms = new System.IO.MemoryStream ();
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
							this.DecodeBlock ();
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
					this.DecodeBlock (count);
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
				System.Text.StringBuilder sb = new System.Text.StringBuilder ((int) (ba.Length * (Ascii85.encodedBlockLength/Ascii85.decodedBlockLength)));
				this.Encode (ba, sb);
				return sb.ToString ();
			}

			/// <summary>
			/// Encodes binary data into a plaintext ASCII85 format string
			/// </summary>
			/// <param name="ba">binary data to encode</param>
			/// <param name="sb">StringBuilder to use</param>
			public void Encode(byte[] ba, System.Text.StringBuilder sb)
			{
				this.linePos = 0;

				if (this.EnforceMarks)
				{
					this.AppendString (sb, this.PrefixMark);
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
							this.AppendChar (sb, 'z');
						}
						else
						{
							this.EncodeBlock (sb);
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
					this.EncodeBlock (count + 1, sb);
				}

				if (this.EnforceMarks)
				{
					this.AppendString (sb, this.SuffixMark);
				}
			}

			#region Private Methods

			private void EncodeBlock(System.Text.StringBuilder sb)
			{
				this.EncodeBlock (Ascii85.encodedBlockLength, sb);
			}

			private void EncodeBlock(int count, System.Text.StringBuilder sb)
			{
				for (int i = Ascii85.encodedBlockLength - 1; i >= 0; i--)
				{
					this.encodedBlock[i] = (byte) ((this.tuple % 85) + Ascii85.asciiOffset);
					this.tuple /= 85;
				}

				for (int i = 0; i < count; i++)
				{
					char c = (char) this.encodedBlock[i];
					this.AppendChar (sb, c);
				}

			}

			private void DecodeBlock()
			{
				this.DecodeBlock (Ascii85.decodedBlockLength);
			}

			private void DecodeBlock(int bytes)
			{
				for (int i = 0; i < bytes; i++)
				{
					this.decodedBlock[i] = (byte) (this.tuple >> 24 - (i * 8));
				}
			}

			private void AppendString(System.Text.StringBuilder sb, string s)
			{
				if (this.LineLength > 0 && (this.linePos + s.Length > this.LineLength))
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

			private void AppendChar(System.Text.StringBuilder sb, char c)
			{
				sb.Append (c);
				this.linePos++;
				if (this.LineLength > 0 && (this.linePos >= this.LineLength))
				{
					this.linePos = 0;
					sb.Append ('\n');
				}
			}

			#endregion

			private byte[] encodedBlock = new byte[Ascii85.encodedBlockLength];
			private byte[] decodedBlock = new byte[Ascii85.decodedBlockLength];
			private uint tuple = 0;
			private int linePos = 0;

			private readonly uint[] pow85 = { 85*85*85*85, 85*85*85, 85*85, 85, 1 };
		}

		#endregion

		private const int asciiOffset = 33;
		private const int encodedBlockLength = 5;
		private const int decodedBlockLength = 4;
	}
}