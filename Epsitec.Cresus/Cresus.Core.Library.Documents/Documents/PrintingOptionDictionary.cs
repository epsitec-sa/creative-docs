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

		public void Keep(IEnumerable<DocumentOption> list)
		{
			//	Ne garde que les options contenues dans la liste.
			if (list != null && list.Count () != 0)
			{
				var toRemove = new List<DocumentOption> ();

				foreach (var option in this.dictionary.Keys)
				{
					if (!list.Contains (option))
					{
						toRemove.Add (option);
					}
				}

				foreach (var option in toRemove)
				{
					this.dictionary.Remove (option);
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


		private FormattedText GetSummary()
		{
			//	Retourne un résumé complet d'un dictionnaire, composé des options et de leurs valeurs.

			//	On ne peut pas utiliser ce foutu TextBuilder, à cause des trop subtiles
			//	opérations effectuées (espaces ajoutés imprévisibles) !
			var builder = new System.Text.StringBuilder ();
			builder.Append ("Options et valeurs définies :<br/><font size=\"25%\"><br/></font>");

			bool first = true;
			var all = VerboseDocumentOption.GetAll ();

			foreach (var option in all)
			{
				if (option.Option != DocumentOption.None && this.ContainsOption (option.Option))
				{
					var description = option.Description;
					var value = this[option.Option];

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

					if (!first)  // déjà mis une option précédemment ?
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

			if (first)  // aucune option ?
			{
				builder.Append ("● <i>Aucune</i>");
			}

			return builder.ToString ();
		}


		private class TextFormatterConverter : TextFormatterConverter<PrintingOptionDictionary>
		{
			protected override FormattedText ToFormattedText(PrintingOptionDictionary value, System.Globalization.CultureInfo culture, TextFormatterDetailLevel detailLevel)
			{
				return value.GetSummary ();
			}
		}


		private readonly Dictionary<DocumentOption, string>		dictionary;
	}
}