//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Business;

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
			var printableEntities = EnumKeyValues.FromEnum<PrintableEntities> ();

			var t = printableEntities.Where (x => x.Key == this.PrintableEntity).FirstOrDefault ();
			if (t == null)
			{
				return TextFormatter.FormatText ("(inconnu)").ApplyItalic ();
			}
			else
			{
				return t.Values[0];
			}
		}
	}
}
