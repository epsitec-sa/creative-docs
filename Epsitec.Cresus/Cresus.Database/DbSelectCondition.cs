//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Database.Collections;


namespace Epsitec.Cresus.Database
{
	
	
	/// <summary>
	/// The <c>DbSelectCondition</c> class represents a condition for the
	/// WHERE clause.
	/// </summary>
	public sealed class DbSelectCondition
	{
		
		
		/// <summary>
		/// Initializes a new instance of the <see cref="DbSelectCondition"/> class.
		/// </summary>
		public DbSelectCondition(): this (DbSelectRevision.All)
		{
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="DbSelectCondition"/> class.
		/// </summary>
		/// <param name="revision">The expected revision.</param>
		public DbSelectCondition(DbSelectRevision revision) : base()
		{
			this.Revision = revision;
		}


		/// <summary>
		/// Gets or sets the expected revision.
		/// </summary>
		/// <value>The revision.</value>
		public DbSelectRevision Revision
		{
			get;
			set;
		}


		public DbAbstractCondition Condition
		{
			get;
			set;
		}


		internal SqlField CreateConditions()
		{
			return this.CreateConditions (null, null);
		}


		internal SqlField CreateConditions(DbTable mainTable, string mainTableAlias)
		{
			SqlField conditions = this.CreateConditionSqlField ();
			SqlField revision = this.CreateRevisionSqlField (mainTable, mainTableAlias);

			if (conditions != null && revision != null)
			{
				SqlFunction function = new SqlFunction (SqlFunctionCode.LogicAnd, conditions, revision);

				return SqlField.CreateFunction (function);
			}
			else
			{
				return conditions ?? revision ?? null;
			}
		}


		private SqlField CreateConditionSqlField()
		{
			return (this.Condition == null) ? null : this.Condition.CreateSqlField ();
		}


		private SqlField CreateRevisionSqlField(DbTable mainTable, string mainTableAlias)
		{
			DbSimpleCondition revisionCondition = null;
			
			if (mainTable != null && mainTableAlias != null)
			{
				switch (this.Revision)
				{
					case DbSelectRevision.LiveAll:
						//	Select all live revisions of the rows: live (0), copied (1)
						//	and archive copy (2) are all < deleted (3).

						revisionCondition = new DbSimpleCondition (new DbTableColumn (mainTableAlias, mainTable.Columns[Tags.ColumnStatus]),
														   DbSimpleConditionOperator.LessThan,
														   DbKey.ConvertToIntStatus (DbRowStatus.Deleted), DbRawType.Int16);
						break;

					case DbSelectRevision.LiveActive:
						//	Select only the active revisions of the rows: live (0) and
						//	copied (1) both describe active rows and are < archive copy (2).

						revisionCondition = new DbSimpleCondition (new DbTableColumn (mainTableAlias, mainTable.Columns[Tags.ColumnStatus]),
														   DbSimpleConditionOperator.LessThan,
														   DbKey.ConvertToIntStatus (DbRowStatus.ArchiveCopy), DbRawType.Int16);
						break;

					case DbSelectRevision.All:
						//	Select all revisions of the rows; there is no need to add an
						//	additional condition for this !

						break;

					default:
						throw new System.NotSupportedException (string.Format ("DbSelectRevision.{0} not supported", this.Revision));
				}
			}

			return (revisionCondition == null) ? null : revisionCondition.CreateSqlField ();
		}


	}


}
