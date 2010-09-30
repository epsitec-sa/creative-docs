//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class ArticleQuantityEntity
	{
		public override FormattedText GetSummary()
		{
			return this.GetCompactSummary ();
		}

		public override FormattedText GetCompactSummary()
		{
			string type = null;
			foreach (var q in Business.Enumerations.GetAllPossibleValueArticleQuantityType ())
			{
				if (q.Key == this.QuantityType)
				{
					type = q.Values[0];
					break;
				}
			}

			string unit = Misc.FormatUnit (this.Quantity, this.Unit.Code);

			return FormattedText.Concat (type, " ", unit);
		}
	}
}
