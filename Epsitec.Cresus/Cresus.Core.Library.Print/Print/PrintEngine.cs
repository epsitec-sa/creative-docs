﻿//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
			var deserializedJobs = optionsDialog.GetJobs ();
			if (deserializedJobs.Count > 0)
			{
				PrintEngine.RecordXmlSources (businessContext, entity, optionsDialog.GetXmlSources ());
				//?PrintEngine.RemoveUnprintablePages (deserializedJobs);
				//?PrintEngine.PrintJobs (businessContext, deserializedJobs);
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

		private static void RemoveUnprintablePages(IEnumerable<DeserializedJob> jobs)
		{
			jobs.ForEach (x => x.RemoveUnprintablePages ());
		}
		#endregion


		#region xml source recording
		private static void RecordXmlSources(IBusinessContext businessContext, AbstractEntity entity, IList<string> xmlSources)
		{
			//	Stocke xml source d'une impression dans l'entité DocumentMetadataEntity.
			var documentMetadata = entity as DocumentMetadataEntity;

			if (documentMetadata != null)
			{
				// TODO: Pour tester, on stocke tout !
				//?if (documentMetadata.DocumentState != DocumentState.Draft)  // on ne stocke pas les brouillons
				{
					foreach (var xmlSource in xmlSources)
					{
						PrintEngine.RecordXmlSource (businessContext, documentMetadata, xmlSource);
					}
				}
			}
		}

		private static void RecordXmlSource(IBusinessContext businessContext, DocumentMetadataEntity documentMetadata, string xmlSource)
		{
			byte[] data = Epsitec.Common.IO.Serialization.SerializeAndCompressToMemory(xmlSource, Epsitec.Common.IO.Compressor.Zip);

			//	Cherche si ce document a déjà été imprimé.
			var weakHash   = IDataHashExtension.GetWeakHash (data);
			var strongHash = IDataHashExtension.GetStrongHash (data);

			var currentBlob = documentMetadata.SerializedDocumentVersions.Where (x => x.WeakHash == weakHash && x.StrongHash == strongHash).FirstOrDefault ();

			if (currentBlob == null)
			{
				//	Crée une nouvelle instance.
				var newBlob = businessContext.CreateEntity<SerializedDocumentBlobEntity> ();

				newBlob.CreationDate = System.DateTime.Now;
				newBlob.LastModificationDate = newBlob.CreationDate;

				newBlob.Data = data;
				newBlob.SetHashes (newBlob.Data);

				documentMetadata.SerializedDocumentVersions.Add (newBlob);
			}
			else
			{
				//	Modifie la date de l'instance existante.
				currentBlob.LastModificationDate = System.DateTime.Now;
			}

			businessContext.DataContext.SaveChanges ();
		}

		public static List<DeserializedJob> SearchXmlSource(IBusinessContext businessContext, SerializedDocumentBlobEntity blob)
		{
			//	Régénère les jobs d'impression stockés dans une entité SerializedDocumentBlobEntity.
			if (blob.Data != null)
			{
				string xmlSource = Epsitec.Common.IO.Serialization.DeserializeAndDecompressFromMemory<string> (blob.Data);

				if (!string.IsNullOrEmpty (xmlSource))
				{
					return SerializationEngine.DeserializeJobs (businessContext, xmlSource);
				}
			}

			return null;
		}
		#endregion


		public static void SendDataToPrinter(IBusinessContext businessContext, string xml)
		{
			//	Imprime effectivement le source xml d'un document.
			var deserializeJobs = SerializationEngine.DeserializeJobs (businessContext, xml);
			PrintEngine.PrintJobs (businessContext, deserializeJobs);
		}

		private static void PrintJobs(IBusinessContext businessContext, IEnumerable<DeserializedJob> jobs)
		{
			//	Imprime effectivement une liste de jobs d'impression.
			foreach (var job in jobs)
			{
				if (job.PrintablePagesCount > 0)
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

			return SerializationEngine.SerializeJobs (businessContext, jobs);
		}

		public static string MakePrintingData(IBusinessContext businessContext, AbstractEntity entity, PrintingOptionDictionary options, PrintingUnitDictionary printingUnits)
		{
			//	Fabrique les données permettant d'imprimer un document, sans aucune interaction.
			//	Retourne le source xml correspondant.
			var jobs = PrintEngine.MakePrintingJobs (businessContext, entity, options, printingUnits);
			return SerializationEngine.SerializeJobs (businessContext, jobs);
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
			var documentPrinter = AbstractPrinter.CreateDocumentPrinter (businessContext, entity, options, printingUnits);

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
				var pageType                 = pair.Key;
				var documentPrintingUnitCode = pair.Value;

				var printingUnit = Common.GetPrintingUnit (businessContext.Data.Host, documentPrintingUnitCode);

				if (printingUnit != null)
				{
					//	Fabrique le dictionnaire des options à partir des options de base et
					//	des options définies avec l'unité d'impression.
					var customOptions = new PrintingOptionDictionary ();
					customOptions.MergeWith (options);						// options les moins prioritaires
					customOptions.MergeWith (printingUnit.Options);			// options les plus prioritaires

					documentPrinter.SetPrintingUnit (printingUnit, customOptions, PreviewMode.Print);
					var err = documentPrinter.BuildSections ();

					if (err.IsNullOrEmpty)  // ok ?
					{
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
					}
					else  // erreur ?
					{
						sections.Add (new SectionToPrint (err));
						break;
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

				if (p1.IsOK && p2.IsOK &&
					p1.InternalJobName                  == p2.InternalJobName                  &&
					p1.PrintingUnit.PhysicalPrinterName == p2.PrintingUnit.PhysicalPrinterName &&
					p1.FirstPage                        == p2.FirstPage                        )
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

				if (p1.IsOK && p2.IsOK &&
					p1.InternalJobName          == p2.InternalJobName &&
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

				if (j1.IsOK && j2.IsOK &&
					j1.InternalJobName     == j2.InternalJobName     &&
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
				var jobName = job.Sections[0].IsError ? "Erreur" : job.Sections[0].DocumentPrinter.JobName;
				job.JobFullName = string.Concat (jobName, ".", (index+1).ToString ());
				index++;
			}

			return jobs;
		}
		
		
		#region Dictionary getters
		private static PrintingOptionDictionary GetOptions(IBusinessContext businessContext, AbstractEntity entity)
		{
			//	Retourne les options à utiliser pour l'entité.
			var result = new PrintingOptionDictionary ();

			var category = PrintEngine.GetDocumentCategoryEntity(businessContext, entity);
			if (category != null)
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

			return result;
		}

		private static PrintingUnitDictionary GetPrintingUnits(IBusinessContext businessContext, AbstractEntity entity)
		{
			//	Retourne les unités d'impression à utiliser pour l'entité.
			var result = new PrintingUnitDictionary ();

			var category = PrintEngine.GetDocumentCategoryEntity (businessContext, entity);
			if (category != null)
			{
				if (category.DocumentPrintingUnits != null)
				{
#if false
					foreach (var printingUnits in category.DocumentPrintingUnits)
					{
						var printingUnit = printingUnits.GetPrintingUnits ();
						result.MergeWith (printingUnit);
					}
#else
					foreach (var documentPrintingUnit in category.DocumentPrintingUnits)
					{
						PrintingUnitDictionary dict = new PrintingUnitDictionary ();
						var pageTypes = documentPrintingUnit.GetPageTypes ();

						foreach (var pageType in pageTypes)
						{
							dict[pageType] = documentPrintingUnit.Code;
						}

						result.MergeWith (dict);
					}
#endif
				}
			}

			return result;
		}

		public static DocumentCategoryEntity GetDocumentCategoryEntity(IBusinessContext businessContext, AbstractEntity entity)
		{
			//	Retourne l'entité de catégorie de document à utiliser, pour une entité représentant un document quelconque.
			if (entity is DocumentMetadataEntity)
			{
				var metadata = entity as DocumentMetadataEntity;

				if (metadata.DocumentCategory != null)
				{
					return metadata.DocumentCategory;
				}
			}

			var mapping = PrintEngine.GetMappingEntity (businessContext, entity);

			if (mapping != null)
			{
				return mapping.DocumentCategories.FirstOrDefault ();
			}

			return null;
		}

		private static DocumentCategoryMappingEntity GetMappingEntity(IBusinessContext businessContext, AbstractEntity entity)
		{
			var example = new DocumentCategoryMappingEntity ();

			example.PrintableEntity = PrintEngine.GetPrintableEntityId (entity).ToString ();

			return businessContext.DataContext.GetByExample (example).FirstOrDefault ();
		}

		private static Druid GetPrintableEntityId(AbstractEntity entity)
		{
			if (entity != null)
			{
				return EntityInfo.GetTypeId (entity.GetType ());
			}
			else
			{
				return Druid.Empty;
			}
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
