using Epsitec.Aider.Data.Eerv;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.IO;

using System.Linq;


namespace Aider.Tests.Vs
{


	[TestClass]
	public class UnitTestEervMainDataLoader
	{


		[TestMethod]
		public void LoadEervGroupDefinitionsTest()
		{
			var groupDefinitions = EervMainDataLoader.LoadEervGroupDefinitions (this.GroupDefinitionFile).ToList ();

			Assert.AreEqual (657, groupDefinitions.Count);

			var g1 = new EervGroupDefinition ("0100000000", "Paramètres transversaux", false, 0)
			{
				Parent = null,
			};
			this.CheckForEquality (g1, groupDefinitions[0]);

			var g2 = new EervGroupDefinition ("0102000000", "Personnel EERV", false, 1)
			{
				Parent = new EervGroupDefinition ("0100000000", null, false, 0),
			};
			this.CheckForEquality (g2, groupDefinitions[89]);

			var g3 = new EervGroupDefinition ("0301010000", "Assemblée régionale", false, 2)
			{
				Parent = new EervGroupDefinition ("0301000000", null, false, 0),
			};
			this.CheckForEquality (g3, groupDefinitions[281]);

			var g4 = new EervGroupDefinition ("0303030100", "Catéchumènes 2007-10-12", false, 3)
			{
				Parent = new EervGroupDefinition ("0303030000", null, false, 0),
			};
			this.CheckForEquality (g4, groupDefinitions[315]);

			var g5 = new EervGroupDefinition ("0604010203", "Délégués CER", false, 4)
			{
				Parent = new EervGroupDefinition ("0604010200", null, false, 0),
			};
			this.CheckForEquality (g5, groupDefinitions[591]);
		}


		private void CheckForEquality(EervGroupDefinition expected, EervGroupDefinition actual)
		{
			Assert.AreEqual (expected.Id, actual.Id);
			Assert.AreEqual (expected.Name, actual.Name);
			Assert.AreEqual (expected.GroupLevel, actual.GroupLevel);

			if (expected.Parent == null)
			{
				Assert.IsNull (actual.Parent);
			}
			else
			{
				Assert.IsNotNull (actual.Parent);
				Assert.AreEqual (expected.Parent.Id, actual.Parent.Id);
			}
		}


		private readonly FileInfo GroupDefinitionFile = new FileInfo (@"S:\Epsitec.Cresus\App.Aider\Samples\EERV Main\Groupe definition.xlsx");
		

	}


}
