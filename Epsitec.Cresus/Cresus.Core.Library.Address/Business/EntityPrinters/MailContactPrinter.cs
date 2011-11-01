//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Documents;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Print.Bands;
using Epsitec.Cresus.Core.Print.Containers;
using Epsitec.Cresus.Core.Resolvers;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Epsitec.Cresus.Core.Print;
using Epsitec.Cresus.Core.Business;

namespace Epsitec.Cresus.Core.Business.EntityPrinters
{
	public sealed class MailContactPrinter : AbstractPrinter
	{
		private MailContactPrinter(IBusinessContext businessContext, AbstractEntity entity, PrintingOptionDictionary options, PrintingUnitDictionary printingUnits)
			: base (businessContext, entity, options, printingUnits)
		{
		}

		private static IEnumerable<DocumentOption> RequiredDocumentOptions
		{
			get
			{
				yield return DocumentOption.Orientation;
				yield return DocumentOption.Specimen;
				yield return DocumentOption.FontSize;
				yield return DocumentOption.Language;

				yield return DocumentOption.LeftMargin;
				yield return DocumentOption.RightMargin;
				yield return DocumentOption.TopMargin;
				yield return DocumentOption.BottomMargin;
			}
		}

		private static IEnumerable<PageType> RequiredPageTypes
		{
			get
			{
				yield return PageType.Label;
			}
		}


		public override string JobName
		{
			get
			{
				string text = TextFormatter.FormatText (this.Entity.PersonAddress).ToSimpleText ();
				
				if (string.IsNullOrEmpty (text))
				{
					return "Client inconnu";
				}
				else
				{
					return string.Concat ("Client ", text);
				}
			}
		}


		public override Size PreferredPageSize
		{
			get
			{
				return new Size (62, 29);  // petite étiquette pour Brother QL-560
			}
		}


		protected override Margins GetPageMargins()
		{
			double leftMargin   = this.GetOptionValue (DocumentOption.LeftMargin, 3);
			double rightMargin  = this.GetOptionValue (DocumentOption.RightMargin, 3);
			double topMargin    = this.GetOptionValue (DocumentOption.TopMargin, 3);
			double bottomMargin = this.GetOptionValue (DocumentOption.BottomMargin, 3);

			return new Margins (leftMargin, rightMargin, topMargin, bottomMargin);
		}


		public override FormattedText BuildSections()
		{
			base.BuildSections ();

			this.documentContainer.Clear ();

			if (this.HasPrintingUnitDefined (PageType.Label))
			{
				int firstPage = this.documentContainer.PrepareEmptyPage (PageType.Label);
				this.BuildSummary ();
				this.documentContainer.Ending (firstPage);
			}

			return null;  // ok
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
			band.TwoLetterISOLanguageName = this.TwoLetterISOLanguageName;
			band.Text = this.Entity.GetSummary ();
			band.FontSize = this.FontSize;

			this.documentContainer.AddFromTop (band, 0.0);
		}


		private MailContactEntity Entity
		{
			get
			{
				return this.entity as MailContactEntity;
			}
		}
		
		private class Factory : IEntityPrinterFactory
		{
			#region IEntityPrinterFactory Members

			public bool CanPrint(AbstractEntity entity, PrintingOptionDictionary options)
			{
				return entity is MailContactEntity;
			}

			public IEnumerable<System.Type> GetSupportedEntityTypes()
			{
				yield return typeof (MailContactEntity);
			}

			public DocumentType GetDocumentType(AbstractEntity entity)
			{
				if (entity is MailContactEntity)
				{
					return DocumentType.MailContactLabel;
				}

				return DocumentType.Unknown;
			}
			
			public IEntityPrinter CreatePrinter(IBusinessContext businessContext, AbstractEntity entity, PrintingOptionDictionary options, PrintingUnitDictionary printingUnits)
			{
				return new MailContactPrinter (businessContext, entity, options, printingUnits);
			}

			public IEnumerable<DocumentOption> GetRequiredDocumentOptions(DocumentType documentType)
			{
				switch (documentType)
				{
					case DocumentType.MailContactLabel:
						return MailContactPrinter.RequiredDocumentOptions;
				}

				return null;
			}

			public IEnumerable<PageType> GetRequiredPageTypes(DocumentType documentType)
			{
				switch (documentType)
				{
					case DocumentType.MailContactLabel:
						return MailContactPrinter.RequiredPageTypes;
				}

				return null;
			}

			#endregion
		}
	}
}
