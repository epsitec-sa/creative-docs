//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Search.Data
{
	public class SearchData : ISettingsData
	{
		public SearchData()
		{
			this.tabsData = new List<SearchTabData> ();
			this.Clear ();
		}


		public void Clear()
		{
			//	Vide les données et prépare une unique ligne.
			this.tabsData.Clear ();
			this.Adjust ();

			this.OrMode = true;
		}

		public void Adjust()
		{
			//	Adapte les données pour avoir une ligne au minimum.
			if (!this.tabsData.Any ())
			{
				this.tabsData.Add (new SearchTabData ());
			}
		}


		public bool Specialist
		{
			//	false -> mode débutant
			//	true  -> mode spécialiste
			get;
			set;
		}


		public List<SearchTabData> TabsData
		{
			//	Retourne toutes les lignes de données.
			get
			{
				return this.tabsData;
			}
		}

		public bool OrMode
		{
			//	false -> mode "and"
			//	true  -> mode "or"
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


		public string BeginnerSearch
		{
			//	Texte unique de recherche en mode débutant.
			get
			{
				return this.tabsData[0].SearchText.FromText;
			}
			set
			{
				this.tabsData[0].SearchText.FromText = value;
				this.BeginnerAdjust (false);
			}
		}


		public CatégorieDeCompte BeginnerCatégories
		{
			//	Catégories à filtrer en mode débutant.
			get
			{
				var data = this.BeginnerCatégoriesData;

				if (data == null)
				{
					return CatégorieDeCompte.Tous;
				}
				else
				{
					return Converters.StringToCatégories (data.SearchText.FromText);
				}
			}
			set
			{
				var data = this.BeginnerCatégoriesData;

				if (value == CatégorieDeCompte.Tous || value == CatégorieDeCompte.Inconnu)
				{
					if (data != null)
					{
						this.tabsData.Remove (data);
						this.Adjust ();
					}
				}
				else
				{
					if (data == null)
					{
						//	Si on n'a trouvé aucune ligne, on en crée une.
						data = new SearchTabData ();
						this.tabsData.Add (data);
					}

					data.Column              = ColumnType.Catégorie;
					data.SearchText.Mode     = SearchMode.Jokers;
					data.SearchText.FromText = Converters.CatégoriesToString (value);

					this.BeginnerAdjust (true);
				}
			}
		}

		private SearchTabData BeginnerCatégoriesData
		{
			get
			{
				return this.tabsData.Where (x => x.Column == ColumnType.Catégorie).FirstOrDefault ();
			}
		}


		public void GetBeginnerProfondeurs(out int from, out int to)
		{
			from = 1;
			to   = int.MaxValue;  // de 1 à tous

			var data = this.BeginnerProfondeurData;

			if (data != null)
			{
				int p;

				if (int.TryParse (data.SearchText.FromText, out p))
				{
					from = p;
				}
				
				if (int.TryParse (data.SearchText.ToText, out p))
				{
					to = p;
				}
			}
		}

		public void SetBeginnerProfondeurs(int from, int to)
		{
			var data = this.BeginnerProfondeurData;

			if (from == 1 && to == int.MaxValue)  // tous ?
			{
				if (data != null)
				{
					this.tabsData.Remove (data);
					this.Adjust ();
				}
			}
			else
			{
				if (data == null)
				{
					//	Si on n'a trouvé aucune ligne, on en crée une.
					data = new SearchTabData ();
					this.tabsData.Add (data);
				}

				data.Column              = ColumnType.Profondeur;
				data.SearchText.Mode     = SearchMode.Interval;
				data.SearchText.FromText = (from == int.MaxValue) ? null : from.ToString ();
				data.SearchText.ToText   = (to   == int.MaxValue) ? null : to  .ToString ();

				this.BeginnerAdjust (true);
			}
		}

		private SearchTabData BeginnerProfondeurData
		{
			get
			{
				return this.tabsData.Where (x => x.Column == ColumnType.Profondeur).FirstOrDefault ();
			}
		}


		public bool BeginnerSoldesNuls
		{
			get
			{
				var data = this.BeginnerSoldesNulsData;

				if (data != null && data.SearchText.Invert && data.SearchText.FromText == Converters.MontantToString (0))
				{
					return false;
				}

				return true;
			}
			set
			{
				var data = this.BeginnerSoldesNulsData;

				if (value)  // affiche les comptes dont le solde est nul ?
				{
					if (data != null)
					{
						this.tabsData.Remove (data);
						this.Adjust ();
					}
				}
				else
				{
					if (data == null)
					{
						//	Si on n'a trouvé aucune ligne, on en crée une.
						data = new SearchTabData ();
						this.tabsData.Add (data);
					}

					data.Column              = ColumnType.Solde;
					data.SearchText.Mode     = SearchMode.WholeContent;
					data.SearchText.Invert   = true;
					data.SearchText.FromText = Converters.MontantToString (0);

					this.BeginnerAdjust (true);
				}
			}
		}

		private SearchTabData BeginnerSoldesNulsData
		{
			get
			{
				return this.tabsData.Where (x => x.Column == ColumnType.Solde).FirstOrDefault ();
			}
		}


		public void GetBeginnerDates(out Date? beginDate, out Date? endDate)
		{
			//	Retourne les dates à filtrer en mode débutant.
			var data = this.BeginnerDatesData;

			if (data == null)
			{
				beginDate = null;
				endDate   = null;
			}
			else
			{
				data.SearchText.GetIntervalDates (out beginDate, out endDate);
			}
		}

		public void SetBeginnerDates(Date? beginDate, Date? endDate)
		{
			//	Modifie les dates à filtrer en mode débutant.
			var data = this.BeginnerDatesData;

			if (beginDate == null && endDate == null)
			{
				if (data != null)
				{
					this.tabsData.Remove (data);
					this.Adjust ();
				}
			}
			else
			{
				if (data == null)
				{
					//	Si on n'a trouvé aucune ligne, on en crée une.
					data = new SearchTabData ();
					this.tabsData.Add (data);
				}

				data.Column              = ColumnType.Date;
				data.SearchText.Mode     = SearchMode.Interval;
				data.SearchText.FromText = Converters.DateToString (beginDate);
				data.SearchText.ToText   = Converters.DateToString (endDate);

				this.BeginnerAdjust (true);
			}
		}

		private SearchTabData BeginnerDatesData
		{
			get
			{
				foreach (var data in this.tabsData)
				{
					Date? beginDate, endDate;
					if (data.SearchText.GetIntervalDates (out beginDate, out endDate))
					{
						return data;
					}
				}

				return null;
			}
		}


		public void BeginnerAdjust(bool isFilter)
		{
			//	Ajuste les données après une modification en mode débutant.
			//	Il faut supprimer les données surnuméraires, afin d'obtenir un résultat
			//	conforme à ce qui est visible.
			if (isFilter)  // filtre ?
			{
				//	1) Cherche les données effectives.
				var dataCatégories = this.BeginnerCatégoriesData;
				var dataProfondeur = this.BeginnerProfondeurData;
				var dataSoldesNuls = this.BeginnerSoldesNulsData;
				var dataDates      = this.BeginnerDatesData;

				//	2) Supprime toutes les données-
				this.tabsData.Clear ();

				//	3) Puis remet les données effectives, dans le bon ordre.
				if (dataCatégories != null)
				{
					this.tabsData.Add (dataCatégories);
				}

				if (dataProfondeur != null)
				{
					this.tabsData.Add (dataProfondeur);
				}

				if (dataSoldesNuls != null)
				{
					this.tabsData.Add (dataSoldesNuls);
				}

				if (dataDates != null)
				{
					this.tabsData.Add (dataDates);
				}

				//	4) Met au moins une ligne s'il n'y a plus rien.
				this.Adjust ();

				//	Si plusieurs lignes sont utilisées, il faut mettre le mode "and".
				this.OrMode = false;
			}
			else  // recherche ?
			{
				//	Ne conserve que la première ligne.
				while (this.tabsData.Count > 1)
				{
					this.tabsData.RemoveAt (1);
				}
			}
		}


		public bool CompareTo(SearchData other)
		{
			if (other.OrMode != this.OrMode)
			{
				return false;
			}

			if (other.tabsData.Count != this.tabsData.Count)
			{
				return false;
			}

			for (int i = 0; i < this.tabsData.Count; i++)
			{
				if (!this.tabsData[i].CompareTo (other.tabsData[i]))
				{
					return false;
				}
			}

			return true;
		}

		public SearchData CopyFrom()
		{
			var data = new SearchData ();
			this.CopyTo (data);
			return data;
		}

		public void CopyTo(SearchData dst)
		{
			dst.OrMode = this.OrMode;

			dst.tabsData.Clear ();
			foreach (var tab in this.tabsData)
			{
				var n = new SearchTabData ();
				tab.CopyTo (n);
				dst.tabsData.Add (n);
			}
		}


		public FormattedText GetSummary(List<ColumnMapper> columnMappers)
		{
			if (this.IsEmpty)
			{
				return FormattedText.Empty;
			}
			else
			{
				var builder = new System.Text.StringBuilder ();

				bool first = true;
				foreach (var tab in this.tabsData)
				{
					FormattedText s = tab.GetSummary (columnMappers);

					if (!s.IsNullOrEmpty)
					{
						if (!first)
						{
							builder.Append (this.OrMode ? " ou " : " et ");
						}

						builder.Append (s);
						first = false;
					}
				}

				return builder.ToString ();
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