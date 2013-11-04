//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.DataFillers
{
	public struct Node
	{
		//	Noeud correspondant à une ligne d'un TreeTable.
		//	Si Type == TreeTableTreeType.Final, il s'agit d'une ligne ne pouvant
		//	être ni compactée ni étendue (feuille de l'arbre).
		//	Si Type == TreeTableTreeType.Compacted ou TreeTableTreeType.Expanded,
		//	il s'agit d'une ligne avec un petit bouton triangulaire.
		public Node(Guid guid, int level, TreeTableTreeType type)
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
					&& this.Type == TreeTableTreeType.None;
			}
		}

		public static Node Empty = new Node (Guid.Empty, -1, TreeTableTreeType.None);

		public readonly Guid				Guid;
		public readonly int					Level;
		public readonly TreeTableTreeType	Type;
	}
}
