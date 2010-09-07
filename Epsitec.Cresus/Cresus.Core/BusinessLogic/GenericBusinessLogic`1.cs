//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.BusinessLogic
{
	public abstract class GenericBusinessLogic<T> : GenericBusinessLogic
		where T : AbstractEntity
	{
		public sealed override void ApplyRules(RuleType ruleType)
		{
		}
	}
}
