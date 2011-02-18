using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.DataLayer.Loader;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;
using System.Linq;


namespace Epsitec.Cresus.DataLayer.UnitTests.Loader
{


	[TestClass]
	public sealed class UnitTestAliasNode
	{


		[TestMethod]
		public void AliasNodeConstructorTest1()
		{
			AliasNode node = new AliasNode ("node");

			Assert.AreEqual ("node", node.Name);
			Assert.IsNull (node.GetParent());
			Assert.IsNotNull (node.Alias);
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void AliasNodeConstructorTest2()
		{
			AliasNode_Accessor node1 = new AliasNode_Accessor ("node1");
			AliasNode_Accessor node2 = new AliasNode_Accessor (node1, "node2");

			Assert.AreEqual ("node2", node2.Name);
			Assert.AreSame (node1.Target, node2.parent.Target);
			Assert.IsNotNull (node2.Alias);
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void AliasNodeConstructorArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new AliasNode ("")
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new AliasNode (null)
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new AliasNode_Accessor (new AliasNode_Accessor ("node1"), "")
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new AliasNode_Accessor (new AliasNode_Accessor ("node1"), null)
			);
		}


		[TestMethod]
		public void CreateChildTest()
		{
			AliasNode node1 = new AliasNode ("node1");
			AliasNode node2 = node1.CreateChild ("node2");

			Assert.AreEqual ("node2", node2.Name);
			Assert.AreSame (node1, node2.GetParent ());
			Assert.IsNotNull (node2.Alias);
		}


		[TestMethod]
		public void CreateChildTestArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new AliasNode ("node1").CreateChild (null)
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new AliasNode ("node1").CreateChild ("")
			);
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void CreateNewAliasTest()
		{
			List<AliasNode_Accessor> nodes = new List<AliasNode_Accessor> ();

			nodes.Add (new AliasNode_Accessor ("node0"));
			nodes.Add (nodes[0].CreateChild ("node1"));
			nodes.Add (nodes[0].CreateChild ("node2"));
			nodes.Add (nodes[1].CreateChild ("node3"));
			nodes.Add (nodes[2].CreateChild ("node4"));
			nodes.Add (nodes[4].CreateChild ("node5"));
			nodes.Add (nodes[4].CreateChild ("node6"));
			nodes.Add (nodes[4].CreateChild ("node7"));
			nodes.Add (nodes[1].CreateChild ("node8"));
			nodes.Add (nodes[3].CreateChild ("node9"));

			HashSet<string> aliases = new HashSet<string> ();

			for (int i = 0; i < 1000; i++)
			{
				int randomIndex = this.dice.Next (0, 9);
				AliasNode_Accessor randomNode = nodes[randomIndex];

				string alias = randomNode.CreateNewAlias ();

				Assert.IsFalse (aliases.Contains (alias));

				aliases.Add (alias);
			}
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void GenerateNewAliasTest()
		{
			AliasNode_Accessor node = new AliasNode_Accessor ("node");

			HashSet<string> aliases = new HashSet<string> ();

			for (int i = 0; i < 1000; i++)
			{
				string alias = node.GenerateNewAlias ();

				Assert.IsFalse (aliases.Contains (alias));

				aliases.Add (alias);
			}
		}


		[TestMethod]
		public void GetChildTest()
		{
			AliasNode node = new AliasNode ("node");

			Dictionary<string, List<AliasNode>> children = new Dictionary<string, List<AliasNode>> ();

			for (int i = 0; i < 5; i++)
			{
				string name = "name" + i;

				children[name] = new List<AliasNode> ();

				for (int j = 0; j < 5; j++)
				{
					children[name].Add (node.CreateChild (name));
				}
			}

			foreach (string name in children.Keys)
			{
				AliasNode node1 = children[name].First ();
				AliasNode node2 = node.GetChild (name);

				Assert.AreSame (node1, node2);
			}
		}


		[TestMethod]
		public void GetChildTestArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new AliasNode ("node1").GetChild (null)

			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new AliasNode ("node1").GetChild ("")
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new AliasNode ("node1").GetChild ("test")
			);
		}


		[TestMethod]
		public void GetChildrenTest()
		{
			AliasNode node = new AliasNode ("node");

			Dictionary<string, List<AliasNode>> children = new Dictionary<string, List<AliasNode>> ();

			for (int i = 0; i < 5; i++)
			{
				string name = "name" + i;

				children[name] = new List<AliasNode> ();

				for (int j = 0; j < 5; j++)
				{
					children[name].Add (node.CreateChild (name));
				}
			}

			foreach (string name in children.Keys)
			{
				List<AliasNode> children1 = children[name];
				List<AliasNode> children2 = node.GetChildren (name).ToList ();
				
				CollectionAssert.AreEquivalent (children1, children2);
			}
		}


		[TestMethod]
		public void GetChildrenArgumentChec()
		{
			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new AliasNode ("node1").GetChildren (null)
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new AliasNode ("node1").GetChildren ("")
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new AliasNode ("node1").GetChildren ("test")
			);
		}


		[TestMethod]
		public void GetParentTest()
		{
			AliasNode node1 = new AliasNode ("node1");
			AliasNode node2 = node1.CreateChild ("node2");

			Assert.AreSame (node1, node2.GetParent ());
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void AliasTest()
		{
			AliasNode node = new AliasNode ("node");

			Assert.IsNotNull (node.Alias);
		}


		[TestMethod]
		public void NameTest()
		{
			AliasNode node = new AliasNode ("node");

			Assert.AreEqual ("node", node.Name);
		}


		private System.Random dice = new System.Random ();


	}


}
