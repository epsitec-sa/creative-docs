//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;

namespace Epsitec.Cresus.Assets.Server.NodesGetter
{
	public class ErrorNodesGetter : AbstractNodesGetter<AmortissementError>  // outputNodes
	{
		public ErrorNodesGetter(List<AmortissementError> errors)
		{
			this.errors = errors;
		}


		public override int Count
		{
			get
			{
				return this.errors.Count;
			}
		}

		public override AmortissementError this[int index]
		{
			get
			{
				if (index >= 0 && index < this.errors.Count)
				{
					return this.errors[index];
				}
				else
				{
					return AmortissementError.Empty;
				}
			}
		}


		private readonly List<AmortissementError> errors;
	}
}
