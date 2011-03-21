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
		protected override void CreateUI()
		{
			using (var data = TileContainerController.Setup (this))
			{
				this.CreateUIPerson (data);
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
					Name				= "LegalPerson",
					IconUri				= "Data.LegalPerson",
					Title				= TextFormatter.FormatText ("Personne morale"),
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
