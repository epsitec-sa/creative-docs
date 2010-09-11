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
	/// <summary>
	/// Une unité d'impression est un couple imprimante/bac.
	/// </summary>
	public class PrinterUnit
	{
		public PrinterUnit()
		{
			this.Copies = 1;
			this.forcingOptionsToClear = new List<DocumentOption> ();
			this.forcingOptionsToSet   = new List<DocumentOption> ();
		}

		public PrinterUnit(string logicalName)
		{
			this.LogicalName = logicalName;
			this.Copies = 1;
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

		public List<DocumentOption> ForcingOptionsToClear
		{
			get
			{
				return this.forcingOptionsToClear;
			}
		}

		public List<DocumentOption> ForcingOptionsToSet
		{
			get
			{
				return this.forcingOptionsToSet;
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
					"LogicalName=",
					this.LogicalName,

					PrinterUnit.serializableSeparator,
					
					"PhysicalName=",
					this.PhysicalPrinterName,

					PrinterUnit.serializableSeparator,
					
					"Tray=",
					this.PhysicalPrinterTray,

					PrinterUnit.serializableSeparator,
					
					"XOffset=",
					this.XOffset.ToString (CultureInfo.InvariantCulture),

					PrinterUnit.serializableSeparator,

					"YOffset=",
					this.YOffset.ToString (CultureInfo.InvariantCulture),

					PrinterUnit.serializableSeparator,

					"Copies=",
					this.Copies.ToString (CultureInfo.InvariantCulture),

					PrinterUnit.serializableSeparator,

					"Comment=",
					this.Comment,

					PrinterUnit.serializableSeparator,

					"ForcingOptionsToClear=",
					PrinterUnit.GetDocumentOptions (this.forcingOptionsToClear),

					PrinterUnit.serializableSeparator,

					"ForcingOptionsToSet=",
					PrinterUnit.GetDocumentOptions (this.forcingOptionsToSet)
				);
		}

		public void SetSerializableContent(string content)
		{
			//	Initialise l'ensemble de la classe à partir d'une string sérialisée.
			var list = content.Split (new string[] { PrinterUnit.serializableSeparator }, System.StringSplitOptions.RemoveEmptyEntries);

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

						case "ForcingOptionsToClear":
							PrinterUnit.SetDocumentOptions (this.forcingOptionsToClear, words[1]);
							break;

						case "ForcingOptionsToSet":
							PrinterUnit.SetDocumentOptions (this.forcingOptionsToSet, words[1]);
							break;
					}
				}
			}
		}


		private static string GetDocumentOptions(List<DocumentOption> documentOptions)
		{
			var builder = new System.Text.StringBuilder ();

			foreach (var documentOption in documentOptions)
			{
				builder.Append (DocumentTypeDefinition.OptionToString (documentOption));
				builder.Append (" ");
			}

			return builder.ToString ();
		}

		private static void SetDocumentOptions(List<DocumentOption> documentOptions, string text)
		{
			documentOptions.Clear ();

			string[] list = text.Split (new string[] { " " }, System.StringSplitOptions.RemoveEmptyEntries);

			foreach (var t in list)
			{
				var documentOption = DocumentTypeDefinition.StringToOption (t);

				if (documentOption != DocumentOption.None)
				{
					documentOptions.Add (documentOption);
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
				return !value.Contains (PrinterUnit.serializableSeparator);
			}
		}


		private static readonly string serializableSeparator = "•";  // puce, unicode 2022

		private readonly List<DocumentOption> forcingOptionsToClear;
		private readonly List<DocumentOption> forcingOptionsToSet;
	}
}
