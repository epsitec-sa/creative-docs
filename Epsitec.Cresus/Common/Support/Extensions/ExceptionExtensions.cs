using System;
using System.Text;


namespace Epsitec.Common.Support.Extensions
{
	
	
	public static class ExceptionExtensions
	{


		public static string GetFullText(this Exception exception)
		{
			var text = new StringBuilder ();

			for (var e = exception; e != null; e = e.InnerException)
			{
				var headerText = e == exception
					? "Exception:"
					: "Inner exception:";

				var format = "\tType: {0}\n\tMessage: {1}\n\tSource: {2}\n\tStack trace: {3}\n";

				var type = e.GetType ();
				var message = e.Message;
				var source = e.Source;
				var stackTrace = e.StackTrace;

				var exceptionText = string.Format (format, type, message, source, stackTrace);

				text.AppendLine (headerText);
				text.AppendLine (exceptionText);
			}

			return text.ToString ();
		}


	}


}
