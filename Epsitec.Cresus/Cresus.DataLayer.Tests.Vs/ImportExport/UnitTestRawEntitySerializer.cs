using Epsitec.Common.Support;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.ImportExport;
using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.Schema;
using Epsitec.Cresus.DataLayer.Tests.Vs.Entities;
using Epsitec.Cresus.DataLayer.Tests.Vs.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.IO;

using System.Linq;

using System.Xml.Linq;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.ImportExport
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
			using (DB db = DB.ConnectToTestDatabase ())
			{
				FileInfo file = new FileInfo ("test.xml");

				EntityModificationEntry entry = db.DataInfrastructure.CreateEntityModificationEntry ();

				DbInfrastructure dbInfrastructure = db.DataInfrastructure.DbInfrastructure;
				EntityTypeEngine typeEngine = db.DataInfrastructure.EntityEngine.EntityTypeEngine;
				EntitySchemaEngine schemaEngine = db.DataInfrastructure.EntityEngine.EntitySchemaEngine;

				RawEntitySerializer.Export (file, dbInfrastructure, typeEngine, schemaEngine, exportMode);

				if (exportMode == RawExportMode.EpsitecAndUserData)
				{
					RawEntitySerializer.Import (file, dbInfrastructure, entry, RawImportMode.DecrementIds);
					RawEntitySerializer.Export (file, dbInfrastructure, typeEngine, schemaEngine, exportMode);
				}

				RawEntitySerializer.CleanDatabase (file, dbInfrastructure, importMode);

				RawEntitySerializer.Import (file, dbInfrastructure, entry, importMode);

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
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
			using (DB db = DB.ConnectToTestDatabase ())
			{
				FileInfo file = new FileInfo ("test.xml");

				EntityModificationEntry entry = db.DataInfrastructure.CreateEntityModificationEntry ();

				DbInfrastructure dbInfrastructure = db.DataInfrastructure.DbInfrastructure;
				EntityTypeEngine typeEngine = db.DataInfrastructure.EntityEngine.EntityTypeEngine;
				EntitySchemaEngine schemaEngine = db.DataInfrastructure.EntityEngine.EntitySchemaEngine;

				RawEntitySerializer.Export (file, dbInfrastructure, typeEngine, schemaEngine, RawExportMode.UserData);
				RawEntitySerializer.Import (file, dbInfrastructure, entry, RawImportMode.DecrementIds);

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
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

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
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
			using (DB db = DB.ConnectToTestDatabase ())
			{
				FileInfo file = new FileInfo ("test.xml");

				EntityModificationEntry entry = db.DataInfrastructure.CreateEntityModificationEntry ();

				DbInfrastructure dbInfrastructure = db.DataInfrastructure.DbInfrastructure;
				EntityTypeEngine typeEngine = db.DataInfrastructure.EntityEngine.EntityTypeEngine;
				EntitySchemaEngine schemaEngine = db.DataInfrastructure.EntityEngine.EntitySchemaEngine;

				RawEntitySerializer.Export (file, dbInfrastructure, typeEngine, schemaEngine, RawExportMode.UserData);

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

				RawEntitySerializer.Import (file, dbInfrastructure, entry, RawImportMode.DecrementIds);

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
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
			using (DB db = DB.ConnectToTestDatabase ())
			{
				FileInfo file = new FileInfo ("test.xml");

				EntityModificationEntry entry = db.DataInfrastructure.CreateEntityModificationEntry ();

				DbInfrastructure dbInfrastructure = db.DataInfrastructure.DbInfrastructure;
				EntityTypeEngine typeEngine = db.DataInfrastructure.EntityEngine.EntityTypeEngine;
				EntitySchemaEngine schemaEngine = db.DataInfrastructure.EntityEngine.EntitySchemaEngine;

				RawEntitySerializer.Export (file, dbInfrastructure, typeEngine, schemaEngine, RawExportMode.UserData);

				RawEntitySerializer.CleanDatabase (file, dbInfrastructure, RawImportMode.DecrementIds);

				XDocument xDocument = XDocument.Load (file.FullName);

				var xColumnsToRemove = from xTable in xDocument.Descendants ("table")
									   let id = (string) xTable.Attribute ("id")
									   where id == "15"	// Table for natural persons
									   select xTable into xTable
									   from xColumn in xTable.Descendants ("column")
									   let id = (string) xColumn.Attribute ("id")
									   where id == "3" || id == "4" || id == "5"
									   select xColumn;

				foreach (XElement xColumn in xColumnsToRemove.ToList ())
				{
					xColumn.Remove ();
				}

				xDocument.Save (file.FullName);

				RawEntitySerializer.Import (file, dbInfrastructure, entry, RawImportMode.DecrementIds);

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
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
		public void ExportImportWithColumnRemovedInDatabase()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				FileInfo file = new FileInfo ("test.xml");

				EntityModificationEntry entry = db.DataInfrastructure.CreateEntityModificationEntry ();

				DbInfrastructure dbInfrastructure = db.DataInfrastructure.DbInfrastructure;
				EntityTypeEngine typeEngine = db.DataInfrastructure.EntityEngine.EntityTypeEngine;
				EntitySchemaEngine schemaEngine = db.DataInfrastructure.EntityEngine.EntitySchemaEngine;

				RawEntitySerializer.Export (file, dbInfrastructure, typeEngine, schemaEngine, RawExportMode.UserData);

				RawEntitySerializer.CleanDatabase (file, dbInfrastructure, RawImportMode.DecrementIds);

				XDocument xDocument = XDocument.Load (file.FullName);

				var xTables = from xt in xDocument.Descendants ("definition").Single ().Descendants ("table")
							  let id = (string) xt.Attribute ("id")
							  where id == "15"	// Table for natural persons
							  select xt;

				var xTable = xTables.Single ();
				var xColumns = xTable.Descendants ("columns").Single ();
				xColumns.Add (
					new XElement ("column",
						new XAttribute ("id", "6"),
						new XElement ("dbName", "MyColumn"),
						new XElement ("sqlName", "MyColumn"),
						new XElement ("dbRawType", "Int64"),
						new XElement ("adoType", "System.Int64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"),
						new XElement ("isIdColumn", "false")
					)
				);

				var xRows = from xD in xDocument.Descendants ("data").Single ().Descendants ("table")
							let id = (string) xD.Attribute ("id")
							where id == "15"
							select xD into table
							from xRow in table.Descendants ("row")
							select xRow;

				foreach (var xRow in xRows.ToList ())
				{
					xRow.Add (
						new XElement ("column",
							new XAttribute ("id", "6"),
							"42"
						)
					);
				}

				xDocument.Save (file.FullName);

				RawEntitySerializer.Import (file, dbInfrastructure, entry, RawImportMode.DecrementIds);

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
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
		public void ExportImportWithTableRemovedInDatabase()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				FileInfo file = new FileInfo ("test.xml");

				EntityModificationEntry entry = db.DataInfrastructure.CreateEntityModificationEntry ();

				DbInfrastructure dbInfrastructure = db.DataInfrastructure.DbInfrastructure;
				EntityTypeEngine typeEngine = db.DataInfrastructure.EntityEngine.EntityTypeEngine;
				EntitySchemaEngine schemaEngine = db.DataInfrastructure.EntityEngine.EntitySchemaEngine;

				RawEntitySerializer.Export (file, dbInfrastructure, typeEngine, schemaEngine, RawExportMode.UserData);

				RawEntitySerializer.CleanDatabase (file, dbInfrastructure, RawImportMode.DecrementIds);

				DbTable table = EntitySchemaBuilder.BuildTables (typeEngine).Single (t => t.CaptionId == Druid.Parse ("[J1AJ1]"));
				DbColumn column = new DbColumn ("myColumn", table.Columns.First ().Type, DbColumnClass.Data, DbElementCat.ManagedUserData)
				{
					IsNullable = false,
				};

				dbInfrastructure.RemoveTable (table);

				table.Columns.Add (column);

				dbInfrastructure.AddTable (table);

				dbInfrastructure.ClearCaches ();

				RawEntitySerializer.Import (file, dbInfrastructure, entry, RawImportMode.DecrementIds);

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
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
		public void ExportImportWithNonNullableColumnAddedInDatabase()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				FileInfo file = new FileInfo ("test.xml");

				EntityModificationEntry entry = db.DataInfrastructure.CreateEntityModificationEntry ();

				DbInfrastructure dbInfrastructure = db.DataInfrastructure.DbInfrastructure;
				EntityTypeEngine typeEngine = db.DataInfrastructure.EntityEngine.EntityTypeEngine;
				EntitySchemaEngine schemaEngine = db.DataInfrastructure.EntityEngine.EntitySchemaEngine;

				RawEntitySerializer.Export (file, dbInfrastructure, typeEngine, schemaEngine, RawExportMode.UserData);

				RawEntitySerializer.CleanDatabase (file, dbInfrastructure, RawImportMode.DecrementIds);

				DbTable table = dbInfrastructure.ResolveDbTable (Druid.Parse ("[J1AN]"));

				dbInfrastructure.RemoveTable (table);

				dbInfrastructure.ClearCaches ();

				RawEntitySerializer.Import (file, dbInfrastructure, entry, RawImportMode.DecrementIds);

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
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
		public void ExportImportWithColumnChangedToNonNullableInDatabase()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
				{
					NaturalPersonEntity hans = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000003)));

					hans.Firstname = null;

					dataContext.SaveChanges ();
				}

				FileInfo file = new FileInfo ("test.xml");

				EntityModificationEntry entry = db.DataInfrastructure.CreateEntityModificationEntry ();

				DbInfrastructure dbInfrastructure = db.DataInfrastructure.DbInfrastructure;
				EntityTypeEngine typeEngine = db.DataInfrastructure.EntityEngine.EntityTypeEngine;
				EntitySchemaEngine schemaEngine = db.DataInfrastructure.EntityEngine.EntitySchemaEngine;

				RawEntitySerializer.Export (file, dbInfrastructure, typeEngine, schemaEngine, RawExportMode.UserData);

				RawEntitySerializer.CleanDatabase (file, dbInfrastructure, RawImportMode.DecrementIds);

				DbTable table = EntitySchemaBuilder.BuildTables (typeEngine).Single (t => t.CaptionId == Druid.Parse ("[J1AJ1]"));
				DbColumn columnFirstName = table.Columns.Single (c => c.CaptionId == Druid.Parse ("[J1AL1]"));

				columnFirstName.IsNullable = false;

				dbInfrastructure.RemoveTable (table);
				dbInfrastructure.AddTable (table);

				dbInfrastructure.ClearCaches ();

				RawEntitySerializer.Import (file, dbInfrastructure, entry, RawImportMode.DecrementIds);

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
				{
					NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
					NaturalPersonEntity gertrude = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (2)));
					NaturalPersonEntity hans = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (3)));

					Assert.IsTrue (DatabaseCreator2.CheckAlfred (alfred));
					Assert.IsTrue (DatabaseCreator2.CheckGertrude (gertrude));

					Assert.IsNotNull (hans);
					Assert.AreEqual ("", hans.Firstname);
				}
			}
		}


	}


}
