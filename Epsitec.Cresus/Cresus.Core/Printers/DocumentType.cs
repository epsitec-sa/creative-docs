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
	public class DocumentType
	{
		public DocumentType(DocumentTypeEnum type, string shortDescription, string longDescription)
		{
			this.Type = type;
			this.ShortDescription = shortDescription;
			this.LongDescription = longDescription;

			this.options = new List<DocumentOption> ();
			this.printers = new List<DocumentPrinter> ();
		}

		public DocumentTypeEnum Type
		{
			get;
			set;
		}

		public string ShortDescription
		{
			get;
			set;
		}

		public string LongDescription
		{
			get;
			set;
		}

		public List<DocumentOption> DocumentOptions
		{
			get
			{
				return this.options;
			}
		}

		public List<DocumentPrinter> DocumentPrinters
		{
			get
			{
				return this.printers;
			}
		}

		public DocumentPrinter GetDocumentPrinter(PrinterFunction printerFunction)
		{
			return this.printers.Where (x => x.PrinterFunction == printerFunction).FirstOrDefault ();
		}

		public bool IsDocumentPrintersDefined
		{
			get
			{
				DocumentPrinter all = this.GetDocumentPrinter (PrinterFunction.ForAllPages);
				if (all != null && !string.IsNullOrWhiteSpace (all.LogicalPrinterName))
				{
					return true;
				}

				foreach (var p in this.printers)
				{
					if (p.PrinterFunction == PrinterFunction.ForAllPages ||
						p.PrinterFunction == PrinterFunction.ForPagesCopy)
					{
						continue;
					}

					if (string.IsNullOrWhiteSpace (p.LogicalPrinterName))
					{
						return false;
					}
				}

				return true;
			}
		}


		#region Add options
		public void AddDocumentOptionInvoice()
		{
			//	Ajoute les options d'impression liées aux factures.
			if (this.Type == DocumentTypeEnum.BL                   ||
				this.Type == DocumentTypeEnum.InvoiceWithInsideESR ||
				this.Type == DocumentTypeEnum.InvoiceWithOutsideESR||
				this.Type == DocumentTypeEnum.InvoiceWithoutESR    )
			{
				this.options.Add (new DocumentOption ("Delayed", null, "Imprime les articles livrés ultérieurement", true));
			}

			this.options.Add (new DocumentOption ("ArticleId", null, "Imprime les identificateurs d'article", false));

			this.options.Add (new DocumentOption ("Aspect de la liste des articles :"));
			this.options.Add (new DocumentOption ("Frameless", "TableAspect", "Espacé, sans encadrements"));
			this.options.Add (new DocumentOption ("WithLine",  "TableAspect", "Espacé, avec des lignes de séparation", true));
			this.options.Add (new DocumentOption ("WithFrame", "TableAspect", "Serré, avec des encadrements"));

			this.options.Add (new DocumentOption ("Ordre des colonnes :"));

			if (this.Type == DocumentTypeEnum.BL             ||
				this.Type == DocumentTypeEnum.ProductionOrder)
			{
				this.options.Add (new DocumentOption ("ColumnsOrderQD", "ColumnsOrder", "Quantité, Désignation", true));
				this.options.Add (new DocumentOption ("ColumnsOrderDQ", "ColumnsOrder", "Désignation, Quantité"));
			}
			else
			{
				this.options.Add (new DocumentOption ("ColumnsOrderQD", "ColumnsOrder", "Quantité, Désignation, Prix", true));
				this.options.Add (new DocumentOption ("ColumnsOrderDQ", "ColumnsOrder", "Désignation, Quantité, Prix"));
			}
		}

		public void AddDocumentOptionEsr()
		{
			//	Ajoute les options d'impression liées aux BV.
			this.options.Add (new DocumentOption ("Type de bulletin de versement :"));
			this.options.Add (new DocumentOption ("ESR", "ESR", "BVR orange", true));
			this.options.Add (new DocumentOption ("ES",  "ESR", "BV rose"));

			this.options.Add (new DocumentOption ("Mode d'impression du BV :"));
			this.options.Add (new DocumentOption ("ESR.Simul",    null, "Fac-similé complet du BV (pour des essais)", true));
			this.options.Add (new DocumentOption ("ESR.Specimen", null, "Incruste la mention SPECIMEN"));
		}

		public void AddDocumentOptionOrientation()
		{
			//	Ajoute les options d'impression liées à l'orientation portrait/paysage.
			this.options.Add (new DocumentOption ("Orientation du papier :"));
			this.options.Add (new DocumentOption ("Orientation.Vertical",   "Orientation", "Portrait", true));
			this.options.Add (new DocumentOption ("Orientation.Horizontal", "Orientation", "Paysage"));
		}

		public void AddDocumentOptionBL()
		{
			//	Ajoute les options d'impression générales.
			this.options.Add (new DocumentOption ("BL.Signing", null, "Cartouche \"Matériel reçu\"", true));
		}

		public void AddDocumentOptionProd()
		{
			//	Ajoute les options d'impression générales.
			this.options.Add (new DocumentOption ("Prod.Signing", null, "Cartouche \"Matériel produit\"", true));
		}

		public void AddDocumentOptionCommande()
		{
			//	Ajoute les options d'impression générales.
			this.options.Add (new DocumentOption ("Commande.Signing", null, "Cartouche \"Bon pour commande\"", true));
		}

		public void AddDocumentOptionSpecimen()
		{
			//	Ajoute les options d'impression générales.
			this.options.Add (new DocumentOption ("Generic.Specimen", null, "Incruste la mention SPECIMEN"));
		}

		public void AddDocumentOptionMargin()
		{
			//	Ajoute une marge verticale.
			this.options.Add (new DocumentOption (20));
		}
		#endregion


		#region Add printers to use
		public void AddPrinterBase()
		{
			//	Ajoute les imprimantes de base, qui devraient toujours exister.
			this.printers.Add (new DocumentPrinter (PrinterFunction.ForAllPages,       "Pour l'ensemble des pages :",              "Base"));
			this.printers.Add (new DocumentPrinter (PrinterFunction.ForPagesCopy,      "Pour une copie de l'ensemble des pages :", "Base"));

			this.printers.Add (new DocumentPrinter (PrinterFunction.ForFirstPage,      "Pour la première page :",                  "Spec"));
			this.printers.Add (new DocumentPrinter (PrinterFunction.ForFollowingPages, "Pour les pages suivantes :",               "Spec"));
		}

		public void AddPrinterEsr()
		{
			//	Ajoute l'imprimante spécifique pour les BV.
			this.printers.Add (new DocumentPrinter (PrinterFunction.ForEsrPage,        "Pour le BV :",                             "Spec"));
		}
		#endregion

		public static string TypeToString(DocumentTypeEnum type)
		{
			return type.ToString ();
		}

		public static DocumentTypeEnum StringToType(string name)
		{
			DocumentTypeEnum type;

			if (System.Enum.TryParse (name, out type))
			{
				return type;
			}
			else
			{
				return DocumentTypeEnum.None;
			}
		}


		private readonly List<DocumentOption>		options;
		private readonly List<DocumentPrinter>		printers;
	}
}
