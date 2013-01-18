//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Xml.Linq;

namespace Epsitec.Common.Pdf.Engine
{
	public class PdfImageStream : System.IDisposable
	{
		public PdfImageStream(string code, StringBuffer data)
		{
			this.code = code;
			this.path = System.IO.Path.GetTempFileName ();

			System.IO.File.WriteAllText (this.path, data.ToString (), System.Text.Encoding.Default);

			this.stream = System.IO.File.OpenRead (this.path);
			this.length = (int) this.stream.Length;
		}
		
		public string Code
		{
			get
			{
				return this.code;
			}
		}
		
		public System.IO.Stream Stream
		{
			get
			{
				return this.stream;
			}
		}

		

		public int StreamLength
		{
			get
			{
				return this.length;
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			if (this.stream != null)
			{
				this.stream.Dispose ();
			}

			if (this.path != null)
			{
				System.IO.File.Delete (this.path);
			}
		}

		#endregion
		
		
		private readonly string code;
		private readonly System.IO.Stream stream;
		private readonly int length;
		private readonly string path;
	}
}
