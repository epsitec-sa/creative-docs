//	Copyright © 2010-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class LegalPersonTypeEntity
	{
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText (this.Name, "(~", this.ShortName, "~)");
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Name);
		}

		public override IEnumerable<TextValue> GetTextValues()
		{
			yield return new TextValue (TextValueCategory.Name, this.Name);
			yield return new TextValue (TextValueCategory.Name, this.ShortName);
		}

		public override EntityStatus GetEntityStatus()
		{
			return this.Name.GetEntityStatus ();
		}
	}
}
