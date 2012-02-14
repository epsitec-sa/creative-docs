//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Search.Data;
using Epsitec.Cresus.Compta.Options.Data;
using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Memory.Data
{
	/// <summary>
	/// Mémorise les paramètres des recherches, du filtre et des options, c'est-à-dire l'ensemble des paramètres
	/// liés à une présentation.
	/// </summary>
	public class MemoryData
	{
		public FormattedText Name
		{
			get;
			set;
		}


		public SearchData Search
		{
			get;
			set;
		}

		public SearchData Filter
		{
			get;
			set;
		}

		public AbstractOptions Options
		{
			get;
			set;
		}


		public bool ShowSearch
		{
			get;
			set;
		}

		public bool ShowFilter
		{
			get;
			set;
		}

		public bool ShowOptions
		{
			get;
			set;
		}


		public FormattedText GetSummary(List<ColumnMapper> columnMappers)
		{
			var s = this.GetSearchSummary  (columnMappers);
			var f = this.GetFilterSummary  (columnMappers);
			var o = this.GetOptionsSummary (columnMappers);

			var list = new List<string> ();

			if (!s.IsNullOrEmpty)
			{
				list.Add ("Rechercher " + s.ToSimpleText ());
			}

			if (!f.IsNullOrEmpty)
			{
				list.Add ("Filtrer " + f.ToSimpleText ());
			}

			if (!o.IsNullOrEmpty)
			{
				list.Add ("Options " + o.ToSimpleText ());
			}

			return string.Join (", ", list);
		}

		public FormattedText GetSearchSummary(List<ColumnMapper> columnMappers)
		{
			if (this.Search == null)
			{
				return FormattedText.Empty;
			}
			else
			{
				return this.Search.GetSummary (columnMappers);
			}
		}

		public FormattedText GetFilterSummary(List<ColumnMapper> columnMappers)
		{
			if (this.Filter == null)
			{
				return FormattedText.Empty;
			}
			else
			{
				return this.Filter.GetSummary (columnMappers);
			}
		}

		public FormattedText GetOptionsSummary(List<ColumnMapper> columnMappers)
		{
			if (this.Options == null)
			{
				return FormattedText.Empty;
			}
			else
			{
				return this.Options.Summary;
			}
		}

		public FormattedText PanelsSummary
		{
			get
			{
				//	Construit la liste des textes des panneaux visibles.
				var list = new List<string> ();

				if (this.ShowSearch)
				{
					list.Add ("recherche");
				}

				if (this.ShowFilter)
				{
					list.Add ("filtre");
				}

				if (this.ShowOptions)
				{
					list.Add ("options");
				}

				//	Génère le résumé.
				if (list.Count == 0)
				{
					return "Tous les panneaux sont cachés";
				}
				else if (list.Count == 1)
				{
					return "Panneau visible: " + Converters.SentenceConcat (list);
				}
				else
				{
					return "Panneaux visibles: " + Converters.SentenceConcat (list);
				}
			}
		}
	}
}