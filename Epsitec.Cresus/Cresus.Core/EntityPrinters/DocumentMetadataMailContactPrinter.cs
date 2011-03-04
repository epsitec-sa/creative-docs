//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Documents;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Helpers;
using Epsitec.Cresus.Core.Print.Bands;
using Epsitec.Cresus.Core.Print.Containers;
using Epsitec.Cresus.Core.Resolvers;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Epsitec.Cresus.Core.Print.EntityPrinters;

namespace Epsitec.Cresus.Core.EntityPrinters
{
	public class DocumentMetadataMailContactPrinter : AbstractPrinter
	{
		private DocumentMetadataMailContactPrinter(CoreData coreData, IEnumerable<AbstractEntity> entities, OptionsDictionary options, PrintingUnitsDictionary printingUnits)
			: base (coreData, entities, options, printingUnits)
		{
		}

		public override string JobName
		{
			get
			{
				if (this.Entity.LegalPerson.IsNotNull ())
				{
					return TextFormatter.FormatText ("Client", this.Entity.LegalPerson.Name).ToSimpleText ();
				}

				if (this.Entity.NaturalPerson.IsNotNull ())
				{
					return TextFormatter.FormatText ("Client", this.Entity.NaturalPerson.Lastname).ToSimpleText ();
				}

				return "Client inconnu";
			}
		}


		public override Size MinimalPageSize
		{
			get
			{
				return new Size (60, 25);  // à voir à l'usage
			}
		}

		public override Size MaximalPageSize
		{
			get
			{
				return new Size (100, 100);  // carré de 10x10 cm (à voir à l'usage)
			}
		}

		public override Size PreferredPageSize
		{
			get
			{
				return new Size (62, 29);  // petite étiquette pour Brother QL-560
			}
		}


		protected override Margins PageMargins
		{
			get
			{
				return new Margins (3);
			}
		}


		/// <summary>
		/// Spécifie l'unité d'impression, afin de déterminer la taille des pages à produire.
		/// </summary>
		public override void SetPrintingUnit(PrintingUnit printerUnit)
		{
			Size size;

			if (printerUnit == null)
			{
				size = this.PreferredPageSize;
			}
			else
			{
				size = printerUnit.PhysicalPaperSize;
			}

			if (size.Width < size.Height)  // format portrait ?
			{
				// On veut toujours être en paysage.
				size = new Size (size.Height, size.Width);
			}

			if (!Print.Common.InsidePageSize (size, this.MinimalPageSize, this.MaximalPageSize))
			{
				size = this.PreferredPageSize;
			}

			this.requiredPageSize = size;
		}

		public override void BuildSections()
		{
			base.BuildSections ();

			this.documentContainer.Clear ();

			if (this.HasPrintingUnitDefined (PageType.Label))
			{
				int firstPage = this.documentContainer.PrepareEmptyPage (PageType.Label);
				this.BuildSummary ();
				this.documentContainer.Ending (firstPage);
			}
		}

		public override void PrintForegroundCurrentPage(IPaintPort port)
		{
			base.PrintForegroundCurrentPage (port);

			this.documentContainer.PaintBackground (port, this.CurrentPage, this.PreviewMode);
			this.documentContainer.PaintForeground (port, this.CurrentPage, this.PreviewMode);
		}


		private void BuildSummary()
		{
			//	Ajoute le résumé dans le document.
			var band = new TextBand ();
			band.Text = this.Entity.GetSummary ();
			band.FontSize = 3.0;

			this.documentContainer.AddFromTop (band, 0.0);
		}


		private MailContactEntity Entity
		{
			get
			{
				var metadata = this.Metadata;

				if (metadata != null)
				{
					var document = metadata.BusinessDocument as BusinessDocumentEntity;

					if (document != null && document.BillToMailContact != null)
					{
						return document.BillToMailContact;
					}
				}

				throw new System.ArgumentException ("DocumentMetadata.BusinessDocument.BillToMailContact not found.");
			}
		}

		private DocumentMetadataEntity Metadata
		{
			get
			{
				return this.entities.FirstOrDefault () as DocumentMetadataEntity;
			}
		}

		class Factory : IEntityPrinterFactory
		{
			#region IEntityPrinterFactory Members

			bool IEntityPrinterFactory.CanPrint(AbstractEntity entity, OptionsDictionary options)
			{
				//	Pour le moment, on ne veut pas d'étiquettes, mais des factures...
				return false;
//				return entity is DocumentMetadataEntity;
			}

			AbstractPrinter IEntityPrinterFactory.CreatePrinter(CoreData coreData, IEnumerable<AbstractEntity> entities, Documents.OptionsDictionary options, Documents.PrintingUnitsDictionary printingUnits)
			{
				return new DocumentMetadataMailContactPrinter (coreData, entities, options, printingUnits);
			}

			#endregion
		}
	}
}
