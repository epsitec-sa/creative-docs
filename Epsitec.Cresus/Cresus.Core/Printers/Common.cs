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

			if ((printerUnitFunction == Printers.PrinterUnitFunction.ForSinglePage     && pageType == Printers.PageType.Single   ) ||
				(printerUnitFunction == Printers.PrinterUnitFunction.ForFirstPage      && pageType == Printers.PageType.First    ) ||
				(printerUnitFunction == Printers.PrinterUnitFunction.ForFollowingPages && pageType == Printers.PageType.Following) ||
				(printerUnitFunction == Printers.PrinterUnitFunction.ForEsrPage        && pageType == Printers.PageType.ESR      ) ||
				(printerUnitFunction == Printers.PrinterUnitFunction.ForLabelPage      && pageType == Printers.PageType.Label    ))
			{
				return true;
			}

			return false;
		}


		public enum PageSizeCompareEnum
		{
			Different,
			Equal,
			Swaped,		// égal, mais Width et Height sont permutés (rotation de 90 degrés)
		}

		public static PageSizeCompareEnum PageSizeCompare(Size s1, Size s2, double delta=1)
		{
			//	Compare deux tailles de page, avec une tolérence de +/- delta (1mm par défaut).
			if (System.Math.Abs (s1.Width  - s2.Width ) < delta &&
				System.Math.Abs (s1.Height - s2.Height) < delta)
			{
				return PageSizeCompareEnum.Equal;
			}

			if (System.Math.Abs (s1.Width  - s2.Height) < delta &&
				System.Math.Abs (s1.Height - s2.Width ) < delta )
			{
				return PageSizeCompareEnum.Swaped;
			}

			return PageSizeCompareEnum.Different;
		}

		public static bool InsidePageSize(Size pageSize, Size minimalPageSize, Size maximalPageSize)
		{
			//	Compare si une taille de page est comprise entre deux tailles min/max, en acceptant
			//	les formats portrait et paysage.
			double px = pageSize.Width;
			double py = pageSize.Height;

			double minx = minimalPageSize.Width;
			double miny = minimalPageSize.Height;

			double maxx = maximalPageSize.Width;
			double maxy = maximalPageSize.Height;

			return ((px >= minx && py >= miny) || (px >= miny && py >= minx)) &&
				   ((px <= maxx && py <= maxy) || (px <= maxy && py <= maxx));
		}


		public static IEnumerable<DeserializedPage> GetDeserializedPages(List<DeserializedJob> jobs)
		{
			foreach (var job in jobs)
			{
				foreach (var section in job.Sections)
				{
					foreach (var page in section.Pages)
					{
						yield return page;
					}
				}
			}
		}

		public static PrinterUnit GetPrinterUnit(string logicalPrinterName)
		{
			//	Cherche une unité d'impression d'après son nom.
			List<PrinterUnit> printerUnitList = PrinterApplicationSettings.GetPrinterUnitList ();
			return printerUnitList.Where (p => p.LogicalName == logicalPrinterName).FirstOrDefault ();
		}
	}
}
