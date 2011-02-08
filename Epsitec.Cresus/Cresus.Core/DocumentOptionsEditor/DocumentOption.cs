//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Cresus.Core.Controllers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.DocumentOptionsEditor
{
	public class DocumentOption
	{
		public DocumentOption(string title, string group)
		{
			this.Title = title;
			this.Group = group;
		}

		public DocumentOption(string name, string group, string description, string defaultValue, params Business.DocumentType[] documentTypes)
		{
			this.Name          = name;
			this.Group         = group;
			this.Description   = description;
			this.DefaultValue  = defaultValue;
			this.documentTypes = documentTypes.ToList ();
		}

		public string Title
		{
			get;
			private set;
		}

		public string Name
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

		public string DefaultValue
		{
			get;
			private set;
		}

		public List<Business.DocumentType> DocumentTypes
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

				return string.Join ("<br/>", strings);
			}
		}


		public static IEnumerable<DocumentOption> GetAllDocumentOptions()
		{
			var list = new List<DocumentOption> ();

			//	Ajoute les options d'impression liées aux factures.
			list.Add (new DocumentOption ("Options pour les factures :", "InvoiceOption"));
			list.Add (new DocumentOption ("HeaderLogo",     "InvoiceOption", "Imprime le logo de l'entreprise",            "true",  Business.DocumentType.Invoice));
			list.Add (new DocumentOption ("ArticleDelayed", "InvoiceOption", "Imprime les articles livrés ultérieurement", "true",  Business.DocumentType.Invoice));
			list.Add (new DocumentOption ("ArticleId",      "InvoiceOption", "Imprime les identificateurs d'article",      "false", Business.DocumentType.Invoice));

			list.Add (new DocumentOption ("Aspect de la liste des articles :", "InvoiceTableAspect"));
			list.Add (new DocumentOption ("LayoutFrameless", "InvoiceTableAspect", "Espacé, sans encadrements",             "false", Business.DocumentType.Invoice));
			list.Add (new DocumentOption ("LayoutWithLine",  "InvoiceTableAspect", "Espacé, avec des lignes de séparation", "true",  Business.DocumentType.Invoice));
			list.Add (new DocumentOption ("LayoutWithFrame", "InvoiceTableAspect", "Serré, avec des encadrements",          "false", Business.DocumentType.Invoice));

			list.Add (new DocumentOption ("Ordre des colonnes :", "InvoiceColumnsOrder"));
			list.Add (new DocumentOption ("ColumnsOrderQD", "InvoiceColumnsOrder", "Quantité, Désignation, Prix", "true", Business.DocumentType.Invoice));
			list.Add (new DocumentOption ("ColumnsOrderDQ", "InvoiceColumnsOrder", "Désignation, Quantité, Prix", "false", Business.DocumentType.Invoice));

			//	Ajoute les options d'impression liées aux BV.
			list.Add (new DocumentOption ("Type de la facture :", "InvoiceESR"));
			list.Add (new DocumentOption ("InvoiceWithInsideESR",  "InvoiceESR", "Facture avec BV intégré", "false", Business.DocumentType.Invoice));
			list.Add (new DocumentOption ("InvoiceWithOutsideESR", "InvoiceESR", "Facture avec BV séparé",  "false", Business.DocumentType.Invoice));
			list.Add (new DocumentOption ("InvoiceWithoutESR",     "InvoiceESR", "Facture sans BV",         "true",  Business.DocumentType.Invoice));

			list.Add (new DocumentOption ("Type de bulletin de versement :", "InvoiceESRType"));
			list.Add (new DocumentOption ("InvoiceWithESR", "InvoiceESRType", "BVR orange", "true",  Business.DocumentType.Invoice));
			list.Add (new DocumentOption ("InvoiceWithES",  "InvoiceESRType", "BV rose",    "false", Business.DocumentType.Invoice));

			list.Add (new DocumentOption ("Mode d'impression du BV :", "InvoiceESRMode"));
			list.Add (new DocumentOption ("ESRFacsimile", "InvoiceESRMode", "Fac-similé complet du BV", "true", Business.DocumentType.Invoice));

			//	Ajoute les options d'impression liées à l'orientation portrait/paysage.
			list.Add (new DocumentOption ("Orientation du papier :", "Orientation"));
			list.Add (new DocumentOption ("OrientationVertical",   "Orientation", "Portrait", "true"));
			list.Add (new DocumentOption ("OrientationHorizontal", "Orientation", "Paysage",  "false"));

			//	Ajoute les options d'impression générales.
			list.Add (new DocumentOption ("Signing",  null, "Cartouche",                    "true"));
			list.Add (new DocumentOption ("Specimen", null, "Incruste la mention SPECIMEN", "false"));

			return list;
		}


		private readonly List<Business.DocumentType> documentTypes;
	}
}
