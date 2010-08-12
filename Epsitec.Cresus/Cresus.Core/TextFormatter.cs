//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.DataLayer;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core
{
	public static class TextFormatter
	{
		public static FormattedText FormatText(params object[] values)
		{
			var buffer = new System.Text.StringBuilder ();
			var items  = new List<string> ();

			bool emptyItem = true;

			foreach (var value in values.Select (item => TextFormatter.ConvertToText (item)))
			{
				items.Add (value.Replace ("\n", "<br/>").Trim ());
			}

			int count = items.Count;

			items.Add ("");

			for (int i = 0; i < count; i++)
			{
				string text = items[i];
				string next = items[i+1];

				if (text.Length == 0)
				{
					emptyItem = true;
					continue;
				}

				if (text.FirstCharacter () == '~')
				{
					if (emptyItem)
					{
						continue;
					}

					text = text.Substring (1);
				}
				if (text.LastCharacter () == '~')
				{
					if (next.Length == 0)
					{
						continue;
					}

					text = text.Substring (0, text.Length-1);
				}

				if (!emptyItem && buffer.LastCharacter () != '(' && !Misc.IsPunctuationMark (text.FirstCharacter ()))
				{
					buffer.Append (" ");
				}

				buffer.Append (text);

				emptyItem = text.EndsWith ("<br/>");
			}

			return new FormattedText (string.Join ("<br/>", buffer.ToString ().Split (new string[] { "<br/>" }, System.StringSplitOptions.RemoveEmptyEntries)).Replace ("()", ""));
		}
		
		private static string ConvertToText(object value)
		{
			if (value == null)
			{
				return "";
			}

			string text = value as string;

			if (text != null)
			{
				return text;
			}

			if (value is Date)
			{
				return ((Date) value).ToDateTime ().ToShortDateString ();
			}

			return value.ToString ();
		}
	}

	
	static class StringExtension
	{
		public static char LastCharacter(this string text)
		{
			int n = text.Length - 1;
			return n < 0 ? (char) 0 : text[n];
		}
		
		public static char FirstCharacter(this string text)
		{
			int n = text.Length;
			return n < 1 ? (char) 0 : text[0];
		}
	}
	
	static class StringBuilderExtension
	{
		public static char LastCharacter(this System.Text.StringBuilder text)
		{
			int n = text.Length - 1;
			return n < 0 ? (char) 0 : text[n];
		}
	}
}
