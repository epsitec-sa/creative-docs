//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionComptabilitéCompteViewController : EditionViewController<ComptabilitéCompteEntity>
	{
		protected override void CreateBricks(BrickWall<ComptabilitéCompteEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
				  .Field (x => x.Numéro)
				  .Field (x => x.Titre)
				  .Field (x => x.Catégorie)
				  .Field (x => x.Type)
				  .Field (x => x.Groupe)
				  .Field (x => x.TVA)
				  .Field (x => x.CompteOuvBoucl)
				  .Field (x => x.IndexOuvBoucl)
				  .Field (x => x.Monnaie)
			    .End ()
				;
		}
	}
}
