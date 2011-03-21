//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support;

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
				list.Add (DocumentOptions.ToString (pair.Key));
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

				for (int i = 0; i+2 < list.Count; i += 2)
				{
					var option = DocumentOptions.Parse (list[i+0]);
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


		private readonly Dictionary<DocumentOption, string>		dictionary;
	}
}