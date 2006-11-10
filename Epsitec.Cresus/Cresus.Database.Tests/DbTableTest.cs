using NUnit.Framework;

using Epsitec.Common.Types;

namespace Epsitec.Cresus.Database
{
	[TestFixture]
	public class DbTableTest
	{
		[TestFixtureSetUp]
		public void Setup()
		{
			try
			{
				System.IO.File.Delete (@"C:\Program Files\firebird\Data\Epsitec\TABLETEST.FIREBIRD");
			}
			catch (System.IO.IOException ex)
			{
				System.Console.Out.WriteLine ("Cannot delete database file. Error message :\n{0}\nWaiting for 5 seconds...", ex.ToString ());
				System.Threading.Thread.Sleep (5000);

				try
				{
					System.IO.File.Delete (@"C:\Program Files\firebird\Data\Epsitec\TABLETEST.FIREBIRD");
				}
				catch
				{
				}
			}

			using (DbInfrastructure infrastructure = new DbInfrastructure ())
			{
				infrastructure.CreateDatabase (DbInfrastructure.CreateDatabaseAccess ("tabletest"));
			}

			this.infrastructure = DbInfrastructureTest.GetInfrastructureFromBase ("tabletest", true);
		}

		[TestFixtureTearDown]
		public void TearDown()
		{
			this.infrastructure.Dispose ();
			this.infrastructure = null;
		}

		[Test]
		public void CheckNewDbTable()
		{
			DbTable table = new DbTable ("Test");
			
			Assert.IsNotNull (table);
			Assert.AreEqual ("Test", table.Name);
			Assert.AreEqual (DbElementCat.Unknown, table.Category);
			Assert.AreEqual (false, table.HasPrimaryKeys);
			Assert.AreEqual (0, table.PrimaryKeys.Count);
			Assert.AreEqual (0, table.Columns.Count);
			
			table.DefineCategory (DbElementCat.Internal);
			
			Assert.AreEqual (DbElementCat.Internal, table.Category);
			
			table.DefineCategory (DbElementCat.Internal);
		}
		
#if false
		[Test] public void CheckSerializeToXml()
		{
			DbTable table = new DbTable ("Test");
			DbColumn column = new DbColumn ("A", DbNumDef.FromRawType (DbRawType.SmallDecimal), Nullable.Yes);
			
			column.IsIndexed = true;
			column.DefineCategory (DbElementCat.UserDataManaged);
			
			table.DefineCategory (DbElementCat.UserDataManaged);
			table.DefineRevisionMode (DbRevisionMode.Enabled);
			
			table.PrimaryKeys.Add (column);
			table.Columns.Add (column);
			
			Assert.AreEqual (table, column.Table);
			
			string xml = DbTable.SerializeToXml (table, true);
			
			System.Console.Out.WriteLine ("XML: {0}", xml);
			
			DbTable test = DbTable.CreateTable (xml);
			
			Assert.AreEqual (test, test.Columns["A"].Table);
			Assert.AreEqual (DbElementCat.UserDataManaged, test.Category);
			Assert.AreEqual (DbRevisionMode.Enabled, test.RevisionMode);
			Assert.AreEqual ("Test", test.Name);
			Assert.AreEqual (DbElementCat.UserDataManaged, test.Columns["A"].Category);
		}
#endif
		
		[Test]
		public void CheckForeignKeys()
		{
			DbTable table = new DbTable ("Test");
			
			DbColumn column_1 = DbInfrastructure.CreateRefColumn ("A", "ParentTable", new DbTypeDef (Res.Types.Num.NullableKeyId));
			DbColumn column_2 = DbInfrastructure.CreateUserDataColumn ("X", new DbTypeDef (new Epsitec.Common.Types.DecimalType (-999999999.999999999M, 999999999.999999999M, 0.000000001M)));
			DbColumn column_3 = DbInfrastructure.CreateRefColumn ("Z", "Customer", new DbTypeDef (Res.Types.Num.NullableKeyId));
			
			table.Columns.Add (column_1);
			table.Columns.Add (column_2);
			table.Columns.Add (column_3);
			
			DbForeignKey[] fk;
			
			fk = Collection.ToArray<DbForeignKey> (table.ForeignKeys);
			
			Assert.AreEqual (2, fk.Length);
			Assert.AreEqual (1, fk[0].Columns.Length);
			Assert.AreEqual (1, fk[1].Columns.Length);
			Assert.AreEqual (column_1, fk[0].Columns[0]);
			Assert.AreEqual (column_3, fk[1].Columns[0]);
			
			table = new DbTable ("Test");
			
			table.Columns.Add (column_2);
			table.Columns.Add (column_1);
			
			fk = Collection.ToArray<DbForeignKey> (table.ForeignKeys);
			
			Assert.AreEqual (1, fk.Length);
			Assert.AreEqual (1, fk[0].Columns.Length);
			Assert.AreEqual (column_1, fk[0].Columns[0]);
			
			Assert.AreEqual (table, table.Columns[column_1.Name].Table);
			Assert.AreEqual (table, table.Columns[column_2.Name].Table);
			
			table.Columns.Remove (column_2);
			
			Assert.IsNull (table.Columns[column_2.Name]);
			Assert.IsNull (column_2.Table);
		}
		
		[Test] [ExpectedException (typeof (System.InvalidOperationException))] public void CheckNewDbTableEx1()
		{
			DbTable table = new DbTable ("Test");
			
			table.DefineCategory (DbElementCat.Internal);
			table.DefineCategory (DbElementCat.ExternalUserData);
		}
		
#if false
		[Test] public void CheckCreateSqlTable()
		{
			IDbAbstraction db_abstraction = DbFactoryTest.CreateDbAbstraction (true);
			ITypeConverter type_converter = db_abstraction.Factory.TypeConverter;
			
			DbColumn column_a = new DbColumn ("A", DbNumDef.FromRawType (DbRawType.Int32));
			DbColumn column_b = new DbColumn ("B", DbNumDef.FromRawType (DbRawType.Int16));
			DbColumn column_c = new DbColumn ("C", DbSimpleType.String, 50, false, Nullable.Yes);
			DbColumn column_d = new DbColumn ("D", DbSimpleType.Guid, Nullable.Yes);
			DbColumn column_e = new DbColumn ("e/x", DbNumDef.FromRawType (DbRawType.Boolean), Nullable.No);
			
			DbTable db_table = new DbTable ("Test (1)");
			
			db_table.DefineKey (new DbKey (9));
			db_table.DefineCategory (DbElementCat.UserDataManaged);
			db_table.DefineReplicationMode (DbReplicationMode.Shared);
			column_a.DefineCategory (DbElementCat.UserDataManaged);
			column_b.DefineCategory (DbElementCat.UserDataManaged);
			column_c.DefineCategory (DbElementCat.UserDataManaged);
			column_d.DefineCategory (DbElementCat.UserDataManaged);
			column_e.DefineCategory (DbElementCat.UserDataManaged);
			
			db_table.PrimaryKeys.AddRange (new DbColumn[] { column_a, column_b });
			db_table.Columns.AddRange (new DbColumn[] { column_a, column_b, column_c, column_d, column_e });
			
			SqlTable sql_table = db_table.CreateSqlTable (type_converter);
			
			Assert.IsNotNull (sql_table);
			Assert.AreEqual ("U_TEST__1__9", sql_table.Name);
			Assert.AreEqual (db_table.Columns.Count, sql_table.Columns.Count);
			Assert.AreEqual (db_table.PrimaryKeys.Count, sql_table.PrimaryKey.Length);
			Assert.AreEqual ("U_A", sql_table.Columns[0].Name);
			Assert.AreEqual ("U_E_X", sql_table.Columns[4].Name);
		}
#endif
	
#if false
		[Test] [ExpectedException (typeof (Exceptions.SyntaxException))] public void CheckCreateSqlTableEx1()
		{
			IDbAbstraction db_abstraction = DbFactoryTest.CreateDbAbstraction (true);
			ITypeConverter type_converter = db_abstraction.Factory.TypeConverter;
			
			DbColumn column_a = new DbColumn ("A", DbNumDef.FromRawType (DbRawType.Int32));
			DbColumn column_b = new DbColumn ("B", DbNumDef.FromRawType (DbRawType.Int16));
			DbColumn column_c = new DbColumn ("C", DbSimpleType.String, 50, false, Nullable.Yes);
			DbColumn column_d = new DbColumn ("D", DbSimpleType.Guid, Nullable.Yes);
			DbColumn column_e = new DbColumn ("E", DbNumDef.FromRawType (DbRawType.Boolean), Nullable.No);
			
			DbTable db_table = new DbTable ("Test");
			
			db_table.DefineKey (new DbKey (0));
			db_table.DefineCategory (DbElementCat.UserDataManaged);
			db_table.DefineReplicationMode (DbReplicationMode.Shared);
			column_a.DefineCategory (DbElementCat.UserDataManaged);
			column_b.DefineCategory (DbElementCat.UserDataManaged);
			column_c.DefineCategory (DbElementCat.UserDataManaged);
			column_d.DefineCategory (DbElementCat.UserDataManaged);
			column_e.DefineCategory (DbElementCat.UserDataManaged);
			
			db_table.PrimaryKeys.AddRange (new DbColumn[] { column_a, column_b });
			db_table.Columns.AddRange (new DbColumn[] { column_b, column_c, column_d, column_e });
			
			SqlTable sql_table = db_table.CreateSqlTable (type_converter);
			
			//	exception: colonnes a et b ne font pas partie de la table
		}
#endif

#if false		
		[Test] [ExpectedException (typeof (Exceptions.SyntaxException))] public void CheckCreateSqlTableEx2()
		{
			IDbAbstraction db_abstraction = DbFactoryTest.CreateDbAbstraction (true);
			ITypeConverter type_converter = db_abstraction.Factory.TypeConverter;
			
			DbColumn column_a = new DbColumn ("A", DbNumDef.FromRawType (DbRawType.Int32), Nullable.Yes);
			DbColumn column_b = new DbColumn ("B", DbNumDef.FromRawType (DbRawType.Int16));
			DbColumn column_c = new DbColumn ("C", DbSimpleType.String, 50, false, Nullable.Yes);
			DbColumn column_d = new DbColumn ("D", DbSimpleType.Guid, Nullable.Yes);
			DbColumn column_e = new DbColumn ("E", DbNumDef.FromRawType (DbRawType.Boolean), Nullable.No);
			
			DbTable db_table = new DbTable ("Test");
			
			db_table.DefineKey (new DbKey (0));
			db_table.DefineCategory (DbElementCat.UserDataManaged);
			db_table.DefineReplicationMode (DbReplicationMode.Shared);
			column_a.DefineCategory (DbElementCat.UserDataManaged);
			column_b.DefineCategory (DbElementCat.UserDataManaged);
			column_c.DefineCategory (DbElementCat.UserDataManaged);
			column_d.DefineCategory (DbElementCat.UserDataManaged);
			column_e.DefineCategory (DbElementCat.UserDataManaged);
			
			db_table.PrimaryKeys.AddRange (new DbColumn[] { column_a, column_b });
			db_table.Columns.AddRange (new DbColumn[] { column_a, column_b, column_c, column_d, column_e });
			
			SqlTable sql_table = db_table.CreateSqlTable (type_converter);
			
			//	exception: colonne A est nullable
		}
#endif
	
#if false
		[Test] [ExpectedException (typeof (Exceptions.SyntaxException))] public void CheckCreateSqlTableEx3()
		{
			IDbAbstraction db_abstraction = DbFactoryTest.CreateDbAbstraction (true);
			ITypeConverter type_converter = db_abstraction.Factory.TypeConverter;
			
			DbColumn column_a  = new DbColumn ("A", DbNumDef.FromRawType (DbRawType.Int32));
			DbColumn column_b1 = new DbColumn ("B", DbNumDef.FromRawType (DbRawType.Int16));
			DbColumn column_b2 = new DbColumn ("B", DbSimpleType.String, 50, false, Nullable.Yes);
			
			DbTable db_table = new DbTable ("Test");
			
			db_table.DefineKey (new DbKey (0));
			db_table.DefineCategory (DbElementCat.UserDataManaged);
			db_table.DefineReplicationMode (DbReplicationMode.Shared);
			column_a.DefineCategory (DbElementCat.UserDataManaged);
			column_b1.DefineCategory (DbElementCat.UserDataManaged);
			column_b2.DefineCategory (DbElementCat.UserDataManaged);
			
			db_table.Columns.AddRange (new DbColumn[] { column_a, column_b1, column_b2 });
			
			SqlTable sql_table = db_table.CreateSqlTable (type_converter);
			
			//	exception: colonne b à double
		}
#endif
		
		private DbInfrastructure infrastructure;
	}
}
