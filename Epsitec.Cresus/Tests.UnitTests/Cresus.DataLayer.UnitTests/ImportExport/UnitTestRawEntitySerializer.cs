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
	public sealed class UnitTestRawEntitySerializer
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}


		[TestInitialize]
		public void TestInitialize()
		{
			DatabaseCreator2.ResetPopulatedTestDatabase ();
		}


		[TestMethod]
		public void SimpleExportImport1()
		{
			RawExportMode exportMode = RawExportMode.UserData;
			RawImportMode importMode = RawImportMode.DecrementIds;

			this.SimpleExportImport (exportMode, importMode);
		}


		[TestMethod]
		public void SimpleExportImport2()
		{
			RawExportMode exportMode = RawExportMode.UserData;
			RawImportMode importMode = RawImportMode.PreserveIds;

			this.SimpleExportImport (exportMode, importMode);
		}


		[TestMethod]
		public void SimpleExportImport3()
		{
			RawExportMode exportMode = RawExportMode.EpsitecAndUserData;
			RawImportMode importMode = RawImportMode.PreserveIds;

			this.SimpleExportImport (exportMode, importMode);
		}


		private void SimpleExportImport(RawExportMode exportMode, RawImportMode importMode)
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				FileInfo file = new FileInfo ("test.xml");

				DbLogEntry dbLogEntry = dbInfrastructure.ServiceManager.Logger.CreateLogEntry (new DbId (1));

				RawEntitySerializer.Export (file, dbInfrastructure, exportMode);

				if (exportMode == RawExportMode.EpsitecAndUserData)
				{
					RawEntitySerializer.Import (file, dbInfrastructure, dbLogEntry, RawImportMode.DecrementIds);
					RawEntitySerializer.Export (file, dbInfrastructure, exportMode);
				}

				RawEntitySerializer.CleanDatabase (file, dbInfrastructure, importMode);

				RawEntitySerializer.Import (file, dbInfrastructure, dbLogEntry, importMode);

				using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					bool decrementIds = importMode == RawImportMode.DecrementIds;

					NaturalPersonEntity alfred1 = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
					NaturalPersonEntity gertrude1 = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (2)));
					NaturalPersonEntity hans1 = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (3)));

					if (decrementIds || exportMode == RawExportMode.EpsitecAndUserData)
					{
						Assert.IsTrue (DatabaseCreator2.CheckAlfred (alfred1));
						Assert.IsTrue (DatabaseCreator2.CheckGertrude (gertrude1));
						Assert.IsTrue (DatabaseCreator2.CheckHans (hans1));
					}
					else
					{
						Assert.IsNull (alfred1);
						Assert.IsNull (gertrude1);
						Assert.IsNull (hans1);
					}

					NaturalPersonEntity alfred2 = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
					NaturalPersonEntity gertrude2 = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000002)));
					NaturalPersonEntity hans2 = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000003)));

					Assert.IsTrue (DatabaseCreator2.CheckAlfred (alfred2));
					Assert.IsTrue (DatabaseCreator2.CheckGertrude (gertrude2));
					Assert.IsTrue (DatabaseCreator2.CheckHans (hans2));
				}
			}
		}


		[TestMethod]
		public void CleanDatabase1()
		{
			RawImportMode mode = RawImportMode.DecrementIds;

			this.CleanDatabase (mode);
		}
		

		[TestMethod]
		public void CleanDatabase2()
		{
			RawImportMode mode = RawImportMode.PreserveIds;

			this.CleanDatabase (mode);
		}


		private void CleanDatabase(RawImportMode mode)
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				FileInfo file = new FileInfo ("test.xml");

				DbLogEntry dbLogEntry = dbInfrastructure.ServiceManager.Logger.CreateLogEntry (new DbId (1));

				RawEntitySerializer.Export (file, dbInfrastructure, RawExportMode.UserData);
				RawEntitySerializer.Import (file, dbInfrastructure, dbLogEntry, RawImportMode.DecrementIds);

				using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
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

				RawEntitySerializer.CleanDatabase (file, dbInfrastructure, mode);

				using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
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

					bool isNull = mode == RawImportMode.PreserveIds;
					
					Assert.AreEqual (isNull, null == dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001))));
					Assert.AreEqual (isNull, null == dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000002))));
					Assert.AreEqual (isNull, null == dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000003))));

					Assert.AreEqual (isNull, null == dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000001))));
					Assert.AreEqual (isNull, null == dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000002))));
					Assert.AreEqual (isNull, null == dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000003))));
					Assert.AreEqual (isNull, null == dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000004))));

					Assert.AreEqual (isNull, null == dataContext.ResolveEntity<UriSchemeEntity> (new DbKey (new DbId (1000000001))));

					Assert.AreEqual (isNull, null == dataContext.ResolveEntity<PersonGenderEntity> (new DbKey (new DbId (1000000001))));
					Assert.AreEqual (isNull, null == dataContext.ResolveEntity<PersonGenderEntity> (new DbKey (new DbId (1000000002))));

					Assert.AreEqual (isNull, null == dataContext.ResolveEntity<PersonTitleEntity> (new DbKey (new DbId (1000000001))));
					Assert.AreEqual (isNull, null == dataContext.ResolveEntity<PersonTitleEntity> (new DbKey (new DbId (1000000002))));

					Assert.AreEqual (isNull, null == dataContext.ResolveEntity<LanguageEntity> (new DbKey (new DbId (1000000001))));
					Assert.AreEqual (isNull, null == dataContext.ResolveEntity<LanguageEntity> (new DbKey (new DbId (1000000002))));
				}
			}
		}

		[TestMethod]
		public void ExportImportWithoutSomeTables()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				FileInfo file = new FileInfo ("test.xml");

				DbLogEntry dbLogEntry = dbInfrastructure.ServiceManager.Logger.CreateLogEntry (new DbId (1));

				RawEntitySerializer.Export (file, dbInfrastructure, RawExportMode.UserData);

				RawEntitySerializer.CleanDatabase (file, dbInfrastructure, RawImportMode.DecrementIds);

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

				RawEntitySerializer.Import (file, dbInfrastructure, dbLogEntry, RawImportMode.DecrementIds);

				using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
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
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				FileInfo file = new FileInfo ("test.xml");

				DbLogEntry dbLogEntry = dbInfrastructure.ServiceManager.Logger.CreateLogEntry (new DbId (1));

				RawEntitySerializer.Export (file, dbInfrastructure, RawExportMode.UserData);

				RawEntitySerializer.CleanDatabase (file, dbInfrastructure, RawImportMode.DecrementIds);

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

				RawEntitySerializer.Import (file, dbInfrastructure, dbLogEntry, RawImportMode.DecrementIds);

				using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
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
