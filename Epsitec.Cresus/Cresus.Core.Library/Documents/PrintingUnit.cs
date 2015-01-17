﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Printing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Documents
{
	/// <summary>
	/// Une unité d'impression est un couple imprimante/bac.
	/// </summary>
	public class PrintingUnit
	{
		public PrintingUnit()
		{
			this.Copies = 1;
			this.optionsDictionary = new PrintingOptionDictionary ();
		}


		public string							DocumentPrintingUnitCode
		{
			//	Champ Code de l'entité DocumentPrintingUnitEntity.
			get;
			set;
		}

		public string							PhysicalPrinterName
		{
			//	Nom physique de l'imprimante.
			//	Les caractères spéciaux sont encodés (par exemple, un "&" vaut "&amp;").
			//	Cette propriété est donc compatible avec Widget.Text.
			//	En revanche, il faut utiliser FormattedText.Unescape avant de passer la chaîne à Epsitec.Common.Printing.
			get;
			set;
		}

		public string							PhysicalPrinterTray
		{
			//	Nom physique du bac de l'impriante.
			//	Les caractères spéciaux sont encodés (par exemple, un "&" vaut "&amp;").
			//	Cette propriété est donc compatible avec Widget.Text.
			//	En revanche, il faut utiliser FormattedText.Unescape avant de passer la chaîne à Epsitec.Common.Printing.
			get;
			set;
		}

		public DuplexMode						PhysicalDuplexMode
		{
			get;
			set;
		}

		public Size								PhysicalPaperSize
		{
			get;
			set;
		}

		public double							XOffset
		{
			get;
			set;
		}

		public double							YOffset
		{
			get;
			set;
		}

		public int								Copies
		{
			get;
			set;
		}

		public PrintingOptionDictionary			Options
		{
			get
			{
				return this.optionsDictionary;
			}
		}


		public FormattedText GetNiceDescription(BusinessContext businessContext)
		{
			var example = new DocumentPrintingUnitsEntity ();
			example.Code = this.DocumentPrintingUnitCode;

			var documentPrintingUnits = businessContext.DataContext.GetByExample<DocumentPrintingUnitsEntity> (example).FirstOrDefault ();
			return PrintingUnit.GetNiceDescription (this, documentPrintingUnits);
		}

		private static FormattedText GetNiceDescription(PrintingUnit printingUnit, DocumentPrintingUnitsEntity documentPrintingUnits)
		{
			if (printingUnit == null && documentPrintingUnits == null)
			{
				return "null";
			}

			if (printingUnit == null)
			{
				return documentPrintingUnits.Name;
			}
			else if (documentPrintingUnits == null)
			{
				return TextFormatter.FormatText ("Error (Code=", printingUnit.DocumentPrintingUnitCode, ")");
			}
			else
			{
				if (printingUnit.Copies < 2)
				{
					//?return TextFormatter.FormatText (documentPrintingUnits.Name, "(", printingUnit.PhysicalPrinterName, ",~", printingUnit.PhysicalPrinterTray, ")");
					return documentPrintingUnits.Name;
				}
				else
				{
					string copies = string.Format ("{0}×", printingUnit.Copies);
					//?return TextFormatter.FormatText (documentPrintingUnits.Name, copies, "(", printingUnit.PhysicalPrinterName, ",~", printingUnit.PhysicalPrinterTray, ")");
					return TextFormatter.FormatText (documentPrintingUnits.Name, copies);
				}
			}
		}


		public string GetSerializableContent()
		{
			//	Retourne une string permettant de sérialiser l'ensemble de la classe.
			var list = new List<string> ()
			{
				"Code",                     this.DocumentPrintingUnitCode,
				"PhysicalName",             this.PhysicalPrinterName,
				"Tray",                     this.PhysicalPrinterTray,
				"XOffset",                  this.XOffset.ToString (System.Globalization.CultureInfo.InvariantCulture),
				"YOffset",                  this.YOffset.ToString (System.Globalization.CultureInfo.InvariantCulture),
				"Copies",                   this.Copies.ToString (System.Globalization.CultureInfo.InvariantCulture),
				"Options",				    this.optionsDictionary.GetSerializedData (),
				"PhysicalPaperSize.Width",  this.PhysicalPaperSize.Width.ToString (System.Globalization.CultureInfo.InvariantCulture),
				"PhysicalPaperSize.Height", this.PhysicalPaperSize.Height.ToString (System.Globalization.CultureInfo.InvariantCulture),
				"PhysicalDuplexMode",       PrintingUnit.DuplexToString (this.PhysicalDuplexMode),
			};

			return StringPacker.Pack (list);
		}

		public void SetSerializableContent(string data)
		{
			//	Initialise l'ensemble de la classe à partir d'une string sérialisée.
			var list = StringPacker.Unpack (data).ToList ();

			System.Diagnostics.Debug.Assert ((list.Count % 2) == 0);
			
			double paperSizeWidth  = 0;
			double paperSizeHeight = 0;

			this.optionsDictionary.Clear ();

			for (int i = 0; i+2 <= list.Count; i += 2)
			{
				string key   = list[i+0];
				string value = list[i+1];
					
				switch (key)
				{
					case "Code":	    				this.DocumentPrintingUnitCode = value;							break;
					case "PhysicalName":				this.PhysicalPrinterName = value;								break;
					case "Tray":						this.PhysicalPrinterTray = value;								break;
					case "XOffset":						this.XOffset = double.Parse (value);							break;
					case "YOffset":						this.YOffset = double.Parse (value);							break;
					case "Copies":						this.Copies = int.Parse (value);								break;
					case "Options":						this.optionsDictionary.SetSerializedData (value);				break;
					case "PhysicalPaperSize.Width":		paperSizeWidth = int.Parse (value);								break;
					case "PhysicalPaperSize.Height":	paperSizeHeight = int.Parse (value);							break;
					case "PhysicalDuplexMode":			this.PhysicalDuplexMode = PrintingUnit.StringToDuplex (value);	break;
				}
			}

			if (paperSizeWidth != 0 && paperSizeHeight != 0)
			{
				this.PhysicalPaperSize = new Size (paperSizeWidth, paperSizeHeight);
			}
		}


		#region Duplex converter
		public static string DuplexToDescription(DuplexMode duplex)
		{
			switch (duplex)
			{
				case DuplexMode.Simplex:
					return "Simple face";

				case DuplexMode.Horizontal:
					return "Recto verso";

				case DuplexMode.Vertical:
					return "Recto verso retourné";

				default:
					return "Par défaut";

			}
		}

		public static DuplexMode DescriptionToDuplex(string name)
		{
			DuplexMode[] modes = { DuplexMode.Default, DuplexMode.Simplex, DuplexMode.Horizontal, DuplexMode.Vertical };

			foreach (var mode in modes)
			{
				if (name == PrintingUnit.DuplexToDescription (mode))
				{
					return mode;
				}
			}

			return DuplexMode.Default;
		}

		public static string DuplexToString(DuplexMode duplex)
		{
			return duplex.ToString ();
		}

		public static DuplexMode StringToDuplex(string name)
		{
			DuplexMode type;

			if (System.Enum.TryParse (name, out type))
			{
				return type;
			}
			else
			{
				return DuplexMode.Default;
			}
		}
		#endregion


		private readonly PrintingOptionDictionary	optionsDictionary;
	}
}
