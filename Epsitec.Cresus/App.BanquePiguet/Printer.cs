//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Support;

using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Epsitec.App.BanquePiguet
{

	/// <summary>
	/// The Printer class represents a printer and its properties. Moreover, it contais two static methods
	/// used to load and save a list of Printers to a file.
	/// </summary>
	class Printer
	{

		/// <summary>
		/// Initializes a new empty instance of the Printer class.
		/// </summary>
		public Printer()
		{
		}

		/// <summary>
		/// Initializes a new instance of the Printer class.
		/// </summary>
		/// <param name="name">The name of the printer.</param>
		/// <param name="tray">The selected tray.</param>
		/// <param name="xOffset">The offset on the x axis.</param>
		/// <param name="yOffset">The offset on the y axis.</param>
		/// <param name="comment">The comment on the printer.</param>
		public Printer(string name, string tray, double xOffset, double yOffset, string comment)
		{
			this.Name = name;
			this.Tray = tray;
			this.XOffset = xOffset;
			this.YOffset = yOffset;
			this.Comment = comment;
		}

		/// <summary>
		/// Gets or sets the name of the printer.
		/// </summary>
		/// <value>The name of the printer.</value>
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the tray selected to print the bvs.
		/// </summary>
		/// <value>The tray selected to print the bvs.</value>
		public string Tray
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the offset on the x axis to print the bvs.
		/// </summary>
		/// <value>The offset on the x axis to print the bvs.</value>
		public double XOffset
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the offset on the y axis to print the bvs.
		/// </summary>
		/// <value>The offset on the y axis to print the bvs.</value>
		public double YOffset
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the comment on the printer.
		/// </summary>
		/// <value>The comment on the printer.</value>
		public string Comment
		{
			get;
			set;
		}

		/// <summary>
		/// Saves the list of Printers to the configuration file. The file is overwritten, therefore
		/// all printers which might be in the file but not in printers will be erased.
		/// </summary>
		/// <param name="printers">The list of Printers to save.</param>
		public static void Save(IEnumerable<Printer> printers)
		{
			XElement xPrinters = new XElement ("printers");

			foreach (Printer printer in printers)
			{
				xPrinters.Add (
					new XElement ("printer",
						new XElement ("name", printer.Name),
						new XElement ("tray", printer.Tray),
						new XElement ("xOffset", printer.XOffset.ToString (CultureInfo.InvariantCulture)),
						new XElement ("yOffset", printer.YOffset.ToString (CultureInfo.InvariantCulture)),
						new XElement ("comment", printer.Comment)
					)
				);
			}

			XmlWriterSettings xmlSettings = new XmlWriterSettings ()
			{
				Indent = true,
				NewLineOnAttributes = true,
			};

			using (StreamWriter streamWriter = new StreamWriter (Printer.configurationFile))
			{
				using (XmlWriter xmlWriter = XmlWriter.Create (streamWriter, xmlSettings))
				{
					xPrinters.Save (xmlWriter);
				}
			}
		}

		/// <summary>
		/// Loads a list of Printers from the configuration File.
		/// </summary>
		/// <returns>The list of Printers loaded from the configuration file.</returns>
		public static IEnumerable<Printer> Load()
		{
			XElement xPrinters;

			using (StreamReader streamReader = new StreamReader (Printer.configurationFile))
			{
				using (XmlReader xmlReader = XmlReader.Create (streamReader))
				{
					xPrinters = XElement.Load (xmlReader);
				}
			}

			return from xPrinter in xPrinters.Elements ("printer")
				   select new Printer ()
				   {
					   Name = xPrinter.Element ("name").Value,
					   Tray = xPrinter.Element ("tray").Value,
					   XOffset = double.Parse (xPrinter.Element ("xOffset").Value, CultureInfo.InvariantCulture),
					   YOffset = double.Parse (xPrinter.Element ("yOffset").Value, CultureInfo.InvariantCulture),
					   Comment = xPrinter.Element ("comment").Value,
				   };
		}

		/// <summary>
		/// The path to the configuration file containing the Printer definitions.
		/// </summary>
		protected static readonly string configurationFile = string.Format (@"{0}\Printers\printers.xml", Globals.Directories.ExecutableRoot);

	}

}
