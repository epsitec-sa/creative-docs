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

			var value1 = dic["Strings.Application.Name.fr".Split('.')];
			Assert.AreEqual ("Strings.Application.Name.fr", value1);

			dic["Strings.Application.Name.fr".Split ('.')] = "new value";
			Assert.AreEqual (dic["Strings.Application.Name.fr".Split ('.')], "new value");

			dic["Strings.Application.Name.en".Split ('.')] = "english";
			Assert.AreEqual (dic["Strings.Application.Name.en".Split ('.')], "english");

			var value2 = dic["Strings.Application.Name".Split ('.')];
			var dic2 = new CompositeDictionary (value2 as Dictionary<IKey, object>);
		}

		[TestMethod]
		public void ContainsKey()
		{
			var dic = CompositeDictionaryTest.CreateDic1 ();
			Assert.IsTrue (dic.ContainsKey (CompositeKey.Empty));
			Assert.IsTrue (dic.ContainsKey ("Strings"));
			Assert.IsTrue (dic.ContainsKey ("Strings.Root".Split ('.')));
			Assert.IsTrue (dic.ContainsKey ("Strings.Application".Split ('.')));
			Assert.IsTrue (dic.ContainsKey ("Strings.Application.Name".Split ('.')));
			Assert.IsTrue (dic.ContainsKey ("Strings.Application.Name.fr".Split ('.')));

			Assert.IsFalse (dic.ContainsKey ("Strings.Application.Name.fr.xxx".Split ('.')));
			Assert.IsFalse (dic.ContainsKey ("Strings.Application.Name.en".Split ('.')));
		}

		[TestMethod]
		public void TryGetValue()
		{
			var dic = CompositeDictionaryTest.CreateDic1 ();
			object value;
			Assert.IsTrue (dic.TryGetValue ("Strings.Application.Name.fr".Split ('.'), out value));
			Assert.AreEqual (value, "Strings.Application.Name.fr");
			Assert.IsTrue (dic.TryGetValue ("Strings.Application.Name".Split ('.'), out value));
			Assert.IsTrue (value is IDictionary<IKey, object>);

			Assert.IsFalse (dic.TryGetValue ("Strings.Application.Name.fr.xxx".Split ('.'), out value));
			Assert.IsFalse (dic.TryGetValue ("Strings.Application.Name.en".Split ('.'), out value));
			Assert.IsFalse (dic.TryGetValue (CompositeKey.Empty, out value));
		}

		[TestMethod]
		public void Remove()
		{
			var dic = CompositeDictionaryTest.CreateDic1 ();
			var keys0 = dic.Keys;
			Assert.IsTrue (dic.Remove ("Strings.Application.Name.fr".Split ('.')));
			var keys1 = dic.Keys;
			var diff1 = keys0.Except (keys1).Single ();
			Assert.AreEqual (Key.Create("Strings.Application.Name.fr".Split ('.')), diff1);

			Assert.IsFalse (dic.Remove ("Strings.Application.Name.fr.xxx".Split ('.')));
			var keys2 = dic.Keys;
			var diff2 = keys1.Except (keys2);
			Assert.AreEqual (0, diff2.Count ());
		}

		[TestMethod]
		public void SolutionResources()
		{
			var workspace = Workspace.LoadSolution (TestData.CresusGraphSolutionPath);
			var solution = workspace.CurrentSolution;
			var project = solution.Projects.First ();
			var solutionResources = CompositeDictionaryTest.LoadResources (solution);
			var projectResources = CompositeDictionary.Create (solutionResources[project.Id, "Epsitec", "Cresus", "Res"]);
		}

		private static CompositeDictionary CreateDic1()
		{
			var dic = new CompositeDictionary ();
			dic.Add ("Strings.Application.Name.fr".Split ('.'), "Strings.Application.Name.fr");
			dic.Add ("Strings.Application.Name.de".Split ('.'), "Strings.Application.Name.de");
			dic.Add ("Strings.Root".Split ('.'), "Strings.Root");
			return dic;
		}

		private static CompositeDictionary LoadResources(ISolution solution)
		{
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

				var key = Key.Create (
					this.project.Project.Id,
					this.module.Info.ResourceNamespace.Split('.'),
					"Res",
					this.bundle.Name,
					item.Name.Split ('.'),
					this.bundle.Culture);

				map[key] = item;
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

			public readonly CompositeDictionary map = new CompositeDictionary ();

			private SolutionResource solution;
			private ProjectResource project;
			private ResourceModule module;
			private ResourceBundle bundle;
		}
	}
}
