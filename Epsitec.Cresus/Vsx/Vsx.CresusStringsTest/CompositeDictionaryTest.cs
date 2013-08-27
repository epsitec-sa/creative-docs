using System;
using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.ResourceManagement;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Roslyn.Services;

namespace Epsitec.Cresus.Strings
{
	[TestClass]
	public class CompositeDictionaryTest
	{
		[TestMethod]
		public void Keys()
		{
			var dic = CompositeDictionaryTest.CreateDic1 ();
			var keys = dic.Keys;
		}

		[TestMethod]
		public void Values()
		{
			var dic = CompositeDictionaryTest.CreateDic1 ();
			var values = dic.Values;
		}

		[TestMethod]
		public void Indexer()
		{
			var dic = CompositeDictionaryTest.CreateDic1 ();

			var value1 = dic[Key.Split ("Strings.Application.Name.fr")];
			Assert.AreEqual ("Strings.Application.Name.fr", value1);

			dic[Key.Split ("Strings.Application.Name.fr")] = "new value";
			Assert.AreEqual (dic[Key.Split ("Strings.Application.Name.fr")], "new value");

			dic[Key.Split ("Strings.Application.Name.en")] = "english";
			Assert.AreEqual (dic[Key.Split ("Strings.Application.Name.en")], "english");

			var value2 = dic[Key.Split ("Strings.Application.Name")];
			var dic2 = new CompositeDictionary (value2 as Dictionary<IKey, object>);
		}

		[TestMethod]
		public void ContainsKey()
		{
			var dic = CompositeDictionaryTest.CreateDic1 ();
			Assert.IsTrue (dic.ContainsKey (CompositeKey.Empty));
			Assert.IsTrue (dic.ContainsKey (Key.Split ("Strings")));
			Assert.IsTrue (dic.ContainsKey (Key.Split ("Strings.Root")));
			Assert.IsTrue (dic.ContainsKey (Key.Split ("Strings.Application")));
			Assert.IsTrue (dic.ContainsKey (Key.Split ("Strings.Application.Name")));
			Assert.IsTrue (dic.ContainsKey (Key.Split ("Strings.Application.Name.fr")));

			Assert.IsFalse (dic.ContainsKey (Key.Split ("Strings.Application.Name.fr.xxx")));
			Assert.IsFalse (dic.ContainsKey (Key.Split ("Strings.Application.Name.en")));
		}

		[TestMethod]
		public void TryGetValue()
		{
			var dic = CompositeDictionaryTest.CreateDic1 ();
			object value;
			Assert.IsTrue (dic.TryGetValue (Key.Split ("Strings.Application.Name.fr"), out value));
			Assert.AreEqual (value, "Strings.Application.Name.fr");
			Assert.IsTrue (dic.TryGetValue (Key.Split ("Strings.Application.Name"), out value));
			Assert.IsTrue (value is IDictionary<IKey, object>);

			Assert.IsFalse (dic.TryGetValue (Key.Split ("Strings.Application.Name.fr.xxx"), out value));
			Assert.IsFalse (dic.TryGetValue (Key.Split ("Strings.Application.Name.en"), out value));
			Assert.IsFalse (dic.TryGetValue (CompositeKey.Empty, out value));
		}

		[TestMethod]
		public void Remove()
		{
			var dic = CompositeDictionaryTest.CreateDic1 ();
			var keys0 = dic.Keys;
			Assert.IsTrue (dic.Remove (Key.Split ("Strings.Application.Name.fr")));
			var keys1 = dic.Keys;
			var diff1 = keys0.Except (keys1).Single ();
			Assert.AreEqual (Key.Split("Strings.Application.Name.fr"), diff1);

			Assert.IsFalse (dic.Remove (Key.Split ("Strings.Application.Name.fr.xxx")));
			var keys2 = dic.Keys;
			var diff2 = keys1.Except (keys2);
			Assert.AreEqual (0, diff2.Count ());
		}

		[TestMethod]
		public void SolutionDic()
		{
			var dic = CompositeDictionaryTest.CreateDic2 ();
		}

		private static CompositeDictionary CreateDic1()
		{
			var dic = new CompositeDictionary ();
			dic.Add (Key.Split ("Strings.Application.Name.fr"), "Strings.Application.Name.fr");
			dic.Add (Key.Split ("Strings.Application.Name.de"), "Strings.Application.Name.de");
			dic.Add (Key.Split ("Strings.Root"), "Strings.Root");
			return dic;
		}

		private static CompositeDictionary CreateDic2()
		{
			var workspace = Workspace.LoadSolution (TestData.CresusGraphSolutionPath);
			var solution = workspace.CurrentSolution;
			var solutionResource = new SolutionResource (solution);
			var visitor = new MappingVisitor ();
			visitor.VisitSolution (solutionResource);
			return visitor.map;
		}

		private class MappingVisitor : ResourceVisitor
		{
			public override ResourceNode VisitItem(ResourceItem item)
			{
				item = base.VisitItem (item) as ResourceItem;

				try
				{
					var key = new CompositeKey (
						Key.Create (this.bundle.Culture),
						Key.Create (this.project.ToString ()),
						Key.Create (this.module.Info.ResourceNamespace),
						Key.Create (this.bundle.Name),
						Key.Split (item.Name));
					map[key] = item;
				}
				catch (NullReferenceException)
				{
				}
				return item;
			}

			public override ResourceNode VisitBundle(ResourceBundle bundle)
			{
				this.bundle = bundle;
				return base.VisitBundle (bundle);
			}

			public override ResourceNode VisitModule(ResourceModule module)
			{
				this.module = module;
				return base.VisitModule (module);
			}

			public override ResourceNode VisitProject(ProjectResource project)
			{
				this.project = project;
				return base.VisitProject (project);
			}

			public override ResourceNode VisitSolution(SolutionResource solution)
			{
				this.solution = solution;
				return base.VisitSolution (solution);
			}

			//public readonly Dictionary<IKey, object> map = new Dictionary<IKey, object> ();

			public readonly CompositeDictionary map = new CompositeDictionary ();

			private SolutionResource solution;
			private ProjectResource project;
			private ResourceModule module;
			private ResourceBundle bundle;
		}
	}
}
