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

namespace Epsitec.Cresus.Core.Print2
{
	public static class PrinterApplicationSettings
	{
		public static List<PrintingUnit> GetPrintingUnitList()
		{
			var list = new List<PrintingUnit> ();

			//?Dictionary<string, string> settings = CoreApplication.ExtractSettings ("PrintingUnit");
			Dictionary<string, string> settings = CoreApplication.ExtractSettings ("PrinterUnit");

			for (int i = 0; i < settings.Count; i++)
			{
				string key = PrinterApplicationSettings.GetKey (i);
				string setting = settings[key];

				var printingUnit = new PrintingUnit ();
				printingUnit.SetSerializableContent (setting);

				list.Add (printingUnit);
			}

			return list;
		}

		public static void SetPrinterList(List<PrintingUnit> list)
		{
			var settings = new Dictionary<string, string> ();
			int index = 0;

			foreach (var printingUnit in list)
			{
				if (!string.IsNullOrWhiteSpace (printingUnit.LogicalName) &&
					!string.IsNullOrWhiteSpace (printingUnit.PhysicalPrinterName))
				{
					string key = PrinterApplicationSettings.GetKey (index++);
					settings.Add (key, printingUnit.GetSerializableContent ());
				}
			}

			//?CoreApplication.MergeSettings ("PrintingUnit", settings);
			CoreApplication.MergeSettings ("PrinterUnit", settings);
		}

		private static string GetKey(int index)
		{
			//?return string.Concat ("PrintingUnit", (index++).ToString (CultureInfo.InvariantCulture));
			return string.Concat ("PrinterUnit", (index++).ToString (CultureInfo.InvariantCulture));
		}
	}
}
