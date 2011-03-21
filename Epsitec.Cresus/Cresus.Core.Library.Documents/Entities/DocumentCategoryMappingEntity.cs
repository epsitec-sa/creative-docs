//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

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
			Druid id;

			if (Druid.TryParse (this.PrintableEntity, out id))
			{
				//	Found entity ID.
				var caption = EntityInfo.GetStructuredType (id).Caption;
				var text    = caption.DefaultLabel ?? caption.Name;

				return new FormattedText (text);
			}
			else
			{
				return TextFormatter.FormatText ("(inconnu)").ApplyItalic ();
			}
		}
	}
}
