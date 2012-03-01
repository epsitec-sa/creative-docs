﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
			this.nodesData = new List<SearchNodeData> ();
			this.Clear ();
		}


		private void Clear()
		{
			//	Vide les données et prépare une unique ligne.
			this.nodesData.Clear ();
			this.Adjust ();

			this.OrMode = false;
		}

		private void Adjust()
		{
			//	Adapte les données pour avoir une ligne au minimum.
			if (!this.nodesData.Any ())
			{
				this.nodesData.Add (new SearchNodeData ());
			}
		}


		public bool Specialist
		{
			//	false -> mode débutant
			//	true  -> mode spécialiste
			get;
			set;
		}


		public SearchTabData FirstTabData
		{
			//	Retourne la première ligne de données.
			get
			{
				return this.nodesData[0].TabsData[0];
			}
		}

		public List<SearchNodeData> NodesData
		{
			//	Retourne toutes les lignes de données.
			get
			{
				return this.nodesData;
			}
		}

		public bool OrMode
		{
			//	false -> mode "and"
			//	true  -> mode "or"
			get;
			set;
		}

		public bool IsValid
		{
			//	Indique si les données sont certifiées valides, c'est-à-dire aptes à être exploitées.
			get
			{
				if (this.IsEmpty)
				{
					return false;
				}

				foreach (var node in this.nodesData)
				{
					if (!node.IsValid)
					{
						return false;
					}
				}

				return true;
			}
		}

		public bool IsEmpty
		{
			//	Indique si les données sont totalement vides.
			//	Si les données sont partiellement remplies, on peut avoir IsValid et IsEmpty = false !
			get
			{
				if (this.nodesData.Count > 1)
				{
					return false;
				}
				
				foreach (var node in this.nodesData)
				{
					if (!node.IsEmpty)
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
				return this.nodesData[0].TabsData[0].SearchText.FromText;
			}
			set
			{
				this.nodesData[0].TabsData[0].SearchText.FromText = value;
				this.BeginnerAdjust (false);
			}
		}


		public CatégorieDeCompte BeginnerCatégories
		{
			//	Catégories à filtrer en mode débutant.
			get
			{
				return this.nodesData[0].BeginnerCatégories;
			}
			set
			{
				this.nodesData[0].BeginnerCatégories = value;
			}
		}


		public void GetBeginnerProfondeurs(out int from, out int to)
		{
			this.nodesData[0].GetBeginnerProfondeurs(out from, out to);
		}

		public void SetBeginnerProfondeurs(int from, int to)
		{
			this.nodesData[0].SetBeginnerProfondeurs (from, to);
		}


		public bool BeginnerSoldesNuls
		{
			get
			{
				return this.nodesData[0].BeginnerSoldesNuls;
			}
			set
			{
				this.nodesData[0].BeginnerSoldesNuls = value;
			}
		}


		public void GetBeginnerDates(out Date? beginDate, out Date? endDate)
		{
			//	Retourne les dates à filtrer en mode débutant.
			this.nodesData[0].GetBeginnerDates(out beginDate, out endDate);
		}

		public void SetBeginnerDates(Date? beginDate, Date? endDate)
		{
			//	Modifie les dates à filtrer en mode débutant.
			this.nodesData[0].SetBeginnerDates (beginDate, endDate);
		}


		public void BeginnerAdjust(bool isFilter)
		{
			//	Ajuste les données après une modification en mode débutant.
			//	Il faut supprimer les données surnuméraires, afin d'obtenir un résultat
			//	conforme à ce qui est visible.
			this.nodesData[0].BeginnerAdjust (isFilter);
		}


		public bool CompareTo(SearchData other)
		{
			if (other.nodesData.Count != this.nodesData.Count)
			{
				return false;
			}

			//	Avec une seule ligne, il ne faut pas tenir compte du mode et/ou, qui peut
			//	changer sans qu'il faille considérer les données comme différentes !
			if (other.nodesData.Count > 1 && other.OrMode != this.OrMode)
			{
				return false;
			}

			for (int i = 0; i < this.nodesData.Count; i++)
			{
				if (!this.nodesData[i].CompareTo (other.nodesData[i]))
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

			dst.nodesData.Clear ();
			foreach (var node in this.nodesData)
			{
				var n = new SearchNodeData ();
				node.CopyTo (n);
				dst.nodesData.Add (n);
			}
		}


		public FormattedText GetSummary(List<ColumnMapper> columnMappers)
		{
			if (this.IsValid)
			{
				var builder = new System.Text.StringBuilder ();

				bool first = true;
				bool many = this.nodesData.Count > 1;
				foreach (var node in this.nodesData)
				{
					FormattedText s = node.GetSummary (columnMappers);

					if (!s.IsNullOrEmpty)
					{
						if (!first)
						{
							builder.Append (this.OrMode ? " ou " : " et ");
						}

						if (many)
						{
							builder.Append ("(");
						}

						builder.Append (s);

						if (many)
						{
							builder.Append (")");
						}

						first = false;
					}
				}

				return builder.ToString ();
			}
			else
			{
				return FormattedText.Empty;
			}
		}


		public bool Process(List<SearchResult> results, int row, System.Func<int, ColumnType, FormattedText> getText, IEnumerable<ColumnType> columnTypes)
		{
			//	Effectue une recherche/filtre sur une ligne de données.
			//	Retourne true si qq chose a été trouvé.
			bool allNode = true;
			bool oneNode = false;

			foreach (var node in this.nodesData)
			{
				if (node.IsEmpty)
				{
					continue;
				}

				bool allTab = true;
				bool oneTab = false;

				foreach (var tab in node.TabsData)
				{
					bool tabFound = false;

					if (tab.Column == ColumnType.None)  // cherche dans toutes les colonnes ?
					{
						foreach (var column in columnTypes)
						{
							FormattedText text = getText (row, column);
							int n = tab.SearchText.Search (ref text);

							if (n != 0)  // trouvé ?
							{
								if (results != null)
								{
									results.Add (new SearchResult (row, column, text));
								}

								tabFound = true;
							}
						}
					}
					else  // cherche dans une colonne précise ?
					{
						FormattedText text = getText (row, tab.Column);
						int n = tab.SearchText.Search (ref text);

						if (n != 0)  // trouvé ?
						{
							if (results != null)
							{
								results.Add (new SearchResult (row, tab.Column, text));
							}

							tabFound = true;
						}
					}

					if (tabFound)  // trouvé ?
					{
						oneTab = true;
					}
					else  // pas trouvé ?
					{
						allTab = false;
					}
				}

				if (node.OrMode)  // mode "ou" ?
				{
					if (oneTab)
					{
						oneNode = true;
					}
					else
					{
						allNode = false;
					}
				}
				else  // mode "et" ?
				{
					if (allTab)
					{
						oneNode = true;
					}
					else
					{
						allNode = false;
					}
				}
			}

			bool found = false;

			if (this.OrMode)  // mode "ou" ?
			{
				if (oneNode)
				{
					found = true;
				}
			}
			else  // mode "et" ?
			{
				if (allNode)
				{
					found = true;
				}
			}

			return found;
		}


		private readonly List<SearchNodeData>		nodesData;
	}
}