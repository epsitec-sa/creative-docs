//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Entities;

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	public class SummaryMailContactViewController : SummaryViewController<MailContactEntity>
	{
#if false
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
