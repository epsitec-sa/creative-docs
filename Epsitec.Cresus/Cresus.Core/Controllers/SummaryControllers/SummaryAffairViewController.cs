//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	public class SummaryAffairViewController : SummaryViewController<AffairEntity>
	{
		protected override void CreateBricks(Bricks.BrickWall<AffairEntity> wall)
		{
			wall.AddBrick (x => x);

			wall.AddBrick (x => x.Documents)
				.Attribute (BrickMode.HideAddButton)
				.Attribute (BrickMode.HideRemoveButton)
				.Attribute (BrickMode.DefaultToSummarySubview)
				.Name ("DocMetadata")
				.Title ("Document lié")
				.TitleCompact ("Documents liés")
				.Template ()
				  .Title (x => x.GetCompactSummary ())			// TODO: il est normal que cette ligne doivent rester
				  .Text (x => x.GetSummary ())					// TODO: mais pas celle-çi..
				  .TextCompact (x => x.GetCompactSummary ())	// TODO: ..ni celle-çi !
				.End ()
				;
			wall.AddBrick (x => x.Comments)
				.Template ()
				;
		}
	}
}