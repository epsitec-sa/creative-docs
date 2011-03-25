//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

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
		protected override void CreateBricks(Bricks.BrickWall<DocumentMetadataEntity> wall)
		{
			wall.AddBrick (x => x)
				.Name ("InvoiceDocument")
				.Icon ("Data.InvoiceDocument")
				.Title ("Document")
				.Text (x => x.GetSummary ())
				.TextCompact (x => x.GetCompactSummary ());

			wall.AddBrick (x => x.DocumentCategory)
				.Text (x => x.GetSummary ())
				.TextCompact (x => x.GetCompactSummary ());

			wall.AddBrick (x => x.BusinessDocument)
				.AsType<BusinessDocumentEntity> ()
				.Text (x => x.GetSummary ())
				.TextCompact (x => x.GetCompactSummary ());

			wall.AddBrick (x => x.Comments)
				.Template ();
		}
	}
}
