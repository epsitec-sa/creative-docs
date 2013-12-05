//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.NodesGetter
{
	public struct TreeNode
	{
		//	Noeud correspondant à une ligne d'un TreeTable.
		//	Si Type == NodeType.Final, il s'agit d'une ligne ne pouvant
		//	être ni compactée ni étendue (feuille de l'arbre).
		//	Si Type == NodeType.Compacted ou NodeType.Expanded,
		//	il s'agit d'une ligne avec un petit bouton triangulaire.
		public TreeNode(Guid guid, BaseType baseType, int level, decimal? ratio, NodeType type)
		{
			this.Guid     = guid;
			this.BaseType = baseType;
			this.Level    = level;
			this.Ratio    = ratio;
			this.Type     = type;
		}

		public bool IsEmpty
		{
			get
			{
				return this.Guid.IsEmpty
					&& this.Level == -1
					&& this.Type == NodeType.None;
			}
		}

		public static TreeNode Empty = new TreeNode (Guid.Empty, BaseType.Objects, -1, null, NodeType.None);

		public readonly Guid				Guid;
		public readonly BaseType			BaseType;
		public readonly int					Level;
		public readonly decimal?			Ratio;
		public readonly NodeType			Type;
	}
}
