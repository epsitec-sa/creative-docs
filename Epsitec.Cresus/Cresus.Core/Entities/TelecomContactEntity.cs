//	Copyright � 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class TelecomContactEntity
	{
		public FormattedText GetTitle()
		{
			return TextFormatter.FormatText (this.TelecomType.Name);
		}

		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText (this.Number, "(", FormattedText.Join (", ", this.Roles.Select (role => role.Name).ToArray ()), ")");
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Number, "(", this.TelecomType.Name, ")");
		}

		public override EntityStatus GetEntityStatus()
		{
			var s1 = this.Number.GetEntityStatus ();
			var s2 = this.Extension.GetEntityStatus ().TreatAsOptional ();
			var s3 = EntityStatusHelper.CombineStatus (StatusHelperCardinality.All, this.Roles.Select (x => x.GetEntityStatus ()).ToArray ());
			var s4 = EntityStatusHelper.CombineStatus (StatusHelperCardinality.All, this.Comments.Select (x => x.GetEntityStatus ()).ToArray ());

			return EntityStatusHelper.CombineStatus (StatusHelperCardinality.All, s1, s2, s3, s4);
		}
	}
}
