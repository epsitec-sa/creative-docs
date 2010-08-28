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
		public static bool IsPrinterAndPageMatching(Printers.PrinterFunction printerFunction, Printers.PageType pageType)
		{
			//	Retourne true si la fonction d'une imprimante est compatible avec le type d'une page.
			//	Si oui, bingo, on peut imprimer !
			if (printerFunction == Printers.PrinterFunction.ForAllPages ||
				printerFunction == Printers.PrinterFunction.ForPagesCopy)  // imprimente pour tous les types de page ?
			{
				return true;
			}

			if ((printerFunction == Printers.PrinterFunction.ForFirstPage      && pageType == Printers.PageType.First    ) ||
				(printerFunction == Printers.PrinterFunction.ForFollowingPages && pageType == Printers.PageType.Following) ||
				(printerFunction == Printers.PrinterFunction.ForEsrPage        && pageType == Printers.PageType.ESR      ))
			{
				return true;
			}

			return false;
		}
	}
}
