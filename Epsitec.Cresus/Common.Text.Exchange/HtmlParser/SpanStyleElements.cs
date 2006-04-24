using System;
using System.Collections.Generic;
using System.Text;

namespace Epsitec.Common.Text.Exchange.HtmlParser
{
	class SpanStyleElements
	{
		System.Collections.Generic.Dictionary<string, string> dict = new System.Collections.Generic.Dictionary<string, string> ();

		public SpanStyleElements(string spanstylestring)
		{
			char[] semicolonseparators = ";".ToCharArray ();
			char[] colonseparators = ":".ToCharArray ();

			string[] sp = spanstylestring.Split (semicolonseparators);

			foreach (string element in sp)
			{
				string[] pair = element.Split (colonseparators);
				dict[pair[0]] = pair[1];
			}
		}

		public IEnumerator<string> GetEnumerator()
		{
			foreach (KeyValuePair<string, string> kv in this.dict)
			{
				yield return kv.Key;
			}
		}

		public string this[string index]
		{
			get
			{
				string output = null;
				this.dict.TryGetValue (index, out output);
				return output;
			}
		}
	}
}
