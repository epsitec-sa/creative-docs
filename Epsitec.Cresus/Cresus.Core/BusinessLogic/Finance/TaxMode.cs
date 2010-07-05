//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.BusinessLogic.Finance
{
	[DesignerVisible]
	public enum TaxMode
	{
		None			= 0,

		LiableForVat	= 1,										//	assujetti à la TVA
		NotLiableForVat	= 2,										//	non assujetti à la TVA

		ExemptFromVat	= 3,										//	exonéré
	}
}
