//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionComptabilitéViewController : EditionViewController<ComptabilitéEntity>
	{
		protected override void CreateBricks(BrickWall<ComptabilitéEntity> wall)
		{
			wall.AddBrick ()
				.Icon ("Comptabilité")
				.Input ()
				  .Title ("Nom de la comptabilité").Field (x => x.Name)
				  .Title ("Description de la comptabilité").Field (x => x.Description)
				.End ()
				.Separator ()
				.Input ()
				  .Title ("Début de la période comptable").Field (x => x.BeginDate)
				  .Title ("Fin de la période comptable").Field (x => x.EndDate)
				.End ()
				;
		}
	}
}
