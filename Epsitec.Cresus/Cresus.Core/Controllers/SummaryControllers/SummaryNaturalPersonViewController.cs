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

			wall.AddBrick ()
				.Name ("NaturalPerson")
				.Icon ("Data.NaturalPerson")
				.Title (TextFormatter.FormatText ("Personne physique"))
				.TitleCompact (TextFormatter.FormatText ("Personne"))
				.Text (x => x.GetSummary ())
				.TextCompact (x => x.GetCompactSummary ());

			wall.AddBrick (x => x.Contacts)
				.AsType<MailContactEntity> ()
				.Name ("MailContact")
				.Icon ("Data.Mail")
				.Title ("Adresses")
				.TitleCompact ("Adresses")
				.Text (CollectionTemplate.DefaultEmptyText)
				.Template ()
				.Title (x => x.GetTitle ())
				.Text (x => x.GetSummary ())
				.TextCompact (x => x.GetCompactSummary ());

			using (var data = TileContainerController.Setup (this))
			{
				var bridge = new Bridge<NaturalPersonEntity> (this);

				foreach (var brick in wall.Bricks)
				{
					bridge.CreateTileDataItem (data, brick);
				}

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
			var template = new CollectionTemplate<MailContactEntity> ("MailContact", data.Controller, this.BusinessContext.DataContext)
				.DefineTitle (x => x.GetTitle ())
				.DefineText (x => x.GetSummary ())
				.DefineCompactText (x => x.GetCompactSummary ());


			data.Add (
				new TileDataItem
				{
					Name		 = "MailContact",
					IconUri		 = "Data.Mail",
					Title		 = TextFormatter.FormatText ("Adresses"),
					CompactTitle = TextFormatter.FormatText ("Adresses"),
					Text		 = CollectionTemplate.DefaultEmptyText
				});

			data.Add (CollectionAccessor.Create (this.EntityGetter, x => x.Contacts, template));
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
