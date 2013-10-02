using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	//-public class Tree
	//-{
	//-	public Tree()
	//-	{
	//-		this.root = new TreeNode (null);
	//-	}
	//-
	//-	public TreeNode Root
	//-	{
	//-		get
	//-		{
	//-			return this.root;
	//-		}
	//-	}
	//-
	//-	private readonly TreeNode root;
	//-}

	public class TreeNode
	{
		public TreeNode(int value)
		{
			this.treeNodes = new List<TreeNode> ();
			this.value = value;
		}

		public List<TreeNode> TreeNodes
		{
			get
			{
				return this.treeNodes;
			}
		}

		public int Value
		{
			get
			{
				return this.value;
			}
		}

		public IEnumerable<TreeNode> Childrens
		{
			get
			{
				var list = new List<TreeNode> ();
				list.Add (this);
				this.PutChildrens (list);
				return list;
			}
		}

		private void PutChildrens(List<TreeNode> list)
		{
			foreach (var children in this.treeNodes)
			{
				list.Add (children);

				if (children.treeNodes.Any ())
				{
					children.PutChildrens (list);
				}
			}
		}

		private readonly List<TreeNode> treeNodes;
		private readonly int value;
	}

	public static class TreeTest
	{
		public static void Test()
		{
			var root = new TreeNode (1);

			root.TreeNodes.Add (new TreeNode (10));
			root.TreeNodes.Add (new TreeNode (11));
			root.TreeNodes.Add (new TreeNode (12));

			root.TreeNodes[1].TreeNodes.Add (new TreeNode (110));
			root.TreeNodes[1].TreeNodes.Add (new TreeNode (111));
			root.TreeNodes[1].TreeNodes.Add (new TreeNode (112));

			root.TreeNodes[1].TreeNodes[2].TreeNodes.Add (new TreeNode (1120));
			root.TreeNodes[1].TreeNodes[2].TreeNodes.Add (new TreeNode (1121));

			foreach (var x in root.Childrens)
			{
				System.Console.WriteLine (x.Value);
			}
		}
	}
}
