//	Copyright © 2011-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Document.PDF
{
	class PdfExportException : System.Exception
	{
		public PdfExportException(string message)
		{
			this.message = message;
		}
		
		public override string					Message
		{
			get
			{
				return this.message;
			}
		}
		
		private readonly string					message;
	}
}
