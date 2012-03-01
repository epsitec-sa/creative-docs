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
			this.nodesData = new List<SearchNodeData> ();
			this.Clear ();
		}


		private void Clear()
		{
			//	Vide les données et prépare une unique ligne.
			this.nodesData.Clear ();
			this.Adjust ();

			this.OrMode = true;
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

		public bool IsEmpty
		{
			get
			{
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
		}


		private readonly List<SearchNodeData>		nodesData;
	}
}