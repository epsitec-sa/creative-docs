//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Library;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class ArticleQuantityEntity : ICopyableEntity<ArticleQuantityEntity>
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

			foreach (var q in EnumKeyValues.FromEnum<ArticleQuantityType> ())
			{
				if (q.Key == this.QuantityColumn.QuantityType)
				{
					type = q.Values[0];
					break;
				}
			}

			return TextFormatter.FormatText (type, ArticleQuantityEntity.FormatQuantity (this.Quantity, this.Unit.Name));
		}

		public override EntityStatus GetEntityStatus()
		{
			return EntityStatus.Valid;
		}


		public static FormattedText FormatQuantity(decimal quantity, FormattedText unitName)
		{
			//	We have to work in one language, so first map the name to the current
			//	language :

			return TextFormatter.FormatText (quantity, ArticleQuantityEntity.GetUnitNameForQuantity (quantity, unitName));
		}

		public static FormattedText GetUnitNameForQuantity(decimal quantity, FormattedText unitName)
		{
			//	We have to work in one language, so first map the name to the current
			//	language :

			var unit = TextFormatter.ConvertToText (unitName);
			int pos  = unit.IndexOfAny (ArticleQuantityEntity.Separators);

			if (pos < 0)
			{
				return unit;
			}

			//	Plurals of nouns start when there are a least 2 items.
			//	Daniel's ref. : http://orthonet.sdv.fr/pages/informations_p11.html

			if (System.Math.Abs (quantity) < 2)
			{
				return unit.Substring (0, pos).Trim ();
			}
			else
			{
				return unit.Substring (pos+1).Trim ();
			}
		}



		#region ICloneable<ArticleQuantityEntity> Members

		void ICopyableEntity<ArticleQuantityEntity>.CopyTo(IBusinessContext businessContext, ArticleQuantityEntity copy)
		{
			copy.BeginDate          = this.BeginDate;
			copy.EndDate            = this.EndDate;
			copy.Quantity           = this.Quantity;
			copy.QuantityColumn     = this.QuantityColumn;
			copy.Unit               = this.Unit;
			copy.ExpectedDateFormat = this.ExpectedDateFormat;
			copy.ColumnName         = this.ColumnName;
		}

		#endregion


		public static readonly char[]			Separators = new char[] { '/', '|', ':' };
	}
}
