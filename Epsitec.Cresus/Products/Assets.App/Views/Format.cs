//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public static class Format
	{
		public static DecimalFormat GetFieldFormat(ObjectField objectField)
		{
			switch (objectField)
			{
				case ObjectField.TauxAmortissement:
					return DecimalFormat.Rate;

				case ObjectField.ValeurRésiduelle:
					return DecimalFormat.Amount;

				default:
					return DecimalFormat.Unknown;
			}
		}

	}
}
