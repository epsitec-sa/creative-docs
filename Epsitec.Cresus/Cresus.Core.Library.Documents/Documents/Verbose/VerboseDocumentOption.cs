//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Business;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Resolvers;

namespace Epsitec.Cresus.Core.Documents.Verbose
{
	public sealed class VerboseDocumentOption
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

		private VerboseDocumentOption(DocumentOption option, string group, bool isGlobal, DocumentOptionValueType type, string shortDescription, string longDescription, string defaultValue)
		{
			this.Option           = option;
			this.Group            = group;
			this.IsGlobal         = isGlobal;
			this.Type             = type;
			this.IsBoolean        = false;
			this.ShortDescription = shortDescription;
			this.LongDescription  = longDescription;
			this.DefaultValue     = defaultValue;
		}

		private VerboseDocumentOption(DocumentOption option, string group, bool isGlobal, string shortDescription, string longDescription, int defaultIndex)
		{
			var enumeration = new List<string> ();
			enumeration.Add ("false");
			enumeration.Add ("true");

			var enumerationDescription = new List<string> ();
			enumerationDescription.Add ("Non");
			enumerationDescription.Add ("Oui");

			this.Option                 = option;
			this.Group                  = group;
			this.IsGlobal               = isGlobal;
			this.Type                   = DocumentOptionValueType.Enumeration;
			this.IsBoolean              = true;
			this.Enumeration            = enumeration;
			this.EnumerationDescription = enumerationDescription;
			this.ShortDescription       = shortDescription;
			this.LongDescription        = longDescription;
			this.DefaultValue           = enumeration.ElementAt (defaultIndex);
		}

		private VerboseDocumentOption(DocumentOption option, string group, bool isGlobal, IEnumerable<string> enumeration, IEnumerable<string> enumerationDescription, string shortDescription, string longDescription, int defaultIndex)
		{
			System.Diagnostics.Debug.Assert (enumeration != null && enumerationDescription != null);
			System.Diagnostics.Debug.Assert (enumeration.Count () == enumerationDescription.Count ());

			this.Option                 = option;
			this.Group                  = group;
			this.IsGlobal               = isGlobal;
			this.Type                   = DocumentOptionValueType.Enumeration;
			this.IsBoolean              = false;
			this.Enumeration            = enumeration;
			this.EnumerationDescription = enumerationDescription;
			this.ShortDescription       = shortDescription;
			this.LongDescription        = longDescription;
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

		public string SimiliType
		{
			get
			{
				if (this.IsBoolean)
				{
					return "bool";
				}
				else
				{
					switch (this.Type)
					{
						case DocumentOptionValueType.Enumeration:
							return "enum";

						case DocumentOptionValueType.Distance:
							return "distance";

						case DocumentOptionValueType.Size:
							return "size";

						case DocumentOptionValueType.Text:
							return "text";

						case DocumentOptionValueType.TextMultiline:
							return "textMultiline";

						default:
							return "undefined";
					}
				}
			}
		}

		public IEnumerable<string> Enumeration
		{
			get;
			private set;
		}

		public bool IsBoolean
		{
			get;
			private set;
		}

		public string Group
		{
			get;
			private set;
		}

		public string ShortDescription
		{
			get;
			private set;
		}

		public string LongDescription
		{
			get;
			private set;
		}

		public void SetTooltip(Widget widget)
		{
			if (!string.IsNullOrEmpty(this.LongDescription) && this.LongDescription != this.ShortDescription)
			{
				ToolTip.Default.SetToolTip (widget, this.LongDescription);
			}
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

				foreach (DocumentType documentType in EnumType.GetAllEnumValues<DocumentType> ())
				{
					if (documentType == DocumentType.None   ||
						documentType == DocumentType.Unknown)
					{
						continue;
					}

					var options = EntityPrinterFactoryResolver.FindRequiredDocumentOptions (documentType);

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
			list.Add (new VerboseDocumentOption (DocumentOption.Orientation, "Paper.1", true, e, d, "Orientation", "Orientation du papier", 0));

			list.Add (new VerboseDocumentOption (DocumentOption.LeftMargin,   "Paper.2", true, DocumentOptionValueType.Distance, "Marge gauche",     null, "10"));
			list.Add (new VerboseDocumentOption (DocumentOption.RightMargin,  "Paper.2", true, DocumentOptionValueType.Distance, "Marge droite",     null, "10"));
			list.Add (new VerboseDocumentOption (DocumentOption.TopMargin,    "Paper.2", true, DocumentOptionValueType.Distance, "Marge supérieure", null, "10"));
			list.Add (new VerboseDocumentOption (DocumentOption.BottomMargin, "Paper.2", true, DocumentOptionValueType.Distance, "Marge inférieure", null, "10"));

			//	Ajoute les options d'impression générales.
			list.Add (new VerboseDocumentOption ("Options générales", "Global"));

			list.Add (new VerboseDocumentOption (DocumentOption.FontSize, "Global.1", true, DocumentOptionValueType.Size, "Taille de la police", "Taille de la police par défaut", "3"));

			list.Add (new VerboseDocumentOption (DocumentOption.Specimen, "Global.2", true, "Incruste la mention SPECIMEN", "Incruste la mention SPECIMEN en grand, au travers de la page", 0));

			list.Add (new VerboseDocumentOption (DocumentOption.Signing,         "Global.3", true,  "Cartouche pour visa avec signature", "Cartouche pour visa avec signature au bas de la page", 1));
			list.Add (new VerboseDocumentOption (DocumentOption.SigningFontSize, "Global.3", true, DocumentOptionValueType.Distance, "Cartouche, taille police", "Taille de la police du cartouche au bas de la page", "3"));

			e = new string[] { "default", "fr", "de", "en", "ot" };
			d = new string[] { "Par défaut", "Français", "Allemand", "Anglais", "Italien" };
			list.Add (new VerboseDocumentOption (DocumentOption.Language, "Global.4", true, e, d, "Langue", "Langue dans laquelle sera imprimé le document", 0));

			//	Ajoute les options pour l'en-tête.
			list.Add (new VerboseDocumentOption ("En-tête", "Header"));

			list.Add (new VerboseDocumentOption (DocumentOption.HeaderSender,             "Header.1", true, "Imprime le bloc de l'expéditeur", "Imprime le bloc de l'expéditeur comprenant le logo et l'adresse de l'entreprise", 1));
																					      
			list.Add (new VerboseDocumentOption (DocumentOption.HeaderLogoLeft,           "Header.2", true, DocumentOptionValueType.Distance, "Logo, pos. gauche",                      "Position depuis la gauche du logo", "10"));
			list.Add (new VerboseDocumentOption (DocumentOption.HeaderLogoTop,            "Header.2", true, DocumentOptionValueType.Distance, "Logo, pos. sup.",                        "Position depuis le haut du logo",   "10"));
			list.Add (new VerboseDocumentOption (DocumentOption.HeaderLogoWidth,          "Header.2", true, DocumentOptionValueType.Distance, "Logo, largeur",                          "Largeur du logo",                   "60"));
			list.Add (new VerboseDocumentOption (DocumentOption.HeaderLogoHeight,         "Header.2", true, DocumentOptionValueType.Distance, "Logo, hauteur",                          "Hauteur du logo",                   "30"));
																					      																						        
			list.Add (new VerboseDocumentOption (DocumentOption.HeaderFromAddress,        "Header.3", true, DocumentOptionValueType.TextMultiline, "Adresse de l'entreprise",           "Adresse de l'entreprise telle qu'elle apparaît dans l'en-tête", ""));
			list.Add (new VerboseDocumentOption (DocumentOption.HeaderFromLeft,           "Header.3", true, DocumentOptionValueType.Distance, "Entreprise, pos. gauche",                "Position depuis la gauche de l'adresse de l'entreprise", "10"));
			list.Add (new VerboseDocumentOption (DocumentOption.HeaderFromTop,            "Header.3", true, DocumentOptionValueType.Distance, "Entreprise, pos. sup.",                  "Position depuis le haut de l'adresse de l'entreprise",   "40"));
			list.Add (new VerboseDocumentOption (DocumentOption.HeaderFromWidth,          "Header.3", true, DocumentOptionValueType.Distance, "Entreprise, largeur",                    "Largeur pour l'adresse de l'entreprise",                 "100"));
			list.Add (new VerboseDocumentOption (DocumentOption.HeaderFromHeight,         "Header.3", true, DocumentOptionValueType.Distance, "Entreprise, hauteur",                    "Hauteur pour l'adresse de l'entreprise",                 "30"));
			list.Add (new VerboseDocumentOption (DocumentOption.HeaderFromFontSize,       "Header.3", true, DocumentOptionValueType.Size,     "Entreprise, taille police",              "Taille de la police de l'adresse de l'entreprise",       "3"));
																					      																						        
			list.Add (new VerboseDocumentOption (DocumentOption.HeaderForLeft,            "Header.4", true, DocumentOptionValueType.Distance, "Concerne, pos. gauche",                  "Position depuis la gauche du bloc \"concerne\"", "10"));
			list.Add (new VerboseDocumentOption (DocumentOption.HeaderForTop,             "Header.4", true, DocumentOptionValueType.Distance, "Concerne, pos. sup.",                    "Position depuis le haut du bloc \"concerne\"",   "70"));
			list.Add (new VerboseDocumentOption (DocumentOption.HeaderForWidth,           "Header.4", true, DocumentOptionValueType.Distance, "Concerne, largeur",                      "Largeur du bloc \"concerne\"",                   "100"));
			list.Add (new VerboseDocumentOption (DocumentOption.HeaderForHeight,          "Header.4", true, DocumentOptionValueType.Distance, "Concerne, hauteur",                      "Hauteur du bloc \"concerne\"",                   "12"));
			list.Add (new VerboseDocumentOption (DocumentOption.HeaderForFontSize,        "Header.4", true, DocumentOptionValueType.Size,     "Concerne, taille police",                "Taille de la police du bloc \"concerne\"",       "3"));
																					      																						        
			list.Add (new VerboseDocumentOption (DocumentOption.HeaderNumberLeft,         "Header.5", true, DocumentOptionValueType.Distance, "Numéro document, pos. gauche",           "Position depuis la gauche du numéro du document", "10"));
			list.Add (new VerboseDocumentOption (DocumentOption.HeaderNumberTop,          "Header.5", true, DocumentOptionValueType.Distance, "Numéro document, pos. sup.",             "Position depuis le haut du numéro du document",   "80"));
			list.Add (new VerboseDocumentOption (DocumentOption.HeaderNumberWidth,        "Header.5", true, DocumentOptionValueType.Distance, "Numéro document, largeur",               "Largeur pour le numéro du document",              "100"));
			list.Add (new VerboseDocumentOption (DocumentOption.HeaderNumberHeight,       "Header.5", true, DocumentOptionValueType.Distance, "Numéro document, hauteur",               "Hauteur pour le numéro du document",              "10"));
			list.Add (new VerboseDocumentOption (DocumentOption.HeaderNumberFontSize,     "Header.5", true, DocumentOptionValueType.Size,     "Numéro document, taille police",         "Taille de la police du numéro du document",       "4.8"));
			list.Add (new VerboseDocumentOption (DocumentOption.HeaderNumberIncludeOwner, "Header.5", true,                                   "Numéro document, inclure collaborateur", "Inclure le numéro du collaborateur propriétaire de l'affaire", 1));
																					      																						        
			list.Add (new VerboseDocumentOption (DocumentOption.HeaderToLeft,             "Header.6", true, DocumentOptionValueType.Distance, "Destinataire, pos. gauche",              "Position depuis la gauche de l'adresse du destinataire", "120"));
			list.Add (new VerboseDocumentOption (DocumentOption.HeaderToTop,              "Header.6", true, DocumentOptionValueType.Distance, "Destinataire, pos. sup.",                "Position depuis le haut de l'adresse du destinataire",   "50"));
			list.Add (new VerboseDocumentOption (DocumentOption.HeaderToWidth,            "Header.6", true, DocumentOptionValueType.Distance, "Destinataire, largeur",                  "Largeur pour l'adresse du destinataire",                 "80"));
			list.Add (new VerboseDocumentOption (DocumentOption.HeaderToHeight,           "Header.6", true, DocumentOptionValueType.Distance, "Destinataire, hauteur",                  "Hauteur pour l'adresse du destinataire",                 "35"));
			list.Add (new VerboseDocumentOption (DocumentOption.HeaderToFontSize,         "Header.6", true, DocumentOptionValueType.Size,     "Destinataire, taille police",            "Taille de la police de l'adresse du destinataire",       "3"));
																					      																						        
			list.Add (new VerboseDocumentOption (DocumentOption.HeaderLocDateLeft,        "Header.7", true, DocumentOptionValueType.Distance, "Localité et date, pos. gauche",          "Position depuis la gauche du bloc \"localité et date\"", "120"));
			list.Add (new VerboseDocumentOption (DocumentOption.HeaderLocDateTop,         "Header.7", true, DocumentOptionValueType.Distance, "Localité et date, pos. sup.",            "Position depuis le haut du bloc \"localité et date\"",   "80"));
			list.Add (new VerboseDocumentOption (DocumentOption.HeaderLocDateWidth,       "Header.7", true, DocumentOptionValueType.Distance, "Localité et date, largeur",              "Largeur du bloc \"localité et date\"",                   "80"));
			list.Add (new VerboseDocumentOption (DocumentOption.HeaderLocDateHeight,      "Header.7", true, DocumentOptionValueType.Distance, "Localité et date, hauteur",              "Hauteur du bloc \"localité et date\"",                   "10"));
			list.Add (new VerboseDocumentOption (DocumentOption.HeaderLocDateFontSize,    "Header.7", true, DocumentOptionValueType.Size,     "Localité et date, taille police",        "Taille de la police du bloc \"localité et date\"",       "3"));

			//	Ajoute les options pour le pied de page.
			list.Add (new VerboseDocumentOption ("Pied de page", "Footer"));

			list.Add (new VerboseDocumentOption (DocumentOption.FooterTextFontSize, "Footer.1", true, DocumentOptionValueType.Size, "Texte, taille police", "Taille de la police du texte de pied de page", "3"));
			
			//	Ajoute les options pour les listes.
			list.Add (new VerboseDocumentOption ("Aspect des listes", "LayoutFrame"));

			list.Add (new VerboseDocumentOption (DocumentOption.TableTopAfterHeader, "LayoutFrame.1", true, DocumentOptionValueType.Distance, "Début après l'en-tête", "Position depuis le haut du début de les listes, sur la première page, après l'en-tête", "90"));
			list.Add (new VerboseDocumentOption (DocumentOption.TableFontSize,       "LayoutFrame.2", true, DocumentOptionValueType.Size,     "Liste, taille police",  "Taille de la police dans les listes", "3"));

			e = new string[] { "Frameless", "WithLine", "WithFrame" };
			d = new string[] { "Espacé, sans encadrements", "Espacé, avec des lignes de séparation", "Serré, avec des encadrements" };
			list.Add (new VerboseDocumentOption (DocumentOption.LayoutFrame, "LayoutFrame.2", true, e, d, "Cadre", "Type du cadre des listes", 1));

			list.Add (new VerboseDocumentOption (DocumentOption.GapBeforeGroup, "LayoutFrame.3", true, "Interligne supplémentaire entre les groupes", "Interligne supplémentaire entre les groupes, dans les listes", 1));

			list.Add (new VerboseDocumentOption (DocumentOption.IndentWidth, "LayoutFrame.4", true, DocumentOptionValueType.Distance, "Longueur à indenter par niveau", "Longueur à indenter par niveau, dans les listes", "3"));

			//	Ajoute les options d'impression pour les documents commerciaux.
			list.Add (new VerboseDocumentOption ("Options pour les documents commerciaux", "BusinessDocument"));

			e = new string[] { "None", "Group", "Line", "Full" };
			d = new string[] { "Pas de numérotation", "Numérotation des groupes", "Numérotation plate des lignes", "Numérotation hiérarchique des lignes" };
			list.Add (new VerboseDocumentOption (DocumentOption.LineNumber, "BusinessDocument.1", true, e, d, "Numérotation", "Type de numérotatioh dans les listes", 0));

			e = new string[] { "None", "Separate", "ToQuantity", "ToDescription", "ToQuantityAndDescription" };
			d = new string[] { "N'imprime pas les autres quantités", "Autres quantités dans une colonne spécifique", "Toutes les quantités ensembles", "Autres quantités avec les désignations", "Autres quantités réparties" };
			list.Add (new VerboseDocumentOption (DocumentOption.ArticleAdditionalQuantities, "BusinessDocument.2", false, e, d, "Imprime les autres quantités", null, 0));

			list.Add (new VerboseDocumentOption (DocumentOption.ArticleId, "BusinessDocument.2", false, "Imprime les identificateurs d'article", "Imprime les identificateurs ou numéros d'article", 0));

			e = new string[] { "QD", "DQ" };
			d = new string[] { "Quantité, Désignation, Prix", "Désignation, Quantité, Prix" };
			list.Add (new VerboseDocumentOption (DocumentOption.ColumnsOrder, "BusinessDocument.3", false, e, d, "Ordre des colonnes", "Choix de l'ordre des colonnes dans les listes", 0));

			//	Ajoute les options d'impression pour les factures.
			list.Add (new VerboseDocumentOption ("Options pour les factures", "Invoice"));

			e = new string[] { "Without", "WithInside", "WithOutside" };
			d = new string[] { "Facture sans BV", "Facture avec BV intégré", "Facture avec BV séparé" };
			list.Add (new VerboseDocumentOption (DocumentOption.IsrPosition, "Invoice.1", false, e, d, "Type de la facture", null, 0));

			e = new string[] { "Isr", "Is" };
			d = new string[] { "BV orange", "BV rose" };
			list.Add (new VerboseDocumentOption (DocumentOption.IsrType, "Invoice.2", false, e, d, "Type de bulletin de versement", null, 0));

			list.Add (new VerboseDocumentOption (DocumentOption.IsrFacsimile, "Invoice.3", true, "Fac-similé complet du BV", "Fac-similé complet du BV. Lorsqu'on utilise du papier avec BV préimprimé, cette option ne doit pas être cochée.", 1));

			//	Ajoute les options pour les clients.
			list.Add (new VerboseDocumentOption ("Fiche résumée d'un client", "Relation"));

			list.Add (new VerboseDocumentOption (DocumentOption.RelationMail,    "Relation.1", false, "Inclure les adresses",   "Inclure les adresses postales",      1));
			list.Add (new VerboseDocumentOption (DocumentOption.RelationTelecom, "Relation.1", false, "Inclure les téléphones", "Inclure les numéros de téléphones",  1));
			list.Add (new VerboseDocumentOption (DocumentOption.RelationUri,     "Relation.1", false, "Inclure les emails",     "Inclure les adresses électroniques", 1));

			VerboseDocumentOption.allOptions = list;
		}


		private static IEnumerable<VerboseDocumentOption> allOptions;
	}
}
