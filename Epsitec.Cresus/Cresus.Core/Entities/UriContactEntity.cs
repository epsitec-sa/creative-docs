//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class UriContactEntity
	{
		public FormattedText GetSummary()
		{
			return TextFormatter.FormatText (this.Uri, "(", FormattedText.Join (", ", this.Roles.Select (role => role.Name).ToArray ()), ")");
		}

		public FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Uri);
		}
	}
}
