//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

namespace Epsitec.Cresus.Compta.Options.Data
{
	public enum ComparisonShowed
	{
		None               = 0,

		Budget             = 0x0001,
		BudgetProrata      = 0x0002,
		BudgetFutur        = 0x0010,
		BudgetFuturProrata = 0x0020,
		PériodePrécédente  = 0x0100,
		PériodePénultième  = 0x0200,

		All                = 0x0333,
	}
}
