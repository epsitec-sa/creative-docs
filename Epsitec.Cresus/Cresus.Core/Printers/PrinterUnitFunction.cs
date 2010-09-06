//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Printers
{
	public enum PrinterUnitFunction
	{
		ForAllPages,			// unité d'impression pour toutes les pages
		ForPagesCopy,			// unité d'impression pour une copie de toutes les pages

		ForFirstPage,			// unité d'impression pour la première page (0)
		ForFollowingPages,		// unité d'impression pour les pages suivantes (1..n)

		ForEsrPage,				// unité d'impression pour BV
	}
}
