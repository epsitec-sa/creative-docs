//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class PaymentReminderDefinitionEntity
	{
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText (this.Code, this.Name);
		}
	}
}
