//	Copyright © 2006-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;

using NUnit.Framework;

using System.Collections.Generic;

namespace Epsitec.Cresus.DataLayer
{
	[TestFixture]
	public class AdapterTest
	{
#if false
		[TestFixtureSetUp]
		public void Setup()
		{
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
				infrastructure.CreateDatabase (DbInfrastructure.CreateDatabaseAccess ("FICHE"));
			}

			this.infrastructure = new DbInfrastructure ();
			this.infrastructure.AttachToDatabase (DbInfrastructure.CreateDatabaseAccess ("FICHE"));
		}

		[TestFixtureTearDown]
		public void TearDown()
		{
			this.infrastructure.Dispose ();
			this.infrastructure = null;
		}

		[Test]
		public void Check01CreateTypeDefinition()
		{
			using (DbTransaction transaction = this.infrastructure.BeginTransaction ())
			{
				DbTypeDef typeDef1 = Adapter.CreateTypeDefinition (transaction, DecimalType.Default);
				DbTypeDef typeDef2 = Adapter.CreateTypeDefinition (transaction, StringType.Default);

				transaction.Commit ();
			}
		}

		[Test]
		public void Check02CreateTableDefinition()
		{
			System.Diagnostics.Debug.WriteLine ("Create Table Definition 1");
			using (DbTransaction transaction = this.infrastructure.BeginTransaction ())
			{
				DbTable table = Adapter.CreateTableDefinition (transaction, Res.Types.Record.Address);
				
				System.Diagnostics.Debug.WriteLine ("Create Table Definition 1 before commit");

				transaction.Commit ();
			}
			System.Diagnostics.Debug.WriteLine ("Create Table Definition 1 done");
		}

		[Test]
		public void Check03CreateTableDefinitionWithRelations()
		{
			System.Diagnostics.Debug.WriteLine ("Create Table Definition 2");

			using (DbTransaction transaction = this.infrastructure.BeginTransaction ())
			{
				DbTable table = Adapter.CreateTableDefinition (transaction, Res.Types.Record.Invoice);

				System.Diagnostics.Debug.WriteLine ("Create Table Definition 2 before commit");
				
				transaction.Commit ();
			}

			System.Diagnostics.Debug.WriteLine ("Create Table Definition 2 done");
		}

		[Test]
		public void Check04FindTableDefinition()
		{
			using (DbTransaction transaction = this.infrastructure.BeginTransaction (DbTransactionMode.ReadOnly))
			{
				DbTable table1 = Adapter.FindTableDefinition (transaction, Res.Types.Record.Address);
				DbTable table2 = Adapter.FindTableDefinition (transaction, Res.Types.Record.Invoice);

				Assert.IsNotNull (table1);
				Assert.IsNotNull (table2);

				Assert.AreEqual ("Record.Address", table1.Name);
				Assert.AreEqual ("Record.Invoice", table2.Name);
			}
		}

		[Test]
		public void Check05FindTypeDefinition()
		{
			using (DbTransaction transaction = this.infrastructure.BeginTransaction (DbTransactionMode.ReadOnly))
			{
				DbTypeDef type1 = Adapter.FindTypeDefinition (transaction, Res.Types.Text.Name);
				DbTypeDef type2 = Adapter.FindTypeDefinition (transaction, Res.Types.Num.ZipCode);
				DbTypeDef type3 = Adapter.FindTypeDefinition (transaction, Res.Types.Num.MonetaryAmount);

				Assert.IsNotNull (type1);
				Assert.IsNotNull (type2);
				Assert.IsNotNull (type3);

				Assert.AreEqual ("Text.Name", type1.Name);
				Assert.AreEqual ("Num.ZipCode", type2.Name);
				Assert.AreEqual ("Num.MonetaryAmount", type3.Name);
			}
		}

		private DbInfrastructure infrastructure;
#endif
	}
}
