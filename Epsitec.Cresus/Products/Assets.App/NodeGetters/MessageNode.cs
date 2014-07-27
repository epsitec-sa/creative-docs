//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.App.NodeGetters
{
	public struct MessageNode
	{
		public MessageNode(string description)
		{
			this.Description = description;
		}

		public bool IsEmpty
		{
			get
			{
				return this.Description == null;
			}
		}

		public static MessageNode Empty = new MessageNode (null);

		public readonly string				Description;
	}
}
