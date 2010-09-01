//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;

using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core.Printers
{

	public class Printer
	{
		public Printer()
		{
		}

		public Printer(string logicalName)
		{
			this.LogicalName = logicalName;
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
			//	Description de l'imprimante.
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


		public string NiceDescription
		{
			//	Retourne une description consise et claire de l'imprimante.
			get
			{
				if (string.IsNullOrWhiteSpace (this.Comment))
				{
					return TextFormatter.FormatText (this.LogicalName, "(", this.PhysicalPrinterName, ",~", this.PhysicalPrinterTray, ")").ToString ();
				}
				else
				{
					return TextFormatter.FormatText (this.LogicalName, "(", this.Comment, ")").ToString ();
				}
			}
		}


		public string GetSerializableContent()
		{
			//	Retourne une string permettant de sérialiser l'ensemble de la classe.
			return string.Concat
				(
					"LogicalName=",
					this.LogicalName,

					Printer.serializableSeparator,
					
					"PhysicalName=",
					this.PhysicalPrinterName,

					Printer.serializableSeparator,
					
					"Tray=",
					this.PhysicalPrinterTray,

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
			//	Initialise l'ensemble de la classe à partir d'une string sérialisée.
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

						case "Comment":
							this.Comment = words[1];
							break;
					}
				}
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
				return !value.Contains (Printer.serializableSeparator);
			}
		}


		private static readonly string serializableSeparator = "•";  // puce, unicode 2022
	}
}
