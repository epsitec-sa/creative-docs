//	Copyright © 2006-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;

using Demo.Demo5juin.Entities;

using NUnit.Framework;

using System.Collections.Generic;

namespace Epsitec.Cresus.DataLayer
{
	[TestFixture]
	public class DataBrowserTest
	{
		[SetUp]
		public void Initialize()
		{
			this.context = new DataContext (this.infrastructure);

			Assert.IsTrue (this.context.CreateSchema<ArticleEntity> ());
			Assert.IsTrue (this.context.CreateSchema<ArticleVisserieEntity> ());
			Assert.IsTrue (this.context.CreateSchema<PositionEntity> ());
			
			bool result = this.context.CreateSchema<AdresseEntity> ();

			AdresseEntity a1 = this.context.CreateEntity<AdresseEntity> ();
			AdresseEntity a2 = this.context.CreateEntity<AdresseEntity> ();
			AdresseEntity a3 = this.context.CreateEntity<AdresseEntity> ();

			a1.Désignation = "Pierre ARNAUD";	//	[63073]
			a1.Rue = "Ch. du Fontenay";			//	[63083]
			a1.Numéro = "6";
			a1.Npa = "1400";
			a1.Ville = "Yverdon-les-Bains";		//	[630B3]
			a1.Pays = "CH";

			a2.Désignation = "Daniel ROUX";
			a2.Rue = "Ch. de la Crésentine";
			a2.Numéro = "33";
			a2.Npa = "1023";
			a2.Ville = "Crissier";
			a2.Pays = "CH";

			a3.Désignation = "Cathi Nicoud";
			a3.Rue = "Ch. de la Mouette";
			a3.Numéro = "5";
			a3.Npa = "1092";
			a3.Ville = "Belmont/Lausanne";
			a3.Pays = "CH";

			this.context.SaveChanges ();
		}

		[Test]
		public void Check01Reader()
		{
			DbReader reader = new DbReader (this.infrastructure);

			List<DbTableColumn> tableColumns = new List<DbTableColumn> ();
			
			DbTable  t1 = this.context.SchemaEngine.FindTableDefinition (Druid.Parse ("[63081]"));
			
			DbColumn c1 = t1.Columns["63073"];
			DbColumn c2 = t1.Columns["63083"];
			DbColumn c3 = t1.Columns["630B3"];

			tableColumns.Add (new DbTableColumn ("T1", t1, "Name", c1));
			tableColumns.Add (new DbTableColumn ("T1", t1, "Street", c2));
			tableColumns.Add (new DbTableColumn ("T1", t1, "City", c3));

			using (DbTransaction transaction = this.infrastructure.BeginTransaction ())
			{
				System.Data.IDataReader dataReader = reader.CreateReader (transaction, tableColumns);

				while (dataReader.Read ())
				{
					System.Console.Out.WriteLine ("{0}, {1}, {2}", dataReader.GetString (0), dataReader.GetString (1), dataReader.GetString (2));
				}

				dataReader.Close ();
				transaction.Commit ();
			}
		}

		private readonly DbInfrastructure infrastructure = TestSupport.Database.NewInfrastructure ();
		private DataContext context;
	}
}
