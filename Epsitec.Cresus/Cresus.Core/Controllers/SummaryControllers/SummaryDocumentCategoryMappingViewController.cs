//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	public class SummaryDocumentCategoryMappingViewController : SummaryViewController<Entities.DocumentCategoryMappingEntity>
	{
		protected override void CreateBricks(Bricks.BrickWall<Entities.DocumentCategoryMappingEntity> wall)
		{
			wall.AddBrick ();
		}
	}
}
