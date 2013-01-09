//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Pdf
{
	class PdfExportException : System.Exception
	{
		public PdfExportException(string message)
		{
			this.message = message;
		}
		public string Message
		{
			get
			{
				return this.message;
			}
		}
		private readonly string message;
	}
}
