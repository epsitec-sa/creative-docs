//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public static class Format
	{
		public static DecimalFormat GetFieldFormat(ObjectField objectField)
		{
			switch (objectField)
			{
				case ObjectField.AmortizationRate:
					return DecimalFormat.Rate;

				case ObjectField.Round:
				case ObjectField.ResidualValue:
					return DecimalFormat.Amount;

				default:
					return DecimalFormat.Unknown;
			}
		}
	}
}
