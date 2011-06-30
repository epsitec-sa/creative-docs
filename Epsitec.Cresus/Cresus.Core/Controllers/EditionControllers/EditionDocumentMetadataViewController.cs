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
				.Name ("Document")
				.Title ("Informations générales")
				.Input ()
				  .HorizontalGroup ("N° de document (principal, externe et interne)")
					.Field (x => x.IdA).Width (72)
					.Field (x => x.IdB).Width (72)
					.Field (x => x.IdC).Width (72)
				  .End ()
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
