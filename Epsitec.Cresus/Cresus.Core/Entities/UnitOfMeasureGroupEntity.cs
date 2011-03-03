//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Helpers;
using Epsitec.Cresus.Core.Business;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Library;

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

		public override string[] GetEntityKeywords()
		{
			return new string[] { this.Name.ToSimpleText () };
		}

		public override EntityStatus GetEntityStatus()
		{
			using (var a = new EntityStatusAccumulator ())
			{
				a.Accumulate (this.Name.GetEntityStatus ());
				a.Accumulate (this.Description.GetEntityStatus ().TreatAsOptional ());

				return a.EntityStatus;
			}
		}


		private static string GetCategory(Entities.UnitOfMeasureGroupEntity unit)
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
	}
}
