
namespace Epsitec.Common.Document.PDF
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
