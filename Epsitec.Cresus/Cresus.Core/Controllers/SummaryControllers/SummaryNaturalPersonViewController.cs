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
					Name				= "NaturalPerson",
					IconUri				= "Data.NaturalPerson",
					Title				= UIBuilder.FormatText ("Personne physique"),
					CompactTitle		= UIBuilder.FormatText ("Personne"),
					TextAccessor		= Accessor.Create (this.EntityGetter, x => UIBuilder.FormatText (x.Title.Name, "\n", x.Firstname, x.Lastname, "(", x.Gender.Name, ")", "\n", x.BirthDate)),
					CompactTextAccessor = Accessor.Create (this.EntityGetter, x => UIBuilder.FormatText (x.Title.ShortName, x.Firstname, x.Lastname)),
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
				contact.NaturalPerson = this.Entity;
			}
		}
	}
}
