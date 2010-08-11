//	Copyright � 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Controllers.TabIds;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class NumericValueArticleParameterDefinitionEntity
	{
		public override ArticleParameterTabId TabId
		{
			get
			{
				return ArticleParameterTabId.Numeric;
			}
		}
	}
}
