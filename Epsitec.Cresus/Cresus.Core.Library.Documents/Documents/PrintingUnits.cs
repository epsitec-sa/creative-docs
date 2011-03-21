//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Documents
{
	/// <summary>
	/// Ce dictionnaire contient toutes les unités d'impression à utiliser pour l'impression d'un document.
	/// La clé du dictionnaire est un type de page (PageType).
	/// La valeur du dictionnaire est le nom d'une unité d'impression. Par exemple "Blanc".
	/// </summary>
	public class PrintingUnits
	{
		public PrintingUnits()
		{
			this.dictionary = new Dictionary<PageType, string> ();
		}


		public int											Count
		{
			get
			{
				return this.dictionary.Count;
			}
		}
		
		public IEnumerable<PageType>						PageTypes
		{
			get
			{
				return this.dictionary.Keys;
			}
		}

		public IEnumerable<KeyValuePair<PageType, string>>	ContentPair
		{
			get
			{
				foreach (var pair in this.dictionary)
				{
					yield return pair;
				}
			}
		}

		public string										this[PageType pageType]
		{
			get
			{
				string id;

				if (this.dictionary.TryGetValue (pageType, out id))
				{
					return id;
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
					this.dictionary.Remove (pageType);
				}
				else
				{
					this.dictionary[pageType] = value;
				}
			}
		}

		

		public bool ContainsPageType(PageType pageType)
		{
			return this.dictionary.ContainsKey (pageType);
		}

		public void MergeWith(PrintingUnits src)
		{
			foreach (var pair in src.ContentPair)
			{
				this.dictionary[pair.Key] = pair.Value;
			}
		}


		private readonly Dictionary<PageType, string>		dictionary;
	}
}
