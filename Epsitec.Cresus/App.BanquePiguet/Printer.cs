//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Support;

using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Linq;
using System.Xml;
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
				printer => xPrinters.Add (
					new XElement (
						"printer",
						new XElement ("name", printer.Name),
						new XElement ("tray", printer.Tray),
						new XElement ("xOffset", printer.XOffset.ToString (CultureInfo.InvariantCulture)),
						new XElement ("yOffset", printer.YOffset.ToString (CultureInfo.InvariantCulture))
					)
				)
			);

			XmlWriterSettings xmlSettings = new XmlWriterSettings ()
			{
				Indent = true,
				NewLineOnAttributes = true,
			};

			using (XmlWriter xmlWriter = XmlWriter.Create (new StreamWriter (Printer.configurationFile), xmlSettings))
			{
					xPrinters.Save (xmlWriter);
			}

			
		}

		public static List<Printer> Load()
		{
			XElement xPrinters;

			using (XmlReader xmlReader = XmlReader.Create(new StreamReader(Printer.configurationFile)))
			{
				xPrinters = XElement.Load (xmlReader);
			}

			List<Printer> printers = new List<Printer> (
				from xPrinter in xPrinters.Elements ("printer")
				select new Printer ()
				{
					Name = xPrinter.Element ("name").Value,
					Tray = xPrinter.Element ("tray").Value,
					XOffset = Double.Parse (xPrinter.Element ("xOffset").Value, CultureInfo.InvariantCulture),
					YOffset = Double.Parse (xPrinter.Element ("yOffset").Value, CultureInfo.InvariantCulture),
				}
			);

			return printers;
		}

		protected static string configurationFile = String.Format (@"{0}\Printers\printers.xml", Globals.Directories.ExecutableRoot);

	}

}
