//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class BusinessSettingsEntity
	{
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText (this.Company.Person.GetSummary ());
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Company.Person.GetCompactSummary ());
		}

		public override EntityStatus EntityStatus
		{
			get
			{
				return EntityStatus.Valid;
			}
		}
	}
}
