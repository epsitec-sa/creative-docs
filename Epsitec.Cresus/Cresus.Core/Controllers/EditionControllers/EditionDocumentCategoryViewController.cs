﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Bricks;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionDocumentCategoryViewController : EditionViewController<Entities.DocumentCategoryEntity>
	{
		protected override void CreateBricks(BrickWall<DocumentCategoryEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
				  .Field (x => x.Name)
				  .Field (x => x.Description)
				.End ()
				.Separator ()
				.Input ()
				  .Field (x => x.DocumentType)
				  .Field (x => x.DocumentSource)
				  .Field (x => x.DocumentFlowDirection)
				.End ()
				.Separator ()
				.Input ()
				  .Field (x => x.DocumentFooterTexts)
				.End ()
				;
			wall.AddBrick ()
				.Attribute (BrickMode.SpecialController0)
				.Icon ("Data.SpecialController")
				.Title ("Réglages")
				.Text ("Accéder aux réglages de la catégorie du document")
				;
		}
	}
}
