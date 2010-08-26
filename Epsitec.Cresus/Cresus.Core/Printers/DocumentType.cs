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
			this.printersToUse = new List<PrinterToUse> ();
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

		public List<PrinterToUse> PrintersToUse
		{
			get
			{
				return this.printersToUse;
			}
		}

		public PrinterToUse GetPrinterToUse(string code)
		{
			return this.printersToUse.Where (x => x.Code == code).FirstOrDefault ();
		}

		public bool IsPrintersToUseDefined
		{
			get
			{
				foreach (var p in this.printersToUse)
				{
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
			//	Ajoute l'imprimante de base, qui devrait toujours exister.
			this.printersToUse.Add (new PrinterToUse ("All", "Pour l'ensemble des pages :"));
		}

		public void AddPrinterFirst()
		{
			//	Ajoute les imprimantes de base, qui devraient toujours exister.
			this.printersToUse.Add (new PrinterToUse ("First", "Pour la première page :"));
			this.printersToUse.Add (new PrinterToUse ("Following", "Pour les pages suivantes :"));
		}

		public void AddPrinterEsr()
		{
			//	Ajoute l'imprimante spécifique pour les BV.
			this.printersToUse.Add (new PrinterToUse ("Esr", "Pour le BV :"));
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
		private readonly List<PrinterToUse>			printersToUse;
	}
}
