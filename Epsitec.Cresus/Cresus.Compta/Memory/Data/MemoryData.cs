﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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


		public FormattedText GetSummary(List<ColumnMapper> columnMappers, FormattedText prefix, FormattedText postfix)
		{
			//	Retourne le résumé de la mémoire.
			var builder = new System.Text.StringBuilder ();
			bool first = true;

			if (this.Search != null)
			{
				var summary = this.Search.GetSummary (columnMappers, false);

				if (!summary.IsNullOrEmpty)
				{
					if (!first)
					{
						builder.Append (postfix);
					}

					builder.Append (prefix);
					builder.Append (summary);
					first = false;
				}
			}

			if (this.Filter != null)
			{
				var summary = this.Filter.GetSummary (columnMappers, true);

				if (!summary.IsNullOrEmpty)
				{
					if (!first)
					{
						builder.Append (postfix);
					}

					builder.Append (prefix);
					builder.Append (summary);
					first = false;
				}
			}

			if (this.Options != null)
			{
				var summary = this.Options.Summary;

				if (!summary.IsNullOrEmpty)
				{
					if (!first)
					{
						builder.Append (postfix);
					}

					builder.Append (prefix);
					builder.Append (summary);
					first = false;
				}
			}

			if (!first)
			{
				builder.Append (postfix);
			}

			builder.Append (prefix);
			builder.Append (this.SummaryShowedPanels);

			return builder.ToString ();
		}

		private FormattedText SummaryShowedPanels
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
					return "tous les panneaux sont cachés";
				}
				else if (list.Count == 1)
				{
					return "panneau visible: " + Converters.NiceConcat (list);
				}
				else
				{
					return "panneaux visibles: " + Converters.NiceConcat (list);
				}
			}
		}
	}
}