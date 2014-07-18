//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;

namespace Epsitec.Cresus.Assets.Server.NodeGetters
{
	public class WarningNodeGetter : INodeGetter<Warning>  // outputNodes
	{
		public WarningNodeGetter(List<Warning> warnings)
		{
			this.warnings = warnings;
		}


		public int Count
		{
			get
			{
				return this.warnings.Count;
			}
		}

		public Warning this[int index]
		{
			get
			{
				if (index >= 0 && index < this.warnings.Count)
				{
					return this.warnings[index];
				}
				else
				{
					return Warning.Empty;
				}
			}
		}


		private readonly List<Warning> warnings;
	}
}
