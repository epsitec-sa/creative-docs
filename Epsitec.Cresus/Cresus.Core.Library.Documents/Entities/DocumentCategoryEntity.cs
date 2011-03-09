//	Copyright � 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
			return TextFormatter.FormatText (this.Name);
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Name);
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
