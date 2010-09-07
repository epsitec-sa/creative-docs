//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.BusinessLogic
{
	[System.AttributeUsage (System.AttributeTargets.Class)]
	public class BusinessRuleAttribute : System.Attribute
	{
		public BusinessRuleAttribute()
		{
		}

		public RuleType RuleType
		{
			get;
			set;
		}
	}
}
