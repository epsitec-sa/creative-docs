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
		public DocumentTypeDefinition(DocumentType type, string shortDescription, string longDescription)
		{
			this.type             = type;
			this.shortDescription = shortDescription;
			this.longDescription  = longDescription;

			this.options = new List<DocumentOptionDefinition> ();
			this.printers = new List<DocumentPrinter> ();
		}

		public DocumentType Type
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

		public List<DocumentPrinter> DocumentPrinters
		{
			get
			{
				return this.printers;
			}
		}

		public DocumentPrinter GetDocumentPrinter(PrinterUnitFunction printerFunction)
		{
			return this.printers.Where (x => x.PrinterFunction == printerFunction).FirstOrDefault ();
		}

		public bool IsDocumentPrintersDefined
		{
			get
			{
				DocumentPrinter all = this.GetDocumentPrinter (PrinterUnitFunction.ForAllPages);
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

				return true;
			}
		}


		#region Add options
		public void AddDocumentOptionInvoice()
		{
			//	Ajoute les options d'impression liées aux factures.
			this.options.Add (new DocumentOptionDefinition ("Logo", null, "Imprime le logo de l'entreprise", true));

			if (this.Type == DocumentType.BL                   ||
				this.Type == DocumentType.InvoiceWithInsideESR ||
				this.Type == DocumentType.InvoiceWithOutsideESR||
				this.Type == DocumentType.InvoiceWithoutESR    )
			{
				this.options.Add (new DocumentOptionDefinition ("Delayed", null, "Imprime les articles livrés ultérieurement", true));
			}

			this.options.Add (new DocumentOptionDefinition ("ArticleId", null, "Imprime les identificateurs d'article", false));

			this.options.Add (new DocumentOptionDefinition ("Aspect de la liste des articles :"));
			this.options.Add (new DocumentOptionDefinition ("Frameless", "TableAspect", "Espacé, sans encadrements"));
			this.options.Add (new DocumentOptionDefinition ("WithLine",  "TableAspect", "Espacé, avec des lignes de séparation", true));
			this.options.Add (new DocumentOptionDefinition ("WithFrame", "TableAspect", "Serré, avec des encadrements"));

			this.options.Add (new DocumentOptionDefinition ("Ordre des colonnes :"));

			if (this.Type == DocumentType.BL             ||
				this.Type == DocumentType.ProductionOrder)
			{
				this.options.Add (new DocumentOptionDefinition ("ColumnsOrderQD", "ColumnsOrder", "Quantité, Désignation", true));
				this.options.Add (new DocumentOptionDefinition ("ColumnsOrderDQ", "ColumnsOrder", "Désignation, Quantité"));
			}
			else
			{
				this.options.Add (new DocumentOptionDefinition ("ColumnsOrderQD", "ColumnsOrder", "Quantité, Désignation, Prix", true));
				this.options.Add (new DocumentOptionDefinition ("ColumnsOrderDQ", "ColumnsOrder", "Désignation, Quantité, Prix"));
			}
		}

		public void AddDocumentOptionEsr()
		{
			//	Ajoute les options d'impression liées aux BV.
			this.options.Add (new DocumentOptionDefinition ("Type de bulletin de versement :"));
			this.options.Add (new DocumentOptionDefinition ("ESR", "ESR", "BVR orange", true));
			this.options.Add (new DocumentOptionDefinition ("ES",  "ESR", "BV rose"));

			this.options.Add (new DocumentOptionDefinition ("Mode d'impression du BV :"));
			this.options.Add (new DocumentOptionDefinition ("ESR.Simul",    null, "Fac-similé complet du BV (pour des essais)", true));
			this.options.Add (new DocumentOptionDefinition ("ESR.Specimen", null, "Incruste la mention SPECIMEN"));
		}

		public void AddDocumentOptionOrientation()
		{
			//	Ajoute les options d'impression liées à l'orientation portrait/paysage.
			this.options.Add (new DocumentOptionDefinition ("Orientation du papier :"));
			this.options.Add (new DocumentOptionDefinition ("Orientation.Vertical",   "Orientation", "Portrait", true));
			this.options.Add (new DocumentOptionDefinition ("Orientation.Horizontal", "Orientation", "Paysage"));
		}

		public void AddDocumentOptionBL()
		{
			//	Ajoute les options d'impression générales.
			this.options.Add (new DocumentOptionDefinition ("BL.Signing", null, "Cartouche \"Matériel reçu\"", true));
		}

		public void AddDocumentOptionProd()
		{
			//	Ajoute les options d'impression générales.
			this.options.Add (new DocumentOptionDefinition ("Prod.Signing", null, "Cartouche \"Matériel produit\"", true));
		}

		public void AddDocumentOptionCommande()
		{
			//	Ajoute les options d'impression générales.
			this.options.Add (new DocumentOptionDefinition ("Commande.Signing", null, "Cartouche \"Bon pour commande\"", true));
		}

		public void AddDocumentOptionSpecimen()
		{
			//	Ajoute les options d'impression générales.
			this.options.Add (new DocumentOptionDefinition ("Generic.Specimen", null, "Incruste la mention SPECIMEN"));
		}

		public void AddDocumentOptionMargin()
		{
			//	Ajoute une marge verticale.
			this.options.Add (new DocumentOptionDefinition (20));
		}
		#endregion


		#region Add printers to use
		public void AddBasePrinterUnit()
		{
			//	Ajoute les unités d'impression de base, qui devraient toujours exister.
			this.printers.Add (new DocumentPrinter (PrinterUnitFunction.ForAllPages,       "All",  "Pour l'ensemble des pages :"             ));
			this.printers.Add (new DocumentPrinter (PrinterUnitFunction.ForPagesCopy,      "Copy", "Pour une copie de l'ensemble des pages :"));

			this.printers.Add (new DocumentPrinter (PrinterUnitFunction.SinglePage,        "Spec", "Pour une page unique :"                  ));
			this.printers.Add (new DocumentPrinter (PrinterUnitFunction.ForFirstPage,      "Spec", "Pour la première page :"                 ));
			this.printers.Add (new DocumentPrinter (PrinterUnitFunction.ForFollowingPages, "Spec", "Pour les pages suivantes :"              ));
		}

		public void AddEsrPrinterUnit()
		{
			//	Ajoute l'unité d'impression spécifique pour les BV.
			this.printers.Add (new DocumentPrinter (PrinterUnitFunction.ForEsrPage,        "Spec", "Pour le BV :"                            ));
		}
		#endregion

		public static string TypeToString(DocumentType type)
		{
			return type.ToString ();
		}

		public static DocumentType StringToType(string name)
		{
			DocumentType type;

			if (System.Enum.TryParse (name, out type))
			{
				return type;
			}
			else
			{
				return DocumentType.None;
			}
		}


		private readonly DocumentType			type;
		private readonly string						shortDescription;
		private readonly string						longDescription;
		private readonly List<DocumentOptionDefinition>		options;
		private readonly List<DocumentPrinter>		printers;
	}
}
