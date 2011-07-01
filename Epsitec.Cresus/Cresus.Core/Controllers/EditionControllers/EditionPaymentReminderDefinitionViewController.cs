//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Support;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Loader;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionPaymentReminderDefinitionViewController : EditionViewController<Entities.PaymentReminderDefinitionEntity>
	{
		protected override void CreateBricks(BrickWall<PaymentReminderDefinitionEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
				  .Field (x => x.Name)
				  .Field (x => x.Description)
				.End ()
				.Separator ()
				.Input ()
				  .Field (x => x.ExtraPaymentTerm)
				  .Field (x => x.AdministrativeTaxArticle).PickFromCollection (this.GetAdminArticles ())
				.End ()
				;
		}

		private IEnumerable<ArticleDefinitionEntity> GetAdminArticles()
		{
			return this.BusinessContext.Data.GetAllEntities<ArticleDefinitionEntity> ()
				.Where (x => x.ArticleCategory.ArticleType == Business.ArticleType.Admin);
		}
	}
}
