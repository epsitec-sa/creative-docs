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
				case ObjectField.GroupRatio+0:
				case ObjectField.GroupRatio+1:
				case ObjectField.GroupRatio+2:
				case ObjectField.GroupRatio+3:
				case ObjectField.GroupRatio+4:
				case ObjectField.GroupRatio+5:
				case ObjectField.GroupRatio+6:
				case ObjectField.GroupRatio+7:
				case ObjectField.GroupRatio+8:
				case ObjectField.GroupRatio+9:
					return DecimalFormat.Rate;

				case ObjectField.ValeurRésiduelle:
					return DecimalFormat.Amount;

				default:
					return DecimalFormat.Unknown;
			}
		}
	}
}
