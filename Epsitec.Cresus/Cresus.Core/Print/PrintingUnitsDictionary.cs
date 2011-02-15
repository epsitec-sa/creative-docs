//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;

namespace Epsitec.Cresus.Core.Print
{
	/// <summary>
	/// Ce dictionnaire contient toutes les unités d'impression à utiliser pour l'impression d'un document.
	/// La clé du dictionnaire est un type de page (PageType).
	/// La valeur du dictionnaire est le nom d'une unité d'impression. Par exemple "Blanc".
	/// </summary>
	public class PrintingUnitsDictionary
	{
		public PrintingUnitsDictionary()
		{
			this.dictionary = new Dictionary<PageType, string> ();
		}


		public void Add(PageType pageType, string printingUnit)
		{
			this.dictionary[pageType] = printingUnit;
		}

		public void Remove(PageType pageType)
		{
			if (this.dictionary.ContainsKey (pageType))
			{
				this.dictionary.Remove (pageType);
			}
		}


		public int Count
		{
			get
			{
				return this.dictionary.Count;
			}
		}

		public bool ContainsPageType(PageType pageType)
		{
			return this.dictionary.ContainsKey (pageType);
		}

		public string GetPrintingUnit(PageType pageType)
		{
			if (this.dictionary.ContainsKey (pageType))
			{
				return this.dictionary[pageType];
			}
			else
			{
				return null;
			}
		}

		public IEnumerable<PageType> PageTypes
		{
			get
			{
				return this.dictionary.Keys;
			}
		}

		public IEnumerable<KeyValuePair<PageType, string>> ContentPair
		{
			get
			{
				foreach (var pair in this.dictionary)
				{
					yield return pair;
				}
			}
		}


		public void Merge(PrintingUnitsDictionary src)
		{
			foreach (var pair in src.ContentPair)
			{
				this.Add (pair.Key, pair.Value);
			}
		}


		private readonly Dictionary<PageType, string>		dictionary;
	}
}
