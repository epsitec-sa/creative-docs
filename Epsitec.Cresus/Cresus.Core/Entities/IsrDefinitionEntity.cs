//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class IsrDefinitionEntity
	{
		public FormattedText GetSummary()
		{
			var builder = new System.Text.StringBuilder ();

			builder.Append (this.SubscriberNumber);

			return TextFormatter.FormatText (builder.ToString ());
		}
	}
}
