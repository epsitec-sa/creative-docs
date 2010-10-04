//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class ArticleGroupEntity
	{
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText (this.Code, "~:", this.Name);
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Code, "~:", this.Name);
		}

		public override EntityStatus EntityStatus
		{
			get
			{
				var s1 = EntityStatusHelper.GetStatus (this.Code);
				var s2 = EntityStatusHelper.GetStatus (this.Name);

				return EntityStatusHelper.CombineStatus (StatusHelperCardinality.All, s1, s2);
			}
		}
	}
}
