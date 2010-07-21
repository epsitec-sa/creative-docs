using Epsitec.Cresus.DataLayer.Loader;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;
using System.Linq;


namespace Epsitec.Cresus.DataLayer.UnitTests
{


	[TestClass]
	public sealed class UnitTestAliasNode
	{


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void AliasNodeConstructorTest1()
		{
			AliasNode_Accessor node = new AliasNode_Accessor ("node");

			Assert.AreEqual ("node", node.Name);
			Assert.IsNull (node.parent);
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
		public void CreateChildTest()
		{
			AliasNode_Accessor node1 = new AliasNode_Accessor ("node1");
			AliasNode_Accessor node2 = node1.CreateChild ("node2");

			Assert.AreEqual ("node2", node2.Name);
			Assert.AreSame (node1.Target, node2.parent.Target);
			Assert.IsNotNull (node2.Alias);
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
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void GetChildTest()
		{
			AliasNode_Accessor node = new AliasNode_Accessor ("node");

			Dictionary<string, List<AliasNode_Accessor>> children = new Dictionary<string, List<AliasNode_Accessor>> ();

			for (int i = 0; i < 5; i++)
			{
				string name = "name" + i;

				children[name] = new List<AliasNode_Accessor> ();

				for (int j = 0; j < 5; j++)
				{
					children[name].Add (node.CreateChild (name));
				}
			}

			foreach (string name in children.Keys)
			{
				AliasNode_Accessor node1 = children[name].First ();
				AliasNode_Accessor node2 = node.GetChild (name);

				Assert.AreSame (node1.Target, node2.Target);
			}
		}


		// Because of the limitations of the AliasNode_Accessor, it is not possible to test
		// a private method with a generic return type. In the following test, the call to
		// node.getChildren(...) will throw an exception. Therefore, this test is ignored, but I
		// leave it here, in case the support for private method with a generic return type is
		// improved in the next versions of Visual Studio.
		// Marc

		[TestMethod]
		[Ignore]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void GetChildrenTest()
		{
			AliasNode_Accessor node = new AliasNode_Accessor ("node");

			Dictionary<string, List<AliasNode_Accessor>> children = new Dictionary<string, List<AliasNode_Accessor>> ();

			for (int i = 0; i < 5; i++)
			{
				string name = "name" + i;

				children[name] = new List<AliasNode_Accessor> ();

				for (int j = 0; j < 5; j++)
				{
					children[name].Add (node.CreateChild (name));
				}
			}

			foreach (string name in children.Keys)
			{
				List<AliasNode_Accessor> children1 = children[name];
				List<AliasNode_Accessor> children2 = node.GetChildren (name).ToList ();
				
				CollectionAssert.AreEquivalent (children1, children2);
			}
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void GetParentTest()
		{
			AliasNode_Accessor node1 = new AliasNode_Accessor ("node1");
			AliasNode_Accessor node2 = node1.CreateChild ("node2");

			Assert.AreSame (node1.Target, node2.GetParent ().Target);
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void AliasTest()
		{
			AliasNode_Accessor node = new AliasNode_Accessor ("node");

			Assert.IsNotNull (node.Alias);
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void NameTest()
		{
			AliasNode_Accessor node = new AliasNode_Accessor ("node");

			Assert.AreEqual ("node", node.Name);
		}


		private System.Random dice = new System.Random ();


	}


}
