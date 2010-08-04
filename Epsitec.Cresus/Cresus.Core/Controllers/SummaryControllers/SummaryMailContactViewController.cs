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
	public class SummaryMailContactViewController : SummaryViewController<Entities.MailContactEntity>
	{
		public SummaryMailContactViewController(string name, Entities.MailContactEntity entity)
			: base (name, entity)
		{
		}


		protected override void CreateUI(TileContainer container)
		{
			this.TileContainerController = new TileContainerController (this, container, this.DataContext);
			var data = this.TileContainerController.DataItems;

			this.CreateUIMail (data);

			this.TileContainerController.GenerateTiles ();
		}

		protected override void OnChildItemCreated(AbstractEntity entity)
		{
			this.SetupNewContact (entity as AbstractContactEntity);
			base.OnChildItemCreated (entity);
		}


		private void CreateUIMail(SummaryDataItems data)
		{
			data.Add (
				new SummaryData
				{
					Name				= "MailContact",
					IconUri				= "Data.Mail",
					Title				= UIBuilder.FormatText ("Adresse"),
					CompactTitle		= UIBuilder.FormatText ("Adresse"),
					TextAccessor		= Accessor.Create (this.EntityGetter, x => UIBuilder.FormatText (x.Address.Street.StreetName, "\n", x.Address.Location.PostalCode, x.Address.Location.Name)),
					CompactTextAccessor = Accessor.Create (this.EntityGetter, x => UIBuilder.FormatText (x.Address.Street.StreetName, x.Address.Location.PostalCode, x.Address.Location.Name)),
					EntityMarshaler		= this.EntityMarshaler,
				});
		}

		
		private void SetupNewContact(AbstractContactEntity contact)
		{
#if false
			if (contact != null)
			{
				contact.NaturalPerson = this.Entity;
			}
#endif
		}
	}
}
