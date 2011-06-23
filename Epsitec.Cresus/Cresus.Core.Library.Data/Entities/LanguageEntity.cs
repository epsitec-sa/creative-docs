//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class LanguageEntity
	{
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText (this.Name, "(~", this.IsoLanguageCode, "~)");
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Name);
		}
		
		public override Common.Support.EntityEngine.EntityStatus GetEntityStatus()
		{
			using (var a = new EntityStatusAccumulator ())
			{
				a.Accumulate (this.Name.GetEntityStatus ());
				a.Accumulate (this.Description.GetEntityStatus ().TreatAsOptional ());
				a.Accumulate (this.IsoLanguageCode.GetEntityStatus ());

				return a.EntityStatus;
			}
		}
	}
}
