using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace Epsitec
{
	public static partial class Extensions
	{
		public static bool AtStartOfLine(this InlineCollection source)
		{
			var lastInline = source.LastInline;
			return lastInline == null || lastInline is LineBreak;
		}

		public static string GetLeftMargin(this InlineCollection source, int indent, string defaultMargin = null)
		{
			return source.AtStartOfLine ()
				? new string ('\t', indent)
				: (defaultMargin ?? string.Empty);
		}

		public static void GotoStartOfLine(this InlineCollection source)
		{
			if (!source.AtStartOfLine ())
			{
				source.Add (new LineBreak ());
			}
		}
	}
}
