//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	public struct TreeNode
	{
		//	Noeud correspondant à une ligne d'un TreeTable.
		//	Si Type == NodeType.Final, il s'agit d'une ligne ne pouvant
		//	être ni compactée ni étendue (feuille de l'arbre).
		//	Si Type == NodeType.Compacted ou NodeType.Expanded,
		//	il s'agit d'une ligne avec un petit bouton triangulaire.
		public TreeNode(Guid guid, int level = 0, NodeType type = NodeType.None)
		{
			this.Guid  = guid;
			this.Level = level;
			this.Type  = type;
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

		public static TreeNode Empty = new TreeNode (Guid.Empty, -1, NodeType.None);

		public readonly Guid				Guid;
		public readonly int					Level;
		public readonly NodeType			Type;
	}
}
