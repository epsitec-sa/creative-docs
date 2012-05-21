//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta
{
	public enum SettingsEnum
	{
		Unknown,

		YearDigits2,
		YearDigits4,

		YearDMY,
		YearYMD,

		SeparatorNone,
		SeparatorSpace,
		SeparatorDot,
		SeparatorComma,
		SeparatorSlash,
		SeparatorDash,
		SeparatorApostrophe,

		NegativeMinus,
		NegativeParentheses,

		NullPartsZeroZero,
		NullPartsDashZero,
		NullPartsZeroDash,
		NullPartsDashDash,

		PercentFloating,
		PercentFrac1,
		PercentFrac2,
		PercentFrac3,
	}
}
