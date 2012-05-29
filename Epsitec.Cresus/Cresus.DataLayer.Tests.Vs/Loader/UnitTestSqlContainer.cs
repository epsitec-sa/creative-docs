using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Loader;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.Loader
{


	[TestClass]
	public sealed class UnitTestSqlContainer
	{


		[TestMethod]
		public void ConstructorArgumentCheck()
		{
			List<SqlField> tables = new List<SqlField> ();
			List<SqlField> fields = new List<SqlField> ();
			List<SqlJoin> joins = new List<SqlJoin> ();
			List<SqlFunction> conditions = new List<SqlFunction> ();
			List<SqlField> orderBys = new List<SqlField> ();

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new SqlContainer (null, fields, joins, conditions, orderBys)
			);

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new SqlContainer (tables, null, joins, conditions, orderBys)
			);

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new SqlContainer (tables, fields, null, conditions, orderBys)
			);

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new SqlContainer (tables, fields, joins, null, orderBys)
			);

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new SqlContainer (tables, fields, joins, conditions, null)
			);
		}


		[TestMethod]
		public void Constructor()
		{
			List<SqlField> tables = this.GetTableSamples ();
			List<SqlField> fields = this.GetFieldSamples ();
			List<SqlJoin> joins = this.GetJoinSamples ();
			List<SqlFunction> conditions = this.GetConditionSamples ();
			List<SqlField> orderBys = this.GetOrderBySamples ();

			SqlContainer sqlContainer = new SqlContainer (tables, fields, joins, conditions, orderBys);

			CollectionAssert.AreEqual (tables, sqlContainer.SqlTables.ToList ());
			CollectionAssert.AreEqual (fields, sqlContainer.SqlFields.ToList ());
			CollectionAssert.AreEqual (joins, sqlContainer.SqlJoins.ToList ());
			CollectionAssert.AreEqual (conditions, sqlContainer.SqlConditions.ToList ());
			CollectionAssert.AreEqual (orderBys, sqlContainer.SqlOrderBys.ToList ());
		}


		[TestMethod]
		public void BuildSqlSelect1()
		{
			List<SqlField> tables = this.GetTableSamples ();
			List<SqlField> fields = this.GetFieldSamples ();
			List<SqlJoin> joins = this.GetJoinSamples ();
			List<SqlFunction> conditions = this.GetConditionSamples ();
			List<SqlField> orderBys = this.GetOrderBySamples ();

			SqlContainer sqlContainer = new SqlContainer (tables, fields, joins, conditions, orderBys);
			SqlSelect sqlSelect = sqlContainer.BuildSqlSelect ();

			CollectionAssert.AreEqual (tables, sqlSelect.Tables);
			CollectionAssert.AreEqual (fields, sqlSelect.Fields);
			CollectionAssert.AreEqual (joins, sqlSelect.Joins.Select (j => j.AsJoin).ToList ());
			CollectionAssert.AreEqual (conditions, sqlSelect.Conditions.Select (f => f.AsFunction).ToList ());
			CollectionAssert.AreEqual (orderBys, sqlSelect.OrderBy);
			Assert.AreEqual (SqlSelectPredicate.All, sqlSelect.Predicate);
		}


		[TestMethod]
		public void BuildSqlSelect2()
		{
			List<SqlField> tables = this.GetTableSamples ();
			List<SqlField> fields = this.GetFieldSamples ();
			List<SqlJoin> joins = this.GetJoinSamples ();
			List<SqlFunction> conditions = this.GetConditionSamples ();
			List<SqlField> orderBys = this.GetOrderBySamples ();

			SqlContainer sqlContainer = new SqlContainer (tables, fields, joins, conditions, orderBys);
			SqlSelect sqlSelect = sqlContainer.BuildSqlSelect (SqlSelectPredicate.Distinct);

			CollectionAssert.AreEqual (tables, sqlSelect.Tables);
			CollectionAssert.AreEqual (fields, sqlSelect.Fields);
			CollectionAssert.AreEqual (joins, sqlSelect.Joins.Select (j => j.AsJoin).ToList ());
			CollectionAssert.AreEqual (conditions, sqlSelect.Conditions.Select (f => f.AsFunction).ToList ());
			CollectionAssert.AreEqual (orderBys, sqlSelect.OrderBy);
			Assert.AreEqual (SqlSelectPredicate.Distinct, sqlSelect.Predicate);
		}


		[TestMethod]
		public void BuildSqlSelect3()
		{
			List<SqlField> tables = this.GetTableSamples ();
			List<SqlField> fields = this.GetFieldSamples ();
			List<SqlJoin> joins = this.GetJoinSamples ();
			List<SqlFunction> conditions = this.GetConditionSamples ();
			List<SqlField> orderBys = this.GetOrderBySamples ();

			SqlContainer sqlContainer = new SqlContainer (tables, fields, joins, conditions, orderBys);

			var data = Enumerable.Range (0, 10).Cast<int?> ().Concat (new List<int?> () { null });

			foreach (var skip in data)
			{
				foreach (var take in data)
				{
					SqlSelect sqlSelect = sqlContainer.BuildSqlSelect (SqlSelectPredicate.Distinct, skip, take);

					CollectionAssert.AreEqual (tables, sqlSelect.Tables);
					CollectionAssert.AreEqual (fields, sqlSelect.Fields);
					CollectionAssert.AreEqual (joins, sqlSelect.Joins.Select (j => j.AsJoin).ToList ());
					CollectionAssert.AreEqual (conditions, sqlSelect.Conditions.Select (f => f.AsFunction).ToList ());
					CollectionAssert.AreEqual (orderBys, sqlSelect.OrderBy);
					Assert.AreEqual (SqlSelectPredicate.Distinct, sqlSelect.Predicate);
					Assert.AreEqual (skip, sqlSelect.Skip);
					Assert.AreEqual (take, sqlSelect.Take);
				}
			}
		}


		[TestMethod]
		public void CreateSqlTablesArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => SqlContainer.CreateSqlTables (null)
			);
		}


		[TestMethod]
		public void CreateSqlTables()
		{
			List<SqlField> tables = this.GetTableSamples ();
			List<SqlField> fields = new List<SqlField> ();
			List<SqlJoin> joins = new List<SqlJoin> ();
			List<SqlFunction> conditions = new List<SqlFunction> ();
			List<SqlField> orderBys = new List<SqlField> ();

			SqlContainer sqlContainer = SqlContainer.CreateSqlTables (tables.ToArray ());

			CollectionAssert.AreEqual (tables, sqlContainer.SqlTables.ToList ());
			CollectionAssert.AreEqual (fields, sqlContainer.SqlFields.ToList ());
			CollectionAssert.AreEqual (joins, sqlContainer.SqlJoins.ToList ());
			CollectionAssert.AreEqual (conditions, sqlContainer.SqlConditions.ToList ());
			CollectionAssert.AreEqual (orderBys, sqlContainer.SqlOrderBys.ToList ());
		}


		[TestMethod]
		public void CreateSqlFieldsArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => SqlContainer.CreateSqlFields (null)
			);
		}


		[TestMethod]
		public void CreateSqlFields()
		{
			List<SqlField> tables = new List<SqlField> ();
			List<SqlField> fields = this.GetFieldSamples ();
			List<SqlJoin> joins = new List<SqlJoin> ();
			List<SqlFunction> conditions = new List<SqlFunction> ();
			List<SqlField> orderBys = new List<SqlField> ();

			SqlContainer sqlContainer = SqlContainer.CreateSqlFields (fields.ToArray ());

			CollectionAssert.AreEqual (tables, sqlContainer.SqlTables.ToList ());
			CollectionAssert.AreEqual (fields, sqlContainer.SqlFields.ToList ());
			CollectionAssert.AreEqual (joins, sqlContainer.SqlJoins.ToList ());
			CollectionAssert.AreEqual (conditions, sqlContainer.SqlConditions.ToList ());
			CollectionAssert.AreEqual (orderBys, sqlContainer.SqlOrderBys.ToList ());
		}


		[TestMethod]
		public void CreateSqlJoinsArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => SqlContainer.CreateSqlJoins (null)
			);
		}


		[TestMethod]
		public void CreateSqlJoins()
		{
			List<SqlField> tables = new List<SqlField> ();
			List<SqlField> fields = new List<SqlField> ();
			List<SqlJoin> joins = this.GetJoinSamples ();
			List<SqlFunction> conditions = new List<SqlFunction> ();
			List<SqlField> orderBys = new List<SqlField> ();

			SqlContainer sqlContainer = SqlContainer.CreateSqlJoins (joins.ToArray ());

			CollectionAssert.AreEqual (tables, sqlContainer.SqlTables.ToList ());
			CollectionAssert.AreEqual (fields, sqlContainer.SqlFields.ToList ());
			CollectionAssert.AreEqual (joins, sqlContainer.SqlJoins.ToList ());
			CollectionAssert.AreEqual (conditions, sqlContainer.SqlConditions.ToList ());
			CollectionAssert.AreEqual (orderBys, sqlContainer.SqlOrderBys.ToList ());
		}


		[TestMethod]
		public void CreateSqlConditionsArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => SqlContainer.CreateSqlConditions (null)
			);
		}


		[TestMethod]
		public void CreateSqlConditions()
		{
			List<SqlField> tables = new List<SqlField> ();
			List<SqlField> fields = new List<SqlField> ();
			List<SqlJoin> joins = new List<SqlJoin> ();
			List<SqlFunction> conditions = this.GetConditionSamples ();
			List<SqlField> orderBys = new List<SqlField> ();

			SqlContainer sqlContainer = SqlContainer.CreateSqlConditions (conditions.ToArray ());

			CollectionAssert.AreEqual (tables, sqlContainer.SqlTables.ToList ());
			CollectionAssert.AreEqual (fields, sqlContainer.SqlFields.ToList ());
			CollectionAssert.AreEqual (joins, sqlContainer.SqlJoins.ToList ());
			CollectionAssert.AreEqual (conditions, sqlContainer.SqlConditions.ToList ());
			CollectionAssert.AreEqual (orderBys, sqlContainer.SqlOrderBys.ToList ());
		}


		[TestMethod]
		public void CreateSqlOrderBysArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => SqlContainer.CreateSqlOrderBys (null)
			);
		}


		[TestMethod]
		public void CreateSqlOrderBys()
		{
			List<SqlField> tables = new List<SqlField> ();
			List<SqlField> fields = new List<SqlField> ();
			List<SqlJoin> joins = new List<SqlJoin> ();
			List<SqlFunction> conditions = new List<SqlFunction> ();
			List<SqlField> orderBys = this.GetOrderBySamples ();

			SqlContainer sqlContainer = SqlContainer.CreateSqlOrderBys (orderBys.ToArray ());

			CollectionAssert.AreEqual (tables, sqlContainer.SqlTables.ToList ());
			CollectionAssert.AreEqual (fields, sqlContainer.SqlFields.ToList ());
			CollectionAssert.AreEqual (joins, sqlContainer.SqlJoins.ToList ());
			CollectionAssert.AreEqual (conditions, sqlContainer.SqlConditions.ToList ());
			CollectionAssert.AreEqual (orderBys, sqlContainer.SqlOrderBys.ToList ());
		}


		[TestMethod]
		public void PlusArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => SqlContainer.Empty.Plus (null)
			);
		}


		[TestMethod]
		public void Plus()
		{
			List<SqlField> tables = this.GetTableSamples ();
			List<SqlField> fields = this.GetFieldSamples ();
			List<SqlJoin> joins = this.GetJoinSamples ();
			List<SqlFunction> conditions = this.GetConditionSamples ();
			List<SqlField> orderBys = this.GetOrderBySamples ();

			SqlContainer sqlContainer = new SqlContainer (tables, fields, joins, conditions, orderBys);

			sqlContainer = sqlContainer.Plus (sqlContainer);

			CollectionAssert.AreEqual (tables.Concat (tables).ToList (), sqlContainer.SqlTables.ToList ());
			CollectionAssert.AreEqual (fields.Concat (fields).ToList (), sqlContainer.SqlFields.ToList ());
			CollectionAssert.AreEqual (joins.Concat (joins).ToList (), sqlContainer.SqlJoins.ToList ());
			CollectionAssert.AreEqual (conditions.Concat (conditions).ToList (), sqlContainer.SqlConditions.ToList ());
			CollectionAssert.AreEqual (orderBys.Concat (orderBys).ToList (), sqlContainer.SqlOrderBys.ToList ());
		}


		[TestMethod]
		public void PlusSqlTablesArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => SqlContainer.Empty.PlusSqlTables (null)
			);
		}


		[TestMethod]
		public void PlusSqlTables()
		{
			List<SqlField> tables = this.GetTableSamples ();
			List<SqlField> fields = new List<SqlField> ();
			List<SqlJoin> joins = new List<SqlJoin> ();
			List<SqlFunction> conditions = new List<SqlFunction> ();
			List<SqlField> orderBys = new List<SqlField> ();

			SqlContainer sqlContainer = SqlContainer.CreateSqlTables (tables.ToArray ()).PlusSqlTables (tables.ToArray ());

			CollectionAssert.AreEqual (tables.Concat (tables).ToList (), sqlContainer.SqlTables.ToList ());
			CollectionAssert.AreEqual (fields, sqlContainer.SqlFields.ToList ());
			CollectionAssert.AreEqual (joins, sqlContainer.SqlJoins.ToList ());
			CollectionAssert.AreEqual (conditions, sqlContainer.SqlConditions.ToList ());
			CollectionAssert.AreEqual (orderBys, sqlContainer.SqlOrderBys.ToList ());
		}


		[TestMethod]
		public void PlusSqlFieldsArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => SqlContainer.Empty.PlusSqlFields (null)
			);
		}


		[TestMethod]
		public void PlusSqlFields()
		{
			List<SqlField> tables = new List<SqlField> ();
			List<SqlField> fields = this.GetFieldSamples ();
			List<SqlJoin> joins = new List<SqlJoin> ();
			List<SqlFunction> conditions = new List<SqlFunction> ();
			List<SqlField> orderBys = new List<SqlField> ();

			SqlContainer sqlContainer = SqlContainer.CreateSqlFields (fields.ToArray ()).PlusSqlFields (fields.ToArray ());

			CollectionAssert.AreEqual (tables, sqlContainer.SqlTables.ToList ());
			CollectionAssert.AreEqual (fields.Concat (fields).ToList (), sqlContainer.SqlFields.ToList ());
			CollectionAssert.AreEqual (joins, sqlContainer.SqlJoins.ToList ());
			CollectionAssert.AreEqual (conditions, sqlContainer.SqlConditions.ToList ());
			CollectionAssert.AreEqual (orderBys, sqlContainer.SqlOrderBys.ToList ());
		}


		[TestMethod]
		public void PlusSqlJoinsArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => SqlContainer.Empty.PlusSqlJoins (null)
			);
		}


		[TestMethod]
		public void PlusSqlJoins()
		{
			List<SqlField> tables = new List<SqlField> ();
			List<SqlField> fields = new List<SqlField> ();
			List<SqlJoin> joins = this.GetJoinSamples ();
			List<SqlFunction> conditions = new List<SqlFunction> ();
			List<SqlField> orderBys = new List<SqlField> ();

			SqlContainer sqlContainer = SqlContainer.CreateSqlJoins (joins.ToArray ()).PlusSqlJoins (joins.ToArray ());

			CollectionAssert.AreEqual (tables, sqlContainer.SqlTables.ToList ());
			CollectionAssert.AreEqual (fields, sqlContainer.SqlFields.ToList ());
			CollectionAssert.AreEqual (joins.Concat (joins).ToList (), sqlContainer.SqlJoins.ToList ());
			CollectionAssert.AreEqual (conditions, sqlContainer.SqlConditions.ToList ());
			CollectionAssert.AreEqual (orderBys, sqlContainer.SqlOrderBys.ToList ());
		}


		[TestMethod]
		public void PlusSqlConditionsArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => SqlContainer.Empty.PlusSqlConditions (null)
			);
		}


		[TestMethod]
		public void PlusSqlConditions()
		{
			List<SqlField> tables = new List<SqlField> ();
			List<SqlField> fields = new List<SqlField> ();
			List<SqlJoin> joins = new List<SqlJoin> ();
			List<SqlFunction> conditions = this.GetConditionSamples ();
			List<SqlField> orderBys = new List<SqlField> ();

			SqlContainer sqlContainer = SqlContainer.CreateSqlConditions (conditions.ToArray ()).PlusSqlConditions (conditions.ToArray ());

			CollectionAssert.AreEqual (tables, sqlContainer.SqlTables.ToList ());
			CollectionAssert.AreEqual (fields, sqlContainer.SqlFields.ToList ());
			CollectionAssert.AreEqual (joins, sqlContainer.SqlJoins.ToList ());
			CollectionAssert.AreEqual (conditions.Concat (conditions).ToList (), sqlContainer.SqlConditions.ToList ());
			CollectionAssert.AreEqual (orderBys, sqlContainer.SqlOrderBys.ToList ());
		}


		[TestMethod]
		public void PlusSqlOrderBysArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => SqlContainer.Empty.PlusSqlOrderBys (null)
			);
		}


		[TestMethod]
		public void PlusSqlOrderBys()
		{
			List<SqlField> tables = new List<SqlField> ();
			List<SqlField> fields = new List<SqlField> ();
			List<SqlJoin> joins = new List<SqlJoin> ();
			List<SqlFunction> conditions = new List<SqlFunction> ();
			List<SqlField> orderBys = this.GetOrderBySamples ();

			SqlContainer sqlContainer = SqlContainer.CreateSqlOrderBys (orderBys.ToArray ()).PlusSqlOrderBys (orderBys.ToArray ());

			CollectionAssert.AreEqual (tables, sqlContainer.SqlTables.ToList ());
			CollectionAssert.AreEqual (fields, sqlContainer.SqlFields.ToList ());
			CollectionAssert.AreEqual (joins, sqlContainer.SqlJoins.ToList ());
			CollectionAssert.AreEqual (conditions, sqlContainer.SqlConditions.ToList ());
			CollectionAssert.AreEqual (orderBys.Concat (orderBys).ToList (), sqlContainer.SqlOrderBys.ToList ());
		}


		[TestMethod]
		public void Empty()
		{
			Assert.IsFalse (SqlContainer.Empty.SqlTables.Any ());
			Assert.IsFalse (SqlContainer.Empty.SqlFields.Any ());
			Assert.IsFalse (SqlContainer.Empty.SqlJoins.Any ());
			Assert.IsFalse (SqlContainer.Empty.SqlConditions.Any ());
			Assert.IsFalse (SqlContainer.Empty.SqlOrderBys.Any ());
		}


		private List<SqlField> GetTableSamples()
		{
			return new List<SqlField> ()
			{
				SqlField.CreateName ("table1"),
				SqlField.CreateName ("table2"),
			};
		}


		private List<SqlField> GetFieldSamples()
		{
			return new List<SqlField> ()
			{
				SqlField.CreateName ("field1"),
				SqlField.CreateName ("field2"),
			};
		}


		private List<SqlJoin> GetJoinSamples()
		{
			return new List<SqlJoin> ()
			{
				SqlJoin.Create (SqlJoinCode.Inner, SqlField.CreateAliasedName ("a", "a1"), SqlField.CreateAliasedName ("a", "field1", "field1"), SqlField.CreateAliasedName ("a", "field2", "field2")),
				SqlJoin.Create (SqlJoinCode.OuterLeft, SqlField.CreateAliasedName ("a", "a2"), SqlField.CreateAliasedName ("a", "field3", "field3"), SqlField.CreateAliasedName ("a", "field4", "field4")),
			};
		}

		private List<SqlFunction> GetConditionSamples()
		{
			return new List<SqlFunction> ()
			{
				new SqlFunction (SqlFunctionCode.LogicOr, SqlField.CreateName ("field1"), SqlField.CreateName ("field2")),
				new SqlFunction (SqlFunctionCode.LogicAnd, SqlField.CreateName ("field3"), SqlField.CreateName ("field4")),
			};
		}


		private List<SqlField> GetOrderBySamples()
		{
			return new List<SqlField> ()
			{
				SqlField.CreateName ("orderby1"),
				SqlField.CreateName ("orderby2"),
			};
		}


	}


}
