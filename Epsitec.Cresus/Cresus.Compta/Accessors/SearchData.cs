//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Accessors
{
	public class SearchData : ISettingsData
	{
		public SearchData()
		{
			this.tabsData = new List<SearchTabData> ();
			this.OrMode = true;
		}


		public bool Specialist
		{
			get;
			set;
		}


		public List<SearchTabData> TabsData
		{
			get
			{
				return this.tabsData;
			}
		}

		public bool OrMode
		{
			get;
			set;
		}

		public bool IsEmpty
		{
			get
			{
				foreach (var tab in this.tabsData)
				{
					if (!tab.IsEmpty)
					{
						return false;
					}
				}

				return true;
			}
		}


		public bool GetIntervalDates(out Date? beginDate, out Date? endDate)
		{
			foreach (var data in this.tabsData)
			{
				if (data.SearchText.GetIntervalDates (out beginDate, out endDate))
				{
					return true;
				}
			}

			beginDate = null;
			endDate   = null;
			return false;
		}

		public SearchTabData GetIntervalDatesData()
		{
			foreach (var data in this.tabsData)
			{
				Date? beginDate, endDate;
				if (data.SearchText.GetIntervalDates (out beginDate, out endDate))
				{
					data.Column = ColumnType.Date;
					return data;
				}
			}

			//	Si on n'a pas trouvé un intervalle de dates, on prend n'importe lequel.
			foreach (var data in this.tabsData)
			{
				if (data.SearchText.Mode == SearchMode.Interval)
				{
					data.Column = ColumnType.Date;
					return data;
				}
			}

			//	Si on n'a trouvé aucun invervalle, on en crée un.
			var interval = new SearchTabData (null);
			this.tabsData.Add (interval);

			interval.Column = ColumnType.Date;
			interval.SearchText.Mode = SearchMode.Interval;

			return interval;
		}


		public CatégorieDeCompte Catégorie
		{
			get
			{
				var data = this.GetCatégorieData ();
				return SearchData.StringToCatégories (data.SearchText.FromText);
			}
			set
			{
				var data = this.GetCatégorieData ();
				data.SearchText.FromText = SearchData.CatégoriesToString (value);
			}
		}

		public SearchTabData GetCatégorieData()
		{
			foreach (var data in this.tabsData)
			{
				if (data.Column == ColumnType.Catégorie)
				{
					data.SearchText.Mode = SearchMode.Jokers;
					return data;
				}
			}

			//	Si on n'a trouvé aucune catégorie, on en crée une.
			var cat = new SearchTabData (null);
			this.tabsData.Add (cat);

			cat.Column = ColumnType.Catégorie;
			cat.SearchText.Mode = SearchMode.Jokers;

			return cat;
		}

		private static string CatégoriesToString(CatégorieDeCompte catégorie)
		{
			var list = new List<string> ();

			if ((catégorie & CatégorieDeCompte.Actif) != 0)
			{
				list.Add (SearchData.CatégorieToString (CatégorieDeCompte.Actif));
			}

			if ((catégorie & CatégorieDeCompte.Passif) != 0)
			{
				list.Add (SearchData.CatégorieToString (CatégorieDeCompte.Passif));
			}

			if ((catégorie & CatégorieDeCompte.Charge) != 0)
			{
				list.Add (SearchData.CatégorieToString (CatégorieDeCompte.Charge));
			}

			if ((catégorie & CatégorieDeCompte.Produit) != 0)
			{
				list.Add (SearchData.CatégorieToString (CatégorieDeCompte.Produit));
			}

			if ((catégorie & CatégorieDeCompte.Exploitation) != 0)
			{
				list.Add (SearchData.CatégorieToString (CatégorieDeCompte.Exploitation));
			}

			return string.Join ("|", list);
		}

		private static CatégorieDeCompte StringToCatégories(string text)
		{
			var catégorie = CatégorieDeCompte.Inconnu;

			if (!string.IsNullOrEmpty (text))
			{
				var words = text.Split ('|');

				foreach (var word in words)
				{
					catégorie |= SearchData.StringToCatégorie (word);
				}
			}

			return catégorie;
		}

		private static string CatégorieToString(CatégorieDeCompte catégorie)
		{
			switch (catégorie)
			{
				case CatégorieDeCompte.Actif:
					return "Actif";

				case CatégorieDeCompte.Passif:
					return "Passif";

				case CatégorieDeCompte.Charge:
					return "Charge";

				case CatégorieDeCompte.Produit:
					return "Produit";

				case CatégorieDeCompte.Exploitation:
					return "Exploitation";

				default:
					return "?";
			}
		}

		private static CatégorieDeCompte StringToCatégorie(string text)
		{
			switch (text)
			{
				case "Actif":
					return CatégorieDeCompte.Actif;

				case "Passif":
					return CatégorieDeCompte.Passif;

				case "Charge":
					return CatégorieDeCompte.Charge;

				case "Produit":
					return CatégorieDeCompte.Produit;

				case "Exploitation":
					return CatégorieDeCompte.Exploitation;

				default:
					return CatégorieDeCompte.Inconnu;
			}
		}


		public static bool DateInRange(Date? date, Date? beginDate, Date? endDate)
		{
			if (date.HasValue)
			{
				if (beginDate.HasValue && date.Value < beginDate.Value)
				{
					return false;
				}

				if (endDate.HasValue && date.Value > endDate.Value)
				{
					return false;
				}
			}

			return true;
		}


		private readonly List<SearchTabData>		tabsData;
	}
}