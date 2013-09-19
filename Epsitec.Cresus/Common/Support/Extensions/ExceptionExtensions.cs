//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Support.Extensions
{
	public static class ExceptionExtensions
	{
		/// <summary>
		/// Gets the full text for the exception (and possibly for the inner exceptions too).
		/// </summary>
		/// <param name="exception">The exception.</param>
		/// <returns>The full text.</returns>
		public static string GetFullText(this System.Exception exception)
		{
			var buffer = new System.Text.StringBuilder ();
			var title  = "Exception:";

			for (var e = exception; e != null; e = e.InnerException)
			{
				ExceptionExtensions.DumpException (buffer, "", title, e);
				title = "Inner exception:";
			}

			return buffer.ToString ();
		}

		private static void DumpException(System.Text.StringBuilder buffer, string prefix, string title, System.Exception exception)
		{
			var format = "{0}\n"
					   + "  Type: {0}\n"
					   + "  Message: {1}\n"
					   + "  Source: {2}\n"
					   + "  Stack trace:\n"
					   + "    {3}";

			var type    = exception.GetType ();
			var message = exception.Message;
			var source  = exception.Source;
			var trace   = exception.StackTrace == null ? "<none>" : exception.StackTrace.Replace ("\n", "\n    ");

			var exceptionText = prefix + string.Format (format, title, type, message, source, trace).Replace ("\n", "\n" + prefix);

			buffer.AppendLine (exceptionText);

			var aggregate = exception as System.AggregateException;

			if (aggregate != null)
			{
				foreach (var ex in aggregate.InnerExceptions)
				{
					ExceptionExtensions.DumpException (buffer, prefix + "  ", "Aggregate inner exception:", ex);
				}
			}
		}
	}
}
