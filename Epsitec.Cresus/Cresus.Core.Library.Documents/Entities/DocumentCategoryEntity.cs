//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Business;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class DocumentCategoryEntity
	{
		public override FormattedText GetSummary()
		{
			return FormattedText.Concat
				(
					this.Name,
					"<br/> <br/>Options d'impression :<br/><font size=\"25%\"><br/></font>", this.GetDocumentOptionsSummary (),
					"<br/> <br/>Unités d'impression :<br/><font size=\"25%\"><br/></font>", this.GetPrintingUnitsSummary ());
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Name);
		}

		private FormattedText GetDocumentOptionsSummary()
		{
			var list = new List<string> ();

			foreach (var option in this.DocumentOptions)
			{
				list.Add (string.Concat ("● ", option.Name.ToString ()));
			}

			if (list.Count == 0)
			{
				return "● <i>Aucune</i>";
			}
			else
			{
				return string.Join ("<br/>", list);
			}
		}

		private FormattedText GetPrintingUnitsSummary()
		{
			var list = new List<string> ();

			foreach (var option in this.DocumentPrintingUnits)
			{
				list.Add (string.Concat ("● ", option.Name.ToString ()));
			}

			if (list.Count == 0)
			{
				return "● <i>Aucune</i>";
			}
			else
			{
				return string.Join ("<br/>", list);
			}
		}


		public override EntityStatus GetEntityStatus()
		{
			using (var a = new EntityStatusAccumulator ())
			{
				a.Accumulate (this.Code.GetEntityStatus ());
				a.Accumulate (this.Name.GetEntityStatus ().TreatAsOptional ());
				a.Accumulate (this.Description.GetEntityStatus ().TreatAsOptional ());

				return a.EntityStatus;
			}
		}

		public partial class Repository
		{
			public IEnumerable<DocumentCategoryEntity> Find(DocumentType type)
			{
				var example = this.CreateExample ();

				example.IsArchive    = false;
				example.DocumentType = type;

				return this.GetByExample (example).OrderBy (x => x.Rank);
			}
		}
	}
}
