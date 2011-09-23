//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Bricks;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionSerializedDocumentBlobViewController : EditionViewController<Entities.SerializedDocumentBlobEntity>
	{
		protected override void CreateBricks(BrickWall<SerializedDocumentBlobEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
				  .Field (x => x.Code)
				  .Field (x => x.CreationDate)
				  .Field (x => x.LastModificationDate)
				  .Field (x => x.WeakHash)
				  .Field (x => x.StrongHash)
				.End ()
				;
		}
	}
}
