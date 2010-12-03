using Epsitec.Cresus.Database;
using Epsitec.Cresus.Database.Services;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.ImportExport;
using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.UnitTests.Entities;
using Epsitec.Cresus.DataLayer.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.IO;

using System.Linq;

using System.Xml.Linq;


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

			DatabaseCreator2.PupulateDatabase ();
		}


		[TestMethod]
		public void SimpleExportImport()
		{
			FileInfo file = new FileInfo ("test.xml");

			DbLogEntry dbLogEntry = DatabaseHelper.DbInfrastructure.Logger.CreateLogEntry (new DbId (1));

			EpsitecEntitySerializer.Export (file, DatabaseHelper.DbInfrastructure);
			EpsitecEntitySerializer.CleanDatabase (file, DatabaseHelper.DbInfrastructure);
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
		}

		[TestMethod]
		public void CleanDatabase()
		{
			FileInfo file = new FileInfo ("test.xml");

			DbLogEntry dbLogEntry = DatabaseHelper.DbInfrastructure.Logger.CreateLogEntry (new DbId (1));

			EpsitecEntitySerializer.Export (file, DatabaseHelper.DbInfrastructure);
			EpsitecEntitySerializer.Import (file, DatabaseHelper.DbInfrastructure, dbLogEntry);

			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					Assert.IsNotNull (dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1))));
					Assert.IsNotNull (dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (2))));
					Assert.IsNotNull (dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (3))));
							 
					Assert.IsNotNull (dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1))));
					Assert.IsNotNull (dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (2))));
					Assert.IsNotNull (dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (3))));
					Assert.IsNotNull (dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (4))));
							 
					Assert.IsNotNull (dataContext.ResolveEntity<UriSchemeEntity> (new DbKey (new DbId (1))));
							 
					Assert.IsNotNull (dataContext.ResolveEntity<PersonGenderEntity> (new DbKey (new DbId (1))));
					Assert.IsNotNull (dataContext.ResolveEntity<PersonGenderEntity> (new DbKey (new DbId (2))));
							 
					Assert.IsNotNull (dataContext.ResolveEntity<PersonTitleEntity> (new DbKey (new DbId (1))));
					Assert.IsNotNull (dataContext.ResolveEntity<PersonTitleEntity> (new DbKey (new DbId (2))));
							
					Assert.IsNotNull (dataContext.ResolveEntity<LanguageEntity> (new DbKey (new DbId (1))));
					Assert.IsNotNull (dataContext.ResolveEntity<LanguageEntity> (new DbKey (new DbId (2))));

					Assert.IsNotNull (dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001))));
					Assert.IsNotNull (dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000002))));
					Assert.IsNotNull (dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000003))));

					Assert.IsNotNull (dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000001))));
					Assert.IsNotNull (dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000002))));
					Assert.IsNotNull (dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000003))));
					Assert.IsNotNull (dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000004))));

					Assert.IsNotNull (dataContext.ResolveEntity<UriSchemeEntity> (new DbKey (new DbId (1000000001))));

					Assert.IsNotNull (dataContext.ResolveEntity<PersonGenderEntity> (new DbKey (new DbId (1000000001))));
					Assert.IsNotNull (dataContext.ResolveEntity<PersonGenderEntity> (new DbKey (new DbId (1000000002))));

					Assert.IsNotNull (dataContext.ResolveEntity<PersonTitleEntity> (new DbKey (new DbId (1000000001))));
					Assert.IsNotNull (dataContext.ResolveEntity<PersonTitleEntity> (new DbKey (new DbId (1000000002))));

					Assert.IsNotNull (dataContext.ResolveEntity<LanguageEntity> (new DbKey (new DbId (1000000001))));
					Assert.IsNotNull (dataContext.ResolveEntity<LanguageEntity> (new DbKey (new DbId (1000000002))));
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

					Assert.IsNotNull (dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001))));
					Assert.IsNotNull (dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000002))));
					Assert.IsNotNull (dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000003))));

					Assert.IsNotNull (dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000001))));
					Assert.IsNotNull (dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000002))));
					Assert.IsNotNull (dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000003))));
					Assert.IsNotNull (dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000004))));

					Assert.IsNotNull (dataContext.ResolveEntity<UriSchemeEntity> (new DbKey (new DbId (1000000001))));

					Assert.IsNotNull (dataContext.ResolveEntity<PersonGenderEntity> (new DbKey (new DbId (1000000001))));
					Assert.IsNotNull (dataContext.ResolveEntity<PersonGenderEntity> (new DbKey (new DbId (1000000002))));

					Assert.IsNotNull (dataContext.ResolveEntity<PersonTitleEntity> (new DbKey (new DbId (1000000001))));
					Assert.IsNotNull (dataContext.ResolveEntity<PersonTitleEntity> (new DbKey (new DbId (1000000002))));

					Assert.IsNotNull (dataContext.ResolveEntity<LanguageEntity> (new DbKey (new DbId (1000000001))));
					Assert.IsNotNull (dataContext.ResolveEntity<LanguageEntity> (new DbKey (new DbId (1000000002))));
				}
			}
		}


		[TestMethod]
		public void ExportImportWithoutSomeTables()
		{
			FileInfo file = new FileInfo ("test.xml");

			DbLogEntry dbLogEntry = DatabaseHelper.DbInfrastructure.Logger.CreateLogEntry (new DbId (1));

			EpsitecEntitySerializer.Export (file, DatabaseHelper.DbInfrastructure);
			
			EpsitecEntitySerializer.CleanDatabase (file, DatabaseHelper.DbInfrastructure);

			XDocument xDocument = XDocument.Load (file.FullName);

			var xtablesToRemove = from xTable in xDocument.Descendants ("table")
								 let id = (string) xTable.Attribute ("id")
								 where id == "9" || id == "10" || id == "22" || id == "30"
								 select xTable;

			foreach (XElement xTable in xtablesToRemove.ToList ())
			{
				xTable.Remove ();
			}

			foreach (XElement xTable in xDocument.Descendants ("table").ToList ())
			{
				string sid = (string) xTable.Attribute ("id");
				int iid = System.Int32.Parse (sid);

				if (iid > 10 && iid < 22)
				{
					xTable.SetAttributeValue ("id", iid - 2);
				}
				else if (iid > 22 && iid < 30)
				{
					xTable.SetAttributeValue ("id", iid - 3);
				}
				else if (iid > 30)
				{
					xTable.SetAttributeValue ("id", iid - 4);
				}
			}

			xDocument.Save (file.FullName);

			EpsitecEntitySerializer.Import (file, DatabaseHelper.DbInfrastructure, dbLogEntry);

			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
					NaturalPersonEntity gertrude = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (2)));
					NaturalPersonEntity hans = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (3)));

					Assert.IsNotNull (alfred);
					Assert.IsNotNull (gertrude);
					Assert.IsNotNull (hans);
				}
			}
		}


		[TestMethod]
		public void ExportImportWithoutSomeColumns()
		{
			FileInfo file = new FileInfo ("test.xml");

			DbLogEntry dbLogEntry = DatabaseHelper.DbInfrastructure.Logger.CreateLogEntry (new DbId (1));

			EpsitecEntitySerializer.Export (file, DatabaseHelper.DbInfrastructure);

			EpsitecEntitySerializer.CleanDatabase (file, DatabaseHelper.DbInfrastructure);

			XDocument xDocument = XDocument.Load (file.FullName);

			var xColumnsToRemove = from xTable in xDocument.Descendants ("table")
								   let id = (string) xTable.Attribute ("id")
								   where id == "0"
								   select xTable into xTable
								   from xColumn in xTable.Descendants ("column")
								   let id = (string) xColumn.Attribute ("id")
								   where id == "3"
								   select xColumn;

			foreach (XElement xColumn in xColumnsToRemove.ToList ())
			{
				xColumn.Remove ();
			}

			xDocument.Save (file.FullName);

			EpsitecEntitySerializer.Import (file, DatabaseHelper.DbInfrastructure, dbLogEntry);

			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
					NaturalPersonEntity gertrude = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (2)));
					NaturalPersonEntity hans = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (3)));

					Assert.IsNotNull (alfred);
					Assert.IsNotNull (gertrude);
					Assert.IsNotNull (hans);
				}
			}
		}


	}


}
