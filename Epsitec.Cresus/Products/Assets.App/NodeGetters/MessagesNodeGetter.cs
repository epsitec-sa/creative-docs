//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;

namespace Epsitec.Cresus.Assets.App.NodeGetters
{
	public class MessagesNodeGetter : INodeGetter<MessageNode>  // outputNodes
	{
		public void SetParams(IEnumerable<MessageNode> messages)
		{
			this.messages = messages.ToList ();
		}


		public int Count
		{
			get
			{
				return this.messages.Count;
			}
		}

		public MessageNode this[int index]
		{
			get
			{
				if (index >= 0 && index < this.messages.Count)
				{
					return this.messages[index];
				}
				else
				{
					return MessageNode.Empty;
				}
			}
		}


		private List<MessageNode>			messages;
	}
}
