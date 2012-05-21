//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Finance
{
	[DesignerVisible]
	public enum CurrencyCode
	{
		None = 0,

		//	http://www.iso.org/iso/support/faqs/faqs_widely_used_standards/widely_used_standards_other/currency_codes/currency_codes_list-1.htm

		Chf = 756,	//	Swiss Franc
		Eur = 978,	//	Euro
		Usd = 840,	//	US Dollar
		Gbp = 826,	//	Pound Sterling
		Jpy = 392,	//	Yen
		Cny = 156,	//	Yuan Renminbi 
		Aud = 036,	//	Australian Doller
	}
}
