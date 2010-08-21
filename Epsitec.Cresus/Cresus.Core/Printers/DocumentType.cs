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


		#region Add options
		public void DocumentOptionsAddInvoice()
		{
			//	Ajoute les options d'impression liées aux factures.
			if (this.Type == DocumentTypeEnum.BL    ||
				this.Type == DocumentTypeEnum.InvoiceWithESR   ||
				this.Type == DocumentTypeEnum.InvoiceWithoutESR)
			{
				this.options.Add (new DocumentOption ("Delayed", null, "Imprime les articles livrés ultérieurement", true));
			}

			this.options.Add (new DocumentOption ("ArticleId", null, "Imprime les identificateurs d'article", false));

			this.options.Add (new DocumentOption ("Aspect de la liste des articles :"));
			this.options.Add (new DocumentOption ("Frameless", "TableAspect", "Espacé, sans encadrements"));
			this.options.Add (new DocumentOption ("WithLine",  "TableAspect", "Espacé, avec des lignes de séparation", true));
			this.options.Add (new DocumentOption ("WithFrame", "TableAspect", "Serré, avec des encadrements"));

			this.options.Add (new DocumentOption ("Ordre des colonnes :"));

			if (this.Type == DocumentTypeEnum.BL  ||
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

		public void DocumentOptionsAddEsr()
		{
			//	Ajoute les options d'impression liées aux BV.
			this.options.Add (new DocumentOption ("Type de bulletin de versement :"));
			this.options.Add (new DocumentOption ("ESR", "ESR", "BVR orange", true));
			this.options.Add (new DocumentOption ("ES",  "ESR", "BV rose"));

			this.options.Add (new DocumentOption ("Mode d'impression du BV :"));
			this.options.Add (new DocumentOption ("ESR.Simul",    null, "Fac-similé complet du BV (pour des essais)", true));
			this.options.Add (new DocumentOption ("ESR.Specimen", null, "Incruste la mention SPECIMEN"));
		}

		public void DocumentOptionsAddOrientation()
		{
			//	Ajoute les options d'impression liées à l'orientation portrait/paysage.
			this.options.Add (new DocumentOption ("Orientation du papier :"));
			this.options.Add (new DocumentOption ("Orientation.Vertical",   "Orientation", "Portrait", true));
			this.options.Add (new DocumentOption ("Orientation.Horizontal", "Orientation", "Paysage"));
		}

		public void DocumentOptionsAddBL()
		{
			//	Ajoute les options d'impression générales.
			this.options.Add (new DocumentOption ("BL.Signing", null, "Cartouche \"Matériel reçu\" avec signature", true));
		}

		public void DocumentOptionsAddProd()
		{
			//	Ajoute les options d'impression générales.
			this.options.Add (new DocumentOption ("Prod.Signing", null, "Cartouche \"Matériel produit\" avec signature", true));
		}

		public void DocumentOptionsAddCommande()
		{
			//	Ajoute les options d'impression générales.
			this.options.Add (new DocumentOption ("Commande.Signing", null, "Cartouche \"Bon pour commande\" avec signature", true));
		}

		public void DocumentOptionsAddSpecimen()
		{
			//	Ajoute les options d'impression générales.
			this.options.Add (new DocumentOption ("Generic.Specimen", null, "Incruste la mention SPECIMEN"));
		}

		public void DocumentOptionsAddMargin()
		{
			//	Ajoute une marge verticale.
			this.options.Add (new DocumentOption (20));
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


		private readonly List<DocumentOption> options;
	}
}
