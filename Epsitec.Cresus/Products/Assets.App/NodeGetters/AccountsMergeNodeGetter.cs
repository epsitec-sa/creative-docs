//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;

namespace Epsitec.Cresus.Assets.App.NodeGetters
{
	public class AccountsMergeNodeGetter : INodeGetter<AccountMergeTodo>  // outputNodes
	{
		public void SetParams(List<AccountMergeTodo> todo)
		{
			this.nodes = todo;
		}


		public int Count
		{
			get
			{
				return this.nodes.Count;
			}
		}

		public AccountMergeTodo this[int index]
		{
			get
			{
				if (index >= 0 && index < this.nodes.Count)
				{
					return this.nodes[index];
				}
				else
				{
					return AccountMergeTodo.Empty;
				}
			}
		}


		private List<AccountMergeTodo>			nodes;
	}
}
