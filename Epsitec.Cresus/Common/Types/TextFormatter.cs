//	Copyright © 2010-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Text;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types.Formatters;
using Epsitec.Common.Types.Converters;

using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Linq.Expressions;

namespace Epsitec.Common.Types
{
	public static class TextFormatter
	{
		public static string					CurrentTwoLetterISOLanguageName
		{
			get
			{
				if (TextFormatter.cultureOverride != null)
				{
					return TextFormatter.cultureOverride.TwoLetterISOLanguageName;
				}

				return TextFormatter.CurrentCulture.TwoLetterISOLanguageName;
			}
		}

		public static CultureInfo				CurrentCulture
		{
			get
			{
				return TextFormatter.cultureOverride
					?? TextFormatter.activeCulture
					?? System.Threading.Thread.CurrentThread.CurrentCulture;
			}
		}

		public static TextFormatterDetailLevel	CurrentDetailLevel
		{
			get
			{
				return TextFormatter.detailLevel;
			}
		}


		public static FormattedText				DefaultUnknownText
		{
			get
			{
				var text = TextFormatter.GetCurrentCultureText (Epsitec.Common.Types.Res.StringIds.Text.Unknown);

				return text.ApplyItalic ();
			}
		}


		public static void DefineActiveCulture(CultureInfo cultureInfo)
		{
			TextFormatter.activeCulture = cultureInfo;
		}

		public static void DefineActiveCulture(string twoLetterISOLanguageName)
		{
			var fullName = twoLetterISOLanguageName + System.Threading.Thread.CurrentThread.CurrentCulture.Name.Substring (2);
			TextFormatter.DefineActiveCulture (CultureInfo.GetCultureInfo (fullName));
		}

		public static FormattedText GetMonolingualText(FormattedText text, string twoLetterISOLanguageName = null)
		{
			if (MultilingualText.IsMultilingual (text))
			{
				if (string.IsNullOrEmpty (twoLetterISOLanguageName))
				{
					twoLetterISOLanguageName = TextFormatter.CurrentTwoLetterISOLanguageName;
				}

				var multilingualText = new MultilingualText (text);
				var monolingual = multilingualText.GetTextOrDefault (twoLetterISOLanguageName);

				if (monolingual.IsNullOrEmpty ())
				{
					monolingual = multilingualText.GetTextOrDefault (TextFormatter.CurrentTwoLetterISOLanguageName);
				}

				return monolingual;
			}
			else
			{
				return text;
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

		public static FormattedText FormatField<T>(Expression<System.Func<T>> expression)
		{
			var marshaler = Marshaler.Create (expression);
			var fieldType = EntityInfo.GetFieldType (expression);
			
			return TextFormatter.FormatField (fieldType, marshaler.GetStringValue ());
		}

		public static FormattedText FormatField(INamedType type, string value)
		{
			var binder = FieldBinderFactory.Create (type);

			if (binder != null)
			{
				return binder.ConvertToUI (value);
			}
			else
			{
				return value;
			}
		}


		public static Caption GetCurrentCultureCaption(Druid captionId)
		{
			return Resources.DefaultManager.GetCaption (captionId, ResourceLevel.Merged, TextFormatter.CurrentCulture);
		}

		public static Caption GetCurrentCultureCaption(Caption caption)
		{
			return Resources.DefaultManager.GetCaption (caption, TextFormatter.CurrentCulture);
		}

		public static string GetCurrentCultureSimpleText(Druid druid)
		{
			return TextFormatter.GetCurrentCultureText (druid).ToSimpleText ();
		}

		public static FormattedText GetCurrentCultureText(Druid druid)
		{
			return new FormattedText (Resources.DefaultManager.GetText (druid, ResourceLevel.Merged, TextFormatter.CurrentCulture));
		}

		public static TResult ExecuteUsingCulture<TResult>(CultureInfo culture, System.Func<TResult> func)
		{
			if (TextFormatter.IsActiveCulture (culture))
			{
				return func ();
			}
			else
			{
				var savedCulture = TextFormatter.cultureOverride;

				try
				{
					TextFormatter.cultureOverride = culture;
					return func ();
				}
				finally
				{
					TextFormatter.cultureOverride = savedCulture;
				}
			}
		}

		public static void ExecuteUsingCulture(CultureInfo culture, System.Action action)
		{
			if (TextFormatter.IsActiveCulture (culture))
			{
				action ();
			}
			else
			{
				var savedCulture = TextFormatter.cultureOverride;

				try
				{
					TextFormatter.cultureOverride = culture;
					action ();
				}
				finally
				{
					TextFormatter.cultureOverride = savedCulture;
				}
			}
		}

		public static void ExecuteUsingCulture(string twoLetterCode, System.Action action)
		{
			TextFormatter.ExecuteUsingCulture (Resources.FindSpecificCultureInfo (twoLetterCode), action);
		}

		public static TResult ExecuteUsingCulture<TResult>(string twoLetterCode, System.Func<TResult> func)
		{
			return TextFormatter.ExecuteUsingCulture (Resources.FindSpecificCultureInfo (twoLetterCode), func);
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
				var multilingualText = new MultilingualText (formattedText);
				return multilingualText.GetTextOrDefault (TextFormatter.CurrentTwoLetterISOLanguageName).ToString ();
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

		public static System.IDisposable UsingCulture(CultureInfo culture, TextFormatterDetailLevel detailLevel)
		{
			var helper = new UsingCultureHelper ();
			TextFormatter.SetCultureOverride (culture, detailLevel);
			return helper;
		}

		
		private static void SetCultureOverride(CultureInfo culture, TextFormatterDetailLevel detailLevel)
		{
			TextFormatter.cultureOverride = culture;
			TextFormatter.detailLevel = detailLevel;
		}

		private static bool IsActiveCulture(CultureInfo culture)
		{
			return (culture == TextFormatter.CurrentCulture)
				|| (culture == null)
				|| (culture.Name == TextFormatter.CurrentCulture.Name);
		}

		private static void Preprocess(List<object> values)
		{
			int n = values.Count;
			FormatterHelper formatter = null;

			for (int i = 1; i < n; i++)
			{
				var text = values[i] as string;

				if ((text.StartsWith (Prefix.CommandEscape)) &&
					(text.StartsWith (Command.Format)))
				{
					var format = text.RemoveFirstToken (":");

					if (format.Length > 0)
					{
						if (formatter == null)
						{
							formatter = new FormatterHelper ();
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
			bool skipSpace = true;
			bool weKnowItemIsEmpty = false;
			bool conditionalParentheses = false;
			int  count     = items.Count;

			for (int i = 0; i < count; i++)
			{
				bool isLast = (i == count-1);
				string text = items[i] ?? "";
				string next = isLast ? "" : items[i+1];

				if (weKnowItemIsEmpty)
				{
					buffer.Append (text);
					emptyItem = true;
					weKnowItemIsEmpty = false;
					continue;
				}

				weKnowItemIsEmpty = false;

				conditionalParentheses = text.EndsWith ("(~");

				char prefix = text.FirstCharacterOfSimpleText ();

				if (prefix == Prefix.SkipItemIfPreviousEmpty)
				{
					if (emptyItem)
					{
						continue;
					}

					text   = text.Substring (1);
					prefix = text.FirstCharacterOfSimpleText ();
				}

				char suffix = text.LastCharacterOfSimpleText ();

				if (suffix == Suffix.SkipItemIfNextEmpty)
				{
					if (TextFormatter.IsEmptyItem (next, conditionalParentheses))
					{
						weKnowItemIsEmpty = true;
						continue;
					}

					text   = text.Substring (0, text.Length-1);
					suffix = text.LastCharacterOfSimpleText ();
				}

				if (TextFormatter.IsEmptyItem (text, conditionalParentheses))
				{
					buffer.Append (text);
					emptyItem = true;
					skipSpace = skipSpace || TextFormatter.EndsWithWhiteSpace (text);
					continue;
				}

				char lastCharacter = buffer.LastCharacterOfSimpleText ();

				if ((prefix.IsPunctuationMark ()) &&
					((lastCharacter == prefix) || emptyItem))
				{
					//	Duplicate punctuation mark... or punctuation mark on a still empty line.
				}
				else
				{
					if ((skipSpace == false) &&
						(lastCharacter != '(') &&
						(lastCharacter != '-') &&
						(lastCharacter != '+') &&
						(prefix != '\n') &&
						(prefix != '-') &&
						(prefix != '+') &&
						(prefix.IsPunctuationMark () == false) &&
						(prefix != ')'))
					{
						buffer.Append (" ");
					}

					buffer.Append (text);
				}

				emptyItem = TextFormatter.IsEmptyItem (text, conditionalParentheses);
				skipSpace = (emptyItem && skipSpace) || (!emptyItem && TextFormatter.EndsWithWhiteSpace (text));
			}
		}

		private static bool EndsWithWhiteSpace(string text)
		{
			if (string.IsNullOrEmpty (text))
			{
				return false;
			}
			if (text.EndsWith (FormattedText.HtmlBreak))
			{
				return true;
			}
			
			return char.IsWhiteSpace (text.LastCharacter ());
		}

		private static bool IsEmptyItem(string text, bool betweenParentheses = false)
		{
			if (text == null)
			{
				return true;
			}
			if (betweenParentheses && (text == Unicode.ToString (Unicode.Code.EmDash)))
			{
				return true;
			}

			text = FormattedText.Unescape (text);

			return string.IsNullOrWhiteSpace (text);
		}

		#region UsingCultureHelper Class

		private class UsingCultureHelper : System.IDisposable
		{
			public UsingCultureHelper()
			{
				this.cultureOverride = TextFormatter.cultureOverride;
				this.detailLevel     = TextFormatter.detailLevel;
			}

			#region IDisposable Members

			void System.IDisposable.Dispose()
			{
				TextFormatter.SetCultureOverride (this.cultureOverride, this.detailLevel);
			}

			#endregion

			private readonly CultureInfo		cultureOverride;
			private readonly TextFormatterDetailLevel	detailLevel;
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
			public const string ClearToMarkIfEqual	= "‼clearToMarkIfEqual";
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
						(i > 0))
					{
						if (string.IsNullOrWhiteSpace (items[i-1]))
						{
							TextFormatter.ClearGroup (items, i);
							TextFormatter.IgnoreMark (items, i);
							TextFormatter.ProcessTags (items);
						}
						else
						{
							TextFormatter.IgnoreMark (items, i);
							TextFormatter.ProcessTags (items);
						}
						break;
					}

					if ((command == Command.ClearToMarkIfEqual) &&
						(i > 2))
					{
						if (items[i-2] == items[i-1])
						{
							//	mark, "a", "b", "x", "y", clearToMarkIfEqual => 6 x ignore
							TextFormatter.ClearGroup (items, i);
							TextFormatter.IgnoreMark (items, i);
							TextFormatter.ProcessTags (items);
						}
						else
						{
							//	mark, "a", "b", "x", "x", clearToMarkIfEqual => ignore, "a", "b", 3 x ignore
							items[i-2] = Command.Ignore;
							items[i-1] = Command.Ignore;
							items[i-0] = Command.Ignore;
							TextFormatter.IgnoreMark (items, i);
							TextFormatter.ProcessTags (items);
						}
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

		private static void IgnoreMark(List<string> items, int index)
		{
			index = System.Math.Min (items.Count, index);

			for (int i = index-1; i > 0; i--)
			{
				if (items[i] == Command.Mark)
				{
					items[i] = Command.Ignore;
					break;
				}
			}
		}

		private static void ClearGroup(List<string> items, int index)
		{
			index = System.Math.Min (items.Count, index);

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
			TextFormatter.DefineDefaultConverter<Date> (x => x.ToString ("d", DateTimeFormatInfo.CurrentInfo));
			TextFormatter.DefineDefaultConverter<Time> (x => x.ToString ("t", DateTimeFormatInfo.CurrentInfo));
			TextFormatter.DefineDefaultConverter<System.DateTime> (x => x.ToString ("g", DateTimeFormatInfo.CurrentInfo));
			TextFormatter.DefineDefaultConverter<Caption> (x => x.DefaultLabelOrName);
			TextFormatter.DefineDefaultConverter<StructuredType> (x => x.Caption.DefaultLabelOrName);
			TextFormatter.DefineDefaultConverter<Druid> (x => x.IsEmpty ? TextFormatter.DefaultUnknownText.ToString () : EntityInfo.GetStructuredType (x).Caption.DefaultLabelOrName);
		}


		[System.ThreadStatic]
		private static CultureInfo				cultureOverride;

		[System.ThreadStatic]
		private static CultureInfo				activeCulture;

		[System.ThreadStatic]
		private static TextFormatterDetailLevel detailLevel;

		private static readonly object			exclusion = new object ();
		private static readonly Dictionary<System.Type, System.Func<object, string>> defaultConverters = new Dictionary<System.Type, System.Func<object, string>> ();
	}
}