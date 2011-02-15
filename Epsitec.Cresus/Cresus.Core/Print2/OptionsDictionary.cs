//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;

namespace Epsitec.Cresus.Core.Print2
{
	/// <summary>
	/// Ce dictionnaire contient toutes les options permettant d'adapter l'impression d'un document.
	/// La clé du dictionnaire est un nom d'option. Par exemple "HeaderLogo".
	/// La valeur du dictionnaire est la valeur de l'option. Par exemple "true".
	/// </summary>
	public class OptionsDictionary
	{
		public OptionsDictionary()
		{
			this.dictionary = new Dictionary<DocumentOption, string> ();
		}


		public void Add(DocumentOption option, string value)
		{
			this.dictionary[option] = value;
		}

		public void Remove(DocumentOption option)
		{
			if (this.dictionary.ContainsKey (option))
			{
				this.dictionary.Remove (option);
			}
		}


		public int Count
		{
			get
			{
				return this.dictionary.Count;
			}
		}

		public bool ContainsOption(DocumentOption option)
		{
			return this.dictionary.ContainsKey (option);
		}

		public string GetValue(DocumentOption option)
		{
			if (this.dictionary.ContainsKey (option))
			{
				return this.dictionary[option];
			}
			else
			{
				return null;
			}
		}

		public IEnumerable<DocumentOption> Options
		{
			get
			{
				return this.dictionary.Keys;
			}
		}

		public IEnumerable<KeyValuePair<DocumentOption, string>> ContentPair
		{
			get
			{
				foreach (var pair in this.dictionary)
				{
					yield return pair;
				}
			}
		}


		public void Merge(OptionsDictionary src)
		{
			foreach (var pair in src.ContentPair)
			{
				this.Add (pair.Key, pair.Value);
			}
		}


		public static OptionsDictionary GetDefault()
		{
			//	Retourne toutes les options par défaut.
			var dict = new OptionsDictionary ();
			var all = Verbose.VerboseDocumentOption.GetAll ();

			foreach (var one in all)
			{
				dict.Add (one.Option, one.DefaultValue);
			}

			return dict;
		}

		public static OptionsDictionary GetGlobal()
		{
			//	Retourne toutes les options globales.
			var dict = new OptionsDictionary ();
			var all = Verbose.VerboseDocumentOption.GetAll ();

			foreach (var one in all)
			{
				if (one.IsGlobal)
				{
					dict.Add (one.Option, one.DefaultValue);
				}
			}

			return dict;
		}


		private readonly Dictionary<DocumentOption, string>		dictionary;
	}
}
