//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.NodesGetter
{
	/// <summary>
	/// Gère l'accès en lecture "en arbre" à des données quelconques en provenance
	/// de inputNodes. Plusieurs procédures permettent de choisir comment l'arbre
	/// est visible, en compactant/étendant des noeuds.
	/// LevelNode -> TreeNode
	/// </summary>
	public class TreeObjectsNodesGetter : AbstractNodesGetter<TreeNode>  // outputNodes
	{
		public TreeObjectsNodesGetter(AbstractNodesGetter<LevelNode> inputNodes)
		{
			this.inputNodes = inputNodes;

			this.nodes       = new List<TreeNode> ();
			this.nodeIndexes = new List<int> ();
		}


		public bool InputIsMerge;


		public override int Count
		{
			get
			{
				return this.nodeIndexes.Count;
			}
		}

		public override TreeNode this[int index]
		{
			get
			{
				if (index < this.nodeIndexes.Count)
				{
					int i = this.nodeIndexes[index];
					if (i < this.nodes.Count)
					{
						return this.nodes[i];
					}
				}

				return TreeNode.Empty;
			}
		}


		public bool IsAllCompacted
		{
			get
			{
				return !this.nodes.Where (x => x.Type == NodeType.Expanded).Any ();
			}
		}

		public bool IsAllExpanded
		{
			get
			{
				return !this.nodes.Where (x => x.Type == NodeType.Compacted).Any ();
			}
		}


		public int SearchBestIndex(Guid value)
		{
			//	Retourne l'index ayant un Guid donné. Si la ligne correspondante
			//	est cachée, on est assez malin pour retourner la prochaine ligne
			//	visible, vers le haut.
			int index = -1;

			if (!value.IsEmpty)
			{
				var i = this.nodes.FindIndex (x => x.Guid == value);
				if (i != -1)
				{
					index = this.nodeIndexes.Where (x => x <= i).Count () - 1;
				}
			}

			return index;
		}


		public int VisibleToAll(int index)
		{
			if (index >= 0 && index < this.nodeIndexes.Count)
			{
				return this.nodeIndexes[index];
			}
			else
			{
				return -1;
			}
		}

		public int AllToVisible(int index)
		{
			return this.nodeIndexes.IndexOf (index);
		}


		public void CompactOrExpand(int index)
		{
			//	Etend ou compacte une ligne (inverse son mode actuel).
			int i = this.nodeIndexes[index];
			var node = this.nodes[i];

			if (node.Type == NodeType.Compacted)
			{
				this.nodes[i] = new TreeNode (node.Guid, node.BaseType, node.Level, NodeType.Expanded);
			}
			else if (node.Type == NodeType.Expanded)
			{
				this.nodes[i] = new TreeNode (node.Guid, node.BaseType, node.Level, NodeType.Compacted);
			}

			this.UpdateNodeIndexes ();
		}

		public void CompactAll()
		{
			//	Compacte toutes les lignes.
			for (int i=0; i<this.nodes.Count; i++)
			{
				var node = this.nodes[i];

				if (node.Type == NodeType.Expanded)
				{
					this.nodes[i] = new TreeNode (node.Guid, node.BaseType, node.Level, NodeType.Compacted);
				}
			}

			this.UpdateNodeIndexes ();
		}

		public void ExpandAll()
		{
			//	Etend toutes les lignes.
			for (int i=0; i<this.nodes.Count; i++)
			{
				var node = this.nodes[i];

				if (node.Type == NodeType.Compacted)
				{
					this.nodes[i] = new TreeNode (node.Guid, node.BaseType, node.Level, NodeType.Expanded);
				}
			}

			this.UpdateNodeIndexes ();
		}

		
		public override void UpdateData()
		{
			//	Met à jour toutes les données en conservant le mode compacté/étendu.
			var compactedGuids = this.nodes
				.Where (node => node.Type == NodeType.Compacted)
				.Select (node => node.Guid)
				.ToArray ();  // check initial

			this.nodes.Clear ();

			int count = this.inputNodes.Count;
			for (int i=0; i<count; i++)
			{
				var currentNode = this.inputNodes[i];

				//	Par défaut, on considére que la ligne ne peut être ni étendue
				//	ni compactée.
				var type = (this.InputIsMerge && currentNode.BaseType == BaseType.Groups)
					? NodeType.Compacted
					: NodeType.Final;

				if (i < count-1)
				{
					var nextNode = this.inputNodes[i+1];

					//	Si le prochain noeud a un niveau plus élevé, il s'agit d'une
					//	ligne pouvant être étendue ou compactée. On lui redonne alors
					//	son mode initial. S'il s'agit d'une nouvelle ligne (inconnue
					//	lors du check initial), on lui met le mode étendu.
					if (nextNode.Level > currentNode.Level)
					{
						bool isCompacted = compactedGuids.Where (guid => guid == currentNode.Guid).Any ();
						type = isCompacted ? NodeType.Compacted : NodeType.Expanded;
					}
				}

				var node = new TreeNode (currentNode.Guid, currentNode.BaseType, currentNode.Level, type);
				this.nodes.Add (node);
			}

			this.UpdateNodeIndexes ();
		}


		private void UpdateNodeIndexes()
		{
			//	Met à jour l'accès aux noeuds (nodeIndexes) en sautant les
			//	noeuds cachés.
			this.nodeIndexes.Clear ();

			bool skip = false;
			int skipLevel = 0;

			for (int i=0; i<this.nodes.Count; i++)
			{
				var node = this.nodes[i];

				if (skip)
				{
					if (node.Level <= skipLevel)
					{
						skip = false;
					}
					else
					{
						continue;
					}
				}

				if (node.Type == NodeType.Compacted)
				{
					skip = true;
					skipLevel = node.Level;
				}

				this.nodeIndexes.Add (i);
			}
		}


		private readonly AbstractNodesGetter<LevelNode>	inputNodes;
		private readonly List<TreeNode>					nodes;
		private readonly List<int>						nodeIndexes;
	}
}
