//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class AffairEntity
	{
		public FormattedText GetSummary()
		{
			int count = this.Events.Count;

			if (count == 0)
			{
				return TextFormatter.FormatText (this.IdA);
			}
			else
			{
				var date = Misc.GetDateTimeShortDescription (this.Events.First ().Date);
				return TextFormatter.FormatText (this.IdA, " - ", date, "(", count, "év.)");
			}
		}
	}
}