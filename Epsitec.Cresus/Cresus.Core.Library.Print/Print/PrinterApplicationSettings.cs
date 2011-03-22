//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Documents;
using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Print
{
	public static class PrinterApplicationSettings
	{
		public static List<PrintingUnit> GetPrintingUnitList(CoreApp app)
		{
			var list = new List<PrintingUnit> ();

			var settings = app.SettingsManager.ExtractSettings (PrinterApplicationSettings.PrintingUnit);

			for (int i = 0; i < settings.Count; i++)
			{
				string key = PrinterApplicationSettings.GetKey (i);
				string value = settings[key];

				var printingUnit = new PrintingUnit ();
				
				printingUnit.SetSerializableContent (value);

				list.Add (printingUnit);
			}

			return list;
		}

		public static void SetPrintingUnitList(CoreApp app, IEnumerable<PrintingUnit> list)
		{
			var settings = new SettingsCollection ();
			int index = 0;

			foreach (var printingUnit in list)
			{
				if (!string.IsNullOrWhiteSpace (printingUnit.DocumentPrintingUnitCode) &&
					!string.IsNullOrWhiteSpace (printingUnit.PhysicalPrinterName))
				{
					string key = PrinterApplicationSettings.GetKey (index++);
					settings[key] = printingUnit.GetSerializableContent ();
				}
			}

			app.SettingsManager.MergeSettings (PrinterApplicationSettings.PrintingUnit, settings);
		}

		private static string GetKey(int index)
		{
			return string.Concat (PrinterApplicationSettings.PrintingUnit, InvariantConverter.ToString (index));
		}
		
		private const string PrintingUnit = "PrintingUnit";
	}
}
