//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class UnitOfMeasureEntity
	{
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText
				(
					"Nom: ", this.Name, "\n",
					"Catégorie: ", UnitOfMeasureEntity.GetCategory (this), "\n",
					"Valeurs: ", UnitOfMeasureEntity.GetFactors (this)
				);
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Name);
		}

		public override string[] GetEntityKeywords()
		{
			return new string[] { this.Name.ToSimpleText (), this.Code };
		}

		public override EntityStatus GetEntityStatus()
		{
			using (var a = new EntityStatusAccumulator ())
			{
				a.Accumulate (this.Code.GetEntityStatus ());
				a.Accumulate (this.Name.GetEntityStatus ());

				return a.EntityStatus;
			}
		}


		private static string GetCategory(Entities.UnitOfMeasureEntity unit)
		{
			foreach (var item in EnumKeyValues.FromEnum<UnitOfMeasureCategory> ())
			{
				if (item.Key == unit.Category)
				{
					return TextFormatter.FormatText (item.Values).ToSimpleText ();
				}
			}

			return null;
		}

		private static string GetFactors(Entities.UnitOfMeasureEntity unit)
		{
			return string.Format ("÷{0}, ×{1}, ±{2}", unit.DivideRatio.ToString (), unit.MultiplyRatio.ToString (), unit.SmallestIncrement.ToString ());
		}
	}
}
