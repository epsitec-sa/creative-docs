//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionDocumentMetadataViewController : EditionViewController<Entities.DocumentMetadataEntity>
	{
		protected override void CreateBricks(BrickWall<DocumentMetadataEntity> wall)
		{
			wall.AddBrick (x => x)
				.Title ("Informations générales")
				.Input ()
				  .Title ("N° de document").Field (x => x.IdA)
				.End ()
				.Separator ()
				.Input ()
				  .Field (x => x.Description)
				.End ()
				.Separator ()
				.Input ()
				  .Field (x => x.CreationDate)
				  .Field (x => x.LastModificationDate)
				.End ()
				;
			wall.AddBrick (x => x.Comments)
				.Template ()
				.End ()
				;
		}
	}
}
