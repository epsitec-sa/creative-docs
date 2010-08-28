//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Printers
{
	public enum PrinterTypeEnum
	{
		All,			// imprimante pour toutes les pages
		Copy,			// imprimante pour une copie de toutes les pages
		First,			// imprimante pour la page de garde (1)
		Following,		// imprimante pour les pages suivantes (2..n)
		ESR,			// imprimante pour BV
	}
}
