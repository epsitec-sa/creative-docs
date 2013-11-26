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
				case ObjectField.TauxAmortissement:
				case ObjectField.GroupRate+0:
				case ObjectField.GroupRate+1:
				case ObjectField.GroupRate+2:
				case ObjectField.GroupRate+3:
				case ObjectField.GroupRate+4:
				case ObjectField.GroupRate+5:
				case ObjectField.GroupRate+6:
				case ObjectField.GroupRate+7:
				case ObjectField.GroupRate+8:
				case ObjectField.GroupRate+9:
					return DecimalFormat.Rate;

				case ObjectField.ValeurRésiduelle:
					return DecimalFormat.Amount;

				default:
					return DecimalFormat.Unknown;
			}
		}
	}
}
