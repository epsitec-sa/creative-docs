//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	public class SummaryLegalPersonViewController : SummaryViewController<Entities.LegalPersonEntity>
	{
		public SummaryLegalPersonViewController(string name, Entities.LegalPersonEntity entity)
			: base (name, entity)
		{
		}


		protected override void CreateUI(TileContainer container)
		{
			var containerController = new TileContainerController (this, container);
			var data = containerController.DataItems;

			this.CreateUIPerson (data);
			this.CreateUIMailContacts (data);
			this.CreateUITelecomContacts (data);
			this.CreateUIUriContacts (data);

			containerController.GenerateTiles ();
		}


		private void CreateUIPerson(SummaryDataItems data)
		{
			data.Add (
				new SummaryData
				{
					Name				= "LegalPerson",
					IconUri				= "Data.LegalPerson",
					Title				= UIBuilder.FormatText ("Personne morale"),
					CompactTitle		= UIBuilder.FormatText ("Personne"),
					TextAccessor		= Accessor.Create (this.EntityGetter, x => UIBuilder.FormatText (x.Name)),
					CompactTextAccessor = Accessor.Create (this.EntityGetter, x => UIBuilder.FormatText (x.Name)),
					EntityAccessor		= this.EntityGetter,
				});
		}

		private void CreateUIMailContacts(SummaryDataItems data)
		{
			Common.CreateUIMailContacts (data, this.EntityGetter, x => x.Contacts);
		}

		private void CreateUITelecomContacts(SummaryDataItems data)
		{
			var template = new CollectionTemplate<Entities.TelecomContactEntity> ("TelecomContact")
				.DefineTitle (x => UIBuilder.FormatText (x.TelecomType.Name))
				.DefineText (x => UIBuilder.FormatText (x.Number, "(", string.Join (", ", x.Roles.Select (role => role.Name)), ")"))
				.DefineCompactText (x => UIBuilder.FormatText (x.Number, "(", x.TelecomType.Name, ")"));

			data.Add (
				new SummaryData
				{
					AutoGroup    = true,
					Name		 = "TelecomContact",
					IconUri		 = "Data.Telecom",
					Title		 = UIBuilder.FormatText ("Téléphone"),
					CompactTitle = UIBuilder.FormatText ("Téléphone"),
					Text		 = UIBuilder.FormatText ("<i>vide</i>")
				});

			data.Add (CollectionAccessor.Create (this.EntityGetter, x => x.Contacts, template));
		}

		private void CreateUIUriContacts(SummaryDataItems data)
		{
			var template = new CollectionTemplate<Entities.UriContactEntity> ("UriContact", filter: x => x.UriScheme.Code == "mailto")
				.DefineText (x => UIBuilder.FormatText (x.Uri, "(", string.Join (", ", x.Roles.Select (role => role.Name)), ")"))
				.DefineCompactText (x => UIBuilder.FormatText (x.Uri))
				.DefineSetupItem (x => x.UriScheme = CoreProgram.Application.Data.GetUriScheme ("mailto"));

			data.Add (
				new SummaryData
				{
					AutoGroup    = true,
					Name		 = "UriContact",
					IconUri		 = "Data.Uri",
					Title		 = UIBuilder.FormatText ("E-Mail"),
					CompactTitle = UIBuilder.FormatText ("E-Mail"),
					Text		 = UIBuilder.FormatText ("<i>vide</i>")
				});

			data.Add (CollectionAccessor.Create (this.EntityGetter, x => x.Contacts, template));
		}
	}
}
