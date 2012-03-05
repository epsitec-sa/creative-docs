//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus;
using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Entities
{
	public partial class ComptaCodeTVAEntity
	{
		public FormattedText MenuDescription
		{
			get
			{
				if (this.Taux2.HasValue && this.Taux1 != this.Taux2.Value)
				{
					return FormattedText.Concat (this.Code.ApplyBold (), "#", Converters.PercentToString (this.Taux1), " ", Converters.PercentToString (this.Taux2));
				}
				else
				{
					return FormattedText.Concat (this.Code.ApplyBold (), "#", Converters.PercentToString (this.Taux1));
				}
			}
		}
	}
}
