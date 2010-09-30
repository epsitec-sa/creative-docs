//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class UnitOfMeasureGroupEntity
	{
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText
				(
					"Nom: ", this.Name, "\n",
					"Description: ", this.Description, "\n",
					"Catégorie: ", UnitOfMeasureGroupEntity.GetCategory (this)
				);
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Name);
		}

		public override string[] GetTextArray()
		{
			return new string[] { this.Name.ToSimpleText () };
		}


		private static string GetCategory(Entities.UnitOfMeasureGroupEntity unit)
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
	}
}
