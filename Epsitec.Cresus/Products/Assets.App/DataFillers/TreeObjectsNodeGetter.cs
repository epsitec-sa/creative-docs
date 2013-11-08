//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.DataFillers
{
	/// <summary>
	/// Gère l'accès "en arbre" à des données quelconques en provenance de IGetData.
	/// </summary>
	public class TreeObjectsNodeGetter
	{
		public TreeObjectsNodeGetter(IGetData inputData)
		{
			this.inputData = inputData;

			this.nodes       = new List<Node> ();
			this.nodeIndexes = new List<int> ();
		}


		public int NodesCount
		{
			get
			{
				return this.nodeIndexes.Count;
			}
		}

		public Node GetNode(int index)
		{
			return this.nodes[this.nodeIndexes[index]];
		}


		public bool IsAllCompacted
		{
			get
			{
				return !this.nodes.Where (x => x.Type == TreeTableTreeType.Expanded).Any ();
			}
		}

		public bool IsAllExpanded
		{
			get
			{
				return !this.nodes.Where (x => x.Type == TreeTableTreeType.Compacted).Any ();
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

			if (node.Type == TreeTableTreeType.Compacted)
			{
				this.nodes[i] = new Node (node.Guid, node.Level, TreeTableTreeType.Expanded);
			}
			else if (node.Type == TreeTableTreeType.Expanded)
			{
				this.nodes[i] = new Node (node.Guid, node.Level, TreeTableTreeType.Compacted);
			}

			this.UpdateNodeIndexes ();
		}

		public void CompactAll()
		{
			//	Compacte toutes les lignes.
			for (int i=0; i<this.nodes.Count; i++)
			{
				var node = this.nodes[i];

				if (node.Type == TreeTableTreeType.Expanded)
				{
					this.nodes[i] = new Node (node.Guid, node.Level, TreeTableTreeType.Compacted);
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

				if (node.Type == TreeTableTreeType.Compacted)
				{
					this.nodes[i] = new Node (node.Guid, node.Level, TreeTableTreeType.Expanded);
				}
			}

			this.UpdateNodeIndexes ();
		}
	
		
		public void UpdateData()
		{
			//	Met à jour toutes les données en mode étendu.
			this.nodes.Clear ();

			int count = this.DataCount;
			for (int i=0; i<count; i++)
			{
				Guid currentGuid;
				int currentLevel;
				this.GetData (i, out currentGuid, out currentLevel);

				//	Par défaut, on considére que la ligne ne peut être ni étendue
				//	ni compactée.
				var type = TreeTableTreeType.Final;

				if (i < count-2)
				{
					Guid nextGuid;
					int nextLevel;
					this.GetData (i+1, out nextGuid, out nextLevel);

					//	Si le noeud suivant a un niveau plus élevé, il s'agit d'une
					//	ligne pouvant être étendue ou compactée.
					if (nextLevel > currentLevel)
					{
						type = TreeTableTreeType.Expanded;
					}
				}

				var node = new Node (currentGuid, currentLevel, type);
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

				if (node.Type == TreeTableTreeType.Compacted)
				{
					skip = true;
					skipLevel = node.Level;
				}

				this.nodeIndexes.Add (i);
			}
		}


		#region GetData accessor
		private int DataCount
		{
			get
			{
				return this.inputData.DataCount;
			}
		}

		private void GetData(int row, out Guid guid, out int level)
		{
			//	Retourne une donnée.
			this.inputData.GetData (row, out guid, out level);
		}
		#endregion


		private readonly IGetData				inputData;
		private readonly List<Node>				nodes;
		private readonly List<int>				nodeIndexes;
	}
}
