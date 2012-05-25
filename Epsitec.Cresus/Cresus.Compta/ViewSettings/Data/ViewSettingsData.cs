//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Search.Data;
using Epsitec.Cresus.Compta.Options.Data;
using Epsitec.Cresus.Compta.Widgets;
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
			this.Color = TabColor.None;
		}


		public FormattedText Name
		{
			get;
			set;
		}

		public bool Readonly
		{
			//	true -> présentation système non modifiable
			get;
			set;
		}


		public ControllerType ControllerType
		{
			get;
			set;
		}


		public bool ShowSearchPanel
		{
			//	Indique si le panneau des recherches est visible.
			get;
			set;
		}

		public bool ShowFilterPanel
		{
			//	Indique si le panneau du filtre est visible.
			get;
			set;
		}

		public bool ShowOptionsPanel
		{
			//	Indique si le panneau des options est visible.
			get;
			set;
		}


		public TabColor Color
		{
			get;
			set;
		}


		public SearchData BaseFilter
		{
			get;
			set;
		}

		public SearchData CurrentFilter
		{
			get
			{
				if (this.currentFilter == null && this.BaseFilter != null)
				{
					this.currentFilter = this.BaseFilter.CopyFrom ();
				}

				return this.currentFilter;
			}
			set
			{
				this.currentFilter = value;
			}
		}


		public AbstractOptions BaseOptions
		{
			get;
			set;
		}

		public AbstractOptions CurrentOptions
		{
			get
			{
				if (this.currentOptions == null && this.BaseOptions != null)
				{
					this.currentOptions = this.BaseOptions.CopyFrom ();
				}

				return this.currentOptions;
			}
			set
			{
				this.currentOptions = value;
			}
		}


		public FormattedText GetSummary(List<ColumnMapper> columnMappers)
		{
			var f = this.GetFilterSummary  (columnMappers);
			var o = this.GetOptionsSummary (columnMappers);

			var list = new List<string> ();

			if (!f.IsNullOrEmpty)
			{
				list.Add ("Filtrer " + f);
			}

			if (!o.IsNullOrEmpty)
			{
				list.Add ("Options " + o);
			}

			return string.Join (", ", list);
		}

		public FormattedText GetFilterSummary(List<ColumnMapper> columnMappers)
		{
			if (this.BaseFilter == null || columnMappers == null)
			{
				return FormattedText.Empty;
			}
			else
			{
				return this.BaseFilter.GetSummary (columnMappers);
			}
		}

		public FormattedText GetOptionsSummary(List<ColumnMapper> columnMappers)
		{
			if (this.BaseOptions == null)
			{
				return FormattedText.Empty;
			}
			else
			{
				return this.BaseOptions.Summary;
			}
		}


		private SearchData			currentFilter;
		private AbstractOptions		currentOptions;
	}
}