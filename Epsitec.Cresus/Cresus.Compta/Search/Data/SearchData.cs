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
	/// <summary>
	/// Contient les données complètes pour une recherche ou un filtre. Les données sont constituées d'une liste
	/// de SearchNodeData, elles-mêmes constituées d'une liste de SearchTabData.
	/// </summary>
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

			this.Enable = true;
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


		public int DeepCount
		{
			get
			{
				int count = 0;

				foreach (var node in this.NodesData)
				{
					count += node.DeepCount;
				}

				return count;
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

		public bool Enable
		{
			get;
			set;
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
			//	Indique si les données sont totalement vides.
			get
			{
				if (this.Enable == false)
				{
					return true;
				}

				return !this.IsDefined;
			}
		}

		public bool IsDefined
		{
			//	Indique si les données sont définies.
			get
			{
				if (this.nodesData.Count > 1)
				{
					return true;
				}

				foreach (var node in this.nodesData)
				{
					if (!node.IsEmpty)
					{
						return true;
					}
				}

				return false;
			}
		}


		public string BeginnerSearch
		{
			//	Texte unique de recherche en mode débutant.
			get
			{
				return this.nodesData[0].BeginnerSearch;
			}
			set
			{
				this.nodesData[0].BeginnerSearch = value;
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


		public int GetBeginnerNiveau(int niveau)
		{
			//	Retourne le niveau à utiliser pour l'indentation. Si on filtre les comptes inférieures à un
			//	certain niveau, on ne doit jamais indenter avec un niveau inférieur.
			int from, to;
			this.GetBeginnerProfondeurs (out from, out to);

			if (from != int.MaxValue)
			{
				from--;  // 0..n
				niveau = System.Math.Max (niveau-from, 0);
			}

			return niveau;
		}

		public void GetBeginnerProfondeurs(out int from, out int to)
		{
			this.nodesData[0].GetBeginnerProfondeurs(out from, out to);
		}

		public void SetBeginnerProfondeurs(int from, int to)
		{
			this.nodesData[0].SetBeginnerProfondeurs (from, to);
		}


		public bool BeginnerHideNuls
		{
			get
			{
				return this.nodesData[0].BeginnerHideNuls;
			}
			set
			{
				this.nodesData[0].BeginnerHideNuls = value;
			}
		}


		public void GetBeginnerDates(out Date? beginDate, out Date? endDate)
		{
			//	Retourne les dates à filtrer en mode débutant.
			if (this.Enable == false)
			{
				beginDate = null;
				endDate   = null;
				return;
			}

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

			//	Ne conserve que la première ligne.
			while (this.nodesData.Count > 1)
			{
				this.nodesData.RemoveAt (1);
			}

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
			if (this.IsEmpty)
			{
				return FormattedText.Empty;
			}
			else
			{
				var builder = new System.Text.StringBuilder ();

				bool first = true;
				bool many = this.nodesData.Count > 1;

				foreach (var node in this.nodesData)
				{
					int count;
					FormattedText s = node.GetSummary (columnMappers, out count);

					if (!s.IsNullOrEmpty)
					{
						if (!first)
						{
							builder.Append (this.OrMode ? " ou " : " et ");
						}

						if (many && count > 1)
						{
							builder.Append ("(");
						}

						builder.Append (s);

						if (many && count > 1)
						{
							builder.Append (")");
						}

						first = false;
					}
				}

				return builder.ToString ();
			}
		}


		public bool Process(List<SearchResult> results, int row, System.Func<int, ColumnType, FormattedText> getText, IEnumerable<ColumnType> columnTypes)
		{
			//	Effectue une recherche/filtre sur une ligne de données. Gère les modes or/and subtils liés aux nodes/tabs.
			//	La liste 'results' peut être nulle, ce qui est pratique pour le filtre, car on veut juste savoir si la ligne
			//	doit être exclue/inclue en fonction du bool en sortie.
			//	Retourne true si qq chose a été trouvé.
			bool allNode = true;
			bool oneNode = false;

			foreach (var node in this.nodesData.Where (x => !x.IsEmpty))
			{
				var tabs = node.TabsData.Where (x => !x.IsEmpty && x.Active);

				if (!tabs.Any ())
				{
					continue;
				}

				bool allTab = true;
				bool oneTab = false;

				foreach (var tab in tabs)
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