//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Printers
{
	public static class Common
	{
		public static bool IsPrinterAndPageMatching(Printers.PrinterUnitFunction printerUnitFunction, Printers.PageType pageType)
		{
			//	Retourne true si la fonction d'une unité d'impression est compatible avec le type d'une page.
			//	Si oui, bingo, on peut imprimer !
			if (printerUnitFunction == Printers.PrinterUnitFunction.ForAllPages ||
				printerUnitFunction == Printers.PrinterUnitFunction.ForPagesCopy)  // unité d'impression pour tous les types de page ?
			{
				return true;
			}

			if ((printerUnitFunction == Printers.PrinterUnitFunction.ForFirstPage      && pageType == Printers.PageType.First    ) ||
				(printerUnitFunction == Printers.PrinterUnitFunction.ForFollowingPages && pageType == Printers.PageType.Following) ||
				(printerUnitFunction == Printers.PrinterUnitFunction.ForEsrPage        && pageType == Printers.PageType.ESR      ))
			{
				return true;
			}

			return false;
		}
	}
}
