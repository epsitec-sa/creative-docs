//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.Core.Bricks;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Bricks;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionSerializedDocumentBlobViewController : EditionViewController<Entities.SerializedDocumentBlobEntity>
	{
		public override double GetPreferredWidth(int columnIndex, int columnCount)
		{
			return 500;
		}

		protected override void CreateBricks(BrickWall<SerializedDocumentBlobEntity> wall)
		{
			wall.AddBrick ()
				.Icon ("Data.SerializedDocumentBlob")
				.Title ("Aperçu du document")
				.Attribute (BrickMode.FullHeightStretch)
				.Input ()
				  .Field (x => x).WithSpecialController ()
				.End ()
				;
		}
	}
}
