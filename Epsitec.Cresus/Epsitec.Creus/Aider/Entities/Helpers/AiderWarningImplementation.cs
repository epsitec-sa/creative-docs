//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Entities.Helpers
{
	public static class AiderWarningImplementation
	{
		public static FormattedText GetSummary(IAiderWarning warning)
		{
			return TextFormatter.FormatText (warning.Description);
		}

		public static FormattedText GetCompactSummary(IAiderWarning warning)
		{
			return TextFormatter.FormatText (warning.Description.Lines.FirstOrDefault ());
		}

		public static FormattedText GetTitle<T>(T warning)
			where T : AbstractEntity, IAiderWarning
		{
			if (warning.Title.IsNullOrEmpty)
			{
				var entityId = warning.GetEntityStructuredTypeId ();
				var entityType = EntityInfo.GetStructuredType (entityId);
				
				return new FormattedText (entityType.Caption.DefaultLabelOrName);
			}
			else
			{
				return TextFormatter.FormatText (warning.Title);
			}
		}

		public static void Accumulate(IAiderWarning warning, EntityStatusAccumulator a)
		{
			a.Accumulate (warning.Title.GetEntityStatus ().TreatAsOptional ());
			a.Accumulate (warning.WarningType == Enumerations.WarningType.None ? EntityStatus.Empty | EntityStatus.Valid : EntityStatus.Valid);
			a.Accumulate (warning.Description.GetEntityStatus ().TreatAsOptional ());
			a.Accumulate (warning.Actions.Any () ? EntityStatus.Empty | EntityStatus.Valid : EntityStatus.Valid);
		}
	}
}
