//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus.Core;

using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Search.Data;
using Epsitec.Cresus.Compta.Options.Data;

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


		public FormattedText GetSummary(List<ColumnMapper> columnMappers)
		{
			var s = (this.Search  == null) ? FormattedText.Empty : this.Search.GetSummary (columnMappers, false);
			var f = (this.Filter  == null) ? FormattedText.Empty : this.Filter.GetSummary (columnMappers, true);
			var o = (this.Options == null) ? FormattedText.Empty : this.Options.Summary;

			return TextFormatter.FormatText (s, "~, ~", f, "~, ~", o);
		}
	}
}