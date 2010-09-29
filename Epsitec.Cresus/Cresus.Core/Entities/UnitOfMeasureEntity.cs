//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class UnitOfMeasureEntity
	{
		public FormattedText GetSummary()
		{
			return TextFormatter.FormatText
				(
					"Nom: ", this.Name, "\n",
					"Code: ", this.Code, "\n",
					"Catégorie: ", UnitOfMeasureEntity.GetCategory (this), "\n",
					"Valeurs: ", UnitOfMeasureEntity.GetFactors (this)
				);
		}

		public FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Name, "(", this.Code, ")");
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
