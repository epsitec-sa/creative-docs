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
			return TextFormatter.FormatText (this.PrintableEntityId);
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.PrintableEntityId);
		}

		public Druid PrintableEntityId
		{
			get
			{
				Druid id;
				Druid.TryParse (this.PrintableEntity, out id);
				return id;
			}
		}
	}
}
