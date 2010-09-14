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
	public static class PrinterApplicationSettings
	{
		public static List<PrinterUnit> GetPrinterUnitList()
		{
			List<PrinterUnit> list = new List<PrinterUnit> ();

			Dictionary<string, string> settings = CoreApplication.ExtractSettings ("PrinterUnit");

			for (int i = 0; i < settings.Count; i++)
			{
				string key = PrinterApplicationSettings.GetKey (i);
				string setting = settings[key];

				PrinterUnit printerUnit = new PrinterUnit ();
				printerUnit.SetSerializableContent (setting);

				list.Add (printerUnit);
			}

			return list;
		}

		public static void SetPrinterList(List<PrinterUnit> list)
		{
			var settings = new Dictionary<string, string> ();
			int index = 0;

			foreach (var printerUnit in list)
			{
				if (!string.IsNullOrWhiteSpace (printerUnit.LogicalName) &&
					!string.IsNullOrWhiteSpace (printerUnit.PhysicalPrinterName))
				{
					string key = PrinterApplicationSettings.GetKey (index++);
					settings.Add (key, printerUnit.GetSerializableContent ());
				}
			}

			CoreApplication.MergeSettings ("PrinterUnit", settings);
		}

		private static string GetKey(int index)
		{
			return string.Concat ("PrinterUnit", (index++).ToString (CultureInfo.InvariantCulture));
		}
	}
}
