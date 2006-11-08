using NUnit.Framework;

namespace Epsitec.Cresus.Database
{
	[TestFixture]
	public class DbInfrastructureTest
	{
		[Test]
		public void Check01CreateDatabase()
		{
			//	Si on n'arrive pas à détruire le fichier, c'est que le serveur Firebird a peut-être
			//	encore conservé un handle ouvert; par expérience, cela peut prendre ~10s jusqu'à ce
			//	que la fermeture soit effective.

			try
			{
				System.IO.File.Delete (@"C:\Program Files\firebird\Data\Epsitec\FICHE.FIREBIRD");
			}
			catch (System.IO.IOException ex)
			{
				System.Console.Out.WriteLine ("Cannot delete database file. Error message :\n{0}\nWaiting for 5 seconds...", ex.ToString ());
				System.Threading.Thread.Sleep (5000);

				try
				{
					System.IO.File.Delete (@"C:\Program Files\firebird\Data\Epsitec\FICHE.FIREBIRD");
				}
				catch
				{
				}
			}

			using (DbInfrastructure infrastructure = new DbInfrastructure ())
			{
				DbAccess db_access = DbInfrastructure.CreateDbAccess ("fiche");

				infrastructure.CreateDatabase (db_access);

				DbTable table;

				//	Vérifie que les tables principales ont été créées correctement :

				table = infrastructure.ResolveDbTable (null, "CR_TABLE_DEF");

				Assert.IsNotNull (table);
				Assert.AreEqual (1000000000001L, table.Key.Id);
				Assert.AreEqual (6, table.Columns.Count);

				table = infrastructure.ResolveDbTable (null, "CR_COLUMN_DEF");

				Assert.IsNotNull (table);
				Assert.AreEqual (1000000000002L, table.Key.Id);
				Assert.AreEqual (8, table.Columns.Count);

				table = infrastructure.ResolveDbTable (null, "CR_TYPE_DEF");

				Assert.IsNotNull (table);
				Assert.AreEqual (1000000000003L, table.Key.Id);
				Assert.AreEqual (5, table.Columns.Count);

				Assert.AreEqual (0, infrastructure.CountMatchingRows (null, "CR_COLUMN_DEF", "CR_NAME", DbSqlStandard.MakeSimpleSqlName ("MyColumn")));
				Assert.AreEqual (4, infrastructure.CountMatchingRows (null, "CR_COLUMN_DEF", "CR_NAME", "CR_INFO"));

				//	Vérifie que les statements UPDATE ... lors de la création ont bien passé,
				//	puis vérifie aussi que l'incrément de l'ID fonctionne correctement.

				table = infrastructure.ResolveDbTable (null, "CR_TABLE_DEF");

				Assert.AreEqual (1000000000010L, infrastructure.NewRowIdInTable (null, table.Key, 2));
				Assert.AreEqual (1000000000012L, infrastructure.NewRowIdInTable (null, table.Key, 0));
				Assert.AreEqual (1000000000012L, infrastructure.NewRowIdInTable (null, table.Key, 1));
			}
		}

		[Test]
		public void Check02AttachDatabase()
		{
			using (DbInfrastructure infrastructure = DbInfrastructureTest.GetInfrastructureFromBase ("fiche", true))
			{
				Assert.IsNotNull (infrastructure);

				DbTable db_table = infrastructure.ResolveDbTable (null, "CR_TABLE_DEF");
				DbTypeDef db_type1 = infrastructure.ResolveDbType (null, Tags.TypeName);
				DbTypeDef db_type2 = infrastructure.ResolveDbType (null, Tags.TypeName);
				DbTypeDef db_type3 = infrastructure.ResolveDbType (null, Tags.TypeKeyId);

				Assert.IsNotNull (db_table);
				Assert.IsNotNull (db_type1);

				Assert.AreEqual (db_type1, db_type2);

				Assert.AreEqual (6, db_table.Columns.Count);
				Assert.AreEqual ("CR_ID", db_table.Columns[0].Name);
				Assert.AreEqual ("CREF_LOG", db_table.Columns[2].Name);
				Assert.AreEqual (db_type1, db_table.Columns["CR_NAME"].Type);
			}
		}

		[Test]
		public void Check03CreateDbType()
		{
			//	Ce test ne marche que pour une base qui est propre (i.e. qui vient d'être
			//	créée par CheckCreateDatabase).

			using (DbInfrastructure infrastructure = DbInfrastructureTest.GetInfrastructureFromBase ("fiche", false))
			{
				infrastructure.Logger.CreateTemporaryEntry (null);

#if false
				DbEnumValue[] values = new DbEnumValue[3];
				
				values[0] = new DbEnumValue (1, "M");		values[0].DefineAttributes ("capt=Monsieur");
				values[1] = new DbEnumValue (2, "Mme");		values[1].DefineAttributes ("capt=Madame");
				values[2] = new DbEnumValue (3, "Mlle");	values[2].DefineAttributes ("capt=Mademoiselle");
				
				DbTypeString db_type_str  = infrastructure.CreateDbType ("Nom", 40, false) as DbTypeString;
				DbTypeNum    db_type_num  = infrastructure.CreateDbType ("NUPO", new DbNumDef (4, 0, 1000, 9999)) as DbTypeNum;
				DbTypeEnum   db_type_enum = infrastructure.CreateDbType ("Titre", values) as DbTypeEnum;
				
				infrastructure.RegisterNewDbType (null, db_type_str);
				infrastructure.RegisterNewDbType (null, db_type_num);
				infrastructure.RegisterNewDbType (null, db_type_enum);
				
				DbType db_type_1 = infrastructure.ResolveDbType (null, "Nom");
				DbType db_type_2 = infrastructure.ResolveDbType (null, "NUPO");
				DbType db_type_3 = infrastructure.ResolveDbType (null, "Titre");
				
				Assert.IsNotNull (db_type_1);
				Assert.IsNotNull (db_type_2);
				Assert.IsNotNull (db_type_3);
				
				Assert.AreEqual ("Nom",   db_type_1.Name);
				Assert.AreEqual ("NUPO",  db_type_2.Name);
				Assert.AreEqual ("Titre", db_type_3.Name);
#endif

				infrastructure.Logger.CreateTemporaryEntry (null);

#if false
				infrastructure.UnregisterDbType (null, db_type_1);
				infrastructure.UnregisterDbType (null, db_type_2);
//				infrastructure.UnregisterDbType (null, db_type_3);
				
				db_type_1 = infrastructure.ResolveDbType (null, "Nom");
				db_type_2 = infrastructure.ResolveDbType (null, "NUPO");
//				db_type_3 = infrastructure.ResolveDbType (null, "Titre");
				
				Assert.IsNull (db_type_1);
				Assert.IsNull (db_type_2);
//				Assert.IsNull (db_type_3);
#endif
			}
		}

		[Test]
		public void Check04ReadBackDbType()
		{
			//	Ce test ne marche que pour une base qui a été peuplée par la méthode
			//	CheckCreateDbType...

			using (DbInfrastructure infrastructure = DbInfrastructureTest.GetInfrastructureFromBase ("fiche", false))
			{
				infrastructure.Logger.CreateTemporaryEntry (null);

#if false
				DbType db_type_1 = infrastructure.ResolveDbType (null, "Nom");
				DbType db_type_2 = infrastructure.ResolveDbType (null, "NUPO");
				DbType db_type_3 = infrastructure.ResolveDbType (null, "Titre");
				
				Assert.IsNull (db_type_1);
				Assert.IsNull (db_type_2);
				Assert.IsNotNull (db_type_3);
				
				Assert.AreEqual ("Titre", db_type_3.Name);
				
				infrastructure.UnregisterDbType (null, db_type_3);
				
				db_type_3 = infrastructure.ResolveDbType (null, "Titre");
				
				Assert.IsNull (db_type_3);
#endif
			}
		}

		[Test]
		public void Check05CreateDbTable()
		{
			//	Ce test ne marche que pour une base qui est propre (i.e. qui vient d'être
			//	créée par CheckCreateDatabase).

			using (DbInfrastructure infrastructure = DbInfrastructureTest.GetInfrastructureFromBase ("fiche", false))
			{
				infrastructure.Logger.CreateTemporaryEntry (null);

				DbTable db_table1 = infrastructure.CreateDbTable ("SimpleTest", DbElementCat.ManagedUserData, DbRevisionMode.Disabled);

				DbTypeDef db_type_name  = new DbTypeDef ("Name", DbSimpleType.String, null, 80, false, Nullable.No);
				DbTypeDef db_type_level = new DbTypeDef ("Level", DbSimpleType.String, null, 4, false, Nullable.No);
				DbTypeDef db_type_type  = new DbTypeDef ("Type", DbSimpleType.String, null, 25, false, Nullable.Yes);
				DbTypeDef db_type_data  = new DbTypeDef ("Data", DbSimpleType.ByteArray, null, 0, false, Nullable.Yes);

				infrastructure.RegisterNewDbType (null, db_type_name);
				infrastructure.RegisterNewDbType (null, db_type_level);
				infrastructure.RegisterNewDbType (null, db_type_type);
				infrastructure.RegisterNewDbType (null, db_type_data);

				DbColumn col1 = infrastructure.CreateUserDataColumn ("Name", db_type_name);
				DbColumn col2 = infrastructure.CreateUserDataColumn ("Level", db_type_level);
				DbColumn col3 = infrastructure.CreateUserDataColumn ("Type", db_type_type);
				DbColumn col4 = infrastructure.CreateUserDataColumn ("Data", db_type_data);

				db_table1.Columns.AddRange (new DbColumn[] { col1, col2, col3, col4 });

				infrastructure.RegisterNewDbTable (null, db_table1);

				DbTable db_table2 = infrastructure.ResolveDbTable (null, "SimpleTest");

				Assert.IsNotNull (db_table2);

				Assert.AreEqual (db_table1.Name, db_table2.Name);
				Assert.AreEqual (db_table1.Category, db_table2.Category);
				Assert.AreEqual (db_table1.PrimaryKeys.Count, db_table2.PrimaryKeys.Count);
				Assert.AreEqual (db_table1.PrimaryKeys[0].Name, db_table2.PrimaryKeys[0].Name);
				Assert.AreEqual (db_table1.Columns.Count, db_table2.Columns.Count);
				Assert.AreEqual (1000000000013L, db_table2.Key.Id);
			}
		}

		[Test]
		[ExpectedException (typeof (Exceptions.GenericException))]
		public void Check06CreateDbTableEx1()
		{
			//	Exécuter deux fois une création de table va nécessairement générer une exception.
			//	Il faut exécuter le test CheckCreateDbTable avant celui-ci.

			using (DbInfrastructure infrastructure = DbInfrastructureTest.GetInfrastructureFromBase ("fiche", false))
			{
				DbTable db_table = infrastructure.CreateDbTable ("SimpleTest", DbElementCat.ManagedUserData, DbRevisionMode.Disabled);
				infrastructure.RegisterNewDbTable (null, db_table);
			}
		}

		[Test]
		public void Check07FindDbTables()
		{
			//	Il faut exécuter le test CheckCreateDbTable avant celui-ci.

			using (DbInfrastructure infrastructure = DbInfrastructureTest.GetInfrastructureFromBase ("fiche", false))
			{
				System.Diagnostics.Debug.WriteLine ("FindDbTables :");
				System.Diagnostics.Debug.WriteLine ("--------------");
				DbTable[] tables = infrastructure.FindDbTables (null, DbElementCat.Any);
				System.Diagnostics.Debug.WriteLine ("-- (cached) --");
				infrastructure.FindDbTables (null, DbElementCat.Any);
				System.Diagnostics.Debug.WriteLine ("--- (done) ---");

				for (int i = 0; i < tables.Length; i++)
				{
					System.Console.Out.WriteLine ("Table {0} has {1} columns. Category is {2}.", tables[i].Name, tables[i].Columns.Count, tables[i].Category);
					for (int j = 0; j < tables[i].Columns.Count; j++)
					{
						System.Console.Out.WriteLine (" {0}: {1}, {2} (type {3}).", j, tables[i].Columns[j].Name, tables[i].Columns[j].Type.Name, tables[i].Columns[j].Type.SimpleType);
					}
				}
			}
		}

		[Test] public void Check08FindDbTypes()
		{
			//	Il faut exécuter le test CheckCreateDbTable avant celui-ci.
			
			using (DbInfrastructure infrastructure = DbInfrastructureTest.GetInfrastructureFromBase ("fiche", false))
			{
				DbTypeDef[] types = infrastructure.FindDbTypes (null);
				
				for (int i = 0; i < types.Length; i++)
				{
					DbTypeDef type = types[i];
					
					System.Console.Out.Write ("Type {0}, {1}{2}.", type.Name, type.SimpleType, type.IsNullable ? ", nullable" : "");
					
					if (type.SimpleType == DbSimpleType.String)
					{
						System.Console.Out.WriteLine ("  Length={0}, is fixed length={1}, multilingual={2}.", type.Length, type.IsFixedLength, type.IsMultilingual);
					}
					else if (type.NumDef != null)
					{
						DbNumDef numDef = type.NumDef;
						System.Console.Out.WriteLine ("  Numeric type={0}, {1} digits, {2} shift.", numDef.InternalRawType, numDef.DigitPrecision, numDef.DigitShift);
					}
				}
			}
		}

		[Test]
		public void Check09UnregisterDbTable()
		{
			//	Il faut exécuter le test CheckCreateDbTable avant celui-ci.

			using (DbInfrastructure infrastructure = DbInfrastructureTest.GetInfrastructureFromBase ("fiche", false))
			{
				infrastructure.Logger.CreateTemporaryEntry (null);

				DbTable db_table1 = infrastructure.ResolveDbTable (null, "SimpleTest");

				Assert.IsNotNull (db_table1);

				infrastructure.UnregisterDbTable (null, db_table1);

				DbTable db_table2 = infrastructure.ResolveDbTable (null, "SimpleTest");

				Assert.IsNull (db_table2);
			}
		}

		[Test]
		public void Check10RegisterDbTableSameAsUnregistered()
		{
			//	Il faut exécuter le test CheckUnregisterDbTable avant celui-ci.

			using (DbInfrastructure infrastructure = DbInfrastructureTest.GetInfrastructureFromBase ("fiche", false))
			{
				DbTable db_table = infrastructure.CreateDbTable ("SimpleTest", DbElementCat.ManagedUserData, DbRevisionMode.Disabled);
				infrastructure.RegisterNewDbTable (null, db_table);

				Assert.IsNotNull (infrastructure.ResolveDbTable (null, db_table.Name));
				Assert.AreEqual (1000000000014L, db_table.Key.Id);
				Assert.AreEqual (DbRowStatus.Live, db_table.Key.Status);
			}
		}

		[Test]
		[ExpectedException (typeof (Exceptions.GenericException))]
		public void Check11UnregisterDbTableEx1()
		{
			//	Il faut exécuter le test CheckRegisterDbTableSameAsUnregistered avant celui-ci.

			using (DbInfrastructure infrastructure = DbInfrastructureTest.GetInfrastructureFromBase ("fiche", false))
			{
				DbTable db_table = infrastructure.ResolveDbTable (null, "SimpleTest");

				Assert.IsNotNull (db_table);

				infrastructure.UnregisterDbTable (null, db_table);
				Assert.IsNull (infrastructure.ResolveDbTable (null, db_table.Name));
				infrastructure.UnregisterDbTable (null, db_table);
			}
		}

		[Test]
		public void Check12LoggerFind()
		{
			using (DbInfrastructure infrastructure = DbInfrastructureTest.GetInfrastructureFromBase ("fiche", false))
			{
				DbLogger.Entry[] entries = infrastructure.Logger.Find (null, DbId.MinimumTemp, DbId.MaximumTemp);

				foreach (DbLogger.Entry entry in entries)
				{
					System.Diagnostics.Debug.WriteLine (string.Format ("ID={1}:{0}, created at {2} - ticks = {3}", entry.Id.LocalId, entry.Id.ClientId, entry.DateTime, entry.DateTime.Ticks));
				}
			}
		}

		[Test]
		public void Check13LoggerRemove()
		{
			using (DbInfrastructure infrastructure = DbInfrastructureTest.GetInfrastructureFromBase ("fiche", false))
			{
				Assert.AreEqual (DbId.TempClientId, infrastructure.Logger.CurrentId.ClientId);
				Assert.AreEqual (6L, infrastructure.Logger.CurrentId.LocalId);

				infrastructure.Logger.CreateTemporaryEntry (null);
				infrastructure.Logger.CreateTemporaryEntry (null);
				infrastructure.Logger.CreateTemporaryEntry (null);

				Assert.IsTrue (infrastructure.Logger.Remove (null, DbId.MinimumTemp + 7));

				infrastructure.Logger.RemoveRange (null, DbId.MinimumTemp + 7, DbId.MinimumTemp + 8);

				Assert.IsTrue (!infrastructure.Logger.Remove (null, DbId.MinimumTemp + 8));
				Assert.IsTrue (infrastructure.Logger.Remove (null, DbId.MinimumTemp + 9));
			}
		}

		[Test]
		public void Check14MultipleTransactions()
		{
			using (DbInfrastructure infrastructure = DbInfrastructureTest.GetInfrastructureFromBase ("fiche", true))
			{
				Assert.IsNotNull (infrastructure);

				IDbAbstraction dba1 = infrastructure.CreateDbAbstraction ();
				IDbAbstraction dba2 = infrastructure.CreateDbAbstraction ();

				Assert.IsFalse (dba1 == dba2);
				Assert.IsFalse (dba1.SqlBuilder == dba2.SqlBuilder);
				Assert.IsTrue (dba1.Factory == dba2.Factory);

				DbTransaction tr0 = infrastructure.BeginTransaction (DbTransactionMode.ReadOnly);
				DbTransaction tr1 = infrastructure.BeginTransaction (DbTransactionMode.ReadWrite, dba1);
				DbTransaction tr2 = infrastructure.BeginTransaction (DbTransactionMode.ReadOnly, dba2);

				Assert.AreEqual (3, infrastructure.LiveTransactions.Length);

				tr2.Commit ();

				Assert.AreEqual (2, infrastructure.LiveTransactions.Length);

				tr1.Rollback ();

				Assert.AreEqual (1, infrastructure.LiveTransactions.Length);

				tr0.Commit ();

				Assert.AreEqual (0, infrastructure.LiveTransactions.Length);

				dba1.Dispose ();
				dba2.Dispose ();
			}
		}

		[Test]
		public void Check15ClientManager()
		{
			using (DbInfrastructure infrastructure = DbInfrastructureTest.GetInfrastructureFromBase ("fiche", false))
			{
				infrastructure.LocalSettings.IsServer = true;
				DbClientManager cm = infrastructure.ClientManager;
				infrastructure.LocalSettings.IsServer = false;

				Assert.IsNotNull (cm);
				Assert.AreEqual (0, cm.FindAllClients ().Length);

				DbClientRecord client_1;
				DbClientRecord client_2;
				DbClientRecord client_3;

				client_1 = cm.CreateNewClient ("A");
				client_2 = cm.CreateNewClient ("B");

				Assert.AreEqual (DbClientManager.MinClientId, client_1.ClientId);
				Assert.AreEqual (DbClientManager.MinClientId, client_2.ClientId);
				Assert.AreEqual (client_1.CreationDateTime, client_1.ConnectionDateTime);
				Assert.AreEqual (client_2.CreationDateTime, client_2.ConnectionDateTime);

				client_1 = cm.CreateAndInsertNewClient ("A");
				client_2 = cm.CreateAndInsertNewClient ("B");
				client_3 = cm.CreateAndInsertNewClient ("C");

				Assert.AreEqual (DbClientManager.MinClientId + 0, client_1.ClientId);
				Assert.AreEqual (DbClientManager.MinClientId + 1, client_2.ClientId);
				Assert.AreEqual (DbClientManager.MinClientId + 2, client_3.ClientId);

				cm.Remove (client_2.ClientId);

				client_2 = cm.CreateNewClient ("D");

				Assert.AreEqual (DbClientManager.MinClientId + 1, client_2.ClientId);

				cm.Insert (client_2);

				using (DbTransaction transaction = infrastructure.BeginTransaction ())
				{
					cm.PersistToBase (transaction);
					transaction.Commit ();
				}

				cm.Remove (client_2.ClientId);

				client_2 = cm.CreateNewClient ("E");

				Assert.AreEqual (DbClientManager.MinClientId + 1, client_2.ClientId);

				cm.Insert (client_2);
				cm.Remove (client_1.ClientId);
				cm.Remove (client_2.ClientId);
				cm.Remove (client_3.ClientId);

				using (DbTransaction transaction = infrastructure.BeginTransaction ())
				{
					cm.PersistToBase (transaction);
					transaction.Commit ();
				}
			}
		}

		[Test]
		[ExpectedException (typeof (Exceptions.ExistsException))]
		public void Check16ClientManagerEx()
		{
			using (DbInfrastructure infrastructure = DbInfrastructureTest.GetInfrastructureFromBase ("fiche", false))
			{
				infrastructure.LocalSettings.IsServer = true;
				DbClientManager cm = infrastructure.ClientManager;
				infrastructure.LocalSettings.IsServer = false;

				DbClientRecord client_1;
				DbClientRecord client_2;

				client_1 = cm.CreateNewClient ("A");
				client_2 = cm.CreateNewClient ("B");

				cm.Insert (client_1);
				cm.Insert (client_2);
			}
		}


		#region Support Code
		internal static DbInfrastructure GetInfrastructureFromBase(string name, bool debug_attach)
		{
			DbInfrastructure infrastructure = new DbInfrastructure ();
			DbAccess db_access = DbInfrastructure.CreateDbAccess (name);

			if (debug_attach)
			{
				infrastructure.DisplayDataSet = new CallbackDisplayDataSet (DbInfrastructureTest.DisplayDataSet);
				infrastructure.AttachDatabase (db_access);
			}
			else
			{
				infrastructure.AttachDatabase (db_access);
				infrastructure.DisplayDataSet = new CallbackDisplayDataSet (DbInfrastructureTest.DisplayDataSet);
			}

			return infrastructure;
		}

		public static void DisplayDataSet(DbInfrastructure infrastructure, string name, System.Data.DataTable table)
		{
			//			DbInfrastructureTest.display.AddTable (name, table);
			//			DbInfrastructureTest.display.ShowWindow ();
		}
		#endregion
	}
}
