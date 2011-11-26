//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;
using Epsitec.Tool.MailingColis;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.BatchTool.SwissPostWebServices
{
	public class Engine
	{
		public Engine()
		{
			this.SwissPostBarCodeBegin ("TU_512275_02", "3XZEdu2Atk", "60011033,33115268,020148/PP");
			this.SwissPostBarCodeCustomer ("Epsitec SA", "", "Ch. du Fontenay 6", "", "1400", "Yverdon-les-Bains");
			this.SwissPostBarCodePostOffice ("1400 Yverdon-les-Bains");
			this.SwissPostBarCodeLogo ("..\\..\\epsitec-nrr.gif");
			this.SwissPostBarCodeLabelSize ("A6");
			this.SwissPostBarCodeLanguage ("fr");
		}

		public static string GetFullAddressLine(Address address)
		{
			string title   = address.Titre;
			string name1   = address.GetName1 ();
			string name2   = address.GetName2 ();
			string street  = address.GetStreet ();
			string poBox   = address.GetPOBox ();
			string zipCode = address.NPA;
			string city    = address.Localité;

			return string.Concat (title, "\t", name1, "\t", name2, "\t", street, "\t", poBox, "\t", zipCode, "\t", city);
		}

		public void PrintLabel(Address address)
		{
			string printerName = "Brother QL-1060N LE";
			string paperName   = "102mm x 152mm";

			string title   = address.Titre;
			string name1   = address.GetName1 ().Truncate (35);
			string name2   = address.GetName2 ().Truncate (35);
			string street  = address.GetStreet ().Truncate (35);
			string poBox   = address.GetPOBox ();
			string zipCode = address.NPA;
			string city    = address.Localité;

			this.SwissPostBarCodePrestation ("PPR");
			this.SwissPostBarCodeRecipient (title, name1, name2, street, poBox, zipCode, city, "CH");
			this.SwissPostBarCodeGenerateLabel ();
			
			this.PrinterBegin (printerName);
			this.PrinterPaperName (paperName);
			this.PrinterPaperOrientation ("Landscape");
			this.PrinterAddBarCode ("-2.0", "-2.5");
			this.PrinterEnd ();
		}

		
		public void SwissPostBarCodeBegin(string user, string password, string frankingLicense)
		{
			this.barCodeWebServices = new Dictionary<string, BarCodeWebService> ();

			var licenses = frankingLicense.Split (',').Select (x => x.Trim ());

			foreach (var license in licenses)
			{
				bool usePP = false;
				string licenseKey = license;

				if (license.EndsWith ("/PP"))
				{
					usePP = true;
					licenseKey = license.Substring (0, license.Length - 3);
				}

				var service = new BarCodeWebService ();
				service.DefineCredentials (user, password);
				service.DefineFrankingLicense (licenseKey);
				service.DefineFrankingPP (usePP);

				this.barCodeWebServices.Add (license, service);
			}
		}

		public void SwissPostBarCodeCustomer(string name1, string name2, string street, string poBox, string zipCode, string city)
		{
			this.barCodeWebServices.Values.ToList ().ForEach (s => s.DefineCustomerName (name1, name2));
			this.barCodeWebServices.Values.ToList ().ForEach (s => s.DefineCustomerAddress (street, poBox, zipCode, city));
		}

		public void SwissPostBarCodePostOffice(string office)
		{
			this.barCodeWebServices.Values.ToList ().ForEach (s => s.DefineCustomerPostOffice (office));
		}

		public void SwissPostBarCodeResolution(string value)
		{
			this.barCodeWebServices.Values.ToList ().ForEach (s => s.DefineImageResolution (int.Parse (value, System.Globalization.CultureInfo.InvariantCulture)));
		}

		public void SwissPostBarCodeRecipient(string title, string name1, string name2, string street, string poBox, string zipCode, string city, string country)
		{
			this.barCodeWebServices.Values.ToList ().ForEach (s => s.DefineRecipientName (title, name1, name2));
			this.barCodeWebServices.Values.ToList ().ForEach (s => s.DefineRecipientAddress (street, poBox, zipCode, city, country));
		}

		public void SwissPostBarCodeRecipientContacts(string phone, string mobile, string email)
		{
			this.barCodeWebServices.Values.ToList ().ForEach (s => s.DefineRecipientContacts (phone, mobile, email));
		}

		public void SwissPostBarCodeIncludeProClima()
		{
			this.barCodeWebServices.Values.ToList ().ForEach (s => s.DefineLabelProClima ());
		}

		public void SwissPostBarCodeAddressType(string type)
		{
			this.barCodeWebServices.Values.ToList ().ForEach (s => s.DefineAddressType (type));
		}

		public void SwissPostBarCodeLogo(string filename)
		{
			this.barCodeWebServices.Values.ToList ().ForEach (s => s.DefineLogo (filename));
		}

		public void SwissPostBarCodeDefinePaybackAmount(string amount)
		{
			this.barCodeWebServices.Values.ToList ().ForEach (s => s.DefinePaybackAmount (System.Convert.ToInt32 (amount)));
		}

		public void SwissPostBarCodeParcelCount(string index, string total)
		{
			this.barCodeWebServices.Values.ToList ().ForEach (s => s.DefineLabelParcelCount (int.Parse (index, System.Globalization.CultureInfo.InvariantCulture), int.Parse (total, System.Globalization.CultureInfo.InvariantCulture)));
		}

		public void SwissPostBarCodeLanguage(string text)
		{
			this.barCodeWebServices.Values.ToList ().ForEach (s => s.DefineDefaultLanguage (text));
		}

		public void SwissPostBarCodeFreeText(string text)
		{
			this.barCodeWebServices.Values.ToList ().ForEach (s => s.DefineLabelFreeText (text));
		}

		public void SwissPostBarCodeDeliveryPlace(string text)
		{
			this.barCodeWebServices.Values.ToList ().ForEach (s => s.DefineLabelDeliveryPlace (text));
		}

		public void SwissPostBarCodeDeliveryDate(string year, string month, string day)
		{
			this.barCodeWebServices.Values.ToList ().ForEach (s => s.DefineLabelDeliveryDate (new System.DateTime (System.Convert.ToInt32 (year), System.Convert.ToInt32 (month), System.Convert.ToInt32 (day))));
		}

		public void SwissPostBarCodePrestation(string code)
		{
			foreach (var item in code.Replace ("\n", " ").Replace ("\r", "").Split (' '))
			{
				switch (item.Split ('#').First ())
				{
					case "_AUTO":
						this.barCodeWebServices.Values.ToList ().ForEach (s => s.SetAutoCount (int.Parse (item.Split ('#')[1], System.Globalization.CultureInfo.InvariantCulture)));
						break;
					case "_GROUP":
						break;
					default:
						this.barCodeWebServices.Values.ToList ().ForEach (s => s.DefineLabelCodePrestation (item));
						break;
				}
			}
		}

		public void SwissPostBarCodeLabelSize(string size)
		{
			this.barCodeWebServices.Values.ToList ().ForEach (s => s.DefineLabelSize (size));
		}

		public void SwissPostBarCodeClearCache()
		{
			var service = this.barCodeWebServices.First ().Value;

			service.ClearServiceCache ("fr");
			service.ClearServiceCache ("de");
			service.ClearServiceCache ("en");
			service.ClearServiceCache ("it");
		}

		public string SwissPostBarCodeGenerateLabel()
		{
			BarCodeWebService service = null;
			string error = "";

			foreach (var item in this.barCodeWebServices.Values)
			{
				service = item;
				error   = service.GenerateLabel ();

				if (error != Engine.BarCodeLicenseError)
				{
					break;
				}
			}

			this.selectedBarCodeWebService = service;

			return error ?? service.GetLabelCode ();
		}

		public void SwissPostBarCodeGenerateLabel(string path, string outputCodeVariable)
		{
			this.SwissPostBarCodeGenerateLabel ();
			this.SwissPostBarCodeSaveLabelImage (path);
		}

		public void SwissPostBarCodeSaveLabelImage(string path)
		{
			byte[] rawImageData  = this.selectedBarCodeWebService.GetLabelData ();

			if (rawImageData == null)
			{
				if (System.IO.File.Exists (path))
				{
					System.IO.File.Delete (path);
				}
				return;
			}

			string fileExtension = System.IO.Path.GetExtension (path).ToLowerInvariant ();

			if (fileExtension == ".bmp")
			{
				System.Drawing.Image decodedImage = null;

				using (var stream = new System.IO.MemoryStream (rawImageData))
				{
					decodedImage = System.Drawing.Bitmap.FromStream (stream);
				}

				decodedImage.Save (path, System.Drawing.Imaging.ImageFormat.Bmp);
			}
			else
			{
				System.IO.File.WriteAllBytes (path, rawImageData);
			}
		}

		public void SwissPostBarCodeEnd()
		{
			/* Nothing yet */
		}

		public void PrinterBegin()
		{
			this.printer = new Printer ();
		}

		public void PrinterBegin(string printerName)
		{
			this.PrinterBegin ();
			this.PrinterName (printerName);
		}

		public void PrinterName(string printerName)
		{
			if (this.printer.DefinePrinterName (printerName) == false)
			{
				throw new System.Exception ("Invalid printer name: " + printerName);
			}
		}

		public void PrinterName(string printerName, string printFileName)
		{
			if (this.printer.DefinePrinterName (printerName) == false)
			{
				throw new System.Exception ("Invalid printer name: " + printerName);
			}
			if (this.printer.DefinePrintFileName (printFileName) == false)
			{
				throw new System.Exception ("Invalid print file name: " + printFileName);
			}
		}

		public void PrinterPaperName(string paperName)
		{
			if (this.printer.DefinePaperSize (paperName) == false)
			{
				throw new System.Exception ("Invalid paper name: " + paperName);
			}
		}

		public void PrinterPaperSource(string paperSource)
		{
			if (this.printer.DefinePaperSource (paperSource) == false)
			{
				throw new System.Exception ("Invalid paper source: " + paperSource);
			}
		}

		public void PrinterPaperOrientation(string paperOrientation)
		{
			if (this.printer.DefinePaperOrientation (paperOrientation) == false)
			{
				throw new System.Exception ("Invalid paper orientation: " + paperOrientation);
			}
		}

		public void PrinterOffset(string ox, string oy)
		{
			this.printer.DefineOffset (double.Parse (ox, System.Globalization.CultureInfo.InvariantCulture), double.Parse (oy, System.Globalization.CultureInfo.InvariantCulture));
		}

		public void PrinterAddImage(string imagePath, string imageResolution)
		{
			this.printer.AddImage (0, 0, Printer.LoadImage (imagePath), int.Parse (imageResolution, System.Globalization.CultureInfo.InvariantCulture));
		}

		public void PrinterAddImage(string ox, string oy, string imagePath, string imageResolution)
		{
			this.printer.AddImage (
				double.Parse (ox, System.Globalization.CultureInfo.InvariantCulture),
				double.Parse (oy, System.Globalization.CultureInfo.InvariantCulture),
				Printer.LoadImage (imagePath),
				int.Parse (imageResolution, System.Globalization.CultureInfo.InvariantCulture));
		}

		public void PrinterAddBarCode(string ox, string oy)
		{
			if ((this.selectedBarCodeWebService == null) ||
					(this.selectedBarCodeWebService.GetLabelImage () == null))
			{
				throw new System.Exception ("No bar code image available");
			}

			var image = this.selectedBarCodeWebService.GetLabelImage ();
			var dpi = this.selectedBarCodeWebService.LabelResolution;
			this.printer.AddImage (
				double.Parse (ox, System.Globalization.CultureInfo.InvariantCulture),
				double.Parse (oy, System.Globalization.CultureInfo.InvariantCulture),
				image, dpi);
		}

		public void PrinterAddTextBlock(string ox, string oy, string width, string fontFamily, string fontStyle, string fontEmSize, string text)
		{
			this.printer.AddText (
				double.Parse (ox, System.Globalization.CultureInfo.InvariantCulture),
				double.Parse (oy, System.Globalization.CultureInfo.InvariantCulture),
				double.Parse (width, System.Globalization.CultureInfo.InvariantCulture),
				text,
				fontFamily, fontStyle,
				float.Parse (fontEmSize, System.Globalization.CultureInfo.InvariantCulture));
		}

		public void PrinterAddMoreText(string fontFamily, string fontStyle, string fontEmSize, string text)
		{
			this.printer.AddText (
				text,
				fontFamily, fontStyle,
				float.Parse (fontEmSize, System.Globalization.CultureInfo.InvariantCulture));
		}

		public void PrinterAddPage()
		{
			this.printer.AddNextPage ();
		}

		public void PrinterEnd()
		{
			this.printer.PrintDocument ();
			this.printer.Dispose ();
			this.printer = null;
		}

		public static System.Windows.Forms.IWin32Window OwnerWindow;
		
		private static readonly string                BarCodeLicenseError = "E2026";

		private Dictionary<string, BarCodeWebService> barCodeWebServices;
		private BarCodeWebService selectedBarCodeWebService;
		private Printer printer;
	}
}
