//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Debug;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Printers
{
	public class DocumentTypeDefinition
	{
		public DocumentTypeDefinition(Business.DocumentType type, string shortDescription, string longDescription)
		{
			this.type             = type;
			this.shortDescription = shortDescription;
			this.longDescription  = longDescription;

			this.options = new List<DocumentOptionDefinition> ();
			this.printers = new List<DocumentPrinterFunction> ();
		}

		public Business.DocumentType Type
		{
			get
			{
				return this.type;
			}
		}

		public string ShortDescription
		{
			get
			{
				return this.shortDescription;
			}
		}

		public string LongDescription
		{
			get
			{
				return this.longDescription;
			}
		}

		public List<DocumentOptionDefinition> DocumentOptions
		{
			get
			{
				return this.options;
			}
		}

		public List<DocumentPrinterFunction> DocumentPrinterFunctions
		{
			get
			{
				return this.printers;
			}
		}

		public DocumentPrinterFunction GetDocumentPrinter(PrinterUnitFunction printerFunction)
		{
			return this.printers.Where (x => x.PrinterFunction == printerFunction).FirstOrDefault ();
		}

		public bool IsDocumentPrintersDefined
		{
			get
			{
#if false
				DocumentPrinterFunction all = this.GetDocumentPrinter (PrinterUnitFunction.ForAllPages);
				if (all != null && !string.IsNullOrWhiteSpace (all.LogicalPrinterName))
				{
					return true;
				}

				foreach (var p in this.printers)
				{
					if (p.PrinterFunction == PrinterUnitFunction.ForAllPages ||
						p.PrinterFunction == PrinterUnitFunction.ForPagesCopy)
					{
						continue;
					}

					if (string.IsNullOrWhiteSpace (p.LogicalPrinterName))
					{
						return false;
					}
				}
#endif

				return true;
			}
		}


		#region Add options
		public void AddDocumentOptionInvoice()
		{
			//	Ajoute les options d'impression liées aux factures.
			this.options.Add (new DocumentOptionDefinition (DocumentOption.HeaderLogo, null, "Imprime le logo de l'entreprise", true));

			if (this.Type == Business.DocumentType.DeliveryNote   ||
				this.Type == Business.DocumentType.Invoice        ||
				this.Type == Business.DocumentType.InvoiceProForma)
			{
				this.options.Add (new DocumentOptionDefinition (DocumentOption.ArticleDelayed, null, "Imprime les articles livrés ultérieurement", true));
			}

			this.options.Add (new DocumentOptionDefinition (DocumentOption.ArticleId, null, "Imprime les identificateurs d'article", false));

			this.options.Add (new DocumentOptionDefinition ("Aspect de la liste des articles :"));
			this.options.Add (new DocumentOptionDefinition (DocumentOption.LayoutFrameless, "TableAspect", "Espacé, sans encadrements"));
			this.options.Add (new DocumentOptionDefinition (DocumentOption.LayoutWithLine,  "TableAspect", "Espacé, avec des lignes de séparation", true));
			this.options.Add (new DocumentOptionDefinition (DocumentOption.LayoutWithFrame, "TableAspect", "Serré, avec des encadrements"));

			this.options.Add (new DocumentOptionDefinition ("Ordre des colonnes :"));

			if (this.Type == Business.DocumentType.DeliveryNote   ||
				this.Type == Business.DocumentType.ProductionOrder)
			{
				this.options.Add (new DocumentOptionDefinition (DocumentOption.ColumnsOrderQD, "ColumnsOrder", "Quantité, Désignation", true));
				this.options.Add (new DocumentOptionDefinition (DocumentOption.ColumnsOrderDQ, "ColumnsOrder", "Désignation, Quantité"));
			}
			else
			{
				this.options.Add (new DocumentOptionDefinition (DocumentOption.ColumnsOrderQD, "ColumnsOrder", "Quantité, Désignation, Prix", true));
				this.options.Add (new DocumentOptionDefinition (DocumentOption.ColumnsOrderDQ, "ColumnsOrder", "Désignation, Quantité, Prix"));
			}
		}

		public void AddDocumentOptionEsr()
		{
			//	Ajoute les options d'impression liées aux BV.
			this.options.Add (new DocumentOptionDefinition ("Type de la facture :"));
			this.options.Add (new DocumentOptionDefinition (DocumentOption.InvoiceWithInsideESR,  "Invoice", "Facture avec BV intégré"));
			this.options.Add (new DocumentOptionDefinition (DocumentOption.InvoiceWithOutsideESR, "Invoice", "Facture avec BV séparé"));
			this.options.Add (new DocumentOptionDefinition (DocumentOption.InvoiceWithoutESR,     "Invoice", "Facture sans BV", true));

			this.options.Add (new DocumentOptionDefinition ("Type de bulletin de versement :"));
			this.options.Add (new DocumentOptionDefinition (DocumentOption.InvoiceWithESR, "ESR", "BVR orange", true));
			this.options.Add (new DocumentOptionDefinition (DocumentOption.InvoiceWithES,  "ESR", "BV rose"));

			this.options.Add (new DocumentOptionDefinition ("Mode d'impression du BV :"));
			this.options.Add (new DocumentOptionDefinition (DocumentOption.ESRFacsimile, null, "Fac-similé complet du BV", true));
			this.options.Add (new DocumentOptionDefinition (DocumentOption.Specimen,     null, "Incruste la mention SPECIMEN"));
		}

		public void AddDocumentOptionOrientation()
		{
			//	Ajoute les options d'impression liées à l'orientation portrait/paysage.
			this.options.Add (new DocumentOptionDefinition ("Orientation du papier :"));
			this.options.Add (new DocumentOptionDefinition (DocumentOption.OrientationVertical,   "Orientation", "Portrait", true));
			this.options.Add (new DocumentOptionDefinition (DocumentOption.OrientationHorizontal, "Orientation", "Paysage"));
		}

		public void AddDocumentOptionBL()
		{
			//	Ajoute les options d'impression générales.
			this.options.Add (new DocumentOptionDefinition (DocumentOption.Signing, null, "Cartouche \"Matériel reçu\"", true));
		}

		public void AddDocumentOptionProductionOrder()
		{
			//	Ajoute les options d'impression générales.
			this.options.Add (new DocumentOptionDefinition (DocumentOption.Signing, null, "Cartouche \"Matériel produit\"", true));
		}

		public void AddDocumentOptionOrder()
		{
			//	Ajoute les options d'impression générales.
			this.options.Add (new DocumentOptionDefinition (DocumentOption.Signing, null, "Cartouche \"Bon pour commande\"", true));
		}

		public void AddDocumentOptionSpecimen()
		{
			//	Ajoute les options d'impression générales.
			this.options.Add (new DocumentOptionDefinition (DocumentOption.Specimen, null, "Incruste la mention SPECIMEN"));
		}

		public void AddDocumentOptionMargin()
		{
			//	Ajoute une marge verticale.
			this.options.Add (new DocumentOptionDefinition (20));
		}


		public static List<DocumentOptionDefinition> GetForcingOptions()
		{
			var list = new List<DocumentOptionDefinition> ();

			list.Add (new DocumentOptionDefinition (DocumentOption.HeaderLogo,   null, "Imprime le logo de l'entreprise"));
			list.Add (new DocumentOptionDefinition (DocumentOption.ESRFacsimile, null, "Fac-similé complet du BV"));
			list.Add (new DocumentOptionDefinition (DocumentOption.Signing,      null, "Cartouche"));
			list.Add (new DocumentOptionDefinition (DocumentOption.Specimen,     null, "Incruste la mention SPECIMEN"));

			return list;
		}
		#endregion


		#region Add printers to use
		public void AddBasePrinterUnit()
		{
			//	Ajoute les unités d'impression de base, qui devraient toujours exister.
			this.printers.Add (new DocumentPrinterFunction (PrinterUnitFunction.ForAllPages,       "All",  "Pour l'ensemble des pages :"             ));
			this.printers.Add (new DocumentPrinterFunction (PrinterUnitFunction.ForPagesCopy,      "Copy", "Pour une copie de l'ensemble des pages :"));

			this.printers.Add (new DocumentPrinterFunction (PrinterUnitFunction.ForSinglePage,     "Spec", "Pour une page unique :"                  ));
			this.printers.Add (new DocumentPrinterFunction (PrinterUnitFunction.ForFirstPage,      "Spec", "Pour la première page :"                 ));
			this.printers.Add (new DocumentPrinterFunction (PrinterUnitFunction.ForFollowingPages, "Spec", "Pour les pages suivantes :"              ));
		}

		public void AddEsrPrinterUnit()
		{
			//	Ajoute l'unité d'impression spécifique pour les BV.
			this.printers.Add (new DocumentPrinterFunction (PrinterUnitFunction.ForEsrPage, "Spec", "Pour le BV :"));
		}

		public void AddLabelPrinterUnit()
		{
			//	Ajoute l'unité d'impression spécifique pour les étiquettes.
			this.printers.Add (new DocumentPrinterFunction (PrinterUnitFunction.ForLabelPage, "Label", "Pour l'étiquette :"));
		}
		#endregion


		public static string TypeToString(Business.DocumentType type)
		{
			return type.ToString ();
		}

		public static Business.DocumentType StringToType(string name)
		{
			Business.DocumentType type;

			if (System.Enum.TryParse (name, out type))
			{
				return type;
			}
			else
			{
				return Business.DocumentType.None;
			}
		}


		public static string OptionToString(DocumentOption option)
		{
			return option.ToString ();
		}

		public static DocumentOption StringToOption(string name)
		{
			DocumentOption option;

			if (System.Enum.TryParse (name, out option))
			{
				return option;
			}
			else
			{
				return DocumentOption.None;
			}
		}


		private readonly Business.DocumentType				type;
		private readonly string								shortDescription;
		private readonly string								longDescription;
		private readonly List<DocumentOptionDefinition>		options;
		private readonly List<DocumentPrinterFunction>		printers;
	}
}
