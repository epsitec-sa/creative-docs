//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Aider.Entities;
using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;
using Epsitec.Cresus.Core.Entities;
using System.Linq;

namespace Epsitec.Aider.Controllers.SummaryControllers
{
	public sealed class SummaryAiderContactViewController : SummaryViewController<AiderContactEntity>
	{
		protected override void CreateBricks(BrickWall<AiderContactEntity> wall)
		{
			switch (this.Entity.ContactType)
			{
				case Enumerations.ContactType.PersonHousehold:
					wall.AddBrick (x => x.Person);
					wall.AddBrick (x => x.Household);
					break;

				case Enumerations.ContactType.PersonAddress:
					wall.AddBrick (x => x.Person);
					wall.AddBrick (x => x.Address);
					break;

				case Enumerations.ContactType.Legal:
					if (this.Entity.Person.IsNotNull ())
					{
						wall.AddBrick ().Include (x => x.Person);
					}
					wall.AddBrick (x => x.LegalPerson);
					break;
			}
		}
	}
}
