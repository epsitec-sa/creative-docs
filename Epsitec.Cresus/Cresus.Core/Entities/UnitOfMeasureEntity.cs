//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Helpers;
using Epsitec.Cresus.Core.Business;

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
					"Code: ", this.Code, "\n",
					"Catégorie: ", UnitOfMeasureEntity.GetCategory (this), "\n",
					"Valeurs: ", UnitOfMeasureEntity.GetFactors (this)
				);
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Name, "(", this.Code, ")");
		}

		public override string[] GetEntityKeywords()
		{
			return new string[] { this.Name.ToSimpleText (), this.Code };
		}

		public override EntityStatus GetEntityStatus()
		{
			var s1 = this.Code.GetEntityStatus ();
			var s2 = this.Name.GetEntityStatus ();

			return EntityStatusHelper.CombineStatus (StatusHelperCardinality.All, s1, s2);
		}


		private static string GetCategory(Entities.UnitOfMeasureEntity unit)
		{
			foreach (var item in Enumerations.GetAllPossibleUnitOfMeasureCategories ())
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
