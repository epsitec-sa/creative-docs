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

namespace Epsitec.Cresus.Core.Print
{
	public static class PrinterApplicationSettings
	{
		public static List<PrintingUnit> GetPrintingUnitList()
		{
			var list = new List<PrintingUnit> ();

			throw new System.NotImplementedException ();
			Dictionary<string, string> settings = null;
//-			settings = CoreApplication.ExtractSettings ("PrintingUnit");

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

		public static void SetPrintingUnitList(List<PrintingUnit> list)
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

			throw new System.NotImplementedException ();
//-			CoreApplication.MergeSettings ("PrintingUnit", settings);
		}

		private static string GetKey(int index)
		{
			return string.Concat ("PrintingUnit", (index++).ToString (CultureInfo.InvariantCulture));
		}
	}
}
