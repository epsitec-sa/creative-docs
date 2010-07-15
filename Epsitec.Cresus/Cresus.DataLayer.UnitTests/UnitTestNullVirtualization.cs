using Epsitec.Cresus.Core;

using Epsitec.Cresus.DataLayer.Context;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Epsitec.Cresus.DataLayer
{


	[TestClass]
	public class UnitTestNullVirtualization
	{


		[ClassInitialize]
		public void Initialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}


		[TestMethod]
		public void CreateDatabase()
		{
			TestHelper.PrintStartTest ("Create database");

			this.CreateDatabaseHelper ();
		}


		private void CreateDatabaseHelper()
		{
			Database.CreateAndConnectToDatabase ();

			Assert.IsTrue (Database.DbInfrastructure.IsConnectionOpen);

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure) { EnableNullVirtualization = true })
			{
				Database2.PupulateDatabase (dataContext);
			}
		}


	}


}
