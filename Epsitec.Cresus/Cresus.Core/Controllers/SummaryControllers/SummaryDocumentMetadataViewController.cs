//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Factories;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	public class SummaryDocumentMetadataViewController : SummaryViewController<DocumentMetadataEntity>
	{
		protected override void CreateBricks(BrickWall<DocumentMetadataEntity> wall)
		{
			wall.AddBrick (x => x)
				.Name ("InvoiceDocument")
				.Icon ("Data.InvoiceDocument")
				.Title (x => x.DocumentCategory.Name)
				.Text (x => x.GetSummary ())
				.TextCompact (x => x.GetCompactSummary ())
				;

#if true
			wall.AddBrick (x => x.BusinessDocument)
				.OfType<BusinessDocumentEntity> ()
//				.Attribute (BrickMode.DefaultToSummarySubview)
				.Text (x => x.GetSummary ())
				.TextCompact (x => x.GetCompactSummary ())
				;
#endif

			wall.AddBrick (x => x.BusinessDocument)
				.OfType<BusinessDocumentEntity> ()
				.Attribute (BrickMode.SpecialController0)
				.Title ("Lignes du document")
				.Icon ("Data.DocumentItems")
				.Text (x => SummaryDocumentMetadataViewController.GetArticlesSummary (x, 16))
				.TextCompact (x => SummaryDocumentMetadataViewController.GetArticlesSummary (x, 16))
				;

			if (this.Entity.BusinessDocument is BusinessDocumentEntity)
			{
				if (this.Entity.DocumentCategory.DocumentType == Business.DocumentType.Invoice)
				{
					wall.AddBrick (x => SummaryDocumentMetadataViewController.GetPaymentTransactionEntities (x))
						.Attribute (BrickMode.AutoGroup)
						.Template ()
						.End ()
						;
				}
			}

			wall.AddBrick (x => x.SerializedDocumentVersions)
				.Attribute (BrickMode.AutoGroup)
				.Attribute (BrickMode.HideAddButton)
				.Attribute (BrickMode.HideRemoveButton)
				.Template ()
				.End ()
				;

			wall.AddBrick (x => x.Comments)
				.Template ()
				.End ()
				;
		}


		private static FormattedText GetArticlesSummary(BusinessDocumentEntity businessDocumentEntity, int limit)
		{
			//	Retourne un texte qui résume toutes les lignes du document.
			//	Le paramètre 'limit' donne le nombre maximum de lignes à inclure, sans compter les 2 lignes
			//	"..." et le grand total.
			FormattedText summary = new FormattedText ();
			var lines = businessDocumentEntity.GetConciseLines ();

			for (int i=0; i<lines.Count-1; i++)  // toutes les lignes, sauf le grand total
			{
				if (i >= limit)
				{
					summary = summary.AppendLine ("...");
					break;
				}

				var line = lines[i];

				var text = line.GetCompactSummary ();
				text = SummaryDocumentMetadataViewController.GetIndentedText (text, line.GroupLevel);

				summary = summary.AppendLine (text);
			}

			if (lines.Count > 1)
			{
				var line = lines.Last ();
				summary = summary.AppendLine (line.GetCompactSummary ());  // ajoute le grand total
			}

			return summary;
		}

		private static FormattedText GetIndentedText(FormattedText text, int level)
		{
			//	Code expérimental pour mettre en évidence l'indentation des lignes d'un document commercial.
			for (int i = 0; i < level-1; i++)
			{
				text = FormattedText.Concat ("|  ", text);
			}

			return text;
		}

		private static IList<PaymentTransactionEntity> GetPaymentTransactionEntities(DocumentMetadataEntity documentMetadataEntity)
		{
			if (documentMetadataEntity.BusinessDocument is BusinessDocumentEntity)
			{
				var businessDocumentEntity = documentMetadataEntity.BusinessDocument as BusinessDocumentEntity;
				return businessDocumentEntity.PaymentTransactions;
			}

			return null;
		}
	}
}
