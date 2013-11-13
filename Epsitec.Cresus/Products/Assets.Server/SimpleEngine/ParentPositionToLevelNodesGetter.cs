//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	public class ParentPositionToLevelNodesGetter : AbstractNodesGetter<LevelNode>  // outputNodes
	{
		public ParentPositionToLevelNodesGetter(AbstractNodesGetter<ParentPositionNode> inputNodes, DataAccessor accessor, BaseType baseType)
		{
			this.inputNodes = inputNodes;
			this.accessor   = accessor;
			this.baseType   = baseType;

			this.ppNodes = new List<ParentPositionNode> ();
			this.levelNodes = new List<LevelNode> ();
		}


		public Timestamp? Timestamp;


		public override int Count
		{
			get
			{
				return this.levelNodes.Count;
			}
		}

		public override LevelNode this[int index]
		{
			get
			{
				if (index >= 0 && index < this.levelNodes.Count)
				{
					return this.levelNodes[index];
				}
				else
				{
					return LevelNode.Empty;
				}
			}
		}


		public void UpdateData()
		{
			this.ppNodes.Clear ();
			this.levelNodes.Clear ();

			foreach (var node in this.inputNodes.Nodes)
			{
				if (!this.Insert (node))  // parent pas inséré ?
				{
					//	Cherche tous les aïeuls.
					var list = new List<ParentPositionNode> ();
					var n = node;

					while (!n.IsEmpty &&
						   !this.ppNodes.Where (x => x.Guid == n.Guid).Any ())
					{
						list.Insert (0, n);
						n = this.inputNodes.Nodes.Where (x => x.Guid == n.Parent).FirstOrDefault ();
					}

					//	Insère tous les aïeuls, depuis le plus ancien.
					foreach (var nn in list)
					{
						this.Insert (nn);
					}
				}
			}

			foreach (var node in this.ppNodes)
			{
				var n = new LevelNode (node.Guid, this.GetLevel (node));
				this.levelNodes.Add (n);
			}
		}

		private bool Insert(ParentPositionNode node)
		{
			int i = 0;

			if (!node.Parent.IsEmpty)  // est-ce que le noeud a un parent ?
			{
				int parentIndex = this.ppNodes.FindIndex (x => x.Guid == node.Parent);

				//	Si le parent n'est pas déjà inséré, on retourne false.
				if (parentIndex == -1)
				{
					return false;
				}

				//	Cherche l'index où insérer le noeud.
				i = parentIndex + 1;

				while (i < this.ppNodes.Count                   &&
					   this.ppNodes[i].Parent   == node.Parent  &&
					   this.ppNodes[i].Position <= node.Position)
				{
					i++;
				}
			}

			//	Insère le noeud à la bonne place.
			this.ppNodes.Insert (i, node);

			return true;
		}

		private int GetLevel(ParentPositionNode node)
		{
			int level = 0;

			while (!node.Parent.IsEmpty)
			{
				node = this.inputNodes.Nodes.Where (x => x.Guid == node.Parent).FirstOrDefault ();
				level++;
			}

			return level;
		}




		private readonly AbstractNodesGetter<ParentPositionNode> inputNodes;
		private readonly DataAccessor					accessor;
		private readonly BaseType						baseType;
		private readonly List<ParentPositionNode>		ppNodes;
		private readonly List<LevelNode>				levelNodes;
	}
}
