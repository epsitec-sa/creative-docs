using Epsitec.Cresus.Database;
using Epsitec.Cresus.Database.Services;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.ImportExport;
using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.UnitTests.Entities;
using Epsitec.Cresus.DataLayer.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.IO;


namespace Epsitec.Cresus.DataLayer.UnitTests.ImportExport
{


	[TestClass]
	public sealed class UnitTestEpsitecEntitySerializer
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}


		[ClassCleanup]
		public static void ClassCleanup()
		{
			DatabaseHelper.DisconnectFromDatabase ();
		}


		[TestInitialize]
		public void TestInitialize()
		{
			DatabaseHelper.CreateAndConnectToDatabase ();

			Assert.IsTrue (DatabaseHelper.DbInfrastructure.IsConnectionOpen);

			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					DatabaseCreator2.PupulateDatabase (dataContext);
				}
			}
		}


		[TestMethod]
		public void SimpleExportImport()
		{
			FileInfo file = new FileInfo ("test.xml");

			DbLogEntry dbLogEntry = DatabaseHelper.DbInfrastructure.Logger.CreateLogEntry (new DbId (1));

			EpsitecEntitySerializer.Export (file, DatabaseHelper.DbInfrastructure);
			EpsitecEntitySerializer.Import (file, DatabaseHelper.DbInfrastructure, dbLogEntry);

			using (DataInfrastructure dataInfrastructure = new DataInfrastructure(DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
					NaturalPersonEntity gertrude = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (2)));
					NaturalPersonEntity hans = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (3)));

					Assert.IsTrue (DatabaseCreator2.CheckAlfred (alfred));
					Assert.IsTrue (DatabaseCreator2.CheckGertrude (gertrude));
					Assert.IsTrue (DatabaseCreator2.CheckHans (hans));
				}
			}

			EpsitecEntitySerializer.CleanDatabase (file, DatabaseHelper.DbInfrastructure);

			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					Assert.IsNull (dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1))));
					Assert.IsNull (dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (2))));
					Assert.IsNull (dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (3))));

					Assert.IsNull (dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1))));
					Assert.IsNull (dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (2))));
					Assert.IsNull (dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (3))));
					Assert.IsNull (dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (4))));

					Assert.IsNull (dataContext.ResolveEntity<UriSchemeEntity> (new DbKey (new DbId (1))));

					Assert.IsNull (dataContext.ResolveEntity<PersonGenderEntity> (new DbKey (new DbId (1))));
					Assert.IsNull (dataContext.ResolveEntity<PersonGenderEntity> (new DbKey (new DbId (2))));

					Assert.IsNull (dataContext.ResolveEntity<PersonTitleEntity> (new DbKey (new DbId (1))));
					Assert.IsNull (dataContext.ResolveEntity<PersonTitleEntity> (new DbKey (new DbId (2))));

					Assert.IsNull (dataContext.ResolveEntity<LanguageEntity> (new DbKey (new DbId (1))));
					Assert.IsNull (dataContext.ResolveEntity<LanguageEntity> (new DbKey (new DbId (2))));
				}
			}
		}


	}


}
