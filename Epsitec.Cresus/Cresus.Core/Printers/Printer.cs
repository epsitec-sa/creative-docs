//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Support;

using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core.Printers
{

	/// <summary>
	/// The <c>Printer</c> class represents a printer and its properties. Moreover, it contais
	/// two static methods used to load and save a list of <see cref="Printer"/>s to a file.
	/// </summary>
	class Printer
	{

		/// <summary>
		/// Initializes a new empty instance of the <see cref="Printer"/> class.
		/// </summary>
		public Printer()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Printer"/> class.
		/// </summary>
		/// <param name="logicalName">The logical name of the printer.</param>
		/// <param name="physicalName">The name of the printer.</param>
		/// <param name="tray">The selected tray.</param>
		/// <param name="horizontal">True for horizontal orientation, false for vertical orientation.</param>
		/// <param name="xOffset">The offset on the x axis.</param>
		/// <param name="yOffset">The offset on the y axis.</param>
		/// <param name="comment">The comment on the printer.</param>
		public Printer(string logicalName, string physicalName, string tray, bool horizontal, double xOffset, double yOffset, string comment)
		{
			this.LogicalName = logicalName;
			this.PhysicalName = physicalName;
			this.Tray = tray;
			this.Horizontal = horizontal;
			this.XOffset = xOffset;
			this.YOffset = yOffset;
			this.Comment = comment;
		}

		/// <summary>
		/// Gets or sets the logical name of the printer.
		/// </summary>
		/// <value>The logical name of the printer.</value>
		public string LogicalName
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the name of the <see cref="Printer"/>.
		/// </summary>
		/// <value>The name of the <see cref="Printer"/>.</value>
		public string PhysicalName
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
		/// Gets or sets a value indicating whether the bv should be printed horizontaly or
		/// vertically.
		/// </summary>
		/// <value>True for an horiztontal orientation, false for a vertical orientation.</value>
		public bool Horizontal
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
		/// Gets or sets the comment on the <see cref="Printer"/>.
		/// </summary>
		/// <value>The comment on the <see cref="Printer"/>.</value>
		public string Comment
		{
			get;
			set;
		}


		public string GetNiceDescription()
		{
			return TextFormatter.FormatText (this.LogicalName, "(", this.PhysicalName, ",~", this.Tray, ")").ToString ();
		}


		public string GetSerializableContent()
		{
			return string.Concat
				(
					"LogicalName=",
					this.LogicalName,

					Printer.serializableSeparator,
					
					"PhysicalName=",
					this.PhysicalName,

					Printer.serializableSeparator,
					
					"Tray=",
					this.Tray,

					Printer.serializableSeparator,
					
					"XOffset=",
					this.XOffset.ToString (CultureInfo.InvariantCulture),

					Printer.serializableSeparator,
					
					"YOffset=",
					this.YOffset.ToString (CultureInfo.InvariantCulture),

					Printer.serializableSeparator,
					
					"Comment=",
					this.Comment
				);
		}

		public void SetSerializableContent(string content)
		{
			var list = content.Split (new string[] { Printer.serializableSeparator }, System.StringSplitOptions.RemoveEmptyEntries);

			foreach (var line in list)
			{
				var words = line.Split (new string[] { "=" }, System.StringSplitOptions.None);

				if (words.Length == 2)
				{
					switch (words[0])
					{
						case "LogicalName":
							this.LogicalName = words[1];
							break;

						case "PhysicalName":
							this.PhysicalName = words[1];
							break;

						case "Tray":
							this.Tray = words[1];
							break;

						case "XOffset":
							this.XOffset = double.Parse (words[1]);
							break;

						case "YOffset":
							this.YOffset = double.Parse (words[1]);
							break;

						case "Comment":
							this.Comment = words[1];
							break;
					}
				}
			}
		}



		/// <summary>
		/// Saves a list of <see cref="Printer"/>s to the configuration file. The file is overwritten,
		/// therefore all <see cref="Printer"/>s which might be in the file but not in printers will
		/// be erased.
		/// </summary>
		/// <param name="printers">The list of <see cref="Printer"/>s to save.</param>
		public static void Save(IEnumerable<Printer> printers)
		{
			XElement xPrinters = new XElement ("printers");

			foreach (Printer printer in printers)
			{
				xPrinters.Add (
					new XElement ("printer",
						new XElement ("name", printer.PhysicalName),
						new XElement ("tray", printer.Tray),
						new XElement ("horizontal", printer.Horizontal), 
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
		/// Loads a list of <see cref="Printer"/>s from the configuration file.
		/// </summary>
		/// <returns>The list of <see cref="Printer"/>s loaded from the configuration file.</returns>
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
					   PhysicalName = xPrinter.Element ("name").Value,
					   Tray = xPrinter.Element ("tray").Value,
					   Horizontal = bool.Parse(xPrinter.Element ("horizontal").Value),
					   XOffset = double.Parse (xPrinter.Element ("xOffset").Value, CultureInfo.InvariantCulture),
					   YOffset = double.Parse (xPrinter.Element ("yOffset").Value, CultureInfo.InvariantCulture),
					   Comment = xPrinter.Element ("comment").Value,
				   };
		}

		/// <summary>
		/// The path to the configuration file containing the <see cref="Printer"/>s definitions.
		/// </summary>
		protected static readonly string configurationFile = string.Format (@"{0}\Printers\printers.xml", Globals.Directories.ExecutableRoot);



		private static readonly string serializableSeparator = "•";  // puce, unicode 2022
	}

}
