//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	public class SummaryNaturalPersonViewController : SummaryViewController<Entities.NaturalPersonEntity>
	{
		public SummaryNaturalPersonViewController(string name, Entities.NaturalPersonEntity entity)
			: base (name, entity)
		{
		}


		protected override void CreateUI()
		{
			var bridge = new Bridge<NaturalPersonEntity> (this);

			var wall = bridge.CreateBrickWall ();

			wall.AddBrick ()
//				.Name ("NaturalPerson")
//				.Icon ("Data.NaturalPerson")
				.Title (TextFormatter.FormatText ("Personne physique"))
				.TitleCompact (TextFormatter.FormatText ("Personne"))
				.Text (x => x.GetSummary ())
				.TextCompact (x => x.GetCompactSummary ());

			wall.AddBrick (x => x.Contacts)
				.AsType<MailContactEntity> ()
//				.Name ("MailContact")
//				.Icon ("Data.MailContact")
				
				.Title ("Adresses")
				.TitleCompact ("Adresses")
				.Text (CollectionTemplate.DefaultEmptyText)
				
				.Template ()
				 .Title (x => x.GetTitle ())
				 .Text (x => x.GetSummary ())
				 .TextCompact (x => x.GetCompactSummary ())
				.End ();

			wall.AddBrick (x => x.Contacts)
				.AsType<TelecomContactEntity> ()
//				.Name ("TelecomContact")
//				.Icon ("Data.TelecomContact")
				.Title ("Téléphones")
				.TitleCompact ("Téléphones")
				.Text (CollectionTemplate.DefaultEmptyText)
				
				.Template ()
				 .Title (x => x.GetTitle ())
				 .Text (x => x.GetSummary ())
				 .TextCompact (x => x.GetCompactSummary ())
				.End ();

			wall.AddBrick (x => x.Contacts)
				.AsType<UriContactEntity> ()
//				.Name ("UriContact")
//				.Icon ("Data.UriContact")
				.Title ("E-mails")
				.TitleCompact ("E-mails")
				.Text (CollectionTemplate.DefaultEmptyText)
				
				.Template ()
				 .Text (x => x.GetSummary ())
				 .TextCompact (x => x.GetCompactSummary ())
				.End ();

			using (var data = TileContainerController.Setup (this))
			{
				foreach (var brick in wall.Bricks)
				{
					bridge.CreateTileDataItem (data, brick);
				}
			}
		}
	}
}
