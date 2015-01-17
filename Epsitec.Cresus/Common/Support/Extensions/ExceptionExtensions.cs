//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
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
			var set    = new HashSet<System.Exception> ();
			var buffer = new System.Text.StringBuilder ();
			var title  = "Exception:";

			for (var e = exception; e != null; e = e.InnerException)
			{
				if (set.Add (e))
				{
					ExceptionExtensions.DumpException (buffer, "", title, e, set);
				}
				title = "Inner exception:";
			}

			return buffer.ToString ();
		}

		private static void DumpException(System.Text.StringBuilder buffer, string prefix, string title, System.Exception exception, HashSet<System.Exception> set)
		{
			var format = "{0}\n"
					   + "  Type: {1}\n"
					   + "  Message: {2}\n"
					   + "  Source: {3}\n"
					   + "  Stack trace:\n"
					   + " {4}";

			var type    = exception.GetType ();
			var message = exception.Message ?? "<none>";
			var source  = exception.Source ?? "<none>";
			var trace   = exception.StackTrace == null ? "   <none>" : exception.StackTrace.Replace ("\n", "\n ");

			var exceptionText = prefix + string.Format (format, title, type, message, source, trace).Replace ("\n", "\n" + prefix);

			buffer.AppendLine (exceptionText);

			var aggregate = exception as System.AggregateException;

			if (aggregate != null)
			{
				foreach (var ex in aggregate.InnerExceptions)
				{
					if (set.Add (ex))
					{
						ExceptionExtensions.DumpException (buffer, prefix + "  ", "Aggregate inner exception:", ex, set);
					}
				}
			}
		}
	}
}
