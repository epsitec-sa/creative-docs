//	Copyright � 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class PersonTitleEntity
	{
		public FormattedText GetSummary()
		{
			return TextFormatter.FormatText
				(
					"Abr�g�: ", this.ShortName, "\n",
					"Complet: ", this.Name
				);
		}

		public FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Name);
		}
	}
}
