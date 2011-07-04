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

			wall.AddBrick (x => x.BusinessDocument)
				.AsType<BusinessDocumentEntity> ()
				.Attribute (BrickMode.DefaultToSummarySubview)
				.Text (x => x.GetSummary ())
				.TextCompact (x => x.GetCompactSummary ())
				;

			wall.AddBrick (x => x.BusinessDocument)
				.AsType<BusinessDocumentEntity> ()
				.Attribute (BrickMode.SpecialController0)
				.Title ("Lignes du document")
				.Icon ("Data.ArticleDocumentItem")
				.Text (x => SummaryDocumentMetadataViewController.GetArticlesSummary (x))
				.TextCompact (x => SummaryDocumentMetadataViewController.GetArticlesSummary (x))
				;

			if (this.Entity.BusinessDocument is BusinessDocumentEntity)
			{
				if (this.Entity.DocumentCategory.DocumentType == Business.DocumentType.Invoice)
				{
					wall.AddBrick (x => SummaryDocumentMetadataViewController.GetBillingDetailEntities (x))
						.Attribute (BrickMode.AutoGroup)
						.Template ()
						.End ()
						;
				}
			}
			
			wall.AddBrick (x => x.Comments)
				.Template ()
				.End ()
				;
		}


		private static FormattedText GetArticlesSummary(BusinessDocumentEntity businessDocumentEntity)
		{
			FormattedText summary = new FormattedText ();

			foreach (var line in businessDocumentEntity.Lines)
			{
				summary = summary.AppendLine (line.GetCompactSummary ());
			}

			return summary;
		}

		private static IList<BillingDetailEntity> GetBillingDetailEntities(DocumentMetadataEntity documentMetadataEntity)
		{
			if (documentMetadataEntity.BusinessDocument is BusinessDocumentEntity)
			{
				var businessDocumentEntity = documentMetadataEntity.BusinessDocument as BusinessDocumentEntity;
				return businessDocumentEntity.BillingDetails;
			}

			return null;
		}
	}
}
