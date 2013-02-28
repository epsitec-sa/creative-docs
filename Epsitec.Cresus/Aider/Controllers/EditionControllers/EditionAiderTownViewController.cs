//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Controllers.EditionControllers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Controllers.EditionControllers
{
	public sealed class EditionAiderTownViewController : EditionViewController<AiderTownEntity>
	{
		protected override void CreateBricks(BrickWall<AiderTownEntity> wall)
		{
			var favorites = this.GetCountryFavorites ();

			if (this.Entity.Mutability == Mutability.SystemDefined)
			{
				wall.AddBrick ()
					.Input ()
						.Field (x => x.Mutability)
							.ReadOnly ()
						.Field (x => x.ZipCode)
							.ReadOnly ()
						.Field (x => x.SwissZipCodeAddOn)
							.ReadOnly ()
						.Field (x => x.Name)
							.ReadOnly ()
						.Field (x => x.SwissCantonCode)
							.ReadOnly ()
						.Field (x => x.SwissZipCodeId)
							.ReadOnly ()
						.Field (x => x.SwissZipType)
							.ReadOnly ()
						.Field (x => x.Country)
							.ReadOnly ()
					.End ();
			}
			else
			{
				wall.AddBrick ()
					.Input ()
						.Field (x => x.Mutability)
							.ReadOnly ()
						.Field (x => x.ZipCode)
						.Field (x => x.Name)
						.Field (x => x.Country)
							.WithFavorites (favorites)
					.End ();
			}
		}

		private List<AiderCountryEntity> GetCountryFavorites()
		{
			var repository = this.BusinessContext.Data.GetRepository<AiderCountryEntity> ();
			var example    = new AiderCountryEntity
			{
				IsPreferred = true,
			};

			return repository.GetByExample (example).ToList ();
		}
	}
}
