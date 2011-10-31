//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	public class SummaryOtherRelationViewController : SummaryViewController<OtherRelationEntity>
	{
		protected override void CreateBricks(BrickWall<OtherRelationEntity> wall)
		{
			wall.AddBrick ()
				;
			wall.AddBrick (x => x.Person.Contacts)
				.OfType<MailContactEntity> ()
				.Template ()
				.End ();
				;
			wall.AddBrick (x => x.Person.Contacts)
				.OfType<TelecomContactEntity> ()
				.Attribute (BrickMode.AutoGroup)
				.Template ()
				.End ()
				;
			wall.AddBrick (x => x.Person.Contacts)
				.OfType<UriContactEntity> ()
				.Attribute (BrickMode.AutoGroup)
				.Template ()
				.End ()
				;
		}
	}
}
