//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Business;

namespace Epsitec.Aider.Entities
{
	public partial class AiderWarningSourceEntity
	{
		public override FormattedText GetTitle()
		{
			return this.Name;
		}

		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText (this.Description);
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Description.Lines.FirstOrDefault ());
		}

		public override EntityStatus GetEntityStatus()
		{
			using (var a = new EntityStatusAccumulator ())
			{
				a.Accumulate (this.Name.GetEntityStatus ().TreatAsOptional ());
				a.Accumulate (this.Description.GetEntityStatus ().TreatAsOptional ());
				a.Accumulate (this.CreationDate.GetEntityStatus ());
				a.Accumulate (this.WarningEntity.GetEntityStatus ());

				return a.EntityStatus;
			}
		}

		partial void GetWarnings(ref IList<IAiderWarning> value)
		{
			throw new System.NotImplementedException ();
		}

		public static AiderWarningSourceEntity Create<T>(BusinessContext context, System.DateTime date, FormattedText name, FormattedText description)
			where T : AbstractEntity, IAiderWarning, new ()
		{
			var warningSource = context.CreateEntity<AiderWarningSourceEntity> ();

			warningSource.CreationDate  = date;
			warningSource.Name          = name;
			warningSource.Description   = description;
			warningSource.WarningEntity = EntityInfo<T>.GetTypeId ().ToString ();

			return warningSource;
		}
	}
}

