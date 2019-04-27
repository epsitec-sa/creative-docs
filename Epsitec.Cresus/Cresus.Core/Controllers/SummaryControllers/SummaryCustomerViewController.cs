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
	public class SummaryCustomerViewController : SummaryViewController<CustomerEntity>
	{
		protected override void CreateBricks(BrickWall<CustomerEntity> wall)
		{
			wall.AddBrick ()
				;
			wall.AddBrick (x => x.MainRelation.Person.Contacts)
				.OfType<MailContactEntity> ()
				.Template ()
				.End ();
				;
			wall.AddBrick (x => x.MainRelation.Person.Contacts)
				.OfType<TelecomContactEntity> ()
				.Attribute (BrickMode.AutoGroup)
				.Template ()
				.End ()
				;
			wall.AddBrick (x => x.MainRelation.Person.Contacts)
				.OfType<UriContactEntity> ()
				.Attribute (BrickMode.AutoGroup)
				.Template ()
				.End ()
				;
			wall.AddBrick (x => x.MainRelation.Contacts)
				.Attribute (BrickMode.AutoGroup)
				.Template ()
				.End ()
				;
			wall.AddBrick (x => x.Affairs)
				.Attribute (BrickMode.DefaultToSummarySubView)
				.Attribute (BrickMode.HideAddButton)
				.Attribute (BrickMode.AutoGroup)
				.Template ()
				.End ()
				;
		}

		protected override IEnumerable<AbstractEntity> GetMasterEntities()
		{
			yield return this.Entity;
			yield return this.Entity.MainRelation;
		}
	}
}
