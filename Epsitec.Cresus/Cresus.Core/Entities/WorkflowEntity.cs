//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class WorkflowEntity
	{
		public WorkflowEventEntity CreationEvent
		{
			get
			{
				return this.Events.FirstOrDefault ().WrapNullEntity ();
			}
		}

		public FormattedText GetCompactSummary()
		{
			return this.GetDocumentName ();
		}

		public FormattedText GetSummaryDescription()
		{
			FormattedText version  = this.Id == 0 ? FormattedText.Empty : TextFormatter.FormatText ("Variante", this.Id);
			FormattedText creation = this.ActiveDocument.IsNull () ? FormattedText.Empty : TextFormatter.FormatText ("Créé le ", this.ActiveDocument.CreationDate);

			return TextFormatter.FormatText (version, "\n", creation);
		}
		
		private FormattedText GetDocumentName()
		{
			FormattedText name;

			if (this.ActiveDocument.IsNull ())
			{
				name = FormattedText.FromSimpleText ("Aucun document");
			}
			else
			{
				name = TextFormatter.FormatText (this.ActiveDocument.Description, this.ActiveDocument.IdA);
			}

			return name;
		}
	}
}
