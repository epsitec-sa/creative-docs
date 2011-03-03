//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Printing;

using Epsitec.Cresus.Core.Document;

using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core.Print
{
	/// <summary>
	/// Une unité d'impression est un couple imprimante/bac.
	/// </summary>
	public class PrintingUnit
	{
		public PrintingUnit()
		{
			this.Copies = 1;
			this.optionsDictionary = new OptionsDictionary ();
		}

		public PrintingUnit(string logicalName)
		{
			this.LogicalName = logicalName;
			this.Copies = 1;
			this.optionsDictionary = new OptionsDictionary ();
		}


		public string LogicalName
		{
			//	Nom logique choisi par l'utilisateur (nom de fonction).
			//	Les caractères spéciaux sont encodés (par exemple, un "&" vaut "&amp;").
			//	Cette propriété est donc compatible avec Widget.Text.
			get;
			set;
		}

		public string Comment
		{
			//	Description de l'unité d'impression.
			//	Les caractères spéciaux sont encodés (par exemple, un "&" vaut "&amp;").
			//	Cette propriété est donc compatible avec Widget.Text.
			get;
			set;
		}

		public string PhysicalPrinterName
		{
			//	Nom physique de l'imprimante.
			//	Les caractères spéciaux sont encodés (par exemple, un "&" vaut "&amp;").
			//	Cette propriété est donc compatible avec Widget.Text.
			//	En revanche, il faut utiliser FormattedText.Unescape avant de passer la chaîne à Epsitec.Common.Printing.
			get;
			set;
		}

		public string PhysicalPrinterTray
		{
			//	Nom physique du bac de l'impriante.
			//	Les caractères spéciaux sont encodés (par exemple, un "&" vaut "&amp;").
			//	Cette propriété est donc compatible avec Widget.Text.
			//	En revanche, il faut utiliser FormattedText.Unescape avant de passer la chaîne à Epsitec.Common.Printing.
			get;
			set;
		}

		public DuplexMode PhysicalDuplexMode
		{
			get;
			set;
		}

		public Size PhysicalPaperSize
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

		public int Copies
		{
			get;
			set;
		}

		public OptionsDictionary OptionsDictionary
		{
			get
			{
				return this.optionsDictionary;
			}
		}


		public string NiceDescription
		{
			//	Retourne une description consise et claire de l'unité d'impression.
			get
			{
				string c = this.Copies < 2 ? null : string.Format ("{0}×", this.Copies);
				var name = TextFormatter.FormatText (this.LogicalName, c);

				if (string.IsNullOrWhiteSpace (this.Comment))
				{
					return TextFormatter.FormatText (name, "(", this.PhysicalPrinterName, ",~", this.PhysicalPrinterTray, ")").ToString ();
				}
				else
				{
					return TextFormatter.FormatText (name, "(", this.Comment, ")").ToString ();
				}
			}
		}


		public string GetSerializableContent()
		{
			//	Retourne une string permettant de sérialiser l'ensemble de la classe.
			return string.Concat
				(
					                                    "LogicalName=",              this.LogicalName,
					PrintingUnit.serializableSeparator, "PhysicalName=",             this.PhysicalPrinterName,
					PrintingUnit.serializableSeparator, "Tray=",                     this.PhysicalPrinterTray,
					PrintingUnit.serializableSeparator, "XOffset=",                  this.XOffset.ToString (CultureInfo.InvariantCulture),
					PrintingUnit.serializableSeparator, "YOffset=",                  this.YOffset.ToString (CultureInfo.InvariantCulture),
					PrintingUnit.serializableSeparator, "Copies=",                   this.Copies.ToString (CultureInfo.InvariantCulture),
					PrintingUnit.serializableSeparator, "Comment=",                  this.Comment,
					PrintingUnit.serializableSeparator, "OptionsDictionary=",        this.optionsDictionary.GetSerializedData (),
					PrintingUnit.serializableSeparator, "PhysicalPaperSize.Width=",  this.PhysicalPaperSize.Width.ToString (CultureInfo.InvariantCulture),
					PrintingUnit.serializableSeparator, "PhysicalPaperSize.Height=", this.PhysicalPaperSize.Height.ToString (CultureInfo.InvariantCulture),
					PrintingUnit.serializableSeparator, "PhysicalDuplexMode=",       PrintingUnit.DuplexToString (this.PhysicalDuplexMode),
					null  // pour permettre de terminer le dernier par une virgule !
				);
		}

		public void SetSerializableContent(string content)
		{
			//	Initialise l'ensemble de la classe à partir d'une string sérialisée.
			var list = content.Split (new string[] { PrintingUnit.serializableSeparator }, System.StringSplitOptions.RemoveEmptyEntries);

			double paperSizeWidth  = 0;
			double paperSizeHeight = 0;

			this.optionsDictionary.Clear ();

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
							this.PhysicalPrinterName = words[1];
							break;

						case "Tray":
							this.PhysicalPrinterTray = words[1];
							break;

						case "XOffset":
							this.XOffset = double.Parse (words[1]);
							break;

						case "YOffset":
							this.YOffset = double.Parse (words[1]);
							break;

						case "Copies":
							this.Copies = int.Parse (words[1]);
							break;

						case "Comment":
							this.Comment = words[1];
							break;

						case "OptionsDictionary":
							this.optionsDictionary.SetSerializedData (words[1]);
							break;

						case "PhysicalPaperSize.Width":
							paperSizeWidth = int.Parse (words[1]);
							break;

						case "PhysicalPaperSize.Height":
							paperSizeHeight = int.Parse (words[1]);
							break;

						case "PhysicalDuplexMode":
							this.PhysicalDuplexMode = PrintingUnit.StringToDuplex (words[1]);
							break;
					}
				}
			}

			if (paperSizeWidth != 0 && paperSizeHeight != 0)
			{
				this.PhysicalPaperSize = new Size (paperSizeWidth, paperSizeHeight);
			}
		}


		public static bool CheckString(string value)
		{
			if (string.IsNullOrEmpty (value))
			{
				return true;
			}
			else
			{
				return !value.Contains (PrintingUnit.serializableSeparator);
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
					return "Recto verso horizontal";

				case DuplexMode.Vertical:
					return "Recto verso vertical";

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


		private static readonly string serializableSeparator = "•";  // puce, unicode 2022

		private readonly OptionsDictionary optionsDictionary;
	}
}
