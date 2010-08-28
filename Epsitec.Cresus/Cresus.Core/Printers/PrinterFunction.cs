//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Printers
{
	public enum PrinterFunction
	{
		ForAllPages,			// imprimante pour toutes les pages
		ForPagesCopy,			// imprimante pour une copie de toutes les pages

		ForFirstPage,			// imprimante pour la première page (0)
		ForFollowingPages,		// imprimante pour les pages suivantes (1..n)
		
		ForEsrPage,				// imprimante pour BV
	}
}
