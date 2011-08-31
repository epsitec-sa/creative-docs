//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Business;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Documents.Verbose
{
	public class VerboseDocumentOption
	{
		static VerboseDocumentOption()
		{
			VerboseDocumentOption.BuildAll ();
		}


		private VerboseDocumentOption(string title, string group)
		{
			this.Title = title;
			this.Group = group;
		}

		private VerboseDocumentOption(DocumentOption option, string group, bool isGlobal, DocumentOptionValueType type, string description, string defaultValue)
		{
			this.Option        = option;
			this.Group         = group;
			this.IsGlobal      = isGlobal;
			this.Type          = type;
			this.Description   = description;
			this.DefaultValue  = defaultValue;
		}

		private VerboseDocumentOption(DocumentOption option, string group, bool isGlobal, IEnumerable<string> enumeration, IEnumerable<string> enumerationDescription, string description, int defaultIndex)
		{
			System.Diagnostics.Debug.Assert (enumeration != null && enumerationDescription != null);
			System.Diagnostics.Debug.Assert (enumeration.Count () == enumerationDescription.Count ());

			this.Option                 = option;
			this.Group                  = group;
			this.IsGlobal               = isGlobal;
			this.Type                   = DocumentOptionValueType.Enumeration;
			this.Enumeration            = enumeration;
			this.EnumerationDescription = enumerationDescription;
			this.Description            = description;
			this.DefaultValue           = enumeration.ElementAt (defaultIndex);
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

		public bool IsGlobal
		{
			get;
			private set;
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
				var types = EnumKeyValues.FromEnum<DocumentType> ();
				var strings = new List<string> ();

				foreach (DocumentType type in this.DocumentTypes)
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

		private IEnumerable<DocumentType> DocumentTypes
		{
			get
			{
				var list = new List<DocumentType> ();
				bool all = true;

				foreach (DocumentType documentType in System.Enum.GetValues (typeof (DocumentType)))
				{
					if (documentType == DocumentType.None   ||
						documentType == DocumentType.Unknown)
					{
						continue;
					}

					var options = External.CresusCore.GetRequiredDocumentOptionsByDocumentType (documentType);

					if (options != null && options.Contains (this.Option))
					{
						list.Add (documentType);
					}
					else
					{
						if (options != null && options.Count () != 0)  // document implémenté ?
						{
							all = false;
						}
					}
				}

				if (all)
				{
					list.Clear ();
				}

				return list;
			}
		}


		public static IEnumerable<VerboseDocumentOption> GetAll()
		{
			//	Retourne toutes les options existantes.
			return VerboseDocumentOption.allOptions;
		}

		public static IEnumerable<VerboseDocumentOption> GetDefault()
		{
			//	Retourne les options globales, utilisées avec le réglage des unités d'impression (PrintingUnitsTabPage).
			return VerboseDocumentOption.allOptions.Where (x => x.IsGlobal);
		}


		private static void BuildAll()
		{
			var list = new List<VerboseDocumentOption> ();
			string[] e, d;

			//	Ajoute les options d'impression liées au papier.
			list.Add (new VerboseDocumentOption ("Papier", "Paper"));

			e = new string[] { "Portrait", "Landscape" };
			d = new string[] { "Portrait", "Paysage" };
			list.Add (new VerboseDocumentOption (DocumentOption.Orientation, "Paper.1", true, e, d, "Orientation", 0));

			list.Add (new VerboseDocumentOption (DocumentOption.LeftMargin,   "Paper.2", true, DocumentOptionValueType.Distance, "Marge gauche",     "10"));
			list.Add (new VerboseDocumentOption (DocumentOption.RightMargin,  "Paper.2", true, DocumentOptionValueType.Distance, "Marge droite",     "10"));
			list.Add (new VerboseDocumentOption (DocumentOption.TopMargin,    "Paper.2", true, DocumentOptionValueType.Distance, "Marge supérieure", "10"));
			list.Add (new VerboseDocumentOption (DocumentOption.BottomMargin, "Paper.2", true, DocumentOptionValueType.Distance, "Marge inférieure", "10"));

			//	Ajoute les options d'impression générales.
			list.Add (new VerboseDocumentOption ("Options générales", "Global"));

			list.Add (new VerboseDocumentOption (DocumentOption.FontSize, "Global.1", true, DocumentOptionValueType.Size, "Taille de la police", "3"));

			list.Add (new VerboseDocumentOption (DocumentOption.Specimen,        "Global.2", true, DocumentOptionValueType.Boolean,  "Incruste la mention SPECIMEN",       "false"));

			list.Add (new VerboseDocumentOption (DocumentOption.Signing,         "Global.3", true, DocumentOptionValueType.Boolean,  "Cartouche pour visa avec signature", "true"));
			list.Add (new VerboseDocumentOption (DocumentOption.SigningFontSize, "Global.3", true, DocumentOptionValueType.Distance, "Cartouche, taille police",           "3"));

			//	Ajoute les options pour l'en-tête.
			list.Add (new VerboseDocumentOption ("En-tête", "Header"));

			list.Add (new VerboseDocumentOption (DocumentOption.HeaderLogo,            "Header.1", true, DocumentOptionValueType.Boolean, "Imprime le logo de l'entreprise", "true"));
																					   
			list.Add (new VerboseDocumentOption (DocumentOption.HeaderLogoLeft,        "Header.2", true, DocumentOptionValueType.Distance, "Logo, pos. gauche",               "10"));
			list.Add (new VerboseDocumentOption (DocumentOption.HeaderLogoTop,         "Header.2", true, DocumentOptionValueType.Distance, "Logo, pos. sup.",                 "10"));
			list.Add (new VerboseDocumentOption (DocumentOption.HeaderLogoWidth,       "Header.2", true, DocumentOptionValueType.Distance, "Logo, largeur",                   "60"));
			list.Add (new VerboseDocumentOption (DocumentOption.HeaderLogoHeight,      "Header.2", true, DocumentOptionValueType.Distance, "Logo, hauteur",                   "30"));
																					   
			list.Add (new VerboseDocumentOption (DocumentOption.HeaderFromLeft,        "Header.3", true, DocumentOptionValueType.Distance, "Entreprise, pos. gauche",         "10"));
			list.Add (new VerboseDocumentOption (DocumentOption.HeaderFromTop,         "Header.3", true, DocumentOptionValueType.Distance, "Entreprise, pos. sup.",           "40"));
			list.Add (new VerboseDocumentOption (DocumentOption.HeaderFromWidth,       "Header.3", true, DocumentOptionValueType.Distance, "Entreprise, largeur",             "100"));
			list.Add (new VerboseDocumentOption (DocumentOption.HeaderFromHeight,      "Header.3", true, DocumentOptionValueType.Distance, "Entreprise, hauteur",             "30"));
			list.Add (new VerboseDocumentOption (DocumentOption.HeaderFromFontSize,    "Header.3", true, DocumentOptionValueType.Size,     "Entreprise, taille police",       "3"));
																					   
			list.Add (new VerboseDocumentOption (DocumentOption.HeaderForLeft,         "Header.4", true, DocumentOptionValueType.Distance, "Concerne, pos. gauche",           "10"));
			list.Add (new VerboseDocumentOption (DocumentOption.HeaderForTop,          "Header.4", true, DocumentOptionValueType.Distance, "Concerne, pos. sup.",             "70"));
			list.Add (new VerboseDocumentOption (DocumentOption.HeaderForWidth,        "Header.4", true, DocumentOptionValueType.Distance, "Concerne, largeur",               "100"));
			list.Add (new VerboseDocumentOption (DocumentOption.HeaderForHeight,       "Header.4", true, DocumentOptionValueType.Distance, "Concerne, hauteur",               "12"));
			list.Add (new VerboseDocumentOption (DocumentOption.HeaderForFontSize,     "Header.4", true, DocumentOptionValueType.Size,     "Concerne, taille police",         "3"));
																					   
			list.Add (new VerboseDocumentOption (DocumentOption.HeaderNumberLeft,      "Header.5", true, DocumentOptionValueType.Distance, "Numéro document, pos. gauche",    "10"));
			list.Add (new VerboseDocumentOption (DocumentOption.HeaderNumberTop,       "Header.5", true, DocumentOptionValueType.Distance, "Numéro document, pos. sup.",      "80"));
			list.Add (new VerboseDocumentOption (DocumentOption.HeaderNumberWidth,     "Header.5", true, DocumentOptionValueType.Distance, "Numéro document, largeur",        "100"));
			list.Add (new VerboseDocumentOption (DocumentOption.HeaderNumberHeight,    "Header.5", true, DocumentOptionValueType.Distance, "Numéro document, hauteur",        "10"));
			list.Add (new VerboseDocumentOption (DocumentOption.HeaderNumberFontSize,  "Header.5", true, DocumentOptionValueType.Size,     "Numéro document, taille police",  "4.8"));
																					   
			list.Add (new VerboseDocumentOption (DocumentOption.HeaderToLeft,          "Header.6", true, DocumentOptionValueType.Distance, "Destinataire, pos. gauche",       "120"));
			list.Add (new VerboseDocumentOption (DocumentOption.HeaderToTop,           "Header.6", true, DocumentOptionValueType.Distance, "Destinataire, pos. sup.",         "50"));
			list.Add (new VerboseDocumentOption (DocumentOption.HeaderToWidth,         "Header.6", true, DocumentOptionValueType.Distance, "Destinataire, largeur",           "80"));
			list.Add (new VerboseDocumentOption (DocumentOption.HeaderToHeight,        "Header.6", true, DocumentOptionValueType.Distance, "Destinataire, hauteur",           "35"));
			list.Add (new VerboseDocumentOption (DocumentOption.HeaderToFontSize,      "Header.6", true, DocumentOptionValueType.Size,     "Destinataire, taille police",     "3"));
																					   
			list.Add (new VerboseDocumentOption (DocumentOption.HeaderLocDateLeft,     "Header.7", true, DocumentOptionValueType.Distance, "Localité et date, pos. gauche",   "120"));
			list.Add (new VerboseDocumentOption (DocumentOption.HeaderLocDateTop,      "Header.7", true, DocumentOptionValueType.Distance, "Localité et date, pos. sup.",     "80"));
			list.Add (new VerboseDocumentOption (DocumentOption.HeaderLocDateWidth,    "Header.7", true, DocumentOptionValueType.Distance, "Localité et date, largeur",       "80"));
			list.Add (new VerboseDocumentOption (DocumentOption.HeaderLocDateHeight,   "Header.7", true, DocumentOptionValueType.Distance, "Localité et date, hauteur",       "10"));
			list.Add (new VerboseDocumentOption (DocumentOption.HeaderLocDateFontSize, "Header.7", true, DocumentOptionValueType.Size,     "Localité et date, taille police", "3"));
			
			//	Ajoute les options pour les listes.
			list.Add (new VerboseDocumentOption ("Aspect des listes", "LayoutFrame"));

			list.Add (new VerboseDocumentOption (DocumentOption.TableTopAfterHeader, "LayoutFrame.1", true, DocumentOptionValueType.Distance, "Début après l'en-tête", "90"));
			list.Add (new VerboseDocumentOption (DocumentOption.TableFontSize,       "LayoutFrame.2", true, DocumentOptionValueType.Size,     "Liste, taille police",  "3"));

			e = new string[] { "Frameless", "WithLine", "WithFrame" };
			d = new string[] { "Espacé, sans encadrements", "Espacé, avec des lignes de séparation", "Serré, avec des encadrements" };
			list.Add (new VerboseDocumentOption (DocumentOption.LayoutFrame, "LayoutFrame.2", true, e, d, "Cadre", 1));

			list.Add (new VerboseDocumentOption (DocumentOption.GapBeforeGroup, "LayoutFrame.3", true, DocumentOptionValueType.Boolean, "Interligne supplémentaire entre les groupes", "true"));

			list.Add (new VerboseDocumentOption (DocumentOption.IndentWidth, "LayoutFrame.4", true, DocumentOptionValueType.Distance, "Longueur à indenter par niveau", "3"));

			//	Ajoute les options d'impression pour les documents commerciaux.
			list.Add (new VerboseDocumentOption ("Options pour les documents commerciaux", "BusinessDocument"));

			e = new string[] { "None", "Group", "Line", "Full" };
			d = new string[] { "Pas de numérotation", "Numérotation des groupes", "Numérotation plate des lignes", "Numérotation hiérarchique des lignes" };
			list.Add (new VerboseDocumentOption (DocumentOption.LineNumber, "BusinessDocument.1", true, e, d, "Numérotation", 0));

			list.Add (new VerboseDocumentOption (DocumentOption.ArticleAdditionalQuantities, "BusinessDocument.2", false, DocumentOptionValueType.Boolean, "Imprime les autres quantités", "true"));
			list.Add (new VerboseDocumentOption (DocumentOption.ArticleId,                   "BusinessDocument.2", false, DocumentOptionValueType.Boolean, "Imprime les identificateurs d'article", "false"));

			e = new string[] { "QD", "DQ" };
			d = new string[] { "Quantité, Désignation, Prix", "Désignation, Quantité, Prix" };
			list.Add (new VerboseDocumentOption (DocumentOption.ColumnsOrder, "BusinessDocument.3", false, e, d, "Ordre des colonnes", 0));

			//	Ajoute les options d'impression pour les factures.
			list.Add (new VerboseDocumentOption ("Options pour les factures", "Invoice"));

			e = new string[] { "Without", "WithInside", "WithOutside" };
			d = new string[] { "Facture sans BV", "Facture avec BV intégré", "Facture avec BV séparé" };
			list.Add (new VerboseDocumentOption (DocumentOption.IsrPosition, "Invoice.1", false, e, d, "Type de la facture", 0));

			e = new string[] { "Isr", "Is" };
			d = new string[] { "BV orange", "BV rose" };
			list.Add (new VerboseDocumentOption (DocumentOption.IsrType, "Invoice.2", false, e, d, "Type de bulletin de versement", 0));

			list.Add (new VerboseDocumentOption (DocumentOption.IsrFacsimile, "Invoice.3", true, DocumentOptionValueType.Boolean, "Fac-similé complet du BV", "true"));

			//	Ajoute les options pour les clients.
			list.Add (new VerboseDocumentOption ("Fiche résumée d'un client", "Relation"));

			list.Add (new VerboseDocumentOption (DocumentOption.RelationMail,    "Relation.1", false, DocumentOptionValueType.Boolean,  "Inclure les adresses",   "true"));
			list.Add (new VerboseDocumentOption (DocumentOption.RelationTelecom, "Relation.1", false, DocumentOptionValueType.Boolean,  "Inclure les téléphones", "true"));
			list.Add (new VerboseDocumentOption (DocumentOption.RelationUri,     "Relation.1", false, DocumentOptionValueType.Boolean,  "Inclure les emails",     "true"));

			VerboseDocumentOption.allOptions = list;
		}


		private static IEnumerable<VerboseDocumentOption> allOptions;
	}
}
