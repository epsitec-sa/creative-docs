//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class IsrDefinitionEntity
	{
		public override FormattedText GetSummary()
		{
			var builder = new TextBuilder ();

			builder.Append (Isr.FormatSubscriberNumber (this.SubscriberNumber));

			return builder.ToFormattedText ();
		}
	}
}
