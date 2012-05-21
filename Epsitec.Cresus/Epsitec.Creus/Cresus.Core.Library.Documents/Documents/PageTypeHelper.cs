//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Cresus.Core.Documents
{
	public static class PageTypeHelper
	{
		public static bool IsPrinterAndPageMatching(PageType printerUnitFunction, PageType pageType)
		{
			//	Retourne true si la fonction d'une unité d'impression est compatible avec le type d'une page.
			//	Si oui, bingo, on peut imprimer !
			if (printerUnitFunction == PageType.All  ||
				printerUnitFunction == PageType.Copy)  // unité d'impression pour tous les types de page ?
			{
				return true;
			}

			if (printerUnitFunction == PageType.Single    && pageType == PageType.Single    ||
				printerUnitFunction == PageType.First     && pageType == PageType.First     ||
				printerUnitFunction == PageType.Following && pageType == PageType.Following ||
				printerUnitFunction == PageType.Isr       && pageType == PageType.Isr       ||
				printerUnitFunction == PageType.Label     && pageType == PageType.Label)
			{
				return true;
			}

			return false;
		}
	}
}
