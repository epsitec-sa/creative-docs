//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;

namespace Epsitec.Cresus.Assets.Server.NodeGetters
{
	/// <summary>
	///	Noeud correspondant à une ligne d'un TreeTable.
	///	Si Type == NodeType.Final, il s'agit d'une ligne ne pouvant
	///	être ni compactée ni étendue (feuille de l'arbre).
	///	Si Type == NodeType.Compacted ou NodeType.Expanded,
	///	il s'agit d'une ligne avec un petit bouton triangulaire.
	/// </summary>
	public struct TreeNode
	{
		public TreeNode(Guid guid, BaseType baseType, int level, decimal? ratio, NodeType type, int? groupIndex)
		{
			this.Guid       = guid;
			this.BaseType   = baseType;
			this.Level      = level;
			this.Ratio      = ratio;
			this.Type       = type;
			this.GroupIndex = groupIndex;
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

		public static TreeNode Empty = new TreeNode (Guid.Empty, BaseType.Assets, -1, null, NodeType.None, null);

		public readonly Guid				Guid;
		public readonly BaseType			BaseType;
		public readonly int					Level;
		public readonly decimal?			Ratio;
		public readonly NodeType			Type;
		public readonly int?				GroupIndex;
	}
}
