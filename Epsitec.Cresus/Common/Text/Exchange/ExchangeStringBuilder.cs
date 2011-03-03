using System;
using System.Collections.Generic;
using System.Text;

namespace Epsitec.Common.Text.Exchange
{
	public class ExchangeStringBuilder
	{
		public ExchangeStringBuilder()
		{
			theBuilder = new System.Collections.Generic.List<byte> ();
		}

		public int Length
		{
			get
			{
				return this.theBuilder.Count;
			}
		}

		public byte this[int index]
		{
			get
			{
				return this.theBuilder[index];
			}
		}

		public void Append(string str)
		{
			UTF8Encoding utf8 = new UTF8Encoding ();
			byte[] encodedBytes = utf8.GetBytes (str);

			for (int i = 0; i < encodedBytes.Length; i++)
			{
				this.theBuilder.Add (encodedBytes[i]);
			}
#if DEBUG
			this.debugstring.Append (str);
#endif
		}

		public void AppendLine(string str)
		{
			this.length += str.Length + 2;
			this.Append (str);
			this.theBuilder.Add ((byte) '\r');
			this.theBuilder.Add ((byte) '\n');
#if DEBUG
			this.debugstring.Append ("\r\n");
#endif
		}

		public override string ToString()
		{
			StringBuilder tmpBuilder = new StringBuilder ();

			for (int i = 0; i < theBuilder.Count; i++)
			{
				tmpBuilder.Append ((char) theBuilder[i]);
			}

			return tmpBuilder.ToString ();
		}

		private System.Collections.Generic.List<byte> theBuilder;
		private System.Text.StringBuilder debugstring = new System.Text.StringBuilder ();
		private int length;
	}

}
