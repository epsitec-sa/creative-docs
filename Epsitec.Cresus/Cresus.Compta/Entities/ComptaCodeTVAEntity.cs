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
				return FormattedText.Concat (this.Code.ApplyBold (), "#", Converters.PercentToString (this.Taux.Last ().Taux));
			}
		}
	}
}
