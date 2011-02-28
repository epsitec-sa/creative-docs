//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Helpers;

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

		public bool IsEmptyQuantity()
		{
			if (this.Quantity == 0)
			{
				return this.Unit.IsNull ()
					|| this.Unit.GetEntityStatus () == EntityStatus.Empty;
			}

			return false;
		}

		public override FormattedText GetCompactSummary()
		{
			FormattedText type = FormattedText.Empty;

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

		public override EntityStatus GetEntityStatus()
		{
			return EntityStatus.Valid;
		}
	}
}
