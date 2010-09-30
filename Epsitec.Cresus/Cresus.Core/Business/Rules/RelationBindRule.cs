//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Rules
{
	[BusinessRule (RuleType.Bind)]
	internal class RelationBindRule : GenericBusinessRule<RelationEntity>
	{
		protected override void Apply(RelationEntity relation)
		{
			Logic.Current.BusinessContext.Register (relation.Person);
		}
	}
}
