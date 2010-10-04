//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class WorkflowThreadEntity
	{
		public WorkflowStepEntity CreationEvent
		{
			get
			{
				return this.History.FirstOrDefault ().WrapNullEntity ();
			}
		}

		public override FormattedText GetCompactSummary()
		{
			return this.GetDocumentName ();
		}

		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText ("Créé le ", this.History.First ().Date);
		}

		private FormattedText GetDocumentName()
		{
			FormattedText name;

			if (this.Documents.Count == 0)
			{
				name = FormattedText.FromSimpleText ("Aucun document");
			}
			else
			{
				BusinessDocumentEntity doc = this.Documents.First ();
				name = TextFormatter.FormatText ("Document", doc.IdA);
				//	TODO: faire mieux ici
			}

			return name;
		}
	}
}
