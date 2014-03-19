//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Controllers.EditionControllers;
using Epsitec.Aider.Entities;

using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;

using Epsitec.Common.Support;

using System.Linq;
using Epsitec.Aider.Controllers.ActionControllers;
using Epsitec.Aider.Override;

namespace Epsitec.Aider.Controllers.SummaryControllers
{
	public sealed class SummaryAiderLegalPersonViewController : SummaryViewController<AiderLegalPersonEntity>
	{
		protected override void CreateBricks(Cresus.Bricks.BrickWall<AiderLegalPersonEntity> wall)
		{
			var user = AiderUserManager.Current.AuthenticatedUser;
			var legal = this.Entity;

			if (user.LoginName == "epsitec")
			{
				//	Process the degenerated cases when we click on them: no name for the
				//	legal person. This should never happen, but as of dec. 17 2013, the
				//	database contained 6 of these broken legal persons.

				if (string.IsNullOrEmpty (legal.Name))
				{
					var example1 = new AiderLegalPersonEntity
					{
						Name = ""
					};

					var example2 = new AiderLegalPersonEntity
					{
						Name = @"''"
					};

					var emptyItems1 = this.BusinessContext.GetByExample (example1);
					var emptyItems2 = this.BusinessContext.GetByExample (example2);

					var emptyItems = emptyItems1.Concat (emptyItems2).ToList ();

					foreach (var item in emptyItems)
					{
						AiderLegalPersonEntity.Delete (this.BusinessContext, item);
					}

					this.BusinessContext.SaveChanges (Cresus.Core.Business.LockingPolicy.KeepLock);

					return;
				}
			}
			
			wall.AddBrick ()
				.EnableActionMenu<ActionAiderLegalPersonViewController0AddToBag> ();

			wall.AddBrick ()
				.Title ("Adresse de base")
				.Text (x => x.Address.GetPostalAddress (PostalAddressType.Default))
				.Icon ("Data.AiderAddress")
				.WithSpecialController (typeof (EditionAiderLegalPersonViewController1Address));

			var contacts = this.Entity.Contacts;

			if (contacts.Any ())
			{
				wall.AddBrick (x => x.Contacts)
					.Title (contacts.Count > 1 ? Resources.Text ("Contacts") : Resources.Text ("Contact"))
					.Attribute (BrickMode.HideAddButton)
					.Attribute (BrickMode.HideRemoveButton)
					.Attribute (BrickMode.AutoGroup)
					.Template ()
					.End ()
					.Attribute (BrickMode.DefaultToSummarySubView);
			}

			wall.AddBrick (x => x.Comment)
				.Attribute (BrickMode.AutoCreateNullEntity);
		}
	}
}
