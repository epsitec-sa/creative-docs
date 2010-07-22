using Epsitec.Common.Support;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Expressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.UnitTests
{


	[TestClass]
	public sealed class UnitTestField
	{


		[TestMethod]
		public void FieldConstructorTest()
		{
			foreach (Druid druid in this.GetSampleData ())
			{
				new Field (druid);
			}
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void FieldIdTest()
		{
			foreach (Druid druid in this.GetSampleData ())
			{
				Field field = new Field (druid);

				Assert.AreEqual (druid, field.FieldId);
			}
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void CreateDbTableColumnTest()
		{
			Dictionary<Druid, DbTableColumn> resolver = new Dictionary<Druid, DbTableColumn> ();

			foreach (Druid druid in this.GetSampleData ())
			{
				DbTable dbTable = new DbTable (druid);
				DbColumn dbColumn = new DbColumn (druid, new DbTypeDef ());
				dbTable.Columns.Add (dbColumn);

				DbTableColumn dbTableColumn = new DbTableColumn (dbColumn);

				resolver[druid] = dbTableColumn;
			}

			foreach (Druid druid in this.GetSampleData ())
			{
				Field_Accessor field = new Field_Accessor (druid);

				DbTableColumn dbTableColumn1 = field.CreateDbTableColumn (id => resolver[id]);
				DbTableColumn dbTableColumn2 = resolver[druid];

				Assert.AreSame (dbTableColumn1, dbTableColumn2);
			}
		}


		private IEnumerable<Druid> GetSampleData()
		{
			for (int i = 0; i < 100; i++)
			{
				yield return Druid.FromLong (i);	
			}
		}


	}


}
