//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.NodeGetters
{
	public class UserFieldNodeGetter : AbstractNodeGetter<UserFieldNode>  // outputNodes
	{
		public UserFieldNodeGetter(DataAccessor accessor, BaseType baseType)
		{
			this.accessor = accessor;
			this.baseType = baseType;

			this.nodes = new List<UserFieldNode> ();
		}


		public void SetParams()
		{
			this.nodes.Clear ();

			foreach (var userField in this.accessor.Mandat.Settings.GetUserFields (this.baseType))
			{
				var node = new UserFieldNode (userField.Field);
				this.nodes.Add (node);
			}
		}


		public override int Count
		{
			get
			{
				return this.nodes.Count;
			}
		}

		public override UserFieldNode this[int index]
		{
			get
			{
				if (index >= 0 && index < this.nodes.Count)
				{
					return this.nodes[index];
				}
				else
				{
					return UserFieldNode.Empty;
				}
			}
		}


		private readonly DataAccessor			accessor;
		private readonly BaseType				baseType;
		private readonly List<UserFieldNode>	nodes;
	}
}
