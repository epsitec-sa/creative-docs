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
			Cresus.Bricks.BrickWall<NaturalPersonEntity> wall = new Bricks.BrickWall<NaturalPersonEntity> ();

			var brick1 = wall.AddBrick ()
				.Name ("NaturalPerson")
				.Icon ("Data.NaturalPerson")
				.Title (TextFormatter.FormatText ("Personne physique"))
				.TitleCompact (TextFormatter.FormatText ("Personne"))
				.Text (x => x.GetSummary ())
				.TextCompact (x => x.GetCompactSummary ());

			using (var data = TileContainerController.Setup (this))
			{
				var bridge = new Bridge<NaturalPersonEntity> (this);

				data.Add (bridge.CreateTileDataItem (brick1));

//-				this.CreateUIPerson (data);
				this.CreateUIMailContacts (data);
				this.CreateUITelecomContacts (data);
				this.CreateUIUriContacts (data);
			}
		}

		private void CreateUIPerson(TileDataItems data)
		{
			data.Add (
				new TileDataItem
				{
					Name				= "NaturalPerson",
					IconUri				= "Data.NaturalPerson",
					Title				= TextFormatter.FormatText ("Personne physique"),
					CompactTitle		= TextFormatter.FormatText ("Personne"),
					TextAccessor		= this.CreateAccessor (x => x.GetSummary ()),
					CompactTextAccessor = this.CreateAccessor (x => x.GetCompactSummary ()),
					EntityMarshaler		= this.CreateEntityMarshaler (),
				});
		}

		private void CreateUIMailContacts(TileDataItems data)
		{
			Common.CreateUIMailContacts (this.BusinessContext, data, this.EntityGetter, x => x.Contacts);
		}

		private void CreateUITelecomContacts(TileDataItems data)
		{
			Common.CreateUITelecomContacts (this.BusinessContext, data, this.EntityGetter, x => x.Contacts);
		}

		private void CreateUIUriContacts(TileDataItems data)
		{
			Common.CreateUIUriContacts (this.BusinessContext, data, this.EntityGetter, x => x.Contacts);
		}
	}
}
