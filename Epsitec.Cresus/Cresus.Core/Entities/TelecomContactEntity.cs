//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

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
	}
}
