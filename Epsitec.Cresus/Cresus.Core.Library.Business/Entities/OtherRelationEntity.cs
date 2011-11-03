//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Business;

using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class OtherRelationEntity
	{
		public override FormattedText GetSummary()
		{
			return this.GetCompactSummary ();
		}

		public override FormattedText GetCompactSummary()
		{
			if (!this.Name.IsNullOrEmpty)
			{
				return this.Name;
			}
			else if (this.Person.IsNotNull ())
			{
				return this.Person.GetCompactSummary ();
			}
			else
			{
				return TextFormatter.FormatText ("vide").ApplyItalic ();
			}
		}

		public override EntityStatus GetEntityStatus()
		{
			using (var a = new EntityStatusAccumulator ())
			{
				a.Accumulate (this.Name.GetEntityStatus ().TreatAsOptional ());
				a.Accumulate (this.Description.GetEntityStatus ().TreatAsOptional ());
				a.Accumulate (this.Person, EntityStatusAccumulationMode.NoneIsPartiallyCreated);

				return a.EntityStatus;
			}
		}
	}
}
