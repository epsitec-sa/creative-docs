//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Support;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.Core.Helpers;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Business;

namespace Epsitec.Cresus.Core.Controllers.BusinessDocumentControllers
{
	public class TreeEngine
	{
		public TreeEngine()
		{
			this.root = new TreeNode ();
		}

		public TreeNode Root
		{
			get
			{
				return this.root;
			}
		}

		public void Create(IList<AbstractDocumentItemEntity> lines)
		{
			this.root.Childrens.Clear ();

			foreach (var line in lines)
			{
				this.InsertEntity (line);
			}
		}

		public TreeNode Search(AbstractDocumentItemEntity line)
		{
			this.InitialiseForDeepNext ();

			var current = this.root;
			while (true)
			{
				current = TreeEngine.DeepNext (current);

				if (current == null)
				{
					break;
				}

				if (current.Entity == line)
				{
					return current;
				}
			}

			return null;
		}

		public void RegenerateGroupIndexes()
		{
			this.InitialiseForDeepNext ();

			var current = this.root;
			while (true)
			{
				current = TreeEngine.DeepNext (current);

				if (current == null)
				{
					break;
				}

				TreeEngine.RegenerateGroupIndex (current);
			}
		}

		private static void RegenerateGroupIndex(TreeNode node)
		{
			if (node.Entity == null)
			{
				var regeneratedNode = node;
				int regeneratedGroupIndex = 0;

				while (true)
				{
					var parent = node.Parent;

					if (parent == null)
					{
						break;
					}

					int i = TreeEngine.GetIndex (parent, node);

					if (parent.Deep >= 0 && i != -1)
					{
						regeneratedGroupIndex = LinesEngine.LevelReplace (regeneratedGroupIndex, parent.Deep, i+1);
					}

					node = parent;
				}

				regeneratedNode.FullGroupIndex = regeneratedGroupIndex;
			}
			else
			{
				node.Entity.GroupIndex = node.Parent.FullGroupIndex;
			}
		}

		private static int GetIndex(TreeNode parent, TreeNode node)
		{
			int index = 0;

			foreach (var child in parent.Childrens)
			{
				if (child == node)
				{
					break;
				}

				if (child.Entity == null)
				{
					index++;
				}
			}

			return index;
		}


		private void InsertEntity(AbstractDocumentItemEntity entity)
		{
			int level = LinesEngine.GetLevel (entity.GroupIndex);
			int progressGroupIndex = 0;

			var parent = this.root;

			for (int i = 0; i < level; i++)
			{
				int rank = LinesEngine.LevelExtract (entity.GroupIndex, i);
				progressGroupIndex = LinesEngine.LevelReplace (progressGroupIndex, i, rank);

				var next = parent.Childrens.Where (x => x.FullGroupIndex == progressGroupIndex).FirstOrDefault ();

				if (next == null)
				{
					next = new TreeNode (progressGroupIndex);
					parent.Childrens.Add (next);
				}

				parent = next;
			}

			parent.Childrens.Add (new TreeNode (entity));
		}


		private void InitialiseForDeepNext()
		{
			//	Initialise les propriétés Parent de tous les noeuds de l'arbre.
			//	Cela est indispensable avant de pouvoir utiliser DeepNext.
			this.root.Deep = 0;
			TreeEngine.InitialiseForDeepNext (this.root);
		}

		private static void InitialiseForDeepNext(TreeNode parent)
		{
			foreach (var node in parent.Childrens)
			{
				node.Parent = parent;
				node.Deep = parent.Deep+1;

				TreeEngine.InitialiseForDeepNext (node);
			}
		}

		private static TreeNode DeepNext(TreeNode node)
		{
			if (node.Childrens.Count == 0)
			{
				while (true)
				{
					if (node.Parent == null)
					{
						return null;
					}

					var parent = node.Parent;
					int index = parent.Childrens.IndexOf (node) + 1;

					if (index < parent.Childrens.Count)
					{
						return parent.Childrens[index];
					}

					node = parent;
				}
			}
			else
			{
				return node.Childrens[0];
			}
		}


		private readonly TreeNode root;
	}
}
