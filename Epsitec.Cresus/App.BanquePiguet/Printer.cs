//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.App.BanquePiguet
{

	class Printer
	{

		public Printer()
		{
		}

		public Printer(string name, string tray, double xOffset, double yOffset)
		{
			this.Name = name;
			this.Tray = tray;
			this.XOffset = xOffset;
			this.YOffset = yOffset;
		}

		public string Name
		{
			get;
			set;
		}

		public string Tray
		{
			get;
			set;
		}

		public double XOffset
		{
			get;
			set;
		}

		public double YOffset
		{
			get;
			set;
		}

		public static void Save(List<Printer> printers)
		{
			XElement xPrinters = new XElement ("printers");

			printers.ForEach (
				printer => xPrinters.Add
				(
					new XElement
					(
						"printer",
						new XElement ("name", printer.Name),
						new XElement ("tray", printer.Tray),
						new XElement ("xOffset", printer.XOffset.ToString(CultureInfo.InvariantCulture)),
						new XElement ("yOffset", printer.YOffset.ToString(CultureInfo.InvariantCulture))
					)
				)
			);

			xPrinters.Save (App.BanquePiguet.Properties.Resources.PrintersFile);
		}

		public static List<Printer> Load()
		{
			XElement xPrinters = XElement.Load (App.BanquePiguet.Properties.Resources.PrintersFile);

			return new List<Printer> (
				from xPrinter in xPrinters.Elements ("printer")
				select new Printer ()
				{
					Name = xPrinter.Element ("name").Value,
					Tray = xPrinter.Element ("tray").Value,
					XOffset = Double.Parse (xPrinter.Element ("xOffset").Value, CultureInfo.InvariantCulture),
					YOffset = Double.Parse (xPrinter.Element ("yOffset").Value, CultureInfo.InvariantCulture),
				}
			);
		}

	}

}
