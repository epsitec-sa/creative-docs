//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Entities
{
	public static class AiderWarningImplementation
	{
		public static FormattedText GetSummary(IAiderWarning warning)
		{
			return TextFormatter.FormatText (warning.Title, "~\n~", warning.Description);
		}

		public static FormattedText GetCompactSummary(IAiderWarning warning)
		{
			return TextFormatter.FormatText (warning.Description.Lines.FirstOrDefault ());
		}

		public static FormattedText GetTitle(IAiderWarning warning)
		{
			return TextFormatter.FormatText (warning.WarningType);
		}
		
		public static FormattedText GetTypeName<T>(T warning)
			where T : AbstractEntity, IAiderWarning
		{
			if (warning.Title.IsNullOrEmpty ())
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
			a.Accumulate (warning.StartDate.GetEntityStatus ().TreatAsOptional ());
			a.Accumulate (warning.EndDate.GetEntityStatus ().TreatAsOptional ());
			a.Accumulate (warning.HideUntilDate.GetEntityStatus ().TreatAsOptional ());
			a.Accumulate (warning.Title.GetEntityStatus ().TreatAsOptional ());
			a.Accumulate (warning.WarningType == Enumerations.WarningType.None ? EntityStatus.Empty | EntityStatus.Valid : EntityStatus.Valid);
			a.Accumulate (warning.WarningSource);
			a.Accumulate (warning.Description.GetEntityStatus ().TreatAsOptional ());
		}
	}
}
