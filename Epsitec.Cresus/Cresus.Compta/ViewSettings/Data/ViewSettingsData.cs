//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Search.Data;
using Epsitec.Cresus.Compta.Options.Data;
using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.ViewSettings.Data
{
	/// <summary>
	/// Mémorise les paramètres des recherches, du filtre et des options, c'est-à-dire l'ensemble des paramètres
	/// liés à une présentation (traduit en français par "réglage de présentation").
	/// </summary>
	public class ViewSettingsData
	{
		public ViewSettingsData()
		{
			this.ShowSearch   = ShowPanelMode.Nop;
			this.ShowFilter   = ShowPanelMode.Nop;
			this.ShowTemporal = ShowPanelMode.Nop;
			this.ShowOptions  = ShowPanelMode.Nop;

			this.EnableSearch   = true;
			this.EnableFilter   = true;
			this.EnableTemporal = true;
			this.EnableOptions  = true;
		}


		public FormattedText Name
		{
			get;
			set;
		}

		public bool Readonly
		{
			//	true -> réglage de présentation non modifiable
			get;
			set;
		}

		public bool Permanent
		{
			//	true -> réglage de présentation indestructible
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

		public TemporalData Temporal
		{
			get;
			set;
		}

		public AbstractOptions Options
		{
			get;
			set;
		}


		public bool EnableSearch
		{
			get;
			set;
		}

		public bool EnableFilter
		{
			get;
			set;
		}

		public bool EnableTemporal
		{
			get;
			set;
		}

		public bool EnableOptions
		{
			get;
			set;
		}


		public ShowPanelMode ShowSearch
		{
			get;
			set;
		}

		public ShowPanelMode ShowFilter
		{
			get;
			set;
		}

		public ShowPanelMode ShowTemporal
		{
			get;
			set;
		}

		public ShowPanelMode ShowOptions
		{
			get;
			set;
		}


		public FormattedText GetSummary(List<ColumnMapper> columnMappers)
		{
			var s = this.EnableSearch   ? this.GetSearchSummary   (columnMappers) : FormattedText.Empty;
			var f = this.EnableFilter   ? this.GetFilterSummary   (columnMappers) : FormattedText.Empty;
			var t = this.EnableTemporal ? this.GetTemporalSummary (columnMappers) : FormattedText.Empty;
			var o = this.EnableOptions  ? this.GetOptionsSummary  (columnMappers) : FormattedText.Empty;

			var list = new List<string> ();

			if (!s.IsNullOrEmpty)
			{
				list.Add ("Rechercher " + s);
			}

			if (!f.IsNullOrEmpty)
			{
				list.Add ("Filtrer " + f);
			}

			if (!t.IsNullOrEmpty)
			{
				list.Add ("Filtrer " + t);
			}

			if (!o.IsNullOrEmpty)
			{
				list.Add ("Options " + o);
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

		public FormattedText GetTemporalSummary(List<ColumnMapper> columnMappers)
		{
			if (this.Temporal == null)
			{
				return FormattedText.Empty;
			}
			else
			{
				return this.Temporal.GetSummary (columnMappers);
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


		public FormattedText ShowPanelModeSummary
		{
			//	Retourne le résumé lié à l'affichage des panneaux (action spéciale).
			get
			{
				if ((this.ShowSearch   == ShowPanelMode.Nop || this.ShowSearch   == ShowPanelMode.DoesNotExist) &&
					(this.ShowFilter   == ShowPanelMode.Nop || this.ShowFilter   == ShowPanelMode.DoesNotExist) &&
					(this.ShowTemporal == ShowPanelMode.Nop || this.ShowTemporal == ShowPanelMode.DoesNotExist) &&
					(this.ShowOptions  == ShowPanelMode.Nop || this.ShowOptions  == ShowPanelMode.DoesNotExist))
				{
					return FormattedText.Empty;
				}
				else if ((this.ShowSearch   == ShowPanelMode.Hide || this.ShowSearch   == ShowPanelMode.DoesNotExist) &&
						 (this.ShowFilter   == ShowPanelMode.Hide || this.ShowFilter   == ShowPanelMode.DoesNotExist) &&
						 (this.ShowTemporal == ShowPanelMode.Hide || this.ShowTemporal == ShowPanelMode.DoesNotExist) &&
						 (this.ShowOptions  == ShowPanelMode.Hide || this.ShowOptions  == ShowPanelMode.DoesNotExist))
				{
					return "Tout cacher";
				}
				else if ((this.ShowSearch   == ShowPanelMode.ShowBeginner || this.ShowSearch   == ShowPanelMode.DoesNotExist) &&
						 (this.ShowFilter   == ShowPanelMode.ShowBeginner || this.ShowFilter   == ShowPanelMode.DoesNotExist) &&
						 (this.ShowTemporal == ShowPanelMode.ShowBeginner || this.ShowTemporal == ShowPanelMode.DoesNotExist) &&
						 (this.ShowOptions  == ShowPanelMode.ShowBeginner || this.ShowOptions  == ShowPanelMode.DoesNotExist))
				{
					return "Tout montrer (simple)";
				}
				else if ((this.ShowSearch   == ShowPanelMode.ShowSpecialist || this.ShowSearch   == ShowPanelMode.DoesNotExist) &&
						 (this.ShowFilter   == ShowPanelMode.ShowSpecialist || this.ShowFilter   == ShowPanelMode.DoesNotExist) &&
						 (this.ShowTemporal == ShowPanelMode.ShowSpecialist || this.ShowTemporal == ShowPanelMode.DoesNotExist) &&
						 (this.ShowOptions  == ShowPanelMode.ShowSpecialist || this.ShowOptions  == ShowPanelMode.DoesNotExist))
				{
					return "Tout montrer (avancé)";
				}
				else
				{
					var list = new List<string> ();

					list.Add (this.GetShowPanelModeSummary (this.ShowSearch,   "les recherches"));
					list.Add (this.GetShowPanelModeSummary (this.ShowFilter,   "le filtre"));
					list.Add (this.GetShowPanelModeSummary (this.ShowTemporal, "le filtre"));
					list.Add (this.GetShowPanelModeSummary (this.ShowOptions,  "les options"));

					return Strings.FirstLetterToUpper (Strings.SentenceConcat (list));
				}
			}
		}

		private string GetShowPanelModeSummary(ShowPanelMode mode, string name)
		{
			switch (mode)
			{
				case ShowPanelMode.Hide:
					return string.Format ("cacher {0}", name);

				case ShowPanelMode.ShowBeginner:
					return string.Format ("montrer {0} (simple)", name);

				case ShowPanelMode.ShowSpecialist:
					return string.Format ("montrer {0} (avancé)", name);

				default:
					return null;
			}
		}
	}
}