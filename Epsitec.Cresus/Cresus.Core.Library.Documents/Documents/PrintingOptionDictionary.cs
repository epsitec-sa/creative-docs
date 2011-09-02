//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support;

using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Documents.Verbose;
using Epsitec.Cresus.Core.Business;

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


		private FormattedText GetSummary(int maxOptionLines)
		{
			//	Retourne un résumé complet d'un dictionnaire, composé des options et de leurs valeurs.

			//	On ne peut pas utiliser ce foutu TextBuilder, à cause des trop subtiles
			//	opérations effectuées (espaces ajoutés imprévisibles) !
			var builder = new System.Text.StringBuilder ();
			builder.Append ("Options et valeurs définies :<br/><font size=\"25%\"><br/></font>");

			var options = VerboseDocumentOption.GetAll ().Where (x => x.Option != DocumentOption.None && this.ContainsOption (x.Option));
			var count = options.Count ();

			if (count == 0)
			{
				builder.Append ("● <i>Aucune</i><br/>");
			}
			else
			{
				for (int i = 0; i < count; i++)
				{
					if (i < maxOptionLines || i == count-1)  // n premières options ou dernière ?
					{
						builder.Append (this.GetOptionDescription (options.ElementAt (i), hasBullet: true, hiliteValue: false));
						builder.Append ("<br/>");
					}
					else if (i == maxOptionLines)
					{
						builder.Append (string.Format ("● <i>{0} options diverses...</i><br/>", (count-maxOptionLines-1).ToString ()));
					}
				}
			}

			builder.Append (" <br/>Adapté aux documents suivants :<br/><font size=\"25%\"><br/></font>");
			builder.Append (this.GetDocumentTypesSummary ());

			return builder.ToString ();
		}

		public FormattedText GetOptionDescription(VerboseDocumentOption option, bool hasBullet, bool hiliteValue)
		{
			FormattedText description, value;
			this.GetOptionDescription (option, false, out description, out value);

			FormattedText bullet = hasBullet ? "● " : "";

			if (hiliteValue)
			{
				value = value.ApplyBold ();
			}

			return FormattedText.Concat (bullet, description, " = ", value);
		}

		public void GetOptionDescription(VerboseDocumentOption option, bool useDefaultValue, out FormattedText description, out FormattedText value)
		{
			description = option.ShortDescription;
			value = useDefaultValue ? option.DefaultValue : this[option.Option];

			if (option.Type == DocumentOptionValueType.Boolean)
			{
				if (value == "false")
				{
					value = "non";
				}

				if (value == "true")
				{
					value = "oui";
				}
			}

			if (option.Type == DocumentOptionValueType.Enumeration)
			{
				description = option.ShortDescription;

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

			if (description.IsNullOrEmpty)
			{
				description = option.Option.ToString ();  // faute de mieux !
			}
		}


		private class TextFormatterConverter : TextFormatterConverter<PrintingOptionDictionary>
		{
			protected override FormattedText ToFormattedText(PrintingOptionDictionary value, System.Globalization.CultureInfo culture, TextFormatterDetailLevel detailLevel)
			{
				return value.GetSummary (20);
			}
		}


		public FormattedText GetDocumentTypesSummary()
		{
			var types = EnumKeyValues.FromEnum<DocumentType> ();
			var strings = new List<string> ();

			foreach (DocumentType type in this.DocumentTypes)
			{
				var t = types.Where (x => x.Key == type).FirstOrDefault ();

				if (t != null)
				{
					strings.Add (string.Concat ("● ", t.Values[0]));
				}
			}

			if (strings.Count == 0)
			{
				strings.Add (string.Concat ("● ", "Tous"));
			}

			return string.Join ("<br/>", strings);
		}

		private IEnumerable<DocumentType> DocumentTypes
		{
			get
			{
				var list = new List<DocumentType> ();
				int implementedDocumentCount = 0;

				foreach (DocumentType documentType in System.Enum.GetValues (typeof (DocumentType)))
				{
					if (documentType == DocumentType.None   ||
						documentType == DocumentType.Unknown)
					{
						continue;
					}

					var options = External.CresusCore.GetRequiredDocumentOptionsByDocumentType (documentType);

					if (options != null && options.Any ())
					{
						implementedDocumentCount++;

						foreach (var option in this.Options)
						{
							if (options.Contains (option))
							{
								if (!list.Contains (documentType))
								{
									list.Add (documentType);
								}
							}
						}
					}
				}

				if (list.Count == implementedDocumentCount)
				{
					list.Clear ();
				}

				return list;
			}
		}


		private readonly Dictionary<DocumentOption, string>		dictionary;
	}
}