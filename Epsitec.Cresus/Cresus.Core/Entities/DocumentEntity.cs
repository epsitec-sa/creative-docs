//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class DocumentEntity
	{
		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText ("Document n°", this.IdA);
		}

		public override EntityStatus GetEntityStatus()
		{
			var s1 = this.IdA.GetEntityStatus ();
			var s2 = this.IdB.GetEntityStatus ().TreatAsOptional ();
			var s3 = this.IdC.GetEntityStatus ().TreatAsOptional ();
			var s4 = this.DocumentTitle.GetEntityStatus ();
			var s5 = this.Description.GetEntityStatus ();
			var s6 = EntityStatusHelper.CombineStatus (StatusHelperCardinality.All, this.Comments.Select (x => x.GetEntityStatus ()).ToArray ());

			return EntityStatusHelper.CombineStatus (StatusHelperCardinality.All, s1, s2, s3, s4, s5, s6);
		}
	}
}
