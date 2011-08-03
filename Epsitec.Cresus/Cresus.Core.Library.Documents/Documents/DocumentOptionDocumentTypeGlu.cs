//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Cresus.Core.Business;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Documents
{
	public static class DocumentOptionDocumentTypeGlu
	{
		/// <summary>
		/// Il faudrait pouvoir accéder à Cresus.Core.Print.EntityPrinters.AbstractPrinter.GetRequiredDocumentOptions,
		/// mais on n'a pas accès à Cresus.Core.Library.Print dans Cresus.Core.Library.Documents. D'où ce doublon !
		/// </summary>
		/// <param name="documentType"></param>
		/// <returns></returns>
		public static IEnumerable<DocumentOption> GetRequiredDocumentOptions(DocumentType documentType)
		{
			yield return DocumentOption.Orientation;
			yield return DocumentOption.HeaderLogo;
			yield return DocumentOption.Specimen;
			yield return DocumentOption.FontSize;

			yield return DocumentOption.LeftMargin;
			yield return DocumentOption.RightMargin;
			yield return DocumentOption.TopMargin;
			yield return DocumentOption.BottomMargin;

			switch (documentType)
			{
				case DocumentType.SalesQuote:
					yield return DocumentOption.LayoutFrame;
					yield return DocumentOption.GapBeforeGroup;
					yield return DocumentOption.IndentWidth;

					yield return DocumentOption.LineNumber;
					yield return DocumentOption.ArticleAdditionalQuantities;
					yield return DocumentOption.ArticleId;
					yield return DocumentOption.ColumnsOrder;
					break;

				case DocumentType.OrderBooking:
					yield return DocumentOption.LayoutFrame;
					yield return DocumentOption.GapBeforeGroup;
					yield return DocumentOption.IndentWidth;

					yield return DocumentOption.LineNumber;
					yield return DocumentOption.ArticleAdditionalQuantities;
					yield return DocumentOption.ArticleId;
					yield return DocumentOption.ColumnsOrder;

					yield return DocumentOption.Signing;
					break;

				case DocumentType.OrderConfirmation:
					yield return DocumentOption.LayoutFrame;
					yield return DocumentOption.GapBeforeGroup;
					yield return DocumentOption.IndentWidth;

					yield return DocumentOption.LineNumber;
					yield return DocumentOption.ArticleAdditionalQuantities;
					yield return DocumentOption.ArticleId;
					yield return DocumentOption.ColumnsOrder;
					break;

				case DocumentType.ProductionOrder:
				case DocumentType.ProductionChecklist:
					yield return DocumentOption.LayoutFrame;

					yield return DocumentOption.LineNumber;
					yield return DocumentOption.ArticleId;
					yield return DocumentOption.ColumnsOrder;

					yield return DocumentOption.Signing;
					break;

				case DocumentType.DeliveryNote:
					yield return DocumentOption.LayoutFrame;
					yield return DocumentOption.GapBeforeGroup;
					yield return DocumentOption.IndentWidth;

					yield return DocumentOption.LineNumber;
					yield return DocumentOption.ArticleAdditionalQuantities;
					yield return DocumentOption.ArticleId;
					yield return DocumentOption.ColumnsOrder;
					break;

				case DocumentType.Invoice:
					yield return DocumentOption.LayoutFrame;
					yield return DocumentOption.GapBeforeGroup;
					yield return DocumentOption.IndentWidth;

					yield return DocumentOption.LineNumber;
					yield return DocumentOption.ArticleAdditionalQuantities;
					yield return DocumentOption.ArticleId;
					yield return DocumentOption.ColumnsOrder;

					yield return DocumentOption.IsrPosition;
					yield return DocumentOption.IsrType;
					yield return DocumentOption.IsrFacsimile;
					break;

				case DocumentType.RelationSummary:
					yield return DocumentOption.LayoutFrame;

					yield return DocumentOption.RelationMail;
					yield return DocumentOption.RelationTelecom;
					yield return DocumentOption.RelationUri;
					break;

				case DocumentType.ArticleDefinitionSummary:
					yield return DocumentOption.LayoutFrame;
					break;

				case DocumentType.MailContactLabel:
					yield return DocumentOption.LayoutFrame;
					break;
			}
		}
	}
}
