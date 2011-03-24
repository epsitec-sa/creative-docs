//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

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
		protected override void CreateBricks(Bricks.BrickWall<CustomerEntity> wall)
		{
			wall.AddBrick (x => x);

			wall.AddBrick (x => x.Relation.Person.Contacts)
				.AsType<MailContactEntity> ()
				.Template ()
				 .Title (x => x.GetTitle ())
				 .Text (x => x.GetSummary ())
				 .TextCompact (x => x.GetCompactSummary ())
				.End ();

			wall.AddBrick (x => x.Relation.Person.Contacts)
				.AsType<TelecomContactEntity> ()
				.AutoGroup ()
				.Template ()
				 .Text (x => x.GetSummary ())
				 .TextCompact (x => x.GetCompactSummary ())
				.End ();

			wall.AddBrick (x => x.Relation.Person.Contacts)
				.AsType<UriContactEntity> ()
				.AutoGroup ()
				.Template ()
				 .Text (x => x.GetSummary ())
				 .TextCompact (x => x.GetCompactSummary ())
				.End ();

			wall.AddBrick (x => x.Affairs)
				.Template ()
				 .Text (x => x.GetSummary ())
				 .TextCompact (x => TextFormatter.FormatText (x.IdA))
				.End ();
		}

		protected override IEnumerable<AbstractEntity> GetMasterEntities()
		{
			yield return this.Entity;
			yield return this.Entity.Relation;
		}
	}
}
