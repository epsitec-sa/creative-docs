//	Copyright © 2009-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Graph;
using Epsitec.Common.Graph.Data;
using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Graph.ImportConverters
{
	public static class ImportConverter
	{
		/// <summary>
		/// Converts the raw input data into a cube.
		/// </summary>
		/// <param name="headColumns">The head columns.</param>
		/// <param name="lines">The lines, already split into arrays of values.</param>
		/// <param name="converter">The successful converter.</param>
		/// <param name="cube">The cube.</param>
		/// <returns><c>true</c> if the data could be converted; otherwise, <c>false</c>.</returns>
		public static bool ConvertToCube(
			IEnumerable<string> headColumns, IEnumerable<IEnumerable<string>> lines, string sourcePath, IDictionary<string, string> meta,
			out AbstractImportConverter converter, out GraphDataCube cube)
		{
			List<string> head = new List<string> (headColumns);

			foreach (var pair in ImportConverter.GetConverters ().OrderByDescending (x => x.Value.Priority))
			{
				converter = pair.Value;
				
				if (meta != null)
                {
					if (!converter.CheckCompatibleMeta (meta))
					{
						continue;
					}
                }
				
				cube = converter.ToDataCube (head, lines, sourcePath, meta);

				if (cube != null)
				{
					converter = converter.CreateSpecificConverter (meta);
					return true;
				}
			}

			converter = null;
			cube      = null;

			return false;
		}

		/// <summary>
		/// Finds the converter with the specified name.
		/// </summary>
		/// <param name="name">The converter name.</param>
		/// <returns>The converter instance or <c>null</c> if the name does not match any converter.</returns>
		public static AbstractImportConverter FindConverter(string name)
		{
			var dict = ImportConverter.GetConverters ();
			
			AbstractImportConverter converter;

			if ((!string.IsNullOrEmpty (name)) &&
				(dict.TryGetValue (name, out converter)))
			{
				return converter;
			}
			else
			{
				return null;
			}
		}


		/// <summary>
		/// Splits a (possibly) quoted text, breaking up items between the specified
		/// separators.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <param name="separator">The separator.</param>
		/// <returns>The collection of items.</returns>
		public static IEnumerable<string> QuotedSplit(string text, char separator)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();

			char quote = '"';
			char final = '\0';
			bool insideQuotedText = false;
			int quoteCount = 0;

			foreach (char c in ImportConverter.TextWithFinalMarker (text, final))
			{
				//	Quotes are treated in a special way only if the first character for
				//	the current item was a quote; otherwise, quotes are just plain chars:

				if (c == quote)
				{
					if ((quoteCount == 0) &&
						(buffer.Length == 0))
					{
						insideQuotedText = true;
					}

					if (insideQuotedText)
					{
						quoteCount++;
					}
				}

				//	An unquoted separator (or the final marker) imply that we have reached
				//	the end of the current item and we have to yield it after cleaning up
				//	the possible mess with the quotes:
				
				if (((c == separator) && ((quoteCount % 2) == 0)) ||
					(c == final))
				{
					var value = buffer.ToString ();
					int count = buffer.Length;
					
					if ((insideQuotedText) &&
						(count > 1))
					{
						if ((value[0] == quote) &&
							(value[count-1] == quote))
                        {
							//	The item is a properly formed "xyz" string (quote-text-quote)
							//	but we will remove the quotes only if the text contains "" or
							//	a separator, in order to make up for the Excel strangeness of
							//	handling the quoting (copy&paste and CSV files are not handled
							//	the same way):

							var inner = value.Substring (1, count-2);

							if ((inner.Contains (separator)) ||
								(inner.Contains ("\"\"")))
							{
								value = inner.Replace ("\"\"", "\"");
							}
						}
					}

					yield return value;

					buffer.Length    = 0;
					insideQuotedText = false;
					quoteCount       = 0;
				}
				else
				{
					buffer.Append (c);
				}
			}
		}


		/// <summary>
		/// Enumerate all characters in the specified text, adding a final marker
		/// at the end.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <param name="marker">The marker.</param>
		/// <returns>The collection of characters.</returns>
		private static IEnumerable<char> TextWithFinalMarker(string text, char marker)
		{
			for (int i = 0; i < text.Length; i++)
			{
				yield return text[i];
			}

			yield return marker;
		}

		private static Dictionary<string, AbstractImportConverter> GetConverters()
		{
			if (ImportConverter.converters == null)
			{
				ImportConverter.converters = new Dictionary<string, AbstractImportConverter> ();

				foreach (var pair in ImportConverter.GetConverterTypes (typeof (ImportConverter).Assembly))
				{
					var args = new object[] { pair.Key };
					var type = pair.Value;
					var conv = System.Activator.CreateInstance (type, args) as AbstractImportConverter;

					conv.Priority = type.GetCustomAttributes<ImporterAttribute> ().First ().Priority;
					
					ImportConverter.converters[pair.Key] = conv;
				}
			}

			return ImportConverter.converters;
		}

		private static IEnumerable<KeyValuePair<string, System.Type>> GetConverterTypes(System.Reflection.Assembly assembly)
		{
			return from type in assembly.GetTypes ()
				   from attr in type.GetCustomAttributes<ImporterAttribute> ()
				   select new KeyValuePair<string, System.Type> (attr.Name, type);
		}


		private static Dictionary<string, AbstractImportConverter> converters;
	}
}
