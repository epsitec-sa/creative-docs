//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.NodeGetters
{
	public struct UserFieldNode
	{
		public UserFieldNode(ObjectField field)
		{
			this.Field = field;
		}

		public bool IsEmpty
		{
			get
			{
				return this.Field == ObjectField.Unknown;
			}
		}

		public static UserFieldNode Empty = new UserFieldNode (ObjectField.Unknown);

		public readonly ObjectField			Field;
	}
}
