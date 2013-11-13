//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	/// <summary>
	/// Accès en lecture à des données. En entrée, on reçoit des données parent/position
	/// désordonnées qui servent à reconstruire un arbre. En sortie, on fourni des données
	/// ordonnées avec une indication du level.
	/// ParentPositionNode -> LevelNode
	/// </summary>
	public class ParentPositionToLevelNodesGetter : AbstractNodesGetter<LevelNode>  // outputNodes
	{
		public ParentPositionToLevelNodesGetter(AbstractNodesGetter<ParentPositionNode> inputNodes, DataAccessor accessor, BaseType baseType)
		{
			this.inputNodes = inputNodes;
			this.accessor   = accessor;
			this.baseType   = baseType;

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
			this.levelNodes.Clear ();

			var p = this.inputNodes.Nodes.Where (x => x.Parent.IsEmpty).FirstOrDefault ();
			if (p.IsEmpty)
			{
				return;
			}

			var tree = new TreeNode (null, p);
			this.Insert (tree);

			var list = new List<TreeNode> ();
			list.Add (tree);
			tree.GetNodes (list);

			foreach (var treeNode in list)
			{
				var n = new LevelNode (treeNode.Node.Guid, this.GetLevel (treeNode));
				this.levelNodes.Add (n);
			}
		}

		private void Insert(TreeNode tree)
		{
			var childrens = this.inputNodes.Nodes.Where (x => x.Parent == tree.Node.Guid).OrderBy (x => x.Position);

			foreach (var children in childrens)
			{
				var n = new TreeNode (tree, children);
				tree.Childrens.Add (n);

				this.Insert (n);
			}
		}

		private int GetLevel(TreeNode treeNode)
		{
			int level = 0;

			while (treeNode.Parent != null)
			{
				treeNode = treeNode.Parent;
				level++;
			}

			return level;
		}


		private class TreeNode
		{
			public TreeNode(TreeNode parent, ParentPositionNode node)
			{
				this.parent = parent;
				this.node   = node;

				this.childrens = new List<TreeNode> ();
			}

			public TreeNode Parent
			{
				get
				{
					return this.parent;
				}
			}

			public ParentPositionNode Node
			{
				get
				{
					return this.node;
				}
			}

			public List<TreeNode> Childrens
			{
				get
				{
					return this.childrens;
				}
			}

			public void GetNodes(List<TreeNode> list)
			{
				foreach (var children in this.childrens)
				{
					list.Add (children);
					children.GetNodes (list);
				}
			}

			private readonly TreeNode parent;
			private readonly ParentPositionNode node;
			private readonly List<TreeNode> childrens;
		}


		private readonly AbstractNodesGetter<ParentPositionNode> inputNodes;
		private readonly DataAccessor					accessor;
		private readonly BaseType						baseType;
		private readonly List<LevelNode>				levelNodes;
	}
}
