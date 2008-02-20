//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Michael WALZ

using System;
using System.Collections.Generic;
using System.Text;

namespace Epsitec.Common.Text.Exchange.HtmlParser
{
	class SpanStyleElements
	{
		/// <summary>
		/// La classe SpanStyleElements s'occupe de décoder une chaine du type 
		/// "FONT-SIZE: 9pt; FONT-FAMILY: 'Times Roman' ..."
		/// </summary>

		System.Collections.Generic.Dictionary<string, string> dict = new System.Collections.Generic.Dictionary<string, string> ();

		public SpanStyleElements(string spanstylestring)
		{
			string[] sp = spanstylestring.Split (semicolonseparators);

			foreach (string element in sp)
			{
				string[] pair = element.Split (SpanStyleElements.colonseparators);
				pair[0] = pair[0].Trim ().ToLower();
				pair[1] = pair[1].Trim (SpanStyleElements.quotestotrim);
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

		private static char[] semicolonseparators = ";".ToCharArray ();
		private static char[] colonseparators = ":".ToCharArray ();
		private static char[] quotestotrim = " '\"\r\n".ToCharArray ();

	}
}
