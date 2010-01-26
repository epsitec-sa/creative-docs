//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.App.BanquePiguet
{

	class Printer
	{

		public Printer()
		{
		}

		public Printer(string name, double xOffset, double yOffset)
		{
			this.Name = name;
			this.XOffset = xOffset;
			this.YOffset = yOffset;
		}

		public string Name
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

		public override string ToString()
		{
			return String.Format ("{0} ({1}, {2})", this.Name, this.XOffset, this.YOffset);
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
						new XElement ("xOffset", printer.XOffset),
						new XElement ("yOffset", printer.YOffset)
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
					Name = (string) xPrinter.Element ("name"),
					XOffset = (double) xPrinter.Element ("xOffset"),
					YOffset = (double) xPrinter.Element ("yOffset"),
				}
			);
		}

	}

}
