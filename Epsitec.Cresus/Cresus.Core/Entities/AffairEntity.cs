//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class AffairEntity
	{
		public WorkflowEntity RootWorkflow
		{
			get
			{
				return this.Workflows.FirstOrDefault ().WrapNullEntity ();
			}
		}

		public IList<DocumentEntity> Documents
		{
			get
			{
				if (this.documents == null)
                {
					this.documents = new ObservableList<DocumentEntity> ();
					this.documents.AddRange (this.Workflows.SelectMany (w => w.Events).Where (e => e.HasDocument).Select (e => e.Document).Distinct ());
                }

				//	TODO : refresh this list when changes happen

				return this.documents.AsReadOnly ();
			}
		}


		public override FormattedText GetSummary()
		{
			var root = this.RootWorkflow;

			if (root.IsNull () || root.CreationEvent.IsNull ())
			{
				return TextFormatter.FormatText (this.IdA);
			}
			else
			{
				var date = Misc.GetDateTimeShortDescription (root.CreationEvent.Date);
				return TextFormatter.FormatText (this.IdA, " - ", date, "(", this.Documents.Count, "doc.)");
			}
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.IdA);
		}


		private ObservableList<DocumentEntity> documents;
	}
}