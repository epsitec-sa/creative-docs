//	Copyright © 2007-2014, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Marc BETTEX

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Expressions;
using Epsitec.Cresus.DataLayer.Schema;
using Epsitec.Cresus.DataLayer.Serialization;

using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Epsitec.Cresus.DataLayer.Loader
{
	/// <summary>
	/// The LoaderQueryGenerator is the class that builds and executes the SQL queries that are
	/// used to get entity data out of the database.
	/// </summary>
	internal sealed class LoaderQueryGenerator
	{
		// This class works by building the SQL queries piece by piece, putting them togeter,
		// executing them and packing the result within new objects.
		//
		// The core of this class is in the method BuildFromWhereAndOrderBy(...), which builds
		// the most important parts of the SQL queries, that is, the FROM part which references
		// all tables and their joins, the WHERE part that contains the condition that holds true
		// for all entities that match the example and the conditions given in the Request object,
		// and the ORDER BY part that corresponds to the sort clauses given in the Request.
		//
		// The other main methods, GetCount(...), GetEntityKeys(...), GetIndex(...),
		// GetValueAndReferenceData(...), GetCollectionData(...) all use the parts built by the
		// BuildFromWhereAndOrderBy(...) method as part of their SQL queries. They each simply add
		// a different SELECT part and may embed this SQL query within another one.
		//
		// In order to have those parts of the SQL queries, and to merge them, we store them in
		// instances of the SqlContainer class, that simple have references to these parts of the
		// SQL queries.
		// 
		// The most tricky parts of this class are contained within these methods that build the
		// SQL queries. The rest is comparatively easy, as the execution of the SQL queries is
		// encapsulated in the DbInfrastructure class, and the packing of the result is usually
		// straightforward.

		public LoaderQueryGenerator(DataContext dataContext)
		{
			this.dataContext = dataContext;
		}

		private DbInfrastructure				DbInfrastructure
		{
			get
			{
				return this.dataContext.DataInfrastructure.DbInfrastructure;
			}
		}

		private EntityTypeEngine				TypeEngine
		{
			get
			{
				return this.dataContext.DataInfrastructure.EntityEngine.EntityTypeEngine;
			}
		}

		private EntitySchemaEngine				SchemaEngine
		{
			get
			{
				return this.dataContext.DataInfrastructure.EntityEngine.EntitySchemaEngine;
			}
		}

		private DataConverter					DataConverter
		{
			get
			{
				return this.dataContext.DataConverter;
			}
		}


		public int GetCount(Request request)
		{
			// Builds the SQL query used to get the number of entities matching the given request,
			// executes it, and packs the result in an int value.

			var sqlSelect = this.BuildSelectForCount (request);

			using (var dbTransaction = this.StartTransaction ())
			{
				var count = this.GetInteger (sqlSelect, dbTransaction);

				dbTransaction.Commit ();

				return count;
			}
		}

		public int GetCount(Request request, DbTransaction dbTransaction)
		{
			// See comment in the other overload.

			var sqlSelect = this.BuildSelectForCount (request);

			return this.GetInteger (sqlSelect, dbTransaction);
		}


		private SqlSelect BuildSelectForCount(Request request)
		{
			// The SQL query that we want to build looks like this:
			//
			// SELECT COUNT(e.cr_id) FROM ... WHERE ... ROWS ... TO ...
			//
			// It is quite simple, as most of it is the standard parts of the SQL query used to to
			// filter and order the entities. In addition to these parts, we add the SELECT part
			// which simply counts the number of distinct entity ids in the resulting set.
			//
			// Here we don't include the usual ORDER BY clause as it is not necessary. We are
			// simply counting the elements within the given range, and that number doesn't change
			// based on the order of the elements.

			var builder = this.GetBuilder ();

			var fromWhereOrderBy = this.BuildFromAndWhere (builder, request);
			var fieldForCount = this.BuildFieldForCount (builder, request);

			return fromWhereOrderBy
				.PlusSqlFields (fieldForCount)
				.BuildSqlSelect (skip: request.Skip, take: request.Take);
		}

		private SqlField BuildFieldForCount(SqlFieldBuilder builder, Request request)
		{
			// HERE we build the part of the query that returns the number of entities that
			// satify the query. It look like this:
			//
			// SELECT COUNT(cr_id)
			//
			// If necessary, we add a DISTINCT predicate, to make sure that the same entity id
			// does not appear twice in the resulting set.

			var aggregate = SqlAggregateFunction.Count;
			var predicate = this.GetSqlSelectPredicate (request);
			var entityId = builder.BuildRootId (request.RequestedEntity);

			return SqlField.CreateAggregate (aggregate, predicate, entityId);
		}


		public IEnumerable<EntityKey> GetEntityKeys(Request request, DbTransaction dbTransaction)
		{
			// Builds the SQL query used to get the entity keys matching the given request,
			// executes it, and packs the result in an sequence of EntityKeys.

			var builder = this.GetBuilder ();
			var sqlSelect = this.BuildSelectForEntityKeys (request, builder);

			dbTransaction.SqlBuilder.SelectData (sqlSelect);

			var data = this.DbInfrastructure.ExecuteRetData (dbTransaction);

			var leafEntityTypeId = request.RequestedEntity.GetEntityStructuredTypeId ();
			var rootEntityTypeId = this.TypeEngine.GetRootType (leafEntityTypeId).CaptionId;

			return from DataRow row in data.Tables[0].Rows
				   let rowKey = this.ExtractKey (row[0])
				   select new EntityKey (rootEntityTypeId, rowKey);
		}

		public SqlSelect BuildSelectForEntityKeys(Request request, SqlFieldBuilder builder)
		{
			// The SQL query that we want to build looks like this:
			//
			// SELECT e.cr_id FROM ... WHERE ... ORDER BY ... ROWS ... TO ...
			//
			// It is quite simple, as most of it is the standard parts of the SQL query used to to
			// filter and order the entities. In addition to these parts, we add the SELECT part
			// which simply returns the distinct entity ids in the resulting set.
			//
			// We might add a DISTINCT clause if necessary to ensure that there is no duplicates in
			// the result set.

			var fromWhereOrderBy = this.BuildFromWhereAndOrderBy (builder, request);

			var entityId = builder.BuildRootId (request.RequestedEntity);
			var predicate = this.GetSqlSelectPredicate (request);

			return fromWhereOrderBy
				.PlusSqlFields (entityId)
				.BuildSqlSelect (predicate, request.Skip, request.Take);
		}

		public int? GetIndex(Request request, EntityKey entityKey)
		{
			// Builds the SQL query used to get the index of the given entity key within the result
			// set of the given request executes it, and packs the result in an int.

			// NOTE This SQL query could probably be improved by using the windowing function that
			// will be available in Firebird 3 which is not even yet in alpha stage.

			var sqlSelect = this.BuildSelectForIndex (request, entityKey);

			using (var dbTransaction = this.StartTransaction ())
			{
				var count = this.GetNullableInteger (sqlSelect, dbTransaction);

				dbTransaction.Commit ();

				return this.PostProcessIndex (count);
			}
		}

		public int? GetIndex(Request request, EntityKey entityKey, DbTransaction dbTransaction)
		{
			// See comment in the other overload.

			var sqlSelect = this.BuildSelectForIndex (request, entityKey);

			var count = this.GetNullableInteger (sqlSelect, dbTransaction);

			return this.PostProcessIndex (count);
		}


		private int? PostProcessIndex(int? count)
		{
			// At this point, we have the number of items that come before or at the same position
			// as the one whose index we want to compute. Our item is always included in this count
			// if it is part of the result set.

			// If we don't have that number. That means that the entity whose index we want was not
			// in the result set.
			if (!count.HasValue)
			{
				return null;
			}

			// Same thing here, the entity whose index we want was not in the resulting set
			if (count == 0)
			{
				return null;
			}

			// As our item is always included in the entities that are before it or at the same
			// position, we have to subtract one from the result to get the index.
			return count - 1;
		}

		private SqlSelect BuildSelectForIndex(Request request, EntityKey entityKey)
		{
			// This is probably the most complicated SQL queries built in this class. They will
			// look like this:
			//
			// SELECT COUNT(o.cr_id)
			// FROM [o]
			// CROSS JOIN SELECT [i.v1, i.v2, ..., i.vn]
			//            FROM [i]
			//            WHERE ... AND i.cr_id = entityKey
			//            ROWS 1
			// WHERE [o.v1, o.v2, ..., o.vn] <= [i.v1, i.v2, ..., i.vn]
			//
			// In this pseudo code, [i] are all the tables that are used in the SQL query that gets
			// the entity that matches entity key, and [o] are all eht tables that are used to get
			// the entities against the one we want to compare. [o.v1, o.v2, ..., o.vn] and [i.v1,
			// i.v2, ..., i.vn] is the vector of all values used in the sort clauses of the Request
			// and the <= operation on them returns true if the values of the first values are
			// smaller or equal to the second values.
			//
			// This looks terrible, but basically what we do here is that we count the number of
			// rows that come before or at the same position in the ordering than the row of the
			// entity whose index we want. Say that if we order persons by firstnames ascending, we
			// count the number of persons where the firstname is smaller or equal to the one of
			// the person whose index we want. When we have that number, assuming that the order
			// is strict and total, we subtract 1 from it and we have the index.
			//
			// If we look at what compose this query, we have 2 parts:
			// - The inner part:
			//   SELECT [i.v1, i.v2, ..., i.vn]
			//   FROM [i]
			//   WHERE ... AND i.cr_id = entityKey
			//   ROWS 1
			//   Its purpose is simply to pass the values that would be used in the sort clause
			//   for the entity whose index we want to the outer query.
			//   If we keep our example of persons, and the firstname of the person whose index we
			//   want is Albert, this inner query will return Albert. If we have several sort
			//   clauses, it will return the value for each sort clause.
			// - The outer part:
			//   SELECT COUNT(o.cr_id)
			//   FROM [o]
			//   CROSS JOIN ([inner sql request])
			//   WHERE [o.v1, o.v2, ..., o.vn] <= [i.v1, i.v2, ..., i.vn]
			//   The purpose of this outer query is to count all the entities whose values are
			//   smaller or equal than the values returned by the inner query.
			//   If we keep our example, this will return all persons whose firstname comes before
			//   or at the same position than Albert in the alphabetical order.
			//
			// If you want more details about the inner and outer request, you should go read the
			// comments in these methods. Here we only have the big picture.
			//
			// We cannot simply count the number of rows that come before our position in the order,
			// because then we can't make the difference between the cases where the item is the
			// first one or where the item is not in the list, as both SQL queries would return 0.

			var innerBuilder = this.GetBuilder ();
			var innerSelect = this.BuildInnerRequestForIndex (innerBuilder, request, entityKey);

			return this.BuildOuterRequestForIndex (request, innerBuilder, innerSelect);
		}

		private SqlSelect BuildInnerRequestForIndex(SqlFieldBuilder builder, Request request, EntityKey entityKey)
		{
			// This request is really simple, it looks like this:
			//
			// SELECT v1, v2, ..., vn FROM ... WHERE ... AND cr_id = entityKey ROWS 1
			//
			// We simply return the single row that matches the entity whose index we want. In this
			// row, we keep only the values that are used in the sort clauses, so that we can use
			// them to compare other rows with this one in the outer query.
			//
			// Here we don't need the standard ORDER BY clause, because there is at most one row
			// of data that will matche the condition. To speed up the query, we even express this
			// with a ROWS 1 clause. But this clause is here only for optimization purposes, there
			// would still be at most one row of data without it.

			var fromWhereAndOrderBy = this.BuildFromAndWhere (builder, request);
			var condition = this.BuildInnerRequestForIndexCondition (builder, request, entityKey);
			var fields = this.BuildInnerRequestForIndexFields (builder, request);

			return fromWhereAndOrderBy
				.PlusSqlConditions (condition)
				.PlusSqlFields (fields.ToArray ())
				.BuildSqlSelect (take: 1);
		}

		private SqlFunction BuildInnerRequestForIndexCondition(SqlFieldBuilder builder, Request request, EntityKey entityKey)
		{
			var op = SqlFunctionCode.CompareEqual;
			var idColumn = builder.BuildRootId (request.RequestedEntity);
			var idValue = builder.BuildConstantForKey (entityKey);

			return new SqlFunction (op, idColumn, idValue);
		}

		private IEnumerable<SqlField> BuildInnerRequestForIndexFields(SqlFieldBuilder builder, Request request)
		{
			// The same field of the same entity can appear more that once in the sort clause. If
			// that's the case, we must include it only once in the SQL request. That's why we keep
			// a set with all the fields that we have already included, so we don't include them
			// twice.

			var done = new HashSet<string> ();

			foreach (var sortClause in request.SortClauses)
			{
				var field = sortClause.Field.CreateSqlField (builder);
				var alias = this.GetAliasForInnerQueryForIndexField (field);

				if (!done.Contains (alias))
				{
					done.Add (alias);
					field.Alias = alias;

					yield return field;
				}
			}
		}


		public SqlSelect BuildOuterRequestForIndex(Request request, SqlFieldBuilder innerBuilder, SqlSelect innerSelect)
		{
			// This method builds the outer request of the SQL query that gets the index of an
			// entity. It looks like this:
			//
			// SELECT COUNT(o.cr_id)
			// FROM [o]
			// CROSS JOIN ([inner query])
			// WHERE [o.v1, o.v2, ..., o.vn] <= [i.v1, i.v2, ..., i.vn]
			//
			// It's not really tricky here, most of the complication is actuall in the method
			// that generates the <= condition, which is really tricky.
			//
			// The cross join here is used to add all the values of the single row of the inner
			// query to each rows of the outer query, so that both values can be compared in the
			// condition.

			// We don't want collisions between the aliases defined in the inner and outer
			// queries. As we are done building the inner query, we use the current count of its
			// alias manager to as the current count of the new alias manager.
			var outerBuilder = this.GetBuilder ();
			outerBuilder.AliasManager.AliasCount = innerBuilder.AliasManager.AliasCount;

			var fromAndWhere = this.BuildFromAndWhere (outerBuilder, request);

			var innerQueryAlias = outerBuilder.AliasManager.GetAlias ();
			var innerQueryJoin = this.BuildOuterRequestForIndexInnerQueryJoin (innerSelect, innerQueryAlias);

			var convertedOrderBy = this.BuildOuterRequestForIndexOrderByCondition (outerBuilder, innerBuilder, innerQueryAlias, request);

			var fieldForCount = this.BuildFieldForCount (outerBuilder, request);

			return fromAndWhere
				.PlusSqlJoins (innerQueryJoin)
				.PlusSqlConditions (convertedOrderBy)
				.PlusSqlFields (fieldForCount)
				.BuildSqlSelect ();
		}


		private SqlJoin BuildOuterRequestForIndexInnerQueryJoin(SqlSelect innerSelect, string subQueryAlias)
		{
			var subQuery = SqlField.CreateSubQuery (innerSelect, subQueryAlias);
			var subQueryJoinCode = SqlJoinCode.Cross;

			return new SqlJoin (subQueryJoinCode, subQuery);
		}
		
		private SqlFunction BuildOuterRequestForIndexOrderByCondition(SqlFieldBuilder outerBuilder, SqlFieldBuilder innerBuilder, string innerQueryAlias, Request request)
		{
			// OK. Here there are lots of complicated stuff going on. I'll try to explain it the
			// best as I can.
			//
			// We have n sort clauses, that I will call S1, S2, ..., SN-1, SN. Here we convert these
			// sort clauses into a predicate that will return TRUE for each row in the dataset that
			// come before or at the same position than the row in which we are interested.
			// 
			// I will call the rows of the data set RO (O for outer, as they are the row scanned in
			// the outer part of the query) and RI the row that they are compared against (I for
			// inner, as it is the single row returned by the inner part of the query). 
			//
			// Moreover, I define two functions. The function "equal" returns TRUE when the two rows
			// are considered equal by the sort clause. The function "before" returns TRUE, when the
			// first row is smaller than the other, according to the order of the sort clause. These
			// two functions are defined more formally below.
			// 
			// So, for clauses S1, ..., SN we want to generate the following predicate:
			// (before(RO, RI, S1))
			// OR (equal(RO, RI, S1) AND before(RO, RI, S2))
			// OR (equal(RO, RI, S1) AND equal(RO, RI, S2) AND before(RO, RI, S3))
			// ...
			// OR (equal(RO, RI, S1) AND equal(RO, RI, S2) AND equal(RO, RI, S3) and ... and before(RO, RI, SN))
			// OR (equal(RO, RI, S1) AND equal(RO, RI, S2) AND equal(RO, RI, S3) and ... and equal(RO, RI, SN))
			//
			// In english, that's what we want :
			// RO is before RI according to S1
			// or RO is equal to RI according to S1 and is before RI according to S2
			// or RO is equal to RI according to S1 and S2 and is before RI according to S3
			// ...
			// or RO is equal to RI according to S1, S2, S3, ..., SN-1 and is before RI according to SN
			// or RO is equal to RI according to S1, S2, S3, ..., SN
			//
			// Now I define more formally the two functions. They are complex because the values of
			// the columns might be NULL, and SQL sucks with null values. For these definitions,
			// consider that O and I are the values of the columns in which we are interested for
			// the current sort clause. V is a placeholder for any value that is guaranteed to be
			// NOT NULL.
			//
			// equals (O, I) => ((O IS NULL) AND (I IS NULL)) OR ((O IS NOT NULL) AND (I IS NOT NULL) AND (O = I))
			// before (O, I) for ASC sort clauses => (I IS NOT NULL) AND ((O IS NULL) OR (O < I))
			// before (O, I) for DESC sort clauses => (O IS NOT NULL) AND ((I IS NULL) OR (I < O))
			//
			// And now the explanation for these nasty expression, along with a proof that proves
			// that these expression can never return NULL values and thus can safely be combined
			// together or with other expressions without messing the overall result.
			//
			// equal (O, I) :
			//
			// I and O are equal in the order if and only if one of the two following conditions is
			// true :
			// - Both are NULL
			// - Both are not NULL and they are equal
			//
			// The SQL expression for that is :
			// ((O IS NULL) AND (I IS NULL)) OR ((O IS NOT NULL) AND (I IS NOT NULL) AND (O = I))
			//
			// This expression always returns a boolean value because we can reduce it like that :
			// - If O is not NULL and I is not NULL
			//   => ((O IS NULL) AND (I IS NULL)) OR ((O IS NOT NULL) AND (I IS NOT NULL) AND (O = I))
			//   => ((V IS NULL) AND (V IS NULL)) OR ((V IS NOT NULL) AND (V IS NOT NULL) AND (V = V))
			//   No NULL left.
			// - If O is NULL and I is not NULL
			//   => ((O IS NULL) AND (I IS NULL)) OR ((O IS NOT NULL) AND (I IS NOT NULL) AND (O = I))
			//   => ((NULL IS NULL) AND (V IS NULL)) OR ((NULL IS NOT NULL) AND (V IS NOT NULL) AND (NULL = V))
			//   => (TRUE AND FALSE) OR (FALSE AND TRUE AND NULL)
			//   => FALSE OR FALSE
			//   No NULL left.
			// - If O is not NULL and I is NULL
			//   => ((O IS NULL) AND (I IS NULL)) OR ((O IS NOT NULL) AND (I IS NOT NULL) AND (O = I))
			//   => ((V IS NULL) AND (NULL IS NULL)) OR ((V IS NOT NULL) AND (NULL IS NOT NULL) AND (V = NULL))
			//   => (FALSE AND TRUE) OR (TRUE AND FALSE AND NULL)
			//   => FALSE OR FALSE
			//   No NULL left.
			// - If O is NULL and I is NULL
			//   => ((O IS NULL) AND (I IS NULL)) OR ((O IS NOT NULL) AND (I IS NOT NULL) AND (O = I))
			//   => ((NULL IS NULL) AND (NULL IS NULL)) OR ((NULL IS NOT NULL) AND (NULL IS NOT NULL) AND (NULL = NULL))
			//   => (TRUE AND TRUE) OR (FALSE AND FALSE AND NULL)
			//   => TRUE OR FALSE
			//   No NULL left.
			//
			// before (O, I) for ASC sort clauses :
			//
			// O is before I in the order if and only if one of the two following conditions is
			// true :
			// - O is NULL and I is not NULL
			// - Both are not NULL and O is smaller than I
			//
			// The SQL expression for that is
			// ((O IS NULL) AND (I IS NOT NULL)) OR ((O IS NOT NULL) AND (I IS NOT NULL) AND (O < I))
			// 
			// This expression can be simplified with the boolean algebra to
			// => ((O IS NULL) AND (I IS NOT NULL)) OR ((O IS NOT NULL) AND (I IS NOT NULL) AND (O < I))
			// => (I IS NOT NULL) AND ((O IS NULL) OR ((O IS NOT NULL) AND (O < I)))
			// => (I IS NOT NULL) AND ((O IS NULL) OR (O < I))
			//
			// This expression always returns a boolean value because we can reduce it like that:
			// - If O is not NULL and I is not NULL
			//   => (I IS NOT NULL) AND ((O IS NULL) OR (O < I))
			//   => (V IS NOT NULL) AND ((V IS NULL) OR (V < V))
			//   => No NULL left.
			// - If O is NULL and I is not NULL
			//   => (I IS NOT NULL) AND ((O IS NULL) OR (O < I))
			//   => (V IS NOT NULL) AND ((NULL IS NULL) OR (NULL < V))
			//   => TRUE AND (TRUE OR NULL)
			//   => TRUE AND (TRUE)
			//   => No NULL left.
			// - If O is not NULL and I is NULL
			//   => (I IS NOT NULL) AND ((O IS NULL) OR (O < I))
			//   => (NULL IS NOT NULL) AND ((V IS NULL) OR (V < NULL))
			//   => FALSE AND (FALSE OR NULL)
			//   => FALSE AND NULL
			//   => FALSE
			//   => No NULL left.
			// - If O is NULL and I is NULL
			//   => (I IS NOT NULL) AND ((O IS NULL) OR (O < I))
			//   => (NULL IS NOT NULL) AND ((NULL IS NULL) OR (NULL < NULL))
			//   => FALSE AND (TRUE OR NULL)
			//   => FALSE AND TRUE
			//   => No NULL left.
			//
			// before (O, I) for DESC sort clauses:
			// 
			// This function is the exact opposite as before (O, I) for ASC sort clauses. Therefore
			// we simply have before-DESC (O, I) => before-ASC(I, O).
			//
			// An optimization might be to detect when the values are guaranteed to be not NULL. It
			// would be the case for a SQL column with a NOT NULL constraint that is not accessed
			// via a join on a collection. In this case, the functions could be defined as follow:
			// 
			// equals (O, I) => O = I
			// before-ASC (O, I) => O < I
			// before-DESC (O, I) => O > I

			// OK, now that we have looked at that this method builds, we can get in the
			// implementation details, as they are also not trivial.
			//
			// As you have seen, the expression is in fact a sequence of conditions that all look
			// like the previous one, with minor differences at the end. Therefore, we can reuse
			// part of an expression to build the next one.
			//
			// So the basic idea is to loop over all the sort clauses to build the condition,
			// accumulating the parts of the expression that we can reuse in the accumulator
			// variable, and storing the current expression in the condition variable. At each
			// iteration, we use this accumulator to build the next expression part, we append this
			// next expression part to the current condition, and we update the accumulator so that
			// we can use it at the next iteration.

			// Holds the current condition. At the end, that's our condition.
			SqlFunction condition = null;

			// Holds the accumulator that is used to build new parts of the condition. It will
			// contain expressions such as (S1O equal S1I AND ... AND SNO equal SNI).
			SqlFunction accumulator = null;

			foreach (var sortClause in request.SortClauses)
			{
				// We get the columns for the current sort clause.
				var fieldOuterColumn = sortClause.Field.CreateSqlField (outerBuilder);
				var innerColumn = sortClause.Field.CreateSqlField (innerBuilder);
				var innerColumnName = this.GetAliasForInnerQueryForIndexField (innerColumn);
				var fieldInnerColumn = SqlField.CreateName (innerQueryAlias, innerColumnName);

				// Now we build the two expressions that we will need later on.
				var before = sortClause.SortOrder == SortOrder.Ascending
					? this.CreateBeforeFunction (fieldOuterColumn, fieldInnerColumn)
					: this.CreateBeforeFunction (fieldInnerColumn, fieldOuterColumn);

				var equal = this.CreateEqualFunction (fieldOuterColumn, fieldInnerColumn);

				if (accumulator == null)
				{
					// This is the first iteration, so we initialize the condition and
					// accumulator variables.
					condition = before;
					accumulator = equal;
				}
				else
				{
					// We setup some stuff.
					var fieldCondition = SqlField.CreateFunction (condition);
					var fieldAccumulator = SqlField.CreateFunction (accumulator);
					var fieldBefore = SqlField.CreateFunction (before);
					var fieldEqual = SqlField.CreateFunction (equal);

					// Creates the local part of the condition, based on the previous accumulator
					// and the "before" condition.
					var localCondition = new SqlFunction (SqlFunctionCode.LogicAnd, fieldAccumulator, fieldBefore);
					var fieldLocalCondition = SqlField.CreateFunction (localCondition);

					// Includes the local part of the condition to the condition.
					condition = new SqlFunction (SqlFunctionCode.LogicOr, fieldCondition, fieldLocalCondition);

					// Includes the current "equal" expression to the accumulator.
					accumulator = new SqlFunction (SqlFunctionCode.LogicAnd, fieldAccumulator, fieldEqual);
				}
			}

			// Finally, we append the accumulator to the condition one last time.
			var lastFieldCondition = SqlField.CreateFunction (condition);
			var lastFieldAccumulator = SqlField.CreateFunction (accumulator);

			return new SqlFunction (SqlFunctionCode.LogicOr, lastFieldCondition, lastFieldAccumulator);
		}
		
		private SqlFunction CreateEqualFunction(SqlField a, SqlField b)
		{
			// (A = B)
			var areEqual = new SqlFunction (SqlFunctionCode.CompareEqual, a, b);
			var areEqualField = SqlField.CreateFunction (areEqual);

			// (A IS NULL)
			var aIsNull = new SqlFunction (SqlFunctionCode.CompareIsNull, a);
			var aIsNullField = SqlField.CreateFunction (aIsNull);

			// (B IS NULL)
			var bIsNull = new SqlFunction (SqlFunctionCode.CompareIsNull, b);
			var bIsNullField = SqlField.CreateFunction (bIsNull);

			// (A IS NULL) AND (B IS NULL)
			var bothNull = new SqlFunction (SqlFunctionCode.LogicAnd, aIsNullField, bIsNullField);
			var bothNullField = SqlField.CreateFunction (bothNull);

			// (A IS NOT NULL)
			var aIsNotNull = new SqlFunction (SqlFunctionCode.CompareIsNotNull, a);
			var aIsNotNullField = SqlField.CreateFunction (aIsNotNull);

			// (B IS NOT NULL)
			var bIsNotNull = new SqlFunction (SqlFunctionCode.CompareIsNotNull, b);
			var bIsNotNullField = SqlField.CreateFunction (bIsNotNull);

			// (A IS NOT NULL) AND (B IS NOT NULL)
			var bothNotNull = new SqlFunction (SqlFunctionCode.LogicAnd, aIsNotNullField, bIsNotNullField);
			var bothNotNullField = SqlField.CreateFunction (bothNotNull);

			// (A IS NOT NULL) AND (B IS NOT NULL) AND (A = B)
			var bothNotNullOrEqual = new SqlFunction (SqlFunctionCode.LogicAnd, bothNotNullField, areEqualField);
			var bothNotNullOrEqualField = SqlField.CreateFunction (bothNotNullOrEqual);

			// ((A IS NULL) AND (B IS NULL)) OR ((A IS NOT NULL) AND (B IS NOT NULL) AND (A = B))
			return new SqlFunction (SqlFunctionCode.LogicOr, bothNullField, bothNotNullOrEqualField);
		}
		
		private SqlFunction CreateBeforeFunction(SqlField a, SqlField b)
		{
			// (A < B)
			var aIsSmaller = new SqlFunction (SqlFunctionCode.CompareLessThan, a, b);
			var aIsSmallerField = SqlField.CreateFunction (aIsSmaller);

			// (A IS NULL)
			var aIsNull = new SqlFunction (SqlFunctionCode.CompareIsNull, a);
			var aIsNullField = SqlField.CreateFunction (aIsNull);

			// (A IS NULL) OR (A < B)
			var aIsNullOrSmaller = new SqlFunction (SqlFunctionCode.LogicOr, aIsNullField, aIsSmallerField);
			var aIsNullOrSmallerField = SqlField.CreateFunction (aIsNullOrSmaller);

			// (B IS NOT NULL)
			var bIsNotNull = new SqlFunction (SqlFunctionCode.CompareIsNotNull, b);
			var bIsNotNullField = SqlField.CreateFunction (bIsNotNull);

			// (B IS NOT NULL) AND ((A IS NULL) OR (A < B))
			return new SqlFunction (SqlFunctionCode.LogicAnd, bIsNotNullField, aIsNullOrSmallerField);
		}
		
		private string GetAliasForInnerQueryForIndexField(SqlField field)
		{
			return field.AsQualifier + "_" + field.AsName;
		}
		
		
		public IEnumerable<EntityData> GetEntitiesData(Request request)
		{
			//	Builds the SQL queries used to get the data of the entities matching the given
			//	request, executes them, and packs the result in a sequence of EntityData.
			//	
			//	We cannot make a single SQL query to get the data because of the collections. The
			//	problem with collections, is that a collection field of an entity can have several
			//	values, and would therefore require several rows to be returned. If we have several
			//	collections, for each entity, we would require the cartesian product of all their
			//	collection values to be returned, and that is a number that is exponential in the
			//	number of collections and items within the collections. Clearly, we do not want
			//	that.
			//	
			//	Therefore, we first make a query to return the value and the reference data of the
			//	entities. Then we make one query for each collection field to return their data
			//	separately. This way, we avoid the exponential explosion of redundant data.

			List<System.Tuple<DbKey, Druid, long, ValueData, ReferenceData>> valuesAndReferencesData;
			Dictionary<DbKey, CollectionData> collectionsData;

			using (var dbTransaction = this.StartTransaction ())
			{
				valuesAndReferencesData = this.GetValueAndReferenceData (dbTransaction, request);

				//	We don't bother to make the SQL queries for the collection data if we have no
				//	result for the value and the reference data.
				collectionsData = valuesAndReferencesData.Count > 0
					? this.GetCollectionData (dbTransaction, request, valuesAndReferencesData.Select (x => x.Item1))
					: new Dictionary<DbKey, CollectionData> ();

				dbTransaction.Commit ();
			}

			return this.GetEntitiesData (request, valuesAndReferencesData, collectionsData);
		}

		
		private Dictionary<DbKey, CollectionData> GetCollectionData(DbTransaction dbTransaction, Request request, IEnumerable<DbKey> keys)
		{
			//	We are working inside a transaction. The caller just retrieved the set of all
			//	entities, for which we want to fetch the collection data. We store the keys in
			//	the request, so that future requests can reuse the cached set rather than having
			//	to rely on a sub-SELECT.
			//
			//	Firebird 2.x performance is terrible on SELECT ... WHERE (x IN (SELECT ...)) as
			//	it seems not to use existing INDEX in that scenario. Using the cached set allows
			//	us to rewrite the request to SELECT ... WHERE (x IN (<set>)) which can be 100
			//	times faster than the sub-SELECT.

			var list = keys.ToList ();

			if (list.Count < 200)
			{
				request.CacheSourceSet (list);
			}

			return this.GetCollectionData (dbTransaction, request);
		}

		private List<System.Tuple<DbKey, Druid, long, ValueData, ReferenceData>> GetValueAndReferenceData(DbTransaction transaction, Request request)
		{
			var sqlSelect = this.BuildSelectForValueAndReferenceData (request);

			transaction.SqlBuilder.SelectData (sqlSelect);
			var data = this.DbInfrastructure.ExecuteRetData (transaction);

			// In order to process the data to put in ValueData and ReferenceData instances, we
			// must know the order in which the columns are provided in the result. That's what we
			// put in the valueFields and in the referenceFields variables.

			var leafEntityTypeId = request.RequestedEntity.GetEntityStructuredTypeId ();

			var valueFields = new List<System.Tuple<StructuredTypeField, DbColumn>>
			(
				from field in this.GetValueFields (leafEntityTypeId)
				let fieldId = field.CaptionId
				let localEntityTypeId = this.TypeEngine.GetLocalType (leafEntityTypeId, fieldId).CaptionId
				let dbColumn = this.SchemaEngine.GetEntityFieldColumn (localEntityTypeId, fieldId)
				select System.Tuple.Create (field, dbColumn)
			);

			var referenceFields = this.GetReferenceFields (leafEntityTypeId).ToList ();

			return data.Tables[0].Rows
				.Cast<DataRow> ()
				.Select (r => this.ProcessValueAndReferenceRow (valueFields, referenceFields, r))
				.ToList ();
		}

		private System.Tuple<DbKey, Druid, long, ValueData, ReferenceData> ProcessValueAndReferenceRow(List<System.Tuple<StructuredTypeField, DbColumn>> valueFields, List<StructuredTypeField> referenceFields, DataRow row)
		{
			// The columns for the values fields are the first one in the data table.

			var entityValueData = new ValueData ();

			for (int i = 0; i < valueFields.Count; i++)
			{
				var databaseValue = row[i];
				var field = valueFields[i].Item1;
				var dbColumn = valueFields[i].Item2;

				var cresusValue = databaseValue == System.DBNull.Value
					? null
					: this.ExtractValue (field.Type, dbColumn, databaseValue);

				entityValueData[field.CaptionId] = cresusValue;
			}

			// Then we have the columns for the reference fields.

			var entityReferenceData = new ReferenceData ();

			for (int i = 0; i < referenceFields.Count; i++)
			{
				var value = row[valueFields.Count + i];

				if (value != System.DBNull.Value)
				{
					var key = this.ExtractKey (value);
					var fieldId = referenceFields[i].CaptionId;

					entityReferenceData[fieldId] = key;
				}
			}

			// Finally, we have three more columns with additional metadata about the entity.

			// Here, between the columns for the reference fields and the columns for the
			// metadata, there might be other columns. They are the columns used for the
			// significant fields for the DISTINCT clause. We don't want them in the result, so we
			// simply ignore them. But that means that we want to index the additional metadata
			// columns starting from the end of the column array, and not by using the sum of
			// the value and reference columns. That's why we have rowLength - 3, rowLength - 2 and
			// rowLength - 1.
			var rowLength = row.ItemArray.Length;

			var logId        = this.ExtractLong (row[rowLength - 3]);		//	CR_EM_ID -- entity modification entry ID
			var entityTypeId = this.ExtractDruid (row[rowLength - 2]);		//	CR_TYPE_ID
			var rowKey       = this.ExtractKey (row[rowLength - 1]);		//	CR_ID

			return System.Tuple.Create (rowKey, entityTypeId, logId, entityValueData, entityReferenceData);
		}

		private SqlSelect BuildSelectForValueAndReferenceData(Request request)
		{
			// The SQL query that we want to build looks like this:
			// 
			// SELECT e.v1, e.v2, ..., e.vn, e.r1, e.r2, ..., e.rn, e.log_id, e.type_id e.cr_id
			// FROM [e]
			// WHERE ...
			// ORDER BY ...
			// ROWS ... TO ...
			// 
			// That is, we use the standard query parts, and we add the SELECT clause that returns
			// the values of the columns for the value properties, the reference properties and
			// additional fields such as the id of the entity, the modification log id of the
			// entity and the type id of the entity.
			//
			// We might add a DISTINCT clause if necessary to ensure that there is no duplicate in
			// the resulting set.

			var builder = this.GetBuilder ();

			var fromWhereOrderBy = this.BuildFromWhereAndOrderBy (builder, request);
			var select = this.BuildSelectForValuesAndReferences (builder, request);
			var predicate = this.GetSqlSelectPredicate (request);

			return fromWhereOrderBy
				.PlusSqlFields (select.ToArray ())
				.BuildSqlSelect (predicate, request.Skip, request.Take);
		}

		private IEnumerable<SqlField> BuildSelectForValuesAndReferences(SqlFieldBuilder builder, Request request)
		{
			var entity = request.RequestedEntity;
			var leafEntityTypeId = entity.GetEntityStructuredTypeId ();

			var valueFields = this.GetValueFields (leafEntityTypeId);
			var referenceFields = this.GetReferenceFields (leafEntityTypeId);

			foreach (var field in valueFields.Concat (referenceFields))
			{
				var fieldId = field.CaptionId;

				yield return builder.BuildEntityField (entity, fieldId);
			}

			// Also returns the significant fields of the SQL query, so that they will be taken 
			// into account for the DISTINCT clause.

			foreach (var field in request.SignificantFields)
			{
				yield return field.CreateSqlField (builder);
			}

			yield return builder.BuildRootLogId (entity);
			yield return builder.BuildRootTypeId (entity);
			yield return builder.BuildRootId (entity);
		}

		private IEnumerable<StructuredTypeField> GetValueFields(Druid leafEntityTypeId)
		{
			return from field in this.TypeEngine.GetValueFields (leafEntityTypeId)
				   where !field.Options.HasFlag (FieldOptions.DisablePrefetch)
				   let fieldId = field.CaptionId
				   let fieldName = fieldId.ToResourceId ()
				   orderby fieldName
				   select field;
		}

		private IEnumerable<StructuredTypeField> GetReferenceFields(Druid leafEntityTypeId)
		{
			return from field in this.TypeEngine.GetReferenceFields (leafEntityTypeId)
				   let fieldId = field.CaptionId
				   let fieldName = fieldId.ToResourceId ()
				   orderby fieldName
				   select field;
		}

		private Dictionary<DbKey, CollectionData> GetCollectionData(DbTransaction transaction, Request request)
		{
			// Builds the SQL queries used to get the collection data of the entities, executes
			// them and packs them into instances of CollectionData.
			//
			// We can't do this in a single request, for the reason explained in the comment of the
			// GetEntitiesData() method.

			var collectionData = new Dictionary<DbKey, CollectionData> ();

			var entity = request.RequestedEntity;
			var leafEntityTypeId = entity.GetEntityStructuredTypeId ();

			var fields = this.TypeEngine.GetCollectionFields (leafEntityTypeId);

			foreach (var field in fields)
			{
				var fieldId = field.CaptionId;

				foreach (var relation in this.GetCollectionData (transaction, request, fieldId))
				{
					CollectionData data;

					var sourceKey = relation.Item1;
					var targetKey = relation.Item2;

					if (!collectionData.TryGetValue (sourceKey, out data))
					{
						data = new CollectionData ();

						collectionData[sourceKey] = data;
					}

					data[fieldId].Add (targetKey);
				}
			}

			return collectionData;
		}

		private IEnumerable<System.Tuple<DbKey, DbKey>> GetCollectionData(DbTransaction transaction, Request request, Druid fieldId)
		{
			var select = this.BuildSelectForCollectionData (request, fieldId);

			transaction.SqlBuilder.SelectData (select);
			var data = this.DbInfrastructure.ExecuteRetData (transaction);

			return from DataRow row in data.Tables[0].Rows
				   let targetKey = this.ExtractKey (row[0])
				   let sourceKey = this.ExtractKey (row[1])
				   select System.Tuple.Create (sourceKey, targetKey);
		}

		private SqlSelect BuildSelectForCollectionData(Request request, Druid fieldId)
		{
			//	The SQL query that we want to build looks like this:
			//	
			//	SELECT r.target_id, r.source_id
			//	FROM [r]
			//	WHERE r.source_id IN SELECT e.cr_id
			//	                     FROM [e]
			//	                     WHERE ...
			//	                     ORDER BY ...
			//	                     ROWS ... TO ...
			//	ORDER BY r.rank
			//	
			//	Basically, we have the inner query that returns the id of all the entities whose
			//	collection data we want to have. Then we use the outer query to return only the
			//	collection data of these entities.

			var builder   = this.GetBuilder ();
			var cachedSet = request.CachedSourceSet;

			if (cachedSet == null)
			{
				var innerSelect = this.BuildInnerSelectForCollectionData (request, builder);
				return this.BuildOuterSelectForCollectionData (builder, request, fieldId, innerSelect);
			}
			else
			{
				//	We don't need to build the sub-select for the WHERE ... IN clause, as we have
				//	the full set at hand; just use the indices from the cached set in that case.

				return this.BuildOuterSelectForCollectionData (builder, request, fieldId, cachedSet);
			}
		}

		private SqlField BuildInnerSelectForCollectionData(Request request, SqlFieldBuilder builder)
		{
			// This SQL query looks like this:
			//
			// SELECT e.cr_id FROM [e] WHERE ... ORDER BY ... ROWS ... TO ...
			//
			// It is very similar to the one that returns the value and the reference  data of the
			// entities. The only difference is that here we don't return these data but only the
			// id of the entities, so we can feed these ids in the outer query.
			//
			// Be aware that the ORDER BY clause is necessary. Often I think that it is not
			// required, but it is. If we don't include it, we might return the collection data
			// of other entities than the entities returned by the query that gets the value and
			// reference data, because of the ROWS ... TO ... clause that might filter some
			// entities out of the result set. It is thus required to obtain the ids of the exact
			// same entities as the query that gets their value and reference data.
			//
			// We might add a DISTINCT clause to make sure that we don't have duplicates in the
			// result.

			var fromWhereOrderBy = this.BuildFromWhereAndOrderBy (builder, request);
			var select = builder.BuildRootId (request.RequestedEntity);
			var predicate = this.GetSqlSelectPredicate (request);

			var sqlSelect = fromWhereOrderBy
				.PlusSqlFields (select)
				.BuildSqlSelect (predicate, request.Skip, request.Take);

			return SqlField.CreateSubQuery (sqlSelect);
		}

		private SqlSelect BuildOuterSelectForCollectionData(SqlFieldBuilder builder, Request request, Druid fieldId, SqlField innerSelect)
		{
			// This SQL query looks like this:
			// 
			// SELECT r.target_id, r.source_id
			// FROM [r]
			// WHERE r.source_id IN [inner query]
			// ORDER BY r.rank
			//
			// No big deal here, we simply return the data of the joining table, ordered by rank
			// so that we have the elements already in the proper order to put them in the
			// entity collections. This also ensure that we don't mind if there are holes in the
			// rank, the elements will automatically be packed at the start of the collection.

			var entity = request.RequestedEntity;
			var leafEntityId = entity.GetEntityStructuredTypeId ();
			var localEntityId = this.TypeEngine.GetLocalType (leafEntityId, fieldId).CaptionId;

			var tableAlias = builder.AliasManager.GetAlias ();
			var dbTable = this.SchemaEngine.GetEntityFieldTable (localEntityId, fieldId);

			var table = builder.BuildTable (tableAlias, dbTable);

			var targetId = builder.BuildRelationTargetId (tableAlias, localEntityId, fieldId);
			var sourceId = builder.BuildRelationSourceId (tableAlias, localEntityId, fieldId);

			var condition = new SqlFunction (SqlFunctionCode.SetIn, sourceId, innerSelect);

			var rank = builder.BuildRelationRank (tableAlias, localEntityId, fieldId);
			rank.SortOrder = SqlSortOrder.Ascending;

			return SqlContainer.CreateSqlTables (table)
				.PlusSqlFields (targetId, sourceId)
				.PlusSqlConditions (condition)
				.PlusSqlOrderBys (rank)
				.BuildSqlSelect ();
		}

		private IEnumerable<EntityData> GetEntitiesData(Request request, IEnumerable<System.Tuple<DbKey, Druid, long, ValueData, ReferenceData>> valuesAndReferencesData, Dictionary<DbKey, CollectionData> collectionsData)
		{
			return from valueAndReferenceData in valuesAndReferencesData
				   let loadedTypeId = request.RequestedEntity.GetEntityStructuredTypeId ()
				   let id = valueAndReferenceData.Item1
				   let leafTypeId = valueAndReferenceData.Item2
				   let logId = valueAndReferenceData.Item3
				   let values = valueAndReferenceData.Item4
				   let references = valueAndReferenceData.Item5
				   let collections = collectionsData.ContainsKey (id)
					? collectionsData[id]
					: new CollectionData ()
				   select new EntityData (id, leafTypeId, loadedTypeId, logId, values, references, collections);
		}


		public object GetValueField(AbstractEntity entity, Druid fieldId)
		{
			// This methods gets the value of a single value field of an entity. It is used by the
			// proxies to fill missing values.
			//
			// It works by making a regular Request object that will contain the condition that
			// will leave only the entity whose value we want in the resulting set.

			var request = this.BuildRequestForValueField (entity, fieldId);

			object value;

			using (var transaction = this.StartTransaction ())
			{
				value = this.GetValueField (transaction, request, fieldId);

				transaction.Commit ();
			}

			return value;
		}

		private Request BuildRequestForValueField(AbstractEntity entity, Druid fieldId)
		{
			var leafEntityTypeId = entity.GetEntityStructuredTypeId ();
			var localEntityTypeId = this.TypeEngine.GetLocalType (leafEntityTypeId, fieldId).CaptionId;

			var example = EntityClassFactory.CreateEmptyEntity (localEntityTypeId);
			var key = this.dataContext.GetNormalizedEntityKey (entity).Value.RowKey;

			return Request.Create (example, key);
		}

		private object GetValueField(DbTransaction transaction, Request request, Druid fieldId)
		{
			var select = this.BuildSelectForSingleValue (request, fieldId);

			transaction.SqlBuilder.SelectData (select);
			var data = this.DbInfrastructure.ExecuteRetData (transaction);

			var table = data.Tables[0];

			object value = null;

			if (table.Rows.Count == 1)
			{
				var databaseValue = table.Rows[0][0];

				if (databaseValue != System.DBNull.Value)
				{
					var leafEntityTypeId = request.RequestedEntity.GetEntityStructuredTypeId ();

					var field = this.TypeEngine.GetField (leafEntityTypeId, fieldId);
					var type = field.Type;

					var localEntityTypeId = this.TypeEngine.GetLocalType (leafEntityTypeId, fieldId).CaptionId;
					var dbColumn = this.SchemaEngine.GetEntityFieldColumn (localEntityTypeId, fieldId);

					value = this.ExtractValue (type, dbColumn, databaseValue);
				}
			}

			return value;
		}

		private SqlSelect BuildSelectForSingleValue(Request request, Druid fieldId)
		{
			// The SQL query that we want to build looks like this:
			// 
			// SELECT e.v, FROM [e] WHERE ...
			//
			// Basically, this is the regular SQL query used to get entity value and reference
			// data. Except that we don't bother to include the ORDER BY and the ROWS ... TO
			// clauses as they are useless because we know that there will be at most one row in
			// the result set.
			//
			// This row in the result set contain a single column, the one whose value we want.

			var builder = this.GetBuilder ();

			var fromWhereOrderBy = this.BuildFromAndWhere (builder, request);
			var select = builder.BuildEntityField (request.RequestedEntity, fieldId);

			return fromWhereOrderBy
				.PlusSqlFields (select)
				.BuildSqlSelect ();
		}


		public EntityData GetReferenceField(AbstractEntity entity, Druid fieldId)
		{
			// This methods gets all the data of an entity which is the target of a  reference
			// field of another entity. This is usefull for the proxies.
			//
			// Note that we don't simply want the entity key of the target, we want all of its
			// data so we can directly instantiate this entity. Therefore, we simply forge a
			// Request object that we get this data and give it to the GetEntitiesData() method.
			
			var request = this.GetRequestForReferenceField (entity, fieldId);

			return this.GetEntitiesData (request).FirstOrDefault ();
		}


		private Request GetRequestForReferenceField(AbstractEntity entity, Druid fieldId)
		{
			var leafEntityTypeId = entity.GetEntityStructuredTypeId ();
			var field = this.TypeEngine.GetField (leafEntityTypeId, fieldId);

			var source = EntityClassFactory.CreateEmptyEntity (leafEntityTypeId);
			var sourceKey = this.dataContext.GetNormalizedEntityKey (entity).Value.RowKey;

			var target = EntityClassFactory.CreateEmptyEntity (field.TypeId);

			using (source.DefineOriginalValues ())
			{
				var fieldName = fieldId.ToResourceId ();

				source.SetField (fieldName, target);
			}

			return Request.Create (source, sourceKey, target);
		}


		public IEnumerable<EntityData> GetCollectionField(AbstractEntity entity, Druid fieldId)
		{
			// This method gets all the data of the entities which are the target of a collection
			// field of another entity. This is usefull for the proxies.
			//
			// We don't simply want the entity keys of the targets, we want all their data so we
			// can directly create their instance. Therefore we simply forge a Request object that
			// will get this data and give it to the GetEntitiesData() method.

			// NOTE This query will return duplicate entries for duplicates in the collection. This
			// is important in order to have the collection in memory to match the data in the
			// database. Those duplicates will be mapped to the same entity in the DataLoader.

			var request = this.GetRequestForCollectionField (entity, fieldId);

			return this.GetEntitiesData (request);
		}


		private Request GetRequestForCollectionField(AbstractEntity entity, Druid fieldId)
		{
			var leafEntityTypeId = entity.GetEntityStructuredTypeId ();
			var field = this.TypeEngine.GetField (leafEntityTypeId, fieldId);

			var source = EntityClassFactory.CreateEmptyEntity (leafEntityTypeId);
			var sourceKey = this.dataContext.GetNormalizedEntityKey (entity).Value.RowKey;

			var target = EntityClassFactory.CreateEmptyEntity (field.TypeId);

			using (source.DefineOriginalValues ())
			{
				var fieldName = fieldId.ToResourceId ();
				var collection = source.GetFieldCollection<AbstractEntity> (fieldName);

				collection.Add (target);
			}

			var request = Request.Create (source, sourceKey, target);

			var rankField = CollectionField.CreateRank (source, fieldId, target);

			// We sort the entities by the rank, so they are in the proper order to be added to the
			// collection.
			request.SortClauses.Add (new SortClause (rankField, SortOrder.Ascending));

			// We add the rank as a significant field, otherwhise, if we have a DISTINCT clause,
			// it might filter out duplicate pairs of source and target entity keys. And we want to
			// keep those duplicate as they represent duplicate entries in the collection. This
			// will return redundant data, but it is still more performant to perform a single
			// request that returns the entities in the right order with their entity data, than to
			// execute one query to get the ids of the entities in the right order, and the one
			// query to get their entity data.
			request.SignificantFields.Add (rankField);

			return request;
		}

		private SqlContainer BuildFromWhereAndOrderBy(SqlFieldBuilder builder, Request request)
		{
			// This method builds the standard parts of SQL queries used by most of the SQL queries
			// in this class:
			//
			// FROM ... WHERE ... ORDER BY ...
			//
			// These parts are more detailed in the two methods called by this one.

			var fromAndWhere = this.BuildFromAndWhere (builder, request);
			var orderBy = this.BuildOrderBy (builder, request);

			return fromAndWhere.PlusSqlOrderBys (orderBy.ToArray ());
		}

		private SqlContainer BuildFromAndWhere(SqlFieldBuilder builder, Request request)
		{
			// This method builds the standard parts of SQL queries used by most of the SQL queries
			// in this class:
			//
			// FROM ... WHERE ...
			//
			// These parts are more detailed in the two methods called by this one.

			// Basically, we have an example which is a graph of entities. The persistent and
			// non-persistent entities have a different meaning in this graph. The persistent
			// entities are considered as leaves and we only use their entity key as condition in
			// the query. The non-persistent entities are considered by value and we use all the
			// values of their fields to create the condition.
			var nonPersistentEntities = request.GetNonPersistentEntities (this.dataContext);

			var from = this.BuildFrom (builder, request, nonPersistentEntities);
			var where = this.BuildWhere (builder, request, nonPersistentEntities);

			return from.PlusSqlConditions (where.ToArray ());
		}

		private SqlContainer BuildFrom(SqlFieldBuilder builder, Request request, ICollection<AbstractEntity> nonPersistentEntities)
		{
			// This method builds the FROM part of the the standard SQL query used by this class,
			// that means it works out which tables will be used by the query and the joins that
			// must be made between them.
			//
			// It works in two phases:
			// - Find out what tables are used, and what joins are between them
			// - Pack them in an SqlContainer instance with the right order.
			//
			// You should go read the comments in these two methods if you want more details. But
			// the point is that we have these two phases separeted because it would be too complex
			// to do them at the same time, because the constraints on the ordering of the joins
			// are quite messy.

			// This dictionary will contain the mapping from the table aliases to the SqlField that
			// reprenset the table.
			var tables = new Dictionary<string, SqlField> ();

			// This list will contain all the joins. In this list, a join is (in the order of the
			// items of the tuple):
			// - The alias of the first table
			// - The SqlField that represents the column of the first table used in the join
			// - The alias of the second table
			// - The SqlField that represents the column of the second table used in the join
			// - A boolean indicating whether this is an INNER JOIN (true) or LEFT OUTER JOIN
			//   (false)
			var joins = new List<System.Tuple<string, SqlField, string, SqlField, bool>> ();

			// This populates the two variables above.
			this.BuildTablesAndJoins (builder, request, nonPersistentEntities, tables, joins);

			// And this packs the results in an SqlContainer.
			return this.BuildFromClause (tables, joins);
		}

		private void BuildTablesAndJoins(SqlFieldBuilder builder, Request request, ICollection<AbstractEntity> nonPersistentEntities, Dictionary<string, SqlField> tables, List<System.Tuple<string, SqlField, string, SqlField, bool>> joins)
		{
			// This methods finds out what tables are to be used in a standard SQL query and what
			// kind of join exist between them.
			//
			// Basically, there are two kinds of relations between all the tables used in an SQL
			// query used to return entities:
			// - The relation between two tables that represents the values of the same entity, via
			//   inheritance. If we have NaturalPerson that derives from AbstractPerson, we have
			//   two tables and we must have a join between them, if we have a query that deals
			//   with NaturalPersons.
			// - The relation between two tables that represent different entity, via a reference
			//   or a collection. If we have Person that have a collection of Contacts or a
			//   reference to a Company, we must have joins between the table of the source entity
			//   and the table of the target entity (potentially through a join table, if we have a
			//   collection).
			//
			// We build these tables and joins in two separate passes, one for each kind of
			// relation.

			// We store in this dictionnary the mapping from entities to their sources. That way,
			// we can quickly and easily find all the entities that reference another.
			var targetsWithsources = this.GetTargetsWithSources (nonPersistentEntities);

			// It might happen that an entity is not used at all in the conditions, but only in the
			// sort clauses, to determine the order in which the entities are returned. In this
			// case, we don't want to use an INNER JOIN to join to this entity, because it will
			// filter all entities where the relation is not defined out of the result set.
			// Therefore, we must use a LEFT OUTER JOIN to join all entities that are referenced
			// only in the sort clauses and not in the conditions.
			//
			// For example, imagine that we have persons which can have a contact, and the contacts
			// contain an email field. If we want to return all the persons, ordered by their email
			// we want to return all persons, even the ones that don't have a contact.
			//
			// So we store all entities that are mandatory in this set. The mandatory entities will
			// be joined by an INNER JOIN clause and the entities that are not mandatory will be
			// joined by a LEFT OUTER JOIN clause.
			var mandatoryEntities = this.GetMandatoryEntities (request, targetsWithsources, nonPersistentEntities);

			this.BuildTablesAndJoinsForEntities (builder, nonPersistentEntities, mandatoryEntities, tables, joins);
			this.BuildTablesAndJoinsForRelations (builder, mandatoryEntities, targetsWithsources, tables, joins);
		}

		private Dictionary<AbstractEntity, HashSet<System.Tuple<AbstractEntity, Druid>>> GetTargetsWithSources(IEnumerable<AbstractEntity> entities)
		{
			var targetsWithsources = new Dictionary<AbstractEntity, HashSet<System.Tuple<AbstractEntity, Druid>>> ();

			foreach (var source in entities)
			{
				var fieldsWithChildren = EntityHelper.GetFieldsWithChildren (this.TypeEngine, source);

				foreach (var fieldWithTarget in fieldsWithChildren)
				{
					var fieldId = fieldWithTarget.Item1;
					var target = fieldWithTarget.Item2;

					HashSet<System.Tuple<AbstractEntity, Druid>> parents;

					if (!targetsWithsources.TryGetValue (target, out parents))
					{
						parents = new HashSet<System.Tuple<AbstractEntity, Druid>> ();

						targetsWithsources[target] = parents;
					}

					var element = System.Tuple.Create (source, fieldId);

					parents.Add (element);
				}
			}

			return targetsWithsources;
		}

		private HashSet<AbstractEntity> GetMandatoryEntities(Request request, Dictionary<AbstractEntity, HashSet<System.Tuple<AbstractEntity, Druid>>> targetsWithsources, IEnumerable<AbstractEntity> nonPersistentEntities)
		{
			// As said in the BuildTablesAndJoins(...) method, we have two kinds of entities, the
			// non-mandatory (those that are referenced only in a sort clause) and the mandatory
			// (all the others). This methods find all entities which are mandatory, and by
			// deduction we know that those which are not in the list are non-mandatory.

			var todo = new HashSet<AbstractEntity> (nonPersistentEntities);
			var mandatoryEntities = new HashSet<AbstractEntity> ();

			// Obviously, the root entity of the example is mandatory.
			var rootEntity = request.RootEntity;

			todo.Remove (rootEntity);
			mandatoryEntities.Add (rootEntity);

			// And the requested entity of the example is mandatory as well.
			var requestedEntity = request.RequestedEntity;

			if (requestedEntity != rootEntity)
			{
				todo.Remove (requestedEntity);
				mandatoryEntities.Add (requestedEntity);
			}

			// The entities which are used in a condition clause are mandatory.
			var entitiesWithinConditions = request
				.Conditions
				.SelectMany (c => c.GetEntities ())
				.Distinct ()
				.ToList ();

			todo.ExceptWith (entitiesWithinConditions);
			mandatoryEntities.UnionWith (entitiesWithinConditions);

			// The entities which have a value field defined are mandatory (as it will be used to
			// generate a condition).
			var entitiesWithValueFieldsDefined = todo
				.Where (e => EntityHelper.HasValueFieldDefined (this.TypeEngine, e))
				.ToList ();

			todo.ExceptWith (entitiesWithValueFieldsDefined);
			mandatoryEntities.UnionWith (entitiesWithValueFieldsDefined);

			// The entities which have a relation to a persistent target are mandatory (as this
			// relation will be used to generate a condition).
			var entitiesWithRelationToPersistentTarget = todo
				.Where (e => EntityHelper.HasRelationToPersistentTarget (this.TypeEngine, this.dataContext, e))
				.ToList ();

			todo.ExceptWith (entitiesWithRelationToPersistentTarget);
			mandatoryEntities.UnionWith (entitiesWithRelationToPersistentTarget);

			// Now we have a list of all the entities that are explicitely mandatory. But every
			// entity that is on the path from the root entity to a mandatory entity is also
			// mandatory.
			//
			// Imagine if we have persons, persons have a relation to their parents and to a job
			// entity. Imagine that we have a request that looks for all the persons whose parent
			// is a computer scientist. We have three entities in the request: the person, the
			// parent and the job. At this point, the person and the job would have been determined
			// to be mandatory, but not the parent which is in between. But it is mandatory because
			// it is on the path from the person to the job.
			//
			// So what we do here is that we add to the list of mandatory entities all the entities
			// that have a relation to a mandatory entity. And we do this iteratively in a loop to
			// mark as mandatory all the entities that have a referenced to a newly found mandatory
			// entity, until we don't find any new mandatory entity.

			// Here we store the entities that were discovered to be mandatory in the last pass, so
			// that we look for entities that reference them.
			var oldMandatory = new HashSet<AbstractEntity> (mandatoryEntities);

			// Here we store the entities that are discovered to be mandatory in the current pass.
			// If there is at least one, we should make one more pass, and if there is none, we can
			// stop.
			HashSet<AbstractEntity> newMandatory;

			do
			{
				newMandatory = new HashSet<AbstractEntity> ();

				// Adds all the entities that target an entity that was found to be mandatory in
				// the last pass to the newMandatory variable.
				foreach (var entity in oldMandatory)
				{
					HashSet<System.Tuple<AbstractEntity, Druid>> sources;

					if (targetsWithsources.TryGetValue (entity, out sources))
					{
						foreach (var sourceData in sources)
						{
							var source = sourceData.Item1;

							if (!mandatoryEntities.Contains (source))
							{
								newMandatory.Add (source);
							}
						}
					}
				}

				// Adds these entities to the set of mandatory entities.
				mandatoryEntities.UnionWith (newMandatory);

				// And use these entities as the basis for the next pass.
				oldMandatory = newMandatory;
			}
			while (newMandatory.Count > 0);

			return mandatoryEntities;
		}

		private void BuildTablesAndJoinsForEntities(SqlFieldBuilder builder, IEnumerable<AbstractEntity> entities, HashSet<AbstractEntity> mandatoryEntities, Dictionary<string, SqlField> tables, List<System.Tuple<string, SqlField, string, SqlField, bool>> joins)
		{
			foreach (var entity in entities)
			{
				var isMandatory = this.IsMandatory (entity, mandatoryEntities);

				this.BuildTablesAndJoinsForEntity (builder, entity, isMandatory, tables, joins);
			}
		}

		private bool IsMandatory(AbstractEntity entity, HashSet<AbstractEntity> mandatoryEntities)
		{
			return mandatoryEntities.Contains (entity);
		}

		private void BuildTablesAndJoinsForEntity(SqlFieldBuilder builder, AbstractEntity entity, bool isMandatory, Dictionary<string, SqlField> tables, List<System.Tuple<string, SqlField, string, SqlField, bool>> joins)
		{
			// This methods builds all tables that are used for a single entity (one for each type
			// of the entity) and the joins between those tables.
			// If we have NaturalPerson that derives from AbstractPerson, we'll have two tables and
			// a join between them.

			var leafEntityTypeId = entity.GetEntityStructuredTypeId ();
			var rootEntityTypeId = this.TypeEngine.GetRootType (leafEntityTypeId).CaptionId;

			var rootTableAlias = builder.AliasManager.GetAlias (entity, rootEntityTypeId);
			var rootColumnId = builder.BuildRootId (entity);

			var entityTypes = this.TypeEngine.GetBaseTypes (leafEntityTypeId);

			foreach (var entityType in entityTypes)
			{
				var localEntityTypeId = entityType.CaptionId;

				var localTableAlias = builder.AliasManager.GetAlias (entity, localEntityTypeId);
				var localTable = builder.BuildEntityTable (entity, localEntityTypeId);

				tables[localTableAlias] = localTable;

				if (localEntityTypeId != rootEntityTypeId)
				{
					var localColumnId = builder.BuildEntityId (entity, localEntityTypeId);

					var join = System.Tuple.Create (rootTableAlias, rootColumnId, localTableAlias, localColumnId, isMandatory);
					joins.Add (join);
				}
			}
		}

		private void BuildTablesAndJoinsForRelations(SqlFieldBuilder builder, HashSet<AbstractEntity> mandatoryEntities, Dictionary<AbstractEntity, HashSet<System.Tuple<AbstractEntity, Druid>>> targetsWithsources, Dictionary<string, SqlField> tables, List<System.Tuple<string, SqlField, string, SqlField, bool>> joins)
		{
			foreach (var targetWithsourcesItem in targetsWithsources)
			{
				var target = targetWithsourcesItem.Key;

				foreach (var sourceItem in targetWithsourcesItem.Value)
				{
					var source = sourceItem.Item1;
					var fieldId = sourceItem.Item2;

					this.BuildTablesAndJoinsForRelations (builder, mandatoryEntities, source, fieldId, target, tables, joins);
				}
			}
		}

		private void BuildTablesAndJoinsForRelations(SqlFieldBuilder builder, HashSet<AbstractEntity> mandatoryEntities, AbstractEntity source, Druid fieldId, AbstractEntity target, Dictionary<string, SqlField> tables, List<System.Tuple<string, SqlField, string, SqlField, bool>> joins)
		{
			var leafEntityTypeId = source.GetEntityStructuredTypeId ();
			var field = this.TypeEngine.GetField (leafEntityTypeId, fieldId);

			var isMandatory = this.IsMandatory (target, mandatoryEntities);

			switch (field.Relation)
			{
				case FieldRelation.Reference:
					this.BuildTablesAndJoinsForReference (builder, source, fieldId, target, isMandatory, joins);
					break;

				case FieldRelation.Collection:
					this.BuildTablesAndJoinsForCollection (builder, source, fieldId, target, isMandatory, tables, joins);
					break;

				default:
					throw new System.InvalidOperationException ();
			}
		}

		private void BuildTablesAndJoinsForReference(SqlFieldBuilder builder, AbstractEntity source, Druid fieldId, AbstractEntity target, bool isMandatory, List<System.Tuple<string, SqlField, string, SqlField, bool>> joins)
		{
			// If an entity has a reference to another, we must create a join between the two
			// tables. Both tables have already been created in the BuildTablesAndJoinsForEntity(...)
			// method.
			//
			// We only make the join if the target is not persistent. If it is persistent, we don't
			// need a join as we'll add later on a condition on the value of the reference column
			// in the source entity.
			
			if (!this.dataContext.IsPersistent (target))
			{
				var leafSourceTypeId = source.GetEntityStructuredTypeId ();
				var localSourceTypeId = this.TypeEngine.GetLocalType (leafSourceTypeId, fieldId).CaptionId;

				var sourceTableAlias = builder.AliasManager.GetAlias (source, localSourceTypeId);
				var sourceColumn = builder.BuildEntityField (source, fieldId);

				var leafTargetTypeId = target.GetEntityStructuredTypeId ();
				var rootTargetTypeId = this.TypeEngine.GetRootType (leafTargetTypeId).CaptionId;

				var targetTableAlias = builder.AliasManager.GetAlias (target, rootTargetTypeId);
				var targetColumn = builder.BuildRootId (target);

				var join = System.Tuple.Create (sourceTableAlias, sourceColumn, targetTableAlias, targetColumn, isMandatory);

				joins.Add (join);
			}
		}

		private void BuildTablesAndJoinsForCollection(SqlFieldBuilder builder, AbstractEntity source, Druid fieldId, AbstractEntity target, bool isMandatory, Dictionary<string, SqlField> tables, List<System.Tuple<string, SqlField, string, SqlField, bool>> joins)
		{
			// If an entity has a relation to another entity through a relation, we must add a
			// join between the source and the relation table, and another join between the
			// relation table and the target entity.
			//
			// As the relation table has not already been added to the list of tables, we do that
			// here, before creating the two joins.

			var leafSourceTypeId = source.GetEntityStructuredTypeId ();
			var rootSourceTypeId = this.TypeEngine.GetRootType (leafSourceTypeId).CaptionId;
			var localSourceTypeId = this.TypeEngine.GetLocalType (leafSourceTypeId, fieldId).CaptionId;

			var relationTableAlias = builder.AliasManager.GetAlias (source, fieldId, target);

			var relationTable = builder.BuildRelationTable (source, fieldId, target);
			tables[relationTableAlias] = relationTable;

			var sourceTableAlias = builder.AliasManager.GetAlias (source, rootSourceTypeId);
			var sourceColumnId = builder.BuildRootId (source);

			var relationColumnSourceId = builder.BuildRelationSourceId (relationTableAlias, localSourceTypeId, fieldId);

			var joinToRelation = System.Tuple.Create (sourceTableAlias, sourceColumnId, relationTableAlias, relationColumnSourceId, isMandatory);
			joins.Add (joinToRelation);

			// If the target entity is persistent, we don't join with its table, but later on we'll
			// add a condition on the source id column in the relation table.

			if (!this.dataContext.IsPersistent (target))
			{
				var leafTargetTypeId = target.GetEntityStructuredTypeId ();
				var rootTargetTypeId = this.TypeEngine.GetRootType (leafTargetTypeId).CaptionId;

				var relationColumnTargetId = builder.BuildRelationTargetId (relationTableAlias, localSourceTypeId, fieldId);

				var targetTableAlias = builder.AliasManager.GetAlias (target, rootTargetTypeId);
				var targetColumn = builder.BuildRootId (target);

				var joinToTarget = System.Tuple.Create (relationTableAlias, relationColumnTargetId, targetTableAlias, targetColumn, isMandatory);
				joins.Add (joinToTarget);
			}
		}

		private SqlContainer BuildFromClause(Dictionary<string, SqlField> tables, IEnumerable<System.Tuple<string, SqlField, string, SqlField, bool>> joins)
		{
			// This method looks like terrible, but isn't that much. Basically, we pack all tables
			// and joins within an instance of SqlContainer. The only subtelty is that we must use
			// a precise order for them.
			//
			// The problem is that within an FROM clause, you can only reference tables that have
			// been previously defined. Imagine that you have tables t1, t2 and t3, and want a
			// join between t1 and t2 and a join between t2 and t3. You can't do this:
			// ... FROM t1 INNER JOIN t3 ON t3.id = t2.id INNER JOIN t2 on t2.id = t1.id ...
			// The problem is that you refernce a table in a join that is not yet defined. You have
			// to do this:
			// ... FROM t1 INNER JOIN t2 ON t2.id = t1.id INNER JOIN t3 on t3.id = t2.id ...
			//
			// Therefore, we must order the tables and the join between them, in such a way that a
			// JOIN clause only references tables that have previously been defined.

			// Builds a dictionnary that maps each table that is in a right part of a join to the
			// tables that appear in the left part of its joins.
			var aliasesToJoins = joins
				.GroupBy (j => j.Item3)
				.ToDictionary
				(
					g => g.Key,
					g => g.ToList ()
				);

			// Builds a dictionary that contains all the dependencies between the tables. Each
			// entry within this dictionary means: the table in the key has a dependence on each
			// table in its value, and must appear after them in the final ordering. Then we use an
			// helper class that solves this ordering problem by using a well known algorithm and
			// gives us back the result.
			var dependencies = aliasesToJoins
				.ToDictionary
				(
					x => x.Key,
					x => (ISet<string>) new HashSet<string> (x.Value.Select (j => j.Item1))
				);

			foreach (var tableAlias in tables.Keys)
			{
				if (!dependencies.ContainsKey (tableAlias))
				{
					dependencies[tableAlias] = new HashSet<string> ();
				}
			}

			var ordering = TopologicalSort.GetOrdering (dependencies);

			// Now we put the root table (i.e the single table that has no dependency to other
			// tables) at the start of the FROM clause.

			var mainTableAlias = ordering[0];
			var sqlMainTable = tables[mainTableAlias];

			// Now we build the join between the other tables, by iterating on all these other
			// tables. At each step, we know that the JOIN condition will reference only tables
			// that have previously been defined, thanks to the ordering done above.

			var sqlJoins = new List<SqlJoin> ();

			foreach (var secondaryTableAlias in ordering.Skip (1))
			{
				var joinsWithTable = aliasesToJoins[secondaryTableAlias];

				var sqlJoinCode = joinsWithTable.Any (j => j.Item5)
					? SqlJoinCode.Inner
					: SqlJoinCode.OuterLeft;

				var sqlSecondaryTable = tables[secondaryTableAlias];

				// It might happen that we want the join to occur between the secondary table and
				// serveral other tables that have previously been defined. This will happen if
				// there are several paths to an entity from the root entity in the graph. So the
				// JOIN condition will contain one expression for each of these tables.

				SqlFunction sqlConditions = null;

				foreach (var join in joinsWithTable)
				{
					var sqlCondition = new SqlFunction
					(
						SqlFunctionCode.CompareEqual,
						join.Item2,
						join.Item4
					);

					if (sqlConditions == null)
					{
						sqlConditions = sqlCondition;
					}
					else
					{
						sqlConditions = new SqlFunction
						(
							SqlFunctionCode.LogicAnd,
							SqlField.CreateFunction (sqlConditions),
							SqlField.CreateFunction (sqlCondition)
						);
					}
				}

				var sqlJoin = new SqlJoin (sqlJoinCode, sqlSecondaryTable, sqlConditions);

				sqlJoins.Add (sqlJoin);
			}

			return SqlContainer.CreateSqlTables (sqlMainTable).PlusSqlJoins (sqlJoins.ToArray ());
		}

		private IEnumerable<SqlFunction> BuildWhere(SqlFieldBuilder builder, Request request, IEnumerable<AbstractEntity> nonPersistentEntities)
		{
			// This function builds the standard WHERE clause used by most SQL queries in this
			// class.
			//
			// This clause is made of two parts:
			// - The conditions that are expressed in the fields of the entities that are part of
			//   the example.
			// - The conditions that are expressed by the constraints that are in the Request
			//   object.

			var conditions = this.BuildConditions (builder, nonPersistentEntities);
			var constraints = this.BuildConstraints (builder, request);

			return conditions.Concat (constraints);
		}

		private IEnumerable<SqlFunction> BuildConstraints(SqlFieldBuilder builder, Request request)
		{
			// This method here is really simple, as the building of the condition that are
			// expressed by the constraints is delegated to the DataExpression instances.

			return from condition in request.Conditions
				   select condition.CreateSqlCondition (builder);
		}

		private IEnumerable<SqlFunction> BuildConditions(SqlFieldBuilder builder, IEnumerable<AbstractEntity> entities)
		{
			// We simply iterates over all the fields of all the entities whose value is defined
			// and build the corresponding conditions for each of them.
			//
			// We have three kinds of conditions:
			// - The field is a value field. Then we generate an equal condition between the
			//   column of the value vield and the constant stored in the entity.
			// - The field is a reference field, and the target of the reference is persistent. For
			//   these, we generate an equel condition between the column of the reference field
			//   and the entity id of the target stored in the entity.
			// - The field is a collection field. For each target of the collection that is
			//   persistent, we add a condition on the target column of the relation table between
			//   the two entities.

			var conditions = from entity in entities
							 let leafEntityTypeId = entity.GetEntityStructuredTypeId ()
							 from field in this.TypeEngine.GetFields (leafEntityTypeId)
							 where entity.IsFieldDefined (field.Id)
							 select this.BuildCondition (builder, entity, field);

			return conditions.SelectMany (c => c);
		}

		private IEnumerable<SqlFunction> BuildCondition(SqlFieldBuilder builder, AbstractEntity entity, StructuredTypeField field)
		{
			var conditions = new List<SqlFunction> ();

			switch (field.Relation)
			{
				case FieldRelation.None:
					conditions.Add (this.BuildValueCondition (builder, entity, field));
					break;

				case FieldRelation.Reference:

					var condition = this.BuildReferenceCondition (builder, entity, field);

					if (condition != null)
					{
						conditions.Add (condition);
					}
					break;

				case FieldRelation.Collection:
					conditions.AddRange (this.BuildCollectionCondition (builder, entity, field));
					break;

				default:
					throw new System.InvalidOperationException ();
			}

			return conditions;
		}

		private SqlFunction BuildValueCondition(SqlFieldBuilder builder, AbstractEntity entity, StructuredTypeField field)
		{
			var fieldId = field.CaptionId;

			var sqlFieldColumn = builder.BuildEntityField (entity, fieldId);
			var sqlFieldValue = builder.BuildConstantForField (entity, fieldId);
			var sqlFunctionCode = SqlFunctionCode.CompareEqual;

			return new SqlFunction (sqlFunctionCode, sqlFieldColumn, sqlFieldValue);
		}

		private SqlFunction BuildReferenceCondition(SqlFieldBuilder builder, AbstractEntity source, StructuredTypeField field)
		{
			var fieldId = field.CaptionId;
			var fieldName = fieldId.ToResourceId ();

			var target = source.GetField<AbstractEntity> (fieldName);

			if (!dataContext.IsPersistent (target))
			{
				return null;
			}
			else
			{
				var sourceFieldColumn = builder.BuildEntityField (source, fieldId);
				var targetIdValue = builder.BuildConstantForKey (target);

				var sqlFunctionCode = SqlFunctionCode.CompareEqual;

				return new SqlFunction (sqlFunctionCode, sourceFieldColumn, targetIdValue);
			}
		}

		private IEnumerable<SqlFunction> BuildCollectionCondition(SqlFieldBuilder builder, AbstractEntity entity, StructuredTypeField field)
		{
			var fieldId = field.CaptionId;
			var fieldName = fieldId.ToResourceId ();

			return from target in entity.GetFieldCollection<AbstractEntity> (fieldName)
				   where dataContext.IsPersistent (target)
				   select this.BuildCollectionCondition (builder, entity, field, target);
		}

		private SqlFunction BuildCollectionCondition(SqlFieldBuilder builder, AbstractEntity source, StructuredTypeField field, AbstractEntity target)
		{
			var fieldId = field.CaptionId;

			var leafSourceTypeId = source.GetEntityStructuredTypeId ();
			var localSourceTypeId = this.TypeEngine.GetLocalType (leafSourceTypeId, fieldId).CaptionId;

			var relationTableAlias = builder.AliasManager.GetAlias (source, fieldId, target);
			var relationColumnTargetId = builder.BuildRelationTargetId (relationTableAlias, localSourceTypeId, fieldId);

			var targetIdValue = builder.BuildConstantForKey (target);

			var sqlFunctionCode = SqlFunctionCode.CompareEqual;

			return new SqlFunction (sqlFunctionCode, relationColumnTargetId, targetIdValue);
		}

		private IEnumerable<SqlField> BuildOrderBy(SqlFieldBuilder builder, Request request)
		{
			// This method builds the standard ODRDER BY clause that is used in most of the SQL
			// queries in this class.
			//
			// We simply delegate the construction of this clause to the instances of SortClause
			// contained in the Request object.

			return from sortClause in request.SortClauses
				   select sortClause.CreateSqlField (builder);
		}

		private SqlSelectPredicate GetSqlSelectPredicate(Request request)
		{
			return this.UseDistinct (request)
				? SqlSelectPredicate.Distinct
				: SqlSelectPredicate.All;
		}

		private bool UseDistinct(Request request)
		{
			if (request.Distinct)
			{
				return true;
			}

			// The only queries that must contain a DISTINCT clause are the queries where a
			// collection is involved. If a collection is involved in a WHERE or a ORDER BY clause,
			// there might be duplicate rows in the result if the collection contains more than one
			// element. Therefore, we need to add a DISTINCT clause in the query. We don't need the
			// DISTINCT clause for the other queries.
			//
			// The implementation of this method looks complicated, but it isn't that much. What we
			// do is to walk the graph of entities composed be the example in the Request. As soon
			// as we find an entity with a collection field defined, we return true. If we can walk
			// through all the graph without ever encountering a collection field that is defined,
			// we know that there are no collection at all involved in the query and we can return
			// false.

			// Stores the entities in the graph that we still need to explore. We use a stack as we
			// want to do a depth-first-search, which is more efficiant than a breadth-first-search
			// with regard to memory.
			var todo = new Stack<AbstractEntity> ();

			// Stores the entities that have been explored, so we don't explore them again if the
			// appear more than once in the graph.
			var done = new HashSet<AbstractEntity> ();

			todo.Push (request.RootEntity);
			done.Add (request.RootEntity);

			while (todo.Count > 0)
			{
				var entity = todo.Pop ();

				// If the current entity is persistent, we do not take it into account, and we
				// don't explore the graph further down. This is because the persistent entities
				// within the example are considered by reference and not by value. Their tables
				// are never part of the SQL query, we only use their id in the WHERE part.
				if (this.dataContext.IsPersistent (entity))
				{
					continue;
				}

				// If we encounter a entity with a collection field defined, we must use a DISTINCT
				// clause. So we return true.
				if (EntityHelper.HasCollectionFieldDefined (this.TypeEngine, entity))
				{
					return true;
				}

				// Walk further down the graph by exploring the children of this entity.
				foreach (var child in EntityHelper.GetChildren (this.TypeEngine, entity))
				{
					if (!done.Contains (child))
					{
						todo.Push (child);
						done.Add (child);
					}
				}
			}

			// We have walked all the graph without ever encountering a defined collection field.
			// There are no collection involved in the query, so we don't need to use DISTINCT.
			return false;
		}

		private object ExtractValue(INamedType type, DbColumn dbColumn, object value)
		{
			var dbTypeDef = dbColumn.Type;
			var dbRawType = dbTypeDef.RawType;
			var dbSimpleType = dbTypeDef.SimpleType;
			var dbNumDef = dbTypeDef.NumDef;

			return this.DataConverter.FromDatabaseToCresusValue (type, dbRawType, dbSimpleType, dbNumDef, value);
		}

		private DbKey ExtractKey(object value)
		{
			return new DbKey (new DbId ((long) value));
		}

		private Druid ExtractDruid(object value)
		{
			return Druid.FromLong ((long) value);
		}

		private long ExtractLong(object value)
		{
			return (long) value;
		}

		private SqlFieldBuilder GetBuilder()
		{
			return new SqlFieldBuilder (this, this.dataContext);
		}

		private DbTransaction StartTransaction()
		{
			var mode = DbTransactionMode.ReadOnly;

			return this.DbInfrastructure.InheritOrBeginTransaction (mode);
		}

		private int GetInteger(SqlSelect sqlSelect, DbTransaction dbTransaction)
		{
			dbTransaction.SqlBuilder.SelectData (sqlSelect);

			return (int) this.DbInfrastructure.ExecuteScalar (dbTransaction);
		}

		private int? GetNullableInteger(SqlSelect sqlSelect, DbTransaction dbTransaction)
		{
			dbTransaction.SqlBuilder.SelectData (sqlSelect);

			var result = this.DbInfrastructure.ExecuteScalar (dbTransaction);

			if (result == null || result == System.DBNull.Value)
			{
				return null;
			}
			else
			{
				return (int) result;
			}
		}

		
		private readonly DataContext			dataContext;
	}
}
