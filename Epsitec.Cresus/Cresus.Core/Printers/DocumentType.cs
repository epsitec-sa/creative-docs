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
		public DocumentType(string name, string shortDescription, string longDescription)
		{
			this.Name = name;
			this.ShortDescription = shortDescription;
			this.LongDescription = longDescription;

			this.options = new List<DocumentOption> ();
		}

		public string Name
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
		public void DocumentOptionsAddInvoice(bool isBL)
		{
			//	Ajoute les options d'impression liées aux factures.
			this.options.Add (new DocumentOption ("Delayed",   null, "Imprime les articles livrés ultérieurement", true));
			this.options.Add (new DocumentOption ("ArticleId", null, "Imprime les identificateurs d'article", false));

			this.options.Add (new DocumentOption ("Aspect de la liste des articles :"));
			this.options.Add (new DocumentOption ("Frameless", "TableAspect", "Espacé sans encadrements", true));
			this.options.Add (new DocumentOption ("WithFrame", "TableAspect", "Serré avec encadrements"));

			this.options.Add (new DocumentOption ("Ordre des colonnes :"));

			if (isBL)
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


		private readonly List<DocumentOption> options;
	}
}
