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

			Assert.AreEqual (662, groupDefinitions.Count);

			var g1 = new EervGroupDefinition ("0101000000", "Fonctions générales", false)
			{
				Parent = null,
			};
			this.CheckForEquality (g1, groupDefinitions[0]);

			var g2 = new EervGroupDefinition ("0102000000", "Personnel EERV", false)
			{
				Parent = null,
			};
			this.CheckForEquality (g2, groupDefinitions[88]);

			var g3 = new EervGroupDefinition ("0301010000", "Assemblée", false)
			{
				Parent = new EervGroupDefinition ("0301000000", null, false),
			};
			this.CheckForEquality (g3, groupDefinitions[287]);

			var g4 = new EervGroupDefinition ("0303030100", "Catéchumènes 2007-09-11", false)
			{
				Parent = new EervGroupDefinition ("0303030000", null, false),
			};
			this.CheckForEquality (g4, groupDefinitions[321]);

			var g5 = new EervGroupDefinition ("0604010203", "Délégués CER", false)
			{
				Parent = new EervGroupDefinition ("0604010200", null, false),
			};
			this.CheckForEquality (g5, groupDefinitions[596]);
		}


		private void CheckForEquality(EervGroupDefinition expected, EervGroupDefinition actual)
		{
			Assert.AreEqual (expected.Id, actual.Id);
			Assert.AreEqual (expected.Name, actual.Name);

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


		private readonly FileInfo GroupDefinitionFile = new FileInfo (@"S:\Epsitec.Cresus\App.Aider\Samples\EERV Main\groupdefinition.xlsx");
		

	}


}
