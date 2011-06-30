//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	public class SummaryMailContactViewController : SummaryViewController<Entities.MailContactEntity>
	{
#if true
		protected override void CreateBricks(BrickWall<MailContactEntity> wall)
		{
			wall.AddBrick (x => x);
		}
#else
		protected override void CreateUI()
		{
			using (var data = TileContainerController.Setup (this))
			{
				this.CreateUIMail (data);
			}
		}

		private void CreateUIMail(TileDataItems data)
		{
			data.Add (
				new TileDataItem
				{
					Name				= "MailContact",
					IconUri				= "Data.MailContact",
					Title				= TextFormatter.FormatText ("Adresse", "(", string.Join (", ", this.Entity.ContactGroups.Select (role => role.Name)), ")"),
					CompactTitle		= TextFormatter.FormatText ("Adresse"),
					TextAccessor		= this.CreateAccessor (x => x.GetSummary ()),
					CompactTextAccessor = this.CreateAccessor (x => x.GetCompactSummary ()),
					EntityMarshaler		= this.CreateEntityMarshaler (),
				});
		}
#endif
	}
}
