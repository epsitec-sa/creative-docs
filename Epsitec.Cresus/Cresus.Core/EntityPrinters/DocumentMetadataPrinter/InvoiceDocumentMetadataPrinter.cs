//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Debug;
using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.IO;
using Epsitec.Common.Printing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Documents;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Helpers;
using Epsitec.Cresus.Core.Print;
using Epsitec.Cresus.Core.Print.Bands;
using Epsitec.Cresus.Core.Print.Containers;
using Epsitec.Cresus.Core.Print.EntityPrinters;
using Epsitec.Cresus.Core.Resolvers;
using Epsitec.Cresus.Core.Library.Business.ContentAccessors;
using Epsitec.Cresus.Core.Controllers.BusinessDocumentControllers;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.EntityPrinters
{
	public class InvoiceDocumentMetadataPrinter : AbstractDocumentMetadataPrinter
	{
		public InvoiceDocumentMetadataPrinter(IBusinessContext businessContext, AbstractEntity entity, PrintingOptionDictionary options, PrintingUnitDictionary printingUnits)
			: base (businessContext, entity, options, printingUnits)
		{
		}


		protected override Margins PageMargins
		{
			get
			{
				double leftMargin   = this.GetOptionValue (DocumentOption.LeftMargin, 20);
				double rightMargin  = this.GetOptionValue (DocumentOption.RightMargin, 20);
				double topMargin    = this.GetOptionValue (DocumentOption.TopMargin, 20);
				double bottomMargin = this.GetOptionValue (DocumentOption.BottomMargin, 20);

				double h = AbstractDocumentMetadataPrinter.reportHeight;

				if (this.HasIsr && this.HasOption (DocumentOption.IsrPosition, "WithInside"))
				{
					return new Margins (leftMargin, rightMargin, topMargin+h*2, h+InvoiceDocumentMetadataPrinter.marginBeforeIsr+AbstractIsrBand.DefautlSize.Height);
				}
				else
				{
					return new Margins (leftMargin, rightMargin, topMargin+h*2, h+bottomMargin);
				}
			}
		}


		public override void BuildSections()
		{
			base.BuildSections ();

			if (!this.HasIsr || this.HasOption (DocumentOption.IsrPosition, "Without") || this.PreviewMode == Print.PreviewMode.ContinuousPreview)
			{
				if (this.Entity.BillingDetails.Count != 0)
				{
					var billingDetails = this.Entity.BillingDetails[0];
					int firstPage = this.documentContainer.PrepareEmptyPage (PageType.First);

					this.BuildHeader (billingDetails);
					this.BuildArticles ();
					this.BuildConditions (billingDetails);
					this.BuildPages (billingDetails, firstPage);
					this.BuildReportHeaders (firstPage);
					this.BuildReportFooters (firstPage);

					this.documentContainer.Ending (firstPage);
				}
			}
			else
			{
				int documentRank = 0;
				bool onlyTotal = false;
				foreach (var billingDetails in this.Entity.BillingDetails)
				{
					this.documentContainer.DocumentRank = documentRank++;
					int firstPage = this.documentContainer.PrepareEmptyPage (PageType.First);

					this.BuildHeader (billingDetails);
					this.BuildArticles (onlyTotal: onlyTotal);
					this.BuildConditions (billingDetails);
					this.BuildPages (billingDetails, firstPage);
					this.BuildReportHeaders (firstPage);
					this.BuildReportFooters (firstPage);
					this.BuildIsrs (billingDetails, firstPage);

					this.documentContainer.Ending (firstPage);
					onlyTotal = true;
				}
			}
		}


		private void BuildConditions(BillingDetailEntity billingDetails)
		{
			//	Met les conditions à la fin de la facture.
			FormattedText conditions = FormattedText.Join (FormattedText.HtmlBreak, billingDetails.Text, billingDetails.AmountDue.PaymentMode.Description);

			if (!conditions.IsNullOrEmpty)
			{
				var band = new TextBand ();
				band.Text = conditions;
				band.FontSize = this.FontSize;

				this.documentContainer.AddFromTop (band, 0);
			}
		}


		private void BuildIsrs(BillingDetailEntity billingDetails, int firstPage)
		{
			if (this.HasOption (DocumentOption.IsrPosition, "WithInside"))
			{
				this.BuildInsideIsrs (billingDetails, firstPage);
			}

			if (this.HasOption (DocumentOption.IsrPosition, "WithOutside"))
			{
				this.BuildOutsideIsr (billingDetails, firstPage);
			}
		}

		private void BuildInsideIsrs(BillingDetailEntity billingDetails, int firstPage)
		{
			//	Met un BVR orangé ou un BV rose en bas de chaque page.
			for (int page = firstPage; page < this.documentContainer.PageCount (); page++)
			{
				this.documentContainer.CurrentPage = page;

				this.BuildIsr (billingDetails, mackle: page != this.documentContainer.PageCount ()-1);
			}
		}

		private void BuildOutsideIsr(BillingDetailEntity billingDetails, int firstPage)
		{
			//	Met un BVR orangé ou un BV rose sur une dernière page séparée.
			var bounds = new Rectangle (Point.Zero, AbstractIsrBand.DefautlSize);

			if (this.documentContainer.PageCount () - firstPage > 1 ||
				this.documentContainer.CurrentVerticalPosition - InvoiceDocumentMetadataPrinter.marginBeforeIsr < bounds.Top ||
				this.HasPrintingUnitDefined (PageType.Single) == false)
			{
				//	On ne prépare pas une nouvelle page si on peut mettre la facture
				//	et le BV sur une seule page !
				this.documentContainer.PrepareEmptyPage (PageType.Isr);
			}

			this.BuildIsr (billingDetails);
		}

		private void BuildIsr(BillingDetailEntity billingDetails, bool mackle=false)
		{
			//	Met un BVR orangé ou un BV rose au bas de la page courante.
			AbstractIsrBand isr;

			if (this.HasOption (DocumentOption.IsrType, "Isr"))
			{
				isr = new IsrBand ();  // BVR orangé
			}
			else
			{
				isr = new IsBand ();  // BV rose
			}

			isr.PaintIsrSimulator = this.HasOption (DocumentOption.IsrFacsimile);
			isr.From = this.Entity.BillToMailContact.GetSummary ();
			isr.To = billingDetails.IsrDefinition.SubscriberAddress;
			isr.Communication = InvoiceDocumentHelper.GetTitle (this.Metadata, this.Entity, billingDetails);

			isr.Slip = new IsrSlip (billingDetails);
			isr.NotForUse = mackle;  // pour imprimer "XXXXX XX" sur un faux BVR

			var bounds = new Rectangle (Point.Zero, AbstractIsrBand.DefautlSize);
			this.documentContainer.AddAbsolute (isr, bounds);
		}


		private bool HasIsr
		{
			//	Indique s'il faut imprimer le BV. Pour cela, il faut que l'unité d'impression soit définie pour le type PageType.Isr.
			//	En mode DocumentOption.IsrPosition = "WithOutside", cela évite d'imprimer à double un BV sur l'imprimante 'Blanc'.
			get
			{
				if (this.HasOption (DocumentOption.IsrPosition, "WithInside"))
				{
					return true;
				}

				if (this.HasOption (DocumentOption.IsrPosition, "WithOutside"))
				{
					if (this.currentPrintingUnit != null)
					{
						var example = new DocumentPrintingUnitsEntity ();
						example.Code = this.currentPrintingUnit.DocumentPrintingUnitCode;

						var documentPrintingUnits = this.businessContext.DataContext.GetByExample<DocumentPrintingUnitsEntity> (example).FirstOrDefault ();

						if (documentPrintingUnits != null)
						{
							var pageTypes = documentPrintingUnits.GetPageTypes ();

							return pageTypes.Contains (PageType.Isr);
						}
					}
				}

				return false;
			}
		}


		private static readonly double		marginBeforeIsr = 10;
	}
}
