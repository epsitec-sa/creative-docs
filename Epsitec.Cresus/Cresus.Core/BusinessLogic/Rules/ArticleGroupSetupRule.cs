//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.BusinessLogic.Rules
{
	[BusinessRule (RuleType.Setup)]
	internal class ArticleGroupSetupRule : GenericBusinessRule<ArticleGroupEntity>
	{
		protected override void Apply(ArticleGroupEntity articleGroup)
		{
			int? maxRank = Logic.Current.GetAllEntities<ArticleGroupEntity> ().Max (x => x.Rank);

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