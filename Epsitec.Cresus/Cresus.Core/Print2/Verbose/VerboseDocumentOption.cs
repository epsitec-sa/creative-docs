//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Print2;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Print2.Verbose
{
	public class VerboseDocumentOption
	{
		public VerboseDocumentOption(string title, string group)
		{
			this.Title = title;
			this.Group = group;
		}

		public VerboseDocumentOption(DocumentOption option, string group, DocumentOptionValueType type, string description, string defaultValue, params Business.DocumentType[] documentTypes)
		{
			this.Option        = option;
			this.Group         = group;
			this.Type          = type;
			this.Description   = description;
			this.DefaultValue  = defaultValue;
			this.documentTypes = documentTypes.ToList ();
		}

		public VerboseDocumentOption(DocumentOption option, string group, IEnumerable<string> enumeration, IEnumerable<string> enumerationDescription, string defaultValue, params Business.DocumentType[] documentTypes)
		{
			System.Diagnostics.Debug.Assert (enumeration != null && enumerationDescription != null);
			System.Diagnostics.Debug.Assert (enumeration.Count () == enumerationDescription.Count ());

			this.Option                 = option;
			this.Group                  = group;
			this.Type                   = DocumentOptionValueType.Enumeration;
			this.Enumeration            = enumeration;
			this.EnumerationDescription = enumerationDescription;
			this.DefaultValue           = defaultValue;
			this.documentTypes          = documentTypes.ToList ();
		}

		public string Title
		{
			get;
			private set;
		}

		public DocumentOption Option
		{
			get;
			private set;
		}

		public DocumentOptionValueType Type
		{
			get;
			private set;
		}

		public IEnumerable<string> Enumeration
		{
			get;
			private set;
		}

		public string Group
		{
			get;
			private set;
		}

		public string Description
		{
			get;
			private set;
		}

		public IEnumerable<string> EnumerationDescription
		{
			get;
			private set;
		}

		public string DefaultValue
		{
			get;
			private set;
		}

		public IEnumerable<Business.DocumentType> DocumentTypes
		{
			get
			{
				return this.documentTypes;
			}
		}

		public bool IsTitle
		{
			get
			{
				return !string.IsNullOrEmpty (this.Title);
			}
		}

		public string DocumentTypeDescription
		{
			get
			{
				var types = Business.Enumerations.GetAllPossibleDocumentType ();
				var strings = new List<string> ();

				foreach (Business.DocumentType type in this.documentTypes)
				{
					var t = types.Where (x => x.Key == type).FirstOrDefault ();

					if (t != null)
					{
						strings.Add (string.Concat ("● ", t.Values[0]));
					}
				}

				if (strings.Count == 0)
				{
					strings.Add (string.Concat ("● ", "Tous"));
				}

				return string.Join ("<br/>", strings);
			}
		}


		internal static IEnumerable<VerboseDocumentOption> GetAll()
		{
			var list = new List<VerboseDocumentOption> ();
			string[] e, d;

			//	Ajoute les options d'impression liées à l'orientation portrait/paysage.
			list.Add (new VerboseDocumentOption ("Orientation du papier", "Orientation"));
			e = new string[] { "Portrait", "Landscape" };
			d = new string[] { "Portrait", "Paysage" };
			list.Add (new VerboseDocumentOption (DocumentOption.Orientation, "Orientation", e, d, "Portrait"));

			//	Ajoute les options d'impression générales.
			list.Add (new VerboseDocumentOption ("Options générales", "Global"));
			list.Add (new VerboseDocumentOption (DocumentOption.HeaderLogo, "Global", DocumentOptionValueType.Boolean, "Imprime le logo de l'entreprise", "true"));
			list.Add (new VerboseDocumentOption (DocumentOption.Specimen,   "Global", DocumentOptionValueType.Boolean, "Incruste la mention SPECIMEN",    "false"));

			list.Add (new VerboseDocumentOption (DocumentOption.Margins,      "Global", DocumentOptionValueType.Distance, "Marges",           "20"));
			list.Add (new VerboseDocumentOption (DocumentOption.LeftMargin,   "Global", DocumentOptionValueType.Distance, "Marge gauche",     "20"));
			list.Add (new VerboseDocumentOption (DocumentOption.RightMargin,  "Global", DocumentOptionValueType.Distance, "Marge droite",     "20"));
			list.Add (new VerboseDocumentOption (DocumentOption.TopMargin,    "Global", DocumentOptionValueType.Distance, "Marge supérieure", "20"));
			list.Add (new VerboseDocumentOption (DocumentOption.BottomMargin, "Global", DocumentOptionValueType.Distance, "Marge inférieure", "20"));

			list.Add (new VerboseDocumentOption ("Aspect des listes", "LayoutFrame"));
			e = new string[] { "Frameless", "WithLine", "WithFrame" };
			d = new string[] { "Espacé, sans encadrements", "Espacé, avec des lignes de séparation", "Serré, avec des encadrements" };
			list.Add (new VerboseDocumentOption (DocumentOption.LayoutFrame, "LayoutFrame", e, d, "WithLine"));

			//	Ajoute les options d'impression liées aux factures.
			list.Add (new VerboseDocumentOption ("Options pour les factures", "InvoiceOption"));
			list.Add (new VerboseDocumentOption (DocumentOption.ArticleDelayed, "InvoiceOption", DocumentOptionValueType.Boolean, "Imprime les articles livrés ultérieurement", "true",  Business.DocumentType.Invoice));
			list.Add (new VerboseDocumentOption (DocumentOption.ArticleId,      "InvoiceOption", DocumentOptionValueType.Boolean, "Imprime les identificateurs d'article",      "false", Business.DocumentType.Invoice));

			list.Add (new VerboseDocumentOption ("Ordre des colonnes", "ColumnsOrder"));
			e = new string[] { "QD", "DQ" };
			d = new string[] { "Quantité, Désignation, Prix", "Désignation, Quantité, Prix" };
			list.Add (new VerboseDocumentOption (DocumentOption.ColumnsOrder, "ColumnsOrder", e, d, "QD", Business.DocumentType.Invoice));

			//	Ajoute les options d'impression liées aux BV.
			list.Add (new VerboseDocumentOption ("Type de la facture", "EsrPosition"));
			e = new string[] { "Without", "WithInside", "WithOutside" };
			d = new string[] { "Facture sans BV", "Facture avec BV intégré", "Facture avec BV séparé" };
			list.Add (new VerboseDocumentOption (DocumentOption.EsrPosition, "EsrPosition", e, d, "Without", Business.DocumentType.Invoice));

			list.Add (new VerboseDocumentOption ("Type de bulletin de versement", "EsrType"));
			e = new string[] { "Esr", "Es" };
			d = new string[] { "BV orange", "BV rose" };
			list.Add (new VerboseDocumentOption (DocumentOption.EsrType, "EsrType", e, d, "Esr", Business.DocumentType.Invoice));

			list.Add (new VerboseDocumentOption ("Mode d'impression du BV", "InvoiceEsrMode"));
			list.Add (new VerboseDocumentOption (DocumentOption.EsrFacsimile, "InvoiceEsrMode", DocumentOptionValueType.Boolean, "Fac-similé complet du BV", "true", Business.DocumentType.Invoice));

			list.Add (new VerboseDocumentOption (DocumentOption.Signing, "Signing", DocumentOptionValueType.Boolean, "Cartouche", "true", Business.DocumentType.OrderBooking, Business.DocumentType.OrderConfirmation, Business.DocumentType.ProductionOrder, Business.DocumentType.ProductionChecklist, Business.DocumentType.ShipmentChecklist, Business.DocumentType.DeliveryNote, Business.DocumentType.Receipt));

			//	Ajoute les options pour les clients.
			list.Add (new VerboseDocumentOption ("Données du client à inclure", "Relation"));
			list.Add (new VerboseDocumentOption (DocumentOption.RelationMail,    "Relation", DocumentOptionValueType.Boolean,  "Adresses",   "true", Business.DocumentType.Summary));
			list.Add (new VerboseDocumentOption (DocumentOption.RelationTelecom, "Relation", DocumentOptionValueType.Boolean,  "Téléphones", "true", Business.DocumentType.Summary));
			list.Add (new VerboseDocumentOption (DocumentOption.RelationUri,     "Relation", DocumentOptionValueType.Boolean,  "Emails",     "true", Business.DocumentType.Summary));

			return list;
		}


		private readonly List<Business.DocumentType> documentTypes;
	}
}
