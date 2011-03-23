//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support;

using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Documents.Verbose;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Documents
{
	/// <summary>
	/// Ce dictionnaire contient toutes les options permettant d'adapter l'impression d'un document.
	/// La clé du dictionnaire est un nom d'option (DocumentOption).
	/// La valeur du dictionnaire est la valeur de l'option. Par exemple "true".
	/// </summary>
	public class PrintingOptionDictionary
	{
		public PrintingOptionDictionary()
		{
			this.dictionary = new Dictionary<DocumentOption, string> ();
		}

		
		public int													Count
		{
			get
			{
				return this.dictionary.Count;
			}
		}

		public IEnumerable<DocumentOption>							Options
		{
			get
			{
				return this.dictionary.Keys;
			}
		}

		public IEnumerable<KeyValuePair<DocumentOption, string>>	ContentPair
		{
			get
			{
				foreach (var pair in this.dictionary)
				{
					yield return pair;
				}
			}
		}

		public string												this[DocumentOption option]
		{
			get
			{
				string value;
				if (this.dictionary.TryGetValue (option, out value))
				{
					return value;
				}
				else
				{
					return null;
				}
			}
			set
			{
				if (value == null)
				{
					this.dictionary.Remove (option);
				}
				else
				{
					this.dictionary[option] = value;
				}
			}
		}
		
		
		public void Clear()
		{
			this.dictionary.Clear ();
		}

		public bool ContainsOption(DocumentOption option)
		{
			return this.dictionary.ContainsKey (option);
		}


		public void MergeWith(PrintingOptionDictionary src)
		{
			if (src != null)
			{
				foreach (var pair in src.ContentPair)
				{
					this[pair.Key] = pair.Value;
				}
			}
		}

		public void Remove(PrintingOptionDictionary src)
		{
			if (src != null)
			{
				foreach (var pair in src.ContentPair)
				{
					this[pair.Key] = null;
				}
			}
		}


		public string GetSerializedData()
		{
			//	Retourne une chaîne qui représente tout le contenu du dictionnaire.
			var list = new List<string> ();

			foreach (var pair in this.dictionary)
			{
				list.Add (DocumentOptionConverter.ToString (pair.Key));
				list.Add (pair.Value);
			}

			return StringPacker.Pack (list);
		}

		public void SetSerializedData(string data)
		{
			//	Initialise le dictionnaire à partir d'une chaîne obtenue avec GetSerializedData.
			this.dictionary.Clear ();

			if (!string.IsNullOrEmpty (data))
			{
				var list = StringPacker.Unpack (data).ToList ();

				System.Diagnostics.Debug.Assert ((list.Count % 2) == 0);

				for (int i = 0; i+2 <= list.Count; i += 2)
				{
					var option = DocumentOptionConverter.Parse (list[i+0]);
					var value  = list[i+1];

					this.dictionary.Add (option, value);
				}
			}
		}


		public static PrintingOptionDictionary GetDefault()
		{
			//	Retourne toutes les options par défaut.
			var dict = new PrintingOptionDictionary ();
			var all = Verbose.VerboseDocumentOption.GetAll ();

			foreach (var item in all)
			{
				dict[item.Option] = item.DefaultValue;
			}

			return dict;
		}


		private static FormattedText GetSummary(PrintingOptionDictionary printingOptions)
		{
			var builder = new System.Text.StringBuilder ();
			bool first = true;

			var all = VerboseDocumentOption.GetAll ();
			foreach (var option in all)
			{
				if (option.Option != DocumentOption.None && printingOptions.ContainsOption (option.Option))
				{
					var description = option.Description;
					var value = printingOptions[option.Option];

					if (option.Type == DocumentOptionValueType.Boolean)
					{
						switch (value)
						{
							case "false":
								value = "non";
								break;

							case "true":
								value = "oui";
								break;
						}
					}

					if (option.Type == DocumentOptionValueType.Enumeration)
					{
						description = all.Where (x => x.IsTitle && x.Group == option.Group).Select (x => x.Title).FirstOrDefault ();

						for (int i = 0; i < option.Enumeration.Count (); i++)
						{
							if (option.Enumeration.ElementAt (i) == value)
							{
								value = option.EnumerationDescription.ElementAt (i);
								break;
							}
						}
					}

					if (option.Type == DocumentOptionValueType.Distance ||
						option.Type == DocumentOptionValueType.Size)
					{
						value = string.Concat (value, " mm");
					}

					if (string.IsNullOrEmpty (description))
					{
						description = option.Option.ToString ();
					}

					if (!first)
					{
						builder.Append ("<br/>");
					}

					builder.Append ("● ");
					builder.Append (description);
					builder.Append (" = ");
					builder.Append (value);

					first = false;
				}
			}

			if (first)
			{
				builder.Append ("● <i>Aucune</i>");
			}

			return builder.ToString ();
		}


		private class TextFormatterConverter : ITextFormatterConverter
		{
			#region ITextFormatterConverter Members

			public IEnumerable<System.Type> GetConvertibleTypes()
			{
				yield return typeof (PrintingOptionDictionary);
			}

			public FormattedText ToFormattedText(object value, System.Globalization.CultureInfo culture, TextFormatterDetailLevel detailLevel)
			{
				var printingOptions = (PrintingOptionDictionary) value;

				if (printingOptions == null)
				{
					return FormattedText.Empty;
				}
				else
				{
					switch (detailLevel)
					{
						case TextFormatterDetailLevel.Default:
						case TextFormatterDetailLevel.Title:
						case TextFormatterDetailLevel.Compact:
						case TextFormatterDetailLevel.Full:
							return PrintingOptionDictionary.GetSummary (printingOptions);

						default:
							throw new System.NotSupportedException (string.Format ("Detail level {0} not supported", detailLevel));
					}
				}
			}

			#endregion
		}


		private readonly Dictionary<DocumentOption, string>		dictionary;
	}
}