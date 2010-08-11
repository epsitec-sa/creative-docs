//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

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
	public class SummaryLegalPersonViewController : SummaryViewController<Entities.LegalPersonEntity>
	{
		public SummaryLegalPersonViewController(string name, Entities.LegalPersonEntity entity)
			: base (name, entity)
		{
		}


		protected override void CreateUI(TileContainer container)
		{
			using (var data = TileContainerController.Setup (container))
			{
				this.CreateUIPerson (data);
				this.CreateUIMailContacts (data);
				this.CreateUITelecomContacts (data);
				this.CreateUIUriContacts (data);
			}
		}

		protected override void OnChildItemCreated(AbstractEntity entity)
		{
			this.SetupNewContact (entity as AbstractContactEntity);
			base.OnChildItemCreated (entity);
		}


		private void CreateUIPerson(SummaryDataItems data)
		{
			data.Add (
				new SummaryData
				{
					Name				= "LegalPerson",
					IconUri				= "Data.LegalPerson",
					Title				= TextFormater.FormatText ("Personne morale"),
					CompactTitle		= TextFormater.FormatText ("Personne"),
					TextAccessor		= Accessor.Create (this.EntityGetter, x => TextFormater.FormatText (x.Name)),
					CompactTextAccessor = Accessor.Create (this.EntityGetter, x => TextFormater.FormatText (x.Name)),
					EntityMarshaler		= this.EntityMarshaler,
				});
		}

		private void CreateUIMailContacts(SummaryDataItems data)
		{
			Common.CreateUIMailContacts (this.DataContext, data, this.EntityGetter, x => x.Contacts);
		}

		private void CreateUITelecomContacts(SummaryDataItems data)
		{
			Common.CreateUITelecomContacts (this.DataContext, data, this.EntityGetter, x => x.Contacts);
		}

		private void CreateUIUriContacts(SummaryDataItems data)
		{
			Common.CreateUIUriContacts (this.DataContext, data, this.EntityGetter, x => x.Contacts);
		}


		private void SetupNewContact(AbstractContactEntity contact)
		{
			if (contact != null)
			{
				contact.LegalPerson = this.Entity;
			}
		}
	}
}
