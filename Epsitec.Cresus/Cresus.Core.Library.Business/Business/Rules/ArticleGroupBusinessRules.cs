//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Rules
{
	[BusinessRule]
	internal class ArticleGroupBusinessRules : GenericBusinessRule<ArticleGroupEntity>
	{
		public override void ApplySetupRule(ArticleGroupEntity articleGroup)
		{
			var businessContext = this.GetBusinessContext ();
			int? maxRank = businessContext.Data.GetAllEntities<ArticleGroupEntity> ().Max (x => x.Rank);

			if (maxRank.HasValue)
			{
				articleGroup.Rank = maxRank.Value + 1;
			}
			else
			{
				articleGroup.Rank = 0;
			}
		}
	}
}