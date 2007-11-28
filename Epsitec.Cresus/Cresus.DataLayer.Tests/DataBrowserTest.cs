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

			int count = 0;

			System.Diagnostics.Debug.WriteLine ("Reading sample data file");

			foreach (string[] cols in this.ReadInfiniteSampleDataFile ())
			{
				AdresseEntity a = this.context.CreateEntity<AdresseEntity> ();

				a.Désignation = cols[0].Length == 0 ? string.Concat (cols[1], " ", cols[3], " ", cols[2]).Trim () : cols[0];
				a.Rue = cols[4];
				a.Npa = cols[5];
				a.Ville = cols[6];

				count++;

				if ((count % 1000) == 0)
				{
					this.context.SaveChanges ();
					System.Console.Out.WriteLine ("{0} created", count);
				}
				if (count == 10*1000)
				{
					break;
				}
			}

			System.Diagnostics.Debug.WriteLine ("Created " + count + " address records");

#if false
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
#endif

			this.context.SaveChanges ();

			System.Diagnostics.Debug.WriteLine ("Saved");
		}

		private IEnumerable<string[]> ReadInfiniteSampleDataFile()
		{
			while (true)
			{
				foreach (string line in System.IO.File.ReadAllLines (@"..\..\sample.csv"))
				{
					yield return line.Split (';');
				}
			}
		}

		[Test]
		public void Check01Reader()
		{
			List<DbTableColumn> queryFields = new List<DbTableColumn> ();
			
			DbTable  t1 = this.context.SchemaEngine.FindTableDefinition (Druid.Parse ("[63081]"));
			
			DbColumn c1 = t1.Columns["63073"];
			DbColumn c2 = t1.Columns["63083"];
			DbColumn c3 = t1.Columns["630B3"];

			queryFields.Add (new DbTableColumn ("T1", "Name", c1));
			queryFields.Add (new DbTableColumn ("T1", "Street", c2));
			queryFields.Add (new DbTableColumn ("T1", "City", c3));

			System.Diagnostics.Debug.WriteLine ("Starting reader");

			DbReader reader = new DbReader (this.infrastructure);

			reader.AddQueryFields (queryFields);

			List<string> lines = new List<string> ();
				
			using (DbTransaction transaction = this.infrastructure.BeginTransaction ())
			{
				System.Data.IDataReader dataReader = reader.CreateReader (transaction);

				while (dataReader.Read ())
				{
					lines.Add (string.Format ("{0}, {1}, {2}", dataReader.GetString (0), dataReader.GetString (1), dataReader.GetString (2)));
				}

				dataReader.Close ();
				transaction.Commit ();
			}

			System.Diagnostics.Debug.WriteLine ("Loaded " + lines.Count + " records");
		}

		private readonly DbInfrastructure infrastructure = TestSupport.Database.NewInfrastructure ();
		private DataContext context;
	}
}
