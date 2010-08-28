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
	public static class PrinterSettings
	{
		public static List<Printer> GetPrinterList()
		{
			List<Printer> list = new List<Printer> ();

			Dictionary<string, string> settings = CoreApplication.ExtractSettings ("Printer");

			for (int i = 0; i < settings.Count; i++)
			{
				string key = PrinterSettings.GetKey (i);
				string setting = settings[key];

				Printer printer = new Printer ();
				printer.SetSerializableContent (setting);

				list.Add (printer);
			}

			return list;
		}

		public static void SetPrinterList(List<Printer> list)
		{
			var settings = new Dictionary<string, string> ();
			int index = 0;

			foreach (var printer in list)
			{
				if (!string.IsNullOrWhiteSpace (printer.LogicalName) &&
					!string.IsNullOrWhiteSpace (printer.PhysicalName))
				{
					string key = PrinterSettings.GetKey (index++);
					settings.Add (key, printer.GetSerializableContent ());
				}
			}

			CoreApplication.MergeSettings ("Printer", settings);
		}

		private static string GetKey(int index)
		{
			return string.Concat ("Printer", (index++).ToString (CultureInfo.InvariantCulture));
		}
	}
}
