//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class DocumentEntity
	{
		public virtual FormattedText GetSummary()
		{
			return FormattedText.Empty;
		}

		public virtual FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText ("Document n°", this.IdA);
		}
	}
}
