//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Document.PDF
{
	public class StringBuffer
	{
		public StringBuffer()
		{
			this.buffer = new System.Text.StringBuilder ();
		}

		~StringBuffer()
		{
			if ((this.path != null) &&
				(System.IO.File.Exists (this.path)))
			{
				if (this.stream != null)
				{
					this.stream.Close ();
				}

				System.IO.File.Delete (this.path);
			}
		}
		
		public bool InMemory
		{
			get
			{
				return this.path == null;
			}
		}

		public int Length
		{
			get
			{
				return this.length;
			}
		}

		public bool EndsWithWhitespace
		{
			get
			{
				return this.endsWithWhitespace;
			}
		}

		public void Append(string text)
		{
			if (string.IsNullOrEmpty (text))
			{
				return;
			}

			this.length += text.Length;

			if (text.Length > StringBuffer.Threshold)
			{
				this.FlushBuffer ();
				this.Emit (text);

				this.endsWithWhitespace = char.IsWhiteSpace (text[text.Length-1]);
			}
			else
			{
				this.buffer.Append (text);

				this.endsWithWhitespace = char.IsWhiteSpace (text[text.Length-1]);

				if (this.length > StringBuffer.Threshold)
				{
					this.FlushBuffer ();
				}
			}
		}

		public void AppendNewLine()
		{
			this.Append ("\r\n");
		}

		public override string ToString()
		{
			if (this.path != null)
			{
				this.FlushBuffer ();

				int count = this.length;
				byte[] buffer = new byte[count];

				this.stream.Seek (0, System.IO.SeekOrigin.Begin);
				this.stream.Read (buffer, 0, count);

				return System.Text.Encoding.Default.GetString (buffer);
			}
			else
			{
				return this.buffer.ToString ();
			}
		}

		public System.IO.Stream GetStream()
		{
			this.FlushBuffer ();
			this.stream.Seek (0, System.IO.SeekOrigin.Begin);

			return this.stream;
		}

		private void FlushBuffer()
		{
			if (this.path == null)
			{
				this.path = System.IO.Path.GetTempFileName ();
				this.stream = System.IO.File.Open (this.path, System.IO.FileMode.Open);
			}

			if (this.buffer.Length > 0)
			{
				this.stream.Seek (0, System.IO.SeekOrigin.End);
				this.Emit (this.buffer.ToString ());
			}
			
			this.stream.Flush ();
		}

		private void Emit(string text)
		{
			byte[] data = System.Text.Encoding.Default.GetBytes (text);
			this.stream.Write (data, 0, data.Length);
			this.buffer.Length = 0;
		}
		public void CloseStream(System.IO.Stream stream)
		{
		}

		private const int Threshold = 1024*4;

		private System.Text.StringBuilder buffer;
		private System.IO.Stream stream;
		private int length;
		private string path;
		private bool endsWithWhitespace;
	}
}
