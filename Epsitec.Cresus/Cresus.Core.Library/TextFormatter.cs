﻿//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Library.Internal;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core
{
	public static class TextFormatter
	{
		public static string							CurrentLanguageId
		{
			get
			{
				Library.UI.CultureSettings culture = Library.UI.Services.Settings.CultureForData;
				return culture.LanguageId;
			}
		}

		public static System.Globalization.CultureInfo	CurrentCulture
		{
			get
			{
				return TextFormatter.cultureOverride ?? Library.UI.Services.Settings.CultureForData.CultureInfo;
			}
		}

		public static TextFormatterDetailLevel			CurrentDetailLevel
		{
			get
			{
				return TextFormatter.detailLevel;
			}
		}


		public static FormattedText						DefaultUnknownText
		{
			get
			{
				switch (TextFormatter.CurrentLanguageId)
				{
					case "fr":
						return TextFormatter.FormatText ("<i>inconnu</i>");
					
					case "de":
						return TextFormatter.FormatText ("<i>unbekannt</i>");
					
					default:
					case "en":
						return TextFormatter.FormatText ("<i>unknown</i>");
				}
			}
		}

		public static FormattedText FormatText(params object[] values)
		{
			var buffer = new System.Text.StringBuilder ();

			List<object> items = new List<object> (values);
			List<object> flat  = new List<object> ();

			TextFormatter.Preprocess (items);
			TextFormatter.Flatten (flat, items);
			
			List<string> texts = TextFormatter.ConvertItemsToStrings (flat);

			TextFormatter.ProcessTags (texts);
			TextFormatter.FormatText (buffer, texts);

			return new FormattedText (string.Join (FormattedText.HtmlBreak, buffer.ToString ().Split (new string[] { FormattedText.HtmlBreak }, System.StringSplitOptions.RemoveEmptyEntries)).Replace ("()", ""));
		}

		public static string ConvertToText(object value)
		{
			if (value == null)
			{
				return "";
			}

			var converter = TextFormatter.GetDefaultConverter (value.GetType ());

			if (converter != null)
			{
				return converter (value);
			}

			if (value.GetType ().IsEnum)
			{
				return TextFormatter.ConvertToText (EnumKeyValues.GetEnumKeyValue (value));
			}

			FormattedText formattedText = TextFormatterConverter.ToFormattedText (value, TextFormatter.CurrentCulture, TextFormatter.CurrentDetailLevel);

			//	Multilingual texts must be "flattened" : only one language may survive the conversion
			//	to text, or else TextLayout would crash, not recognizing <div> tags:

			if (MultilingualText.IsMultilingual (formattedText))
			{
				MultilingualText multilingualText = new MultilingualText (formattedText);
				return multilingualText.GetTextOrDefault (TextFormatter.CurrentLanguageId).ToString ();
			}
			else
			{
				return formattedText.ToString ();
			}
		}

		public static void DefineDefaultConverter<T>(System.Func<T, string> converter)
		{
			TextFormatter.DefineDefaultConverter (typeof (T), (object x) => converter ((T) x));
		}

		public static void DefineDefaultConverter(System.Type type, System.Func<object, string> converter)
		{
			lock (TextFormatter.exclusion)
			{
				TextFormatter.defaultConverters[type] = converter;
			}
		}

		private static System.Func<object, string> GetDefaultConverter(System.Type type)
		{
			lock (TextFormatter.exclusion)
			{
				System.Func<object, string> converter;
				TextFormatter.defaultConverters.TryGetValue (type, out converter);
				return converter;
			}
		}

		public static FormattedText Join(string separator, IEnumerable<FormattedText> collection)
		{
			return new FormattedText (string.Join (separator, collection.Select (x => TextFormatter.ConvertToText (x)).ToArray ()));
		}

		public static System.IDisposable UsingCulture(System.Globalization.CultureInfo culture, TextFormatterDetailLevel detailLevel)
		{
			TextFormatter.SetCultureOverride (culture, detailLevel);
			return new UsingCultureHelper ();
		}

		
		private static void SetCultureOverride(System.Globalization.CultureInfo culture, TextFormatterDetailLevel detailLevel)
		{
			TextFormatter.cultureOverride = culture;
			TextFormatter.detailLevel = detailLevel;
		}

		private static void ClearCultureOverride()
		{
			TextFormatter.cultureOverride = null;
			TextFormatter.detailLevel = TextFormatterDetailLevel.Default;
		}

		private static void Preprocess(List<object> values)
		{
			int n = values.Count;
			Library.Formatters.FormatterHelper formatter = null;

			for (int i = 1; i < n; i++)
			{
				var text = values[i] as string;

				if ((text.StartsWith (Prefix.CommandEscape)) &&
					(text.StartsWith (Command.Format)))
				{
					var format = text.SplitAtFirst (":");

					if (format.Length > 0)
					{
						if (formatter == null)
						{
							formatter = new Library.Formatters.FormatterHelper ();
						}

						values[i-1] = formatter.Format (format, values[i-1]);
						values[i-0] = Command.Ignore;
					}
				}
			}

			TextFormatter.RemoveIgnoreTags (values);
		}
		
		private static void Flatten(List<object> flat, System.Collections.IEnumerable values)
		{
			foreach (var value in values)
			{
				var enumerable = value as System.Collections.IEnumerable;
				var convertible = value as ITextFormatter;

				if ((convertible == null) &&
					(enumerable != null) &&
					((value is string) == false) &&
					((value is FormattedText) == false))
				{
					TextFormatter.Flatten (flat, enumerable);
				}
				else
				{
					flat.Add (value);
				}
			}
		}

		private static List<string> ConvertItemsToStrings(IEnumerable<object> items)
		{
			var texts  = new List<string> ();

			foreach (var value in items.Select (item => TextFormatter.ConvertToText (item)))
			{
				texts.Add (value.Replace ("\n", FormattedText.HtmlBreak).Trim ());
			}

			return texts;
		}

		private static void FormatText(System.Text.StringBuilder buffer, List<string> items)
		{
			bool emptyItem = true;
			int  count     = items.Count;

			for (int i = 0; i < count; i++)
			{
				bool isLast = (i == count-1);
				string text = items[i];
				string next = isLast ? "" : items[i+1];

				if (text.Length == 0)
				{
					emptyItem = true;
					continue;
				}

				char prefix = text.RemoveTag ().FirstCharacter ();

				if (prefix == Prefix.SkipItemIfPreviousEmpty)
				{
					if (emptyItem)
					{
						continue;
					}

					text = text.Substring (1);
					prefix = text.RemoveTag ().FirstCharacter ();
				}

				char suffix = text.RemoveTag ().LastCharacter ();

				if (suffix == Suffix.SkipItemIfNextEmpty)
				{
					if (next.Length == 0)
					{
						continue;
					}

					text = text.Substring (0, text.Length-1);
					suffix = text.RemoveTag ().LastCharacter ();
				}

				char lastCharacter = buffer.LastCharacter ();

				if ((prefix.IsPunctuationMark ()) &&
					((lastCharacter == prefix) || emptyItem))
				{
					//	Duplicate punctuation mark... or punctuation mark on a still empty line.
				}
				else
				{
					if ((emptyItem == false) &&
						(lastCharacter != '(') &&
						(prefix != '\n') &&
						(prefix.IsPunctuationMark () == false) &&
						(prefix != ')'))
					{
						buffer.Append (" ");
					}

					buffer.Append (text);
				}

				emptyItem = text.EndsWith (FormattedText.HtmlBreak) || string.IsNullOrEmpty (text.RemoveTag ());
			}
		}


		#region UsingCultureHelper Class

		private class UsingCultureHelper : System.IDisposable
		{
			#region IDisposable Members

			void System.IDisposable.Dispose()
			{
				TextFormatter.ClearCultureOverride ();
			}

			#endregion
		}

		#endregion

		public static class Prefix
		{
			public const char SkipItemIfPreviousEmpty = '~';
			public const char CommandEscape = '‼';
		}

		public static class Suffix
		{
			public const char SkipItemIfNextEmpty = '~';
		}

		public static class Command
		{
			public const string IfEmpty				= "‼ifEmpty";
			public const string IfElseEmpty			= "‼ifElseEmpty";
			public const string Ignore				= "‼ignore";
			public const string Mark				= "‼mark";
			public const string ClearToMarkIfEmpty	= "‼clearToMarkIfEmpty";
			public const string Format				= "‼format";
		}
		
		public static string FormatCommand(string format)
		{
			return string.Concat (Command.Format, ":", format);
		}

		private static void ProcessTags(List<string> items)
		{
			TextFormatter.RemoveIgnoreTags (items);

			int count = items.Count;
			
			for (int i = 0; i < count; i++)
			{
				string command = items[i];

				if ((command.Length > 0) &&
					(command[0] == Prefix.CommandEscape))
				{
					if ((command == Command.ClearToMarkIfEmpty) &&
						(i > 0) &&
						(string.IsNullOrWhiteSpace (items[i-1])))
					{
						TextFormatter.ClearGroup (items, i);
						TextFormatter.ProcessTags (items);
						break;
					}

					if ((command == Command.IfEmpty) &&
						(i > 1))
					{
						if (string.IsNullOrWhiteSpace (items[i-2]))
						{
							//	"", "x", ifEmpty => ignore, "x", ignore
							items[i-2] = Command.Ignore;
							items[i-0] = Command.Ignore;
						}
						else
						{
							//	"a", "x", ifEmpty => "a", ignore, ignore
							items[i-1] = Command.Ignore;
							items[i-0] = Command.Ignore;
						}

						TextFormatter.ProcessTags (items);
						break;
					}
					
					if ((command == Command.IfElseEmpty) &&
						(i > 2))
					{
						if (string.IsNullOrWhiteSpace (items[i-3]))
						{
							//	"", "x", "y", ifElseEmpty => ignore, "x", ignore, ignore
							items[i-3] = Command.Ignore;
							items[i-1] = Command.Ignore;
							items[i-0] = Command.Ignore;
						}
						else
						{
							//	"a", "x", "y", ifElseEmpty => "a", ignore, "y", ignore
							items[i-2] = Command.Ignore;
							items[i-0] = Command.Ignore;
						}

						TextFormatter.ProcessTags (items);
						break;
					}
				}
			}

			TextFormatter.RemoveAllTags (items);
		}

		private static void RemoveAllTags(List<string> items)
		{
			items.RemoveAll (x => x.StartsWith (Prefix.CommandEscape));
		}

		private static void RemoveIgnoreTags(List<object> items)
		{
			items.RemoveAll (x => Command.Ignore.Equals (x));
		}

		private static void RemoveIgnoreTags(List<string> items)
		{
			items.RemoveAll (x => Command.Ignore == x);
		}


		private static void ClearGroup(List<string> items, int index)
		{
			int startIndex = 0;

			for (int i = index-1; i > 0; i--)
			{
				if (items[i] == Command.Mark)
				{
					startIndex = i+1;
					break;
				}
			}

			int count = items.Count;
			int endIndex = count-1;

			for (int i = index+1; i < count; i++)
			{
				if (items[i] == Command.Mark)
				{
					endIndex = i-1;
					break;
				}
			}

			int num = endIndex - startIndex + 1;

			items.RemoveRange (startIndex, num);
		}

		static TextFormatter()
		{
			TextFormatter.DefineDefaultConverter<string> (x => x);
			TextFormatter.DefineDefaultConverter<Date> (x => x.ToString ("d", System.Globalization.DateTimeFormatInfo.CurrentInfo));
			TextFormatter.DefineDefaultConverter<Time> (x => x.ToString ("t", System.Globalization.DateTimeFormatInfo.CurrentInfo));
			TextFormatter.DefineDefaultConverter<System.DateTime> (x => x.ToString ("g", System.Globalization.DateTimeFormatInfo.CurrentInfo));
			TextFormatter.DefineDefaultConverter<Caption> (x => x.DefaultLabelOrName);
			TextFormatter.DefineDefaultConverter<StructuredType> (x => x.Caption.DefaultLabelOrName);
			TextFormatter.DefineDefaultConverter<Druid> (x => x.IsEmpty ? TextFormatter.DefaultUnknownText.ToString () : EntityInfo.GetStructuredType (x).Caption.DefaultLabelOrName);
		}


		[System.ThreadStatic]
		private static System.Globalization.CultureInfo cultureOverride;

		[System.ThreadStatic]
		private static TextFormatterDetailLevel detailLevel;

		private static readonly object exclusion = new object ();
		private static readonly Dictionary<System.Type, System.Func<object, string>> defaultConverters = new Dictionary<System.Type, System.Func<object, string>> ();
	}
}