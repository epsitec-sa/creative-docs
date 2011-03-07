//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class DocumentCategoryMappingEntity
	{
		public override FormattedText GetSummary()
		{
			return this.GetPrintableEntityName ();
		}

		public override FormattedText GetCompactSummary()
		{
			return this.GetPrintableEntityName ();
		}

		public FormattedText GetPrintableEntityName()
		{
			// TODO: DR
			return "coucou";
#if false
			var printableEntities = Business.Enumerations.GetAllPrintableEntities ();

			var t = printableEntities.Where (x => x.Key == this.PrintableEntity).FirstOrDefault ();
			if (t == null)
			{
				return TextFormatter.FormatText ("(inconnu)").ApplyItalic ();
			}
			else
			{
				return t.Values[0];
			}
#endif
		}

		public override EntityStatus GetEntityStatus()
		{
			using (var a = new EntityStatusAccumulator ())
			{
				a.Accumulate (this.Name.GetEntityStatus ());
				a.Accumulate (this.Description.GetEntityStatus ());

				return a.EntityStatus;
			}
		}
	}
}
