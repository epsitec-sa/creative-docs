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

using Epsitec.Cresus.Core.Documents;
using Epsitec.Cresus.Core.Documents.Verbose;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Print.Serialization;

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
		public static void PrintCommand(CoreData coreData, AbstractEntity entity)
		{
			//	La commande 'Print' du ruban a été activée.
			if (entity == null)
			{
				var message = "Il n'y a aucune donnée à imprimer.";
				MessageDialog.CreateOk ("Erreur", DialogIcon.Warning, message).OpenDialog ();
				return;
			}

			var xml = PrintEngine.MakePrintingData (coreData, entity);

			if (string.IsNullOrEmpty (xml))
			{
				var message = "Ce type de donnée ne peut pas être imprimé.";
				MessageDialog.CreateOk ("Erreur", DialogIcon.Warning, message).OpenDialog ();
				return;
			}

			PrintEngine.SendDataToPrinter (coreData, xml);
		}

		public static void PreviewCommand(CoreData coreData, AbstractEntity entity)
		{
			//	La commande 'Preview' du ruban a été activée.
			if (entity == null)
			{
				var message = "Il n'y a aucune donnée à visualiser.";
				MessageDialog.CreateOk ("Erreur", DialogIcon.Warning, message).OpenDialog ();
				return;
			}

			var xml = PrintEngine.MakePrintingData (coreData, entity);

			if (string.IsNullOrEmpty (xml))
			{
				var message = "Ce type de donnée ne peut pas être visualisé.";
				MessageDialog.CreateOk ("Erreur", DialogIcon.Warning, message).OpenDialog ();
				return;
			}

			var deserializeJobs = SerializationEngine.DeserializeJobs (coreData, xml);

			var dialog = new Dialogs.PrintPreviewDialog (coreData, deserializeJobs);
			dialog.IsModal = true;
			dialog.OpenDialog ();

			if (dialog.Result == DialogResult.Accept)  // imprimer ?
			{
				PrintEngine.PrintJobs (coreData, deserializeJobs);
			}
		}
		#endregion


		public static void SendDataToPrinter(CoreData coreData, string xml)
		{
			//	Imprime effectivement le source xml d'un document.
			var deserializeJobs = SerializationEngine.DeserializeJobs (coreData, xml);
			PrintEngine.PrintJobs (coreData, deserializeJobs);
		}

		private static void PrintJobs(CoreData coreData, List<DeserializedJob> jobs)
		{
			//	Imprime effectivement une liste de jobs d'impression.
			foreach (var job in jobs)
			{
				PrintDocument printDocument = new PrintDocument ();

				printDocument.DocumentName = job.JobFullName;
				printDocument.SelectPrinter (FormattedText.Unescape (job.PrinterPhysicalName));
				printDocument.PrinterSettings.Copies = 1;
				printDocument.DefaultPageSettings.Margins = new Margins (0, 0, 0, 0);

				var engine = new XmlJobPrintEngine (coreData, printDocument, job.Sections);
				printDocument.Print (engine);
			}
		}


		public static string MakePrintingData(CoreData coreData, AbstractEntity entity)
		{
			//	Fabrique les données permettant d'imprimer un document, sans aucune interaction.
			//	Retourne le source xml correspondant.
			//	Si l'entité est de type DocumentMetadataEntity, on utilise les options et les unités
			//	d'impression définies dans l'entité DocumentCategory.
			var options       = PrintEngine.GetOptions (entity);
			var printingUnits = PrintEngine.GetPrintingUnits (entity);

			return PrintEngine.MakePrintingData (coreData, entity, options, printingUnits);
		}

		public static string MakePrintingData(CoreData coreData, AbstractEntity entity, OptionsDictionary options, PrintingUnitsDictionary printingUnits)
		{
			//	Fabrique les données permettant d'imprimer un document, sans aucune interaction.
			//	Retourne le source xml correspondant.
			var entities = new List<AbstractEntity> ();
			entities.Add (entity);

			return PrintEngine.MakePrintingData (coreData, entities, options, printingUnits);
		}

		public static string MakePrintingData(CoreData coreData, IEnumerable<AbstractEntity> entities, OptionsDictionary options, PrintingUnitsDictionary printingUnits)
		{
			//	Fabrique les données permettant d'imprimer un document, sans aucune interaction.
			//	Retourne le source xml correspondant.
			System.Diagnostics.Debug.Assert (coreData != null);
			System.Diagnostics.Debug.Assert (entities != null);
			System.Diagnostics.Debug.Assert (options != null);
			System.Diagnostics.Debug.Assert (printingUnits != null);

			if (entities.Count () == 0)
			{
				return null;
			}

			//	Crée les classes qui savent imprimer l'entité qui représente le document (EntityPrinters.AbstrctPrinter).
			var documentPrinters = EntityPrinters.AbstractPrinter.CreateDocumentPrinters (coreData, entities, options, printingUnits);

			if (documentPrinters.Count () == 0 || printingUnits.Count == 0)
			{
				return null;
			}

			//	Prépare toutes les pages à imprimer, pour toutes les entités.
			//	On crée autant de sections que de pages, soit une section par page.
			var sections = new List<SectionToPrint> ();
			var printingUnitList = PrinterApplicationSettings.GetPrintingUnitList ();
			var pageTypes = VerbosePageType.GetAll ();

			for (int entityRank = 0; entityRank < entities.Count (); entityRank++)
			{
				var entity = entities.ElementAt (entityRank);

				int documentPrinterRank = 0;
				foreach (var documentPrinter in documentPrinters)
				{
					foreach (var pair in printingUnits.ContentPair)
					{
						var pageType         = pair.Key;
						var printingUnitName = pair.Value;

						PrintingUnit printingUnit = printingUnitList.Where (p => p.LogicalName == printingUnitName).FirstOrDefault ();

						if (printingUnit != null &&
							Common.InsidePageSize (printingUnit.PhysicalPaperSize, documentPrinter.MinimalPageSize, documentPrinter.MaximalPageSize))
						{
							//	Fabrique le dictionnaire des options à partir des options de base et
							//	des options définies avec l'unité d'impression.
							var customOptions = new OptionsDictionary ();
							customOptions.Merge (options);                         // options les moins prioritaires
							customOptions.Merge (printingUnit.OptionsDictionary);  // options les plus prioritaires

							documentPrinter.SetOptionsDictionary (customOptions);
							documentPrinter.SetPrintingUnit (printingUnit);
							documentPrinter.PreviewMode = PreviewMode.Print;
							documentPrinter.BuildSections ();

							if (!documentPrinter.IsEmpty (pageType))
							{
								var jobName = pageTypes.Where (x => x.Type == pageType).Select (x => x.Job).FirstOrDefault ();

								for (int copy = 0; copy < printingUnit.Copies; copy++)
								{
									var physicalPages = documentPrinter.GetPhysicalPages (pageType);
									foreach (var physicalPage in physicalPages)
									{
										string e = (entityRank+1).ToString ();
										string p = (documentPrinterRank+1).ToString ();
										string d = (documentPrinter.GetDocumentRank (physicalPage)+1).ToString ();
										string c = (copy+1).ToString ();
										string internalJobName = string.Concat (jobName, ".", e, ".", p, ".", d, ".", c);

										sections.Add (new SectionToPrint (printingUnit, internalJobName, physicalPage, entityRank, documentPrinter));
									}
								}
							}

							documentPrinterRank++;
						}
					}
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
					p1.EntityRank               == p2.EntityRank      &&
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
				job.JobFullName = string.Concat (job.Sections[0].EntityPrinter.JobName, ".", (index+1).ToString ());
				index++;
			}

			return SerializationEngine.SerializeJobs (jobs);
		}


		#region Dictionary getters
		private static OptionsDictionary GetOptions(AbstractEntity entity)
		{
			//	Retourne les options à utiliser pour l'entité.
			var result = new OptionsDictionary ();

			throw new System.NotImplementedException ();
#if false
			if (entity is DocumentMetadataEntity)
			{
				var metadata = entity as DocumentMetadataEntity;

				foreach (var documentOptions in metadata.DocumentCategory.DocumentOptions)
				{
					var option = documentOptions.GetOptions ();
					result.Merge (option);
				}
			}
#endif

#if true
			if (result.Count == 0)  // TODO: Hack à supprimer dès que possible !
			{
				result = OptionsDictionary.GetDefault ();
				result.Add (DocumentOption.EsrPosition, "WithInside");  // force un BV
			}
#endif

			return result;
		}

		private static PrintingUnitsDictionary GetPrintingUnits(AbstractEntity entity)
		{
			//	Retourne les unités d'impression à utiliser pour l'entité.
			var result = new PrintingUnitsDictionary ();
			throw new System.NotImplementedException ();
#if false
			if (entity is DocumentMetadataEntity)
			{
				var metadata = entity as DocumentMetadataEntity;

				foreach (var documentOptions in metadata.DocumentCategory.DocumentPrintingUnits)
				{
					var printingUnit = documentOptions.GetPrintingUnits ();
					result.Merge (printingUnit);
				}
			}
#endif

#if true
			if (result.Count == 0)  // TODO: Hack à supprimer dès que possible !
			{
				result.Add (PageType.All, "Blanc");
				//?result.Add (PageType.Esr, "Jaune");
				//?result.Add (PageType.Copy, "Brouillon");
				result.Add (PageType.Label, "Etiquettes");
			}
#endif

			return result;
		}
		#endregion


		public static Image GetImage(CoreData coreData, string id)
		{
			throw new System.NotImplementedException ();
#if false
			//	Retrouve l'image dans la base de données, à partir de son identificateur (ImageBlobEntity.Code).
			var store = coreData.ImageDataStore;
			var data = store.GetImageData (id);
			return data.GetImage ();
#endif
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
