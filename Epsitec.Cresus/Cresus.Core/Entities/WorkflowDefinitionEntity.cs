//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class WorkflowDefinitionEntity
	{
		public bool CheckEnableCondition(string typeKey)
		{
			if (this.EnableCondition == typeKey)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
