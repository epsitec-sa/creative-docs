//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.Printing;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Documents;
using Epsitec.Cresus.Core.Documents.Verbose;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Print.Serialization;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Data;

using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core.Print
{
	public static class PrintEngine
	{
		static PrintEngine()
		{
			PrintEngine.RegisterFonts ();
		}

		public static void Setup()
		{
			//	Force execution of static constructor.
		}


		#region Command handlers
		public static void PrintCommand(IBusinessContext businessContext, AbstractEntity entity)
		{
			//	La commande 'Print' du ruban a été activée.
			PrintEngine.PrintOrPreviewCommand (businessContext, entity, isPreview: false);
		}

		public static void PreviewCommand(IBusinessContext businessContext, AbstractEntity entity)
		{
			//	La commande 'Preview' du ruban a été activée.
			PrintEngine.PrintOrPreviewCommand (businessContext, entity, isPreview: true);
		}


		private static void PrintOrPreviewCommand(IBusinessContext businessContext, AbstractEntity entity, bool isPreview)
		{
			if (entity == null)
			{
				var message = "Il n'y a aucune donnée à imprimer.";
				MessageDialog.CreateOk ("Erreur", DialogIcon.Warning, message).OpenDialog ();
				return;
			}

			//	Prépare les entités à imprimer.
			var entitiesToPrint = PrintEngine.GetEntitiesToPrint (businessContext, entity);

			//	Choix des options et des pages à imprimer.
			var optionsDialog = new Dialogs.PrintOptionsDialog (businessContext, entitiesToPrint, isPreview);
			optionsDialog.IsModal = true;
			optionsDialog.OpenDialog ();

			if (optionsDialog.Result != DialogResult.Accept)
			{
				return;
			}

			//	Impression effective, sans aucune interaction.
			var deserializeJobs = optionsDialog.DeserializeJobs;
			if (deserializeJobs.Count != 0)
			{
				PrintEngine.RemoveUnprintablePages (deserializeJobs);
				PrintEngine.PrintJobs (businessContext, deserializeJobs);
			}
		}

		private static IEnumerable<EntityToPrint> GetEntitiesToPrint(IBusinessContext businessContext, AbstractEntity entity)
		{
			//	Prépare les entités à imprimer. Une entité peut déboucher sur plusieurs entités
			//	filles à imprimer (par exemple sous forme d'étiquettes).
			var entitiesToPrint = new List<EntityToPrint> ();

			PrintEngine.AddEntityToPrint (businessContext, entitiesToPrint, entity, "Document principal");

			// TODO: DR
#if false
			if (entity is DocumentMetadataEntity)
			{
				//	S'il s'agit d'une facture, on cherche l'adresse de livraison pour en imprimer une étiquette.
				var meta = entity as DocumentMetadataEntity;

				if (meta.BusinessDocument != null && meta.BusinessDocument.BillToMailContact != null)
				{
					PrintEngine.AddEntityToPrint (businessContext, entitiesToPrint, meta.BusinessDocument.BillToMailContact, "Etiquette adresse de livraison");
				}
			}

			if (entity is RelationEntity)
			{
				//	S'il s'agit d'une relation (personne physique ou morale), on cherche toutes les adresses liées
				//	pour en faire des étiquettes.
				var relation = entity as RelationEntity;

				int rank = 0;
				foreach (var contact in relation.Person.Contacts)
				{
					if (contact is MailContactEntity)
					{
						PrintEngine.AddEntityToPrint (businessContext, entitiesToPrint, contact, string.Format ("Etiquette adresse n° {0}", (++rank).ToString ()));
					}
				}
			}
#endif

			return entitiesToPrint;
		}

		private static void AddEntityToPrint(IBusinessContext businessContext, List<EntityToPrint> entitiesToPrint, AbstractEntity entity, string title)
		{
			var options       = PrintEngine.GetOptions       (businessContext, entity);
			var printingUnits = PrintEngine.GetPrintingUnits (businessContext, entity);

			entitiesToPrint.Add (new EntityToPrint (entity, options, printingUnits, title));
		}

		private static void RemoveUnprintablePages(List<DeserializedJob> jobs)
		{
			for (int i = 0; i < jobs.Count; i++)
			{
				jobs[i].RemoveUnprintablePages ();
			}
		}
		#endregion


		public static void SendDataToPrinter(IBusinessContext businessContext, string xml)
		{
			//	Imprime effectivement le source xml d'un document.
			var deserializeJobs = SerializationEngine.DeserializeJobs (businessContext, xml);
			PrintEngine.PrintJobs (businessContext, deserializeJobs);
		}

		private static void PrintJobs(IBusinessContext businessContext, List<DeserializedJob> jobs)
		{
			//	Imprime effectivement une liste de jobs d'impression.
			foreach (var job in jobs)
			{
				if (job.PrintablePagesCount != 0)
				{
					PrintDocument printDocument = new PrintDocument ();

					printDocument.DocumentName = job.JobFullName;
					printDocument.SelectPrinter (FormattedText.Unescape (job.PrinterPhysicalName));
					printDocument.PrinterSettings.Copies = 1;
					printDocument.DefaultPageSettings.Margins = new Margins (0, 0, 0, 0);

					var engine = new XmlJobPrintEngine (businessContext, printDocument, job.Sections);
					printDocument.Print (engine);
				}
			}
		}


		private static string MakePrintingData(IBusinessContext businessContext, IEnumerable<EntityToPrint> entitiesToPrint)
		{
			//	Fabrique les données permettant d'imprimer un document, sans aucune interaction.
			//	Retourne le source xml correspondant.
			var jobs = new List<JobToPrint> ();

			foreach (var entityToPrint in entitiesToPrint)
			{
				var job = PrintEngine.MakePrintingJobs (businessContext, entityToPrint.Entity, entityToPrint.Options, entityToPrint.PrintingUnits);
				jobs.AddRange (job);
			}

			return SerializationEngine.SerializeJobs (jobs);
		}

		public static string MakePrintingData(IBusinessContext businessContext, AbstractEntity entity, PrintingOptionDictionary options, PrintingUnitDictionary printingUnits)
		{
			//	Fabrique les données permettant d'imprimer un document, sans aucune interaction.
			//	Retourne le source xml correspondant.
			var jobs = PrintEngine.MakePrintingJobs (businessContext, entity, options, printingUnits);
			return SerializationEngine.SerializeJobs (jobs);
		}

		private static List<JobToPrint> MakePrintingJobs(IBusinessContext businessContext, AbstractEntity entity, PrintingOptionDictionary options, PrintingUnitDictionary printingUnits)
		{
			//	Fabrique les données permettant d'imprimer un document, sans aucune interaction.
			//	Retourne les jobs correspondant.
			System.Diagnostics.Debug.Assert (businessContext != null);
			System.Diagnostics.Debug.Assert (entity != null);
			System.Diagnostics.Debug.Assert (options != null);
			System.Diagnostics.Debug.Assert (printingUnits != null);

			//	Crée les classes qui savent imprimer l'entité qui représente le document (EntityPrinters.AbstrctPrinter).
			var documentPrinter = EntityPrinters.AbstractPrinter.CreateDocumentPrinter (businessContext, entity, options, printingUnits);

			if (documentPrinter == null || printingUnits.Count == 0)
			{
				return null;
			}

			//	Prépare toutes les pages à imprimer, pour toutes les entités.
			//	On crée autant de sections que de pages, soit une section par page.
			var sections = new List<SectionToPrint> ();
			var pageTypes = VerbosePageType.GetAll ();

			int documentPrinterRank = 0;
			foreach (var pair in printingUnits.ContentPair)
			{
				var pageType         = pair.Key;
				var printingUnitName = pair.Value;

				var printingUnit = Common.GetPrintingUnit (businessContext.Data.Host, printingUnitName);

				if (printingUnit != null && printingUnit.PageTypes.Contains (pageType))
				{
					//	Fabrique le dictionnaire des options à partir des options de base et
					//	des options définies avec l'unité d'impression.
					var customOptions = new PrintingOptionDictionary ();
					customOptions.MergeWith (options);						// options les moins prioritaires
					customOptions.MergeWith (printingUnit.Options);			// options les plus prioritaires

					documentPrinter.SetPrintingUnit (printingUnit, customOptions, PreviewMode.Print);
					documentPrinter.BuildSections ();

					if (!documentPrinter.IsEmpty (pageType))
					{
						var jobName = pageTypes.Where (x => x.Type == pageType).Select (x => x.Job).FirstOrDefault ();

						for (int copy = 0; copy < printingUnit.Copies; copy++)
						{
							var physicalPages = documentPrinter.GetPhysicalPages (pageType);
							foreach (var physicalPage in physicalPages)
							{
								string p = (documentPrinterRank+1).ToString ();
								string d = (documentPrinter.GetDocumentRank (physicalPage)+1).ToString ();
								string c = (copy+1).ToString ();
								string internalJobName = string.Concat (jobName, ".", p, ".", d, ".", c);

								sections.Add (new SectionToPrint (printingUnit, internalJobName, physicalPage, documentPrinter, documentPrinter.RequiredPageSize, customOptions));
							}
						}
					}

					documentPrinterRank++;
				}
			}

			//	Trie toutes les pages, qui sont regroupées logiquement par jobs et par imprimantes physiques.
			sections.Sort (SectionToPrint.CompareSectionToPrint);

			//	Sépare les copies multiples de pages identiques sur une même imprimante.
			//	Par exemple: 0,0,0,1,1,1,2,2,2 devient 0,1,2,0,1,2,0,1,2
			int index = 0;
			while (index < sections.Count-1)
			{
				SectionToPrint p1 = sections[index];
				SectionToPrint p2 = sections[index+1];

				if (p1.InternalJobName                  == p2.InternalJobName                  &&
					p1.PrintingUnit.PhysicalPrinterName == p2.PrintingUnit.PhysicalPrinterName &&
					p1.FirstPage                        == p2.FirstPage)
				{
					sections.RemoveAt (index+1);  // supprime p2...
					sections.Add (p2);            // ...puis remet-la à la fin
				}
				else
				{
					index++;
				}
			}

			//	Fusionne toutes les pages contigües qui font partie du même job en sections de plusieurs pages.
			//	Par exemple: 0,1,2,0,1,2,0,1,2 devient 0..2, 0..2, 0..2
			index = 0;
			while (index < sections.Count-1)
			{
				SectionToPrint p1 = sections[index];
				SectionToPrint p2 = sections[index+1];

				if (p1.InternalJobName          == p2.InternalJobName &&
					p1.PrintingUnit             == p2.PrintingUnit    &&
					p1.FirstPage + p1.PageCount == p2.FirstPage)
				{
					p1.PageCount += p2.PageCount;  // ajoute les pages de p2 à p1
					sections.RemoveAt (index+1);   // supprime p2
				}
				else
				{
					index++;
				}
			}

			//	Fusionne toutes les sections en jobs. Un job est un ensemble de sections utilisant la
			//	même imprimante physique, mais pas forcément le même bac.
			var jobs = new List<JobToPrint> ();

			foreach (var page in sections)
			{
				var job = new JobToPrint ();
				job.Sections.Add (page);

				jobs.Add (job);
			}

			index = 0;
			while (index < jobs.Count-1)
			{
				JobToPrint j1 = jobs[index];
				JobToPrint j2 = jobs[index+1];

				if (j1.InternalJobName     == j2.InternalJobName     &&
					j1.PrinterPhysicalName == j2.PrinterPhysicalName)
				{
					j1.Sections.AddRange (j2.Sections);  // ajoute les sections de j2 à j1
					jobs.RemoveAt (index+1);             // supprime j2
				}
				else
				{
					index++;
				}
			}

			//	Nomme tous les jobs.
			index = 0;
			foreach (var job in jobs)
			{
				job.JobFullName = string.Concat (job.Sections[0].DocumentPrinter.JobName, ".", (index+1).ToString ());
				index++;
			}

			return jobs;
		}
		
		
		#region Dictionary getters
		private static PrintingOptionDictionary GetOptions(IBusinessContext businessContext, AbstractEntity entity)
		{
			//	Retourne les options à utiliser pour l'entité.
			var result = new PrintingOptionDictionary ();

			var categories = PrintEngine.GetDocumentCategoryEntities(businessContext, entity);
			if (categories != null)
			{
				foreach (var category in categories)
				{
					if (category.DocumentOptions != null)
					{
						foreach (var documentOptions in category.DocumentOptions)
						{
							var option = documentOptions.GetOptions ();
							result.MergeWith (option);
						}
					}
				}
			}

#if false
			if (result.Count == 0)  // TODO: Hack à supprimer dès que possible !
			{
				result = OptionsDictionary.GetDefault ();
				result.Add (DocumentOption.EsrPosition, "WithInside");  // force un BV
			}
#endif

			return result;
		}

		private static PrintingUnitDictionary GetPrintingUnits(IBusinessContext businessContext, AbstractEntity entity)
		{
			//	Retourne les unités d'impression à utiliser pour l'entité.
			var result = new PrintingUnitDictionary ();

			var categories = PrintEngine.GetDocumentCategoryEntities (businessContext, entity);
			if (categories != null)
			{
				foreach (var category in categories)
				{
					if (category.DocumentOptions != null)
					{
						foreach (var printingUnits in category.DocumentPrintingUnits)
						{
							var printingUnit = printingUnits.GetPrintingUnits ();
							result.MergeWith (printingUnit);
						}
					}
				}
			}

#if true
			if (result.Count == 0)  // TODO: Hack à supprimer dès que possible !
			{
				result[PageType.All] = "Blanc";
				//?result.Add (PageType.Esr, "Jaune");
				//?result.Add (PageType.Copy, "Brouillon");
				result[PageType.Label] = "Etiquettes";
			}
#endif

			return result;
		}

		private static IEnumerable<DocumentCategoryEntity> GetDocumentCategoryEntities(IBusinessContext businessContext, AbstractEntity entity)
		{
			//	Retourne les entités de catégorie de document pour une entité représentant un document quelconque.
			if (entity is DocumentMetadataEntity)
			{
				var metadata = entity as DocumentMetadataEntity;

				if (metadata.DocumentCategory != null)
				{
					var list = new List<DocumentCategoryEntity> ();
					list.Add (metadata.DocumentCategory);
					return list;
				}
			}

			DocumentType type = DocumentType.Unknown;

#if true
			if (entity is DocumentMetadataEntity)  // TODO: Hack à supprimer dès que possible !
			{
				type = DocumentType.Invoice;
			}
#endif

			// TODO: DR
#if false
			if (entity is ArticleDefinitionEntity)
			{
				type = DocumentType.ArticleDefinitionSummary;
			}

			if (entity is RelationEntity)
			{
				type = DocumentType.RelationSummary;
			}

			if (entity is MailContactEntity)
			{
				type = DocumentType.MailContactLabel;
			}
#endif

			if (type != DocumentType.Unknown)
			{

				DocumentCategoryEntity example = new DocumentCategoryEntity ();
				example.DocumentType = type;

				businessContext.DataContext.GetByExample<DocumentCategoryEntity> (example);
			}

			return null;
		}
		#endregion


		public static Image GetImage(IBusinessContext businessContext, string id)
		{
			//	Retrouve l'image dans la base de données, à partir de son identificateur (ImageBlobEntity.Code).
			var store = businessContext.Data.GetComponent<ImageDataStore> ();

			if (store == null)
			{
				return null;
			}

			var data = store.GetImageData (id);

			if (data == null)
			{
				return null;
			}

			return data.GetImage ();
		}

	
		private static void RegisterFonts()
		{
			using (var stream = System.Reflection.Assembly.GetExecutingAssembly ().GetManifestResourceStream ("Epsitec.Creus.Core.Resources.OCR_BB.tff"))
			{
				Font.RegisterDynamicFont (stream);
			}
		}
	}
}
