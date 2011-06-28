//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class DocumentMetadataEntity
	{
		public bool IsFrozen
		{
			get
			{
				return this.DocumentState == Business.DocumentState.Frozen;
			}
		}

		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText ("Document n°", this.IdA);
		}

		public override FormattedText GetCompactSummary()
		{
			if (this.DocumentCategory.IsNull ())
			{
				return TextFormatter.FormatText ("Document");
			}
			else
			{
				return TextFormatter.FormatText (this.DocumentCategory.Name);
			}
		}

		public FormattedText GetTitle()
		{
			return this.GetCompactSummary ();
		}


		public override EntityStatus GetEntityStatus()
		{
			using (var a = new EntityStatusAccumulator ())
			{
				a.Accumulate (this.IdA.GetEntityStatus ());
				a.Accumulate (this.IdB.GetEntityStatus ().TreatAsOptional ());
				a.Accumulate (this.IdC.GetEntityStatus ().TreatAsOptional ());

				a.Accumulate (this.DocumentTitle.GetEntityStatus ());
				a.Accumulate (this.Description.GetEntityStatus ());
				a.Accumulate (this.Comments.Select (x => x.GetEntityStatus ()));

				return a.EntityStatus;
			}
		}
	}
}
